using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components.DataGrid;

public partial class _DataGridBody<TItem>
{
    #region Public properties region

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    [Parameter]
    public IEnumerable<DataGridColumn<TItem>>? Columns { get; set; }

    #endregion

}