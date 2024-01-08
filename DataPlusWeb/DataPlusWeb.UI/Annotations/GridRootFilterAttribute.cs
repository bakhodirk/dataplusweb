using DataPlus.Web.UI;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class GridRootFilterAttribute : Attribute
    {
        #region Private fields region

        private bool? _counts;

        #endregion

        #region Internal methods region

        internal void InitDefaultCounts(Type type, string declatedCountMethod = nameof(IModelContext.ListCountAsync))
        {
            if (_counts == null && type.IsAssignableTo(typeof(IModelContext)) == true)
                _counts = type.GetMethod(declatedCountMethod, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance) != null;
        }

        #endregion

        #region Public methods region

        public bool? GetCounts() => _counts;

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets or set a <see cref="Boolean"/> value which indicating whther load and show counts in root filter content.
        /// </summary>
        public bool Counts { get => _counts ?? false; set => _counts = value; }

        #endregion
    }
}
