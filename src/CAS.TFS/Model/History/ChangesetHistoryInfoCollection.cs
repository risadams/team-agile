using System;
using System.Collections.Generic;
using System.Linq;
using CAS.Common;
using CAS.TFS.Model.WorkItems;
using CAS.TFS.Util;

namespace CAS.TFS.Model.History
{
    /// <summary>
    /// Class ChangesetHistoryInfoCollection.
    /// </summary>
    [Serializable]
    public class ChangesetHistoryInfoCollection : List<ChangesetHistoryInfo>
    {
        /// <summary>
        ///     Gets the total changesets grouped by individual author.
        /// </summary>
        /// <value>The total changeset owners.</value>
        public Dictionary<string, int> TotalChangesetOwners
        {
            get
            {
                var dictionary = new Dictionary<string, int>();
                foreach (var artifact in this)
                    dictionary.AddOrIncrement(artifact.Owner);
                return dictionary;
            }
        }

        /// <summary>
        ///     Gets the total work items Grouped by type.
        /// </summary>
        /// <value>The total work item types.</value>
        public Dictionary<string, int> TotalWorkItemTypes
        {
            get
            {
                var dictionary = new Dictionary<string, int>();
                foreach (var workItem in this.SelectMany(artifact => artifact.WorkItems))
                    dictionary.AddOrIncrement(workItem.WorkItemType);
                return dictionary;
            }
        }

        /// <summary>
        ///     Gets the work items.
        /// </summary>
        /// <value>The work items.</value>
        public WorkItemInfoCollection AllWorkItems
        {
            get
            {
                var items = new WorkItemInfoCollection();

                foreach (var item in this
                    .Select(cs => cs.WorkItems)
                    .SelectMany(wi => wi.Where(item => !items.Contains(item, new WorkItemInfoComparer()))))
                {
                    items.Add(item);
                }
                return items;
            }
        }
    }
}