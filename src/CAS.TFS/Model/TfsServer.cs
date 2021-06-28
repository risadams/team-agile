#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CAS.TFS.Model.NuGet;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

#endregion

namespace CAS.TFS.Model
{
    /// <summary>
    /// Class TfsServer.
    /// </summary>
    [Serializable]
    public abstract class TfsServer : IDisposable
    {
        private static readonly List<string> RegisteredServers = new List<string>();

        /// <summary>
        ///     The source control root identifier
        /// </summary>
        public static string SourceControlRoot = @"$/";

        [NonSerialized]
        private readonly TfsTeamProjectCollection _server;

        /// <summary>
        ///     Prevents a default instance of the <see cref="TfsServer" /> class from being created.
        /// </summary>
        private TfsServer()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TfsServer" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        protected TfsServer(string serverName)
        {
            _server = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(serverName));
            _server.EnsureAuthenticated();
            ServerName = _server.Name;
            ServerUri = _server.Uri.ToString();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TfsServer" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="networkCredential">The network credential.</param>
        protected TfsServer(string serverName, ICredentials networkCredential)
        {
            var basicCredentials = new BasicAuthCredential(networkCredential);
            var tfsCredentials = new TfsClientCredentials(basicCredentials) {AllowInteractive = false};

            _server = new TfsTeamProjectCollection(new Uri(serverName), tfsCredentials);
            _server.Authenticate();

            _server.EnsureAuthenticated();
            ServerName = _server.Name;
            ServerUri = _server.Uri.ToString();
        }

        /// <summary>
        ///     Gets the NuGet feeds.
        /// </summary>
        /// <value>The NuGet feeds.</value>
        public abstract IReadOnlyCollection<NuGetFeed> NuGetFeeds { get; }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public TfsTeamProjectCollection Server
        {
            get { return _server; }
        }

        /// <summary>
        ///     Gets the server URI.
        /// </summary>
        /// <value>The server URI.</value>
        public string ServerUri { get; private set; }

        /// <summary>
        ///     Gets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName { get; private set; }

        /// <summary>
        ///     Gets or sets the team project.
        /// </summary>
        /// <value>The team project.</value>
        public string TeamProject { get; protected set; }

        /// <summary>
        ///     Gets the type of the feature work item.
        /// </summary>
        /// <value>The type of the feature work item.</value>
        public abstract string FeatureWorkItemType { get; }

        /// <summary>
        ///     Gets the type of the defect work item.
        /// </summary>
        /// <value>The type of the defect work item.</value>
        public abstract string DefectWorkItemType { get; }

        /// <summary>
        ///     Gets the type of the product backlog work item.
        /// </summary>
        /// <value>The type of the product backlog work item.</value>
        public abstract string ProductBacklogWorkItemType { get; }

        /// <summary>
        ///     Gets the type of the task work item.
        /// </summary>
        /// <value>The type of the task work item.</value>
        public abstract string TaskWorkItemType { get; }

        /// <summary>
        ///     Gets the name of the workstation.
        /// </summary>
        /// <value>The name of the workstation.</value>
        public string WorkstationName
        {
            get { return Workstation.Current.Name; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
            }
            finally
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     Gets the registered servers.
        /// </summary>
        /// <returns>List{System.String}.</returns>
        public static List<string> GetRegisteredServers()
        {
            var collection = RegisteredTfsConnections.GetProjectCollections();
            if (RegisteredServers.Any())
                return RegisteredServers;

            foreach (var rpc in collection
                .Where(rpc => !rpc.Offline))
                RegisteredServers.Add(rpc.Uri.ToString());

            return RegisteredServers;
        }

        /// <summary>
        ///     Gets the version control server.
        /// </summary>
        /// <returns>VersionControlServer.</returns>
        public VersionControlServer GetVersionControlServer()
        {
            return _server.GetService<VersionControlServer>();
        }

        /// <summary>
        ///     Gets the common structure.
        /// </summary>
        /// <returns>ICommonStructureService.</returns>
        public ICommonStructureService GetCommonStructure()
        {
            return _server.GetService<ICommonStructureService>();
        }

        /// <summary>
        ///     Gets the work item store.
        /// </summary>
        /// <returns>WorkItemStore.</returns>
        public WorkItemStore GetWorkItemStore()
        {
            return _server.GetService<WorkItemStore>();
        }

        /// <summary>
        ///     Gets the authorization service.
        /// </summary>
        /// <returns>IAuthorizationService.</returns>
        public IAuthorizationService GetAuthorizationService()
        {
            return _server.GetService<IAuthorizationService>();
        }

        /// <summary>
        ///     Gets the identity management service.
        /// </summary>
        /// <returns>IIdentityManagementService.</returns>
        public IIdentityManagementService GetIdentityManagementService()
        {
            return _server.GetService<IIdentityManagementService>();
        }

        /// <summary>
        ///     Gets the service of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>``0.</returns>
        public T GetService<T>()
        {
            return _server.GetService<T>();
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
            if (disposing)
            {
                _server.Dispose();
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TfsServer" /> class.
        /// </summary>
        ~TfsServer()
        {
            Dispose(false);
        }
    }
}