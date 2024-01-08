
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class GridAttribute : ElementBaseAttribute
    {
        #region Private fields region

        private bool? _hideColumns;

        #endregion

        #region Public methods region

        public bool? GetHideColumns() => _hideColumns;

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or sets a System.Boolean value which indicating whether should hide grid columns or not.
        /// </summary>
        public bool HideColumns { get => _hideColumns ?? false; set => _hideColumns = value; }

        #endregion
    }
}
