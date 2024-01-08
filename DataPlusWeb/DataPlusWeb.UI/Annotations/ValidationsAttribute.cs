using DataPlus.Web.UI;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ValidationsAttribute : Attribute
    {
        #region Private fields region

        private const ValidationMessage _defaultValidationMessages = ValidationMessage.Error;
        private const bool _defaultAutoValidate = true;
        private const bool _defaultIsToolTip = false;
        private static readonly string _defaultRequimentMessage = SR.ValidationsAttribute_DefaultRequimentMessage;
        private static readonly string _defaultSuccessMessage = SR.ValidationsAttribute_DefaultSuccessMessage;

        private string? _requirementMessage;
        private ValidationMessage? _validationMessage;
        private string? _validationSuccessMessage;
        private bool? _autoValidate;
        private bool? _isToolTip;

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or set validation messages to display.
        /// </summary>
        public ValidationMessage Messages
        {
            get => _validationMessage ?? _defaultValidationMessages;
            set => _validationMessage = value;
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value which indicating whether should display requirement message.
        /// </summary>
        public string RequirementMessage
        {
            get => _requirementMessage ?? _defaultRequimentMessage;
            set => _requirementMessage = value;
        }

        /// <summary>
        /// Gets or sets validation success message.
        /// </summary>
        public string SuccessMessage
        {
            get => _validationSuccessMessage ?? _defaultSuccessMessage;
            set => _validationSuccessMessage = value;
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value which indicating whether the validation will occurs automaticly.
        /// </summary>
        public bool AutoValidate
        {
            get => _autoValidate ?? _defaultAutoValidate;
            set => _autoValidate = value;
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value which indicating whether the validation message displays as tooltip message.
        /// </summary>
        public bool IsToolTip
        {
            get => _isToolTip ?? _defaultIsToolTip;
            set => _isToolTip = value;
        }

        #endregion
    }
}
