
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DescriptionAttribute : FragmentAttribute
    {
        #region Public constructors region

        /// <inheritdoc/>
        public DescriptionAttribute(Type componentType)
            : base(componentType)
        { }

        /// <inheritdoc/>
        public DescriptionAttribute(string fragmentMember)
            : base(fragmentMember)
        { }

        /// <inheritdoc/>
        public DescriptionAttribute(Type containerType, string fragmentMember)
            : base(containerType, fragmentMember)
        { }

        #endregion
    }
}
