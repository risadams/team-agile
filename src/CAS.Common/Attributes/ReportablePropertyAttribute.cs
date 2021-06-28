// -----------------------------------------------------------------------
//  <copyright file="ReportablePropertyAttribute.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Globalization;
using System.Reflection;

#endregion

namespace CAS.Common.Attributes
{
    /// <summary>
    ///     Class ReportablePropertyAttribute.
    ///     Identifies a property within a class that will be reportable upon request
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ReportablePropertyAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportablePropertyAttribute" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        public ReportablePropertyAttribute(string format = "{0}")
        {
            FormatString = format;
            UseFunction = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportablePropertyAttribute" /> class.
        /// </summary>
        /// <param name="formatterType">Type of the formatter.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     (specified type does not implement <see cref="IExpansionFormatter" />)
        ///     Formatter must be of type IExpansionFormatter
        ///     or
        ///     (specified type does not have accessible constructor)
        ///     Formatter must have accessible default constructor: formatterType.Name
        /// </exception>
        public ReportablePropertyAttribute(Type formatterType)
        {
            var @interface = formatterType.GetInterface("IExpansionFormatter");
            if (@interface == null)
                throw new InvalidOperationException("Formatter must be of type IExpansionFormatter");

            var constructor = formatterType.GetConstructor(new Type[] {});
            if (constructor == null)
                throw new InvalidOperationException("Formatter must have accessible default constructor: " +
                                                    formatterType.Name);

            var instance = constructor.Invoke(BindingFlags.Default, null, null, CultureInfo.CurrentCulture);
            Formatter = (IExpansionFormatter) instance;
            UseFunction = true;
        }

        /// <summary>
        ///     Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        private string FormatString { get; set; }

        /// <summary>
        ///     Gets or sets the format function.
        /// </summary>
        /// <value>The format function.</value>
        private IExpansionFormatter Formatter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use the function instead of the simple format string.
        /// </summary>
        /// <value><c>true</c> if function will be used; otherwise, <c>false</c>.</value>
        private bool UseFunction { get; set; }

        /// <summary>
        ///     Reports the specified value using the format string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public string Report(object value)
        {
            if (UseFunction)
                return Formatter.Format(value);

            return value != null && !string.IsNullOrEmpty(value.ToString())
                ? string.Format(CultureInfo.CurrentCulture, FormatString, value)
                : "Not Specified";
        }
    }
}
