using Microsoft.AspNetCore.Components;
using DataPlus.Web.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DataPlus.Web.UI.Components;

public partial class Contextual<TContext> : IActionArgumentValueProviderContainer
{
    #region Private fields region

    private Instantiator<InjectAttribute>? _instantiator;
    private readonly List<IActionArgumentValueProvider> _actionArgumentValueProviders = new();
    private ContextManager? _contextManager;

    #endregion

    #region Private methods region

    private TContext CreateContext()
    {
        // Ensure the instantiator is initialized.
        _instantiator ??= new InjectableInstantiator(ServiceProvider);

        return Argument switch
        {
            object[] args => _instantiator.Instantiate<TContext>(args),
            object arg when arg is not null => _instantiator.Instantiate<TContext>(arg),
            _ => _instantiator.Instantiate<TContext>()
        };
    }

    private ContextManager GetContextManager() => _contextManager ??= new ContextManager(Instance);

    #endregion

    #region Public properties region

    [Parameter]
    public RenderFragment<TContext>? ChildContent { get; set; }

    [Parameter, NotNull]
    public TContext Instance { get; set; } = default!;

    [Parameter]
    public object? Argument { get; set; }

    #endregion

    #region Protected methods region

    protected override void OnParametersSet()
    {
        Instance ??= CreateContext();
        base.OnParametersSet();
    }

    #endregion

    #region Protected properties region

    [Inject]
    protected IServiceProvider ServiceProvider { get; set; } = default!;

    #endregion

    #region IActionArgumentValueProviderContainer members

    void IActionArgumentValueProviderContainer.Register(IActionArgumentValueProvider provider) => _actionArgumentValueProviders.Add(provider);

    IEnumerable<IActionArgumentValueProvider> IActionArgumentValueProviderContainer.GetProviders() => _actionArgumentValueProviders;

    #endregion
}