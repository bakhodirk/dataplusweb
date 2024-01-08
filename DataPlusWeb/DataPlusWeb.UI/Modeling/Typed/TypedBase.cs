using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace DataPlus.Web.UI.Components;

public abstract class TypedBase : BaseComponent
{
    #region Private fields region

    private const string _memberWrapElementName = "span";

    #endregion

    #region Public constructors region

    public TypedBase(params Type[] baseTypes)
        : base()
    {
        if ((baseTypes ?? throw new ArgumentNullException(nameof(baseTypes))).Length == 0)
            throw new ArgumentException($"Argument {nameof(baseTypes)} cannot be empty.", nameof(baseTypes));
        BaseTypes = baseTypes;
    }

    #endregion

    #region Public properties region

    [Parameter]
    public ComponentBase? Owning { get; set; }

    [Parameter]
    public AccessorBase Accessor { get; set; } = default!;

    [Parameter]
    public object? Instance { get; set; }

    #endregion

    #region Protected methods region

    protected override void OnParametersSet()
    {
        if (Accessor is null)
            throw new InvalidOperationException($"Should be provided {nameof(Accessor)}.");

        base.OnParametersSet();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<TypedBase>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<TypedBase>.IsFixed), true);
        builder.AddAttribute(2, nameof(CascadingValue<TypedBase>.Value), this);
        builder.AddAttribute(3, nameof(CascadingValue<TypedBase>.ChildContent), new RenderFragment(b =>
        {
            var renderHandled = false;
            if (Accessor.RenderAsFragment != null)
            {
                Accessor.RenderAsFragment.Invoke(Instance).Invoke(b);
                renderHandled = true;
            }
            else if (Accessor is MemberAccessor member)
            {
                var valueTypes = new[] { member.ValueType };
                if (TypedComponentFactory.ResolveType(BaseTypes, valueTypes, member.DataType, member.CustomDataType, null, out var componentType, out var isGenericDefinition, out var genericTypes, out var parameters))
                {
                    if (isGenericDefinition)
                        componentType = componentType.MakeGenericType(member!.ValueType);

                    b.OpenComponent(4, componentType);
                    b.AddAttribute(5, nameof(TypedComponent.Owning), Owning);
                    b.AddAttribute(6, nameof(TypedComponent.Accessor), Accessor);
                    b.AddAttribute(7, nameof(TypedComponent.Instance), Instance);
                    b.AddAttribute(8, nameof(TypedComponent.Parameters), parameters);
                    b.AddAttribute(9, nameof(Class), Accessor.ElementClass);
                    b.AddAttribute(10, nameof(Style), Accessor.ElementStyle);
                    OnRenderComponent(b, 11, componentType, genericTypes ?? valueTypes);
                    b.CloseComponent();

                    renderHandled = true;
                }
            }
            else if (Accessor is ActionAccessor action)
            {
                if (TypedComponentFactory.ResolveType(BaseTypes, action.ArgumentTypes.DefaultIfEmpty(typeof(void)).ToArray(), action.DataType, action.CustomDataType, null, out var componentType, out var isGenericDefinition, out var genericTypes, out var parameters) ||
                    TypedComponentFactory.ResolveType(BaseTypes, new[] { typeof(void) }, action.DataType, action.CustomDataType, null, out componentType, out isGenericDefinition, out genericTypes, out parameters))
                {
                    if (isGenericDefinition)
                        throw new NotSupportedException("Generic definated action is not supported.");

                    b.OpenComponent(4, componentType);
                    b.AddAttribute(5, nameof(TypedComponent.Owning), Owning);
                    b.AddAttribute(6, nameof(TypedComponent.Accessor), Accessor);
                    b.AddAttribute(7, nameof(TypedComponent.Instance), Instance);
                    b.AddAttribute(8, nameof(TypedComponent.Parameters), parameters);
                    b.AddAttribute(9, nameof(Class), Accessor.ElementClass);
                    b.AddAttribute(10, nameof(Style), Accessor.ElementStyle);
                    OnRenderComponent(b, 11, componentType, genericTypes);
                    b.CloseComponent();

                    renderHandled = true;
                }
            }

            if (!renderHandled)
                BuildDefaultRenderTree(b);
        }));
        builder.CloseComponent();

        base.BuildRenderTree(builder);
    }

    protected virtual void BuildDefaultRenderTree(RenderTreeBuilder builder)
    {
        if (Instance is null) return;

        if (Accessor is MemberAccessor member)
        {
            var elementName = Accessor.ElementName ?? (Accessor.ElementClass is null && Accessor.ElementStyle is null ? null : _memberWrapElementName);
            if (elementName is null)
                builder.AddContent(0, member.GetValue(Instance));
            else
            {
                builder.OpenElement(0, elementName);
                builder.AddAttribute(1, "class", Accessor.ElementClass);
                builder.AddAttribute(2, "style", Accessor.ElementStyle);
                builder.AddContent(3, member.GetValue(Instance));
                builder.CloseElement();
            }
        }
        else if (Accessor is ActionAccessor action)
        {
            builder.OpenComponent<Button>(0);
            builder.AddContent(1, action.ShortName ?? action.DisplayName);
            builder.CloseComponent();
        }
    }

    protected virtual void OnRenderComponent(RenderTreeBuilder builder, int sequence, Type componentType, Type[]? genericValueTypes)
    { }

    #endregion

    #region Protected properties region

    protected Type[] BaseTypes { get; }

    [Inject]
    protected ITypedComponentFactory TypedComponentFactory { get; set; } = default!;

    #endregion
}
