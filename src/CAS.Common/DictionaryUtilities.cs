// -----------------------------------------------------------------------
//  <copyright file="DictionaryUtilities.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace CAS.Common
{
    /// <summary>
    ///     Class DictionaryUtilities.
    /// </summary>
    public static class DictionaryUtilities
    {
        /// <summary>
        ///     Merges the specified target.
        /// </summary>
        /// <typeparam name="TDict">The type of the t dictionary.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="sources">The sources.</param>
        /// <returns>TDict.</returns>
        public static TDict Merge<TDict, TKey, TValue>(this TDict target, params IDictionary<TKey, TValue>[] sources)
            where TDict : IDictionary<TKey, TValue>, new()
        {
            var map = new TDict();
            foreach (var p in (new List<IDictionary<TKey, TValue>> {target}).Concat(sources).SelectMany(src => src))
            {
                map[p.Key] = p.Value;
            }
            return map;
        }

        /// <summary>
        ///     Appends the specified target.
        /// </summary>
        /// <typeparam name="TDict">The type of the t dictionary.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="sources">The sources.</param>
        public static void Append<TDict, TKey, TValue>(this TDict target, params IDictionary<TKey, TValue>[] sources)
            where TDict : IDictionary<TKey, TValue>, new()
        {
            if (target == null)
                return;

            foreach (var source in sources)
            {
                foreach (var key in source.Keys)
                {
                    if (!target.ContainsKey(key))
                        target.Add(key, source[key]);
                    else
                    {
                        target[key] = source[key];
                    }
                }
            }
        }
    }
}
