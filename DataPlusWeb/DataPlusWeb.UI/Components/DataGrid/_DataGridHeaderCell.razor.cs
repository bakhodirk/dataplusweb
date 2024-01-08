using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components.DataGrid
{
    public partial class _DataGridHeaderCell<TItem>
    {
        #region Public properties region

        /// <summary>
        /// The header of the cell.
        /// </summary>
        [Parameter]
        public _DataGridHeader<TItem>? Header { get; set; }

        /// <summary>
        /// The column of the cell.
        /// </summary>
        [Parameter]
        public DataGridColumn<TItem>? Column { get; set; }

        #endregion
    }
}