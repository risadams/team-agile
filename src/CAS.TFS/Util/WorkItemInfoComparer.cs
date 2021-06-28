using System.Collections.Generic;
using CAS.TFS.Model.WorkItems;

namespace CAS.TFS.Util
{
    /// <summary>
    /// Class WorkItemInfoComparer.
    /// </summary>
    public class WorkItemInfoComparer : IEqualityComparer<WorkItemInfo>
    {
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="x" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="y" /> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(WorkItemInfo x, WorkItemInfo y)
        {
            return x.Id == y.Id;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(WorkItemInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}