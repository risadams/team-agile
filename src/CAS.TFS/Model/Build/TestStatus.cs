using System;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    /// Enum TestStatus
    /// </summary>
    [Serializable]
    public enum TestStatus
    {
        /// <summary>
        ///     The unknown
        /// </summary>
        Unknown,

        /// <summary>
        ///     The failed
        /// </summary>
        Failed,

        /// <summary>
        ///     The succeeded
        /// </summary>
        Succeeded,
    }
}