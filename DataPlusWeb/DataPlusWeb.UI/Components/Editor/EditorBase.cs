using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components;

public partial class EditorBase : BaseComponent, IEditor
{
    #region Private fields region

    private IModelContext? _baseContext;
    private object? _model;
    private bool _isNew = false;
    private readonly Dictionary<string, EditMember> _members = new();

    #endregion

    #region Internal methods region

    internal void DoExtend(EditMember member)
    {
        if (member.Name is null) return;
        _members.Add(member.Name, member);
    }

    #endregion

    #region Protected methods region

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeModelAsync();
            StateHasChanged();
        }
        base.OnAfterRender(firstRender);
    }

    protected virtual async Task InitializeModelAsync()
    {
        if (_baseContext == null) return;

        // Gets model to edit or create.
        _model = await _baseContext.GetAsync();
        if (_model is null)
        {
            _model = await _baseContext.CreateAsync();
            _isNew = true;
        }
    }

    protected RenderFragment? GetExtendMemberContent(MemberAccessor accessor)
    {
        if (_members.TryGetValue(accessor.Name, out var member))
            return member.ChildContent;
        return null;
    }

    #endregion

    #region Protected properties region

    /// <summary>
    /// Gets or sets base model context.
    /// </summary>
    protected IModelContext? BaseContext { get => _baseContext; set => _baseContext = value; }

    /// <summary>
    /// Gets or sets current model.
    /// </summary>
    protected object? BaseModel
    {
        get => _model;
        set => _model = value;
    }

    protected bool IsNew => _isNew;

    #endregion
}

internal interface IEditor
{ }