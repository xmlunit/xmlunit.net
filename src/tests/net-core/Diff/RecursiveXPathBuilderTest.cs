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
using NUnit.Framework;

namespace Org.XmlUnit.Diff {

    [TestFixture]
    public class RecursiveXPathBuilderTest {
        private XmlDocument doc;
        private RecursiveXPathBuilder builder = new RecursiveXPathBuilder();

        [SetUp]
        public void Initialize() {
            doc = new XmlDocument();
        }

        [Test]
        public void SoleElement() {
            Assert.AreEqual("/foo[1]",
                            builder.Map(doc.CreateElement("foo")).XPath);
        }

        [Test]
        public void RootElement() {
            XmlElement e = doc.CreateElement("foo");
            doc.AppendChild(e);
            Assert.AreEqual("/foo[1]",
                            builder.Map(e).XPath);
        }

        [Test]
        public void DeeperStructure() {
            XmlElement e = doc.CreateElement("foo");
            doc.AppendChild(e);
            XmlElement e2 = doc.CreateElement("foo");
            e.AppendChild(e2);
            e2.AppendChild(doc.CreateElement("foo"));
            XmlElement e3 = doc.CreateElement("foo");
            e2.AppendChild(e3);
            e2.AppendChild(doc.CreateComment("foo"));
            XmlComment c = doc.CreateComment("foo");
            e2.AppendChild(c);
            Assert.AreEqual("/foo[1]/foo[1]/foo[2]",
                            builder.Map(e3).XPath);
            Assert.AreEqual("/foo[1]/foo[1]/comment()[2]",
                            builder.Map(c).XPath);
        }

        [Test]
        public void Attribute() {
            XmlElement e = doc.CreateElement("foo");
            e.SetAttribute("foo", "bar");
            e.SetAttribute("baz", "xyzzy");
            Assert.AreEqual("/foo[1]/@foo",
                            builder.Map(e.GetAttributeNode("foo")).XPath);
        }

        [Test]
        public void NamespaceButNoMap() {
            XmlElement e = doc.CreateElement("foo", "http://www.xmlunit.org/test");
            e.SetAttribute("foo", "http://www.xmlunit.org/test", "bar");
            e.SetAttribute("baz", "http://www.xmlunit.org/test", "xyzzy");
            Assert.AreEqual("/foo[1]/@foo",
                            builder.Map(e.GetAttributeNode("foo")).XPath);
        }

        [Test]
        public void NamespaceWithMap() {
            XmlElement e = doc.CreateElement("foo", "http://www.xmlunit.org/test");
            e.SetAttribute("foo", "http://www.xmlunit.org/test", "bar");
            e.SetAttribute("baz", "http://www.xmlunit.org/test", "xyzzy");
            var p2u = new Dictionary<string, string> {{"x", "http://www.xmlunit.org/test"}};
            builder.NamespaceContext = p2u;
            Assert.AreEqual("/x:foo[1]/@x:foo",
                            builder.Map(e.GetAttributeNode("foo")).XPath);
        }
    }
}
