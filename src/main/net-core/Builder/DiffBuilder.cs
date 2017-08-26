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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
     * Diff myDiff = DiffBuilder.compare(Input.FromString(controlXml)).WithTest(Input.fromString(testXml))
     *     .CheckForSimilar()
     *     .IgnoreWhitespace()
     *     .Build();
     * Assert.IsFalse(&quot;XML similar &quot; + myDiff, myDiff.HasDifferences());
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

        private Predicate<XmlAttribute> attributeFilter;

        private Predicate<XmlNode> nodeFilter;

        private IComparisonFormatter formatter;

        private ComparisonResult[] comparisonResultsToCheck = CHECK_FOR_IDENTICAL;

        private IDictionary<string, string> namespaceContext;

        private bool ignoreWhitespace;

        private bool normalizeWhitespace;

        private bool ignoreComments;

        private string ignoreCommentVersion = null;

        /// <summary>
        ///   Create a DiffBuilder instance.
        /// </summary>
        /// <param name="controlSource">controlSource the expected reference Result.</param>
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
        /// </remarks>
        public DiffBuilder NormalizeWhitespace() {
            normalizeWhitespace = true;
            return this;
        }

        /// <summary>
        /// Will remove all comment-Tags "&lt;!-- Comment --&gt;" from
        /// test- and control-XML before comparing.
        /// </summary>
        public DiffBuilder IgnoreComments() {
            return IgnoreCommentsUsingXSLTVersion(null);
        }

        /// <summary>
        /// Will remove all comment-Tags "&lt;!-- Comment --&gt;" from
        /// test- and control-XML before comparing.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     since XMLUnit 2.5.0
        ///   </para>
        /// </remarks>
        /// <param name="xsltVersion">use this version for the stylesheet</param>
        public DiffBuilder IgnoreCommentsUsingXSLTVersion(string xsltVersion) {
            ignoreComments = true;
            ignoreCommentVersion = xsltVersion;
            return this;
        }

        /// <summary>
        /// Sets the strategy for selecting nodes to compare.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// Example with org.xmlunit.diff.DefaultNodeMatcher:
        /// <pre>
        /// .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndText))
        /// </pre>
        ///   </para>
        /// </remarks>
        /* @see org.xmlunit.diff.DifferenceEngine#setNodeMatcher(NodeMatcher) */
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

        /// <summary>
        ///   Registers a listener that is notified of each comparison.
        /// </summary>
        /* @see org.xmlunit.diff.DifferenceEngine#addComparisonListener(ComparisonListener) */
        public DiffBuilder WithComparisonListeners(params ComparisonListener[] comparisonListeners) {
            this.comparisonListeners.AddRange(comparisonListeners);
            return this;
        }

        /// <summary>
        ///   Registers a listener that is notified of each comparison with
        ///   outcome other than ComparisonResult#EQUAL.
        /// </summary>
        /* @see org.xmlunit.diff.DifferenceEngine#addDifferenceListener(ComparisonListener) */
        public DiffBuilder WithDifferenceListeners(params ComparisonListener[] comparisonListeners) {
            this.differenceListeners.AddRange(comparisonListeners);
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
        public DiffBuilder WithAttributeFilter(Predicate<XmlAttribute> attributeFilter) {
            this.attributeFilter = attributeFilter;
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
        public DiffBuilder WithNodeFilter(Predicate<XmlNode> nodeFilter) {
            this.nodeFilter = nodeFilter;
            return this;
        }

        /// <summary>
        ///   check test source with the control source for similarity.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Example for Similar: The XML node
        ///     "&lt;a&gt;Text&lt;/a&gt;" and
        ///     "&lt;a&gt;&lt;![CDATA[Text]]&gt;&lt;/a&gt;" are
        ///     similar and the Test will not fail.
        ///   </para>
        ///   <para>
        ///     The rating, if a node is similar, will be done by the
        ///     DifferenceEvaluators#Default.
        ///   </para>
        ///   <para>
        ///     Default is {@link #CheckForIdentical()}.
        ///   </para>
        /// </remarks>
        /* See {@link #withDifferenceEvaluator(DifferenceEvaluator)} */
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

        /// <summary>
        /// Establish a namespace context mapping from URI to prefix
        /// that will be used in Comparison.Detail.XPath.
        /// </summary>
        /// <remarks>
        /// Without a namespace context (or with an empty context) the
        /// XPath expressions will only use local names for elements and
        /// attributes.
        /// </remarks>
        public DiffBuilder WithNamespaceContext(IDictionary<string, string> ctx) {
            namespaceContext = ctx;
            return this;
        }

        /// <summary>
        /// Sets a non-default formatter for the differences found.
        /// </summary>
        public DiffBuilder WithComparisonFormatter(IComparisonFormatter formatter) {
            this.formatter = formatter;
            return this;
        }

        /// <summary>
        ///   Compare the Test-XML (WithTest(Object)) with the
        ///   Control-XML (Ccompare(Object)) and return the collected
        ///   differences in a Diff object.
        /// </summary>
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
            if (namespaceContext != null) {
                d.NamespaceContext = namespaceContext;
            }
            if (attributeFilter != null) {
                d.AttributeFilter = attributeFilter;
            }
            if (nodeFilter != null) {
                d.NodeFilter = nodeFilter;
            }
            d.Compare(Wrap(controlSource), Wrap(testSource));

            return formatter == null
                ? new Org.XmlUnit.Diff.Diff(controlSource, testSource,
                                            collectResultsListener.Differences)
                : new Org.XmlUnit.Diff.Diff(controlSource, testSource, formatter,
                                            collectResultsListener.Differences);
        }

        private ISource Wrap(ISource source) {
            ISource newSource = source;
            if (ignoreWhitespace) {
                newSource = new WhitespaceStrippedSource(newSource);
            }
            if (normalizeWhitespace) {
                newSource = new WhitespaceNormalizedSource(newSource);
            }
            if (ignoreComments) {
                newSource = ignoreCommentVersion == null
                    ? new CommentLessSource(newSource)
                    : new CommentLessSource(newSource, ignoreCommentVersion);
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
