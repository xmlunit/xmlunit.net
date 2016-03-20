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
using NUnit.Framework;

namespace Org.XmlUnit.Constraints {
    [TestFixture]
    public class HasXPathConstraintTest {

        [Test]
        public void XPathIsFoundInDocumentWithASingleOccurence() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<feed>" +
                "   <title>title</title>" +
                "   <entry>" +
                "       <title>title1</title>" +
                "       <id>id1</id>" +
                "   </entry>" +
                "</feed>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var xmlRootElement = doc.DocumentElement;
            
            Assert.That(xmlRootElement, HasXPathConstraint.HasXPath("entry/id"));
            Assert.That(xmlRootElement, HasXPathConstraint.HasXPath("entry/title"));
            Assert.That(xmlRootElement, !HasXPathConstraint.HasXPath("entry/description"));
        }

        [Test]
        public void XPathIsFoundInStringWithASingleOccurence() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<feed>" +
                "   <title>title</title>" +
                "   <entry>" +
                "       <title>title1</title>" +
                "       <id>id1</id>" +
                "   </entry>" +
                "</feed>";

            Assert.That(xml, HasXPathConstraint.HasXPath("//feed/entry/id"));
            Assert.That(xml, HasXPathConstraint.HasXPath("//feed/entry/title"));
            Assert.That(xml, !HasXPathConstraint.HasXPath("//feed/entry/description"));
        }

        [Test]
        public void XPathIsFoundInStringWithMultipleOccurences() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<feed>" +
                "   <title>title</title>" +
                "   <entry>" +
                "       <title>title1</title>" +
                "       <id>id1</id>" +
                "   </entry>" +
                "   <entry>" +
                "       <title>title2</title>" +
                "       <id>id2</id>" +
                "   </entry>" +
                "</feed>";

            Assert.That(xml, HasXPathConstraint.HasXPath("//feed/entry/id"));
            Assert.That(xml, HasXPathConstraint.HasXPath("//feed/entry/title"));
            Assert.That(xml, !HasXPathConstraint.HasXPath("//feed/entry/description"));
        }

        [Test]
        public void XPathAttributeIsFound() {
            string xml = "<a><b attr=\"abc\"></b></a>";
            Assert.That(xml, HasXPathConstraint.HasXPath("//a/b/@attr"));

            Assert.That(() => Assert.That(xml,
                                          HasXPathConstraint.HasXPath("//a/b[@attr=\"abcd\"]")),
                        Throws.TypeOf<AssertionException>()
                        .With.Property("Message").Contains(" returned no results"));
        }

        [Test]
        public void XPathIsFoundInDocumentWithNamespaceContextWithASingleOccurence() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
                "   <title>title</title>" +
                "   <entry>" +
                "       <title>title1</title>" +
                "       <id>id1</id>" +
                "   </entry>" +
                "</feed>";

            var prefix2Uri = new Dictionary<string, string>();
            prefix2Uri["atom"] = "http://www.w3.org/2005/Atom";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var xmlRootElement = doc.DocumentElement;

            Assert.That(xmlRootElement, HasXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:id").WithNamespaceContext(prefix2Uri));
            Assert.That(xmlRootElement, HasXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:title").WithNamespaceContext(prefix2Uri));
            Assert.That(xmlRootElement,
                        !HasXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:description").WithNamespaceContext(prefix2Uri));
        }
    }
}