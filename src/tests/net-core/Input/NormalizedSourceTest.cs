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
using System.Xml;
using NUnit.Framework;

namespace Org.XmlUnit.Input {
    [TestFixture]
    public class NormalizedSourceTest {

        private XmlDocument doc;

        [SetUp]
        public void createDoc() {
            doc = new XmlDocument();
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantWrapNullSource() {
            new NormalizedSource((ISource) null);
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantWrapNullNode() {
            new NormalizedSource((XmlNode) null);
        }

        [Test]
        public void NormalizesNode() {
            XmlElement control = doc.CreateElement("e");
            control.AppendChild(doc.CreateTextNode("a"));
            control.AppendChild(doc.CreateTextNode(""));
            control.AppendChild(doc.CreateTextNode("b"));
            NormalizedSource s = new NormalizedSource(control);
            Assert.AreEqual(1, s.Node.ChildNodes.Count);
            Assert.AreEqual("ab", s.Node.FirstChild.Value);
        }

        [Test]
        public void NormalizesDOMSource() {
            XmlElement control = doc.CreateElement("e");
            doc.AppendChild(control);
            control.AppendChild(doc.CreateTextNode("a"));
            control.AppendChild(doc.CreateTextNode(""));
            control.AppendChild(doc.CreateTextNode("b"));
            NormalizedSource s = new NormalizedSource(new DOMSource(doc));
            Assert.AreEqual(1, s.Node.ChildNodes.Count);
            Assert.AreEqual(1, s.Node.FirstChild.ChildNodes.Count);
            Assert.AreEqual("ab", s.Node.FirstChild.FirstChild.Value);
        }

        [Test]
        public void KeepsSystemId() {
            DOMSource d = new DOMSource(doc);
            d.SystemId = "foo";
            NormalizedSource s = new NormalizedSource(d);
            Assert.AreEqual("foo", s.SystemId);
        }
    }
}
