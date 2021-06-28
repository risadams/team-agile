#region References

using System;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.Build;

#endregion

namespace CAS.TFS
{
    /// <summary>
    ///     Interface IBuildController.
    ///     Interface for interaction with TFS Build Services
    /// </summary>
    public interface IBuildController : ITeamController
    {
        /// <summary>
        ///     Retrieves a collection of all builds between the specified starting and ending date range.
        /// </summary>
        /// <param name="dateStart">The date range start.</param>
        /// <param name="dateEnd">The date range end.</param>
        /// <returns>A <see cref="BuildArtifactCollection" /> containing all located builds.</returns>
        BuildArtifactCollection GetBuildsByDate(DateTime dateStart, DateTime dateEnd);

        /// <summary>
        ///     Gets the project build definition tree.
        /// </summary>
        /// <returns>BuildTree.</returns>
        BuildTreeDictionary GetProjectBuildDefinitionTree();

        /// <summary>
        ///     Gets the build artifact.
        /// </summary>
        /// <param name="buildUri">The build URI.</param>
        /// <returns>BuildArtifact.</returns>
        BuildArtifact GetBuildArtifact(Uri buildUri);

        /// <summary>
        ///     Gets the builds by target release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>BuildArtifactCollection.</returns>
        BuildArtifactCollection GetBuildsByTargetRelease(TargetRelease targetRelease);

        /// <summary>
        ///     Gets the builds by definition.
        /// </summary>
        /// <param name="buildDefinition">The build definition.</param>
        /// <returns>BuildArtifactCollection.</returns>
        BuildArtifactCollection GetBuildsByDefinition(string buildDefinition);
    }
}