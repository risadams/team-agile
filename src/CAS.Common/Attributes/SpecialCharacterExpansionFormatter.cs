// -----------------------------------------------------------------------
//  <copyright file="SpecialCharacterExpansionFormatter.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

namespace CAS.Common.Attributes
{
    /// <summary>
    ///     Expands several special characters while formatting.
    ///     adds a spaces after comma and semi-colon
    ///     removes quotes
    /// </summary>
    public sealed class SpecialCharacterExpansionFormatter : IExpansionFormatter
    {
        /// <summary>
        ///     Formats the specified object value according to the expansion rules of the active instance.
        /// </summary>
        /// <remarks>
        ///     appends all instances of certain special characters (, ;) with an additional space
        ///     removes all double-quotations
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public string Format(object value)
        {
            var str = value.ToString();
            return str
                .Replace(",", ", ")
                .Replace(";", "; ")
                .Replace("\"", "");
        }
    }
}
