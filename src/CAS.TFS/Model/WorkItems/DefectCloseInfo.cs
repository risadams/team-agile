using System;
using System.Diagnostics;

namespace CAS.TFS.Model.WorkItems
{
    /// <summary>
    /// Class DefectCloseInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Closed: {IsClosed,nq}")]
    public class DefectCloseInfo
    {
        /// <summary>
        ///     Gets or sets the work item identifier.
        /// </summary>
        /// <value>The work item identifier.</value>
        public int WorkItemId { get; set; }

        /// <summary>
        ///     Gets or sets the close date.
        /// </summary>
        /// <value>The close date.</value>
        public DateTime? CloseDate { get; set; }

        /// <summary>
        ///     Gets or sets the closed by.
        /// </summary>
        /// <value>The closed by.</value>
        public string ClosedBy { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed
        {
            get { return CloseDate.HasValue; }
        }
    }
}