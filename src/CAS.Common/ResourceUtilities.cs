// -----------------------------------------------------------------------
//  <copyright file="ResourceUtilities.cs" company="Ris Adams">
//      Copyright © 2014 Ris Adams. All rights reserved.
//  </copyright>
// <author></author>
// <project>CAS.Common</project>
// -----------------------------------------------------------------------

#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Resources;

#endregion

namespace CAS.Common
{
    /// <summary>
    ///     Class ResourceUtilities.
    ///     Utility for working with Resource files
    /// </summary>
    public static class ResourceUtilities
    {
        /// <summary>
        ///     Gets the memory stream for image resource.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="name">The name.</param>
        /// <returns>MemoryStream.</returns>
        /// <exception cref="System.ArgumentNullException">resourceManager</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Unable to create memory stream from image resource
        /// </exception>
        public static MemoryStream GetMemoryStreamForImageResource(this ResourceManager resourceManager, string name)
        {
            if (resourceManager == null)
                throw new ArgumentNullException("resourceManager");

            var image = resourceManager.GetObject(name) as Image;
            if (image == null)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    "Resource could not be located: {0}", name));

            var b = new Bitmap(image);
            var ic = new ImageConverter();
            var ba = (Byte[]) ic.ConvertTo(b, typeof (Byte[]));
            return ba != null ? new MemoryStream(ba) : null;
        }

        /// <summary>
        ///     Gets the base64 encoding for image resource.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">resourceManager</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static string GetBase64EncodingForImageResource(this ResourceManager resourceManager, string name)
        {
            if (resourceManager == null)
                throw new ArgumentNullException("resourceManager");

            var image = resourceManager.GetObject(name) as Image;
            if (image == null)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    "Resource could not be located: {0}", name));

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);

            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     Loads the base64 image from resource.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>System.String.</returns>
        public static string LoadBase64ImageFromResource(ResourceManager resourceManager, string resourceName)
        {
            return resourceManager.GetBase64EncodingForImageResource(resourceName);
        }

        /// <summary>
        ///     Loads the image stream from resource.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream LoadImageStreamFromResource(ResourceManager resourceManager, string resourceName)
        {
            return resourceManager.GetMemoryStreamForImageResource(resourceName);
        }

        /// <summary>
        ///     Gets the base64 encoding for image memory stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        public static string GetBase64EncodingForImageMemoryStream(MemoryStream stream)
        {
            var bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     Gets the image memory stream from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        public static MemoryStream GetImageMemoryStreamFromStream(MemoryStream stream)
        {
            var b = Image.FromStream(stream);
            var ic = new ImageConverter();
            var ba = (Byte[]) ic.ConvertTo(b, typeof (Byte[]));
            return ba != null ? new MemoryStream(ba) : null;
        }

        /// <summary>
        ///     Loads an image from a base64 string.
        /// </summary>
        /// <param name="base64">The base64 string.</param>
        /// <returns>the Image.</returns>
        public static Image LoadImageFromBase64String(string base64)
        {
            var bytes = Convert.FromBase64String(base64);

            Image image;
            using (var ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }
    }
}
