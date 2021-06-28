#region References

using System;
using System.Collections.Generic;
using System.Linq;
using CAS.TFS.Model.Build;
using CAS.TFS.Model.Iteration;
using Microsoft.TeamFoundation.Build.Client;
using BuildStatus = CAS.TFS.Model.Build.BuildStatus;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    ///     The build controller class is responsible for all activities related to TFS Team Build objects and events.
    /// </summary>
    public class BuildController : TeamController, IBuildController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="CAS.TFS.ITeamController" /> base team controller instance.</param>
        public BuildController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Retrieves a collection of all builds between the specified starting and ending date range.
        /// </summary>
        /// <param name="dateStart">The date range start.</param>
        /// <param name="dateEnd">The date range end.</param>
        /// <returns>A <see cref="BuildArtifactCollection" /> containing all located builds.</returns>
        public BuildArtifactCollection GetBuildsByDate(DateTime dateStart, DateTime dateEnd)
        {
            var collection = new BuildArtifactCollection();
            var projects = GetProjectNames();
            foreach (var buildSpec in projects.Select(project => BuildServer.CreateBuildDetailSpec(project)))
            {
                buildSpec.MaxFinishTime = dateEnd;
                buildSpec.MinFinishTime = dateStart;

                var buildResults = BuildServer.QueryBuilds(buildSpec);
                var builds = buildResults.Builds;

                collection.AddRange(builds.Select(ConstructBuildArtifact));
            }

            return collection;
        }

        /// <summary>
        ///     Gets the project build definition tree.
        /// </summary>
        /// <returns>BuildTree.</returns>
        public BuildTreeDictionary GetProjectBuildDefinitionTree()
        {
            var dictionary = new BuildTreeDictionary();
            var projects = GetProjectNames();

            foreach (var project in projects)
            {
                var buildDefs = BuildServer.QueryBuildDefinitions(project);
                var tree = BuildTree(buildDefs);

                dictionary.Add(project, tree);
            }

            return dictionary;
        }

        /// <summary>
        ///     Gets the build artifact.
        /// </summary>
        /// <param name="buildUri">The build URI.</param>
        /// <returns>BuildArtifact.</returns>
        public BuildArtifact GetBuildArtifact(Uri buildUri)
        {
            if (buildUri == null)
                return null;

            var raw = BuildServer.GetBuild(buildUri);
            return ConstructBuildArtifact(raw);
        }

        /// <summary>
        ///     Gets the builds by target release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>BuildArtifactCollection.</returns>
        public BuildArtifactCollection GetBuildsByTargetRelease(TargetRelease targetRelease)
        {
            return GetBuildsByDefinition(targetRelease.ToString());
        }

        /// <summary>
        ///     Gets the builds by definition.
        /// </summary>
        /// <param name="buildDefinition">The build definition.</param>
        /// <returns>EncoreTeamUtilities.TFS.Model.BuildArtifactCollection.</returns>
        public BuildArtifactCollection GetBuildsByDefinition(string buildDefinition)
        {
            var collection = new BuildArtifactCollection();
            var projects = GetProjectNames();

            foreach (var builds in projects
                .Select(project => BuildServer.CreateBuildDetailSpec(project, buildDefinition))
                .Select(spec => BuildServer.QueryBuilds(spec))
                .Select(buildResults => buildResults.Builds))
            {
                collection.AddRange(builds.Select(ConstructBuildArtifact));
            }

            return collection;
        }

        /// <summary>
        ///     Constructs the build artifact.
        /// </summary>
        /// <param name="buildDetail">The build detail.</param>
        /// <returns>BuildArtifact.</returns>
        private static BuildArtifact ConstructBuildArtifact(IBuildDetail buildDetail)
        {
            if (buildDetail == null)
                return null;

            return new BuildArtifact
            {
                Project = buildDetail.TeamProject,
                BuildName = buildDetail.BuildNumber,
                BuildStatus = ((BuildStatus) (int) buildDetail.Status),
                LabelName = buildDetail.LabelName,
                DropLocation = buildDetail.DropLocation,
                StartTime = buildDetail.StartTime,
                EndTime = buildDetail.FinishTime,
                DropLocationRoot = buildDetail.DropLocationRoot,
                BuildFinished = buildDetail.BuildFinished,
                Deleted = buildDetail.IsDeleted,
                LogLocation = buildDetail.LogLocation,
                Quality = buildDetail.Quality,
                RequestedBy = buildDetail.RequestedBy,
                RequestedFor = buildDetail.RequestedFor,
                RetainForever = buildDetail.KeepForever,
                SourceVersionSpec = buildDetail.SourceGetVersion,
                TestStatus = ((TestStatus) (int) buildDetail.TestStatus),
                CompilationStatus = ((CompilationStatus) (int) buildDetail.TestStatus),
                BuildReason = ((BuildReasons) (int) buildDetail.Reason),
                LastChangedBy = buildDetail.LastChangedBy,
                LastChangedByDisplayName = buildDetail.LastChangedByDisplayName,
                LastChangedOn = buildDetail.LastChangedOn,
                ProcessParameters = buildDetail.ProcessParameters,
                ShelvesetName = buildDetail.ShelvesetName
            };
        }

        private IList<BuildDefinitionInfo> BuildTree(IEnumerable<IBuildDefinition> buildDefinitions)
        {
            var root = new BuildDefinitionInfo("root", this);
            foreach (var buildDefinition in buildDefinitions)
            {
                var name = buildDefinition.Name;
                root = BuildTree(root, buildDefinition, name.Split('.'));
            }

            return root.Children;
        }

        private BuildDefinitionInfo BuildTree(BuildDefinitionInfo node, IBuildDefinition model, string[] tail)
        {
            if (tail.Any())
            {
                var head = tail.First();

                if (node.Children.Any(n => n.Name.Equals(head, StringComparison.OrdinalIgnoreCase)))
                {
                    BuildTree(node.Children.Single(n => n.Name.Equals(head, StringComparison.OrdinalIgnoreCase)), model, tail.Skip(1).ToArray());
                }
                else
                {
                    node.Children.Add(BuildTree(new BuildDefinitionInfo(head, this), model, tail.Skip(1).ToArray()));
                }
            }
            else
            {
                node.BuildDefinition = model;
            }

            return node;
        }
    }
}