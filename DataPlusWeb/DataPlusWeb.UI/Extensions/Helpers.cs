using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace DataPlus.Web.UI;

internal static class Helpers
{
    #region Public fields region

    public const string CurrentModuleStateCascadingPropertyName = "CurrentModuleState_ADF1B262CEB647EFA39A6EAB7255B758";

    public const string CurrentLayoutStateCascadingPropertyName = "CurrentLayoutState_ADF1B262CEB647EFA39A6EAB7255B758";

    public const string ContextCascadingPropertyName = "Context_ADF1B262CEB647EFA39A6EAB7255B758";

    public const string TypedComponentEditMode = "edit";

    public const string TypedComponentDisplayMode = "display";

    public const string TypedComponentNoneMode = "none";

    #endregion

    #region Public methods region

    public static void EmptyRenderFragment(RenderTreeBuilder _) { }

    public static RenderFragment EmptyRenderFragment<T>(T _) => EmptyRenderFragment;

    public static RenderFragment<T> EmptyRenderFragment<T>() => EmptyRenderFragment;

    #endregion
}
