// -----------------------------------------------------------------------
//  <copyright file="PropertyReportingExtensions.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#endregion

namespace CAS.Common.Attributes
{
    /// <summary>
    ///     Class PropertyReportingExtensions.
    ///     Retrieves a list of property name value pairs.
    /// </summary>
    public static class PropertyReportingExtensions
    {
        /// <summary>
        ///     Reports the values of each property decorated with the <see cref="ReportablePropertyAttribute" /> formatted
        ///     according to the appropriate format value.
        /// </summary>
        /// <param name="instance">The object instance to report upon.</param>
        /// <returns>All formatted property values</returns>
        public static IEnumerable<Tuple<string, string, string>> ReportPropertyValues(this object instance)
        {
            var type = instance.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return from property in properties
                let attributes = property.GetCustomAttributes(typeof (ReportablePropertyAttribute), false)
                where attributes.Length != 0
                let value = ((ReportablePropertyAttribute) attributes[0]).Report(property.GetValue(instance))
                select new Tuple<string, string, string>(property.Name, property.GetDisplayNameAttributeValue(), value);
        }

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayNameAttributeValue(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof (DisplayNameAttribute), false);
            return attributes.Length == 0 ? property.Name : ((DisplayNameAttribute) attributes[0]).DisplayName;
        }
    }
}
