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
    public class DifferenceTest {

        [Test]
        public void TrivialToStringTest() {
            Difference d = new Difference(new Comparison(ComparisonType.ELEMENT_TAG_NAME,
                                                         new XmlDocument().CreateElement("a"),
                                                         "/",
                                                         Convert.ToInt16("2"),
                                                         new XmlDocument().CreateElement("y"),
                                                         "/",
                                                         Convert.ToInt16("2")),
                                          ComparisonResult.EQUAL);
            Assert.That(d.ToString(), Is.StringContaining(" (EQUAL)"));
        }
    }
}