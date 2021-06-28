using System;
using System.Diagnostics;

namespace CAS.TFS.Model.Query
{
    /// <summary>
    ///     Class QueryInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Name,nq}")]
    public class QueryInfo
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public string Scope { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the wiql XML.
        /// </summary>
        /// <value>The wiql XML.</value>
        public WorkItemQuery Wiql { get; set; }
    }
}