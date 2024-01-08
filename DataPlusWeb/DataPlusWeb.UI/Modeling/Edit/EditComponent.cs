using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace DataPlus.Web.UI.Components
{
    public abstract class EditComponent : EditComponent<object>
    { }

    public abstract class EditComponent<T> : TypedComponent
    {
        #region Public properties region

        /// <summary>
        /// Occurs when value has been changed.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        #endregion

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

        [CascadingParameter]
        protected EditorBase? Editor { get; set; }

        //[CascadingParameter]
        //protected Validation? Validation { get; set; }

        /// <summary>
        /// Gets member accessor.
        /// </summary>
        protected MemberAccessor Member => (MemberAccessor)Accessor;

        /// <summary>
        /// Gets or sets the member value of the instance.
        /// </summary>
        /// <returns>Returns the member value.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="Instance"/> property is not set.</exception>
        protected virtual T? Value
        {
            get => (T?)Member.GetValue(Instance!);
            set => Member.SetValue(Instance!, value);
        }

        /// <summary>
        /// Gets the member formatted value of the instance.
        /// </summary>
        /// <returns>Returns the member formatted value.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="Instance"/> property is not set.</exception>
        protected string? FormattedValue => Member.GetStringValue(Instance!);

        /// <summary>
        /// Gets member value expression.
        /// </summary>
        public Expression<Func<T>> GetValueExpression() => Member.GetExpression<T>(Instance);

        /// <summary>
        /// Gets member value expression with specifeid value type.
        /// </summary>
        public Expression<Func<TValue>> GetValueExpression<TValue>() where TValue : T => Member.GetExpression<TValue>(Instance);

        #endregion
    }
}
