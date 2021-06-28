using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PackageName = System.String;
using TeamProject = System.String;
using SourcePath = System.String;

namespace CAS.TFS.Model.NuGet
{
    /// <summary>
    /// Class NuGetPackageInfoCollection.
    /// </summary>
    [Serializable]
    public class NuGetPackageInfoCollection : List<NuGetPackageInfo>
    {
        /// <summary>
        ///     Gets the reference locations.
        ///     Dictionary contains: Dictionary(PackageName,Dictionary(TeamProject,SourcePath))
        /// </summary>
        /// <param name="branchRoots">The branch roots.</param>
        /// <returns>Dictionary{System.StringDictionary{System.StringIList{System.String}}}.</returns>
        public Dictionary<PackageName, Dictionary<TeamProject, IList<SourcePath>>> GetReferenceLocations(IEnumerable<string> branchRoots)
        {
            var roots = branchRoots as IList<string> ?? branchRoots.ToList();
            var dictionary = new Dictionary<PackageName, Dictionary<TeamProject, IList<SourcePath>>>();

            foreach (var package in this)
            {
                var key = package.ToString();
                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, new Dictionary<TeamProject, IList<SourcePath>>());

                var path = GetProjectFromNuGetPackageReference(roots, package.ServerItem);

                if (!dictionary[key].ContainsKey(path.TeamProject))
                    dictionary[key].Add(path.TeamProject, new List<string>());

                dictionary[key][path.TeamProject].Add(path.SourcePath);
            }
            return dictionary;
        }

        /// <summary>
        ///     Gets the team project name from the NuGet package reference.
        /// </summary>
        /// <param name="branchRoots">The branch roots.</param>
        /// <param name="nuGetServerItem">The number get server item.</param>
        /// <returns>System.String.</returns>
        private static TeamProjectPathInfo GetProjectFromNuGetPackageReference(IEnumerable<string> branchRoots, string nuGetServerItem)
        {
            var roots = branchRoots as IList<string> ?? branchRoots.ToList();
            var stripped = nuGetServerItem.Replace("/packages.config", string.Empty);
            var split = roots.Aggregate(stripped, (current, root) => current.Replace(root, ParseTeamProjectRoot(root)))
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return new TeamProjectPathInfo(split[0], split[1]);
        }

        /// <summary>
        ///     Parses the team project from the branch root.
        /// </summary>
        /// <param name="root">The branch root.</param>
        /// <returns>System.String.</returns>
        private static string ParseTeamProjectRoot(string root)
        {
            root = root.Replace(TfsServer.SourceControlRoot, string.Empty);
            var index = root.IndexOf("/", StringComparison.Ordinal);
            root = root.Substring(0, index);
            return string.Format(CultureInfo.CurrentCulture, "{0},", root);
        }

        [Serializable]
        private class TeamProjectPathInfo : Tuple<string, string>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="TeamProjectPathInfo" /> class.
            /// </summary>
            /// <param name="item1">The item1.</param>
            /// <param name="item2">The item2.</param>
            public TeamProjectPathInfo(string item1, string item2)
                : base(item1, item2)
            {
            }

            /// <summary>
            ///     Gets the team project.
            /// </summary>
            /// <value>The team project.</value>
            public string TeamProject
            {
                get { return Item1; }
            }

            /// <summary>
            ///     Gets the source path.
            /// </summary>
            /// <value>The source path.</value>
            public string SourcePath
            {
                get { return Item2; }
            }
        }
    }
}