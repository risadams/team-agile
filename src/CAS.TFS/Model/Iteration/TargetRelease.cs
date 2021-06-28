using System;
using System.Globalization;

namespace CAS.TFS.Model.Iteration
{
    /// <summary>
    /// Class TargetRelease.
    /// </summary>
    [Serializable]
    public class TargetRelease : Tuple<string, string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetRelease" /> class.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="version">The version.</param>
        public TargetRelease(string product, string version)
            : base(product, version)
        {
        }

        /// <summary>
        ///     Gets the empty.
        /// </summary>
        /// <value>The empty.</value>
        public static TargetRelease Empty
        {
            get { return new TargetRelease("Unprioritized", string.Empty); }
        }

        /// <summary>
        ///     Gets the product.
        /// </summary>
        /// <value>The product.</value>
        public string Product
        {
            get { return Item1; }
        }

        /// <summary>
        ///     Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get { return Item2; }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1}", Product, Version);
        }

        /// <summary>
        ///     Parses the specified target release value from a string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>TargetRelease.</returns>
        public static TargetRelease Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Empty;

            var chunks = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return chunks.Length == 1
                ? new TargetRelease(chunks[0], null)
                : new TargetRelease(chunks[0].Trim(), chunks[1].Trim());
        }
    }
}