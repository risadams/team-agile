// -----------------------------------------------------------------------
//  <copyright file="DirectoryUserInfo.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.DirectoryServices</project>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace CAS.DirectoryServices.Models
{
    /// <summary>
    /// Class DirectoryUserInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DisplayName,nq}")]
    public class DirectoryUserInfo
    {
        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        /// <value> The username. </value>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value> The display name. </value>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the mail.
        /// </summary>
        /// <value> The mail. </value>
        public string Mail { get; set; }

        /// <summary>
        ///     Gets or sets the department.
        /// </summary>
        /// <value> The department. </value>
        public string Department { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value> The title. </value>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the telephone.
        /// </summary>
        /// <value>The telephone.</value>
        public string Telephone { get; set; }

        /// <summary>
        ///     Gets the telephone link.
        /// </summary>
        /// <value>The telephone link.</value>
        public string TelephoneLink
        {
            get
            {
                if (!string.IsNullOrEmpty(Telephone))
                {
                    return Telephone
                        .Replace("+", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace("x", string.Empty);
                }
                return Telephone;
            }
        }

        /// <summary>
        ///     Gets or sets the manager identifier.
        /// </summary>
        /// <value>The manager identifier.</value>
        public string ManagerId { get; set; }

        /// <summary>
        ///     Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public DirectoryUserInfo Manager { get; set; }
    }
}
