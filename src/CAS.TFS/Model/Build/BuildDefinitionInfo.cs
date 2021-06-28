#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.TeamFoundation.Build.Client;

#endregion

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Class BuildDefinitionInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Name,nq}")]
    public class BuildDefinitionInfo
    {
        [NonSerialized]
        private readonly IBuildController _controller;

        [NonSerialized]
        private IBuildDefinition _buildDefinition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildDefinitionInfo" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="buildController"></param>
        public BuildDefinitionInfo(string name, IBuildController buildController)
        {
            Name = name;
            _controller = buildController;
            Children = new List<BuildDefinitionInfo>();
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the build definition.
        /// </summary>
        /// <value>The build definition.</value>
        public IBuildDefinition BuildDefinition
        {
            get { return _buildDefinition; }
            internal set { if (value != null) UpdateProperties(value); }
        }

        /// <summary>
        ///     Gets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize { get; private set; }

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        ///     Gets the default drop location.
        /// </summary>
        /// <value>The default drop location.</value>
        public string DefaultDropLocation { get; private set; }

        /// <summary>
        ///     Gets or sets the enabled status.
        /// </summary>
        /// <value>The enabled status.</value>
        public BuildEnabledStatus EnabledState { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the last good build label.
        /// </summary>
        /// <value>The last good build label.</value>
        public string LastGoodBuildLabel { get; set; }

        /// <summary>
        ///     Gets or sets the process parameters.
        /// </summary>
        /// <value>The process parameters.</value>
        public string ProcessParameters { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        /// <value>The date created.</value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the last build URI.
        /// </summary>
        /// <value>The last build URI.</value>
        public Uri LastBuildUri { get; set; }

        /// <summary>
        ///     Gets or sets the last good build URI.
        /// </summary>
        /// <value>The last good build URI.</value>
        public Uri LastGoodBuildUri { get; set; }

        /// <summary>
        ///     Gets or sets the continuous integration quiet period.
        /// </summary>
        /// <value>The continuous integration quiet period.</value>
        public int ContinuousIntegrationQuietPeriod { get; set; }

        /// <summary>
        ///     Gets the trigger.
        /// </summary>
        /// <value>The trigger.</value>
        public BuildTriggers Trigger { get; private set; }

        /// <summary>
        ///     Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public List<BuildDefinitionInfo> Children { get; private set; }

        /// <summary>
        ///     Gets the last build artifact.
        /// </summary>
        /// <value>The last build artifact.</value>
        public BuildArtifact LastBuildArtifact { get; private set; }

        /// <summary>
        ///     Gets the last good build artifact.
        /// </summary>
        /// <value>The last good build artifact.</value>
        public BuildArtifact LastGoodBuildArtifact { get; private set; }

        /// <summary>
        ///     Updates the properties.
        /// </summary>
        /// <param name="model">The model.</param>
        private void UpdateProperties(IBuildDefinition model)
        {
            _buildDefinition = model;
            BatchSize = model.BatchSize;
            Description = model.Description;
            DefaultDropLocation = model.DefaultDropLocation;
            EnabledState = SetState(model.QueueStatus);
            Id = model.Id;
            LastGoodBuildLabel = model.LastGoodBuildLabel;
            ProcessParameters = model.ProcessParameters;
            DateCreated = model.DateCreated;
            Trigger = SetTrigger(model.TriggerType);
            ContinuousIntegrationQuietPeriod = model.ContinuousIntegrationQuietPeriod;
            LastBuildUri = model.LastBuildUri;
            LastGoodBuildUri = model.LastGoodBuildUri;
            LastBuildArtifact = _controller.GetBuildArtifact(LastBuildUri);

            //If last build was good, no need to get the details twice, this takes long enough as it is.
            LastGoodBuildArtifact = (Uri.Compare(LastBuildUri, LastGoodBuildUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.InvariantCultureIgnoreCase) == 0)
                ? LastBuildArtifact
                : _controller.GetBuildArtifact(LastGoodBuildUri);
        }

        /// <summary>
        ///     Sets the trigger.
        /// </summary>
        /// <param name="triggerType">Type of the trigger.</param>
        /// <returns>BuildTriggers.</returns>
        private static BuildTriggers SetTrigger(DefinitionTriggerType triggerType)
        {
            switch (triggerType)
            {
                case DefinitionTriggerType.None:
                    return BuildTriggers.None;
                case DefinitionTriggerType.All:
                    return BuildTriggers.All;
                case DefinitionTriggerType.BatchedContinuousIntegration:
                    return BuildTriggers.BatchedContinuousIntegration;
                case DefinitionTriggerType.BatchedGatedCheckIn:
                    return BuildTriggers.BatchedGatedCheckIn;
                case DefinitionTriggerType.ContinuousIntegration:
                    return BuildTriggers.ContinuousIntegration;
                case DefinitionTriggerType.GatedCheckIn:
                    return BuildTriggers.GatedCheckIn;
                case DefinitionTriggerType.Schedule:
                    return BuildTriggers.Schedule;
                case DefinitionTriggerType.ScheduleForced:
                    return BuildTriggers.ScheduleForced;

                default:
                    return BuildTriggers.Unknown;
            }
        }

        /// <summary>
        ///     Sets the state.
        /// </summary>
        /// <param name="queueStatus">The queue status.</param>
        /// <returns>BuildEnabledStatus.</returns>
        private static BuildEnabledStatus SetState(DefinitionQueueStatus queueStatus)
        {
            switch (queueStatus)
            {
                case DefinitionQueueStatus.Paused:
                    return BuildEnabledStatus.Paused;
                case DefinitionQueueStatus.Disabled:
                    return BuildEnabledStatus.Disabled;
                default:
                case DefinitionQueueStatus.Enabled:
                    return BuildEnabledStatus.Enabled;
            }
        }
    }
}