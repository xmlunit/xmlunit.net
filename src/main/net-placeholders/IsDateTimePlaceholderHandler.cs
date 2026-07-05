/*
  This file is licensed to You under the Apache License, Version 2.0
  (the "License"); you may not use this file except in compliance with
  the License.  You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using Org.XmlUnit.Diff;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.XmlUnit.Placeholder
{
    /// <summary>
    /// Handler for the "isDateTime" handler placeholder keyword
    /// </summary>
    /// <remarks>
    ///   <para>
    /// since 2.8.0
    ///   </para>
    ///   <para>
    /// The first optional argument is the date/time pattern, the second optional
    /// argument is the culture used to parse it, given as a culture name (for example
    /// <c>de</c> or <c>fr-FR</c>). When no culture is given <see cref="CultureInfo.InvariantCulture"/>
    /// is used.
    /// </para>
    /// </remarks>
    public class IsDateTimePlaceholderHandler : IPlaceholderHandler
    {
        private const string _keyword = "isDateTime";

        private static readonly IEnumerable<string> _isoPatterns = new List<string>
        {
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-ddTHH:mm:ss.fffK",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ssK",
            "yyyy-MM-dd HH:mm:ss.fffK"
        };

        /// <inheritdoc/>
        public string Keyword { get { return _keyword; } }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// When <paramref name="args"/> contains one element, it is used as the date/time pattern
        /// and <see cref="CultureInfo.InvariantCulture"/> is used for parsing.
        /// </para>
        /// <para>
        /// When <paramref name="args"/> contains two elements, the first is used as the date/time
        /// pattern and the second as the culture name for parsing.
        /// </para>
        /// <para>
        /// When no arguments are provided, the method tries to parse the text using ISO patterns
        /// with <see cref="CultureInfo.InvariantCulture"/>.
        /// </para>
        /// </remarks>
        public ComparisonResult Evaluate(string testText, params string[] args)
        {
            if (args != null && args.Length >= 1) {
                var culture = args.Length >= 2 && !string.IsNullOrEmpty(args[1])
                    ? new CultureInfo(args[1])
                    : CultureInfo.InvariantCulture;
                return CanParse(args[0], testText, culture)
                    ? ComparisonResult.EQUAL
                    : ComparisonResult.DIFFERENT;
            }
            return CanParse(testText)
                ? ComparisonResult.EQUAL
                : ComparisonResult.DIFFERENT;
        }

        private bool CanParse(string testText) {
            if (string.IsNullOrEmpty(testText)) {
                return false;
            }
            // Try locale-aware short date and datetime formats with current culture
            if (CanParseWithCurrentCulture(testText)) {
                return true;
            }
            // Try all ISO patterns with InvariantCulture
            foreach (var pattern in _isoPatterns) {
                if (CanParse(pattern, testText, CultureInfo.InvariantCulture)) {
                    return true;
                }
            }
            return false;
        }

        private bool CanParseWithCurrentCulture(string testText) {
            DateTime result;
            // Try short date format
            if (DateTime.TryParse(testText, out result)) {
                return true;
            }
            return false;
        }

        private bool CanParse(string pattern, string testText, CultureInfo culture) {
            try {
                var _ = DateTime.ParseExact(testText, pattern, culture);
                return true;
            } catch (FormatException) {
                return false;
            }
        }
    }
}
