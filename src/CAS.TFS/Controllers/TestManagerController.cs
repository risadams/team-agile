using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class TestManagerController.
    /// </summary>
    public class TestManagerController : TeamController, ITestManagerController
    {
        private ITestManagementTeamProject _project;

        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public TestManagerController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Queries the test runs.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;ITestRun&gt;.</returns>
        public IEnumerable<ITestRun> QueryTestRuns(string query)
        {
            return TestManagementService.QueryTestRuns(query);
        }

        /// <summary>
        ///     Queries the test results.
        /// </summary>
        /// <param name="projectWorkItemReference">The project work item reference.</param>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;ITestResult&gt;.</returns>
        public IEnumerable<ITestCaseResult> QueryTestResults(WorkItem projectWorkItemReference, string query)
        {
            _project = TestManagementService.GetTeamProject(projectWorkItemReference.Project);
            return _project.TestResults.Query(query);
        }
    }
}