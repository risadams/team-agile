#region References

using System;

#endregion

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Enum BuildTriggers
    /// </summary>
    [Flags]
    [Serializable]
    public enum BuildTriggers
    {
        /// <summary>
        ///     The unknown
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        ///     All
        /// </summary>
        All = 0x7f,

        /// <summary>
        ///     The batched continuous integration
        /// </summary>
        BatchedContinuousIntegration = 0x4,

        /// <summary>
        ///     The batched gated check in
        /// </summary>
        BatchedGatedCheckIn = 0x40,

        /// <summary>
        ///     The continuous integration
        /// </summary>
        ContinuousIntegration = 0x2,

        /// <summary>
        ///     The gated check in
        /// </summary>
        GatedCheckIn = 0x20,

        /// <summary>
        ///     The none
        /// </summary>
        None = 0x1,

        /// <summary>
        ///     The schedule
        /// </summary>
        Schedule = 0x8,

        /// <summary>
        ///     The schedule forced
        /// </summary>
        ScheduleForced = 0x10,
    }
}