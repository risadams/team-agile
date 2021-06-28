using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CAS.TFS
{
    /// <summary>
    /// Interface ILabelController
    /// </summary>
    public interface ILabelController : ITeamController
    {
          /// <summary>
        ///     Gets the label reference specified by a name and scope.
        /// </summary>
        /// <param name="labelName">Name of the label.</param>
        /// <param name="labelScope">The label scope.</param>
        /// <returns>the populated label object.</returns>
        VersionControlLabel GetLabel(string labelName, string labelScope);

        /// <summary>
        ///     Gets the label reference specified by label specification.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>the populated label object.</returns>
        VersionControlLabel GetLabel(VersionControlLabel label);

        /// <summary>
        ///     Gets the labels associated with the specified item.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <param name="owner">The label owner.</param>
        /// <returns>IEnumerable{VersionControlLabel}.</returns>
        IEnumerable<VersionControlLabel> GetItemLabels(Item item, string owner);

        /// <summary>
        ///     Gets the changesets contained by the label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>SortedDictionary{System.Int32,Changeset}.</returns>
        SortedDictionary<int, Changeset> GetLabelChangesets(VersionControlLabel label);

        /// <summary>
        ///     Gets the changesets modified between two labels.
        /// </summary>
        /// <param name="previousLabel">The previous label.</param>
        /// <param name="currentLabel">The current label.</param>
        /// <returns>IEnumerable{Changeset}.</returns>
        IEnumerable<Changeset> GetChangesetsModifiedSinceLabel(VersionControlLabel previousLabel, VersionControlLabel currentLabel);
    }
}