using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace DataPlus.Web.UI;

public class MemberAccessor : AccessorBase
{
    #region Private fields region

    private readonly Type _valueType;
    private readonly Func<object?, object?>? _getter;
    private readonly Action<object?, object?>? _setter;
    private bool _hasValidation = false;
    private bool _isRequired = false;
    private TypeAccessor? _valueTypeAccessor;
    private bool? _allowNull;

    #endregion

    #region Private methods region

    private static Type GetValueType(MemberInfo memberInfo) => memberInfo switch
    {
        PropertyInfo propertyInfo => propertyInfo.PropertyType,
        FieldInfo fieldInfo => fieldInfo.FieldType,
        _ => typeof(object)
    };

    private void Initialize(MemberInfo memberInfo)
    {
        foreach (Attribute attr in memberInfo.GetCustomAttributes(true))
        {
            switch (attr)
            {
                case DisplayAttribute displayAttr:
                    DisplayAttribute ??= displayAttr;
                    break;

                case DataTypeAttribute dataTypeAttr:
                    DataTypeAttribute ??= dataTypeAttr;
                    break;

                case ValidationsAttribute validationsAttr:
                    ValidationsAttribute ??= validationsAttr;
                    break;

                case ValidationAttribute validationAttr:
                    _hasValidation = true;
                    if (validationAttr is RequiredAttribute)
                        _isRequired = true;
                    break;

                case DescriptionAttribute descriptionAttr:
                    DescriptionAttribute ??= descriptionAttr;
                    break;

                case RenderAsAttribute renderAsAttr:
                    RenderAsAttribute ??= renderAsAttr;
                    break;

                case GridAttribute gridAttr:
                    GridAttribute ??= gridAttr;
                    break;

                case GridColumnAttribute gridColumnAttr:
                    GridColumnAttribute ??= gridColumnAttr;
                    break;

                case GridCellAttribute gridCellAttr:
                    GridCellAttribute ??= gridCellAttr;
                    break;

                case ElementAttribute elementAttr:
                    ElementAttribute ??= elementAttr;
                    break;

                case GridRootFilterAttribute viewFilterAttr:
                    GridRootFilterAttribute ??= viewFilterAttr;
                    GridRootFilterAttribute.InitDefaultCounts(Type.BaseType);
                    break;

                case DisplayFormatAttribute displayFormatAttr:
                    DisplayFormatAttribute ??= displayFormatAttr;
                    break;

                default:
                    SetAttribute(-1, attr);
                    break;
            }
        }

        // DisplayAttribute
        DisplayAttribute ??= new();
        DisplayAttribute.Name ??= memberInfo.Name;

        // ValidationsAttribute
        ValidationsAttribute ??= new ValidationsAttribute();
    }

    private string? FormatValue(object? value) => value is null ? null : Convert.ToString(value);

    private object? ConvertValue(string? formattedValue) => formattedValue is null ? null : Convert.ChangeType(formattedValue, ValueType);

    private bool IsValueAllowNull() => !ValueType.IsValueType || ValueType.IsNullable();

    #endregion

    #region Internal constructors region

    internal MemberAccessor(TypeAccessor type, MemberInfo memberInfo, Func<object?, object?>? getter, Action<object?, object?>? setter)
        : base(memberInfo.Name)
    {
        Type = type;
        _valueType = GetValueType(memberInfo);
        _getter = getter;
        _setter = setter;
        Initialize(memberInfo);
    }

    #endregion

    #region Public methods region

    public object? GetValue() => _getter?.Invoke(null);

    public object? GetValue(object instance) => _getter?.Invoke(instance);

    public void SetValue(object? value) => _setter?.Invoke(null, value);

    public void SetValue(object instance, object? value) => _setter?.Invoke(instance, value);

    public string? GetStringValue() => FormatValue(GetValue());

    public string? GetStringValue(object instance) => FormatValue(GetValue(instance));

    public void SetStringValue(string? formattedValue) => SetValue(ConvertValue(formattedValue));

    public void SetStringValue(object instance, string? formattedValue) => SetValue(instance, ConvertValue(formattedValue));

    public Expression<Func<T>> GetExpression<T>(object? instance)
    {
        var instanceType = instance?.GetType() ?? typeof(object);
        var instanceTyped = Expression.Convert(Expression.Constant(instance), instanceType);
        return Expression.Lambda<Func<T>>(Expression.Property(instanceTyped, Name));
    }

    #endregion

    #region Public properties region

    public TypeAccessor Type { get; }

    public TypeAccessor ValueTypeAccessor => _valueTypeAccessor ??= AccessorHelper.GetTypeAccessor(_valueType);

    public Type ValueType => _valueType;

    public bool CanGet => _getter is not null;

    public bool CanSet => _setter is not null;

    public bool AllowNull => _allowNull ??= IsValueAllowNull();

    public bool HasValidation => _hasValidation;

    public bool IsRequired => _isRequired;

    public bool? GridColumnFilterable => GridColumnAttribute?.GetFilterable();

    public string? GridColumnFilterField => GridColumnAttribute?.FilterField;

    public string? GridCellClass => GridCellAttribute?.Class;

    public string? GridCellStyle => GridCellAttribute?.Style;

    public bool? GridRootFilterCounts => GridRootFilterAttribute?.Counts;

    #endregion

    #region Protected properties region

    protected GridColumnAttribute? GridColumnAttribute { get => (GridColumnAttribute?)GetAttribute(CustomAttributeStartIndex); set => SetAttribute(CustomAttributeStartIndex, value!); }

    protected GridCellAttribute? GridCellAttribute { get => (GridCellAttribute?)GetAttribute(CustomAttributeStartIndex + 1); set => SetAttribute(CustomAttributeStartIndex + 1, value!); }

    protected GridRootFilterAttribute? GridRootFilterAttribute { get => (GridRootFilterAttribute?)GetAttribute(CustomAttributeStartIndex + 2); set => SetAttribute(CustomAttributeStartIndex + 2, value!); }

    #endregion
}
