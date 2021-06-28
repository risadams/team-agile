using System.Collections.Generic;

namespace CAS.TFS
{
    /// <summary>
    /// Interface ISecurityController
    /// </summary>
    public interface ISecurityController
    {
        /// <summary>
        ///     Gets the member groups.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        IList<string> GetMemberGroups(string userName);
    }
}