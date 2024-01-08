using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlus.Web.UI
{
    public interface IModelContext
    {
        #region Properties region

        /// <summary>
        /// Gets type accessor of the model.
        /// </summary>
        TypeAccessor ModelType { get; }

        /// <summary>
        /// Gets type accessor of the context.
        /// </summary>
        TypeAccessor ContextType { get; }

        #endregion

        #region Methods region

        /// <summary>
        /// Gets single model.
        /// </summary>
        ValueTask<object?> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a model.
        /// </summary>
        ValueTask<object> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="model"></param>
        Task UpdateAsync(object model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the model.
        /// </summary>
        /// <param name="model"></param>
        Task DeleteAsync(object model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of models to view.
        /// </summary>
        /// <param name="args">A get list arguments.</param>
        /// <param name="cancellationToken">An cancellation token.</param>
        IAsyncEnumerable<object> ListAsync(ModelContextListArgs args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets number of models in list.
        /// </summary>
        /// <param name="args">A get list arguments.</param>
        /// <param name="cancellationToken">An cancellation token.</param>
        ValueTask<int> ListCountAsync(ModelContextListArgs args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets members which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select members.</param>
        /// <param name="attributeType">The type of attribute to filter.</param>
        IEnumerable<MemberAccessor> GetMembers(TypeAccessor type, Type? attributeType);

        /// <summary>
        /// Gets actions which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select actions.</param>
        /// <param name="attributeType">The type of attribute to filter.</param>
        IEnumerable<ActionAccessor> GetActions(TypeAccessor type, Type? attributeType);

        #endregion
    }

    public interface IModelContext<TModel>
    {
        #region Properties region

        /// <summary>
        /// Gets type accessor of the model.
        /// </summary>
        TypeAccessor ModelType { get; }

        /// <summary>
        /// Gets type accessor of the context.
        /// </summary>
        TypeAccessor ContextType { get; }

        #endregion

        #region Methods region

        /// <summary>
        /// Gets single model.
        /// </summary>
        ValueTask<TModel?> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a model.
        /// </summary>
        /// <returns></returns>
        ValueTask<TModel> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task DeleteAsync(TModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of models to view.
        /// </summary>
        /// <param name="args">A get list arguments.</param>
        /// <param name="cancellationToken">An cancellation token.</param>
        IAsyncEnumerable<TModel> ListAsync(ModelContextListArgs args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets number of models in list.
        /// </summary>
        /// <param name="args">A get list arguments.</param>
        /// <param name="cancellationToken">An cancellation token.</param>
        ValueTask<int> ListCountAsync(ModelContextListArgs args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets members which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select members.</param>
        /// <param name="attributeType">The type of attribute to filter.</param>
        IEnumerable<MemberAccessor> GetMembers(TypeAccessor type, Type? attributeType);

        /// <summary>
        /// Gets actions which defined sepcifeid attribute.
        /// </summary>
        /// <param name="type">The type accessor to select actions.</param>
        /// <param name="attributeType">The type of attribute to filter.</param>
        IEnumerable<ActionAccessor> GetActions(TypeAccessor type, Type? attributeType);

        #endregion
    }
}
