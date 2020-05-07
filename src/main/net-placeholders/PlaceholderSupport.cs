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
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {
    /// <summary>
    /// Adds support for the placeholder feature to a
    /// DifferenceEngineConfigurer - like DiffBuilder or
    /// CompareConstraint.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// This class and the whole module are considered experimental
    /// and any API may change between releases of XMLUnit.
    ///   </para>
    ///   <para>
    /// since 2.6.0
    ///   </para>
    /// </remarks>
    public static class PlaceholderSupport {
        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupport<D>(this D configurer)
            where D : IDifferenceEngineConfigurer<D> {
            return WithPlaceholderSupportUsingDelimiters(configurer, null, null);
        }

        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
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
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupportUsingDelimiters<D>(this D configurer, string placeholderOpeningDelimiterRegex,
            string placeholderClosingDelimiterRegex)
            where D : IDifferenceEngineConfigurer<D> {
            return WithPlaceholderSupportUsingDelimiters(configurer, placeholderOpeningDelimiterRegex,
                placeholderClosingDelimiterRegex, null, null, null);
        }

        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
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
        /// <param name="placeholderArgsOpeningDelimiterRegex">regular
        /// expression for the opening delimiter of the placeholder's
        /// argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_OPENING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        /// <param name="placeholderArgsClosingDelimiterRegex">regular
        /// expression for the closing delimiter of the placeholder's
        /// argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_CLOSING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        /// <param name="placeholderArgsSeparatorRegex">regular
        /// expression for the delimiter between arguments inside of
        /// the placeholder's argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_SEPARATOR_REGEX
        /// if the parameter is null or blank</param>
        /// <remarks>
        ///   <para>
        /// since 2.8.0
        ///   </para>
        /// </remarks>
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupportUsingDelimiters<D>(this D configurer, string placeholderOpeningDelimiterRegex,
            string placeholderClosingDelimiterRegex,
            string placeholderArgsOpeningDelimiterRegex,
            string placeholderArgsClosingDelimiterRegex,
            string placeholderArgsSeparatorRegex)
            where D : IDifferenceEngineConfigurer<D> {
            return configurer.WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator(placeholderOpeningDelimiterRegex,
                placeholderClosingDelimiterRegex, placeholderArgsOpeningDelimiterRegex,
                placeholderArgsClosingDelimiterRegex, placeholderArgsSeparatorRegex).Evaluate);
        }

        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer
        /// considering an additional DifferenceEvaluator.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
        /// <param name="evaluator">the additional evaluator -
        /// placeholder support is chained after the given
        /// evaluator.</param>
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupportChainedAfter<D>(this D configurer, DifferenceEvaluator evaluator)
            where D : IDifferenceEngineConfigurer<D> {
            return WithPlaceholderSupportUsingDelimitersChainedAfter(configurer, null, null, evaluator);
        }

        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer
        /// considering an additional DifferenceEvaluator.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
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
        /// <param name="evaluator">the additional evaluator -
        /// placeholder support is chained after the given
        /// evaluator.</param>
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupportUsingDelimitersChainedAfter<D>(this D configurer,
            string placeholderOpeningDelimiterRegex, string placeholderClosingDelimiterRegex,
            DifferenceEvaluator evaluator)
            where D : IDifferenceEngineConfigurer<D> {
            return WithPlaceholderSupportUsingDelimitersChainedAfter(configurer, placeholderOpeningDelimiterRegex,
                placeholderClosingDelimiterRegex, null, null, null, evaluator);
        }

        /// <summary>
        /// Adds placeholder support to an IDifferenceEngineConfigurer
        /// considering an additional DifferenceEvaluator.
        /// </summary>
        /// <param name="configurer">configurer the configurer to add support to</param>
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
        /// <param name="evaluator">the additional evaluator -
        /// placeholder support is chained after the given
        /// evaluator.</param>
        /// <param name="placeholderArgsOpeningDelimiterRegex">regular
        /// expression for the opening delimiter of the placeholder's
        /// argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_OPENING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        /// <param name="placeholderArgsClosingDelimiterRegex">regular
        /// expression for the closing delimiter of the placeholder's
        /// argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_CLOSING_DELIMITER_REGEX
        /// if the parameter is null or blank</param>
        /// <param name="placeholderArgsSeparatorRegex">regular
        /// expression for the delimiter between arguments inside of
        /// the placeholder's argument list, defaults to
        /// PlaceholderDifferenceEvaluator#PLACEHOLDER_DEFAULT_ARGS_SEPARATOR_REGEX
        /// if the parameter is null or blank</param>
        /// <remarks>
        ///   <para>
        /// since 2.8.0
        ///   </para>
        /// </remarks>
        /// <return>the configurer with placeholder support added in.</return>
        public static D WithPlaceholderSupportUsingDelimitersChainedAfter<D>(this D configurer,
            string placeholderOpeningDelimiterRegex, string placeholderClosingDelimiterRegex,
            string placeholderArgsOpeningDelimiterRegex,
            string placeholderArgsClosingDelimiterRegex,
            string placeholderArgsSeparatorRegex,
            DifferenceEvaluator evaluator)
            where D : IDifferenceEngineConfigurer<D> {
            return configurer.WithDifferenceEvaluator(DifferenceEvaluators.Chain(
                evaluator, new PlaceholderDifferenceEvaluator(placeholderOpeningDelimiterRegex,
                placeholderClosingDelimiterRegex, placeholderArgsOpeningDelimiterRegex,
                placeholderArgsClosingDelimiterRegex, placeholderArgsSeparatorRegex).Evaluate));
        }
    }
}
