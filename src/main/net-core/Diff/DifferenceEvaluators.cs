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

using System.Linq;
using System.Xml;
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Evaluators used for the base cases.
    /// </summary>
    public static class DifferenceEvaluators {

        /// <summary>
        /// Difference evaluator that just echos the result passed in.
        /// </summary>
        public static ComparisonResult Accept(Comparison comparison,
                                              ComparisonResult outcome) {
            return outcome;
        }

        /// <summary>
        /// The "standard" difference evaluator which decides which
        /// differences make two XML documents really different and which
        /// still leave them similar.
        /// </summary>
        public static ComparisonResult Default(Comparison comparison,
                                               ComparisonResult outcome) {
            if (outcome == ComparisonResult.DIFFERENT) {
                switch (comparison.Type) {
                case ComparisonType.NODE_TYPE:
                    XmlNodeType control =
                        (XmlNodeType) comparison.ControlDetails.Value;
                    XmlNodeType test =
                        (XmlNodeType) comparison.TestDetails.Value;
                    if ((control == XmlNodeType.Text && test == XmlNodeType.CDATA)
                        ||
                        (control == XmlNodeType.CDATA && test == XmlNodeType.Text)
                        ) {
                        outcome = ComparisonResult.SIMILAR;
                    }
                    break;
                case ComparisonType.XML_ENCODING:
                case ComparisonType.HAS_DOCTYPE_DECLARATION:
                case ComparisonType.DOCTYPE_SYSTEM_ID:
                case ComparisonType.SCHEMA_LOCATION:
                case ComparisonType.NO_NAMESPACE_SCHEMA_LOCATION:
                case ComparisonType.NAMESPACE_PREFIX:
                case ComparisonType.ATTR_VALUE_EXPLICITLY_SPECIFIED:
                case ComparisonType.CHILD_NODELIST_SEQUENCE:
                    outcome = ComparisonResult.SIMILAR;
                    break;
                default:
                    break;
                }
            }
            return outcome;
        }

        /// <summary>
        /// Combines multiple DifferenceEvaluators so that the first
        /// one that changes the outcome wins.
        /// </summary>
        public static DifferenceEvaluator
            First(params DifferenceEvaluator[] evaluators)
        {
            return (comparison, orig) =>
                    evaluators.Select(ev => ev(comparison, orig))
                    .FirstOrDefaultValue(evaluated => evaluated != orig,
                                         orig);
        }

        /// <summary>
        /// Combines multiple DifferenceEvaluators so that the result
        /// of the first Evaluator will be passed to the next
        /// Evaluator.
        /// </summary>
        public static DifferenceEvaluator
            Chain(params DifferenceEvaluator[] evaluators) {
            return (comparison, orig) =>
                evaluators.Aggregate(orig, (r, ev) => ev(comparison, r));
        }

        /// <summary>
        /// Creates a DifferenceEvaluator that returns a EQUAL result for
        /// differences found in one of the given ComparisonTypes.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///  since XMLUnit 2.1.0
        ///   </para>
        /// </remarks>
        public static DifferenceEvaluator
            DowngradeDifferencesToEqual(params ComparisonType[] types) {
            return RecordDifferencesAs(ComparisonResult.EQUAL, types);
        }

        /// <summary>
        /// Creates a DifferenceEvaluator that returns a SIMILAR result for
        /// differences (Comparisons that are not EQUAL) found in one of
        /// the given ComparisonTypes.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// since XMLUnit 2.1.0
        ///   </para>
        /// </remarks>
        public static DifferenceEvaluator
            DowngradeDifferencesToSimilar(params ComparisonType[] types) {
            return RecordDifferencesAs(ComparisonResult.SIMILAR, types);
        }

        /// <summary>
        /// Creates a DifferenceEvaluator that returns a DIFFERENT result
        /// for differences (Comparisons that are not EQUAL) found in one
        /// of the given ComparisonTypes.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// since XMLUnit 2.1.0
        ///   </para>
        /// </remarks>
        public static DifferenceEvaluator
            UpgradeDifferencesToDifferent(params ComparisonType[] types) {
            return RecordDifferencesAs(ComparisonResult.DIFFERENT, types);
        }

        /// <summary>
        /// Ignore any differences that are part of the XML prolog.
        /// </summary>
        /// <remarks>
        /// <para>Here "ignore" means return {@code ComparisonResult.EQUAL}.</para>
        /// </remarks>
        /// <remarks>
        ///   <para>
        /// since XMLUnit 2.1.0
        ///   </para>
        /// </remarks>
        public static DifferenceEvaluator IgnorePrologDifferences() {
            return (comparison, orig) =>
                BelongsToProlog(comparison, true) || IsSequenceOfRootElement(comparison)
                ? ComparisonResult.EQUAL : orig;
        }

        /// <summary>
        /// Ignore any differences except differences inside the doctype
        /// declaration that are part of the XML prolog.
        /// </summary>
        /// <remarks>
        /// <p>Here "ignore" means return {@code ComparisonResult.EQUAL}.</p>
        /// </remarks>
        /// <remarks>
        ///   <para>
        /// since XMLUnit 2.1.0
        ///   </para>
        /// </remarks>
        public static DifferenceEvaluator IgnorePrologDifferencesExceptDoctype() {
            return (comparison, orig) =>
                BelongsToProlog(comparison, false) || IsSequenceOfRootElement(comparison)
                ? ComparisonResult.EQUAL : orig;
        }

        private static DifferenceEvaluator
            RecordDifferencesAs(ComparisonResult outcome, ComparisonType[] types) {
            return (comparison, orig) =>
                orig != ComparisonResult.EQUAL && types.Contains(comparison.Type)
                ? outcome : orig;
        }

        private static bool BelongsToProlog(Comparison comparison,
                                            bool ignoreDoctypeDeclarationAsWell) {
            if (comparison.Type.IsDoctypeComparison()) {
                return ignoreDoctypeDeclarationAsWell;
            }
            return BelongsToProlog(comparison.ControlDetails.Target,
                                   ignoreDoctypeDeclarationAsWell)
                || BelongsToProlog(comparison.TestDetails.Target,
                                   ignoreDoctypeDeclarationAsWell);
        }

        private static bool BelongsToProlog(XmlNode n,
                                            bool ignoreDoctypeDeclarationAsWell) {
            if (n == null || n is XmlElement) {
                return false;
            }
            if (!ignoreDoctypeDeclarationAsWell && n is XmlDocumentType) {
                return false;
            }
            if (n is XmlDocument) {
                return true;
            }
            return BelongsToProlog(n.ParentNode, ignoreDoctypeDeclarationAsWell);
        }

        private static bool IsSequenceOfRootElement(Comparison comparison) {
            return comparison.Type == ComparisonType.CHILD_NODELIST_SEQUENCE
                && comparison.ControlDetails.Target is XmlElement
                && comparison.ControlDetails.Target.ParentNode is XmlDocument;
        }

    }
}