using System;
using CAS.Common.Attributes.EncoreTeamUtilities.Common.Attributes;

namespace CAS.TFS.Model
{
    /// <summary>
    /// Enum TfsWorkItemType
    /// </summary>
    [Serializable]
    public enum TfsWorkItemType
    {
        /// <summary>
        ///     The task
        /// </summary>
        [AssociatedText("Task")]
        Task,

        /// <summary>
        ///     The pbi
        /// </summary>
        [AssociatedText("Product Backlog Item")]
        PBI,

        /// <summary>
        ///     The defect
        /// </summary>
        [AssociatedText("Bug")]
        Defect,

        /// <summary>
        ///     The feature
        /// </summary>
        [AssociatedText("Feature")]
        Feature,

        /// <summary>
        ///     The impediment
        /// </summary>
        [AssociatedText("Impediment")]
        Impediment
    }
}