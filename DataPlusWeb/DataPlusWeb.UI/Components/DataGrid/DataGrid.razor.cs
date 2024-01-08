using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace DataPlus.Web.UI.Components;

public partial class DataGrid<TItem>
{
    #region Internal methods region

    internal void Hook(DataGridColumn<TItem> column)
    {
        Columns.Add(column);
    }

    internal IEnumerable<TItem> GetDisplayData()
    {
        return Items ?? Enumerable.Empty<TItem>();
    }

    internal bool IsRowSelectable(TItem item) => SelectionMode != DataGridSelectionMode.None && RowSelectable?.Invoke(item) != false;

    internal void SelectRow(TItem item, bool unselect)
    {
        if (SelectionMode == DataGridSelectionMode.Single)
        {
            if (unselect)
            {
                if (object.Equals(item, SelectedItem))
                {
                    SelectedItem = default;
                    SelectedItemChanged.InvokeAsync(SelectedItem);
                }
            }
            else
            {
                if (!object.Equals(item, SelectedItem))
                {
                    SelectedItem = item;
                    SelectedItemChanged.InvokeAsync(SelectedItem);
                }
            }
        }
        else if (SelectionMode == DataGridSelectionMode.Multiple)
        {
            if (unselect)
            {
                if (SelectedItems != null && SelectedItems.Remove(item))
                    SelectedItemsChanged.InvokeAsync(SelectedItems);
            }
            else
            {
                if (SelectedItems is null)
                {
                    SelectedItems = new() { item };
                    SelectedItemsChanged.InvokeAsync(SelectedItems);
                }
                else if (SelectedItems.Remove(item))
                    SelectedItemsChanged.InvokeAsync(SelectedItems);
                else
                {
                    SelectedItems.Add(item);
                    SelectedItemsChanged.InvokeAsync(SelectedItems);
                }
            }
        }
    }

    internal Task InvokeRowClicked(DataGridRowMouseEventArgs<TItem> eventArgs) => OnRowClicked(eventArgs);

    internal Task InvokeRowDoubleClicked(DataGridRowMouseEventArgs<TItem> eventArgs) => OnRowDoubleClicked(eventArgs);

    #endregion

    #region Internal properties region

    /// <summary>
    /// Gets or sets columns.
    /// </summary>
    internal List<DataGridColumn<TItem>> Columns { get; } = new();

    /// <summary>
    /// True if user is using <see cref="ReadItems"/> for loading the data.
    /// </summary>
    internal bool HasReadItems => ReadItems.HasDelegate;

    internal bool CanShowEmpty { get; set; }

    #endregion

    #region Public methods region

    /// <summary>
    /// Triggers the reload of the <see cref="DataGrid{TItem}"/> data.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    public Task Reload()
    {
        if (HasReadItems)
            return InvokeAsync(() => HandleReadData(CancellationToken.None));
        else
            return InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Public properties region

    /// <summary>
    /// Template for holding the datagrid columns.
    /// </summary>
    [Parameter]
    public RenderFragment? DataGridColumns { get; set; }

    /// <summary>
    /// Gets or sets the data source of the <see cref="DataGrid{TItem}"/>.
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Occurs after the <see cref="Items"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<IEnumerable<TItem>?> ItemsChanged { get; set; }

    /// <summary>
    /// Event handler used to load data manually based on the current page and filter data settings.
    /// </summary>
    [Parameter]
    public EventCallback<DataGridReadDataEventArgs<TItem>> ReadItems { get; set; }

    /// <summary>
    /// Gets or sets content of table body for empty DisplayData.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of cell body for empty DisplayData.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? EmptyCellTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of table body for handle ReadData.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items for each page.
    /// </summary>
    [Parameter]
    public int PageSize { get; set; }

    /// <summary>
    /// Occurs after the <see cref="PageSize"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageSizeChanged { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the sort property.
    /// </summary>
    [Parameter]
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the sort is ascending.
    /// </summary>
    [Parameter]
    public bool? IsSortAscending { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="System.Boolean"/> value which indicating whether should hide grid columns or not.
    /// </summary>
    [Parameter]
    public bool HideColumns { get; set; }

    /// <summary>
    /// Adds a hover effect when mousing over rows.
    /// </summary>
    [Parameter]
    public bool Hoverable { get; set; }

    /// <summary>
    /// Gets or sets current selection mode.
    /// </summary>
    [Parameter]
    public DataGridSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Handles the selection of the clicked row. If not set it will default to always true.
    /// </summary>
    [Parameter]
    public Func<TItem, bool>? RowSelectable { get; set; }

    /// <summary>
    /// Gets or sets currently selected item.
    /// </summary>
    [Parameter]
    public TItem? SelectedItem { get; set; }

    /// <summary>
    /// Occurs after the selected item has changed.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> SelectedItemChanged { get; set; }

    /// <summary>
    /// Gets or sets currently selected items.
    /// </summary>
    [Parameter]
    public List<TItem>? SelectedItems { get; set; }

    /// <summary>
    /// Occurs after multi selection has changed.
    /// </summary>
    [Parameter]
    public EventCallback<List<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Handles the selection of the cursor for a hovered row. If not set, <see cref="Cursor.Default"/> will be used.
    /// </summary>
    [Parameter]
    public Func<TItem, Cursor>? RowHoverCursor { get; set; }

    /// <summary>
    /// Event called after the row is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DataGridRowMouseEventArgs<TItem>> RowClicked { get; set; }

    /// <summary>
    /// Event called after the row is double clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DataGridRowMouseEventArgs<TItem>> RowDoubleClicked { get; set; }

    /// <summary>
    /// Custom handler for each row.
    /// </summary>
    [Parameter]
    public Action<TItem, DataGridRowStyle>? RowStyling { get; set; }

    /// <summary>
    /// Custom handler for currently selected row.
    /// </summary>
    [Parameter]
    public Action<TItem, DataGridRowStyle>? SelectedRowStyling { get; set; }

    /// <summary>
    /// Template for displaying detail or nested row.
    /// </summary>
    [Parameter, AllowNull]
    public RenderFragment<TItem>? DetailRowTemplate { get; set; }

    /// <summary>
    /// A trigger function used to handle the visibility of detail row.
    /// </summary>
    [Parameter]
    public Func<TItem, bool>? DetailRowTrigger { get; set; }

    #endregion

    #region Protected methods region

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CanShowEmpty = true;

            if (HasReadItems)
            {
                await HandleReadData(CancellationToken.None);
                return;
            }

            // after all the columns have being "hooked" we need to resfresh the grid
            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task HandleReadData(CancellationToken cancellationToken)
    {
        try
        {
            IsLoading = true;

            if (!cancellationToken.IsCancellationRequested)
                await ReadItems.InvokeAsync(new(this, cancellationToken)).ConfigureAwait(false);
        }
        finally
        {
            IsLoading = false;

            await InvokeAsync(StateHasChanged);
        }
    }

    protected Task OnRowClicked(DataGridRowMouseEventArgs<TItem> eventArgs) => RowClicked.InvokeAsync(eventArgs);

    protected Task OnRowDoubleClicked(DataGridRowMouseEventArgs<TItem> eventArgs) => RowDoubleClicked.InvokeAsync(eventArgs);

    #endregion

    #region Protected properties region

    /// <summary>
    /// Returns true if ReadData will be invoked.
    /// </summary>
    protected internal bool IsLoading { get; set; }

    /// <summary>
    /// Gets the context manager.
    /// </summary>
    [CascadingParameter]
    protected ContextManager? ContextManager { get; set; }

    #endregion
}