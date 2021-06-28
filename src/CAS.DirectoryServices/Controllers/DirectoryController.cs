// -----------------------------------------------------------------------
//  <copyright file="DirectoryController.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.DirectoryServices</project>
// -----------------------------------------------------------------------

#region

using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using CAS.DirectoryServices.Models;

#endregion

namespace CAS.DirectoryServices.Controllers
{
    /// <summary>
    ///     Class DirectoryController.
    /// </summary>
    public class DirectoryController : IDirectoryController
    {
        /// <summary>
        ///     Searches all users based on the specified search query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="getManager">if set to <c>true</c> [get manager].</param>
        /// <returns>IEnumerable&lt;DirectoryUserInfo&gt;.</returns>
        public IEnumerable<DirectoryUserInfo> SearchUsers(string query, bool getManager = false)
        {
            using (var directorySearcher = new DirectorySearcher(string.Empty,string domainPrefix="")
            {
                Filter = string.Format(CultureInfo.InvariantCulture, "(&(anr={0})(objectCategory=person))", query)
            })
            {
                var results = directorySearcher.FindAll();
                foreach (var user in
                    from SearchResult searchResult in results
                    select new DirectoryUserInfo
                    {
                        Username = FetchPropertyValue(searchResult, "name"),
                        DisplayName = FetchPropertyValue(searchResult, "displayname"),
                        Title = FetchPropertyValue(searchResult, "title"),
                        Mail = FetchPropertyValue(searchResult, "userprincipalname"),
                        Department = FetchPropertyValue(searchResult, "department"),
                        Telephone = FetchPropertyValue(searchResult, "telephonenumber"),
                        ManagerId = StripUserFromCn(FetchPropertyValue(searchResult, "manager"))
                    })
                {
                    if (getManager && user.ManagerId != null && user.Username != user.ManagerId)
                    {
                        user.Manager = GetUserInfo(domainPrefix + user.ManagerId, false);
                    }
                    yield return user;
                }
            }
        }

        /// <summary>
        ///     Gets the user information for the specified user.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <param name="getManager">if set to <c>true</c> [get manager].</param>
        /// <returns>DirectoryUserInfo.</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public DirectoryUserInfo GetUserInfo(string userName, bool getManager = true)
        {
            var users = SearchUsers(StripDomain(userName), getManager).ToList();
            if (!users.Any())
                throw new UserNotFoundException(string.Format(CultureInfo.InvariantCulture, "User {0} not found",
                    userName));
            return users.FirstOrDefault();
        }

        /// <summary>
        ///     Strips the domain.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>System.String.</returns>
        private static string StripDomain(string userName)
        {
            var split = userName.Split(new[] {'\\'});
            return split.Length == 0 ? split[0] : split[1];
        }

        /// <summary>
        ///     Strips the user from cn.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <remarks>
        ///     Raw value should be in format of "CN=XXXXXXXX,OU=Users,OU=YYYYYYY,OU=ZZZZZZZ,DC=DCDCDCDC,DC=DCDCDCDC,DC=DCDCDCDC,DC=DCDCDCDC"
        ///     we need to pull the CN value
        /// </remarks>
        /// <returns>System.String.</returns>
        private static string StripUserFromCn(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return null;

            var split = raw.Split(new[] {','});

            if (split.Length < 1)
                return null;
            var splitCn = split.First().Split(new[] {'='});
            return splitCn.Length < 1 ? null : splitCn[1];
        }

        /// <summary>
        ///     Fetches the property value.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        /// <param name="prop">The prop.</param>
        /// <returns></returns>
        private static string FetchPropertyValue(SearchResult searchResult, string prop)
        {
            if (!searchResult.Properties.Contains(prop))
                return string.Empty;

            var resultValues = searchResult.Properties[prop];
            if (resultValues.Count >= 1)
                return searchResult.Properties[prop][0] as string;

            return string.Empty;
        }
    }
}
