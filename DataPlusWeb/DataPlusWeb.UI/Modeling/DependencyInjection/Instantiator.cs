using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DataPlus.Web.DependencyInjection;

/// <summary>
/// Represent class to instantaite object by specified types, which supportes DI of service and arguments. 
/// </summary>
public class Instantiator
{
    #region Private fields region

    private const BindingFlags _propertyBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly InstantiatorObjectFactoryCache _internalCache = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly InstantiatorObjectFactoryCache _cache;

    #endregion

    #region Private methods region

    private static Type[] GetArgumentTypes(object[] arguments) => Array.ConvertAll(arguments, a => a?.GetType() ?? typeof(object));

    private static object CallObjectFactory(ObjectFactory objectFactory, IServiceProvider serviceProvider, object[] arguments)
        => objectFactory(serviceProvider, arguments);

    private Type[] GetAtributeTypes()
    {
        if (InjectAttribute != null && ArgumentAttribute != null) return new[] { InjectAttribute, ArgumentAttribute };
        if (InjectAttribute != null) return new[] { InjectAttribute };
        if (ArgumentAttribute != null) return new[] { ArgumentAttribute };
        return Array.Empty<Type>();
    }

    private (ObjectFactory, IList<PropertySetter>?) CreateInitializer(Type instanceType)
    {
        var argumentTypes = ArgumentTypes ?? Array.Empty<Type>();
        var createFactory = CreateInstanceFactory(instanceType, argumentTypes);

        var attributeTypes = GetAtributeTypes();
        if (attributeTypes.Length == 0) return (createFactory, null);

        var properties = instanceType.GetMembers<PropertyInfo>(_propertyBindingFlags, attributeTypes).ToArray();
        if (properties.Length == 0) return (createFactory, null);

        #region Create expression and compile object factory

        // Gets calling methods.
        var callObjectFactoryMethod = typeof(Instantiator).GetMethod(nameof(CallObjectFactory), BindingFlags.Static | BindingFlags.NonPublic)!;
        var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService), BindingFlags.Instance | BindingFlags.Public)!;

        // Parameter: IServiceProvider serviceProvider
        var serviceProviderParam = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

        // Parameter: IServiceProvider arguments
        var argumentsParam = Expression.Parameter(typeof(object[]), "arguments");

        // Variable: <InstanceType> instance
        var instanceVar = Expression.Variable(instanceType, "instance");

        var bodyExpressions = new List<Expression>()
        {
            // Assign: instance = CallObjectFactory(createFactory, serviceProvider, arguments)
            Expression.Assign(instanceVar, Expression.Convert(Expression.Call(callObjectFactoryMethod, Expression.Constant(createFactory), serviceProviderParam, argumentsParam), instanceType))
        };

        List<PropertySetter>? propertySetters = null;
        List<int> assignedIndices = new();
        foreach (var (property, attributeType) in properties)
        {
            if (attributeType != null)
            {
                if (attributeType == InjectAttribute)
                {
                    // Assign: instance.<Property> = (<PropertyType>)serviceProvider.GetService(typeof(<PropertyType>))
                    bodyExpressions.Add(
                        Expression.Assign(Expression.Property(instanceVar, property),
                        Expression.Convert(Expression.Call(serviceProviderParam, getServiceMethod, Expression.Constant(property.PropertyType)), property.PropertyType)));
                }
                else if (attributeType == ArgumentAttribute)
                {
                    if (TryFindArgumentIndex(property.PropertyType, out var argumentIndex))
                    {
                        // Assign: instance.<Property> = (<PropertyType>)arguments[<argumentIndex>]
                        bodyExpressions.Add(
                            Expression.Assign(Expression.Property(instanceVar, property),
                            Expression.Convert(Expression.ArrayAccess(argumentsParam, Expression.Constant(argumentIndex)), property.PropertyType)));
                    }
                    else
                    {
                        (propertySetters ??= new()).Add(new PropertySetter(instanceType, property));
                    }

                    bool TryFindArgumentIndex(Type propertyType, out int argumentIndex)
                    {
                        for (int i = 0; i < argumentTypes.Length; i++)
                        {
                            var argumentType = argumentTypes[i];
                            if (!assignedIndices.Contains(i) && propertyType.IsAssignableFrom(argumentType))
                            {
                                assignedIndices.Add(i);
                                argumentIndex = i;
                                return true;
                            }
                        }
                        argumentIndex = -1;
                        return false;
                    }
                }
            }
        }
        // Return: (object)instance
        bodyExpressions.Add(Expression.Convert(instanceVar, typeof(object)));

        // Creates lambda expression and compile object factory.
        createFactory = Expression.Lambda<ObjectFactory>(Expression.Block(typeof(object), new[] { instanceVar }, bodyExpressions), serviceProviderParam, argumentsParam).Compile();

        #endregion

        return (createFactory, propertySetters);
    }

    private object CreateInstance(Type instanceType, object?[] arguments)
    {
        var (factory, setters) = _cache.GetOrAdd(instanceType, InjectAttribute, ArgumentAttribute, t => CreateInitializer(t));
        var instance = factory(_serviceProvider, arguments);

        if (setters != null && arguments?.Length > 0)
        {
            List<int> assignedIndices = new();
            foreach (var setter in setters)
            {
                if (TryFindArgument(setter, out var argument))
                    setter.SetValue(instance, argument);
            }

            bool TryFindArgument(PropertySetter setter, out object? argument)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (!assignedIndices.Contains(i) && setter.CanSet(argument = arguments[i]))
                    {
                        assignedIndices.Add(i);
                        return true;
                    }
                }
                argument = null;
                return false;
            }
        }

        return instance;

    }

    #endregion

    #region Public constructors region

    /// <summary>
    /// Initializes a new instance of the <see cref="Instantiator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Instantiator(IServiceProvider serviceProvider)
        : base()
    {
        _serviceProvider = serviceProvider;
        _cache = serviceProvider.GetService<InstantiatorObjectFactoryCache>() ?? _internalCache;
    }

    #endregion

    #region Public methods region

    /// <summary>
    /// Initializes <see cref="Instantiator"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="factoryCache">The factory cache.</param>
    /// <param name="injectAttributeType">The type of inject attribute.</param>
    /// <param name="argumentAttributeType">The type of argument attribute.</param>
    /// <param name="argumentTypes">The type of arguments.</param>
    public static Instantiator Create(IServiceProvider serviceProvider, Type injectAttributeType, Type argumentAttributeType, params Type[] argumentTypes)
        => new(serviceProvider) { InjectAttribute = injectAttributeType, ArgumentAttribute = argumentAttributeType, ArgumentTypes = argumentTypes?.Length > 0 ? argumentTypes : null };

    /// <summary>
    /// Initializes <see cref="Instantiator"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="injectAttributeType">The type of inject attribute.</param>
    /// <param name="argumentTypes">The type of arguments.</param>
    public static Instantiator Create(IServiceProvider serviceProvider, Type injectAttributeType, params Type[] argumentTypes)
        => Create(serviceProvider, injectAttributeType, typeof(ArgumentAttribute), argumentTypes);

    /// <summary>
    /// Instantiate the <paramref name="instanceType"/> type.
    /// </summary>
    /// <param name="instanceType">The type to instantiate.</param>
    public object Instantiate(Type instanceType) => CreateInstance(instanceType, Array.Empty<object>());

    /// <summary>
    /// Instantiate the <paramref name="instanceType"/> type with special <paramref name="arguments"/> arguments.
    /// </summary>
    /// <param name="instanceType">The type to instantiate.</param>
    /// <param name="arguments">The additional arguments.</param>
    public object Instantiate(Type instanceType, params object?[] arguments) => CreateInstance(instanceType, arguments);

    /// <summary>
    /// Instantiate the <typeparam name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    public T Instantiate<T>() => (T)CreateInstance(typeof(T), Array.Empty<object>());

    /// <summary>
    /// Instantiate the <typeparam name="T"/> type with special <paramref name="arguments"/> arguments.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    /// <param name="arguments">The additional arguments.</param>
    public T Instantiate<T>(params object?[] arguments) => (T)CreateInstance(typeof(T), arguments);

    #endregion

    #region Protected methods region

    protected virtual ObjectFactory CreateInstanceFactory(Type instanceType, Type[] argumentTypes) => ActivatorUtilities.CreateFactory(instanceType, argumentTypes);

    #endregion

    #region Protected properties region

    protected Type? InjectAttribute { get; set; }

    protected Type? ArgumentAttribute { get; set; }

    protected Type[]? ArgumentTypes { get; set; }

    #endregion
}

/// <summary>
/// Represent class to store compiled object factories.
/// </summary>
public sealed class InstantiatorObjectFactoryCache
{
    #region Private fields region

    /// <remarks>Key is contains of instance ytpe, inject attribute type and argument attribute type.</remarks>
    private readonly ConcurrentDictionary<(Type InstanceType, Type?, Type?), (ObjectFactory Factory, IList<PropertySetter>? Setters)> _cached = new();

    #endregion

    #region Internal methods region

    internal (ObjectFactory Factory, IList<PropertySetter>? Setters) GetOrAdd(Type instanceType, Type? injectAttrType, Type? argumentAttrType, Func<Type, (ObjectFactory Factory, IList<PropertySetter>? Setters)> creator)
    {
        return _cached.GetOrAdd((instanceType, injectAttrType, argumentAttrType), k => creator(k.InstanceType));
    }

    #endregion
}

/// <summary>
/// Marks the constructor to be used when instantiate type using <see cref="Instantiator"/>.
/// </summary>
public sealed class InstantiateConstructorAttribute : ActivatorUtilitiesConstructorAttribute { }

/// <summary>
/// Injects passed arguments using <see cref="Instantiator"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
public sealed class ArgumentAttribute : Attribute { }

/// <inheritdoc/>
public class Instantiator<TInjectAttribute> : Instantiator
    where TInjectAttribute : Attribute
{
    #region Public constructors region

    /// <inheritdoc/>
    public Instantiator(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        InjectAttribute = typeof(TInjectAttribute);
        ArgumentAttribute = typeof(ArgumentAttribute);
    }

    #endregion
}

/// <inheritdoc/>
public class Instantiator<TInjectAttribute, TArgumentType> : Instantiator<TInjectAttribute>
    where TInjectAttribute : Attribute
{
    #region Public constructors region

    /// <inheritdoc/>
    public Instantiator(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        ArgumentTypes = new[] { typeof(TArgumentType) };
    }

    #endregion

    #region Public methods region

    /// <summary>
    /// Instantiate the <paramref name="instanceType"/> type with special <paramref name="argument"/> arguments.
    /// </summary>
    /// <param name="instanceType">The type to instantiate.</param>
    /// <param name="argument">The additional argument.</param>
    public object Instantiate(Type instanceType, TArgumentType? argument) => base.Instantiate(instanceType, argument);

    /// <summary>
    /// Instantiate the <typeparam name="T"/> type with special <paramref name="argument"/> arguments.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    /// <param name="argument">The additional argument.</param>
    public T Instantiate<T>(TArgumentType? argument) => base.Instantiate<T>(argument);

    #endregion
}

/// <inheritdoc/>
public class Instantiator<TInjectAttribute, TArgumentType1, TArgumentType2> : Instantiator<TInjectAttribute>
    where TInjectAttribute : Attribute
{
    #region Public constructors region

    /// <inheritdoc/>
    public Instantiator(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        ArgumentTypes = new[] { typeof(TArgumentType1), typeof(TArgumentType2) };
    }

    #endregion

    #region Public methods region

    /// <summary>
    /// Instantiate the <paramref name="instanceType"/> type with special <paramref name="argument1"/>, <paramref name="argument2"/> arguments.
    /// </summary>
    /// <param name="instanceType">The type to instantiate.</param>
    /// <param name="argument1">The additional first argument.</param>
    /// <param name="argument2">The additional second argument.</param>
    public object Instantiate(Type instanceType, TArgumentType1? argument1, TArgumentType2? argument2) => base.Instantiate(instanceType, argument1, argument2);

    /// <summary>
    /// Instantiate the <typeparam name="T"/> type with special <paramref name="argument1"/>, <paramref name="argument2"/> arguments.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    /// <param name="argument1">The additional first argument.</param>
    /// <param name="argument2">The additional second argument.</param>
    public T Instantiate<T>(TArgumentType1? argument1, TArgumentType2? argument2) => base.Instantiate<T>(argument1, argument2);

    #endregion
}

/// <inheritdoc/>
public class Instantiator<TInjectAttribute, TArgumentType1, TArgumentType2, TArgumentType3> : Instantiator<TInjectAttribute>
    where TInjectAttribute : Attribute
{
    #region Public constructors region

    /// <inheritdoc/>
    public Instantiator(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        ArgumentTypes = new[] { typeof(TArgumentType1), typeof(TArgumentType2), typeof(TArgumentType3) };
    }

    #endregion

    #region Public methods region

    /// <summary>
    /// Instantiate the <paramref name="instanceType"/> type with special <paramref name="argument1"/>, <paramref name="argument2"/>, <paramref name="argument3"/> arguments.
    /// </summary>
    /// <param name="instanceType">The type to instantiate.</param>
    /// <param name="argument1">The additional first argument.</param>
    /// <param name="argument2">The additional second argument.</param>
    /// <param name="argument3">The additional third argument.</param>
    public object Instantiate(Type instanceType, TArgumentType1? argument1, TArgumentType2? argument2, TArgumentType2? argument3) => base.Instantiate(instanceType, argument1, argument2, argument3);

    /// <summary>
    /// Instantiate the <typeparam name="T"/> type with special <paramref name="argument1"/>, <paramref name="argument2"/>, <paramref name="argument3"/> arguments.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    /// <param name="argument1">The additional first argument.</param>
    /// <param name="argument2">The additional second argument.</param>
    /// <param name="argument3">The additional third argument.</param>
    public T Instantiate<T>(TArgumentType1? argument1, TArgumentType2? argument2, TArgumentType2? argument3) => base.Instantiate<T>(argument1, argument2, argument3);

    #endregion
}
