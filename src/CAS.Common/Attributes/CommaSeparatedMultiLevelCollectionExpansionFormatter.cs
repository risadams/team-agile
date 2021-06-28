// -----------------------------------------------------------------------
//  <copyright file="CommaSeparatedMultiLevelCollectionExpansionFormatter.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

using System;
using System.Text;

namespace CAS.Common.Attributes
{
    /// <summary>
    ///     String formatter that breaks each comma in the string into a new line, also adds a space after each semi-colon
    /// </summary>
    public sealed class CommaSeparatedMultiLevelCollectionExpansionFormatter : IExpansionFormatter
    {
        /// <summary>
        ///     Formats the specified object value according to the expansion rules of the active instance.
        /// </summary>
        /// <remarks>
        ///     Splits commas, returns each value on a new line
        ///     within each line, replace all semi-colons with spaces
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public string Format(object value)
        {
            var str = value.ToString();
            var builder = new StringBuilder();
            var split = str.Split(new[] { ',' });
            foreach (var component in split)
            {
                builder.Append(component.Replace(';', ' ') + Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
