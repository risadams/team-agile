using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS
{
    /// <summary>
    /// Interface ITestManagerController
    /// </summary>
    public interface ITestManagerController : ITeamController
    {
        /// <summary>
        ///     Queries the test runs.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;ITestRun&gt;.</returns>
        IEnumerable<ITestRun> QueryTestRuns(string query);

        /// <summary>
        ///     Queries the test results.
        /// </summary>
        /// <param name="projectWorkItemReference">The project work item identifier.</param>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;ITestResult&gt;.</returns>
        IEnumerable<ITestCaseResult> QueryTestResults(WorkItem projectWorkItemReference, string query);
    }
}