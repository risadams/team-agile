#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace CAS.Common
{
    /// <summary>
    /// Class StringUtilities.
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        ///     Returns the first letter of each reverse-split (on comma) value in the list.
        ///     useful for converting names to initials (e.g. last,first =&gt; fi.la.)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="numLength">number of initials to include in the output.</param>
        /// <returns>System.String.</returns>
        public static string ReverseSplitAndInitialize(this string value, int numLength = 2)
        {
            var builder = new StringBuilder();
            var parts = value.Split(new[] {','});
            foreach (var arr in parts.Reverse().Select(part => part.Trim().ToCharArray()))
            {
                var currLen = 0;

                while (++currLen <= numLength)
                {
                    if (arr.Length >= currLen)
                        builder.Append(arr[currLen - 1]);
                }
                builder.Append(".");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Parses the server item and returns the item name (last segment).
        /// </summary>
        /// <param name="serverItem">The server item.</param>
        /// <returns>System.String.</returns>
        public static string ParseServerItem(string serverItem)
        {
            if (serverItem == @"$/")
                return serverItem;

            var relativePath = serverItem.LastIndexOf('/');
            return relativePath == -1
                ? string.Empty
                : serverItem.Substring(relativePath + 1);
        }

        /// <summary>
        ///     Parses the file item.
        /// </summary>
        /// <param name="fileItem">The file item.</param>
        /// <returns>System.String.</returns>
        public static string ParseFileItem(string fileItem)
        {
            var relativePath = fileItem.LastIndexOf('\\');
            return relativePath == -1
                ? string.Empty
                : fileItem.Substring(relativePath + 1);
        }

        /// <summary>
        ///     Splits a string on commas and returns the first result.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SplitAndReturnFirst(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var chunks = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return chunks.First();
        }

        /// <summary>
        ///     Splits the and wrap quotes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SplitAndWrapQuotes(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var chunks = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var list = chunks.Select(chunk => string.Format(CultureInfo.InvariantCulture, "'{0}'", chunk)).ToList();
            return string.Join(",", list);
        }

        /// <summary>
        ///     Parses the path and returns the file extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string ParseFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        ///     Returns the last occurrence.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>System.String.</returns>
        public static string ReturnLastOccurrence(this string source, char seperator)
        {
            var relativePath = source.LastIndexOf(seperator);
            return relativePath == -1
                ? string.Empty
                : source.Substring(relativePath + 1);
        }

        /// <summary>
        ///     Replaces the last occurrence of a matching value within a string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        /// <summary>
        ///     Strips HTML from the specified text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string StripHtml(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            //strip out all html tags
            var tag = new Regex("<[^>]*>",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
            var stripped = tag.Replace(value, string.Empty);

            //replace newlines, html entities etc.
            return stripped
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&");
        }

        /// <summary>
        ///     Pads a list of string items into a column display
        ///     adds a padding value of the specified size (default 2) to ensure all items are visible.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="padding">The padding size.</param>
        public static void PadList(IList<string[]> data, int padding = 2)
        {
            var maxWidth = new List<int>();
            FindMaxColumn(data, maxWidth);
            PadListData(data, padding, maxWidth);
        }

        private static void PadListData(IList<string[]> data, int padding, IReadOnlyList<int> maxWidth)
        {
            for (var i = 0; i < data.Count; i++)
            {
                var s = data[i];
                var newS = new string[s.Length];
                for (var j = 0; j < s.Length - 1; j++)
                {
                    var oldSt = s[j];
                    var len = maxWidth[j] + padding;
                    var newSt = "";
                    if (!string.IsNullOrEmpty(oldSt))
                    {
                        newSt = oldSt;
                    }
                    if (newSt.Length < len)
                    {
                        newSt = newSt + new string(' ', len - newSt.Length);
                    }
                    newS[j] = newSt;
                }
                //copy last data
                newS[s.Length - 1] = s[s.Length - 1];
                data[i] = newS;
            }
        }

        private static void FindMaxColumn(IList<string[]> data, IList<int> maxWidth)
        {
            var col = 0;
            bool foundColumn;
            do
            {
                foundColumn = false;
                foreach (var s in data)
                {
                    if (s.Length <= col)
                    {
                        continue;
                    }
                    if (maxWidth.Count <= col)
                    {
                        maxWidth.Add(0);
                    }
                    foundColumn = true;
                    var st = s[col];
                    if (!string.IsNullOrEmpty(st))
                        maxWidth[col] = Math.Max(maxWidth[col], st.Length);
                }
                col++;
            } while (foundColumn);
        }
    }
}