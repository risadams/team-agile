#region References

using System;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace CAS.Common.Comparers
{
    /// <summary>
    ///     String comparer which uses an ordinal ordering which takes numerics into account to items sort correctly
    /// </summary>
    public sealed class OrdinalStringComparer : IComparer<string>
    {
        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare.</param>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as
        ///     shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />
        ///     .Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        public int Compare(string x, string y)
        {
            var left = new AlphaNumericSortedString(x);
            var right = new AlphaNumericSortedString(y);

            return left.CompareTo(right);
        }

        private sealed class AlphaNumericSortedString : IComparable<AlphaNumericSortedString>
        {
            private readonly List<long> _numbers;
            private readonly List<string> _strings;

            /// <summary>
            ///     Initializes a new instance of the <see cref="AlphaNumericSortedString" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public AlphaNumericSortedString(string value)
            {
                _strings = new List<string>();
                _numbers = new List<long>();
                var pos = 0;
                var number = false;
                while (pos < value.Length)
                {
                    var len = 0;
                    while (pos + len < value.Length && Char.IsDigit(value[pos + len]) == number)
                    {
                        len++;
                    }
                    if (number)
                    {
                        _numbers.Add(long.Parse(value.Substring(pos, len), CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        _strings.Add(value.Substring(pos, len));
                    }
                    pos += len;
                    number = !number;
                }
            }

            /// <summary>
            ///     Compares the current object with another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            ///     A value that indicates the relative order of the objects being compared. The return value has the following
            ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
            ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
            ///     <paramref name="other" />.
            /// </returns>
            public int CompareTo(AlphaNumericSortedString other)
            {
                var index = 0;
                while (index < _strings.Count && index < other._strings.Count)
                {
                    var result = String.Compare(_strings[index], other._strings[index], StringComparison.Ordinal);
                    if (result != 0)
                        return result;
                    if (index < _numbers.Count && index < other._numbers.Count)
                    {
                        result = _numbers[index].CompareTo(other._numbers[index]);
                        if (result != 0)
                            return result;
                    }
                    index++;
                }
                return 0;
            }
        }
    }
}