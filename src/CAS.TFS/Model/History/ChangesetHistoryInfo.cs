using System;
using System.Linq;
using CAS.TFS.Model.WorkItems;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CAS.TFS.Model.History
{
    /// <summary>
    /// Class ChangesetHistoryInfo.
    /// </summary>
    [Serializable]
    public class ChangesetHistoryInfo
    {
        /// <summary>
        ///     The value displayed to users as a default if no policy override comment was entered by the user.
        /// </summary>
        private const string NO_OVERRIDE_MESSAGE = " --- NO OVERRIDE REASON SPECIFIED ---";

        [NonSerialized]
        private Changeset _changeset;

        /// <summary>
        ///     Prevents a default instance of the <see cref="ChangesetHistoryInfo" /> class from being created.
        /// </summary>
        private ChangesetHistoryInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangesetHistoryInfo" /> class.
        /// </summary>
        /// <param name="changeset">The changeset.</param>
        public ChangesetHistoryInfo(Changeset changeset)
            : this()
        {
            Changeset = changeset;
            ChangesetId = changeset.ChangesetId;
            Owner = changeset.Owner;
            Date = changeset.CreationDate;
            Comment = string.Empty;
            //TODO: Infer from server URI
            //WebAccessLink = string.Format(CultureInfo.InvariantCulture, "WEBACCESSLINK:8080/tfs/web/UI/Pages/Scc/ViewChangeset.aspx?changeset={0}", ChangesetId);

            if (!string.IsNullOrEmpty(changeset.Comment))
                Comment = changeset.Comment;

            if (changeset.PolicyOverride != null)
            {
                OverrideReason = changeset.PolicyOverride.Comment ?? NO_OVERRIDE_MESSAGE;
            }

            WorkItems = new WorkItemInfoCollection();
            foreach (var awi in changeset.AssociatedWorkItems)
            {
                WorkItems.Add(new WorkItemInfo
                {
                    AssignedTo = awi.AssignedTo,
                    Id = awi.Id,
                    State = awi.State,
                    Title = awi.Title,
                    WorkItemType = awi.WorkItemType
                });
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this changeset violates the policy override failure conditions.
        /// </summary>
        /// <remarks>
        ///     If item was associated to a work item, this changeset is not in violation
        ///     If item was not associated to a work item, this changeset MAY be in violation, unless an override comment was
        ///     provided
        /// </remarks>
        /// <value><c>true</c> if in this changeset is in violation; otherwise, <c>false</c>.</value>
        public bool ViolatesPolicyOverrideFailure
        {
            get
            {
                if (WorkItems.Any()) return false;
                return OverrideReason == NO_OVERRIDE_MESSAGE;
            }
        }

        /// <summary>
        ///     Gets or sets the web access link.
        /// </summary>
        /// <value>The web access link.</value>
        public string WebAccessLink { get; set; }

        /// <summary>
        ///     Gets the changeset unique identifier.
        /// </summary>
        /// <value>The changeset unique identifier.</value>
        public int ChangesetId { get; private set; }

        /// <summary>
        ///     Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public string Owner { get; private set; }

        /// <summary>
        ///     Gets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; private set; }

        /// <summary>
        ///     Gets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; private set; }

        /// <summary>
        ///     Gets the TFS changeset reference.
        /// </summary>
        /// <remarks>This value is not serialized.</remarks>
        /// <value>The changeset.</value>
        public Changeset Changeset
        {
            get { return _changeset; }
            private set { _changeset = value; }
        }

        /// <summary>
        ///     Gets or sets the work items.
        /// </summary>
        /// <value>The work items.</value>
        public WorkItemInfoCollection WorkItems { get; set; }

        /// <summary>
        ///     Gets or sets the override reason.
        /// </summary>
        /// <value>The override reason.</value>
        public string OverrideReason { get; set; }
    }
}