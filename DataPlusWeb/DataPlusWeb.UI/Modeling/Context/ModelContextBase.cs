using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlus.Web.UI
{
    public abstract class ModelContextBase<TModel> : IModelContext<TModel>, IModelContext
    {
        #region Private fields region

        private TypeAccessor? _modelType;
        private TypeAccessor? _contextType;

        #endregion

        #region Public methods region

        /// <inheritdoc/>
        public virtual ValueTask<TModel?> GetAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual ValueTask<TModel> CreateAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual Task UpdateAsync(TModel model, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual Task DeleteAsync(TModel model, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TModel> ListAsync(ModelContextListArgs args, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual ValueTask<int> ListCountAsync(ModelContextListArgs args, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual IEnumerable<MemberAccessor> GetMembers(TypeAccessor type, Type? attributeType) => attributeType == null ? type.Members : type.GetMembers(attributeType);

        /// <summary>
        /// Gets members which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select members.</param>
        /// <typeparam name="TAttribute">The type of attribute to filter.</typeparam>
        public IEnumerable<MemberAccessor> GetMembers<TAttribute>(TypeAccessor type) where TAttribute : Attribute => GetMembers(type, typeof(TAttribute));

        /// <inheritdoc/>
        public virtual IEnumerable<ActionAccessor> GetActions(TypeAccessor type, Type? attributeType) => attributeType == null ? type.Actions : type.GetActions(attributeType);

        /// <summary>
        /// Gets actions which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select members.</param>
        /// <typeparam name="TAttribute">The type of attribute to filter.</typeparam>
        public IEnumerable<ActionAccessor> GetActions<TAttribute>(TypeAccessor type) where TAttribute : Attribute => GetActions(type, typeof(TAttribute));

        #endregion

        #region Public properties region

        /// <inheritdoc/>
        public TypeAccessor ModelType { get => _modelType ??= AccessorHelper.GetTypeAccessor<TModel>(); }

        /// <inheritdoc/>
        public TypeAccessor ContextType { get => _contextType ??= AccessorHelper.GetTypeAccessor(GetType()); }

        #endregion

        #region IModelContext members

        /// <inheritdoc/>
        async ValueTask<object?> IModelContext.GetAsync(CancellationToken cancellationToken) => await GetAsync(cancellationToken);

        /// <inheritdoc/>
        async ValueTask<object> IModelContext.CreateAsync(CancellationToken cancellationToken) => (await CreateAsync(cancellationToken))!;

        /// <inheritdoc/>
        async Task IModelContext.UpdateAsync(object model, CancellationToken cancellationToken) => await UpdateAsync((TModel)model, cancellationToken);

        /// <inheritdoc/>
        async Task IModelContext.DeleteAsync(object model, CancellationToken cancellationToken) => await DeleteAsync((TModel)model, cancellationToken);

        /// <inheritdoc/>
        async IAsyncEnumerable<object> IModelContext.ListAsync(ModelContextListArgs filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in ListAsync(filter, cancellationToken))
                yield return item!;
        }

        /// <inheritdoc/>
        ValueTask<int> IModelContext.ListCountAsync(ModelContextListArgs filter, CancellationToken cancellationToken) => ListCountAsync(filter, cancellationToken);

        /// <inheritdoc/>
        IEnumerable<MemberAccessor> IModelContext.GetMembers(TypeAccessor type, Type? attributeType) => GetMembers(type, attributeType);

        /// <inheritdoc/>
        IEnumerable<ActionAccessor> IModelContext.GetActions(TypeAccessor type, Type? attributeType) => GetActions(type, attributeType);

        #endregion
    }
}
