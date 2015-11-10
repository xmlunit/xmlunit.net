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
using System.Xml;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// XMLUnit's difference engine.
    /// </summary>
    public interface IDifferenceEngine {
        /// <summary>
        /// Is notified of each comparison.
        /// </summary>
        event ComparisonListener ComparisonListener;

        /// <summary>
        /// Is notified of each comparison with outcome {@link
        /// ComparisonResult#EQUAL}.
        /// </summary>
        event ComparisonListener MatchListener;

        /// <summary>
        /// Is notified of each comparison with
        /// outcome other than {@link ComparisonResult#EQUAL}.
        /// </summary>
        event ComparisonListener DifferenceListener;

        /// <summary>
        /// Sets the strategy for selecting nodes to compare.
        /// </summary>
        INodeMatcher NodeMatcher { set; }

        /// <summary>
        /// Evaluates the severity of a difference.
        /// </summary>
        DifferenceEvaluator DifferenceEvaluator { set; }

        /// <summary>
        /// Determines whether the comparison should stop after given
        /// difference has been found.
        /// </summary>
        ComparisonController ComparisonController { set; }

        /// <summary>
        /// Establish a namespace context mapping from URI to prefix
        /// that will be used in Comparison.Detail.XPath.
        /// </summary>
        /// <remarks>
        /// Without a namespace context (or with an empty context) the
        /// XPath expressions will only use local names for elements and
        /// attributes.
        /// </remarks>
        IDictionary<string, string> NamespaceContext { set; }

        /// <summary>
        /// Sets the optional strategy that decides which attributes to
        /// consider and which to ignore during comparison.
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
        Predicate<XmlAttribute> AttributeFilter { set; }

        /// <summary>
        /// Compares two pieces of XML and invokes the registered listeners.
        /// </summary>
        /// <param name="control">the control document holding the expected content</param>
        /// <param name="test">the document to test</param>
        void Compare(ISource control, ISource test);
    }
}