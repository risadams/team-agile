using System;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Enum BuildStatus
    /// </summary>
    [Flags]
    [Serializable]
    public enum BuildStatus
    {
        /// <summary>
        ///     The none
        /// </summary>
        None = 0x0,

        /// <summary>
        ///     The in progress
        /// </summary>
        InProgress = 0x1,

        /// <summary>
        ///     The succeeded
        /// </summary>
        Succeeded = 0x2,

        /// <summary>
        ///     The partially succeeded
        /// </summary>
        PartiallySucceeded = 0x4,

        /// <summary>
        ///     The failed
        /// </summary>
        Failed = 0x8,

        /// <summary>
        ///     The stopped
        /// </summary>
        Stopped = 0x10,

        /// <summary>
        ///     The not started
        /// </summary>
        NotStarted = 0x20,

        /// <summary>
        ///     All
        /// </summary>
        All = 0x3F,
    }
}