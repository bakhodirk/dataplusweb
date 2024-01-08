using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlus.Web.UI
{
    internal sealed class DelegatedModelContext<TModel> : ModelContextBase<TModel>
    {
        #region Public methods region

        public override ValueTask<TModel?> GetAsync(CancellationToken cancellationToken) => (GetFactory ?? throw new NotImplementedException())(cancellationToken);

        public override ValueTask<TModel> CreateAsync(CancellationToken cancellationToken) => (CreateFactory ?? throw new NotImplementedException())(cancellationToken);

        public override Task UpdateAsync(TModel model, CancellationToken cancellationToken) => (UpdateFactory ?? throw new NotImplementedException())(model, cancellationToken);

        public override Task DeleteAsync(TModel model, CancellationToken cancellationToken) => (DeleteFactory ?? throw new NotImplementedException())(model, cancellationToken);

        public override IAsyncEnumerable<TModel> ListAsync(ModelContextListArgs args, CancellationToken cancellationToken) => (ListFactory ?? throw new NotImplementedException())(args, cancellationToken);

        #endregion

        #region Public properties region

        public Func<CancellationToken, ValueTask<TModel?>>? GetFactory { get; set; }

        public Func<CancellationToken, ValueTask<TModel>>? CreateFactory { get; set; }

        public Func<TModel, CancellationToken, Task>? UpdateFactory { get; set; }

        public Func<TModel, CancellationToken, Task>? DeleteFactory { get; set; }

        public Func<ModelContextListArgs, CancellationToken, IAsyncEnumerable<TModel>>? ListFactory { get; set; }

        #endregion
    }
}
