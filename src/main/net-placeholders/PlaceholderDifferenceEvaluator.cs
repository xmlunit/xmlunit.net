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
using System.Collections.Generic;
using System.Xml;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {
    /// <summary>
    /// This class is used to add placeholder feature to XML comparison.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// This class and the whole module are considered experimental
    /// and any API may change between releases of XMLUnit.
    ///   </para>
    ///   <para>
    /// Supported scenarios are demonstrated in the unit tests
    /// (PlaceholderDifferenceEvaluatorTest).
    ///   </para>
    ///   <para>
    /// Default delimiters for placeholder are ${ and }. To use custom
    /// delimiters (in regular expression), create instance with the
    /// PlaceholderDifferenceEvaluator(string
    /// placeholderOpeningDelimiterRegex, string
    /// placeholderClosingDelimiterRegex) constructor.
    ///   </para>
    ///   <para>
    /// since 2.5.1
    ///   </para>
    /// </remarks>
     /*
      * <p>To use it, just add it with {@link
      * org.xmlunit.builder.DiffBuilder} like below</p>
      *
      * <pre>
      * Diff diff = DiffBuilder.compare(control).withTest(test).withDifferenceEvaluator(new PlaceholderDifferenceEvaluator()).build();
      * </pre>
      */
    public class PlaceholderDifferenceEvaluator {

        /// <summary>
        /// Creates a PlaceholderDifferenceEvaluator with default
        /// delimiters PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX and
        /// PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX.
        /// </summary>
        public PlaceholderDifferenceEvaluator() : this(null, null) {
        }

        /// <summary>
        /// Creates a PlaceholderDifferenceEvaluator with default
        /// delimiters PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX and
        /// PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX.
        /// </summary>
        /// <param name="placeholderOpeningDelimiterRegex">regular
        /// expression for the opening delimiter of placeholder,
        /// defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        /// <param name="placeholderClosingDelimiterRegex">regular
        /// expression for the closing delimiter of placeholder,
        /// defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        public PlaceholderDifferenceEvaluator(string placeholderOpeningDelimiterRegex,
                                              string placeholderClosingDelimiterRegex) {
            /*
        if (placeholderOpeningDelimiterRegex == null
            || placeholderOpeningDelimiterRegex.trim().length() == 0) {
            placeholderOpeningDelimiterRegex = PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX;
        }
        if (placeholderClosingDelimiterRegex == null
            || placeholderClosingDelimiterRegex.trim().length() == 0) {
            placeholderClosingDelimiterRegex = PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX;
        }

        placeholderRegex = Pattern.compile("(\\s*" + placeholderOpeningDelimiterRegex
            + "\\s*" + PLACEHOLDER_PREFIX_REGEX + "(.+)" + "\\s*"
            + placeholderClosingDelimiterRegex + "\\s*)");
            */
        }

        /// <summary>
        ///   DifferenceEvaluator using the configured PlaceholderHandlers.
        /// </summary>
        public ComparisonResult Evaluate(Comparison comparison, ComparisonResult outcome) {
            return ComparisonResult.EQUAL;
        }
    }
}
