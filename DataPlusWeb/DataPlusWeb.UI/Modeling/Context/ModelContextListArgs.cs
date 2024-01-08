using System;

namespace DataPlus.Web.UI
{
    public class ModelContextListArgs : EventArgs
    {
        #region Public properties region

        /// <summary>
        /// Gets or sets filters
        /// </summary>
        public object? Filter { get; set; }

        /// <summary>
        /// Gets offset of the list result. Available for virtualize mode.
        /// </summary>
        public int? Offset { get; }

        /// <summary>
        /// Gets count of the list result. Available for virtualize mode.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets total count of the list.
        /// </summary>
        public int? ResultCount { get; set; }

        #endregion
    }
}
