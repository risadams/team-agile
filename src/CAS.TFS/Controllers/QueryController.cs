using System.Globalization;
using System.Linq;
using System.Text;
using CAS.Common;
using CAS.TFS.Model.Query;
using CAS.TFS.Model.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class QueryController.
    /// </summary>
    public class QueryController : TeamController, IQueryController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public QueryController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>StoredQuery.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public QueryDefinition GetStoredQuery(string projectName, string queryName)
        {
            var project = WorkItemStore.Projects[projectName];
            return FindQueryItemByPath(queryName, project.QueryHierarchy);
        }

        /// <summary>
        ///     Executes the stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        public WorkItemInfoCollection ExecuteStoredQuery(string projectName, string queryName)
        {
            var collection = new WorkItemInfoCollection();

            var definition = GetStoredQuery(projectName, queryName);

            var query = string.Format(CultureInfo.CurrentCulture, definition.QueryText);
            var items = WorkItemStore.Query(query);

            collection.AddRange(from WorkItem item in items select new WorkItemInfo(item, Server.ServerUri));
            return collection;
        }

        /// <summary>
        ///     Gets the wiql for stored query.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string GetWiqlForStoredQuery(string projectName, string queryName)
        {
            var query = GetStoredQuery(projectName, queryName);
            var wiql = new WorkItemQuery
            {
                Query = query.QueryText
            };

            var xml = XmlUtilities.SerializeObjectToXmlString(wiql, new UTF8Encoding(false));
            return xml;
        }

        /// <summary>
        ///     Lists the queries.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>QueryInfoCollection.</returns>
        public QueryInfoCollection ListQueries(string projectName)
        {
            var collection = new QueryInfoCollection();
            var project = WorkItemStore.Projects[projectName];

            collection.AddRange(from StoredQuery query in project.StoredQueries
                                select new QueryInfo
                                {
                                    Name = query.Name.Replace("⁄", "\\").Replace("«", string.Empty).Replace("»", "\\").Replace("/", "\\"),
                                    Scope = query.QueryScope.ToString(),
                                    Description = query.Description,
                                    Wiql = new WorkItemQuery { Query = query.QueryText }
                                });

            return collection;
        }

        /// <summary>
        ///     Gets the XML for wiql.
        /// </summary>
        /// <param name="wiql">The wiql.</param>
        /// <returns>System.String.</returns>
        public string GetXmlForWiql(WorkItemQuery wiql)
        {
            return XmlUtilities.SerializeObjectToXmlString(wiql, new UTF8Encoding(false));
        }

        /// <summary>
        ///     Finds the query item by path.
        /// </summary>
        /// <param name="queryName">Name of the query.</param>
        /// <param name="currentNode">The current node.</param>
        /// <returns>QueryDefinition.</returns>
        private static QueryDefinition FindQueryItemByPath(string queryName, QueryItem currentNode)
        {
            var queryDefinition = currentNode as QueryDefinition;

            if (queryDefinition != null && queryDefinition.Path == queryName)
                return queryDefinition;

            var queryFolder = currentNode as QueryFolder;

            if (queryFolder == null)
                return null;

            return queryFolder.Select(qi => FindQueryItemByPath(queryName, qi)).FirstOrDefault(ret => ret != null);
        }
    }
}