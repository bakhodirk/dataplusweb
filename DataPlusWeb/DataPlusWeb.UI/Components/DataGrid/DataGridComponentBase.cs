using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace DataPlus.Web.UI.Components;

public abstract class DataGridComponentBase<TItem> : BaseComponent
{
    #region Protected properties region

    [CascadingParameter, AllowNull]
    protected DataGrid<TItem> Grid { get; set; }

    #endregion
}