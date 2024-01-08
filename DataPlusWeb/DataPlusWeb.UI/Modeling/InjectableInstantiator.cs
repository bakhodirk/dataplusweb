using Microsoft.AspNetCore.Components;
using System;

namespace DataPlus.Web.DependencyInjection
{
    /// <inheritdoc/>
    public sealed class InjectableInstantiator : Instantiator<InjectAttribute>
    {
        /// <inheritdoc/>
        public InjectableInstantiator(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    /// <inheritdoc/>
    public sealed class InjectableInstantiator<TArgumentType> : Instantiator<InjectAttribute, TArgumentType>
    {
        /// <inheritdoc/>
        public InjectableInstantiator(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    /// <inheritdoc/>
    public sealed class InjectableInstantiator<TArgumentType1, TArgumentType2> : Instantiator<InjectAttribute, TArgumentType1, TArgumentType2>
    {
        /// <inheritdoc/>
        public InjectableInstantiator(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    /// <inheritdoc/>
    public sealed class InjectableInstantiator<TArgumentType1, TArgumentType2, TArgumentType3> : Instantiator<InjectAttribute, TArgumentType1, TArgumentType2, TArgumentType3>
    {
        /// <inheritdoc/>
        public InjectableInstantiator(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
