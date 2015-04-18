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
                }
            }
            return outcome;
        }

        /// <summary>
        /// Combines multiple DifferenceEvaluators so that the first
        /// one that changes the outcome wins.
        /// </summary>
        public static DifferenceEvaluator
            First(params DifferenceEvaluator[] evaluators) {
            return (comparison, orig) =>
                Linqy
                    .FirstOrDefaultValue(evaluators
                                            .Select(ev => ev(comparison, orig)),
                                         evaluated => evaluated != orig,
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

    }
}