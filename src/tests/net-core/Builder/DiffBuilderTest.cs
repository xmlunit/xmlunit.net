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
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using Org.XmlUnit.Diff;
using NUnit.Framework;

namespace Org.XmlUnit.Builder {
    [TestFixture]
    public class DiffBuilderTest {

        [Test]
        public void TestDiff_withoutIgnoreWhitespaces_shouldFail() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>\n  Test Value\n </b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withIgnoreWhitespaces_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>\n  Test Value\n </b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .IgnoreWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void IgnoreWhitespaceAndIgnoreXmlWhitespaceWorkAsExpected() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml1 = "<a>\n <b>\n  Test Value\n </b>\n</a>";
            string testXml2 = "<a>\n\u00a0<b>\n  Test Value\n </b>\n</a>";

            // run test
            var plainDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .IgnoreWhitespace()
                .Build();
            var xmlDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .IgnoreXmlWhitespace()
                .Build();
            var plainDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .IgnoreWhitespace()
                .Build();
            var xmlDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .IgnoreXmlWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(plainDiff1.HasDifferences(), "XML similar " + plainDiff1.ToString());
            Assert.IsFalse(xmlDiff1.HasDifferences(), "XML similar " + xmlDiff1.ToString());
            Assert.IsFalse(plainDiff2.HasDifferences(), "XML similar " + plainDiff2.ToString());
            Assert.IsTrue(xmlDiff2.HasDifferences(), "XML similar " + xmlDiff2.ToString());
        }

        [Test]
        public void TestDiff_withoutNormalizeWhitespaces_shouldFail() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>\n  Test Value\n </b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withNormalizeWhitespaces_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>\n  Test\nValue\n </b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                      .WithTest(Input.FromString(testXml).Build())
                      .NormalizeWhitespace()
                      .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void NormalizeWhitespaceAndNormalizeXmlWhitespaceWorkAsExpected() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml1 = "<a>\n <b>\n  Test\nValue\n </b>\n</a>";
            string testXml2 = "<a>\n <b>\n  Test\u00a0Value\n </b>\n</a>";

            // run test
            var plainDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .NormalizeWhitespace()
                .Build();
            var xmlDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .NormalizeXmlWhitespace()
                .Build();
            var plainDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .NormalizeWhitespace()
                .Build();
            var xmlDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .NormalizeXmlWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(plainDiff1.HasDifferences(), "XML similar " + plainDiff1.ToString());
            Assert.IsFalse(xmlDiff1.HasDifferences(), "XML similar " + xmlDiff1.ToString());
            Assert.IsFalse(plainDiff2.HasDifferences(), "XML similar " + plainDiff2.ToString());
            Assert.IsTrue(xmlDiff2.HasDifferences(), "XML similar " + xmlDiff2.ToString());
        }

        [Test]
        public void TestDiff_withNormalizeAndIgnoreWhitespaces_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>\n Test\n Value\n </b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .NormalizeWhitespace()
                .IgnoreWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_withCheckForIdentical_shouldFail() {
            // prepare testData
            string controlXml = "<a>Test Value</a>";
            string testXml = "<a><![CDATA[Test Value]]></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .CheckForIdentical()
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withCheckForSimilar_shouldSucceed() {
            // prepare testData
            string controlXml = "<a>Test Value</a>";
            string testXml = "<a><![CDATA[Test Value]]></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .CheckForSimilar()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_withoutIgnoreComments_shouldFail() {
            // prepare testData
            string controlXml = "<a><b><!-- A comment -->Test Value</b></a>";
            string testXml = "<a><b><!-- An other comment -->Test Value</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withIgnoreComments_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b><!-- A comment -->Test Value</b></a>";
            string testXml = "<a><b><!-- An other comment -->Test Value</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .IgnoreComments()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [TestCase("1.0")]
        [TestCase("2.0")]
        public void TestDiff_withIgnoreCommentsUsingVersion_shouldSucceed(string xsltVersion) {
            // prepare testData
            string controlXml = "<a><b><!-- A comment -->Test Value</b></a>";
            string testXml = "<a><b><!-- An other comment -->Test Value</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .IgnoreCommentsUsingXSLTVersion(xsltVersion)
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_fromCombinedSourceAndstring_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(controlXml)
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_fromBuilder_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml))
                .WithTest(Input.FromString(controlXml))
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_fromByteArray_shouldSucceed() {
            // prepare testData
            byte[] controlXml = Encoding.UTF8.GetBytes("<a><b>Test Value</b></a>");

            // run test
            var myDiff = DiffBuilder.Compare(controlXml).WithTest(controlXml).Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void TestDiff_fromStream_shouldSucceed() {
            // prepare testData
            using (FileStream fs = new FileStream(TestResources.ANIMAL_FILE,
                                                  FileMode.Open,
                                                  FileAccess.Read)) {
                using (StreamReader r = new StreamReader(TestResources.ANIMAL_FILE)) {

                    // run test
                    var myDiff = DiffBuilder.Compare(fs).WithTest(r).Build();

                    // validate result
                    Assert.IsFalse(myDiff.HasDifferences(),
                                   "XML similar " + myDiff.ToString());
                }
            }
        }

        [Test]
        public void TestDiff_withComparisonListener_shouldCallListener() {
            // prepare testData
            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";
            List<Difference> diffs = new List<Difference>();
            ComparisonListener comparisonListener = (comparison, outcome) => {
                diffs.Add(new Difference(comparison, outcome));
            };

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonListeners(comparisonListener)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
            Assert.That(diffs.Count, Is.GreaterThan(1));
        }

        [Test]
        public void TestDiff_withDifferenceListener_shouldCallListener() {
            // prepare testData
            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";
            List<Difference> diffs = new List<Difference>();
            ComparisonListener comparisonListener = (comparison, outcome) => {
                diffs.Add(new Difference(comparison, outcome));
            };

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceListeners(comparisonListener)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
            Assert.That(diffs.Count, Is.EqualTo(1));
            Assert.That(diffs[0].Result, Is.EqualTo(ComparisonResult.DIFFERENT));
            Assert.That(diffs[0].Comparison.Type, Is.EqualTo(ComparisonType.ATTR_VALUE));
        }

        [Test]
        public void TestDiff_withDifferenceEvaluator_shouldSucceed() {
            // prepare testData
            string control = "<a><b attr=\"abc\"></b></a>";
            string test = "<a><b attr=\"xyz\"></b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(IgnoreAttributeDifferenceEvaluator("attr"))
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withDifferenceEvaluator_shouldNotInterfereWithSimilar() {
            // prepare testData
            string control = "<a><b><![CDATA[abc]]></b></a>";
            string test = "<a><b>abc</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(
                    DifferenceEvaluators.Chain(DifferenceEvaluators.Default,
                                               IgnoreAttributeDifferenceEvaluator("attr")))
                .CheckForSimilar()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withCustomDifferenceEvaluator_shouldNotEvaluateSimilar() {
            // prepare testData
            string control = "<a><b><![CDATA[abc]]></b></a>";
            string test = "<a><b>abc</b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(IgnoreAttributeDifferenceEvaluator("attr"))
                .CheckForSimilar()
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
            IEnumerator<Difference> e = myDiff.Differences.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.That(e.Current.Result, Is.EqualTo(ComparisonResult.DIFFERENT));
        }

        private DifferenceEvaluator IgnoreAttributeDifferenceEvaluator(string attributeName) {
            return (comparison, outcome) => {
                XmlAttribute attr = comparison.ControlDetails.Target as XmlAttribute;
                if (attr != null && attr.Name == attributeName) {
                    return ComparisonResult.EQUAL;
                }
                return outcome;
            };
        }

        [Test]
        public void TestDiff_withDefaultComparisonController_shouldReturnAllDifferences() {
            // prepare testData
            string control = "<a><b attr1=\"abc\" attr2=\"def\"></b></a>";
            string test = "<a><b attr1=\"uvw\" attr2=\"xyz\"></b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences());
            Assert.That(myDiff.Differences.Count(), Is.EqualTo(2));
        }


        [Test]
        public void TestDiff_withStopWhenDifferentComparisonController_shouldReturnOnlyFirstDifference() {
            // prepare testData
            string control = "<a><b attr1=\"abc\" attr2=\"def\"></b></a>";
            string test = "<a><b attr1=\"uvw\" attr2=\"xyz\"></b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences());
            Assert.That(myDiff.Differences.Count(), Is.EqualTo(1));
        }

        [Test]
        public void TestDiff_withAttributeDifferences() {
            // prepare testData
            string control = "<a><b attr1=\"abc\" attr2=\"def\"></b></a>";
            string test = "<a><b attr1=\"uvw\" attr2=\"def\"></b></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences());
            Assert.That(myDiff.Differences.Count(), Is.EqualTo(1));

            // run test
            var myDiffWithFilter = DiffBuilder.Compare(control).WithTest(test)
                .WithAttributeFilter(a => "attr1" != a.Name)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .Build();

            // validate result
            Assert.IsFalse(myDiffWithFilter.HasDifferences());
        }

        [Test]
        public void TestDiff_withExtraNodes() {
            // prepare testData
            string control = "<a><b></b><c/></a>";
            string test = "<a><b></b><c/><d/></a>";

            // run test
            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences());
            Assert.That(myDiff.Differences.Count(), Is.EqualTo(1));

            // run test
            var myDiffWithFilter = DiffBuilder.Compare(control).WithTest(test)
                .WithNodeFilter(n => "d" != n.Name)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .Build();

            // validate result
            Assert.IsFalse(myDiffWithFilter.HasDifferences());
        }

        [Test]
        public void UsesCustomComparisonFormatter() {
            string control = "<a><b></b><c/></a>";
            string test = "<a><b></b><c/><d/></a>";

            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .WithComparisonFormatter(new DummyComparisonFormatter())
                .Build();

            Assert.AreEqual("foo", myDiff.ToString());
        }

        [Test]
        public void UsesCustomComparisonFormatterForDifferences() {
            string control = "<a><b></b><c/></a>";
            string test = "<a><b></b><c/><d/></a>";

            var myDiff = DiffBuilder.Compare(control).WithTest(test)
                .WithComparisonController(ComparisonControllers.StopWhenDifferent)
                .WithComparisonFormatter(new DummyComparisonFormatter())
                .Build();

            Assert.AreEqual("foo (DIFFERENT)", myDiff.Differences.First().ToString());
        }

        [Test]
        [Ignore("Looks as if XmlDocument stripped ECW by itself")]
        public void TestDiff_withoutIgnoreElementContentWhitespaces_shouldFail() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>Test Value</b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .Build();

            // validate result
            Assert.IsTrue(myDiff.HasDifferences(), myDiff.ToString());
        }

        [Test]
        public void TestDiff_withIgnoreElementContentWhitespaces_shouldSucceed() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml = "<a>\n <b>Test Value</b>\n</a>";

            // run test
            var myDiff = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml).Build())
                .IgnoreElementContentWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(myDiff.HasDifferences(), "XML similar " + myDiff.ToString());
        }

        [Test]
        public void IgnoreElementContentWhitespacesAndIgnoreXmlElementContentWhitespacesWorkAsExpected() {
            // prepare testData
            string controlXml = "<a><b>Test Value</b></a>";
            string testXml1 = "<a>\n <b>Test Value</b>\n</a>";
            string testXml2 = "<a>\n <b>Test Value</b>\u00a0</a>";

            // run test
            var plainDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .IgnoreElementContentWhitespace()
                .Build();
            var plainDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .IgnoreElementContentWhitespace()
                .Build();
            var xmlDiff1 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml1).Build())
                .IgnoreXmlElementContentWhitespace()
                .Build();
            var xmlDiff2 = DiffBuilder.Compare(Input.FromString(controlXml).Build())
                .WithTest(Input.FromString(testXml2).Build())
                .IgnoreXmlElementContentWhitespace()
                .Build();

            // validate result
            Assert.IsFalse(plainDiff1.HasDifferences(), "XML similar " + plainDiff1.ToString());
            Assert.IsFalse(plainDiff2.HasDifferences(), "XML similar " + plainDiff2.ToString());
            Assert.IsFalse(xmlDiff1.HasDifferences(), "XML similar " + xmlDiff1.ToString());
            Assert.IsTrue(xmlDiff2.HasDifferences(), "XML similar " + xmlDiff2.ToString());
        }

        internal class DummyComparisonFormatter : IComparisonFormatter {
            public string GetDescription(Comparison difference) {
                return "foo";
            }

            public string GetDetails(Comparison.Detail details, ComparisonType type,
                                     bool formatXml) {
                return "bar";
            }
        }

    }
}
