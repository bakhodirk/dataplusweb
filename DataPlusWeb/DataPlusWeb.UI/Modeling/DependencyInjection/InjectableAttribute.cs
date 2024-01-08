using System;

namespace Microsoft.Extensions.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class InjectableAttribute : Attribute
    {
        #region Public properties region

        /// <summary>
        /// Gets or sets service type.
        /// </summary>
        public Type? ServiceType { get; set; }

        /// <summary>
        /// Gets or sets service implementation type.
        /// </summary>
        public Type? ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets service life time. By default scoped.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

        #endregion
    }
}
