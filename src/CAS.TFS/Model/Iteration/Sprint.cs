using System;
using System.Diagnostics;
using System.Globalization;
using CAS.Common;

namespace CAS.TFS.Model.Iteration
{
    /// <summary>
    /// Class Sprint.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Name,nq}, [{StartDate.ToShortDateString(),nq} - {EndDate.ToShortDateString(),nq}]")]
    public class Sprint
    {
        private string _name;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _name ?? (_name = string.IsNullOrEmpty(Iteration)
                    ? string.Empty
                    : ParseSprintName(Iteration.ReturnLastOccurrence('\\')));
            }
            set { _name = value; }
        }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the iteration.
        /// </summary>
        /// <value>The iteration.</value>
        public string Iteration { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}, [{1} - {2}]", Name, StartDate.ToShortDateString(), EndDate.ToShortDateString());
        }

        /// <summary>
        ///     Parses the name of the sprint.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private static string ParseSprintName(string value)
        {
            var split = value.Split(new[] { ' ' });
            if (split.Length >= 2)
                return split[0] + " " + split[1];
            return value;
        }
    }
}