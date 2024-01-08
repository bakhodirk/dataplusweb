using Microsoft.AspNetCore.Components.Web;

namespace DataPlus.Web.UI.Components;

/// <summary>
/// Provides the <see cref="DataGrid{TItem}"/> event args.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// Initializes a new instance of grid event argument.
/// </remarks>
/// <param name="grid">Data grid.</param>
public class DataGridEventArgs<TItem>(DataGrid<TItem> grid) : EventArgs
{
    #region Public properties region

    /// <summary>
    /// Gets the data grid.
    /// </summary>
    public DataGrid<TItem> Grid { get; } = grid;

    #endregion
}

/// <summary>
/// Provides all the information for loading the <see cref="DataGrid{TItem}"/> data.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// Initializes a new instance of read-data event argument.
/// </remarks>
/// <param name="grid">Data grid.</param>
public class DataGridReadDataEventArgs<TItem>(DataGrid<TItem> grid, CancellationToken cancellationToken)
    : DataGridEventArgs<TItem>(grid)
{
    #region Public properties region

    /// <summary>
    /// Gets the CancellationToken
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = cancellationToken;

    /// <summary>
    /// Gets or set loading state.
    /// </summary>
    public bool IsLoading { get => Grid.IsLoading; set => Grid.IsLoading = value; }

    #endregion
}

/// <summary>
/// Provides all the information about the clicked event on <see cref="DataGrid{TItem}"/> row.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridRowMouseEventArgs<TItem>(TItem item, MouseEventArgs mouseEventArgs) : EventArgs
{
    #region Public methods region

    /// <summary>
    /// Gets the model.
    /// </summary>
    public TItem Item { get; } = item;

    /// <summary>
    /// Gets the mouse event details.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; } = mouseEventArgs;

    #endregion
}

/// <summary>
/// Provides item the <see cref="DataGrid{TItem}"/> data.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// Initializes a new instance of read-data event argument.
/// </remarks>
/// <param name="grid">Data grid.</param>
/// <param name="item">Data grid data item.</param>
public class FluidGridItemEventArgs<TItem>(DataGrid<TItem> grid, TItem item)
    : DataGridEventArgs<TItem>(grid)
{
    #region Public properties region

    /// <summary>
    /// Gets the fluid grid item.
    /// </summary>
    public TItem Item { get; } = item;

    #endregion
}
