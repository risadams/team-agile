using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace CAS.TFS.Controllers
{
    /// <summary>
    /// Class LabelController.
    /// </summary>
    public class LabelController : TeamController, ILabelController
    {
        /// <summary>
        ///     Initializes a new instance of the class.
        /// </summary>
        /// <param name="controller">The <seealso cref="ITeamController" /> base team controller instance.</param>
        public LabelController(TeamController controller)
            : base(controller)
        {
        }

        /// <summary>
        ///     Gets the label reference specified by a name and scope.
        /// </summary>
        /// <param name="labelName">Name of the label.</param>
        /// <param name="labelScope">The label scope.</param>
        /// <returns>the populated label object.</returns>
        public VersionControlLabel GetLabel(string labelName, string labelScope)
        {
            return VersionControl.QueryLabels(labelName, labelScope, null, true).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the label reference specified by label specification.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>the populated label object.</returns>
        public VersionControlLabel GetLabel(VersionControlLabel label)
        {
            return VersionControl.QueryLabels(label.Name, label.Scope, label.OwnerName, true).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the labels associated with the specified item.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <param name="owner">The label owner.</param>
        /// <returns>IEnumerable{VersionControlLabel}.</returns>
        public IEnumerable<VersionControlLabel> GetItemLabels(Item item, string owner)
        {
            return VersionControl.QueryLabels(null, "$/" + VersionControlPath.GetTeamProjectName(item.ServerItem), owner, true, item.ServerItem, VersionSpec.Latest, false);
        }

        /// <summary>
        ///     Gets the changesets contained by the label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>SortedDictionary{System.Int32Changeset}.</returns>
        public SortedDictionary<int, Changeset> GetLabelChangesets(VersionControlLabel label)
        {
            var changesets = new SortedDictionary<int, Changeset>();
            foreach (var item in label.Items)
            {
                if (!changesets.ContainsKey(item.ChangesetId))
                    changesets[item.ChangesetId] = VersionControl.GetChangeset(item.ChangesetId, false, false);
            }
            return changesets;
        }

        /// <summary>
        ///     Gets the changesets modified between two labels.
        /// </summary>
        /// <param name="previousLabel">The previous label.</param>
        /// <param name="currentLabel">The current label.</param>
        /// <returns>IEnumerable{Changeset}.</returns>
        public IEnumerable<Changeset> GetChangesetsModifiedSinceLabel(VersionControlLabel previousLabel, VersionControlLabel currentLabel)
        {
            var previousLabelChanges = GetLabelChangesets(previousLabel);
            var currentLabelChanges = GetLabelChangesets(currentLabel);

            return from changesetId in currentLabelChanges.Keys
                   where !previousLabelChanges.ContainsKey(changesetId)
                   select currentLabelChanges[changesetId];
        }
    }
}