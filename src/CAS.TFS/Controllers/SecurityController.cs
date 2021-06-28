using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class SecurityController.
    /// </summary>
    public class SecurityController : TeamController, ISecurityController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public SecurityController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the member groups.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public IList<string> GetMemberGroups(string userName)
        {
            try
            {
                var id = IdentityManagement.ReadIdentity(IdentitySearchFactor.AccountName, userName, MembershipQuery.Direct, ReadIdentityOptions.None);
                if (id == null)
                    return new List<string>();

                return (from memberGroupId
                    in id.MemberOf
                        select GetGroup(memberGroupId)
                            into @group
                            select @group.DisplayName).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        ///     Gets the group Identity.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>TeamFoundationIdentity.</returns>
        private TeamFoundationIdentity GetGroup(IdentityDescriptor identifier)
        {
            return IdentityManagement.ReadIdentity(identifier, MembershipQuery.Direct, ReadIdentityOptions.None);
        }
    }
}