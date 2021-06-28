// -----------------------------------------------------------------------
//  <copyright file="IDirectoryController.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.DirectoryServices</project>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CAS.DirectoryServices.Models;

namespace CAS.DirectoryServices
{
    /// <summary>
    /// Interface IDirectoryController
    /// </summary>
    public interface IDirectoryController
    {
        /// <summary>
        ///     Searches all users based on the specified search query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="getManager">if set to <c>true</c> [get manager].</param>
        /// <returns>IEnumerable&lt;DirectoryUserInfo&gt;.</returns>
        IEnumerable<DirectoryUserInfo> SearchUsers(string query, bool getManager = false);

        /// <summary>
        ///     Gets the user information for the specified user.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <param name="getManager">if set to <c>true</c> [get manager].</param>
        /// <returns>DirectoryUserInfo.</returns>
        DirectoryUserInfo GetUserInfo(string userName, bool getManager = true);
    }
}
