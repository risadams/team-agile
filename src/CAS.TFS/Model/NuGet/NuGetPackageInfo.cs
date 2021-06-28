using System;
using System.Diagnostics;
using System.Globalization;

namespace CAS.TFS.Model.NuGet
{
    /// <summary>
    /// Class NuGetPackageInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Name,nq}, {PackageVersion,nq}")]
    public class NuGetPackageInfo
    {
        /// <summary>
        ///     Gets or sets the server item.
        /// </summary>
        /// <value>The server item.</value>
        public string ServerItem { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the package version.
        /// </summary>
        /// <value>The package version.</value>
        public string PackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the target framework.
        /// </summary>
        /// <value>The target framework.</value>
        public string TargetFramework { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}, {1}", Name, PackageVersion);
        }

        /// <summary>
        ///     parses a new package info from a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>NuGetPackageInfo.</returns>
        /// <exception cref="System.ArgumentNullException">Unable to get NuGetPackageInfo, value must not be null</exception>
        /// <exception cref="System.InvalidOperationException">Unable to get NuGetPackageInfo from value:  + value</exception>
        public static NuGetPackageInfo FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value, "Unable to get NuGetPackageInfo, value must not be null");

            var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
                return new NuGetPackageInfo
                {
                    Name = parts[0],
                    PackageVersion = parts[1]
                };
            throw new InvalidOperationException("Unable to get NuGetPackageInfo from value: " + value);
        }
    }
}