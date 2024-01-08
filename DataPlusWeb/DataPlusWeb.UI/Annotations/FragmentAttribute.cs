using Microsoft.AspNetCore.Components;
using DataPlus.Web.UI;
using DataPlus.Web.UI.Components;
using System.Linq.Expressions;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    public abstract class FragmentAttribute : Attribute
    {
        #region Private fields region

        private FragmentInvoker? _invoker;

        #endregion

        #region Public constructors region

        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentAttribute"/> class with component type.
        /// </summary>
        /// <param name="componentType">The type of the component.</param>
        public FragmentAttribute(Type componentType)
            : base()
        {
            Type = componentType ?? throw new ArgumentNullException(nameof(componentType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentAttribute"/> class with component type.
        /// </summary>
        /// <param name="fragmentMember">The name of the fragment which is name of the member.</param>
        public FragmentAttribute(string fragmentMember)
            : base()
        {
            Member = fragmentMember ?? throw new ArgumentNullException(nameof(fragmentMember));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentAttribute"/> class with component type.
        /// </summary>
        /// <param name="containerType">The type of the class which containes specified <paramref name="fragmentMember"/>.</param>
        /// <param name="fragmentMember">The name of the fragment which is name of the member.</param>
        public FragmentAttribute(Type containerType, string fragmentMember)
            : base()
        {
            Type = containerType ?? throw new ArgumentNullException(nameof(containerType));
            Member = fragmentMember ?? throw new ArgumentNullException(nameof(fragmentMember));
        }

        #endregion

        #region Internal methods region

        internal FragmentInvoker GetFragment(AccessorBase accessor)
        {
            if (_invoker == null)
            {
                RenderFragment<object>? fragment = null;
                var valueType = typeof(object);

                var type = Type;
                if (type is null)
                {
                    type = accessor switch
                    {
                        MemberAccessor memberAccessor => memberAccessor.Type.BaseType.IsNested ? memberAccessor.Type.BaseType.DeclaringType : memberAccessor.Type.BaseType,
                        TypeAccessor typeAccessor => typeAccessor.BaseType.IsNested ? typeAccessor.BaseType.DeclaringType : typeAccessor.BaseType,
                        _ => null
                    };
                }

                if (type is null)
                    fragment = _ => throw new InvalidOperationException(string.Format(SR.Type_CouldNotRecognizeFragmentType, Member));
                else
                {
                    if (Member is null)
                    {
                        if (!type.IsAssignableTo(typeof(ComponentBase)))
                            fragment = _ => throw new InvalidOperationException(string.Format(SR.Type_ShouldBeDerivedFrom, type.FullName, typeof(ComponentBase).FullName));
                        else
                        {
                            var memberAccessorPropertyInfo = type.GetParameterProperty<AccessorBase>();
                            var instancePropertyInfo = type.GetParameterProperty<object>(propertyName: nameof(FragmentComponent.Instance));

                            fragment = instance => RenderComponent(type, (memberAccessorPropertyInfo, accessor), (instancePropertyInfo, instance));
                        }
                    }
                    else
                    {
                        const BindingFlags fragmentMemberBindingFlag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                        // Get fragment from field.
                        var fieldInfo = type.GetField(Member, fragmentMemberBindingFlag);
                        if (fieldInfo != null)
                            fragment = CreateRenderFragment(Expression.Field(null, fieldInfo), ref valueType);

                        if (fragment is null)
                        {
                            // Gets fragment from property.
                            var propertyInfo = type.GetProperty(Member, fragmentMemberBindingFlag);
                            if (propertyInfo != null)
                                fragment = CreateRenderFragment(Expression.Property(null, propertyInfo), ref valueType);
                        }

                        fragment ??= _ => throw new InvalidOperationException(string.Format(SR.Type_MissingFragmentMember, Member, Type));
                    }
                }

                _invoker = new FragmentInvoker(fragment, valueType, accessor);
            }
            return _invoker;

            static RenderFragment<object>? CreateRenderFragment(Expression expression, ref Type valueType)
            {
                var type = expression.Type;
                if (type == typeof(RenderFragment))
                {
                    // Creates fragment factory to read from field.
                    var fragmentFactory = Expression.Lambda<Func<RenderFragment?>>(expression).Compile();

                    return _ => fragmentFactory() ?? Helpers.EmptyRenderFragment;
                }
                else if (type == typeof(RenderFragment<object>))
                {
                    // Creates fragment factory to expression.
                    var fragmentFactory = Expression.Lambda<Func<RenderFragment<object>?>>(expression).Compile();

                    return instance => fragmentFactory()?.Invoke(instance) ?? Helpers.EmptyRenderFragment;
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RenderFragment<>))
                {
                    valueType = type.GenericTypeArguments[0];

                    // Creates fragment factory to expression.
                    var instanceParam = Expression.Parameter(typeof(object), "instance");
                    var invokeExp = Expression.Invoke(expression, Expression.Convert(instanceParam, valueType));
                    var fragmentFactory = Expression.Lambda<Func<object, RenderFragment?>>(invokeExp, instanceParam).Compile();

                    return instance => fragmentFactory(instance) ?? Helpers.EmptyRenderFragment;
                }

                return null;
            }

            static RenderFragment RenderComponent(Type componentType, params (PropertyInfo?, object?)[] attributes)
            {
                return b =>
                {
                    b.OpenComponent(0, componentType);
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        var (property, value) = attributes[i];
                        if (property != null)
                            b.AddAttribute(i + 1, property.Name, value);
                    }
                    b.CloseComponent();
                };
            }
        }

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or set type of the component.
        /// </summary>
        public Type? Type { get; }

        /// <summary>
        /// Gets or sets member name in compaonent type.
        /// </summary>
        public string? Member { get; }

        #endregion
    }
}
