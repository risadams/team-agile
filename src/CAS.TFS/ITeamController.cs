#region References

using System.Collections.Generic;
using CAS.TFS.Model;

#endregion

namespace CAS.TFS
{
    /// <summary>
    ///     Interface ITeamController
    ///     Root level interface for all TFS project services
    /// </summary>
    public interface ITeamController
    {
        /// <summary>
        ///     Gets the registered users within the team project collection.
        /// </summary>
        /// <value>The <seealso cref="TfsRegisteredUserData" /> user list.</value>
        TfsRegisteredUserData Users { get; }

        /// <summary>
        ///     Gets the Tfs Server Instance.
        /// </summary>
        /// <value>The <see cref="TfsServer" /> instance.</value>
        TfsServer Server { get; }

        /// <summary>
        ///     Gets a list of the team project names available on the current server.
        /// </summary>
        /// <returns>IEnumerable{System.String} containing the team project names.</returns>
        IEnumerable<string> GetProjectNames();

        /// <summary>
        ///     Gets a list of the project team names available on the current server.
        /// </summary>
        /// <remarks>
        ///     This only works on the Philips Respironics TFS Server, as this is a custom field
        /// </remarks>
        /// <returns>IEnumerable{System.String} containing the project teams.</returns>
        IEnumerable<string> GetProjectTeams();

        /// <summary>
        ///     Gets the area list from the TFS Server.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        IEnumerable<string> GetAreas(string project);

        /// <summary>
        ///     Gets the iteration list from the TFS Server.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        IEnumerable<string> GetIterations(string project);

        /// <summary>
        /// Gets the work item types.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetWorkItemTypes();

        /// <summary>
        /// Gets the work item types.
        /// </summary>
        /// <param name="teamProject">The team project.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetWorkItemTypes(string teamProject);
    }
}