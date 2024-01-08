using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components.DataGrid;

public partial class _DataGridCell<TItem>
{
    #region Public properties region

    /// <summary>
    /// Item associated with the data set.
    /// </summary>
    [Parameter]
    public TItem? Item { get; set; }

    /// <summary>
    /// The row of the cell.
    /// </summary>
    [Parameter]
    public _DataGridRow<TItem>? Row { get; set; }

    /// <summary>
    /// The column of the cell.
    /// </summary>
    [Parameter]
    public DataGridColumn<TItem>? Column { get; set; }

    #endregion
}