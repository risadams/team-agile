// -----------------------------------------------------------------------
//  <copyright file="LinqExtensions.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System.Collections.Generic;

#endregion

namespace CAS.Common
{
    /// <summary>
    /// Class LinqExtensions.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        ///     if default is returned, return a specified value
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="alternate">The alternate.</param>
        /// <returns>T.</returns>
        public static TType IfDefault<TType>(this TType value, TType alternate)
        {
            return value.Equals(default(TType)) ? alternate : value;
        }

        /// <summary>
        ///     Firsts the or.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="alternate">The alternate.</param>
        /// <returns>T.</returns>
        public static TType FirstOr<TType>(this IEnumerable<TType> source, TType alternate)
        {
            foreach (var item in source)
                return item;
            return alternate;
        }
    }
}
