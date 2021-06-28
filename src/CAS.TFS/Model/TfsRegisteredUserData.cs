#region References

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

#endregion

namespace CAS.TFS.Model
{
    /// <summary>
    /// Class TfsRegisteredUserData.
    /// </summary>
    [Serializable]
    public class TfsRegisteredUserData : IDisposable
    {
        /// <summary>
        ///     The column identifier
        /// </summary>
        public const string ColumnId = @"Id";

        /// <summary>
        ///     The column name
        /// </summary>
        public const string ColumnName = @"Name";

        /// <summary>
        ///     The column mail
        /// </summary>
        public const string ColumnMail = @"Mail";

        /// <summary>
        ///     The _identity management
        /// </summary>
        [NonSerialized]
        private readonly IIdentityManagementService _identityManagement;

        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TfsRegisteredUserData" /> class.
        /// </summary>
        /// <param name="groupSecurityService">The group security service.</param>
        public TfsRegisteredUserData(IIdentityManagementService groupSecurityService)
        {
            UserDataTable = CreateTable();

            _identityManagement = groupSecurityService;
            Initialized = new EventWaitHandle(true, EventResetMode.ManualReset);
            UserCount = 0;
        }

        /// <summary>
        ///     Gets or sets the user data table.
        /// </summary>
        /// <value>The user data table.</value>
        protected DataTable UserDataTable { get; set; }

        /// <summary>
        ///     Gets the users table.
        /// </summary>
        /// <value>The users table.</value>
        public DataTable UsersTable
        {
            get
            {
                lock (UserDataTable)
                    return UserDataTable.Copy();
            }
        }

        /// <summary>
        ///     Gets or sets the user count.
        /// </summary>
        /// <value>The user count.</value>
        public int UserCount { get; protected set; }

        /// <summary>
        ///     Gets the initialized indicator.
        /// </summary>
        /// <value>The initialized.</value>
        public EventWaitHandle Initialized { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TfsRegisteredUserData" /> class.
        /// </summary>
        ~TfsRegisteredUserData()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Creates the table.
        /// </summary>
        /// <returns>DataTable.</returns>
        public static DataTable CreateTable()
        {
            var table = new DataTable();
            table.Columns.Add(ColumnId);
            table.Columns.Add(ColumnName);
            table.Columns.Add(ColumnMail);
            table.Constraints.Add("pk", table.Columns[ColumnId], true);
            return table;
        }

        /// <summary>
        ///     Retrieves the user list from the TFS server.
        /// </summary>
        public virtual void RetrieveUsers()
        {
            Initialized.Reset();

            UserCount = 0;

            var users = UserDataTable.Clone();
            var foundationIdentity = _identityManagement.ReadIdentity(GroupWellKnownDescriptors.EveryoneGroup, MembershipQuery.Expanded, ReadIdentityOptions.None);
            if (foundationIdentity.Members.Length > 100)
            {
                var srcIndex = 0;
                while (srcIndex < foundationIdentity.Members.Length)
                {
                    var len = 100;
                    if (srcIndex + 100 > foundationIdentity.Members.Length)
                        len = foundationIdentity.Members.Length % 100;

                    var descriptors = new IdentityDescriptor[len];
                    Array.Copy(foundationIdentity.Members, srcIndex, descriptors, 0, len);
                    var validUsers = _identityManagement.ReadIdentities(descriptors, MembershipQuery.Direct, ReadIdentityOptions.None);
                    AddUsersToTable(users, validUsers);
                    srcIndex += 100;
                }
            }
            else
            {
                var validUsers = _identityManagement.ReadIdentities(foundationIdentity.Members, MembershipQuery.Direct, ReadIdentityOptions.None);
                AddUsersToTable(users, validUsers);
            }

            lock (UserDataTable)
                UserDataTable = users;

            Initialized.Set();
        }

        /// <summary>
        ///     Adds the users to the internal data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="validUsers">The valid users.</param>
        private void AddUsersToTable(DataTable table, IEnumerable<TeamFoundationIdentity> validUsers)
        {
            foreach (var identity in validUsers)
            {
                if (identity == null || (identity.Descriptor.IdentityType != IdentityConstants.WindowsType &&
                                         identity.Descriptor.IdentityType != IdentityConstants.ClaimsType) ||
                    identity.IsContainer)
                    continue;
                var row = table.NewRow();
                row[ColumnId] = (identity.GetAttribute("Domain", string.Empty) + "\\" + identity.GetAttribute("Account", string.Empty));
                row[ColumnName] = identity.DisplayName;
                row[ColumnMail] = identity.GetAttribute("Mail", string.Empty);
                table.Rows.Add(row);
                ++UserCount;
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                    if (UserDataTable != null)
                        UserDataTable.Dispose();
                }


                _disposed = true;
            }
        }
    }
}