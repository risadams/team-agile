using System;

namespace CAS.TFS.Model.Build
{
    /// <summary>
    ///     Enum BuildReason
    /// </summary>
    [Serializable]
    [Flags]
    public enum BuildReasons
    {
        /// <summary>
        ///     All
        /// </summary>
        All = 255,

        /// <summary>
        ///     The batched ci
        /// </summary>
        BatchedCI = 4,

        /// <summary>
        ///     The check in shelveset
        /// </summary>
        CheckInShelveset = 128,

        /// <summary>
        ///     The individual ci
        /// </summary>
        IndividualCI = 2,

        /// <summary>
        ///     The manual
        /// </summary>
        Manual = 1,

        /// <summary>
        ///     The none
        /// </summary>
        None = 0,

        /// <summary>
        ///     The schedule
        /// </summary>
        Schedule = 8,

        /// <summary>
        ///     The schedule forced
        /// </summary>
        ScheduleForced = 16,

        /// <summary>
        ///     The triggered
        /// </summary>
        Triggered = 191,

        /// <summary>
        ///     The user created
        /// </summary>
        UserCreated = 32,

        /// <summary>
        ///     The validate shelveset
        /// </summary>
        ValidateShelveset = 64,
    }
}