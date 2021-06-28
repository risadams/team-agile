// -----------------------------------------------------------------------
//  <copyright file="AssociatedTextAttribute.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;

#endregion

namespace CAS.Common.Attributes
{
    namespace EncoreTeamUtilities.Common.Attributes
    {
        /// <summary>
        ///     Attribute for attaching text to enumerations
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public sealed class AssociatedTextAttribute : Attribute
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="AssociatedTextAttribute" /> class.
            /// </summary>
            /// <param name="text">The text.</param>
            public AssociatedTextAttribute(string text)
            {
                Text = text;
            }

            /// <summary>
            ///     Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            public string Text { get; private set; }
        }
    }
}
