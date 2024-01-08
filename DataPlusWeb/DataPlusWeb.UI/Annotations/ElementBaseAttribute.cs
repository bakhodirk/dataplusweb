
namespace System.ComponentModel.DataAnnotations
{
    public abstract class ElementBaseAttribute : Attribute
    {
        #region Public properties region

        /// <summary>
        /// Gets or sets class of the component's root element.
        /// </summary>
        public string? Class { get; set; }

        /// <summary>
        /// Gets or sets style of the component's root element.
        /// </summary>
        public string? Style { get; set; }

        #endregion
    }
}
