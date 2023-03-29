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
using InputBuilder = Org.XmlUnit.Builder.Input;

namespace Org.XmlUnit.Util {
    [TestFixture]
    public class NodesTest {

        private const string FOO = "foo";
        private const string BAR = "bar";
        private const string SOME_URI = "urn:some:uri";

        private XmlDocument doc;

        [SetUp]
        public void CreateDoc() {
            doc = new XmlDocument();
        }

        [Test] public void QNameOfElementWithNoNs() {
            XmlElement e = doc.CreateElement(FOO);
            XmlQualifiedName q = Nodes.GetQName(e);
            Assert.AreEqual(FOO, q.Name);
            Assert.AreEqual(string.Empty, q.Namespace);
            Assert.AreEqual(new XmlQualifiedName(FOO), q);
        }

        [Test] public void QNameOfXmlElementWithNsNoPrefix() {
            XmlElement e = doc.CreateElement(FOO, SOME_URI);
            XmlQualifiedName q = Nodes.GetQName(e);
            Assert.AreEqual(FOO, q.Name);
            Assert.AreEqual(SOME_URI, q.Namespace);
            Assert.AreEqual(new XmlQualifiedName(FOO, SOME_URI), q);
        }

        [Test] public void QNameOfXmlElementWithNsAndPrefix() {
            XmlElement e = doc.CreateElement(BAR, FOO, SOME_URI);
            XmlQualifiedName q = Nodes.GetQName(e);
            Assert.AreEqual(FOO, q.Name);
            Assert.AreEqual(SOME_URI, q.Namespace);
            Assert.AreEqual(new XmlQualifiedName(FOO, SOME_URI), q);
        }

        [Test] public void MergeNoTexts() {
            XmlElement e = doc.CreateElement(FOO);
            Assert.AreEqual(string.Empty, Nodes.GetMergedNestedText(e));
        }

        [Test] public void MergeSingleTextNode() {
            XmlElement e = doc.CreateElement(FOO);
            XmlText t = doc.CreateTextNode(BAR);
            e.AppendChild(t);
            Assert.AreEqual(BAR, Nodes.GetMergedNestedText(e));
        }

        [Test] public void MergeSingleCDATASection() {
            XmlElement e = doc.CreateElement(FOO);
            XmlCDataSection t = doc.CreateCDataSection(BAR);
            e.AppendChild(t);
            Assert.AreEqual(BAR, Nodes.GetMergedNestedText(e));
        }

        [Test] public void MergeIgnoresTextOfChildren() {
            XmlElement e = doc.CreateElement(FOO);
            XmlElement c = doc.CreateElement("child");
            XmlText t = doc.CreateTextNode(BAR);
            e.AppendChild(c);
            c.AppendChild(t);
            Assert.AreEqual(string.Empty, Nodes.GetMergedNestedText(e));
        }

        [Test] public void MergeIgnoresComments() {
            XmlElement e = doc.CreateElement(FOO);
            XmlComment c = doc.CreateComment(BAR);
            e.AppendChild(c);
            Assert.AreEqual(string.Empty, Nodes.GetMergedNestedText(e));
        }

        [Test] public void MergeMultipleChildren() {
            XmlElement e = doc.CreateElement(FOO);
            XmlCDataSection c = doc.CreateCDataSection(BAR);
            e.AppendChild(c);
            e.AppendChild(doc.CreateElement("child"));
            XmlText t = doc.CreateTextNode(BAR);
            e.AppendChild(t);
            Assert.AreEqual(BAR + BAR, Nodes.GetMergedNestedText(e));
        }

        [Test] public void AttributeMapNoAttributes() {
            XmlElement e = doc.CreateElement(FOO);
            IDictionary<XmlQualifiedName, string> m = Nodes.GetAttributes(e);
            Assert.AreEqual(0, m.Count);
        }

        [Test] public void AttributeIDictionaryNoNS() {
            XmlElement e = doc.CreateElement(FOO);
            e.SetAttribute(FOO, BAR);
            IDictionary<XmlQualifiedName, string> m = Nodes.GetAttributes(e);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(BAR, m[new XmlQualifiedName(FOO)]);
        }

        [Test] public void AttributeIDictionarywithNS() {
            XmlElement e = doc.CreateElement(FOO);
            e.SetAttribute(FOO, SOME_URI, BAR);
            IDictionary<XmlQualifiedName, string> m = Nodes.GetAttributes(e);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(BAR, m[new XmlQualifiedName(FOO, SOME_URI)]);
        }

        [Test] public void AttributeMapWithFilter() {
            XmlElement e = doc.CreateElement(FOO);
            e.SetAttribute(FOO, BAR);
            e.SetAttribute("x", "y");
            IDictionary<XmlQualifiedName, string> m = Nodes.GetAttributes(e, a => FOO == a.Name);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(BAR, m[new XmlQualifiedName(FOO)]);
        }

        private XmlDocument HandleWsSetup() {
            return Convert.ToDocument(InputBuilder.FromString(
                "<root>\n"
                + "<!--\u00a0 trim\tme\u00a0 -->\n"
                + "<child attr=' trim me ' attr2='not me'>\n"
                + " trim me \n"
                + "</child><![CDATA[ trim me ]]>\n"
                + "<?target  trim me ?>\n"
                + "<![CDATA[          ]]>\n"
                + "</root>").Build());
        }

        private KeyValuePair<XmlDocument, XmlNode> StripWsSetup() {
            XmlDocument toTest = HandleWsSetup();
            return new KeyValuePair<XmlDocument,
                XmlNode>(toTest, Nodes.StripWhitespace(toTest));
        }

        private KeyValuePair<XmlDocument, XmlNode> NormalizeWsSetup() {
            XmlDocument toTest = HandleWsSetup();
            return new KeyValuePair<XmlDocument,
                XmlNode>(toTest, Nodes.NormalizeWhitespace(toTest));
        }

        private KeyValuePair<XmlDocument, XmlNode> StripXmlWsSetup() {
            XmlDocument toTest = HandleWsSetup();
            return new KeyValuePair<XmlDocument,
                XmlNode>(toTest, Nodes.StripXmlWhitespace(toTest));
        }

        [Test]
        public void StripWhitespaceWorks() {
            HandleWsWorks(StripWsSetup(), "trim\tme");
        }

        [Test]
        public void StripXmlWhitespaceWorks() {
            HandleWsWorks(StripXmlWsSetup(), "\u00a0 trim\tme\u00a0");
        }

        [Test]
        public void NormalizeWhitespaceWorks() {
            HandleWsWorks(NormalizeWsSetup(), "trim me");
        }

        private void HandleWsWorks(KeyValuePair<XmlDocument, XmlNode> s,
                                   string commentContent) {
            Assert.IsTrue(s.Value is XmlDocument);
            XmlNodeList top = s.Value.ChildNodes;
            Assert.AreEqual(1, top.Count);
            Assert.IsTrue(top[0] is XmlElement);
            Assert.AreEqual("root", top[0].Name);
            XmlNodeList rootsChildren = top[0].ChildNodes;
            Assert.AreEqual(4, rootsChildren.Count);
            Assert.IsTrue(rootsChildren[0] is XmlComment,
                          "should be comment, is " + rootsChildren[0].GetType());
            Assert.AreEqual(commentContent,
                            ((XmlComment) rootsChildren[0]).Data);
            Assert.IsTrue(rootsChildren[1] is XmlElement,
                          "should be element, is " + rootsChildren[1].GetType());
            Assert.AreEqual("child", rootsChildren[1].Name);
            Assert.IsTrue(rootsChildren[2] is XmlCDataSection,
                          "should be cdata, is " + rootsChildren[2].GetType());
            Assert.AreEqual("trim me",
                            ((XmlCDataSection) rootsChildren[2]).Data);
            Assert.IsTrue(rootsChildren[3] is XmlProcessingInstruction,
                          "should be PI, is " + rootsChildren[3].GetType());
            Assert.AreEqual("trim me",
                            ((XmlProcessingInstruction) rootsChildren[3]).Data);
            XmlNode child = rootsChildren[1];
            XmlNodeList grandChildren = child.ChildNodes;
            Assert.AreEqual(1, grandChildren.Count);
            Assert.IsTrue(grandChildren[0] is XmlText,
                          "should be text, is " + grandChildren[0].GetType());
            Assert.AreEqual("trim me", ((XmlText) grandChildren[0]).Data);
            XmlNamedNodeMap attrs = child.Attributes;
            Assert.AreEqual(2, attrs.Count);
            XmlAttribute a = (XmlAttribute) attrs.GetNamedItem("attr");
            Assert.AreEqual("trim me", a.Value);
            XmlAttribute a2 = (XmlAttribute) attrs.GetNamedItem("attr2");
            Assert.AreEqual("not me", a2.Value);
        }

        [Test]
        public void StripWhitespaceDoesntAlterOriginal() {
            HandleWsDoesntAlterOriginal(StripWsSetup());
        }

        [Test]
        public void StripXmlWhitespaceDoesntAlterOriginal() {
            HandleWsDoesntAlterOriginal(StripXmlWsSetup());
        }

        [Test]
        public void NormalizeWhitespaceDoesntAlterOriginal() {
            HandleWsDoesntAlterOriginal(NormalizeWsSetup());
        }

        private void HandleWsDoesntAlterOriginal(KeyValuePair<XmlDocument,
                                                 XmlNode> s) {
            XmlNodeList top = s.Key.ChildNodes;
            Assert.AreEqual(1, top.Count);
            Assert.IsTrue(top[0] is XmlElement);
            Assert.AreEqual("root", top[0].Name);
            XmlNodeList rootsChildren = top[0].ChildNodes;
            Assert.AreEqual(5, rootsChildren.Count);
            Assert.IsTrue(rootsChildren[0] is XmlComment,
                          "should be comment, is " + rootsChildren[0].GetType());
            Assert.AreEqual("\u00a0 trim\tme\u00a0 ",
                            ((XmlComment) rootsChildren[0]).Data);
            Assert.IsTrue(rootsChildren[1] is XmlElement,
                          "should be element, is " + rootsChildren[1].GetType());
            Assert.AreEqual("child", rootsChildren[1].Name);
            Assert.IsTrue(rootsChildren[2] is XmlCDataSection,
                          "should be cdata, is " + rootsChildren[2].GetType());
            Assert.AreEqual(" trim me ",
                            ((XmlCDataSection) rootsChildren[2]).Data);
            Assert.IsTrue(rootsChildren[3] is XmlProcessingInstruction,
                          "should be PI, is " + rootsChildren[3].GetType());
            Assert.AreEqual("trim me ",
                            ((XmlProcessingInstruction) rootsChildren[3]).Data);
            Assert.IsTrue(rootsChildren[4] is XmlCDataSection,
                          "should be cdata, is " + rootsChildren[4].GetType());
            Assert.AreEqual("          ",
                            ((XmlCDataSection) rootsChildren[4]).Data);
            XmlNode child = rootsChildren[1];
            XmlNodeList grandChildren = child.ChildNodes;
            Assert.AreEqual(1, grandChildren.Count);
            Assert.IsTrue(grandChildren[0] is XmlText,
                          "should be text, is " + grandChildren[0].GetType());
            Assert.AreEqual("\n trim me \n", ((XmlText) grandChildren[0]).Data);
            XmlNamedNodeMap attrs = child.Attributes;
            Assert.AreEqual(2, attrs.Count);
            XmlAttribute a = (XmlAttribute) attrs.GetNamedItem("attr");
            Assert.AreEqual(" trim me ", a.Value);
            XmlAttribute a2 = (XmlAttribute) attrs.GetNamedItem("attr2");
            Assert.AreEqual("not me", a2.Value);
        }

        [Test]
        public void Normalize() {
            Assert.AreSame("foo", Nodes.Normalize("foo"));
            Assert.AreSame("foo bar", Nodes.Normalize("foo bar"));
            Assert.AreEqual("foo bar", Nodes.Normalize("foo\nbar"));
            Assert.AreEqual("foo bar", Nodes.Normalize("foo  \r\n\t bar"));
        }

        [Test]
        public void StripECWWorks() {
            XmlNode orig = HandleWsSetup();
            XmlNode s = Nodes.StripElementContentWhitespace(orig);

            Assert.IsTrue(s is XmlDocument);
            XmlNodeList top = s.ChildNodes;
            Assert.AreEqual(1, top.Count);
            Assert.IsTrue(top[0] is XmlElement);
            Assert.AreEqual("root", top[0].Name);
            XmlNodeList rootsChildren = top[0].ChildNodes;
            Assert.AreEqual(4, rootsChildren.Count);
            Assert.IsTrue(rootsChildren[0] is XmlComment,
                          "should be comment, is " + rootsChildren[0].GetType());
            Assert.AreEqual("\u00a0 trim\tme\u00a0 ",
                            ((XmlComment) rootsChildren[0]).Data);
            Assert.IsTrue(rootsChildren[1] is XmlElement,
                          "should be element, is " + rootsChildren[1].GetType());
            Assert.AreEqual("child", rootsChildren[1].Name);
            Assert.IsTrue(rootsChildren[2] is XmlCDataSection,
                          "should be cdata, is " + rootsChildren[2].GetType());
            Assert.AreEqual(" trim me ",
                            ((XmlCDataSection) rootsChildren[2]).Data);
            Assert.IsTrue(rootsChildren[3] is XmlProcessingInstruction,
                          "should be PI, is " + rootsChildren[3].GetType());
            Assert.AreEqual("trim me ",
                            ((XmlProcessingInstruction) rootsChildren[3]).Data);
            XmlNode child = rootsChildren[1];
            XmlNodeList grandChildren = child.ChildNodes;
            Assert.AreEqual(1, grandChildren.Count);
            Assert.IsTrue(grandChildren[0] is XmlText,
                          "should be text, is " + grandChildren[0].GetType());
            Assert.AreEqual("\n trim me \n", ((XmlText) grandChildren[0]).Data);
            XmlNamedNodeMap attrs = child.Attributes;
            Assert.AreEqual(2, attrs.Count);
            XmlAttribute a = (XmlAttribute) attrs.GetNamedItem("attr");
            Assert.AreEqual(" trim me ", a.Value);
            XmlAttribute a2 = (XmlAttribute) attrs.GetNamedItem("attr2");
            Assert.AreEqual("not me", a2.Value);
        }
    }
}
