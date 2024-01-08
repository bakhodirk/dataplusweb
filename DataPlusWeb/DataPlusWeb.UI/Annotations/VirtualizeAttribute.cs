
namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class VirtualizeAttribute : Attribute
    {
        #region Private fields region

        private bool? _enabled = true;

        #endregion

        #region Public methods region

        public bool? GetEnabled() => _enabled;

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value which indicating whether should source shoudl be virtualized.
        /// By default, enabled is true.
        /// </summary>
        public bool Enabled { get => _enabled ?? false; set => _enabled = value; }

        #endregion
    }
}
