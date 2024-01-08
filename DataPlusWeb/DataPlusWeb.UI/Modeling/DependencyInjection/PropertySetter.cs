using System.Reflection;

namespace DataPlus.Web.DependencyInjection;

internal sealed class PropertySetter
{
    #region Private fields region

    private static readonly MethodInfo CallPropertySetterOpenGenericMethod = typeof(PropertySetter).GetMethod(nameof(CallPropertySetter), BindingFlags.Static | BindingFlags.NonPublic)!;
    private readonly Action<object, object> _setterDelegate;
    private readonly Type _propertyType;
    private bool? _allowNull;

    #endregion

    #region Private methods region

    private static void CallPropertySetter<TTarget, TValue>(Action<TTarget, TValue> setter, object target, object value)
        where TTarget : notnull
    {
        setter((TTarget)target, (TValue)value);
    }

    private bool IsAllowNull() => _allowNull ??= !_propertyType.IsValueType || _propertyType.IsNullable();

    #endregion

    #region Public constructors region

    public PropertySetter(Type targetType, PropertyInfo property)
    {
        if (property.SetMethod == null)
            throw new InvalidOperationException($"Cannot provide a value for property '{property.Name}' on type '{targetType.FullName}' because the property has no setter.");
        Delegate target = property.SetMethod!.CreateDelegate(typeof(Action<,>).MakeGenericType(targetType, property.PropertyType));
        MethodInfo methodInfo = CallPropertySetterOpenGenericMethod.MakeGenericMethod(targetType, property.PropertyType);
        _setterDelegate = (Action<object, object>)methodInfo.CreateDelegate(typeof(Action<object, object>), target);
        _propertyType = property.PropertyType;
    }

    #endregion

    #region Public methods region

    public bool CanSet(object? value) => value is null ? IsAllowNull() : _propertyType.IsAssignableFrom(value.GetType());


    public void SetValue(object target, object? value) => _setterDelegate(target, value!);

    #endregion
}
