#region References

using System.Linq;
using CAS.TFS.Model.LabManager;
using Microsoft.TeamFoundation.Lab.Client;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class LabController.
    /// </summary>
    public class LabController : TeamController, ILabController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public LabController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the labs available for team project.
        /// </summary>
        /// <param name="projectName">Name of the team project.</param>
        /// <returns>LabInfoCollection.</returns>
        public LabInfoCollection GetLabs(string projectName)
        {
            var collection = new LabInfoCollection();

            var spec = new LabEnvironmentQuerySpec {Project = projectName};
            var labs = LabService.QueryLabEnvironments(spec);

            collection.AddRange(labs.Select(lab => new LabInfo(lab)));

            return collection;
        }
    }
}