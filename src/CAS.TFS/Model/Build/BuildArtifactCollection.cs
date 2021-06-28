#region References

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Class BuildArtifactCollection.
    /// </summary>
    [Serializable]
    public class BuildArtifactCollection : List<BuildArtifact>
    {
        /// <summary>
        ///     Gets the total number of builds in each status.
        /// </summary>
        /// <value>The total builds in each status.</value>
        public Dictionary<BuildStatus, int> TotalStatus
        {
            get
            {
                var dictionary = new Dictionary<BuildStatus, int>();
                foreach (var artifact in this)
                {
                    if (dictionary.ContainsKey(artifact.BuildStatus))
                        dictionary[artifact.BuildStatus]++;
                    else
                        dictionary.Add(artifact.BuildStatus, 1);
                }
                return dictionary;
            }
        }

        /// <summary>
        ///     Gets the build dates.
        /// </summary>
        /// <value>The build dates.</value>
        public IEnumerable<DateTime> BuildDates
        {
            get { return this.Select(build => build.EndTime).ToList(); }
        }

        /// <summary>
        ///     Gets the total number of succeeded builds.
        /// </summary>
        /// <value>The total succeeded builds.</value>
        public int TotalSucceeded
        {
            get { return !TotalStatus.ContainsKey(BuildStatus.Succeeded) ? 0 : TotalStatus[BuildStatus.Succeeded]; }
        }

        /// <summary>
        ///     Gets the total number of failed builds.
        /// </summary>
        /// <value>The total failed builds.</value>
        public int TotalFailed
        {
            get { return !TotalStatus.ContainsKey(BuildStatus.Failed) ? 0 : TotalStatus[BuildStatus.Failed]; }
        }
    }
}