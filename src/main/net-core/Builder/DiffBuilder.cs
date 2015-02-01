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
using Org.XmlUnit.Diff;
using Org.XmlUnit.Input;

namespace Org.XmlUnit.Builder {


    /// <summary>
    ///   DiffBuilder to create a Diff instance.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Valid inputs for control and test are all objects supported by Input#From(object).
    ///   </para>
    /// </remarks>
    /* <b>Example Usage:</b>
     *
     * <pre>
     * String controlXml = &quot;&lt;a&gt;&lt;b&gt;Test Value&lt;/b&gt;&lt;/a&gt;&quot;;
     * String testXml = &quot;&lt;a&gt;\n &lt;b&gt;\n  Test Value\n &lt;/b&gt;\n&lt;/a&gt;&quot;;
     * Diff myDiff = DiffBuilder.compare(Input.fromMemory(controlXml)).withTest(Input.fromMemory(testXml))
     *     .checkForSimilar()
     *     .ignoreWhitespace()
     *     .build();
     * assertFalse(&quot;XML similar &quot; + myDiff.toString(), myDiff.hasDifferences());
     * </pre>
     */
    public class DiffBuilder {

        private static readonly ComparisonResult[] CHECK_FOR_SIMILAR = new ComparisonResult[] {
            ComparisonResult.DIFFERENT};

        private static readonly ComparisonResult[] CHECK_FOR_IDENTICAL = new ComparisonResult[] {
            ComparisonResult.SIMILAR, ComparisonResult.DIFFERENT};

        private readonly ISource controlSource;

        private ISource testSource;

        private INodeMatcher nodeMatcher;

        private ComparisonController comparisonController = ComparisonControllers.Default;

        private DifferenceEvaluator differenceEvaluator = DifferenceEvaluators.Default;

        private List<ComparisonListener> comparisonListeners = new List<ComparisonListener>();
        private List<ComparisonListener> differenceListeners = new List<ComparisonListener>();

        private ComparisonResult[] comparisonResultsToCheck = CHECK_FOR_IDENTICAL;

        private bool ignoreWhitespace;

        private bool normalizeWhitespace;

        private bool ignoreComments;

        /// <summary>
        ///   Create a DiffBuilder instance.
        /// </summary>
        /// <param name="param">controlSource the expected reference Result.</param>
        /// <param name="testSource">the test result which must be compared with the control source.</param>
        private DiffBuilder(ISource controlSource) {
            this.controlSource = controlSource;
        }

        /// <summary>
        ///   Create a DiffBuilder from all kind of types supported by Input#From(object).
        /// </summary>
        /// <param name="control">the expected reference document.</param>
        public static DiffBuilder Compare(object control) {
            ISource controlSource = GetSource(control);
            return new DiffBuilder(controlSource);
        }

        /// <summary>
        ///   Set the Test-Source from all kind of types supported by Input#From(object).
        /// </summary>
        /// <param name="test">the test document which must be compared with the control document.</param>
        public DiffBuilder WithTest(object test) {
            testSource = GetSource(test);
            return this;
        }

        private static ISource GetSource(object source) {
            return Input.From(source).Build();
        }

        /// <summary>
        ///   Ignore whitespace by removing all empty text nodes and trimming the non-empty ones.
        /// </summary>
        public DiffBuilder IgnoreWhitespace() {
            ignoreWhitespace = true;
            return this;
        }

        /// <summary>
        ///   Normalize Text-Elements by removing all empty text nodes and normalizing the non-empty ones.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     "normalized" in this context means all whitespace
        ///     characters are replaced by space characters and
        ///     consecutive whitespace characters are collapsed.
        ///   </para>
        ///   <para>
        ///     This flag has no effect if #IgnoreWhitespace() is already activated.
        ///   </para>
        /// </remarks>
        public DiffBuilder NormalizeWhitespace() {
            normalizeWhitespace = true;
            return this;
        }

        public DiffBuilder IgnoreComments() {
            ignoreComments = true;
            return this;
        }

        public DiffBuilder WithNodeMatcher(INodeMatcher nodeMatcher) {
            this.nodeMatcher = nodeMatcher;
            return this;
        }

        /// <summary>
        /// Provide your own custom {@link DifferenceEvaluator} implementation.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This overwrites the Default DifferenceEvaluator.
        ///   </para>
        ///   <para>
        ///     If you want use your custom DifferenceEvaluator in
        ///     combination with the default or another
        ///     DifferenceEvaluator you should use
        ///     DifferenceEvaluators#Chain() or
        ///     DifferenceEvaluators#First() to combine them:
        ///   </para>
        /// </remarks>
        /* <pre>
         * Diff myDiff = DiffBuilder.Compare(control).WithTest(test)
         *         .WithDifferenceEvaluator(
         *             DifferenceEvaluators.Chain(
         *                 DifferenceEvaluators.Default,
         *                 new MyCustomDifferenceEvaluator()))
         *         ....
         *         .Build();
         * </pre>
         */
        public DiffBuilder WithDifferenceEvaluator(DifferenceEvaluator differenceEvaluator) {
            this.differenceEvaluator = differenceEvaluator;
            return this;
        }

        /// <summary>
        ///   Replace the {@link ComparisonControllers#Default} with your own ComparisonController.
        /// </summary>
        /*
         *
         * <p>
         * Example use:
         * <pre>
         * Diff myDiff = DiffBuilder.Compare(control).WithTest(test)
         *      .WithComparisonController(ComparisonControllers.StopWhenDifferent)
         *      .Build();
         * </pre>
         */
        public DiffBuilder WithComparisonController(ComparisonController comparisonController) {
            this.comparisonController = comparisonController;
            return this;
        }

        public DiffBuilder WithComparisonListeners(params ComparisonListener[] comparisonListeners) {
            this.comparisonListeners.AddRange(comparisonListeners);
            return this;
        }

        public DiffBuilder WithDifferenceListeners(params ComparisonListener[] comparisonListeners) {
            this.differenceListeners.AddRange(comparisonListeners);
            return this;
        }

        /// <summary>
        ///   check test source with the control source for similarity.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Default is {@link #CheckForIdentical()}.
        ///   </para>
        /// </remarks>
        public DiffBuilder CheckForSimilar() {
            comparisonResultsToCheck = CHECK_FOR_SIMILAR;
            return this;
        }

        /// <summary>
        ///   check test source with the control source for identically.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is the Default.
        ///   </para>
        /// </remarks>
        public DiffBuilder CheckForIdentical() {
            comparisonResultsToCheck = CHECK_FOR_IDENTICAL;
            return this;
        }

        public Org.XmlUnit.Diff.Diff Build() {

            DOMDifferenceEngine d = new DOMDifferenceEngine();
            CollectResultsListener collectResultsListener = new CollectResultsListener(comparisonResultsToCheck);
            d.DifferenceListener += collectResultsListener.Listen;
            if (nodeMatcher != null) {
                d.NodeMatcher = nodeMatcher;
            }
            d.DifferenceEvaluator = differenceEvaluator;
            d.ComparisonController = comparisonController;
            foreach (ComparisonListener comparisonListener in differenceListeners) {
                d.DifferenceListener += comparisonListener;
            }
            foreach (ComparisonListener comparisonListener in comparisonListeners) {
                d.ComparisonListener += comparisonListener;
            }
            d.Compare(Wrap(controlSource), Wrap(testSource));

            return new Org.XmlUnit.Diff.Diff(controlSource, testSource,
                                             collectResultsListener.Differences);
        }

        private ISource Wrap(ISource source) {
            ISource newSource = source;
            if (ignoreWhitespace) {
                newSource = new WhitespaceStrippedSource(newSource);
            } else if (normalizeWhitespace) {
                newSource = new WhitespaceNormalizedSource(newSource);
            }
            if (ignoreComments) {
                newSource = new CommentLessSource(newSource);
            }
            return newSource;
        }

        internal class CollectResultsListener {

            private readonly IList<Difference> results;
            private readonly IEnumerable<ComparisonResult> comparisonResultsToCheck;

            internal CollectResultsListener(params ComparisonResult[] comparisonResultsToCheck) {
                results = new List<Difference>();
                this.comparisonResultsToCheck = comparisonResultsToCheck;
            }

            internal void Listen(Comparison comparison, ComparisonResult outcome) {
                if (comparisonResultsToCheck.Contains(outcome)) {
                    results.Add(new Difference(comparison, outcome));
                }
            }

            internal IEnumerable<Difference> Differences {
                get {
                    return results.ToArray();
                }
            }
        }
    }
}
