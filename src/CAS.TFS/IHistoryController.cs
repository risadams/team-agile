using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CAS.TFS.Model.History;
using CAS.TFS.Model.SourceControl;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CAS.TFS
{
    /// <summary>
    /// Interface IHistoryController
    /// </summary>
    public interface IHistoryController : ITeamController
    {
        /// <summary>
        ///     Gets the changeset with the specified id.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>Changeset.</returns>
        Changeset GetChangeset(int id);

        /// <summary>
        ///     Gets the files modified by the specified changesets.
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>Dictionary{System.Int32IEnumerable{Item}}.</returns>
        Dictionary<int, IEnumerable<Item>> GetFilesForChangesets(IEnumerable<int> changesetIds);

        /// <summary>
        ///     gets the files associated with the specified changesets, filter out known file extensions not normally reviewed
        ///     during a code review
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>SortedDictionary{System.StringList{System.Int32}}.</returns>
        SortedDictionary<string, List<int>> ChangedFileReviewList(IList<int> changesetIds);

        /// <summary>
        ///     Gets the file content stream.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>Stream.</returns>
        Stream GetFileContentStream(string serverPath, int changeSet);

        /// <summary>
        ///     Gets the file content stream.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <returns>Stream.</returns>
        Stream GetFileContentStream(string serverPath);

        /// <summary>
        ///     Gets the image file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>Image.</returns>
        Image GetImageFileContents(string serverPath, int changeSet);

        /// <summary>
        ///     Gets the file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="changeSet">The change set.</param>
        /// <returns>System.String.</returns>
        string GetFileContents(string serverPath, int changeSet);

        /// <summary>
        ///     Gets the file contents.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <returns>System.String.</returns>
        string GetFileContents(string serverPath);

        /// <summary>
        ///     Gets the changeset owners for each of the specified changesets.
        /// </summary>
        /// <param name="changesetIds">The changeset ids.</param>
        /// <returns>IEnumerable{System.String}.</returns>
        IEnumerable<string> GetChangesetOwners(IList<int> changesetIds);

        /// <summary>
        ///     Gets the changeset owner for the specified changeset.
        /// </summary>
        /// <param name="changesetId">The changeset unique identifier.</param>
        /// <returns>System.String.</returns>
        string GetChangesetOwner(int changesetId);

        //IEnumerable<Changeset> SearchHistory(Item item, string userName);

        /// <summary>
        ///     Searches the changeset history information for the specified server item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>IEnumerable{ChangesetHistoryInfo}.</returns>
        IEnumerable<ChangesetHistoryInfo> SearchHistoryInfo(Item item, string userName);

        /// <summary>
        ///     Searches the changeset history information for the specified server item.
        ///     limits the search to changesets occurring after the specified minimum date
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <returns>IEnumerable{ChangesetHistoryInfo}.</returns>
        IEnumerable<ChangesetHistoryInfo> SearchHistoryInfo(Item item, string userName, DateTime minDate);

        /// <summary>
        ///     Gets the first level items in the TFS hierarchy.
        /// </summary>
        /// <returns>IEnumerable{Item}.</returns>
        IEnumerable<Item> GetFirstLevelItems();

        /// <summary>
        ///     Gets the sub items of the specified server item in the TFS hierarchy.
        /// </summary>
        /// <param name="serverItem">The server item.</param>
        /// <returns>IEnumerable{Item}.</returns>
        IEnumerable<Item> GetSubItems(string serverItem);

        /// <summary>
        ///     Gets a list of all changesets between the specified start and end date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>the list of all changesets between the specified start and end date range.</returns>
        ChangesetHistoryInfoCollection GetChangesetsByDate(DateTime startDate, DateTime endDate);

        /// <summary>
        ///     Recursively gets all children of the specified root and return any item which is a folder.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>IEnumerable{Item}.</returns>
        IEnumerable<Item> GetSubFoldersRecursive(string root);

        /// <summary>
        ///     Gets the branch hierarchy.
        /// </summary>
        /// <param name="includeDeleted">if set to <c>true</c> deleted branches will be included.</param>
        /// <returns>BranchTree.</returns>
        IEnumerable<BranchInfo> GetFullBranchList(bool includeDeleted = false);
    }
}