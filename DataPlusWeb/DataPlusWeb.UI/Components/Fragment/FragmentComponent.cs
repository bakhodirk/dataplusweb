using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components;

public abstract class FragmentComponent : BaseComponent
{
    #region Public properties region

    /// <summary>
    /// Gets or sets member to provide the fragment.
    /// </summary>
    [Parameter]
    public MemberAccessor? Member { get; set; }

    /// <summary>
    /// Gets or sets instance which specified member is belong to.
    /// </summary>
    [Parameter]
    public object? Instance { get; set; }

    #endregion
}
