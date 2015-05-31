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
using System.Xml;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Strategy that matches control and tests nodes for comparison.
    /// </summary>
    public class DefaultNodeMatcher : INodeMatcher {
        private static readonly object DUMMY = new object();

        private readonly ElementSelector elementSelector;
        private readonly NodeTypeMatcher nodeTypeMatcher;

        /// <summary>
        /// Creates a new DefaultNodeMatcher using <see cref="ElementSelectors.Default"/> and <see cref="DefaultNodeTypeMatcher"/>.
        /// </summary>
        public DefaultNodeMatcher() : this(ElementSelectors.Default) {
        }

        /// <summary>
        /// Creates a new DefaultNodeMatcher using the given <see cref="ElementSelector"/> and <see cref="DefaultNodeTypeMatcher"/>.
        /// </summary>
        /// <param name="es">the element selector to use</param>
        public DefaultNodeMatcher(ElementSelector es) :
            this(es, DefaultNodeTypeMatcher) {
        }

        /// <summary>
        /// Creates a new DefaultNodeMatcher using the given <see cref="ElementSelector"/> and <see cref="NodeTypeMatcher"/>.
        /// </summary>
        /// <param name="es">the element selector to use</param>
        /// <param name="ntm">NodeTypeMatcher to use</param>
        public DefaultNodeMatcher(ElementSelector es, NodeTypeMatcher ntm)
        {
            elementSelector = es;
            nodeTypeMatcher = ntm;
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<XmlNode, XmlNode>>
            Match(IEnumerable<XmlNode> controlNodes,
                  IEnumerable<XmlNode> testNodes) {
            LinkedList<KeyValuePair<XmlNode, XmlNode>> matches =
                new LinkedList<KeyValuePair<XmlNode, XmlNode>>();
            IList<XmlNode> controlList = new List<XmlNode>(controlNodes);
            IList<XmlNode> testList = new List<XmlNode>(testNodes);
            ICollection<int> unmatchedTestIndexes = new HashSet<int>();
            for (int i = 0; i < testList.Count; i++) {
                unmatchedTestIndexes.Add(i);
            }
            int controlSize = controlList.Count;
            MatchInfo lastMatch = new MatchInfo(null, -1);
            for (int i = 0; i < controlSize; i++) {
                XmlNode control = controlList[i];
                MatchInfo testMatch = FindMatchingNode(control, testList,
                                                       lastMatch.Index,
                                                       unmatchedTestIndexes);
                if (testMatch != null) {
                    unmatchedTestIndexes.Remove(testMatch.Index);
                    matches.AddLast(new KeyValuePair<XmlNode,
                                    XmlNode>(control, testMatch.Node));
                }
            }
            return matches;
        }

        private MatchInfo FindMatchingNode(XmlNode searchFor,
                                           IList<XmlNode> searchIn,
                                           int indexOfLastMatch,
                                           ICollection<int> availableIndexes) {
            MatchInfo m = SearchIn(searchFor, searchIn,
                                   availableIndexes,
                                   indexOfLastMatch + 1, searchIn.Count);
            return m ?? SearchIn(searchFor, searchIn,
                                 availableIndexes,
                                 0, indexOfLastMatch);
        }

        private MatchInfo SearchIn(XmlNode searchFor,
                                   IList<XmlNode> searchIn,
                                   ICollection<int> availableIndexes,
                                   int fromInclusive, int toExclusive) {
            for (int i = fromInclusive; i < toExclusive; i++) {
                if (!availableIndexes.Contains(i)) {
                    continue;
                }
                if (NodesMatch(searchFor, searchIn[i])) {
                    return new MatchInfo(searchIn[i], i);
                }
            }
            return null;
        }

        private bool NodesMatch(XmlNode n1, XmlNode n2) {
            if (n1 is XmlElement && n2 is XmlElement) {
                return elementSelector(n1 as XmlElement, n2 as XmlElement);
            }
            return nodeTypeMatcher(n1.NodeType, n2.NodeType);
        }

        internal class MatchInfo {
            internal readonly XmlNode Node;
            internal readonly int Index;
            internal MatchInfo(XmlNode match, int index) {
                Node = match;
                Index = index;
            }
        }

        /// <summary>
        /// Decides whether to node types can be compared to each other.
        /// </summary>
        /// <param name="controlType">node type on the control side</param>
        /// <param name="testType">node type on the test side</param>
        /// <returns>whether the two node types can be compared</returns>
        public delegate bool NodeTypeMatcher(XmlNodeType controlType,
                                             XmlNodeType testType);

        /// <summary>
        /// Default implementation of <see cref="NodeTypeMatcher"/> that makes node types of the same type eligible and allows Text and CDATA nodes to be compared to each other.
        /// </summary>
        /// <param name="controlType">node type on the control side</param>
        /// <param name="testType">node type on the test side</param>
        /// <returns>whether the two node types can be compared</returns>
        public static bool DefaultNodeTypeMatcher(XmlNodeType controlType,
                                                  XmlNodeType testType) {
            return controlType == testType
                || (controlType == XmlNodeType.CDATA
                    && testType == XmlNodeType.Text)
                || (controlType == XmlNodeType.Text
                    && testType == XmlNodeType.CDATA);
        }
    }
}