// -----------------------------------------------------------------------
//  <copyright file="XmlUtilities.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

#endregion

namespace CAS.Common
{
    /// <summary>
    /// Class XmlUtilities.
    /// </summary>
    public static class XmlUtilities
    {
        /// <summary>
        ///     Loads an XML XDocument class from a stream.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>XDocument.</returns>
        public static XDocument LoadXmlFromStream(Stream value)
        {
            return XDocument.Load(value);
        }

        /// <summary>
        ///     Gets the attribute value or specified default if the attribute does not exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="default">The default.</param>
        /// <returns>the retrieved value or default</returns>
        public static string GetAttributeValueOrDefault(XElement element, string attribute, string @default)
        {
            if (element.HasAttributes && element.Attribute(attribute) != null)
                return element.Attribute(attribute).Value;

            return @default;
        }

        /// <summary>
        ///     Serializes the object to XML string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        public static string SerializeObjectToXmlString(object obj, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = new UTF8Encoding(false);

            return Serialize(obj, encoding);
        }

        /// <summary>
        ///     Serializes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        private static string Serialize(object o, Encoding encoding)
        {
            return Serialize(o, null, encoding);
        }

        /// <summary>
        ///     Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static string Serialize(object obj, XmlSerializer serializer, Encoding encoding)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (serializer == null)
            {
                serializer = new XmlSerializer(obj.GetType());
            }
            if (encoding == null)
                encoding = new UnicodeEncoding(false, false);

            var memoryStream = new MemoryStream();
            using (var xmlTextWriter = new XmlTextWriter(memoryStream, encoding))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                serializer.Serialize(xmlTextWriter, obj);
            }
            return encoding.GetString(memoryStream.ToArray());
        }

        /// <summary>
        ///     To the XDocument.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>XDocument.</returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var reader = new XmlNodeReader(xmlDocument))
            {
                reader.MoveToContent();
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        ///     To the XML document.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <returns>XmlDocument.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var doc = new XmlDocument();
            using (var reader = xDocument.CreateReader())
            {
                doc.Load(reader);
            }
            return doc;
        }
    }
}
