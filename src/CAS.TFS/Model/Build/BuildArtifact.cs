using System;
using System.Diagnostics;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Class BuildArtifact.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{BuildName,nq} - {BuildStatus,nq}")]
    public class BuildArtifact
    {
        /// <summary>
        ///     Gets or sets the name of the build.
        /// </summary>
        /// <value>The name of the build.</value>
        public string BuildName { get; set; }

        /// <summary>
        ///     Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public string Project { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public BuildStatus BuildStatus { get; set; }

        /// <summary>
        ///     Gets or sets the test status.
        /// </summary>
        /// <value>The test status.</value>
        public TestStatus TestStatus { get; set; }

        /// <summary>
        ///     Gets or sets the drop location.
        /// </summary>
        /// <value>The drop location.</value>
        public string DropLocation { get; set; }

        /// <summary>
        ///     Gets or sets the drop location root.
        /// </summary>
        /// <value>The drop location root.</value>
        public string DropLocationRoot { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether thsi build will be retained forever.
        /// </summary>
        /// <value><c>true</c> if retained forever; otherwise, <c>false</c>.</value>
        public bool RetainForever { get; set; }

        /// <summary>
        ///     Gets or sets the name of the label.
        /// </summary>
        /// <value>The name of the label.</value>
        public string LabelName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this build has finished.
        /// </summary>
        /// <value><c>true</c> if build has finished; otherwise, <c>false</c>.</value>
        public bool BuildFinished { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
        public bool Deleted { get; set; }

        /// <summary>
        ///     Gets or sets the source version spec.
        /// </summary>
        /// <value>The source version spec.</value>
        public string SourceVersionSpec { get; set; }

        /// <summary>
        ///     Gets or sets the requested by.
        /// </summary>
        /// <value>The requested by.</value>
        public string RequestedBy { get; set; }

        /// <summary>
        ///     Gets or sets the requested for.
        /// </summary>
        /// <value>The requested for.</value>
        public string RequestedFor { get; set; }

        /// <summary>
        ///     Gets or sets the last changed by.
        /// </summary>
        /// <value>The last changed by.</value>
        public string LastChangedBy { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the changed by display.
        /// </summary>
        /// <value>The last name of the changed by display.</value>
        public string LastChangedByDisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the last changed on.
        /// </summary>
        /// <value>The last changed on.</value>
        public DateTime LastChangedOn { get; set; }

        /// <summary>
        ///     Gets or sets the process parameters.
        /// </summary>
        /// <value>The process parameters.</value>
        public string ProcessParameters { get; set; }

        /// <summary>
        ///     Gets or sets the build reason.
        /// </summary>
        /// <value>The build reason.</value>
        public BuildReasons BuildReason { get; set; }

        /// <summary>
        ///     Gets or sets the name of the shelveset.
        /// </summary>
        /// <value>The name of the shelveset.</value>
        public string ShelvesetName { get; set; }

        /// <summary>
        ///     Gets or sets the quality.
        /// </summary>
        /// <value>The quality.</value>
        public string Quality { get; set; }

        /// <summary>
        ///     Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        public string LogLocation { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     Gets or sets the compilation status.
        /// </summary>
        /// <value>The compilation status.</value>
        public CompilationStatus CompilationStatus { get; set; }
    }
}