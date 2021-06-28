using System;
using System.Collections.Generic;
using CAS.TFS.Model;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.WorkItems;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS
{
    /// <summary>
    /// Interface IWorkItemController
    /// </summary>
    public interface IWorkItemController : ITeamController
    {
        /// <summary>
        ///     Queries the open defects for the server's specified team project.
        /// </summary>
        /// <returns>a list of all non-closed defects for the specified team project</returns>
        WorkItemInfoCollection QueryOpenDefectsForTeamProject();

        /// <summary>
        ///     Queries the defects detected on the specified date for the server's specified team project.
        /// </summary>
        /// <param name="date">The specified date.</param>
        /// <returns>A list of all defects in the specified team project detected on the specified date</returns>
        WorkItemInfoCollection QueryDefectsDetectedOnDateForTeamProject(DateTime date);

        /// <summary>
        ///     Gets the work items associated with the specified changeset.
        /// </summary>
        /// <remarks>
        ///     This method is identical to GetWorkItemsAssociatedWithChangesetLight(IEnumerable{Changeset})
        ///     except it returns a full TFS WorkItem instead of a lightweight wrapper
        /// </remarks>
        /// <param name="changesets">The changeset list for which to retrieve work items.</param>
        /// <returns>A list of all work items associated to the specified changeset.</returns>
        WorkItemInfoCollection GetWorkItemsAssociatedWithChangeset(IEnumerable<Changeset> changesets);

        /// <summary>
        ///     Gets the work items associated with the specified changeset.
        /// </summary>
        /// <remarks>
        ///     This method is identical to GetWorkItemsAssociatedWithChangesetLight(Changeset) except
        ///     except it returns a full TFS WorkItem instead of a lightweight wrapper
        /// </remarks>
        /// <param name="changeset">The changeset  for which to retrieve work items.</param>
        /// <returns>A list of all work items associated to the specified changeset.</returns>
        WorkItemInfoCollection GetWorkItemsAssociatedWithChangeset(Changeset changeset);

        /// <summary>
        ///     Creates the a new work item of the specified type, within the specified project.
        /// </summary>
        /// <param name="type">The work item type.</param>
        /// <param name="project">The project in which to create the item.</param>
        /// <returns>the Work Item object reference.</returns>
        WorkItem CreateItem(string type, string project);

        /// <summary>
        /// Gets the associated parent work items.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        /// <returns>WorkItemCollection.</returns>
        WorkItemInfoCollection GetAssociatedParentWorkItems(WorkItemInfoCollection workItems);

        /// <summary>
        ///     Modifies the specified work item to include a versioned link to the specified server path.
        /// </summary>
        /// <param name="workItem">The work item to be linked.</param>
        /// <param name="serverPath">the versioned server path of the item to link.</param>
        void AddVersionedItemLink(WorkItem workItem, string serverPath);

        /// <summary>
        ///     Gets a list of available the target releases.
        /// </summary>
        /// <returns>a list of all located target releases.</returns>
        IEnumerable<TargetRelease> GetTargetReleases();

        /// <summary>
        ///     Queries the items for release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>IEnumerable{WorkItemInfo}.</returns>
        WorkItemInfoCollection QueryItemsForRelease(TargetRelease targetRelease);

        /// <summary>
        ///     Queries the items for release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>IEnumerable{WorkItemInfo}.</returns>
        WorkItemInfoCollection QueryItemsForRelease(string targetRelease);

        /// <summary>
        ///     Gets a lightweight work item wrapper for the specified work item id.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>WorkItemLight.</returns>
        WorkItemInfo GetWorkItem(int id);

        /// <summary>
        ///     Gets the open deferred defect list for the specified product.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="asOfDateTime">As of date time.</param>
        /// <returns>IEnumerable{DeferredWorkItemLight}.</returns>
        WorkItemInfoCollection GetOpenDefectsForProduct(string project, DateTime? asOfDateTime = null);

        /// <summary>
        ///     Gets the work item revision history.
        /// </summary>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns>Revision.</returns>
        IEnumerable<Revision> GetWorkItemRevisionHistory(int workItemId);

        /// <summary>
        ///     Gets the work item close date.
        /// </summary>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        DefectCloseInfo GetWorkItemCloseInfo(int workItemId);

        /// <summary>
        ///     Gets the feature work item flow map.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="numPreviousDays">The number of previous days.</param>
        /// <param name="skipDays">The skip days.</param>
        /// <param name="release">The release.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>IDictionary&lt;DateTime, WorkItemInfoCollection&gt;.</returns>
        IDictionary<DateTime, WorkItemInfoCollection> GetWorkItemFlowMapForProduct(DateTime startDate, TargetRelease release, TfsWorkItemType itemType, int numPreviousDays, int skipDays);

        /// <summary>
        ///     Gets the work item reference.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>WorkItem.</returns>
        WorkItem GetWorkItemReference(int id);

        /// <summary>
        ///     Gets the work item type reference.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="workItemType">Type of the work item.</param>
        /// <returns>WorkItemType.</returns>
        WorkItemType GetWorkItemTypeReference(string project, string workItemType);

        /// <summary>
        ///     Gets the open defects monitored by DRB.
        /// </summary>
        /// <returns>WorkItemInfoCollection.</returns>
        WorkItemInfoCollection GetOpenDefectsMonitoredByDrb();

        /// <summary>
        ///     Gets the bugmaster assigned items.
        /// </summary>
        /// <returns>WorkItemInfoCollection.</returns>
        WorkItemInfoCollection GetBugmasterAssignedItems();

        /// <summary>
        ///     Gets the work items assigned to user.
        /// </summary>
        /// <param name="tfsWorkItemType">Type of the TFS work item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        WorkItemInfoCollection GetWorkItemsAssignedToUser(TfsWorkItemType tfsWorkItemType, string userName);
    }
}