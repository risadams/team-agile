// -----------------------------------------------------------------------
//  <copyright file="MemoryStreamSerializer.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace CAS.Common
{
    /// <summary>
    ///     Class MemoryStreamSerializer.
    ///     Utility for serializing to and from a MemoryStream object
    /// </summary>
    public static class MemoryStreamSerializer
    {
        /// <summary>
        ///     Serializes an object to a memory stream.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream SerializeToStream(object o)
        {
            var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        /// <summary>
        ///     Deserializes an object from a memory stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.Object.</returns>
        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            var o = formatter.Deserialize(stream);
            return o;
        }
    }
}
