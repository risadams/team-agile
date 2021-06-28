using System;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    /// Enum BuildEnabledStatus
    /// </summary>
    [Serializable]
    public enum BuildEnabledStatus
    {
        /// <summary>
        ///     enabled
        /// </summary>
        Enabled = 0x00,

        /// <summary>
        ///     paused
        /// </summary>
        Paused = 0x01,

        /// <summary>
        ///     disabled
        /// </summary>
        Disabled = 0x02
    }
}