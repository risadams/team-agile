// -----------------------------------------------------------------------
//  <copyright file="IExpansionFormatter.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

namespace CAS.Common.Attributes
{
    /// <summary>
    ///     Behavioral utility which formats an object (generally a string, or object's .ToString() value) using the specific
    ///     implementation of the formatter
    /// </summary>
    public interface IExpansionFormatter
    {
        /// <summary>
        ///     Formats the specified object value according to the expansion rules of the active instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        string Format(object value);
    }
}
