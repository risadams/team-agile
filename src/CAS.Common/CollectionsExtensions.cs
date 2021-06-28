// -----------------------------------------------------------------------
//  <copyright file="CollectionsExtensions.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace CAS.Common
{
    /// <summary>
    ///     Class CollectionsExtensions.
    ///     Utility methods for working with ICollection and IDictionary types
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        ///     Thread safe add method for generic collections, will only add unique value to the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="value">The value.</param>
        public static void ThreadSafeAddUnique<T>(this ICollection<T> collection, T value)
        {
            if (collection.Contains(value))
                return;

            collection.ThreadSafeAdd(value);
        }

        /// <summary>
        ///     Thread safe remove method for generic collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public static void ThreadSafeRemove<T>(this ICollection<T> collection, T item)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var locked = false;
            try
            {
                Monitor.Enter(collection, ref locked);
                collection.Remove(item);
            }
            finally
            {
                if (locked)
                    Monitor.Exit(collection);
            }
        }

        /// <summary>
        ///     Thread safe add method for generic collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public static void ThreadSafeAdd<T>(this ICollection<T> collection, T item)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var locked = false;
            try
            {
                Monitor.Enter(collection, ref locked);
                collection.Add(item);
            }
            finally
            {
                if (locked)
                    Monitor.Exit(collection);
            }
        }

        /// <summary>
        ///     Conditionally adds the specified item to the collection.
        /// </summary>
        /// <typeparam name="TType">The type of the t type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="predicate">The predicate.</param>
        public static void ConditionalAdd<TType>(this ICollection<TType> collection, TType item,
            Func<ICollection<TType>, TType, bool> predicate)
        {
            if (predicate(collection, item))
                collection.Add(item);
        }

        /// <summary>
        ///     Attempts to add a value to a dictionary, if the key being added already exists the value will be updated instead.
        /// </summary>
        /// <typeparam name="TKey">The type of the attribute key.</typeparam>
        /// <typeparam name="TValue">The type of the attribute value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        /// <summary>
        ///     Attempts to increment the integer value of a dictionary based upon the key.
        ///     if the key does not exist it will be added with the default value
        ///     useful for aggregating group by values
        /// </summary>
        /// <typeparam name="TKey">The type of the attribute key.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="incrementBy">The integer value to increment by. (default 1)</param>
        /// <param name="defaultValue">The default value when adding new keys. (default 1)</param>
        public static void AddOrIncrement<TKey>(this IDictionary<TKey, int> dictionary, TKey key, int incrementBy = 0x01,
            int defaultValue = 0x01)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += incrementBy;
            else
                dictionary.Add(key, defaultValue);
        }
    }
}
