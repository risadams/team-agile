#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CAS.Common;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    ///     Class IterationController.
    /// </summary>
    public class IterationController : TeamController, IIterationController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public IterationController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the sprint for date.
        /// </summary>
        /// <param name="activeSprintDate">The start date.</param>
        /// <returns>Sprint.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Sprint GetSprintForDate(DateTime activeSprintDate)
        {
            const string query = @"select * from WorkItems where [System.TeamProject] in ({0}) and [System.WorkItemType] = 'Sprint' and [Microsoft.VSTS.Scheduling.StartDate] <= '{1}' and [Microsoft.VSTS.Scheduling.FinishDate] >= '{1}'";
            var wiql = string.Format(CultureInfo.InvariantCulture, query, Server.TeamProject.SplitAndWrapQuotes(), activeSprintDate.FormatDateInvariant());
            var items = WorkItemStore.Query(wiql);
            if (items.Count <= 0)
                return null;

            //Possibly returning more than one, they should all have the same start/end date.
            var item = items[0];

            return new Sprint
            {
                Iteration = item.GetProperty("Iteration Path", string.Empty),
                StartDate = item.GetProperty("Start Date", d => ParseDate(d.ToString())),
                EndDate = item.GetProperty("Finish Date", d => ParseDate(d.ToString()))
            };
        }

        /// <summary>
        ///     Gets the sprint range for date.
        /// </summary>
        /// <param name="activeSprintDate">The active sprint date.</param>
        /// <param name="previousSprints">The previous sprints.</param>
        /// <param name="futureSprints">The future sprints.</param>
        /// <returns>SprintCollection.</returns>
        public SprintCollection GetSprintRangeForDate(DateTime activeSprintDate, int previousSprints = 10, int futureSprints = 5)
        {
            var collection = new SprintCollection();
            var farPrevDate = CalculatePrevSprintDate(activeSprintDate, previousSprints);
            var farFutureDate = CalculateFutureSprintDate(activeSprintDate, futureSprints);

            const string query = @"select * from WorkItems where [System.TeamProject] in ({0}) and [System.WorkItemType] = 'Sprint' and [Microsoft.VSTS.Scheduling.StartDate] >= '{1}' and [Microsoft.VSTS.Scheduling.FinishDate] <= '{2}'";
            var wiql = string.Format(CultureInfo.InvariantCulture, query, Server.TeamProject.SplitAndWrapQuotes(), farPrevDate.FormatDateInvariant(), farFutureDate.FormatDateInvariant());

            var items = WorkItemStore.Query(wiql);
            if (items.Count <= 0)
                return new SprintCollection();

            foreach (var sprint in
                from WorkItem item in items
                select new Sprint
                {
                    Iteration = item.GetProperty("Iteration Path", string.Empty),
                    StartDate = item.GetProperty("Start Date", d => ParseDate(d.ToString())),
                    EndDate = item.GetProperty("Finish Date", d => ParseDate(d.ToString()))
                })
            {
                //only add the item if there is not another sprint of the same name already in the collection
                collection.ConditionalAdd(sprint, (c, s) => (!(from i in c where i.Name == s.Name select i).Any()));
            }

            return collection;
        }

        /// <summary>
        ///     Gets the sprint backlog.
        /// </summary>
        /// <param name="activeSprint">The active sprint.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <param name="backlogQueryFormat">The backlog query format.</param>
        /// <param name="asOfDate">As of date.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        public WorkItemInfoCollection GetSprintBacklog(Sprint activeSprint, string teamName, string backlogQueryFormat, DateTime? asOfDate = null)
        {
            var collection = new WorkItemInfoCollection();

            if (string.IsNullOrEmpty(backlogQueryFormat))
                return collection;

            if (asOfDate == null)
                asOfDate = DateTime.Now;
            var iteration = string.Format(CultureInfo.InvariantCulture, backlogQueryFormat, activeSprint.Name);

            const string query = @"select * from WorkItems where [System.TeamProject] in ({0}) and [System.IterationPath] under '{1}' AND [System.State] <> 'Removed' asof '{2}'";
            var wiql = string.Format(CultureInfo.InvariantCulture, query, Server.TeamProject.SplitAndWrapQuotes(), iteration, asOfDate.Value.FormatDateInvariant());

            try
            {
                var items = WorkItemStore.Query(wiql);
                collection.AddRange(from WorkItem item in items
                    where item.Type.Name != "Sprint"
                    //exclude Sprint WIT items
                    select new WorkItemInfo(item, Server.ServerUri));
            }
            catch
            {
                //Most likely issue is iteration path does not exist; return empty collection
                return collection;
            }
            return collection;
        }

        /// <summary>
        ///     Gets the sprint burndown data.
        /// </summary>
        /// <remarks>
        ///     Based on the algorithim detailed here:
        ///     http://blog.johnsworkshop.net/understanding-how-the-team-webaccess-burndown-chart-works-part-1/
        ///     http://blog.johnsworkshop.net/understanding-how-the-team-webaccess-burndown-chart-works-part-2/
        ///     http://blog.johnsworkshop.net/understanding-how-the-team-webaccess-burndown-chart-works-part-3/
        /// </remarks>
        /// <param name="activeSprint">The active sprint.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <param name="backlogQueryFormat">The backlog query format.</param>
        /// <returns>System.Object.</returns>
        public Dictionary<DateTime, WorkItemInfoCollection> GetSprintBurndownData(Sprint activeSprint, string teamName, string backlogQueryFormat)
        {
            //1: Pre-Process, Prepare the query expression to retrieve a filtered set of work items.
            /*
                Include work items which are included in the specified team’s scope.
                Include work items in the specified iteration.
                Include work item types from the Tasks work item type category.
                Include work items that are in the InProgress or Proposed metastate.
                Include work items where the remaining work is greater than or equal zero.
             */

            var data = new Dictionary<DateTime, WorkItemInfoCollection>();

            var currentDay = DateTime.Now;
            while (currentDay >= activeSprint.StartDate)
            {
                var workItems = GetSprintBacklog(activeSprint, teamName, backlogQueryFormat, currentDay);

                data.Add(currentDay, new WorkItemInfoCollection());

                data[currentDay].AddRange(from wi in workItems
                    where (wi.WorkItemType == Server.TaskWorkItemType || wi.WorkItemType == Server.DefectWorkItemType)
                    select wi);

                currentDay = currentDay.AddDays(-1);
            }

            //2: Processing, retrieve the burndown data.

            //3: Calculate the trend lines

            return data;
        }

        /// <summary>
        ///     Calculates the future sprint date.
        /// </summary>
        /// <param name="activeSprintDate">The active sprint date.</param>
        /// <param name="futureSprints">The future sprints.</param>
        /// <returns>DateTime.</returns>
        private static DateTime CalculateFutureSprintDate(DateTime activeSprintDate, int futureSprints)
        {
            return activeSprintDate.AddDays(14 * futureSprints);
        }

        /// <summary>
        ///     Calculates the previous sprint date.
        /// </summary>
        /// <param name="activeSprintDate">The active sprint date.</param>
        /// <param name="previousSprints">The previous sprints.</param>
        /// <returns>DateTime.</returns>
        private static DateTime CalculatePrevSprintDate(DateTime activeSprintDate, int previousSprints)
        {
            return activeSprintDate.AddDays(-(14 * previousSprints));
        }

        /// <summary>
        ///     Parses the date.
        /// </summary>
        /// <param name="rawDate">The raw date string.</param>
        /// <returns>DateTime.</returns>
        private static DateTime ParseDate(string rawDate)
        {
            DateTime date;
            DateTime.TryParse(rawDate, out date);
            return date;
        }
    }
}