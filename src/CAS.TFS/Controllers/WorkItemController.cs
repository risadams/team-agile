// <copyright file="WorkItemController.cs" company="Philips Respironics">
// © 2014 Koninklijke Philips N.V.
// </copyright>
// <author>Adams, Chris</author>

#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CAS.TFS.Model;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.WorkItems;
using CAS.Common;
using CAS.Common.Attributes;
using CAS.TFS.Util;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    ///     Class WorkItemController.
    /// </summary>
    public class WorkItemController : TeamController, IWorkItemController
    {
        /// <summary>
        ///     The date time format
        /// </summary>
        public const string DateTimeFormat = @"dd-MMM-yyyy";

        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public WorkItemController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the work item reference.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>WorkItem.</returns>
        public WorkItem GetWorkItemReference(int id)
        {
            return WorkItemStore.GetWorkItem(id);
        }

        /// <summary>
        ///     Gets the work item type reference.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="workItemType">Type of the work item.</param>
        /// <returns>WorkItemType.</returns>
        public WorkItemType GetWorkItemTypeReference(string project, string workItemType)
        {
            return WorkItemStore.Projects[project].WorkItemTypes[workItemType];
        }

        /// <summary>
        ///     Gets the open defects monitored by DRB.
        /// </summary>
        /// <returns>WorkItemInfoCollection.</returns>
        public WorkItemInfoCollection GetOpenDefectsMonitoredByDrb()
        {
            var collection = new WorkItemInfoCollection();

            var query = string.Format(CultureInfo.CurrentCulture, "SELECT * FROM WorkItems WHERE [Work Item Type]='Bug' AND [State] <> 'Closed' AND [Team Project] in ({0}) AND [Custom.DRBMonitored] = 'true'",
                Server.TeamProject.SplitAndWrapQuotes());
            var items = WorkItemStore.Query(query);

            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Gets the bugmaster assigned items.
        /// </summary>
        /// <returns>WorkItemInfoCollection.</returns>
        public WorkItemInfoCollection GetBugmasterAssignedItems()
        {
            var collection = new WorkItemInfoCollection();

            var query = string.Format(CultureInfo.CurrentCulture, "SELECT * FROM WorkItems WHERE [Work Item Type]='Bug' AND [State] <> 'Closed' AND [Team Project] in ({0}) AND [System.AssignedTo] = 'Bugmaster, Encore'",
                Server.TeamProject.SplitAndWrapQuotes());
            var items = WorkItemStore.Query(query);

            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Gets the work items assigned to user.
        /// </summary>
        /// <param name="tfsWorkItemType">Type of the TFS work item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        public WorkItemInfoCollection GetWorkItemsAssignedToUser(TfsWorkItemType tfsWorkItemType, string userName)
        {
            var query = string.Format(CultureInfo.CurrentCulture,
                "SELECT * FROM WorkItems WHERE [Work Item Type]='{1}' AND [State] <> 'Closed' AND [State] <> 'Done' AND [State] <> 'Removed' AND [Team Project] in ({0}) AND [System.AssignedTo] = '{2}'",
                Server.TeamProject.SplitAndWrapQuotes(), tfsWorkItemType.AssociatedText(), userName);

            var items = WorkItemStore.Query(query);
            var infoItems = from WorkItem item in items
                            select new WorkItemInfo(item, Server.ServerUri);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(infoItems);

            return collection;
        }

        /// <summary>
        ///     Queries the open defects for the server's specified team project.
        /// </summary>
        /// <returns>a list of all non-closed defects for the specified team project</returns>
        public WorkItemInfoCollection QueryOpenDefectsForTeamProject()
        {
            var query = string.Format(CultureInfo.CurrentCulture, "SELECT * FROM WorkItems WHERE [Work Item Type]='Bug' AND [State] <> 'Closed' AND [Team Project] in ({0})", Server.TeamProject.SplitAndWrapQuotes());
            var items = WorkItemStore.Query(query);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Queries the defects detected on the specified date for the server's specified team project.
        /// </summary>
        /// <param name="date">The specified date.</param>
        /// <returns>A list of all defects in the specified team project detected on the specified date</returns>
        public WorkItemInfoCollection QueryDefectsDetectedOnDateForTeamProject(DateTime date)
        {
            var query = string.Format(CultureInfo.CurrentCulture, "SELECT * FROM WorkItems WHERE [Work Item Type]='Bug' AND [Created Date] == '{1}' AND [Team Project] in ({0})", Server.TeamProject.SplitAndWrapQuotes(),
                date.FormatDateTimeInvariant());
            var items = WorkItemStore.Query(query);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Gets the work items associated with the specified changeset.
        /// </summary>
        /// <param name="changesets">The changeset list for which to retrieve work items.</param>
        /// <returns>A list of all work items associated to the specified changeset.</returns>
        /// <remarks>
        ///     This method is identical to GetWorkItemsAssociatedWithChangesetLight(IEnumerable{Changeset})
        ///     except it returns a full TFS WorkItem instead of a lightweight wrapper
        /// </remarks>
        public WorkItemInfoCollection GetWorkItemsAssociatedWithChangeset(IEnumerable<Changeset> changesets)
        {
            var uniqueList = new HashSet<WorkItemInfo>();
            foreach (var workItemInfo in changesets
                .Select(GetWorkItemsAssociatedWithChangeset)
                .SelectMany(workItems => workItems.Where(workItemInfo => !uniqueList.Contains(workItemInfo, new WorkItemInfoComparer()))))
            {
                uniqueList.Add(workItemInfo);
            }

            var collection = new WorkItemInfoCollection();
            collection.AddRange(uniqueList);
            return collection;
        }

        /// <summary>
        ///     Gets the work items associated with the specified changeset.
        /// </summary>
        /// <param name="changeset">The changeset  for which to retrieve work items.</param>
        /// <returns>A list of all work items associated to the specified changeset.</returns>
        /// <remarks>
        ///     This method is identical to GetWorkItemsAssociatedWithChangesetLight(Changeset) except
        ///     except it returns a full TFS WorkItem instead of a lightweight wrapper
        /// </remarks>
        public WorkItemInfoCollection GetWorkItemsAssociatedWithChangeset(Changeset changeset)
        {
            var collection = new WorkItemInfoCollection();
            collection.AddRange(changeset.WorkItems.Select(workItem => new WorkItemInfo(workItem, Server.ServerUri)));
            return collection;
        }

        /// <summary>
        ///     Creates the a new work item of the specified type, within the specified project.
        /// </summary>
        /// <param name="type">The work item type.</param>
        /// <param name="project">The project in which to create the item.</param>
        /// <returns>the Work Item object reference.</returns>
        public WorkItem CreateItem(string type, string project)
        {
            var teamProject = WorkItemStore.Projects[project];
            var itemType = teamProject.WorkItemTypes[type];

            var item = new WorkItem(itemType);
            return item;
        }

        /// <summary>
        ///     Modifies the specified work item to include a versioned link to the specified server path.
        /// </summary>
        /// <param name="workItem">The work item to be linked.</param>
        /// <param name="serverPath">the versioned server path of the item to link.</param>
        public void AddVersionedItemLink(WorkItem workItem, string serverPath)
        {
            var type = WorkItemStore.RegisteredLinkTypes["Source Code File"];
            var linkedFile = VersionControl.GetItem(serverPath, VersionSpec.Latest);

            workItem.Links.Add(new ExternalLink(type, linkedFile.ArtifactUri.AbsoluteUri));
            workItem.Save(SaveFlags.MergeLinks);
        }

        /// <summary>
        ///     Gets a list of available the target releases.
        /// </summary>
        /// <returns>a list of all located target releases.</returns>
        public IEnumerable<TargetRelease> GetTargetReleases()
        {
            var project = WorkItemStore.Projects[Server.TeamProject.SplitAndReturnFirst()];
            var workItemType = project.WorkItemTypes["Feature"];

            var item = new WorkItem(workItemType);

            var releases = new List<TargetRelease>();
            foreach (var field in item.Fields.Cast<Field>().Where(field => field.Name == "TargetRelease"))
            {
                releases.AddRange(
                    from object value in field.AllowedValues
                    where ((string)value) != "Not Reviewed"
                          && ((string)value) != "Future"
                          && ((string)value) != "CCB"
                          && ((string)value) != "1.0"
                          && ((string)value) != "2.0"
                    select TargetRelease.Parse(value.ToString())
                    );
            }
            item.Close();

            return releases;
        }

        /// <summary>
        ///     Queries the items for release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>IEnumerable{WorkItemInfo}.</returns>
        public WorkItemInfoCollection QueryItemsForRelease(TargetRelease targetRelease)
        {
            var query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM WorkItems WHERE [TargetRelease] IN ('{0}') OR [Target Release] IN ('{0}')", targetRelease);
            var items = WorkItemStore.Query(query);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Queries the items for release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>IEnumerable{WorkItemInfo}.</returns>
        public WorkItemInfoCollection QueryItemsForRelease(string targetRelease)
        {
            var query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM WorkItems WHERE [TargetRelease] IN ('{0}') OR [Target Release] IN ('{0}')", targetRelease);
            var items = WorkItemStore.Query(query);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Gets a lightweight work item wrapper for the specified work item id.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>WorkItemLight.</returns>
        public WorkItemInfo GetWorkItem(int id)
        {
            try
            {
                var item = WorkItemStore.GetWorkItem(id);
                return new WorkItemInfo(item);
            }
            catch
            {
                //don't care why we fail here, just return null
                return null;
            }
        }

        ///// <summary>
        /////     Gets the feature integration pipeline for target release.
        ///// </summary>
        ///// <param name="targetRelease">The target release.</param>
        ///// <returns>FeatureWorkItemCollection.</returns>
        //[Obsolete("This method should not be used, it exists only as a reference for hierarchical queries")]
        //public FeatureWorkItemCollection GetFeatureIntegrationPipelineForTargetRelease(TargetRelease targetRelease,object DO_NOT_USE=null)
        //{
        //    var query = string.Format(CultureInfo.InvariantCulture,
        //        "select [System.Id], [System.State], [System.Links.LinkType], [System.WorkItemType], [System.Title], [Custom.TargetRelease], [Custom.TeamBacklog], [System.AssignedTo], [Custom.ReleasePriority] from WorkItemLinks where (Source.[System.WorkItemType] = 'Feature' and Source.[Custom.TargetRelease] = '{0}') and (Target.[System.WorkItemType] = 'Feature Status') order by [System.Id] mode (MayContain)",
        //        targetRelease);
        //    var queryTree = new Query(WorkItemStore, query);

        //    var items = queryTree.RunLinkQuery();


        //    //multi-level queries require two calls, one to get the links and one to get the items for each returned link.
        //    //details at: http://blogs.msdn.com/b/jsocha/archive/2012/02/22/retrieving-tfs-results-from-a-tree-query.aspx
        //    var ids = (from WorkItemLinkInfo info in items select info.TargetId).ToArray();

        //    var detailsWiql = new StringBuilder();
        //    detailsWiql.AppendLine("SELECT");
        //    var first = true;

        //    foreach (FieldDefinition field in queryTree.DisplayFieldList)
        //    {
        //        detailsWiql.Append("    ");
        //        if (!first)
        //            detailsWiql.Append(",");
        //        detailsWiql.AppendLine("[" + field.ReferenceName + "]");
        //        first = false;
        //    }
        //    detailsWiql.AppendLine("FROM WorkItems");

        //    var flatQuery = new Query(WorkItemStore, detailsWiql.ToString(), ids);
        //    var details = flatQuery.RunQuery();

        //    //based on convention there should be one feature and one task for each feature in the release. merge the items into the FeatureWorkItem.
        //    var features = from WorkItem item in details
        //                   where item.Type.Name == "Feature" && item.State != "Removed"
        //                   select new FeatureWorkItem
        //                   {
        //                       Id = item.Id,
        //                       Title = item.Title,
        //                       ReleasePriority = item.GetProperty("Release Priority", 3),
        //                       TeamBacklog = item.GetProperty("Team Backlog", "Unclassified"),
        //                       State = item.State,
        //                       TargetRelease = targetRelease //pull from paramater instead of item, it should be the same anyway
        //                   };

        //    var collection = new FeatureWorkItemCollection();
        //    foreach (var feature in features)
        //    {
        //        var currentFeature = feature;
        //        var associatedTask = (from WorkItem item in details
        //                              where item.Type.Name == "Feature Status"
        //                              // && (item.Fields["Feature ID"].Value.ToString()) == currentFeature.Id.ToString(CultureInfo.InvariantCulture)
        //                              select item).FirstOrDefault();

        //        //only update state if associated task exists.
        //        currentFeature.Status = associatedTask != null ? ConstructFeatureStatus(associatedTask) : null;
        //        collection.Add(currentFeature);
        //    }

        //    return collection;
        //}


        /// <summary>
        ///     Gets the feature associated with work items.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        /// <returns>FeatureWorkItemCollection.</returns>
        public WorkItemInfoCollection GetAssociatedParentWorkItems(WorkItemInfoCollection workItems)
        {
            var collection = new WorkItemInfoCollection();
            foreach (var item in workItems)
            {
                //1:  get all parent links from work item
                var links = GetParentWorkItems(item.Id).ToList();

                //2: if link is of type feature, add to collection
                foreach (var link in links.Where(link => link.Type.Name == Server.FeatureWorkItemType))
                {
                    collection.AddIfUnique(GetWorkItem(link.Id));
                    break;
                }

                //3: recursivly get grandparent links and include in collection
                var parentLinkCollection = new WorkItemInfoCollection(from l in links select new WorkItemInfo(l,Server.ServerUri));
                var features = GetAssociatedParentWorkItems(parentLinkCollection);
                collection.AddIfUnique(features);
            }

            return collection;
        }

        /// <summary>
        ///     Gets the work item revision history.
        /// </summary>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns>Revision.</returns>
        public IEnumerable<Revision> GetWorkItemRevisionHistory(int workItemId)
        {
            var item = WorkItemStore.GetWorkItem(workItemId);
            var revisions = item.Revisions;
            return revisions.Cast<Revision>();
        }

        /// <summary>
        ///     Gets the close date.
        /// </summary>
        /// <param name="workItemId">The identifier.</param>
        /// <returns>DefectCloseInfo.</returns>
        public DefectCloseInfo GetWorkItemCloseInfo(int workItemId)
        {
            var closeInfo = new DefectCloseInfo { WorkItemId = workItemId, CloseDate = null, ClosedBy = null };
            var history = GetWorkItemRevisionHistory(workItemId).ToList();
            history.Reverse(); //Reverse so that most recent changes take precedence.
            var curClosed = false;

            //loop through the revision history in reverse order to find the first instance of an item being closed
            //this will remove false-positives from updates to items after closure
            foreach (var revision in history)
            {
                if ((string)revision.Fields[CoreField.State].Value == "Closed")
                {
                    closeInfo = new DefectCloseInfo
                    {
                        WorkItemId = workItemId,
                        CloseDate = (DateTime)revision.Fields[CoreField.ChangedDate].Value,
                        ClosedBy = (string)revision.Fields[CoreField.ChangedBy].Value
                    };
                    curClosed = true;
                }
                else if (curClosed)
                    return closeInfo;
            }
            return new DefectCloseInfo { WorkItemId = workItemId, CloseDate = null, ClosedBy = null };
        }

        /// <summary>
        ///     Gets the feature work item flow map.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="release">The release.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="numPreviousDays">The number of previous days.</param>
        /// <param name="skipDays">The skip days.</param>
        /// <returns>IDictionary&lt;DateTime, FeatureWorkItemCollection&gt;.</returns>
        public IDictionary<DateTime, WorkItemInfoCollection> GetWorkItemFlowMapForProduct(DateTime startDate, TargetRelease release, TfsWorkItemType itemType = TfsWorkItemType.Defect, int numPreviousDays = 90, int skipDays = 1)
        {
            skipDays = Math.Abs(skipDays); //This must be positive, if not just flip it
            if (skipDays == 0)
                skipDays = 1; //min value is 1 day between increments

            var itemFlowMap = new Dictionary<DateTime, WorkItemInfoCollection>();
            const string wiql = @"select * from WorkItems where [System.TeamProject] in ({0}) and [System.WorkItemType] = '{2}' and ([TargetRelease] IN ('{3}') OR [Target Release] IN ('{3}')) asof '{1}'";

            for (var i = 0; i <= numPreviousDays; i += skipDays)
            {
                var date = startDate.AddDays(-i);
                var query = string.Format(CultureInfo.InvariantCulture, wiql, Server.TeamProject.SplitAndWrapQuotes(), date.ToString("M/d/yyyy", CultureInfo.InvariantCulture), itemType.AssociatedText(), release);

                var items = WorkItemStore.Query(query);

                var collection = new WorkItemInfoCollection();
                collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));

                itemFlowMap.Add(date, collection);
            }

            return itemFlowMap;
        }

        /// <summary>
        ///     Gets the open deferred defect list for the specified product.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="asOfDateTime">As of date time.</param>
        /// <returns>IEnumerable{DeferredWorkItemLight}.</returns>
        public WorkItemInfoCollection GetOpenDefectsForProduct(string project, DateTime? asOfDateTime = null)
        {
            var query = string.Format(CultureInfo.InvariantCulture,
                "SELECT [ID],[Title],[Type],[Assigned To],[State],[Target Release],[Deferral Date],[Deferral Justification],[Found In],[Created Date] FROM WorkItems WHERE [Affected Product] IN ('{0}') AND [State] NOT IN ('Closed') AND [Work Item Type] IN ('Bug') AND [Team Project] in ({1}) asof '{2}'",
                project, Server.TeamProject.SplitAndWrapQuotes(), asOfDateTime.HasValue ? asOfDateTime.Value.ToString("M/d/yyyy", CultureInfo.InvariantCulture) : DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture));
            var items = WorkItemStore.Query(query);

            var collection = new WorkItemInfoCollection();
            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        /// Gets the parent work items.
        /// </summary>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns>IEnumerable&lt;WorkItem&gt;.</returns>
        private IEnumerable<WorkItem> GetParentWorkItems(int workItemId)
        {
            var parentLinkType = WorkItemStore.WorkItemLinkTypes.LinkTypeEnds["Parent"];
            var item = WorkItemStore.GetWorkItem(workItemId);
            var links = item.WorkItemLinks;
            return from WorkItemLink link in links
                   where link.LinkTypeEnd == parentLinkType
                   select WorkItemStore.GetWorkItem(link.TargetId);
        }

        /// <summary>
        ///     Gets the child work item hierarchy and return as a aggregated list.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>IEnumerable&lt;WorkItemInfo&gt;.</returns>
        private IEnumerable<WorkItemInfo> GetChildWorkItems(WorkItem parent)
        {
            foreach (var item in
                from WorkItemLink child in parent.WorkItemLinks
                where child.BaseType == BaseLinkType.WorkItemLink && child.LinkTypeEnd.Name == "Child"
                select WorkItemStore.GetWorkItem(child.TargetId))
            {
                yield return new WorkItemInfo(item, Server.ServerUri);

                //var grandChildren = GetChildWorkItems(item);
                //foreach (var grandChild in grandChildren)
                //{
                //    yield return grandChild;
                //}
            }
        }
    }
}