using Microsoft.AspNetCore.Components;

namespace DataPlus.Web.UI.Components
{
    public abstract class TypedComponent : BaseComponent
    {
        #region Public properties region

        [Parameter]
        public ComponentBase? Owning { get; set; }

        [Parameter]
        public AccessorBase Accessor { get; set; } = default!;

        [Parameter]
        public object? Instance { get; set; }

        [Parameter]
        public object?[]? Parameters { get; set; }

        #endregion

        #region Protected methods region

        protected TParameterValue? GetParameter<TParameterValue>(int index = 0)
        {
            if (index < Parameters?.Length)
            {
                var parameter = Parameters[index];
                if (parameter == null) return default;
                return (TParameterValue)parameter;
            }
            return default;
        }

        #endregion

        #region Protected properties region

        /// <summary>
        /// Gets component typed base.
        /// </summary>
        [CascadingParameter]
        protected TypedBase? Typed { get; set; }

        #endregion
    }
}
