// <copyright file="WorkItemInfoCollection.cs" company="Philips Respironics">
// © 2014 Koninklijke Philips N.V.
// </copyright>

#region References

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace CAS.TFS.Model.WorkItems
{
    /// <summary>
    /// Class WorkItemInfoCollection.
    /// </summary>
    [Serializable]
    public class WorkItemInfoCollection : List<WorkItemInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemInfoCollection"/> class.
        /// </summary>
        public WorkItemInfoCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemInfoCollection"/> class.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        public WorkItemInfoCollection(IEnumerable<WorkItemInfo> workItems)
            : base(workItems)
        {
        }

        /// <summary>
        ///     Gets the total state.
        /// </summary>
        /// <value>The total state.</value>
        public Dictionary<string, int> TotalState
        {
            get
            {
                var dictionary = new Dictionary<string, int>();
                foreach (var item in this)
                {
                    if (dictionary.ContainsKey(item.State))
                        dictionary[item.State]++;
                    else
                        dictionary.Add(item.State, 1);
                }
                return dictionary;
            }
        }

        /// <summary>
        /// Determines whether the specified identifier contains the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if the specified identifier contains identifier; otherwise, <c>false</c>.</returns>
        public bool ContainsId(int id)
        {
            return this.Any(item => item.Id == id);
        }

        /// <summary>
        /// Adds a work item to the collection if it does not already exist.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddIfUnique(WorkItemInfo item)
        {
            if (ContainsId(item.Id))
                return;
            Add(item);
        }

        /// <summary>
        /// Adds a work item to the collection if it does not already exist.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddIfUnique(IEnumerable<WorkItemInfo> items)
        {
            foreach (var item in items)
            {
                AddIfUnique(item);
            }
        }
    }
}