using System;
using System.Diagnostics;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CAS.TFS.Model.SourceControl
{
    /// <summary>
    /// Class BranchInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Path,nq}")]
    public class BranchInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BranchInfo" /> class.
        /// </summary>
        public BranchInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BranchInfo" /> class.
        /// </summary>
        /// <param name="branchObject">The branch object.</param>
        public BranchInfo(BranchObject branchObject)
        {
            Path = branchObject.Properties.RootItem.Item;
            CreationDate = branchObject.DateCreated;
            IsDeleted = branchObject.Properties.RootItem.IsDeleted;
            Owner = branchObject.Properties.Owner;
            Description = branchObject.Properties.Description;
            Version = branchObject.Properties.RootItem.Version.DisplayString;
        }

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public string Owner { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; set; }
    }
}