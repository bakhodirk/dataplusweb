namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class GridColumnAttribute : ElementBaseAttribute
{
    #region Private fields region

    private bool? _filterable;

    #endregion

    #region Internal methods region

    internal bool? GetFilterable() => _filterable;

    #endregion

    #region Public properties region

    /// <summary>
    /// Gets or set a <see cref="System.Boolean"/> value which indicating cn be filtered the column.
    /// </summary>
    public bool Filterable { get => _filterable ?? false; set => _filterable = value; }

    /// <summary>
    /// Gets or set a <see cref="System.String"/> value which indicating cn be filter field name.
    /// </summary>
    public string? FilterField { get; set; }

    #endregion
}
