// -----------------------------------------------------------------------
//  <copyright file="CommaSeparatedCollectionExpansionFormatter.cs" company="Ris Adams">
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
    ///     String formatter that breaks each comma in the string into a new line
    /// </summary>
    public sealed class CommaSeparatedCollectionExpansionFormatter : IExpansionFormatter
    {
        /// <summary>
        ///     Formats the specified object value according to the expansion rules of the active instance.
        /// </summary>
        /// <remarks>
        ///     Splits all commas and returns a new line for each expanded value
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
                builder.Append(component + "," + Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
