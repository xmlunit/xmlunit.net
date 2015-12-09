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

using System;
using System.Linq;
using System.Xml;
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

        /// <summary>
        /// Ignore whitespace differences.
        /// </summary>
        public CompareConstraint IgnoreWhitespace() {
            formatXml = true;
            diffBuilder.IgnoreWhitespace();
            return this;
        }

        /// <summary>
        /// Normalize whitespace before comparing.
        /// </summary>
        public CompareConstraint NormalizeWhitespace() {
            formatXml = true;
            diffBuilder.NormalizeWhitespace();
            return this;
        }

        /// <summary>
        /// Ignore comments.
        /// </summary>
        public CompareConstraint IgnoreComments()
        {
            diffBuilder.IgnoreComments();
            return this;
        }

        /// <summary>
        /// Use the given <see cref="INodeMatcher"/> when comparing.
        /// </summary>
        /// <param name="nodeMatcher">INodeMatcher to use</param>
        public CompareConstraint WithNodeMatcher(INodeMatcher nodeMatcher) {
            diffBuilder.WithNodeMatcher(nodeMatcher);
            return this;
        }

        /// <summary>
        /// Use the given <see cref="DifferenceEvaluator"/> when comparing.
        /// </summary>
        /// <param name="differenceEvaluator">DifferenceEvaluator to use</param>
        public CompareConstraint WithDifferenceEvaluator(DifferenceEvaluator differenceEvaluator)
        {
            diffBuilder.WithDifferenceEvaluator(differenceEvaluator);
            return this;
        }

        /// <summary>
        /// Use the given <see cref="ComparisonListener"/>s when comparing.
        /// </summary>
        /// <param name="comparisonListeners">ComparisonListeners to use</param>
        public CompareConstraint WithComparisonListeners(params ComparisonListener[] comparisonListeners)
        {
            diffBuilder.WithComparisonListeners(comparisonListeners);
            return this;
        }

        /// <summary>
        /// Use the given <see cref="ComparisonListener"/>s as difference listeners when comparing.
        /// </summary>
        /// <param name="comparisonListeners">ComparisonListeners to use</param>
        public CompareConstraint WithDifferenceListeners(params ComparisonListener[] comparisonListeners)
        {
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

        /// <summary>
        ///   Registers a filter for attributes.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// Only attributes for which the predicate returns true are
        /// part of the comparison.  By default all attributes are
        /// considered.
        ///   </para>
        ///   <para>
        /// The "special" namespace, namespace-location and
        /// schema-instance-type attributes can not be ignored this way.
        /// If you want to suppress comparison of them you'll need to
        /// implement <see cref="DifferenceEvaluator"/>
        ///   </para>
        /// </remarks>
        public CompareConstraint WithAttributeFilter(Predicate<XmlAttribute> attributeFilter) {
            diffBuilder.WithAttributeFilter(attributeFilter);
            return this;
        }

        /// <summary>
        ///   Registers a filter for nodes.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// Only nodes for which the predicate returns true are part
        /// of the comparison.  By default nodes that are neither
        /// document types nor XML declarations are considered.
        ///   </para>
        /// </remarks>
        public CompareConstraint WithNodeFilter(Predicate<XmlNode> nodeFilter) {
            diffBuilder.WithNodeFilter(nodeFilter);
            return this;
        }

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo<TActual>(TActual actual) {
            if (checkFor == ComparisonResult.EQUAL) {
                diffBuilder.WithComparisonController(ComparisonControllers.StopWhenSimilar);
            } else if (checkFor == ComparisonResult.SIMILAR) {
                diffBuilder.WithComparisonController(ComparisonControllers.StopWhenDifferent);
            }

            Diff.Diff diffResult = diffBuilder.WithTest(actual).Build();
            return new CompareConstraintResult(this, actual, diffResult);
        }

        public class CompareConstraintResult : ConstraintResult {
            private readonly CompareConstraint constraint;
            private readonly Diff.Diff diffResult;

            public CompareConstraintResult(CompareConstraint constraint, object actualValue, Diff.Diff diffResult)
                : base(constraint, actualValue, !diffResult.HasDifferences()) {
                this.constraint = constraint;
                this.diffResult = diffResult;
            }

            /// <inheritdoc/>
            public override void WriteMessageTo(MessageWriter writer) {
                Comparison c = diffResult.Differences.First().Comparison;
                writer.WriteMessageLine(constraint.comparisonFormatter.GetDescription(c));
                if (diffResult.TestSource.SystemId != null
                    || diffResult.ControlSource.SystemId != null)
                {
                    writer.WriteMessageLine(string.Format("comparing {0} to {1}",
                                                          diffResult.TestSource.SystemId,
                                                          diffResult.ControlSource.SystemId));
                }
                writer.DisplayDifferences(GetDetails(c.ControlDetails, c.Type),
                                          GetDetails(c.TestDetails, c.Type));
            }

            private string GetDetails(Comparison.Detail detail, ComparisonType type) {
                return constraint.comparisonFormatter.GetDetails(detail, type, constraint.formatXml);
            }
        }
    }
}