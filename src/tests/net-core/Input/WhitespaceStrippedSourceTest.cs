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

using System.Xml;
using Org.XmlUnit.Util;
using NUnit.Framework;

namespace Org.XmlUnit.Input {

    [TestFixture]
    public class WhitespaceStrippedSourceTest {

        [Test]
        public void WhitespaceIsStrippedProperly() {
            WhitespaceIsStrippedProperly(new XmlDocument());
        }

        // see https://github.com/xmlunit/xmlunit.net/issues/38
        [Test]
        public void WhitespaceIsStrippedProperlyEvenWithPreserveWhitespaceDoc() {
            XmlDocument testDoc = new XmlDocument();
            testDoc.PreserveWhitespace = true;
            WhitespaceIsStrippedProperly(testDoc);
        }

        private void WhitespaceIsStrippedProperly(XmlDocument testDoc) {
            string testXml = "<a>\n <b>\n  Test Value\n </b>\n</a>";
            testDoc.LoadXml(testXml);
            WhitespaceStrippedSource s = new WhitespaceStrippedSource(new DOMSource(testDoc));
            XmlNode root = s.Node;
            Assert.AreEqual(1, root.ChildNodes.Count);
            XmlNode a = root.FirstChild;
            Assert.AreEqual(1, a.ChildNodes.Count);
            XmlNode b = a.FirstChild;
            Assert.AreEqual(1, b.ChildNodes.Count);
            Assert.AreEqual("Test Value", b.FirstChild.Value);
        }
    }
}
