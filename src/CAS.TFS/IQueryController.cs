using CAS.TFS.Model.WorkItems;
using CAS.TFS.Model.Query;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS
{
    /// <summary>
    /// Interface IQueryController
    /// </summary>
    public interface IQueryController : ITeamController
    {
        /// <summary>
        ///     Gets the stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>StoredQuery.</returns>
        QueryDefinition GetStoredQuery(string projectName, string queryName);

        /// <summary>
        /// Executes the stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        WorkItemInfoCollection ExecuteStoredQuery(string projectName, string queryName);

        /// <summary>
        ///     Gets the wiql for stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>System.String.</returns>
        string GetWiqlForStoredQuery(string projectName, string queryName);

        /// <summary>
        ///     Lists the queries.
        /// </summary>
        /// <param name="projectName">The project.</param>
        /// <returns>QueryInfoCollection.</returns>
        QueryInfoCollection ListQueries(string projectName);

        /// <summary>
        ///     Gets the XML for wiql.
        /// </summary>
        /// <param name="wiql">The wiql.</param>
        /// <returns>System.String.</returns>
        string GetXmlForWiql(WorkItemQuery wiql);
    }
}