using System;

namespace CAS.TFS.Model.NuGet
{
    /// <summary>
    /// Class NuGetFeed.
    /// </summary>
    [Serializable]
    public class NuGetFeed
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
    }
}