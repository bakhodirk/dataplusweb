using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components;

public abstract class BaseComponent : ComponentBase
{
    #region Private fields region

    private Queue<Func<Task>>? _executeAfterRenderQueue;

    #endregion

    #region Public methods region


    public void Dispose()
    {
        Dispose(disposing: true);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        Dispose(disposing: false);
    }

    #endregion

    #region Public properties region

    public string? Class {  get; set; }

    public string? Style { get; set; }

    #endregion

    #region Protected methods region

    /// <summary>
    /// Pushes an action to the stack to be executed after the rendering is done.
    /// </summary>
    /// <param name="action">An action to execute after render.</param>
    protected void ExecuteAfterRender(Func<Task> action)
    {
        _executeAfterRenderQueue ??= new Queue<Func<Task>>();
        _executeAfterRenderQueue.Enqueue(action);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Rendered = true;
        if (_executeAfterRenderQueue?.Count > 0)
        {
            while (_executeAfterRenderQueue.Count > 0)
            {
                await _executeAfterRenderQueue.Dequeue()();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BaseComponent"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            Disposed = true;
            if (disposing)
            {
                _executeAfterRenderQueue?.Clear();
            }
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BaseComponent"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True if the component is in the disposing process.</param>
    /// <returns></returns>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        try
        {
            if (!AsyncDisposed)
            {
                AsyncDisposed = true;
                if (disposing)
                {
                    _executeAfterRenderQueue?.Clear();
                }
            }

            return default;
        }
        catch (Exception exception)
        {
            return new ValueTask(Task.FromException(exception));
        }
    }

    #endregion

    #region Protected properties region

    /// <summary>
    /// Indicates if the component is already fully disposed.
    /// </summary>
    protected bool Disposed { get; private set; }

    /// <summary>
    /// Indicates if the component is already fully disposed (asynchronously).
    /// </summary>
    protected bool AsyncDisposed { get; private set; }

    /// <summary>
    /// Indicates if component has been rendered in the browser.
    /// </summary>
    protected bool Rendered { get; private set; }

    #endregion
}
