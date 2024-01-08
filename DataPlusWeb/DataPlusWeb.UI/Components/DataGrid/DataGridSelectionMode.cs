
namespace DataPlus.Web.UI;

/// <summary>
/// Defines the selection mode of the data grid.
/// </summary>
public enum DataGridSelectionMode
{
    #region Enums

    /// <summary>
    /// The data grid can not be selected.
    /// </summary>
    None,
    /// <summary>
    /// The data grid can only select a row at a time.
    /// </summary>
    Single,
    /// <summary>
    /// The data grid can select multiple rows.
    /// </summary>
    Multiple

    #endregion
}
