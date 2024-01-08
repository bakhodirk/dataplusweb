
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataPlus.Web.UI.Components
{
    public sealed class Edit : TypedBase
    {
        #region Private fields region

        private static readonly ConcurrentDictionary<Type, Func<object, object>> _calueChangedCallbackFactories = new();

        #endregion

        #region Public constructors region

        public Edit()
            : base(typeof(EditComponent<>))
        { }

        #endregion

        #region Private methods region

        private object GetChangedEventCallback(Type valueType) => _calueChangedCallbackFactories.GetOrAdd(valueType, t =>
        {
            // Func<valueType, Task> callbackExp = value => this.ValueChanged.InvokeAsync((object)value)
            var valueParam = Expression.Parameter(t, "value");
            var callbackExp = Expression.Lambda(
                Expression.GetFuncType(t, typeof(Task)),
                Expression.Call(Expression.Property(Expression.Constant(this), nameof(ValueChanged)), nameof(EventCallback.InvokeAsync), null, Expression.Convert(valueParam, typeof(object))),
                valueParam);

            // EventCallback.Factory.Create<valueType>(receiver, callbackExp)
            var receiverParam = Expression.Parameter(typeof(object), "receiver");
            var creatorExp = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Call(Expression.Constant(EventCallback.Factory), nameof(EventCallbackFactory.Create), new[] { t }, receiverParam, callbackExp), typeof(object)), receiverParam);

            return creatorExp.Compile();
        })(this);

        #endregion

        #region Protected methods region

        protected override void OnRenderComponent(RenderTreeBuilder builder, int sequence, Type componentType, Type[]? genericValueTypes)
        {
            var genericValueType = genericValueTypes != null && genericValueTypes.Length == 1 ? genericValueTypes[0] : typeof(object);

            builder.AddAttribute(sequence, nameof(EditComponent.ValueChanged), genericValueType == typeof(object) ? ValueChanged : GetChangedEventCallback(genericValueType));
        }

        #endregion

        #region Public properties region

        /// <summary>
        /// An event callback which occurs after edit valud has been changed.
        /// </summary>
        [Parameter]
        public EventCallback<object> ValueChanged { get; set; }

        #endregion
    }
}
