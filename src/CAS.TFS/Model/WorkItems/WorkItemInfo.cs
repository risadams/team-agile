using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS.Model.WorkItems
{
    /// <summary>
    ///     Class WorkItemInfo.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Id,nq} - {Title,nq}")]
    public class WorkItemInfo
    {
        [NonSerialized]
        private WorkItem _workItem;

        /// <summary>
        ///     Prevents a default instance of the <see cref="WorkItemInfo" /> class from being created.
        /// </summary>
        internal WorkItemInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkItemInfo" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="tfsServerUri">The TFS server URI.</param>
        public WorkItemInfo(WorkItem item, string tfsServerUri = "")
        {
            WorkItem = item;
            Id = item.GetProperty<int>("ID");
            Title = item.GetProperty("Title", string.Empty);
            AssignedTo = item.GetProperty("Assigned To", string.Empty);
            State = item.GetProperty("State", string.Empty);
            ActivatedBy = item.MatchFirstProperty(new[] {"Activated By", "Created By"}, string.Empty);
            CreatedDate = item.GetProperty("Created Date", DateTime.MinValue);
            Priority = item.GetProperty("Priority", 3);
            TeamProject = item.Project.Name;
            WorkItemType = item.GetProperty("Work Item Type", string.Empty);
            Effort = item.MatchFirstProperty(new[] {"Effort - Microsoft Visual Studio Scrum 1_0", "Original Estimate"}, e =>
            {
                double d;
                return double.TryParse(e.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0f;
            });
            RemainingWork = item.GetProperty("Remaining Work", e =>
            {
                double d;
                return double.TryParse(e.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0f;
            });
            WebAccessLink = GetWebAccessLink(tfsServerUri, item.Id);
            FieldReference = SetFields(item.Fields);
        }

        /// <summary>
        ///     Gets the <see cref="System.Object" /> with the specified field name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">No field exists with the the key  + fieldName</exception>
        public object this[string fieldName]
        {
            get
            {
                object @ref;
                if (FieldReference.TryGetValue(fieldName, out @ref))
                    return @ref;

                throw new KeyNotFoundException("No field exists with the the key " + fieldName);
            }
        }

        /// <summary>
        ///     Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int Id { get; internal set; }

        /// <summary>
        ///     Gets or sets the effort.
        /// </summary>
        /// <value>The effort.</value>
        public double Effort { get; set; }

        /// <summary>
        ///     Gets or sets the remaining work.
        /// </summary>
        /// <value>The remaining work.</value>
        public double RemainingWork { get; set; }

        /// <summary>
        ///     Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; internal set; }

        /// <summary>
        ///     Gets the assigned automatic.
        /// </summary>
        /// <value>The assigned automatic.</value>
        public string AssignedTo { get; internal set; }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; internal set; }

        /// <summary>
        ///     Gets the activated by.
        /// </summary>
        /// <value>The activated by.</value>
        public string ActivatedBy { get; private set; }

        /// <summary>
        ///     Gets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; private set; }


        /// <summary>
        ///     Gets the type of the work item.
        /// </summary>
        /// <value>The type of the work item.</value>
        public string WorkItemType { get; internal set; }

        /// <summary>
        ///     Gets the web access link.
        /// </summary>
        /// <value>The web access link.</value>
        public string WebAccessLink { get; private set; }

        /// <summary>
        ///     Gets the team project.
        /// </summary>
        /// <value>The team project.</value>
        public string TeamProject { get; private set; }

        /// <summary>
        ///     Gets the work item.
        /// </summary>
        /// <value>The work item.</value>
        public WorkItem WorkItem
        {
            get { return _workItem; }
            private set { _workItem = value; }
        }

        /// <summary>
        ///     Gets or sets the field reference.
        /// </summary>
        /// <value>The field reference.</value>
        public ReadOnlyDictionary<string, object> FieldReference { get; set; }

        /// <summary>
        ///     Gets the web access link.
        /// </summary>
        /// <param name="tfsServerUri">The TFS server URI.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        private static string GetWebAccessLink(string tfsServerUri, int id)
        {
            return string.IsNullOrEmpty(tfsServerUri)
                ? string.Empty
                : string.Format(CultureInfo.InvariantCulture, "{0}/web/UI/Pages/WorkItems/WorkItemEdit.aspx?id={1}", tfsServerUri, id);
        }

        /// <summary>
        ///     Sets the fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>ReadOnlyDictionary&lt;System.String, System.Object&gt;.</returns>
        private static ReadOnlyDictionary<string, object> SetFields(IEnumerable fields)
        {
            var dictionary = fields.Cast<Field>().ToDictionary(field => field.Name, field => field.Value);
            return new ReadOnlyDictionary<string, object>(dictionary);
        }

        /// <summary>
        ///     Gets a mock work item used only for unit testing.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>WorkItemInfo.</returns>
        public static WorkItemInfo GetMockWorkItem(int id)
        {
            return new WorkItemInfo
            {
                WorkItem = null,
                ActivatedBy = "ActivatedBy" + id,
                Id = id,
                CreatedDate = DateTime.MinValue,
                AssignedTo = "AssignedTo" + id,
                Priority = id,
                State = "State" + id,
                TeamProject = "TeamProject" + id,
                Title = "Title" + id,
                WebAccessLink = "http://site/?id=" + id,
                WorkItemType = "Type" + id
            };
        }
    }
}