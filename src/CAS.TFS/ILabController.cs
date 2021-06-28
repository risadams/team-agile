
using CAS.TFS.Model.LabManager;

namespace CAS.TFS
{
    /// <summary>
    /// Interface ILabController
    /// </summary>
    public interface ILabController : ITeamController
    {
        /// <summary>
        ///     Gets the labs available for team project.
        /// </summary>
        /// <param name="projectName">Name of the team project.</param>
        /// <returns>LabInfoCollection.</returns>
        LabInfoCollection GetLabs(string projectName);
    }
}