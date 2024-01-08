using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace DataPlus.Web.UI.Components.DataGrid;

public partial class _DataGridRow<TItem>
{
    #region Public properties region

    /// <summary>
    /// Item associated with the data set.
    /// </summary>
    [Parameter, AllowNull]
    public TItem Item { get; set; }

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    [Parameter]
    public IEnumerable<DataGridColumn<TItem>>? Columns { get; set; }

    [Parameter]
    public Cursor HoverCursor { get; set; }

    /// <summary>
    /// Occurs when the row is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DataGridRowMouseEventArgs<TItem>> Clicked { get; set; }

    /// <summary>
    /// Occurs when the row is double clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DataGridRowMouseEventArgs<TItem>> DoubleClicked { get; set; }

    #endregion

    #region Protected methods region

    protected async Task OnClicked(MouseEventArgs e)
    {
        if (e.Detail == 1)
        {
            await Clicked.InvokeAsync(new(Item, e));

            if (!Grid.IsRowSelectable(Item)) return;
            Grid.SelectRow(Item, e.CtrlKey && (MouseButton)e.Button == MouseButton.Left);
        }
        else if (e.Detail == 2)
        {
            await DoubleClicked.InvokeAsync(new(Item, e));
        }
    }

    #endregion

    #region Protected properties region

    protected bool IsSelected =>
        Grid.SelectionMode == DataGridSelectionMode.Single && object.Equals(Item, Grid.SelectedItem) ||
        Grid.SelectionMode == DataGridSelectionMode.Multiple && Grid.SelectedItems?.Contains(Item) == true;

    #endregion
}