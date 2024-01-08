using System.ComponentModel.DataAnnotations;
using DataPlus.Web.UI.Components;

namespace DataPlus.Web.UI;

public abstract class AccessorBase
{
    #region Private fields region

    private const int _attributeCapacity = 4;
    private Attribute[] _fixedAttributes = new Attribute[_attributeCapacity];
    private List<Attribute> _otherAttributes = new List<Attribute>();
    private AttributeCollection? _attributeCollection;

    #endregion

    #region Public constructors region

    public AccessorBase(string name)
        : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    #endregion

    #region Public methods region

    public bool HasAttribute(Type attributeType) => Attributes.Contains(attributeType);

    #endregion

    #region Protected fields region

    protected const int CustomAttributeStartIndex = 8;

    #endregion

    #region Public properties region

    public AttributeCollection Attributes => _attributeCollection ??= new(_fixedAttributes.Where(a => a != null).Concat(_otherAttributes).ToArray());

    public virtual string Name { get; }

    public virtual string? GroupName => DisplayAttribute?.GetGroupName();

    public virtual string? DisplayName => DisplayAttribute?.GetName();

    public virtual string? Description => DisplayAttribute?.GetDescription();

    public virtual string? Prompt => DisplayAttribute?.GetPrompt();

    public virtual int? Order => DisplayAttribute?.GetOrder();

    public virtual string? ShortName => DisplayAttribute?.GetShortName();

    public virtual DataType? DataType => DataTypeAttribute?.DataType;

    public virtual string? CustomDataType => DataTypeAttribute?.CustomDataType;

    public virtual ValidationMessage? ValidationMessages => ValidationsAttribute?.Messages;

    public virtual string? ValidationSuccessMessage => ValidationsAttribute?.SuccessMessage;

    public virtual string? ValidationRequiementMessage => ValidationsAttribute?.RequirementMessage;

    public virtual bool? AutoValidate => ValidationsAttribute?.AutoValidate;

    public virtual bool? IsValidationMessageAsToolTip => ValidationsAttribute?.IsToolTip;

    public virtual FragmentInvoker? DescriptionFragment => DescriptionAttribute?.GetFragment(this);

    public virtual bool? GridHideColumns => GridAttribute?.GetHideColumns();

    public virtual FragmentInvoker? RenderAsFragment => RenderAsAttribute?.GetFragment(this);

    public virtual string? ElementName => ElementAttribute?.Name;

    public virtual string? ElementClass => ElementAttribute?.Class;

    public virtual string? ElementStyle => ElementAttribute?.Style;

    public virtual string? DisplayFormatNullableText => DisplayFormatAttribute?.GetNullDisplayText();

    public virtual string? DisplayFormatString => DisplayFormatAttribute?.DataFormatString;

    public virtual bool? DisplayFormatEmptyToNull => DisplayFormatAttribute?.ConvertEmptyStringToNull;

    public virtual bool? DisplayFormatApplyInEdit => DisplayFormatAttribute?.ApplyFormatInEditMode;

    #endregion

    #region Protected methods region

    protected Attribute? GetAttribute(int index)
    {
        if (index < _fixedAttributes.Length)
            return _fixedAttributes[index];
        return null;
    }

    protected void SetAttribute(int index, Attribute attribute)
    {
        if (index == -1)
            _otherAttributes.Add(attribute);
        else
        {
            if (index >= _fixedAttributes.Length)
                Array.Resize(ref _fixedAttributes, (index / _attributeCapacity + 1) * _attributeCapacity);
            else if (_fixedAttributes[index] != null)
                throw new InvalidOperationException($"An attrabute for index {index} already has set.");
            _fixedAttributes[index] = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }
    }

    #endregion

    #region Protected properties region

    protected DisplayAttribute? DisplayAttribute { get => (DisplayAttribute?)GetAttribute(0); set => SetAttribute(0, value!); }

    protected DataTypeAttribute? DataTypeAttribute { get => (DataTypeAttribute?)GetAttribute(1); set => SetAttribute(1, value!); }

    protected ValidationsAttribute? ValidationsAttribute { get => (ValidationsAttribute?)GetAttribute(2); set => SetAttribute(2, value!); }

    protected DescriptionAttribute? DescriptionAttribute { get => (DescriptionAttribute?)GetAttribute(3); set => SetAttribute(3, value!); }

    protected RenderAsAttribute? RenderAsAttribute { get => (RenderAsAttribute?)GetAttribute(4); set => SetAttribute(4, value!); }

    protected GridAttribute? GridAttribute { get => (GridAttribute?)GetAttribute(5); set => SetAttribute(5, value!); }

    protected ElementAttribute? ElementAttribute { get => (ElementAttribute?)GetAttribute(6); set => SetAttribute(6, value!); }

    protected DisplayFormatAttribute? DisplayFormatAttribute { get => (DisplayFormatAttribute?)GetAttribute(7); set => SetAttribute(7, value!); }

    #endregion
}
