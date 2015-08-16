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
    /// Maps XmlNode to XPathContext by assuming all nodes
    /// passed in are child nodes of the same parent node who's
    /// XPathContext is provided as argument to the constructor.
    /// </summary>
    internal class ChildNodeXPathContextProvider {
        private readonly XPathContext xpathContext;
        private readonly Dictionary<XmlNode, int> childIndex;

        /// <summary>
        /// Creates an instance of ChildNodeXPathContextProvider.
        /// </summary>
        /// <param name="parentContext">parentContext context of the
        /// parent of all Nodes ever expected to be passed in as
        /// arguments to Map.  This XPathContext must be "positioned
        /// at" the parent element and already know about all its
        /// children.</param>
        /// <param name="children">all child nodes of the parent in
        /// the same order they are known to the XPathContext.</param>
        internal ChildNodeXPathContextProvider(XPathContext parentContext,
                                             IEnumerable<XmlNode> children) {
            this.xpathContext = (XPathContext) parentContext.Clone();
            childIndex = children.Select((n, i) => new { Node = n, Index = i })
                .ToDictionary(t => t.Node, t => t.Index);
        }

        internal XPathContext Map(XmlNode n) {
            XPathContext ctx = (XPathContext) xpathContext.Clone();
            ctx.NavigateToChild(childIndex[n]);
            return ctx;
        }
    }
}
