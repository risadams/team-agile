using System;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    /// Enum CompilationStatus
    /// </summary>
    [Serializable]
    public enum CompilationStatus
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