using System;

namespace DataPlus.Web.UI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TypedForAttribute : Attribute
    {
        #region Public constructors region

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="types">value type to bind with typed component.</param>
        public TypedForAttribute(params Type[] types)
            : base()
        {
            if ((types ?? throw new ArgumentNullException(nameof(types))).Length == 0)
                throw new ArgumentException($"Argument {nameof(types)} cannot be empty.", nameof(types));
            Types = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="types">value type to bind with typed component.</param>
        /// <param name="parameters">Additional parameters.</param>
        public TypedForAttribute(Type[] types, params object?[] parameters)
            : this(types)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="dataType">data type to bind with typed component.</param>
        public TypedForAttribute(System.ComponentModel.DataAnnotations.DataType dataType)
            : base()
        {
            if (dataType == System.ComponentModel.DataAnnotations.DataType.Custom)
                throw new ArgumentException($"{nameof(dataType)} cannot be '{dataType}'.", nameof(dataType));
            DataType = dataType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="dataType">data type to bind with typed component.</param>
        /// <param name="parameters">Additional parameters.</param>
        public TypedForAttribute(System.ComponentModel.DataAnnotations.DataType dataType, params object?[] parameters)
            : this(dataType)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="customDataType">custom data type to bind with typed component.</param>
        public TypedForAttribute(string customDataType)
            : base()
        {
            CustomDataType = customDataType ?? throw new ArgumentNullException(nameof(customDataType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedForAttribute"/> class.
        /// </summary>
        /// <param name="customDataType">custom data type to bind with typed component.</param>
        /// <param name="parameters">Additional parameters.</param>
        public TypedForAttribute(string customDataType, params object?[] parameters)
            : this(customDataType)
        {
            Parameters = parameters;
        }

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets value type.
        /// </summary>
        public Type[]? Types { get; }

        /// <summary>
        /// Gets data type.
        /// </summary>
        public System.ComponentModel.DataAnnotations.DataType? DataType { get; }

        /// <summary>
        /// Gets custom data type.
        /// </summary>
        public string? CustomDataType { get; }

        /// <summary>
        /// Gets additional parameters which provide the <see cref="TypedComponent{T}.Parameters"/> property.
        /// </summary>
        public object?[]? Parameters { get; }

        /// <summary>
        /// Gets or sets order of the typed component.
        /// </summary>
        public int Order { get; set; }

        #endregion
    }
}
