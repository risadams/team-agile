using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CAS.TFS.Model.WorkItems
{
    /// <summary>
    ///     Class WorkItemExtensions.
    /// </summary>
    public static class WorkItemExtensions
    {
        /// <summary>
        ///     returns the first value found in any of the specified <paramref name="fieldNames" /> arry.
        /// </summary>
        /// <remarks>
        ///     This is useful for scenarios in which identical fields have different names in different work item types:
        ///     e.g "TargetRelease" vs "Target Release"
        /// </remarks>
        /// <typeparam name="TPropertyType">The type of the attribute property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <param name="default">The default.</param>
        /// <returns>``0.</returns>
        public static TPropertyType MatchFirstProperty<TPropertyType>(this WorkItem item, string[] fieldNames, TPropertyType @default)
        {
            try
            {
                foreach (var field in fieldNames)
                {
                    TPropertyType val;
                    if (item.TryGetProperty(field, out val))
                        return val;
                }
                return @default;
            }
            catch
            {
                return default(TPropertyType);
            }
        }

        /// <summary>
        ///     Matches the first property.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the t property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>TPropertyType.</returns>
        public static TPropertyType MatchFirstProperty<TPropertyType>(this WorkItem item, string[] fieldNames, Func<object, TPropertyType> transform)
        {
            try
            {
                foreach (var field in fieldNames)
                {
                    TPropertyType val;
                    if (item.TryGetProperty(field, out val))
                        return transform(val);
                }
                return default(TPropertyType);
            }
            catch
            {
                return default(TPropertyType);
            }
        }

        /// <summary>
        ///     Gets a property from the work item.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the attribute property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryGetProperty<TPropertyType>(this WorkItem item, string fieldName, out TPropertyType value)
        {
            try
            {
                value = (TPropertyType)item.Fields[fieldName].Value;
                return true;
            }
            catch
            {
                value = default(TPropertyType);
                return false;
            }
        }

        /// <summary>
        ///     Gets a property from the work item.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the attribute property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>``0.</returns>
        public static TPropertyType GetProperty<TPropertyType>(this WorkItem item, string fieldName)
        {
            try
            {
                return (TPropertyType)item.Fields[fieldName].Value;
            }
            catch
            {
                return default(TPropertyType);
            }
        }

        /// <summary>
        ///     Gets a property from the work item.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the attribute property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="default">The default.</param>
        /// <returns>``0.</returns>
        public static TPropertyType GetProperty<TPropertyType>(this WorkItem item, string fieldName, TPropertyType @default)
        {
            try
            {
                return (TPropertyType)item.Fields[fieldName].Value;
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        ///     Gets a property from the work item.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the attribute property type.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>``0.</returns>
        public static TPropertyType GetProperty<TPropertyType>(this WorkItem item, string fieldName, Func<object, TPropertyType> transform)
        {
            try
            {
                return transform(item.Fields[fieldName].Value);
            }
            catch
            {
                return default(TPropertyType);
            }
        }
    }
}