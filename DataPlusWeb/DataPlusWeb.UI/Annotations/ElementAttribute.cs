
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class ElementAttribute : ElementBaseAttribute
    {
        #region Public constructors region

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAttribute"/> class.
        /// </summary>
        public ElementAttribute()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAttribute"/> class.
        /// </summary>
        /// <param name="name">Name of the element tag.</param>
        public ElementAttribute(string name)
            : base()
        {
            Name = name;
        }

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or sets class of the component's root element name.
        /// </summary>
        public string? Name { get; }

        #endregion
    }
}
