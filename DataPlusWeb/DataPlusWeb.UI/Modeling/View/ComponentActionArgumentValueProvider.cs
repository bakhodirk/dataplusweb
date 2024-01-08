using System;

namespace DataPlus.Web.UI
{
    internal class ComponentActionArgumentValueProvider : IActionArgumentValueProvider
    {
        #region Private fields region

        private readonly IActionArgumentValueProviderContainer? _container;

        #endregion

        #region Public constructors region

        public ComponentActionArgumentValueProvider(IActionArgumentValueProviderContainer? container)
            : base()
        {
            _container = container;
        }

        #endregion

        #region Public methods region

        public bool TryGetValue(int index, Type type, out object? value)
        {
            if (_container != null)
            {
                foreach (var provider in _container.GetProviders())
                {
                    if (provider.TryGetValue(index, type, out value))
                        return true;
                }
            }
            value = null;
            return true;
        }

        #endregion
    }
}
