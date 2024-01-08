
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ActionAttribute : Attribute
    {
        #region Public properties region

        /// <summary>
        /// Gets or sets that button which is default action.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets that button which cancel an action.
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// Action to navigate.
        /// </summary>
        public string? NavigateTo { get; set; }

        #endregion
    }
}
