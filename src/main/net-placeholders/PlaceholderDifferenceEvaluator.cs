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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System;

using Org.XmlUnit;
using Org.XmlUnit.Diff;
using Org.XmlUnit.Util;

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
    /// since 2.6.0
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
        public static readonly string PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX = Regex.Escape("${");
        public static readonly string PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX = Regex.Escape("}");
        /// <remarks>
        ///   <para>
        /// since 2.8.0
        ///   </para>
        /// </remarks>
        public static readonly string PLACEHOLDER_DEFAULT_ARGS_OPENING_DELIMITER_REGEX = Regex.Escape("(");

        /// <remarks>
        ///   <para>
        /// since 2.8.0
        ///   </para>
        /// </remarks>
        public static readonly string PLACEHOLDER_DEFAULT_ARGS_CLOSING_DELIMITER_REGEX = Regex.Escape(")");

        /// <remarks>
        ///   <para>
        /// since 2.8.0
        ///   </para>
        /// </remarks>
        public static readonly string PLACEHOLDER_DEFAULT_ARGS_SEPARATOR_REGEX = Regex.Escape(",");

        private static readonly string PLACEHOLDER_PREFIX_REGEX = Regex.Escape("xmlunit.");
        // IReadOnlyDictionary is .NET Framework 4.5
        private static readonly IDictionary<string, IPlaceholderHandler> KNOWN_HANDLERS;
        private static readonly string[] NO_ARGS = new string[0];

        static PlaceholderDifferenceEvaluator() {
            var m = new Dictionary<string, IPlaceholderHandler>();
            foreach (var h in Load()) {
                m[h.Keyword] = h;
            }
            KNOWN_HANDLERS = m;
        }

        private static IEnumerable<IPlaceholderHandler> Load() {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => GetLoadableTypes(s))
                .Where(t => !t.IsAbstract)
                .Where(t => t.FindInterfaces((type, n) => type.FullName == (string) n,
                                typeof(IPlaceholderHandler).FullName).Length > 0)
                .Select(t => Activator.CreateInstance(t))
                .Cast<IPlaceholderHandler>();
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
            try {
                return assembly.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null);
            }
        }

        private readonly Regex placeholderRegex;
        private readonly Regex argsRegex;
        private readonly Regex argsSplitter;

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
                                              string placeholderClosingDelimiterRegex)
            : this(placeholderOpeningDelimiterRegex, placeholderClosingDelimiterRegex, null, null, null) {
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
        public PlaceholderDifferenceEvaluator(string placeholderOpeningDelimiterRegex,
                                              string placeholderClosingDelimiterRegex,
                                              string placeholderArgsOpeningDelimiterRegex,
                                              string placeholderArgsClosingDelimiterRegex,
                                              string placeholderArgsSeparatorRegex) {
            if (placeholderOpeningDelimiterRegex == null
                || placeholderOpeningDelimiterRegex.Trim().Length == 0) {
                placeholderOpeningDelimiterRegex = PLACEHOLDER_DEFAULT_OPENING_DELIMITER_REGEX;
            }
            if (placeholderClosingDelimiterRegex == null
                || placeholderClosingDelimiterRegex.Trim().Length == 0) {
                placeholderClosingDelimiterRegex = PLACEHOLDER_DEFAULT_CLOSING_DELIMITER_REGEX;
            }
            if (placeholderArgsOpeningDelimiterRegex == null
                || placeholderArgsOpeningDelimiterRegex.Trim().Length == 0) {
                placeholderArgsOpeningDelimiterRegex = PLACEHOLDER_DEFAULT_ARGS_OPENING_DELIMITER_REGEX;
            }
            if (placeholderArgsClosingDelimiterRegex == null
                || placeholderArgsClosingDelimiterRegex.Trim().Length == 0) {
                placeholderArgsClosingDelimiterRegex = PLACEHOLDER_DEFAULT_ARGS_CLOSING_DELIMITER_REGEX;
            }
            if (placeholderArgsSeparatorRegex == null
                || placeholderArgsSeparatorRegex.Trim().Length == 0) {
                placeholderArgsSeparatorRegex = PLACEHOLDER_DEFAULT_ARGS_SEPARATOR_REGEX;
            }

            placeholderRegex = new Regex("(\\s*" + placeholderOpeningDelimiterRegex
                + "\\s*" + PLACEHOLDER_PREFIX_REGEX + "(.+)" + "\\s*"
                + placeholderClosingDelimiterRegex + "\\s*)");
            argsRegex = new Regex("((.*)\\s*" + placeholderArgsOpeningDelimiterRegex
                + "(.+)"
                + "\\s*" + placeholderArgsClosingDelimiterRegex + "\\s*)");
            argsSplitter = new Regex(placeholderArgsSeparatorRegex);
        }

        /// <summary>
        ///   DifferenceEvaluator using the configured PlaceholderHandlers.
        /// </summary>
        public ComparisonResult Evaluate(Comparison comparison, ComparisonResult outcome) {
            if (outcome == ComparisonResult.EQUAL) {
                return outcome;
            }

            Comparison.Detail controlDetails = comparison.ControlDetails;
            XmlNode controlTarget = controlDetails.Target;
            Comparison.Detail testDetails = comparison.TestDetails;
            XmlNode testTarget = testDetails.Target;

            // comparing textual content of elements
            if (comparison.Type == ComparisonType.TEXT_VALUE) {
                return EvaluateConsideringPlaceholders((string) controlDetails.Value,
                    (string) testDetails.Value, outcome);

            // "test document has no text-like child node but control document has"
            } else if (IsMissingTextNodeDifference(comparison)) {
                return EvaluateMissingTextNodeConsideringPlaceholders(comparison, outcome);

            // may be comparing TEXT to CDATA
            } else if (IsTextCDATAMismatch(comparison)) {
                return EvaluateConsideringPlaceholders(controlTarget.Value, testTarget.Value, outcome);

            // comparing textual content of attributes
            }
            else if (comparison.Type == ComparisonType.ATTR_VALUE && controlDetails.Value is string &&
                     testDetails.Value is string)
            {
                return EvaluateConsideringPlaceholders((string) controlDetails.Value,
                    (string) testDetails.Value, outcome);

            // "test document has no attribute but control document has"
            } else if (IsMissingAttributeDifference(comparison)) {
                return EvaluateMissingAttributeConsideringPlaceholders(comparison, outcome);
            }

            // default, don't apply any placeholders at all
            return outcome;
        }

        private bool IsMissingTextNodeDifference(Comparison comparison) {
            return ControlHasOneTextChildAndTestHasNone(comparison)
                || CantFindControlTextChildInTest(comparison);
        }

        private bool ControlHasOneTextChildAndTestHasNone(Comparison comparison) {
            Comparison.Detail controlDetails = comparison.ControlDetails;
            XmlNode controlTarget = controlDetails.Target;
            Comparison.Detail testDetails = comparison.TestDetails;
            return comparison.Type == ComparisonType.CHILD_NODELIST_LENGTH &&
                1 == (int) controlDetails.Value &&
                0 == (int) testDetails.Value &&
                IsTextLikeNode(controlTarget.FirstChild);
        }

        private bool CantFindControlTextChildInTest(Comparison comparison) {
            XmlNode controlTarget = comparison.ControlDetails.Target;
            return comparison.Type == ComparisonType.CHILD_LOOKUP
                && controlTarget != null && IsTextLikeNode(controlTarget);
    }

        private ComparisonResult EvaluateMissingTextNodeConsideringPlaceholders(Comparison comparison, ComparisonResult outcome) {
            XmlNode controlTarget = comparison.ControlDetails.Target;
            string value;
            if (ControlHasOneTextChildAndTestHasNone(comparison)) {
                value = controlTarget.FirstChild.Value;
            } else {
                value = controlTarget.Value;
            }
            return EvaluateConsideringPlaceholders(value, null, outcome);
        }

        private bool IsTextCDATAMismatch(Comparison comparison) {
            return comparison.Type == ComparisonType.NODE_TYPE
                && IsTextLikeNode(comparison.ControlDetails.Target)
                && IsTextLikeNode(comparison.TestDetails.Target);
        }

        private bool IsTextLikeNode(XmlNode node) {
            var nodeType = node.NodeType;
            return nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA;
        }

        private bool IsMissingAttributeDifference(Comparison comparison) {
            return comparison.Type == ComparisonType.ELEMENT_NUM_ATTRIBUTES
                || (comparison.Type == ComparisonType.ATTR_NAME_LOOKUP
                    && comparison.ControlDetails.Target != null
                    && comparison.ControlDetails.Value != null);
        }

        private ComparisonResult EvaluateMissingAttributeConsideringPlaceholders(Comparison comparison,
            ComparisonResult outcome) {
            if (comparison.Type == ComparisonType.ELEMENT_NUM_ATTRIBUTES) {
                return EvaluateAttributeListLengthConsideringPlaceholders(comparison, outcome);
            }
            string controlAttrValue = Nodes.GetAttributes(comparison.ControlDetails.Target)
                [(XmlQualifiedName) comparison.ControlDetails.Value];
            return EvaluateConsideringPlaceholders(controlAttrValue, null, outcome);
        }

        private ComparisonResult EvaluateAttributeListLengthConsideringPlaceholders(Comparison comparison,
            ComparisonResult outcome) {
            var controlAttrs = Nodes.GetAttributes(comparison.ControlDetails.Target);
            var testAttrs = Nodes.GetAttributes(comparison.TestDetails.Target);

            int cAttrsMatched = 0;
            foreach (var cAttr in controlAttrs) {
                string testValue;
                if (!testAttrs.TryGetValue(cAttr.Key, out testValue)) {
                    ComparisonResult o = EvaluateConsideringPlaceholders(cAttr.Value, null, outcome);
                    if (o != ComparisonResult.EQUAL) {
                        return outcome;
                    }
                } else {
                    cAttrsMatched++;
                }
            }
            if (cAttrsMatched != testAttrs.Count) {
                // there are unmatched test attributes
                return outcome;
            }
            return ComparisonResult.EQUAL;
        }

        private ComparisonResult EvaluateConsideringPlaceholders(string controlText, string testText,
            ComparisonResult outcome) {
            Match placeholderMatch = placeholderRegex.Match(controlText);
            if (placeholderMatch.Success) {
                string content = placeholderMatch.Groups[2].Captures[0].Value.Trim();
                Match argsMatch = argsRegex.Match(content);
                string keyword;
                string[] args;
                if (argsMatch.Success) {
                    keyword = argsMatch.Groups[2].Captures[0].Value.Trim();
                    args = argsSplitter.Split(argsMatch.Groups[3].Captures[0].Value);
                } else {
                    keyword = content;
                    args = NO_ARGS;
                }
                if (IsKnown(keyword)) {
                    if (placeholderMatch.Groups[1].Captures[0].Value.Trim() != controlText.Trim()) {
                        throw new XMLUnitException("The placeholder must exclusively occupy the text node.");
                    }
                    return Evaluate(keyword, testText, args);
                }
            }

            // no placeholder at all or unknown keyword
            return outcome;
        }

        private bool IsKnown(string keyword) {
            return KNOWN_HANDLERS.ContainsKey(keyword);
        }

        private ComparisonResult Evaluate(string keyword, string testText, string[] args) {
            return KNOWN_HANDLERS[keyword].Evaluate(testText, args);
        }
    }
}
