#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CAS.Common.Comparers;
using CAS.TFS.Model;
using CAS.TFS.Model.History;
using CAS.TFS.Model.SourceControl;
using Microsoft.TeamFoundation.VersionControl.Client;

#endregion

namespace CAS.TFS.Controllers
{
    /// <summary>
    ///     Class HistoryController.
    /// </summary>
    public class HistoryController : TeamController, IHistoryController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public HistoryController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the changeset with the specified id.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>Changeset.</returns>
        public Changeset GetChangeset(int id)
        {
            return VersionControl.GetChangeset(id, true, false);
        }

        /// <summary>
        ///     Gets the files modified by the specified changesets.
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>Dictionary{System.Int32IEnumerable{Item}}.</returns>
        public Dictionary<int, IEnumerable<Item>> GetFilesForChangesets(IEnumerable<int> changesetIds)
        {
            var filesForChangesets = new Dictionary<int, IEnumerable<Item>>();
            foreach (var changesetId in changesetIds)
            {
                var changeset = GetChangeset(changesetId);
                var files = ExtractReviewableFiles(changeset);
                filesForChangesets.Add(changesetId, files);
            }
            return filesForChangesets;
        }

        /// <summary>
        ///     gets the files associated with the specified changesets, filter out known file extensions not normally reviewed
        ///     during a code review
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>SortedDictionary{System.StringList{System.Int32}}.</returns>
        public SortedDictionary<string, List<int>> ChangedFileReviewList(IList<int> changesetIds)
        {
            var changedFileReviewList = new SortedDictionary<string, List<int>>(new OrdinalStringComparer());

            if (changesetIds == null || !changesetIds.Any())
            {
                return changedFileReviewList;
            }

            foreach (var changesetId in changesetIds)
            {
                var changeset = GetChangeset(changesetId);
                var files = ExtractReviewableFiles(changeset);
                foreach (var file in files)
                {
                    if (!changedFileReviewList.ContainsKey(file.ServerItem))
                        changedFileReviewList.Add(file.ServerItem, new List<int>());
                    changedFileReviewList[file.ServerItem].Add(changesetId);
                }
            }
            return changedFileReviewList;
        }

        /// <summary>
        ///     Gets the file content stream.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>Stream.</returns>
        public Stream GetFileContentStream(string serverPath, int changeSet)
        {
            var item = VersionControl.GetItem(serverPath, new ChangesetVersionSpec(changeSet));
            return item.DownloadFile();
        }

        /// <summary>
        ///     Gets the file content stream.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <returns>Stream.</returns>
        public Stream GetFileContentStream(string serverPath)
        {
            var item = VersionControl.GetItem(serverPath, VersionSpec.Latest);
            return item.DownloadFile();
        }

        /// <summary>
        ///     Gets the image file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>Image.</returns>
        public Image GetImageFileContents(string serverPath, int changeSet)
        {
            var itemStream = GetFileContentStream(serverPath, changeSet);
            return Image.FromStream(itemStream);
        }

        /// <summary>
        ///     Gets the file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>System.String.</returns>
        public string GetFileContents(string serverPath, int changeSet)
        {
            var itemStream = GetFileContentStream(serverPath, changeSet);
            string contents;
            using (var reader = new StreamReader(itemStream))
            {
                contents = reader.ReadToEnd();
            }
            return contents;
        }

        /// <summary>
        ///     Gets the file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <returns>System.String.</returns>
        public string GetFileContents(string serverPath)
        {
            var itemStream = GetFileContentStream(serverPath);
            string contents;
            using (var reader = new StreamReader(itemStream))
            {
                contents = reader.ReadToEnd();
            }
            return contents;
        }

        /// <summary>
        ///     Gets the changeset owners for each of the specified changesets.
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        public IEnumerable<string> GetChangesetOwners(IList<int> changesetIds)
        {
            var list = new HashSet<string>();
            if (changesetIds == null || !changesetIds.Any())
                return list;

            foreach (var changeset in changesetIds
                .Select(GetChangeset)
                .Where(changeset => !list.Contains(changeset.Owner)))
                list.Add(changeset.Owner);

            return list;
        }

        /// <summary>
        ///     Gets the changeset owner for the specified changeset.
        /// </summary>
        /// <param name="changesetId">The changeset unique identifier.</param>
        /// <returns>System.String.</returns>
        public string GetChangesetOwner(int changesetId)
        {
            var cs = GetChangeset(changesetId);
            return cs != null ? cs.Owner : string.Empty;
        }

        /*
        public IEnumerable<Changeset> SearchHistory(Item item, string userName)
        {
            var changesets = new Dictionary<int, Changeset>();
            IEnumerable enumerable;
            if (item.ItemType == ItemType.Folder)
            {
                foreach (Changeset changeset
                    in VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, true, false))
                    changesets.Add(changeset.ChangesetId, changeset);

                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.Full, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, false, false);
            }
            else
                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, true, false);

            return from Changeset changeset in enumerable
                select changesets.ContainsKey(changeset.ChangesetId)
                    ? changesets[changeset.ChangesetId]
                    : changeset;
        }*/

        /// <summary>
        ///     Searches the changeset history information for the specified server item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>IEnumerable{ChangesetHistoryInfo}.</returns>
        public IEnumerable<ChangesetHistoryInfo> SearchHistoryInfo(Item item, string userName)
        {
            var changesets = new Dictionary<int, Changeset>();
            IEnumerable enumerable;
            if (item.ItemType == ItemType.Folder)
            {
                foreach (Changeset changeset
                    in VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, true, false))
                    changesets.Add(changeset.ChangesetId, changeset);

                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.Full, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, false, false);
            }
            else
                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new ChangesetVersionSpec(1), VersionSpec.Latest, int.MaxValue, true, false);

            return from Changeset changeset in enumerable
                select changesets.ContainsKey(changeset.ChangesetId)
                    ? TranslateChangeset(changesets[changeset.ChangesetId])
                    : TranslateChangeset(changeset);
        }

        /// <summary>
        ///     Searches the changeset history information for the specified server item.
        ///     limits the search to changesets occurring after the specified minimum date
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <returns>IEnumerable{ChangesetHistoryInfo}.</returns>
        public IEnumerable<ChangesetHistoryInfo> SearchHistoryInfo(Item item, string userName, DateTime minDate)
        {
            var changesets = new Dictionary<int, Changeset>();
            IEnumerable enumerable;
            if (item.ItemType == ItemType.Folder)
            {
                foreach (Changeset changeset
                    in VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new DateVersionSpec(minDate), VersionSpec.Latest, int.MaxValue, true, false))
                    changesets.Add(changeset.ChangesetId, changeset);

                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.Full, userName, new DateVersionSpec(minDate), VersionSpec.Latest, int.MaxValue, false, false);
            }
            else
                enumerable = VersionControl.QueryHistory(item.ServerItem, new ChangesetVersionSpec(item.ChangesetId), 0, RecursionType.None, userName, new DateVersionSpec(minDate), VersionSpec.Latest, int.MaxValue, true, false);

            return from Changeset changeset in enumerable
                select changesets.ContainsKey(changeset.ChangesetId)
                    ? TranslateChangeset(changesets[changeset.ChangesetId])
                    : TranslateChangeset(changeset);
        }

        /// <summary>
        ///     Gets the first level items in the TFS hierarchy.
        /// </summary>
        /// <returns>IEnumerable{Item}.</returns>
        public IEnumerable<Item> GetFirstLevelItems()
        {
            return VersionControl.GetItems(TfsServer.SourceControlRoot, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.Any, false).Items;
        }

        /// <summary>
        ///     Gets the sub items of the specified server item in the TFS hierarchy.
        /// </summary>
        /// <param name="serverItem">The server item.</param>
        /// <returns>IEnumerable{Item}.</returns>
        public IEnumerable<Item> GetSubItems(string serverItem)
        {
            return VersionControl.GetItems(new ItemSpec(serverItem, RecursionType.OneLevel), VersionSpec.Latest, DeletedState.NonDeleted, ItemType.Any, GetItemsOptions.IncludeBranchInfo).Items;
        }

        /*
        public IEnumerable<Item> GetBranchRelatives(Item item)
        {
            var list = new List<Item>();
            var itemBranches = GetItemBranches(item);

            BranchHistoryTreeItem branchHistoryTreeItem1 = null;
            if (itemBranches.Length != 0 && itemBranches[0].Length != 0)
                branchHistoryTreeItem1 = itemBranches[0][0];
            if (branchHistoryTreeItem1 == null)
                return list;

            var requestedItem = branchHistoryTreeItem1.GetRequestedItem();
            if (requestedItem.Parent != null && requestedItem.Parent.Relative.BranchToItem != null && requestedItem.Parent.Relative.BranchToItem.DeletionId == 0)
                list.Add(requestedItem.Parent.Relative.BranchToItem);

            list.AddRange(from BranchHistoryTreeItem branchHistoryTreeItem2 in requestedItem.Children
                where branchHistoryTreeItem2.Relative.BranchToItem != null && branchHistoryTreeItem2.Relative.BranchToItem.DeletionId == 0
                select branchHistoryTreeItem2.Relative.BranchToItem);

            return list;
        }
        
        public int GetBranchRelativeBaselineChangeset(string path)
        {
            return VersionControl.QueryHistory(path, VersionSpec.Latest, 0, RecursionType.None, null, null, null, 1, false, false, false, true).Cast<Changeset>().First().ChangesetId;
        }

        public int GetPreviousChangeset(string path)
        {
            var hist = VersionControl.QueryHistory(path, VersionSpec.Latest, 0, RecursionType.Full, null, null, null, 2, false, false, false, true).Cast<Changeset>();
            return hist.First().ChangesetId;
        }*/

        /// <summary>
        ///     Gets a list of all changesets between the specified start and end date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>the list of all changesets between the specified start and end date range.</returns>
        public ChangesetHistoryInfoCollection GetChangesetsByDate(DateTime startDate, DateTime endDate)
        {
            var collection = new ChangesetHistoryInfoCollection();

            var changesets =
                VersionControl.QueryHistory(TfsServer.SourceControlRoot, new DateVersionSpec(startDate), 0, RecursionType.Full, null, new DateVersionSpec(startDate), new DateVersionSpec(endDate), Int32.MaxValue, false, false, false,
                    true)
                    .Cast<Changeset>();

            collection.AddRange(from Changeset changeset in changesets
                select new ChangesetHistoryInfo(changeset));

            return collection;
        }

        /// <summary>
        ///     Recursively gets all children of the specified root and return any item which is a folder.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>IEnumerable{Item}.</returns>
        public IEnumerable<Item> GetSubFoldersRecursive(string root)
        {
            var items = VersionControl.GetItems(root, RecursionType.Full);
            return items.Items.Where(item => item.ItemType == ItemType.Folder);
        }

        /// <summary>
        ///     Gets the branch hierarchy.
        /// </summary>
        /// <param name="includeDeleted">if set to <c>true</c> deleted branches will be included.</param>
        /// <returns>BranchTree.</returns>
        public IEnumerable<BranchInfo> GetFullBranchList(bool includeDeleted = false)
        {
            var branchRoots = VersionControl.QueryRootBranchObjects(RecursionType.OneLevel).Take(3);
            return branchRoots.SelectMany(br => GetChildBranchesRecursive(br, includeDeleted));
        }

        /// <summary>
        ///     Gets the child branches recursively.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="includeDeleted">if set to <c>true</c> deleted branches will be included.</param>
        /// <returns>IEnumerable&lt;BranchInfo&gt;.</returns>
        private IEnumerable<BranchInfo> GetChildBranchesRecursive(BranchObject root, bool includeDeleted)
        {
            if (includeDeleted || !root.Properties.RootItem.IsDeleted)
            {
                yield return new BranchInfo(root);
            }

            var children = VersionControl.QueryBranchObjects(root.Properties.RootItem, RecursionType.OneLevel);
            foreach (var child in children)
            {
                if (child.Properties.RootItem.Item == root.Properties.RootItem.Item)
                    continue;

                var items = GetChildBranchesRecursive(child, includeDeleted);
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        ///     Translates the changeset.
        /// </summary>
        /// <param name="changeset">The changeset.</param>
        /// <returns>ChangesetHistoryInfo.</returns>
        private static ChangesetHistoryInfo TranslateChangeset(Changeset changeset)
        {
            return new ChangesetHistoryInfo(changeset);
        }

        /// <summary>
        ///     Extracts the reviewable files.
        /// </summary>
        /// <param name="changeset">The changeset.</param>
        /// <returns>IEnumerable{Item}.</returns>
        private static IEnumerable<Item> ExtractReviewableFiles(Changeset changeset)
        {
            var files = from change in changeset.Changes
                where ((change.ChangeType & ChangeType.Edit) == ChangeType.Edit
                       || (change.ChangeType & ChangeType.Add) == ChangeType.Add
                       || (change.ChangeType & ChangeType.Delete) == ChangeType.Delete)
                      && change.Item.ItemType == ItemType.File
                    /* Filter out common items that do not need reviewed */
                      && !change.Item.ServerItem.EndsWith("proj", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".sln", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".dll", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".pdb", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".designer.cs", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".Designer.cs", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".resx", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".xap", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".vssscc", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith(".vspscc", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith("Version.txt", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.EndsWith("LastBuildChangeset.txt", StringComparison.InvariantCulture)
                      && !change.Item.ServerItem.Contains("ClinHelp")
                select change.Item;
            return files;
        }
    }
}