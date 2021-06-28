// -----------------------------------------------------------------------
//  <copyright file="UserNotFoundException.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.DirectoryServices</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Runtime.Serialization;

#endregion

namespace CAS.DirectoryServices
{
    /// <summary>
    ///     Class UserNotFoundException.
    /// </summary>
    [Serializable]
    public class UserNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class.
        /// </summary>
        public UserNotFoundException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UserNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class with serialized data.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected UserNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
