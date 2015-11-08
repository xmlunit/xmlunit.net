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

namespace Org.XmlUnit.Diff {

    [TestFixture]
    public class ByNameAndTextRecSelectorTest {

        private XmlDocument doc;

        [SetUp]
        public void CreateDoc() {
            doc = new XmlDocument();
        }

        [Test]
        public void ByNameAndTextRec_NamePart() {
            ElementSelectorsTest.PureElementNameComparisons(ByNameAndTextRecSelector.CanBeCompared,
                                                            doc);
        }

        [Test]
        public void ByNameAndTextRec_Single() {
            ElementSelectorsTest.ByNameAndText_SingleLevel(ByNameAndTextRecSelector.CanBeCompared,
                                                           doc);
        }

        [Test]
        public void ByNameAndTextRec() {
            XmlElement control = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child = doc.CreateElement(ElementSelectorsTest.BAR);
            control.AppendChild(child);
            child.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));
            XmlElement equal = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child2 = doc.CreateElement(ElementSelectorsTest.BAR);
            equal.AppendChild(child2);
            child2.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));
            XmlElement equalC = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child3 = doc.CreateElement(ElementSelectorsTest.BAR);
            equalC.AppendChild(child3);
            child3.AppendChild(doc.CreateCDataSection(ElementSelectorsTest.BAR));
            XmlElement noText = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement differentLevel = doc.CreateElement(ElementSelectorsTest.FOO);
            differentLevel.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));
            XmlElement differentElement = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child4 = doc.CreateElement(ElementSelectorsTest.FOO);
            differentElement.AppendChild(child4);
            child4.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));
            XmlElement differentText = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child5 = doc.CreateElement(ElementSelectorsTest.BAR);
            differentText.AppendChild(child5);
            child5.AppendChild(doc.CreateTextNode(ElementSelectorsTest.FOO));

            ElementSelector s = ByNameAndTextRecSelector.CanBeCompared;
            Assert.IsTrue(s(control, equal));
            Assert.IsTrue(s(control, equalC));
            Assert.IsFalse(s(control, noText));
            Assert.IsFalse(s(control, differentLevel));
            Assert.IsFalse(s(control, differentElement));
            Assert.IsFalse(s(control, differentText));
        }

        [Test]
        public void ByNameAndTextRec_Multilevel() {
            XmlDocument control = new XmlDocument();
            {
                XmlElement root = control.CreateElement("root");
                control.AppendChild(root);

                XmlElement controlSub = control.CreateElement("sub");
                root.AppendChild(controlSub);
                XmlElement controlSubSubValue = control.CreateElement("value");
                controlSub.AppendChild(controlSubSubValue);
                controlSubSubValue.AppendChild(control.CreateTextNode("1"));
                controlSubSubValue = control.CreateElement("value");
                controlSub.AppendChild(controlSubSubValue);
                controlSubSubValue.AppendChild(control.CreateTextNode("2"));

                controlSub = control.CreateElement("sub");
                root.AppendChild(controlSub);
                controlSubSubValue = control.CreateElement("value");
                controlSub.AppendChild(controlSubSubValue);
                controlSubSubValue.AppendChild(control.CreateTextNode("3"));
                controlSubSubValue = control.CreateElement("value");
                controlSub.AppendChild(controlSubSubValue);
                controlSubSubValue.AppendChild(control.CreateTextNode("4"));
            }
            XmlDocument test = new XmlDocument();
            {
                XmlElement root = test.CreateElement("root");
                test.AppendChild(root);

                XmlElement testSub = test.CreateElement("sub");
                root.AppendChild(testSub);
                XmlElement testSubValue = test.CreateElement("value");
                testSub.AppendChild(testSubValue);
                testSubValue.AppendChild(test.CreateTextNode("1"));
                testSubValue = test.CreateElement("value");
                testSub.AppendChild(testSubValue);
                testSubValue.AppendChild(test.CreateTextNode("2"));

                testSub = test.CreateElement("sub");
                root.AppendChild(testSub);
                testSubValue = test.CreateElement("value");
                testSub.AppendChild(testSubValue);
                testSubValue.AppendChild(test.CreateTextNode("4"));
                testSubValue = test.CreateElement("value");
                testSub.AppendChild(testSubValue);
                testSubValue.AppendChild(test.CreateTextNode("3"));
            }

            Org.XmlUnit.Builder.DiffBuilder builder = Org.XmlUnit.Builder.DiffBuilder.Compare(control)
                .WithTest(test).CheckForSimilar()
                .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.Or(ByNameAndTextRecSelector.CanBeCompared,
                                                                            ElementSelectors.ByName)));
            Diff d = builder.Build();
            Assert.IsTrue(d.HasDifferences(), d.ToString(new DefaultComparisonFormatter()));

            builder = Org.XmlUnit.Builder.DiffBuilder.Compare(control)
                .WithTest(test).CheckForSimilar()
                .WithNodeMatcher(new DefaultNodeMatcher(ByNameAndTextRecSelector.CanBeCompared,
                                                        ElementSelectors.ByName));
            d = builder.Build();
            Assert.IsFalse(d.HasDifferences(), d.ToString(new DefaultComparisonFormatter()));
        }
    }

}
