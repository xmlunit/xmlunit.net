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
    public class EvaluateXPathConstraintTest {

        [Test]
        public void XPathCountInXmlString() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<fruits>" +
                "<fruit name=\"apple\"/>" +
                "<fruit name=\"orange\"/>" +
                "<fruit name=\"banana\"/>" +
                "</fruits>";
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//fruits/fruit)",
                                                              Is.EqualTo("3")));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("//fruits/fruit/@name",
                                                              Is.EqualTo("apple")));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//fruits/fruit[@name=\"orange\"])",
                                                              Is.EqualTo("1")));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//fruits/fruit[@name=\"apricot\"])",
                                                              Is.EqualTo("0")));
        }

        [Test]
        public void XPathAttributeValueMatchingInXmlString() {
            string xml = "<a><b attr=\"abc\"></b></a>";
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("//a/b/@attr",
                                                              Is.EqualTo("abc")));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//a/b/c)",
                                                              Is.EqualTo("0")));
            Assert.That(() => Assert.That(xml,
                                          EvaluateXPathConstraint
                                          .HasXPath("//a/b/@attr", Is.EqualTo("something"))),
                        Throws.TypeOf<AssertionException>()
                        .With.Property("Message").Contains("But was:  \"abc\""));
        }

        [Test]
        public void XPathAttributeValueMatchingInXmlElement() {
            string xml = "<a><b attr=\"abc\"></b></a>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var xmlRootElement = doc.DocumentElement;
            Assert.That(xmlRootElement, EvaluateXPathConstraint.HasXPath("//a/b/@attr",
                                                                         Is.EqualTo("abc")));
        }

        [Test]
        public void XPathEvaluationWithNamespaceContext() {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
                "   <title>Search Engine Feed</title>" +
                "   <link href=\"https://en.wikipedia.org/wiki/Web_search_engine\"/>" +
                "   <entry>" +
                "       <title>Google</title>" +
                "       <id>goog</id>" +
                "   </entry>" +
                "   <entry>" +
                "       <title>Bing</title>" +
                "       <id>msft</id>" +
                "   </entry>" +
                "</feed>";

            var prefix2Uri = new Dictionary<string, string>();
            prefix2Uri["atom"] = "http://www.w3.org/2005/Atom";

            Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//atom:feed/atom:entry)",
                                                              Is.EqualTo("2"))
                        .WithNamespaceContext(prefix2Uri));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:title/text()",
                                                              Is.EqualTo("Google"))
                        .WithNamespaceContext(prefix2Uri));
            Assert.That(xml, EvaluateXPathConstraint.HasXPath("//atom:feed/atom:entry[2]/atom:title/text()",
                                                              Is.EqualTo("Bing"))
                        .WithNamespaceContext(prefix2Uri));
        }
    }
}