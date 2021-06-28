using System;
using System.Collections.Generic;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.WorkItems;

namespace CAS.TFS
{
    /// <summary>
    /// Interface IIterationController
    /// </summary>
    public interface IIterationController : ITeamController
    {
        /// <summary>
        ///     Gets the sprint for date.
        /// </summary>
        /// <param name="activeSprintDate">The start date.</param>
        /// <returns>Sprint.</returns>
        Sprint GetSprintForDate(DateTime activeSprintDate);

        /// <summary>
        ///     Gets the sprint range for date.
        /// </summary>
        /// <param name="activeSprintDate">The active sprint date.</param>
        /// <param name="previousSprints">The previous sprints.</param>
        /// <param name="futureSprints">The future sprints.</param>
        /// <returns>SprintCollection.</returns>
        SprintCollection GetSprintRangeForDate(DateTime activeSprintDate, int previousSprints = 10, int futureSprints = 5);

        /// <summary>
        ///     Gets the sprint backlog.
        /// </summary>
        /// <param name="activeSprint">The active sprint.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <param name="backlogQueryFormat">The backlog query format.</param>
        /// <param name="asOfDate">As of date.</param>
        /// <returns>WorkItemInfoCollection.</returns>
        WorkItemInfoCollection GetSprintBacklog(Sprint activeSprint, string teamName, string backlogQueryFormat, DateTime? asOfDate = null);

        /// <summary>
        ///     Gets the sprint burndown data.
        /// </summary>
        /// <param name="activeSprint">The active sprint.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <param name="backlogQueryFormat">The backlog query format.</param>
        Dictionary<DateTime, WorkItemInfoCollection> GetSprintBurndownData(Sprint activeSprint, string teamName, string backlogQueryFormat);
    }
}