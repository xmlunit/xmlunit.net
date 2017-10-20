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
using System.Collections.Generic;
using System.Xml;
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Diff {
    /// <summary>
    /// Finds the XPathContext of a Node by recursively building up the XPathContext.
    /// </summary>
    public class RecursiveXPathBuilder {

        private IDictionary<string, string> prefix2uri;

        /// <summary>
        /// Establish a namespace context mapping from prefix to URI
        /// that will be used in Comparison.Detail.XPath.
        /// </summary>
        /// <remarks>
        /// Without a namespace context (or with an empty context) the
        /// XPath expressions will only use local names for elements and
        /// attributes.
        /// </remarks>
        public IDictionary<string, string> NamespaceContext
        {
            set {
                prefix2uri = value == null ? value : new Dictionary<string, string>(value);
            }
        }

        /// <summary>
        /// Maps a node to an XPathContext by recursing the DOM tree
        /// towards the root node.
        /// </summary>
        public XPathContext Map(XmlNode n) {
            if (n is XmlAttribute) {
                return GetXPathForAttribute(n as XmlAttribute);
            } else {
                return GetXPathForNonAttribute(n);
            }
        }

        private XPathContext GetXPathForNonAttribute(XmlNode n) {
            XmlNode parent = n.ParentNode;
            if (parent == null || parent is XmlDocument) {
                return new XPathContext(prefix2uri, n);
            }
            XPathContext parentContext = GetXPathForNonAttribute(parent);
            IEnumerable<XmlNode> children = parent.ChildNodes.Cast<XmlNode>();
            parentContext.SetChildren(children.Select<XmlNode, XPathContext.INodeInfo>(ElementSelectors.TO_NODE_INFO));
            ChildNodeXPathContextProvider cn = new ChildNodeXPathContextProvider(parentContext,
                                                                                 children);
            return cn.Map(n);
        }

        private XPathContext GetXPathForAttribute(XmlAttribute a) {
            XPathContext elementContext = GetXPathForNonAttribute(a.OwnerElement);
            XmlQualifiedName q = Nodes.GetQName(a);
            elementContext.AddAttribute(q);
            elementContext.NavigateToAttribute(q);
            return elementContext;
        }
    }
}
