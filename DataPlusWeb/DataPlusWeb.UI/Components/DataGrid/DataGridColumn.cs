using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using System.Globalization;

namespace DataPlus.Web.UI.Components;

public class DataGridColumn<TItem> : DataGridComponentBase<TItem>
{
    #region Private fields region

    private static readonly ConcurrentDictionary<string, Func<TItem, object?>> _getterCache = new();

    private readonly Lazy<Func<TItem, object?>> _valueGetter;

    #endregion

    #region Internal methods region

    internal object? GetDefaultValue(TItem item) => default;

    internal object? GetValue(TItem item) => _valueGetter.Value(item);

    internal string? FormatCellValue(object? value)
    {
        if (CellValueFormat != null)
            return string.Format(CellValueFormatProvider ?? CultureInfo.CurrentCulture, CellValueFormat, value);
        return value?.ToString();
    }

    #endregion

    #region Public constructors region

    public DataGridColumn()
        : base()
    {
        _valueGetter = new Lazy<Func<TItem, object?>>(() => string.IsNullOrEmpty(Property) ? GetDefaultValue : _getterCache.GetOrAdd(Property, p => ModellingHelper.CreateValueGetter<TItem>(p)));
    }

    #endregion

    #region Public properties region

    /// <summary>
    /// Gets or stes item property name.
    /// </summary>
    [Parameter]
    public string? Property { get; set; }

    /// <summary>
    /// Gets or sets header name.
    /// </summary>
    [Parameter]
    public string? Header { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Template for custom cell display formating.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? CellTemplate { get; set; }

    /// <summary>
    /// Defines the format for display value.
    /// </summary>
    [Parameter]
    public string? CellValueFormat { get; set; }

    /// <summary>
    /// Defines the format provider info for display value.
    /// </summary>
    [Parameter]
    public IFormatProvider? CellValueFormatProvider { get; set; }

    #endregion

    #region Protected methods region

    protected override void OnInitialized()
    {
        Grid?.Hook(this);
        base.OnInitialized();
    }

    #endregion
}
