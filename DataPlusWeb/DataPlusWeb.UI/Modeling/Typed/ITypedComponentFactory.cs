using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DataPlus.Web.UI
{
    public interface ITypedComponentFactory
    {
        #region Methods region

        bool ResolveType(Type[] baseTypes, Type[]? types, DataType? dataType, string? customDataType, Assembly? caller, out Type componentType, out bool isGenericDefinition, out Type[]? genericTypes, out object?[]? parameters);

        #endregion
    }
}
