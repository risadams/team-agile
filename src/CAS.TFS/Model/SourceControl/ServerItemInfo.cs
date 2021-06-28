using System;
using System.Diagnostics;
using CAS.Common;

namespace CAS.TFS.Model.SourceControl
{
    /// <summary>
    /// Class ServerItemInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{ChangesetId,nq} - {ServerPath,nq}")]
    public class ServerItemInfo
    {
        /// <summary>
        ///     Gets or sets the server path.
        /// </summary>
        /// <value>The server path.</value>
        public string ServerPath { get; set; }

        /// <summary>
        ///     Gets or sets the changeset unique identifier.
        /// </summary>
        /// <value>The changeset unique identifier.</value>
        public int ChangesetId { get; set; }

        /// <summary>
        ///     Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public string Environment { get; set; }

        /// <summary>
        ///     Gets the server parent location.
        /// </summary>
        /// <value>The server parent location.</value>
        public string ServerParent
        {
            get { return ServerPath.Replace(Name, string.Empty); }
        }

        /// <summary>
        ///     Gets the name of the change script.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return StringUtilities.ParseServerItem(ServerPath); }
        }
    }
}