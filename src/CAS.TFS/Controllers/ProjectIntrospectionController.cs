#region References

using System.Collections.Generic;
using System.Linq;
using CAS.TFS.Model.Iteration;
using CAS.TFS.Model.NuGet;
using CAS.Common;
using CAS.TFS.Model.SourceControl;
using Microsoft.TeamFoundation.VersionControl.Client;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    ///     Class ProjectIntrospectionController.
    /// </summary>
    public class ProjectIntrospectionController
        : HistoryController
    {
        private readonly List<Item> _packageConfigItemList = new List<Item>();
        private readonly List<string> _packageConfigNameList = new List<string>();
        private readonly NuGetPackageInfoCollection _packages = new NuGetPackageInfoCollection();

        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public ProjectIntrospectionController(TeamController controller)
            : base(controller)
        {
            NuGetFeedSources = new List<NuGetFeed>();
            if (controller.Server.NuGetFeeds.Any())
            {
                NuGetFeedSources = controller.Server.NuGetFeeds.ToList();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectIntrospectionController" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="feeds">The feeds.</param>
        public ProjectIntrospectionController(TeamController controller, IEnumerable<NuGetFeed> feeds)
            : base(controller)
        {
            NuGetFeedSources = feeds.ToList();
        }

        /// <summary>
        ///     Gets or sets the NuGet feed sources.
        /// </summary>
        /// <value>The feed sources.</value>
        public IList<NuGetFeed> NuGetFeedSources { get; set; }

        /// <summary>
        ///     Queries the NuGet get package information for the specified branch location.
        /// </summary>
        /// <param name="sourceBranches">The source branches.</param>
        /// <returns>NuGetPackageInfoCollection.</returns>
        public NuGetPackageInfoCollection QueryNuGetPackagesForBranches(IEnumerable<string> sourceBranches)
        {
            foreach (var branch in sourceBranches)
            {
                QueryNuGetPackageConfig(branch);
            }

            var packages = _packageConfigItemList.SelectMany(GetNuGetPackageInfoFromConfiguration);
            foreach (var nuGetPackageInfo in packages)
            {
                var info = nuGetPackageInfo;

                _packages.Add(info);
            }
            return _packages;
        }

        /// <summary>
        ///     Gets the database change scripts for the specfied target release.
        /// </summary>
        /// <param name="targetRelease">The target release.</param>
        /// <returns>IDictionary{System.StringIList{ServerItemInfo}}.</returns>
        public IDictionary<string, IList<ServerItemInfo>> GetDatabaseChangeScriptsForTargetRelease(TargetRelease targetRelease)
        {
            var scriptDictionary = new SortedDictionary<string, IList<ServerItemInfo>>();

            var root = "$/EncoreDatabase/ChangeScripting/" + targetRelease.Product + "/" + targetRelease.Version;
            var environments = GetSubItems(root).ToList();

            if (!environments.Any())
                return scriptDictionary;

            foreach (var instance in environments)
            {
                if (instance.ServerItem == root)
                    continue;

                var environment = StringUtilities.ParseServerItem(instance.ServerItem);
                var envRoot = root + "/" + environment;

                var changeScripts = GetSubItems(instance.ServerItem).ToList();
                if (!changeScripts.Any())
                    continue;

                foreach (var changeScript in changeScripts)
                {
                    if (changeScript.ServerItem == envRoot)
                        continue;

                    if (!scriptDictionary.ContainsKey(environment))
                        scriptDictionary.Add(environment, new List<ServerItemInfo>());

                    var script = new ServerItemInfo
                    {
                        ChangesetId = changeScript.ChangesetId,
                        Environment = environment,
                        ServerPath = changeScript.ServerItem
                    };

                    scriptDictionary[environment].Add(script);
                }
            }

            return scriptDictionary;
        }

        /// <summary>
        ///     Loads the package config file from TFS and converts to contens into one or more <see cref="NuGetPackageInfo" />
        ///     instances.
        /// </summary>
        /// <param name="serverPackageConfig">The server package configuration.</param>
        /// <returns>IEnumerable{NuGetPackageInfo}.</returns>
        private IEnumerable<NuGetPackageInfo> GetNuGetPackageInfoFromConfiguration(Item serverPackageConfig)
        {
            var fileContentStream = GetFileContentStream(serverPackageConfig.ServerItem);
            var xDocument = XmlUtilities.LoadXmlFromStream(fileContentStream);

            return from package in xDocument.Descendants("package")
                   select new NuGetPackageInfo
                   {
                       ServerItem = serverPackageConfig.ServerItem,
                       Name = XmlUtilities.GetAttributeValueOrDefault(package, "id", "N/A"),
                       PackageVersion = XmlUtilities.GetAttributeValueOrDefault(package, "version", "N/A"),
                       TargetFramework = XmlUtilities.GetAttributeValueOrDefault(package, "targetFramework", "N/A")
                   };
        }


        /// <summary>
        ///     retrieves a list of all files on the branch named <c>packages.config</c>.
        /// </summary>
        /// <param name="branch">The branch.</param>
        private void QueryNuGetPackageConfig(string branch)
        {
            var allItems = VersionControl.GetItems(new ItemSpec(branch, RecursionType.Full), VersionSpec.Latest, DeletedState.NonDeleted, ItemType.File, GetItemsOptions.None).Items;
            foreach (var item in
                from item in allItems
                let name = StringUtilities.ParseServerItem(item.ServerItem)
                where name.ToLowerInvariant() == "packages.config"
                where !_packageConfigNameList.Contains(item.ServerItem)
                select item)
            {
                _packageConfigNameList.Add(item.ServerItem);
                _packageConfigItemList.Add(item);
            }
        }

        /// <summary>
        ///     Gets the database change scripts for server root.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="recursive">if set to <c>true</c> all recursivly retrieve files from subfolders.</param>
        /// <returns>IDictionary&lt;System.String, IList&lt;ServerItemInfo&gt;&gt;.</returns>
        public IEnumerable<ServerItemInfo> GetFilesForServerRoot(string root, bool recursive = true)
        {
            var files = GetSubItems(root).ToList();

            foreach (var instance in files)
            {
                if (instance.ServerItem == root)
                    continue;

                if (instance.ItemType == ItemType.File)
                    yield return new ServerItemInfo
                    {
                        ChangesetId = instance.ChangesetId,
                        ServerPath = instance.ServerItem
                    };
                else if (recursive && instance.ItemType == ItemType.Folder)
                {
                    var items = GetFilesForServerRoot(root + "/" + StringUtilities.ParseServerItem(instance.ServerItem)).ToList();
                    foreach (var item in items)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}