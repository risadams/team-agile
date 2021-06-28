using System.Collections.Generic;
using CAS.TFS.Model.NuGet;

namespace CAS.TFS.Model
{
    /// <summary>
    /// Class TfsServerBase.
    /// </summary>
    public class TfsServerBase : TfsServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TfsServerBase"/> class.
        /// </summary>
        /// <param name="serverUrl">The server URL.</param>

        public TfsServerBase(string serverUrl) : base(serverUrl) { }
        /// <summary>
        /// Gets the NuGet feeds.
        /// </summary>
        /// <value>The NuGet feeds.</value>
        public override IReadOnlyCollection<NuGetFeed> NuGetFeeds
        {
            get
            {
                return new List<NuGetFeed>
                {
                    new NuGetFeed {Name = "Nuget.org", Url = "https://www.nuget.org/api/v2/"}
                };
            }
        }
        /// <summary>
        ///     Gets the type of the feature work item.
        /// </summary>
        /// <value>The type of the feature work item.</value>
        public override string FeatureWorkItemType
        {
            get { return "Feature"; }
        }

        /// <summary>
        ///     Gets the type of the defect work item.
        /// </summary>
        /// <value>The type of the defect work item.</value>
        public override string DefectWorkItemType
        {
            get { return "Bug"; }
        }

        /// <summary>
        ///     Gets the type of the product backlog work item.
        /// </summary>
        /// <value>The type of the product backlog work item.</value>
        public override string ProductBacklogWorkItemType
        {
            get { return "Product Backlog Item"; }
        }

        /// <summary>
        ///     Gets the type of the task work item.
        /// </summary>
        /// <value>The type of the task work item.</value>
        public override string TaskWorkItemType
        {
            get { return "Task"; }
        }
    }
}