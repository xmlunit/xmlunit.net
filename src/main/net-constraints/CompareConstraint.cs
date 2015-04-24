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
using NUnit.Framework.Constraints;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Constraints {

    /// <summary>
    /// Constraint that compares two XML sources with each other.
    /// </summary>
    public class CompareConstraint : Constraint {

        private readonly DiffBuilder diffBuilder;
        private ComparisonResult checkFor;
        private Org.XmlUnit.Diff.Diff diffResult;
        private bool formatXml;
        private IComparisonFormatter comparisonFormatter = new DefaultComparisonFormatter();

        private CompareConstraint(object control) {
            diffBuilder = DiffBuilder.Compare(control);
        }

        /// <summary>
        ///   Create a CompareConstraint which compares the
        ///   test-Object with the given control Object for identity.
        /// </summary>
        public static CompareConstraint IsIdenticalTo(object control) {
            return new CompareConstraint(control).CheckForIdentical();
        }

        /// <summary>
        ///   Create a CompareConstraint which compares the
        ///   test-Object with the given control Object for similarity.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Example for Similar: The XML node
        ///     "&lt;a&gt;Text&lt;/a&gt;" and
        ///     "&lt;a&gt;&lt;![CDATA[Text]]&gt;&lt;/a&gt;" are
        ///     similar and the Test will not fail.
        ///   </para>
        /// </remarks>
        public static CompareConstraint IsSimilarTo(object control) {
            return new CompareConstraint(control).CheckForSimilar();
        }

        private CompareConstraint CheckForSimilar() {
            diffBuilder.CheckForSimilar();
            checkFor = ComparisonResult.SIMILAR;
            return this;
        }

        private CompareConstraint CheckForIdentical() {
            diffBuilder.CheckForIdentical();
            checkFor = ComparisonResult.EQUAL;
            return this;
        }

        public CompareConstraint IgnoreWhitespace() {
            formatXml = true;
            diffBuilder.IgnoreWhitespace();
            return this;
        }

        public CompareConstraint NormalizeWhitespace() {
            formatXml = true;
            diffBuilder.NormalizeWhitespace();
            return this;
        }

        public CompareConstraint IgnoreComments() {
            diffBuilder.IgnoreComments();
            return this;
        }

        public CompareConstraint WithNodeMatcher(INodeMatcher nodeMatcher) {
            diffBuilder.WithNodeMatcher(nodeMatcher);
            return this;
        }

        public CompareConstraint WithDifferenceEvaluator(DifferenceEvaluator differenceEvaluator) {
            diffBuilder.WithDifferenceEvaluator(differenceEvaluator);
            return this;
        }

        public CompareConstraint WithComparisonListeners(params ComparisonListener[] comparisonListeners) {
            diffBuilder.WithComparisonListeners(comparisonListeners);
            return this;
        }

        public CompareConstraint WithDifferenceListeners(params ComparisonListener[] comparisonListeners) {
            diffBuilder.WithDifferenceListeners(comparisonListeners);
            return this;
        }

        /// <summary>
        ///   Use a custom Formatter for the Error Messages. The defaultFormatter is DefaultComparisonFormatter.
        /// </summary>
        public CompareConstraint WithComparisonFormatter(IComparisonFormatter comparisonFormatter) {
            this.comparisonFormatter = comparisonFormatter;
            return this;
        }

        public override bool Matches(object o) {
            if (checkFor == ComparisonResult.EQUAL) {
                diffBuilder.WithComparisonController(ComparisonControllers.StopWhenSimilar);
            } else if (checkFor == ComparisonResult.SIMILAR) {
                diffBuilder.WithComparisonController(ComparisonControllers.StopWhenDifferent);
            }

            diffResult = diffBuilder.WithTest(o).Build();
            return !diffResult.HasDifferences();
        }

        public override void WriteDescriptionTo(MessageWriter writer) {
            writer.Write("{0} is {1} to {2}", diffResult.TestSource.SystemId,
                         checkFor == ComparisonResult.EQUAL ? "identical" : "similar",
                         diffResult.ControlSource.SystemId);
        }

        public override void WriteMessageTo(MessageWriter writer) {
            Comparison c = diffResult.Differences.First().Comparison;
            writer.WriteMessageLine(comparisonFormatter.GetDescription(c));
            if (diffResult.TestSource.SystemId != null
                || diffResult.ControlSource.SystemId != null) {
                writer.WriteMessageLine(string.Format("comparing {0} to {1}",
                                                      diffResult.TestSource.SystemId,
                                                      diffResult.ControlSource.SystemId));
            }
            writer.DisplayDifferences(GetDetails(c.ControlDetails, c.Type),
                                      GetDetails(c.TestDetails, c.Type));
        }

        private string GetDetails(Comparison.Detail detail, ComparisonType type) {
            return comparisonFormatter.GetDetails(detail, type, formatXml);
        }
    }
}