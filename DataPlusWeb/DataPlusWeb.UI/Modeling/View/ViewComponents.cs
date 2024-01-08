using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlus.Web.UI.Components
{
    public abstract class ViewComponent : TypedComponent
    { }

    public abstract class ViewComponent<T> : TypedComponent
    { }

    public abstract class MemberViewComponent : MemberViewComponent<object>
    { }

    public abstract class MemberViewComponent<T> : ViewComponent<T>
    {
        #region Protected methods region

        /// <summary>
        /// Try gets the member value of the instance.
        /// </summary>
        /// <param name="value">The member value.</param>
        /// <returns>true if instance is valid; otherwise false.</returns>
        protected bool TryGetValue(out T value)
        {
            if (Instance != null)
            {
                value = (T)Member.GetValue(Instance)!;
                return true;
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Try gets the member formatted  value of the instance.
        /// </summary>
        /// <param name="value">The member formatted value.</param>
        /// <returns>true if instance is valid; otherwise false.</returns>
        protected bool TryGetFormattedValue(out string value)
        {
            if (Instance != null)
            {
                value = Member.GetStringValue(Instance)!;
                return true;
            }
            value = default!;
            return false;
        }

        #endregion

        #region Protected properties region

        /// <summary>
        /// Gets member accessor.
        /// </summary>
        protected MemberAccessor Member => (MemberAccessor)Accessor;

        /// <summary>
        /// Gets the member value of the instance.
        /// </summary>
        /// <returns>Returns the member value.</returns>
        protected T? Value => (T?)Member.GetValue(Instance!);

        /// <summary>
        /// Gets the member formatted value of the instance.
        /// </summary>
        /// <returns>Returns the member formatted value.</returns>
        protected string? FormattedValue => Member.GetStringValue(Instance!);

        #endregion
    }

    public abstract class ActionComponent : ViewComponent
    {
        #region Private fields region

        [AllowNull]
        private ComponentActionArgumentValueProvider _argumentValueProvider;

        #endregion

        #region Protected methods region

        protected override void OnInitialized()
        {
            _argumentValueProvider = new(ActionArgumentValueProviderContainer);
            base.OnInitialized();
        }

        protected Task<object?> InvokeAsync()
        {
            if (Instance is null) return Task.FromException<object?>(new InvalidOperationException($"Target to invoke action is not available. Provide the value of {GetType()}.{Instance} property."));
            return Action.InvokeAsync(Instance, _argumentValueProvider, CancellationToken.None);
        }

        protected object? Invoke()
        {
            if (Instance is null) throw new InvalidOperationException($"Target to invoke action is not available. Provide the value of {GetType()}.{Instance} property.");
            return Action.Invoke(Instance, _argumentValueProvider);
        }

        #endregion

        #region Protected properties region

        /// <summary>
        /// Gets action accessor.
        /// </summary>
        protected ActionAccessor Action => (ActionAccessor)Accessor;

        [CascadingParameter]
        protected IActionArgumentValueProviderContainer? ActionArgumentValueProviderContainer { get; set; }

        #endregion
    }

    public abstract class ActionComponent<TArgument> : ActionComponent
    { }

    public abstract class ActionComponent<TArgument1, TArgument2> : ActionComponent
    { }

    public abstract class ActionComponent<TArgument1, TArgument2, TArgument3> : ActionComponent
    { }

    public abstract class ActionComponent<TArgument1, TArgument2, TArgument3, TArgument4> : ActionComponent
    { }

    public abstract class ActionComponent<TArgument1, TArgument2, TArgument3, TArgument4, TArgument5> : ActionComponent
    { }
}
