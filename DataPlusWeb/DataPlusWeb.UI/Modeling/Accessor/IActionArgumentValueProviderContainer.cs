using System.Collections.Generic;

namespace DataPlus.Web.UI
{
    public interface IActionArgumentValueProviderContainer
    {
        #region Methods region

        void Register(IActionArgumentValueProvider provider);

        IEnumerable<IActionArgumentValueProvider> GetProviders();

        #endregion
    }
}
