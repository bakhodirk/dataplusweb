using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataPlus.Web.UI
{
    public static class ModellingHelper
    {
        #region Public methods region

        public static Func<TItem, object?> CreateValueGetter<TItem>(string propertyOrFieldName)
        {
            var item = Expression.Parameter(typeof(TItem), "item");
            var property = AccessorHelper.GetPropertyOrField(item, propertyOrFieldName);
            return Expression.Lambda<Func<TItem, object?>>(Expression.Convert(property, typeof(object)), item).Compile();
        }

        public static PropertyInfo? GetParameterProperty(this Type componentType, Type propertyType, string? propertyName = null)
        {
            if (propertyName is null)
            {
                return componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => (propertyName == null || p.Name == propertyName) && IsParameterProperty(p, propertyType))
                .FirstOrDefault();
            }
            else
            {
                var property = componentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null || !IsParameterProperty(property, propertyType)) return null;
                return property;
            }

            static bool IsParameterProperty(PropertyInfo propertyInfo, Type propertyType)
                => propertyInfo.PropertyType.IsAssignableFrom(propertyType) && propertyInfo.CanWrite && propertyInfo.IsDefined(typeof(ParameterAttribute));
        }

        public static PropertyInfo? GetParameterProperty<TPropertyType>(this Type componentType, string? propertyName = null)
            => GetParameterProperty(componentType, typeof(TPropertyType), propertyName: propertyName);

        public static bool CanShowErrorMessage(this MemberAccessor accessor)
             => accessor.ValidationMessages?.HasFlag(ValidationMessage.Error) ?? false;

        public static bool CanShowSuccessMessage(this MemberAccessor accessor)
             => accessor.ValidationMessages?.HasFlag(ValidationMessage.Success) ?? false && !string.IsNullOrEmpty(accessor.ValidationSuccessMessage);

        public static bool CanShowRequirementMessage(this MemberAccessor accessor)
             => accessor.ValidationMessages?.HasFlag(ValidationMessage.Requirement) ?? false && accessor.IsRequired && !string.IsNullOrEmpty(accessor.ValidationRequiementMessage);

        #endregion
    }
}
