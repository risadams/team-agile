// -----------------------------------------------------------------------
//  <copyright file="EnumAttributeExtensions.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using CAS.Common.Attributes.EncoreTeamUtilities.Common.Attributes;

#endregion

namespace CAS.Common.Attributes
{
    /// <summary />
    public static class EnumAttributeExtensions
    {
        /// <summary>
        ///     Attach a description to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string AssociatedText(this Enum value)
        {
            var attributes = (AssociatedTextAttribute[]) value
                .GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof (AssociatedTextAttribute), false);
            return attributes.Length > 0 ? attributes[0].Text : null;
        }
    }
}
