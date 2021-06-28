using System;
using System.Xml.Serialization;

namespace CAS.TFS.Model.Query
{
    /// <summary>
    ///     Class WorkItemQuery.
    /// </summary>
    [Serializable]
    public class WorkItemQuery
    {
        private int _version = 1;

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute]
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        ///     Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        [XmlElement("Wiql")]
        public string Query { get; set; }
    }
}