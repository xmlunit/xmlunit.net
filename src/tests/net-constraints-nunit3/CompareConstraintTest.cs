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
using System.IO;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Org.XmlUnit.Diff;
using Org.XmlUnit.Input;

namespace Org.XmlUnit.Constraints {
    [TestFixture]
    public class CompareConstraintTest {

        [Test]
        public void TestIsIdenticalTo_WithErrorForAttributes_FailsWithExpectedMessage() {
            ExpectThrows("Expected attribute value 'xy' but was 'xyz'",
                         "at /Element[1]/@attr2",
                         () => Assert.That("<Element attr2=\"xyz\" attr1=\"12\"/>",
                                           CompareConstraint.IsIdenticalTo("<Element attr1=\"12\" attr2=\"xy\"/>")));
        }

        [Test]
        public void TestIsIdenticalTo_WithErrorForElementOrder_FailsWithExpectedMessage() {
            ExpectThrows("Expected child nodelist sequence '0' but was '1'",
                         "comparing <b...> at /a[1]/b[1] to <b...> at /a[1]/b[1]",
                         () => Assert.That("<a><c/><b/></a>",
                                           CompareConstraint.IsIdenticalTo("<a><b/><c/></a>")
                                           .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndText))));
        }

        [Test]
        public void TestIsSimilarTo_WithErrorForElementOrder_FailsWithExpectedMessage() {
            Assert.That("<a><c/><b/></a>", CompareConstraint.IsSimilarTo("<a><b/><c/></a>")
                                           .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndText)));
        }

        [Test]
        public void IsIdenticalTo_WithIgnoreWhitespaces_ShouldSucceed() {
            Assert.That("<a>\n  <b/>\n</a>",
                        CompareConstraint.IsIdenticalTo("<a><b/></a>").IgnoreWhitespace());
        }

        [Test]
        public void IsIdenticalTo_WithIgnoreElementContentWhitespaces_ShouldSucceed() {
            Assert.That("<a>\n  <b/>\n</a>",
                        CompareConstraint.IsIdenticalTo("<a><b/></a>").IgnoreElementContentWhitespace());
        }

        [Test]
        public void TestIsIdenticalTo_withIgnoreComments_shouldSucceed() {
            Assert.That("<a><!-- Test --></a>",
                        CompareConstraint.IsIdenticalTo("<a></a>").IgnoreComments());
        }

        [TestCase("1.0")]
        [TestCase("2.0")]
        public void TestIsIdenticalTo_withIgnoreCommentsVersion_shouldSucceed(string xsltVersion) {
            Assert.That("<a><!-- Test --></a>",
                        CompareConstraint.IsIdenticalTo("<a></a>").IgnoreCommentsUsingXSLTVersion(xsltVersion));
        }

        [Test]
        public void TestIsIdenticalTo_withNormalizeWhitespace_shouldSucceed() {
            Assert.That("<a>\n  <b>\n  Test\n  Node\n  </b>\n</a>",
                        CompareConstraint.IsIdenticalTo("<a><b>Test Node</b></a>")
                        .NormalizeWhitespace());
        }

        [Test]
        public void TestIsIdenticalTo_withNormalizeWhitespace_shouldFail() {
            ExpectThrows("Expected text value 'TestNode' but was 'Test Node'",
                         "/a[1]/b[1]",
                         () => Assert.That("<a>\n  <b>\n  Test\n  Node\n  </b>\n</a>",
                                           CompareConstraint.IsIdenticalTo("<a><b>TestNode</b></a>")
                                           .NormalizeWhitespace()));
        }

        [Test]
        public void TestIsSimilarTo_withSwappedElements_shouldSucceed() {
            Assert.That("<a><c/><b/></a>", CompareConstraint.IsSimilarTo("<a><b/><c/></a>")
                        .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndText)));
        }

        [Test]
        public void TestIsSimilarTo_withFileInput() {
            ExpectThrows("test1.xml", "test2.xml",
                         () => Assert.That(new StreamSource(TestResources.TESTS_DIR + "test1.xml"),
                                           CompareConstraint.IsSimilarTo(new StreamSource(TestResources.TESTS_DIR
                                                                                          + "test2.xml"))));
        }

        [Test]
        public void TestIsSimilarTo_withDifferenceEvaluator_shouldSucceed() {
            // prepare TestData
            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";

            // run Test
            Assert.That(test, CompareConstraint.IsSimilarTo(control)
                              .WithDifferenceEvaluator(IgnoreAttributeDifferenceEvaluator("attr")));
        }

        [Test]
        public void TestIsSimilarTo_withComparisonFormatter_shouldFailWithCustomMessage() {
            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";

            ExpectThrows("DESCRIPTION", "DETAIL-abc",
                         () => Assert.That(test, CompareConstraint
                                           .IsSimilarTo(control)
                                           .WithComparisonFormatter(new DummyComparisonFormatter())));
        }

        [Test]
        public void TestIsSimilarTo_withComparisonListener_shouldCollectChanges() {
            CounterComparisonListener comparisonListener = new CounterComparisonListener();
            string controlXml = "<a><b>Test Value</b><c>ABC</c></a>";
            string testXml = "<a><b><![CDATA[Test Value]]></b><c>XYZ</c></a>";
            ExpectThrows("Expected text value 'ABC' but was 'XYZ'", "",
                         () => Assert.That(testXml, CompareConstraint
                                           .IsSimilarTo(controlXml)
                                           .WithComparisonListeners(comparisonListener.Notify)));

            Assert.That(comparisonListener.Differents, Is.EqualTo(1));
            Assert.That(comparisonListener.Similars, Is.EqualTo(1));
            Assert.That(comparisonListener.Equal, Is.GreaterThan(10));
        }

        [Test]
        public void TestIsSimilarTo_withDifferenceListener_shouldCollectChanges() {
            CounterComparisonListener comparisonListener = new CounterComparisonListener();
            string controlXml = "<a><b>Test Value</b><c>ABC</c></a>";
            string testXml = "<a><b><![CDATA[Test Value]]></b><c>XYZ</c></a>";
            ExpectThrows("Expected text value 'ABC' but was 'XYZ'", "",
                         () => Assert.That(testXml, CompareConstraint
                                           .IsSimilarTo(controlXml)
                                           .WithDifferenceListeners(comparisonListener.Notify)));

            Assert.That(comparisonListener.Differents, Is.EqualTo(1));
            Assert.That(comparisonListener.Similars, Is.EqualTo(1));
            Assert.That(comparisonListener.Equal, Is.EqualTo(0));
        }

        [Test]
        public void TestCompareConstraintWrapper_shouldWriteFailedTestInput() {

            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";

            string fileName = GetTestResultFolder() + "/testCompareConstraintWrapper.xml";
            GetTestResultFolder();
            ExpectThrows("Expected attribute value 'abc' but was 'xyz'", "",
                         () => Assert.That(test,
                                           TestCompareConstraintWrapper.IsSimilarTo(control)
                                           .WithTestFileName(fileName)));
            Assert.That(new StreamSource(fileName),
                        CompareConstraint.IsSimilarTo(test));
        }

        [Test]
        public void TestDiff_withAttributeDifferences() {
            string control = "<a><b attr1=\"abc\" attr2=\"def\"></b></a>";
            string test = "<a><b attr1=\"xyz\" attr2=\"def\"></b></a>";

            ExpectThrows("Expected attribute value 'abc' but was 'xyz'", "",
                         () => Assert.That(test,
                                           CompareConstraint.IsSimilarTo(control)));
            Assert.That(test,
                        CompareConstraint.IsSimilarTo(control)
                        .WithAttributeFilter(a => "attr1" != a.Name));
        }

        [Test]
        public void TestDiff_withExtraNodes() {
            string control = "<a><b></b><c/></a>";
            string test = "<a><b></b><c/><d/></a>";

            ExpectThrows("Expected child nodelist length '2' but was '3'", "",
                         () => Assert.That(test,
                                           CompareConstraint.IsSimilarTo(control)));
            Assert.That(test,
                        CompareConstraint.IsSimilarTo(control)
                        .WithNodeFilter(n => "d" != n.Name));
        }

        /// <summary>
        /// Really only tests there is no NPE. See https://github.com/xmlunit/xmlunit/issues/81
        /// </summary>
        [Test]
        public void CanBeCombinedWithFailingMatcher() {
            Assert.That(() =>
                        Assert.That("foo", 
                            Does.Contain("bar")
                                    & CompareConstraint.IsSimilarTo("")),
                        Throws.TypeOf<AssertionException>());
        }

        [Test]
        public void CanBeCombinedWithPassingMatcher() {
            string xml = "<a><c/><b/></a>";
            Assert.That(xml, Does.Contain("c")
                        & CompareConstraint.IsSimilarTo("<a><b/><c/></a>")
                        .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByName)));
        }

        [Test]
        public void CantSetComparisonController() {
            Assert.That(() => CompareConstraint.IsSimilarTo("<foo/>")
                        .WithComparisonController(null),
                        Throws.TypeOf<NotImplementedException>());
        }

        [Test]
        public void CreatesAUsefulMessageWhenFailingCombinedWithNot() {
            ExpectThrows("not is similar to the control document", "<a><b></b><c/></a>",
                         () =>
                         Assert.That("<a><b></b><c/></a>",
                                     !CompareConstraint.IsSimilarTo("<a><b></b><c/></a>")));
        }

        private void ExpectThrows(string start, string detail, TestDelegate act) {
            Assert.That(act, Throws.TypeOf<AssertionException>()
                        .With.Property("Message").Contains(start)
                        .And.Property("Message").Contains(detail));
        }

        private static string GetTestResultFolder() {
            string folder = string.Format("{0}/{1}/{2}", TestResources.PREFIX,
                                          "build/net/test-report",
                                          typeof(CompareConstraintTest).Name);
            if (!Directory.Exists(folder)) {
                var info = Directory.CreateDirectory(folder);
                Assert.IsTrue(info.Exists);
            }
            return folder;
        }

        private class DummyComparisonFormatter : IComparisonFormatter {
            public string GetDetails(Comparison.Detail details, ComparisonType type, bool formatXml) {
                return "DETAIL-" + details.Value;
            }

            public string GetDescription(Comparison difference) {
                return "DESCRIPTION";
            }
        }

        private DifferenceEvaluator IgnoreAttributeDifferenceEvaluator(string attributeName) {
            return (comparison, outcome) => {
                XmlNode controlNode = comparison.ControlDetails.Target;
                if (controlNode is XmlAttribute) {
                    XmlAttribute attr = (XmlAttribute) controlNode;
                    if (attr.Name == attributeName) {
                        return ComparisonResult.EQUAL;
                    }
                }
                return outcome;
            };
        }

        private class CounterComparisonListener  {

            internal int Equal;
            internal int Similars;
            internal int Differents;

            public void Notify(Comparison comparison, ComparisonResult outcome) {
                switch (outcome) {
                    case ComparisonResult.EQUAL:
                        Equal++;
                        break;
                    case ComparisonResult.SIMILAR:
                        Similars++;
                        break;
                    case ComparisonResult.DIFFERENT:
                        Differents++;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
