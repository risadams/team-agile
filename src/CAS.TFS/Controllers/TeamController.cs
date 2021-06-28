#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CAS.Common;
using CAS.TFS.Model;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Lab.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Field = Microsoft.TeamFoundation.WorkItemTracking.Client.Field;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class TeamController.
    /// </summary>
    public class TeamController : ITeamController
    {
        private IBuildServer _buildServer;
        private LabService _labService;
        private ProjectProcessConfigurationService _processConfigurationService;
        private ISecurityService _securityService;
        private ITestManagementService _testManagementService;
        private VersionControlServer _versionControlServer;
        private WorkItemStore _workItemStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TeamController" /> class using a generic <see cref="TfsServer" />
        ///     instance.
        /// </summary>
        /// <param name="tfsServerUri">The TFS server url.</param>
        public TeamController(string tfsServerUri)
            : this(new TfsServerBase(tfsServerUri))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TeamController" /> class using a specific <see cref="TfsServer" />
        ///     instance.
        /// </summary>
        /// <param name="tfsServer">The TFS server instance.</param>
        public TeamController(TfsServer tfsServer)
        {
            Server = tfsServer;
            IdentityManagement = tfsServer.GetIdentityManagementService();
            CommonStructure = tfsServer.GetCommonStructure();
            Authorization = tfsServer.GetAuthorizationService();
            Users = new TfsRegisteredUserData(IdentityManagement);
        }

        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        protected TeamController(TeamController controller)
        {
            Server = controller.Server;
            IdentityManagement = controller.IdentityManagement;
            CommonStructure = controller.CommonStructure;
            Authorization = controller.Authorization;
            RegistrationService = controller.RegistrationService;
            Users = controller.Users;
        }

        /// <summary>
        ///     Gets the version control server service.
        /// </summary>
        /// <value>The version control server service.</value>
        protected VersionControlServer VersionControl
        {
            get { return _versionControlServer ?? (_versionControlServer = Server.GetService<VersionControlServer>()); }
        }

        /// <summary>
        ///     Gets the work item store service.
        /// </summary>
        /// <value>The work item store service.</value>
        protected WorkItemStore WorkItemStore
        {
            get { return _workItemStore ?? (_workItemStore = Server.GetService<WorkItemStore>()); }
        }

        /// <summary>
        ///     Gets the build server service.
        /// </summary>
        /// <value>The build server service.</value>
        protected IBuildServer BuildServer
        {
            get { return _buildServer ?? (_buildServer = Server.GetService<IBuildServer>()); }
        }

        /// <summary>
        ///     Gets the lab service.
        /// </summary>
        /// <value>The lab service.</value>
        protected LabService LabService
        {
            get { return _labService ?? (_labService = GetLabService()); }
        }

        /// <summary>
        ///     Gets the test management service.
        /// </summary>
        /// <value>The test management service.</value>
        protected ITestManagementService TestManagementService
        {
            get { return _testManagementService ?? (_testManagementService = Server.GetService<ITestManagementService>()); }
        }

        /// <summary>
        ///     Gets the security service.
        /// </summary>
        /// <value>The security service.</value>
        protected ISecurityService SecurityService
        {
            get { return _securityService ?? (_securityService = Server.GetService<ISecurityService>()); }
        }

        /// <summary>
        ///     Gets the project process configuration service.
        /// </summary>
        /// <value>The project process configuration service.</value>
        protected ProjectProcessConfigurationService ProjectProcessConfigurationService
        {
            get { return _processConfigurationService ?? (_processConfigurationService = Server.GetService<ProjectProcessConfigurationService>()); }
        }

        /// <summary>
        ///     Gets the identity management service.
        /// </summary>
        /// <value>The identity management.</value>
        protected IIdentityManagementService IdentityManagement { get; private set; }

        /// <summary>
        ///     Gets the common structure service.
        /// </summary>
        /// <value>The common structure service.</value>
        protected ICommonStructureService CommonStructure { get; private set; }

        /// <summary>
        ///     Gets the authorization service.
        /// </summary>
        /// <value>The authorization service.</value>
        protected IAuthorizationService Authorization { get; private set; }

        /// <summary>
        ///     Gets the registration service.
        /// </summary>
        /// <value>The registration service.</value>
        protected IRegistration RegistrationService { get; private set; }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public TfsServer Server { get; private set; }

        /// <summary>
        ///     Gets the registered users of the current server.
        /// </summary>
        /// <value>The users.</value>
        public TfsRegisteredUserData Users { get; private set; }

        /// <summary>
        ///     Gets a list of the team project names available on the current server.
        /// </summary>
        /// <returns>IEnumerable{System.String}.</returns>
        public IEnumerable<string> GetProjectNames()
        {
            return CommonStructure.ListAllProjects().Select(p => p.Name);
        }

        /// <summary>
        ///     Gets a list of the project team names available on the current server.
        /// </summary>
        /// <returns>IEnumerable{System.String} containing the project teams.</returns>
        /// <remarks>This only works on the Philips Respironics TFS Server, as this is a custom field</remarks>
        public IEnumerable<string> GetProjectTeams()
        {
            var project = WorkItemStore.Projects[Server.TeamProject.SplitAndReturnFirst()];
            var workItemType = project.WorkItemTypes[Server.DefectWorkItemType];

            var item = new WorkItem(workItemType);
            var projectTeams = new List<string>();
            foreach (var field in item.Fields.Cast<Field>().Where(field => field.Name == "Team Backlog"))
            {
                projectTeams.AddRange(from object value in field.AllowedValues select value.ToString());
            }
            item.Close();

            return projectTeams;
        }

        /// <summary>
        ///     Gets the area list from the TFS Server.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        public IEnumerable<string> GetAreas(string project)
        {
            var projectInfo = CommonStructure.GetProjectFromName(project);
            var structs = CommonStructure.ListStructures(projectInfo.Uri);

            var areas = CommonStructure.GetNodesXml(new[] {structs[(int) StructType.Area].Uri}, true);
            return GetChildNodeNames(areas.ChildNodes[0]);
        }

        /// <summary>
        ///     Gets the iteration list from the TFS Server.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        public IEnumerable<string> GetIterations(string project)
        {
            var projectInfo = CommonStructure.GetProjectFromName(project);
            var structs = CommonStructure.ListStructures(projectInfo.Uri);

            var areas = CommonStructure.GetNodesXml(new[] {structs[(int) StructType.Iteration].Uri}, true);
            return GetChildNodeNames(areas.ChildNodes[0]);
        }

        /// <summary>
        ///     Gets the work item types.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetWorkItemTypes()
        {
            return GetWorkItemTypes(Server.TeamProject.SplitAndReturnFirst());
        }

        /// <summary>
        ///     Gets the work item types.
        /// </summary>
        /// <param name="teamProject">The team project.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public IEnumerable<string> GetWorkItemTypes(string teamProject)
        {
            var projects = GetProjectNames().ToList();
            if (!projects.Contains(teamProject))
                throw new ApplicationException(string.Format("Team Project {0} does not exist in the current collection", teamProject));

            var types = WorkItemStore.Projects[teamProject].WorkItemTypes;
            return (types.Cast<WorkItemType>()).Select(type => type.Name);
        }

        /// <summary>
        ///     Gets the registered link types.
        /// </summary>
        /// <returns>IEnumerable&lt;RegisteredLinkType&gt;.</returns>
        public RegisteredLinkTypeCollection GetRegisteredLinkTypes()
        {
            return WorkItemStore.RegisteredLinkTypes;
        }

        /// <summary>
        ///     Gets the child node names.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        private static IEnumerable<string> GetChildNodeNames(XmlNode tree)
        {
            if (tree.FirstChild == null)
                yield break;

            var count = tree.FirstChild.ChildNodes.Count;
            for (var i = 0; i < count; i++)
            {
                var node = tree.ChildNodes[0].ChildNodes[i];
                if (node.Attributes != null)
                    yield return node.Attributes["Name"].Value;

                if (node.HasChildNodes)
                {
                    var childrens = GetChildNodeNames(node);
                    foreach (var children in childrens)
                    {
                        yield return children;
                    }
                }
            }
        }

        private LabService GetLabService()
        {
            if (RegistrationService == null)
            {
                RegistrationService = Server.GetService<IRegistration>();
            }
            var entries = RegistrationService.GetRegistrationEntries("LabManagement");
            foreach (var registrationEntry in entries)
            {
                foreach (var @interface in registrationEntry.ServiceInterfaces)
                {
                    if (@interface.Name == "LabManagementService4")
                    {
                    }
                    if (@interface.Name == "LabManagementService")
                    {
                        return Server.Server.GetService<LabService>();
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Enum StructType
        /// </summary>
        private enum StructType
        {
            /// <summary>
            ///     The iteration
            /// </summary>
            Iteration = 0,

            /// <summary>
            ///     The area
            /// </summary>
            Area = 1
        }
    }
}