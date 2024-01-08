
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RenderAsAttribute : FragmentAttribute
    {
        #region Public constructors region
        /// <inheritdoc/>
        public RenderAsAttribute(Type componentType)
            : base(componentType)
        { }

        /// <inheritdoc/>
        public RenderAsAttribute(string fragmentMember)
            : base(fragmentMember)
        { }

        /// <inheritdoc/>
        public RenderAsAttribute(Type containerType, string fragmentMember)
            : base(containerType, fragmentMember)
        { }

        #endregion
    }
}
