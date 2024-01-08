using Microsoft.AspNetCore.Components;
using System;

namespace DataPlus.Web.UI.Components
{
    public class FragmentInvoker
    {
        #region Private fields region

        private readonly RenderFragment<object> _fragment;
        private readonly Type _valueType;
        private readonly AccessorBase? _accessor;

        #endregion

        #region Public constructors region

        public FragmentInvoker(RenderFragment<object> fragment, Type valueType, AccessorBase? accessor)
            : base()
        {
            _fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
            _valueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            _accessor = accessor;
        }

        #endregion

        #region Public methods region

        public RenderFragment Invoke(object? instance)
        {
            if (_valueType == typeof(object) || instance is null)
                return _fragment(instance!);

            var instanceType = instance.GetType();
            if (instanceType == _valueType || _valueType.IsAssignableFrom(instanceType))
                return _fragment(instance);

            if (_accessor is MemberAccessor memberAccessor)
                return _fragment(memberAccessor.GetValue(instance)!);

            return _fragment(instance);
        }

        #endregion
    }
}
