using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlus.Web.UI
{
    public class ActionAccessor : AccessorBase
    {
        private struct ArgumentInfo
        {
            #region Public constructors region

            public ArgumentInfo(Type type, bool hasDefaultValue, object? defaultValue)
            {
                Type = type;
                HasDefaultValue = hasDefaultValue;
                DefaultValue = defaultValue;
            }

            #endregion

            #region Public fields region

            public Type Type { get; }

            public bool HasDefaultValue { get; }

            public object? DefaultValue { get; }

            #endregion
        }

        private class ActionInvoker
        {
            #region Private fields region

            private readonly ActionAccessor _accessor;
            private bool _isHandlerInitialized = false;
            private Func<object, IActionArgumentValueProvider, object?>? _invokeHandler;
            private Func<object, IActionArgumentValueProvider, CancellationToken, Task<object?>>? _invokeHandlerAsync;

            #endregion

            #region Private methods region

            private static async Task<object?> ConvertionToTaskObject(Task action) { await action; return null; }
            private static async Task<object?> ConvertionToTaskObject<T>(Task<T> func) => await func;
            private static async Task<object?> ConvertionToTaskObject(ValueTask action) { await action; return null; }
            private static async Task<object?> ConvertionToTaskObject<T>(ValueTask<T> func) => await func;

            private static T? GetArgumentValue<T>(int index, bool hasDefaultValue, T? defaultValue, IActionArgumentValueProvider valueProvider)
            {
                if (valueProvider.TryGetValue(index, typeof(T), out var value)) return (T?)value;
                if (hasDefaultValue) return defaultValue;
                throw new ArgumentException($"Cannot resolve value for argument[{index}].");
            }

            private void InitializeHandler()
            {
                if (_isHandlerInitialized) return;

                // Argument: object instance
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                // Argument: IActionArgumentValueProvider argumentValueProvider
                var argumentValueProviderParam = Expression.Parameter(typeof(IActionArgumentValueProvider), "argumentValueProvider");
                // Variable: <_accessor.Type.BaseTyp> instanceTyped
                var instanceVar = Expression.Variable(_accessor.Type.BaseType, "instanceTyped");

                if (_accessor._asyncType == ASYNC_TYPE_NONE)
                {
                    // Variable: object return
                    var returnExp = Expression.Variable(typeof(object), "return");

                    var isVoid = _accessor._returnType == typeof(void);
                    var isObjectReturning = _accessor._returnType == typeof(object);
                    var expressions = new List<Expression>
                    {
                        // instanceTyped = (object)instance
                        Expression.Assign(instanceVar, Expression.Convert(instanceParam, _accessor.Type.BaseType))
                    };

                    var callArguments = GetCallArguments(null);
                    var callExp = callArguments is null ?
                        Expression.Call(instanceVar, _accessor._methodInfo) :
                        Expression.Call(instanceVar, _accessor._methodInfo, callArguments);

                    if (isObjectReturning)
                    {
                        // return = instanceTyped.<ActionMethod>({call arguments})
                        expressions.Add(Expression.Assign(returnExp, callExp));
                    }
                    else if (isVoid)
                    {
                        // instanceTyped.<ActionMethod>({call arguments})
                        expressions.Add(callExp);
                        // return = null
                        expressions.Add(Expression.Assign(returnExp, Expression.Constant(null)));
                    }
                    else
                    {
                        // return = (object)instanceTyped.<ActionMethod>({call arguments})
                        expressions.Add(Expression.Assign(returnExp, Expression.Convert(callExp, typeof(object))));
                    }
                    // return
                    expressions.Add(returnExp);

                    var invokeExp = Expression.Lambda<Func<object, IActionArgumentValueProvider, object?>>(
                        Expression.Block(typeof(object), new[] { instanceVar, returnExp }, expressions), instanceParam, argumentValueProviderParam);

                    // Compiling the lambda expression to handler.
                    _invokeHandler = invokeExp.Compile();
                }
                else
                {
                    // Argument: CancellationToken cancellationToken
                    var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    // Variable: object return
                    var returnExp = Expression.Variable(typeof(Task<object?>), "return");

                    var isValueTask = _accessor._asyncType == ASYNC_TYPE_TASKVALUE;
                    var isVoid = _accessor._returnType == typeof(void);
                    var isObjectReturning = _accessor._returnType == typeof(object);

                    var expressions = new List<Expression>
                    {
                        // instanceTyped = (object)instance
                        Expression.Assign(instanceVar, Expression.Convert(instanceParam, _accessor.Type.BaseType))
                    };

                    var callArguments = GetCallArguments(cancellationTokenParam);
                    var callExp = callArguments is null ?
                        Expression.Call(instanceVar, _accessor._methodInfo) :
                        Expression.Call(instanceVar, _accessor._methodInfo, callArguments);

                    if (!isValueTask && isObjectReturning)
                    {
                        // return = instanceTyped.<ActionMethod>({call arguments})
                        expressions.Add(Expression.Assign(returnExp, callExp));
                    }
                    else if (isVoid)
                    {
                        // return = ActionInvoker.ConvertionToTaskObject(instanceTyped.<ActionMethod>({call arguments}))
                        expressions.Add(Expression.Assign(returnExp, Expression.Call(typeof(ActionInvoker), nameof(ConvertionToTaskObject), Array.Empty<Type>(), callExp)));
                    }
                    else
                    {
                        // return = ActionInvoker.ConvertionToTaskObject<T>(instanceTyped.<ActionMethod>({call arguments}))
                        expressions.Add(Expression.Assign(returnExp, Expression.Call(typeof(ActionInvoker), nameof(ConvertionToTaskObject), new[] { _accessor._returnType }, callExp)));
                    }
                    // return
                    expressions.Add(returnExp);

                    var invokeExp = Expression.Lambda<Func<object, IActionArgumentValueProvider, CancellationToken, Task<object?>>>(
                        Expression.Block(typeof(Task<object?>), new[] { instanceVar, returnExp }, expressions), instanceParam, argumentValueProviderParam, cancellationTokenParam);

                    // Compiling the lambda expression to handler.
                    _invokeHandlerAsync = invokeExp.Compile();
                }

                _isHandlerInitialized = true;

                Expression[]? GetCallArguments(Expression? cancellationToken)
                {
                    var isAsync = cancellationToken != null;
                    var cancellationTokenType = typeof(CancellationToken);
                    Expression[]? callArguments = null;
                    if (_accessor._arguments.Length != 0)
                    {
                        callArguments = new Expression[_accessor._arguments.Length];
                        for (int i = 0; i < callArguments.Length; i++)
                        {
                            var argument = _accessor._arguments[i];

                            if (isAsync && argument.Type == cancellationTokenType)
                            {
                                // Call Argument: cancellationToken
                                callArguments[i] = cancellationToken!;
                            }
                            else
                            {
                                // Call Argument: GetArgumentValue<argument.Type>(i, argument.HasDefaultValue, argument.HasDefaultValue ? argument.DefaultValue : default(argument.Type), argumentValueProviderParam)
                                callArguments[i] = Expression.Call(typeof(ActionInvoker), nameof(GetArgumentValue), new[] { argument.Type },
                                    Expression.Constant(i), Expression.Constant(argument.HasDefaultValue), argument.HasDefaultValue ? Expression.Constant(argument.DefaultValue) : Expression.Default(argument.Type), argumentValueProviderParam);
                            }
                        }
                    }
                    return callArguments;
                }
            }

            #endregion

            #region Public constructors region

            public ActionInvoker(ActionAccessor accessor)
                : base()
            {
                _accessor = accessor;
            }

            #endregion

            #region Public methods region

            public object? Invoke(object target, IActionArgumentValueProvider argumentValueProvider)
            {
                InitializeHandler();
                if (_invokeHandler != null) return _invokeHandler(target, argumentValueProvider);
                if (_invokeHandlerAsync != null) return _invokeHandlerAsync(target, argumentValueProvider, CancellationToken.None).GetAwaiter().GetResult();
                throw new InvalidOperationException("Invoke handler is not initialized.");
            }

            public Task<object?> InvokeAsync(object target, IActionArgumentValueProvider argumentValueProvider, CancellationToken cancellationToken)
            {
                InitializeHandler();
                if (_invokeHandlerAsync != null) return _invokeHandlerAsync(target, argumentValueProvider, cancellationToken);
                if (_invokeHandler != null) return Task.FromResult(_invokeHandler(target, argumentValueProvider));
                return Task.FromException<object?>(new InvalidOperationException("Invoke handler is not initialized."));
            }

            #endregion
        }

        #region Private fields region

        private const int ASYNC_TYPE_NONE = 0;
        private const int ASYNC_TYPE_TASK = 1;
        private const int ASYNC_TYPE_TASKVALUE = 2;

        private readonly MethodInfo _methodInfo;
        private readonly int _asyncType;
        private readonly Type _returnType;
        private readonly ArgumentInfo[] _arguments;
        private ActionInvoker? _invoker;

        #endregion

        #region Private methods region

        private static void GetMethodDetails(MethodInfo methodInfo, out ArgumentInfo[] argumentInfos, out Type returnType, out int asyncType)
        {
            var parameters = methodInfo.GetParameters();
            argumentInfos = parameters.Length == 0 ? Array.Empty<ArgumentInfo>() : Array.ConvertAll(parameters, p => new ArgumentInfo(p.ParameterType, p.HasDefaultValue, p.HasDefaultValue ? p.DefaultValue : null));

            if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(ValueTask))
            {
                returnType = typeof(void);
                asyncType = methodInfo.ReturnType == typeof(Task) ? ASYNC_TYPE_TASK : ASYNC_TYPE_TASKVALUE;
            }
            else if (methodInfo.ReturnType.IsGenericType)
            {
                var typeDefinition = methodInfo.ReturnType.GetGenericTypeDefinition();
                if (typeDefinition == typeof(Task<>) || typeDefinition == typeof(ValueTask<>))
                {
                    returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                    asyncType = typeDefinition == typeof(Task<>) ? ASYNC_TYPE_TASK : ASYNC_TYPE_TASKVALUE;
                }
                else
                {
                    returnType = methodInfo.ReturnType;
                    asyncType = ASYNC_TYPE_NONE;
                }
            }
            else
            {
                returnType = methodInfo.ReturnType;
                asyncType = ASYNC_TYPE_NONE;
            }
        }

        private void Initialize(MethodInfo methodInfo)
        {
            foreach (Attribute attr in methodInfo.GetCustomAttributes(true))
            {
                switch (attr)
                {
                    case ActionAttribute actionAttr:
                        ActionAttribute ??= actionAttr;
                        break;

                    case DisplayAttribute displayAttr:
                        DisplayAttribute ??= displayAttr;
                        break;

                    case DescriptionAttribute descriptionAttr:
                        DescriptionAttribute ??= descriptionAttr;
                        break;

                    case RenderAsAttribute renderAsAttr:
                        RenderAsAttribute ??= renderAsAttr;
                        break;

                    case ElementAttribute elementAttr:
                        ElementAttribute ??= elementAttr;
                        break;
                }
            }

            // DisplayAttribute
            DisplayAttribute ??= new();
            DisplayAttribute.Name ??= methodInfo.Name;
        }

        private ActionInvoker EnsureInvoker() => _invoker ??= new ActionInvoker(this);

        #endregion

        #region Internal constructors region

        internal ActionAccessor(TypeAccessor type, MethodInfo methodInfo)
            : base(methodInfo.Name)
        {
            Type = type;
            GetMethodDetails(_methodInfo = methodInfo, out _arguments, out _returnType, out _asyncType);
            Initialize(methodInfo);
        }

        #endregion

        #region Public methods region

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="target">The action target.</param>
        /// <param name="args">The action arguments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<object?> InvokeAsync(object target, IActionArgumentValueProvider argumentValueProvider, CancellationToken cancellationToken)
        {
            return EnsureInvoker().InvokeAsync(target, argumentValueProvider, cancellationToken);
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="target">The action target.</param>
        /// <param name="args">The action arguments.</param>
        public object? Invoke(object target, IActionArgumentValueProvider argumentValueProvider)
        {
            return EnsureInvoker().Invoke(target, argumentValueProvider);
        }

        public bool? ActionIsDefault => ActionAttribute?.IsDefault;

        public bool? ActionIsCancel => ActionAttribute?.IsCancel;

        public string? ActionNavigateTo => ActionAttribute?.NavigateTo;

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets type accessor.
        /// </summary>
        public TypeAccessor Type { get; }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicating the method is returning task.
        /// </summary>
        public bool IsAsync => _asyncType != ASYNC_TYPE_NONE;

        /// <summary>
        /// Gets type of return value of the action.
        /// </summary>
        public Type ReturnType => _returnType;

        /// <summary>
        /// Gets argument types of the action.
        /// </summary>
        public IEnumerable<Type> ArgumentTypes => _arguments.Select(a => a.Type);

        #endregion

        #region Protected properties region

        protected ActionAttribute? ActionAttribute { get => (ActionAttribute?)GetAttribute(CustomAttributeStartIndex); set => SetAttribute(CustomAttributeStartIndex, value!); }

        #endregion
    }
}
