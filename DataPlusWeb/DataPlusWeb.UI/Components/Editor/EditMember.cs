using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components;

public class EditMember : ComponentBase
{
    #region Internal properties region

    [CascadingParameter]
    public EditorBase? Editor { get; set; }

    #endregion

    #region Public properties region

    [Parameter]
    public string? Name { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    #endregion

    #region Protected methods region

    protected override void OnInitialized()
    {
        Editor?.DoExtend(this);
        base.OnInitialized();
    }

    #endregion
}
