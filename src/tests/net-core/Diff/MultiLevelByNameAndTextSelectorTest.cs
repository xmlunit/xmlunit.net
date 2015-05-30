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
    public class MultiLevelByNameAndTextSelectorTest {

        private XmlDocument doc;

        [SetUp]
        public void CreateDoc() {
            doc = new XmlDocument();
        }

        [Test]
        public void SingleLevel() {
            ElementSelectorsTest
                .ByNameAndText_SingleLevel(new MultiLevelByNameAndTextSelector(1).CanBeCompared,
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

            ElementSelector s = new MultiLevelByNameAndTextSelector(2).CanBeCompared;
            Assert.IsTrue(s(control, equal));
            Assert.IsTrue(s(control, equalC));
            Assert.IsFalse(s(control, noText));
            Assert.IsFalse(s(control, differentLevel));
            Assert.IsFalse(s(control, differentElement));
            Assert.IsFalse(s(control, differentText));
        }

        [Test]
        public void EmptyTexts() {
            XmlElement control = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child = doc.CreateElement(ElementSelectorsTest.BAR);
            control.AppendChild(doc.CreateTextNode(string.Empty));
            control.AppendChild(child);
            child.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));
            XmlElement test = doc.CreateElement(ElementSelectorsTest.FOO);
            XmlElement child2 = doc.CreateElement(ElementSelectorsTest.BAR);
            test.AppendChild(child2);
            child2.AppendChild(doc.CreateTextNode(ElementSelectorsTest.BAR));

            Assert.IsFalse(new MultiLevelByNameAndTextSelector(2)
                           .CanBeCompared(control, test));
            Assert.IsTrue(new MultiLevelByNameAndTextSelector(2, true)
                          .CanBeCompared(control, test));
        }
    }
}
