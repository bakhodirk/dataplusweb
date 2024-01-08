
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Validation messages.
    /// </summary>
    [Flags]
    public enum ValidationMessage
    {
        #region Enums

        /// <summary>
        /// Requirement message.
        /// </summary>
        Requirement = 1,
        /// <summary>
        /// Message for success validation.
        /// </summary>
        Success = 2,
        /// <summary>
        /// Message for error validation.
        /// </summary>
        Error = 4

        #endregion
    }
}
