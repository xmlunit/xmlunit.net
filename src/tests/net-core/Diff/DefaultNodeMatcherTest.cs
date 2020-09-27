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
    public class DefaultNodeMatcherTest {

        private XmlDocument doc;

        [SetUp]
        public void CreateDoc() {
            doc = new XmlDocument();
        }

        [Test]
        public void ElementSelectorsAreQueriedInSequence() {
            XmlElement control1 = doc.CreateElement("a");
            control1.AppendChild(doc.CreateTextNode("foo"));
            XmlElement control2 = doc.CreateElement("a");
            control2.AppendChild(doc.CreateTextNode("bar"));

            XmlElement test1 = doc.CreateElement("a");
            test1.AppendChild(doc.CreateTextNode("baz"));
            XmlElement test2 = doc.CreateElement("a");
            test2.AppendChild(doc.CreateTextNode("foo"));

            DefaultNodeMatcher m =
                new DefaultNodeMatcher(ElementSelectors.ByNameAndText,
                    ElementSelectors.ByName);
            List<KeyValuePair<XmlNode, XmlNode>> result =
                m.Match(new XmlNode[] { control1, control2},
                        new XmlNode[] { test1, test2 }).ToList();
            Assert.AreEqual(result.Count, 2);

            // ByNameAndText
            Assert.AreSame(result[0].Key, control1);
            Assert.AreSame(result[0].Value, test2);

            // ByName
            Assert.AreSame(result[1].Key, control2);
            Assert.AreSame(result[1].Value, test1);
        }

        [Test]
        // https://github.com/xmlunit/xmlunit/issues/197
        public void ElementSelectorsAreQueriedInSequenceWithConditionalSelector() {
            XmlElement control1 = doc.CreateElement("a");
            control1.AppendChild(doc.CreateTextNode("foo"));
            XmlElement control2 = doc.CreateElement("a");
            control2.AppendChild(doc.CreateTextNode("bar"));

            XmlElement test1 = doc.CreateElement("a");
            test1.AppendChild(doc.CreateTextNode("baz"));
            XmlElement test2 = doc.CreateElement("a");
            test2.AppendChild(doc.CreateTextNode("foo"));

            DefaultNodeMatcher m =
                new DefaultNodeMatcher(ElementSelectors.SelectorForElementNamed("a", ElementSelectors.ByNameAndText),
                    ElementSelectors.ByName);
            List<KeyValuePair<XmlNode, XmlNode>> result =
                m.Match(new XmlNode[] { control1, control2},
                        new XmlNode[] { test1, test2 }).ToList();
            Assert.AreEqual(result.Count, 2);

            // ByNameAndText
            Assert.AreSame(result[0].Key, control1);
            Assert.AreSame(result[0].Value, test2);

            // ByName
            Assert.AreSame(result[1].Key, control2);
            Assert.AreSame(result[1].Value, test1);
        }

        [Test]
        public void ElementSelectorsAreQueriedInSequenceWithControlNodesSwapped() {
            XmlElement control1 = doc.CreateElement("a");
            control1.AppendChild(doc.CreateTextNode("bar"));
            XmlElement control2 = doc.CreateElement("a");
            control2.AppendChild(doc.CreateTextNode("foo"));

            XmlElement test1 = doc.CreateElement("a");
            test1.AppendChild(doc.CreateTextNode("foo"));
            XmlElement test2 = doc.CreateElement("a");
            test2.AppendChild(doc.CreateTextNode("baz"));

            DefaultNodeMatcher m =
                new DefaultNodeMatcher(ElementSelectors.ByNameAndText,
                    ElementSelectors.ByName);
            List<KeyValuePair<XmlNode, XmlNode>> result =
                m.Match(new XmlNode[] { control1, control2},
                        new XmlNode[] { test1, test2 }).ToList();
            Assert.AreEqual(result.Count, 2);

            // ByNameAndText
            Assert.AreSame(result[0].Key, control2);
            Assert.AreSame(result[0].Value, test1);

            // ByName
            Assert.AreSame(result[1].Key, control1);
            Assert.AreSame(result[1].Value, test2);
        }
    }
}
