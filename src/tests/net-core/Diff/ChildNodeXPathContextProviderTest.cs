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
using NUnit.Framework;

namespace Org.XmlUnit.Diff {

    [TestFixture]
    public class ChildNodeXPathContextProviderTest {

        private XmlDocument doc;
        private XPathContext ctx;
        private List<XmlNode> elements;

        [SetUp]
        public void Init() {
            doc = new XmlDocument();
            elements = new List<XmlNode>();
            elements.Add(doc.CreateElement("foo"));
            elements.Add(doc.CreateElement("foo"));
            elements.Add(doc.CreateElement("bar"));
            elements.Add(doc.CreateElement("foo"));
            ctx = new XPathContext();
            ctx.SetChildren(elements.Select<XmlNode, XPathContext.INodeInfo>(ElementSelectors.TO_NODE_INFO));
        }
    
        [Test]
        public void ShouldReturnACopyOfOriginalXPathContext() {
            ChildNodeXPathContextProvider p = new ChildNodeXPathContextProvider(ctx, elements);
            XPathContext provided = p.Map(elements[0]);
            Assert.AreNotSame(ctx, provided);
        }

        [Test]
        public void ShouldFindCorrectChildIndex() {
            ChildNodeXPathContextProvider p = new ChildNodeXPathContextProvider(ctx, elements);
            XPathContext provided = p.Map(elements[1]);
            Assert.AreEqual("/foo[2]", provided.XPath);
        }

        [Test]
        public void ShouldThrowIfNodeIsNotInInitialList() {
            Assert.Throws<KeyNotFoundException>(() => {
            ChildNodeXPathContextProvider p = new ChildNodeXPathContextProvider(ctx, elements);
            p.Map(doc.CreateElement("foo"));});
        }
    }
}
