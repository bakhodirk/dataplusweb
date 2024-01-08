using System;

namespace DataPlus.Web.UI
{
    public interface IActionArgumentValueProvider
    {
        #region Methods region

        bool TryGetValue(int index, Type type, out object? value);

        #endregion
    }
}
