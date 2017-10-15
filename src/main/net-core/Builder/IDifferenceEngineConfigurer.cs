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

namespace Org.XmlUnit.Builder {
    /// <summary>
    /// Subset of the configuration options available for a DifferenceEngine.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     since XMLUnit 2.5.1
    ///   </para>
    /// </remarks>
    public interface IDifferenceEngineConfigurer<D> where D : IDifferenceEngineConfigurer<D> {

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
        D WithNodeMatcher(INodeMatcher nodeMatcher);

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
         *         .WithDifferenceEvaluator(
         *             DifferenceEvaluators.Chain(
         *                 DifferenceEvaluators.Default,
         *                 new MyCustomDifferenceEvaluator()))
         *         ....
         * </pre>
         */
        D WithDifferenceEvaluator(DifferenceEvaluator differenceEvaluator);

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
        D WithComparisonController(ComparisonController comparisonController);

        /// <summary>
        ///   Registers listeners that are notified of each comparison.
        /// </summary>
        /* @see org.xmlunit.diff.DifferenceEngine#addComparisonListener(ComparisonListener) */
        D WithComparisonListeners(params ComparisonListener[] comparisonListeners);

        /// <summary>
        ///   Registers listeners that are notified of each comparison with
        ///   outcome other than ComparisonResult#EQUAL.
        /// </summary>
        /* @see org.xmlunit.diff.DifferenceEngine#addDifferenceListener(ComparisonListener) */
        D WithDifferenceListeners(params ComparisonListener[] comparisonListeners);

        /// <summary>
        /// Establish a namespace context mapping from URI to prefix
        /// that will be used in Comparison.Detail.XPath.
        /// </summary>
        /// <remarks>
        /// Without a namespace context (or with an empty context) the
        /// XPath expressions will only use local names for elements and
        /// attributes.
        /// </remarks>
        D WithNamespaceContext(IDictionary<string, string> prefix2Uri);

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
        D WithAttributeFilter(Predicate<XmlAttribute> attributeFilter);

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
        D WithNodeFilter(Predicate<XmlNode> nodeFilter);

        /// <summary>
        /// Sets a non-default formatter for the differences found.
        /// </summary>
        D WithComparisonFormatter(IComparisonFormatter formatter);
    }
}
