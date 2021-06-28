// -----------------------------------------------------------------------
//  <copyright file="DateTimeUtilities.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Globalization;

#endregion

namespace CAS.Common
{
    /// <summary>
    ///     Class DateTimeUtilities.
    ///     Utility methods for working with DateTime types
    /// </summary>
    public static class DateTimeUtilities
    {
        /// <summary>
        ///     The date time format used by Team Foundation Server
        /// </summary>
        /// <example>01-NOV-2013 08:00:00</example>
        public const string DateTimeFormat = @"dd-MMM-yyyy HH:mm:ss";

        /// <summary>
        ///     The date format used by Team Foundation Server
        /// </summary>
        /// <example>01-NOV-2013</example>
        public const string DateFormat = @"dd-MMM-yyyy";

        /// <summary>
        ///     Parses the date time using the invariant format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if the date was parsed, <c>false</c> otherwise.</returns>
        public static bool ParseDateTimeInvariant(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value, DateTimeFormat, DateTimeFormatInfo.InvariantInfo,
                DateTimeStyles.AssumeLocal, out date);
        }

        /// <summary>
        ///     Formats the date time using the invariant format.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string FormatDateTimeInvariant(this DateTime date)
        {
            return date.ToString(DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        ///     Formats the date using the invariant format.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string FormatDateInvariant(this DateTime date)
        {
            return date.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo);
        }
    }
}
