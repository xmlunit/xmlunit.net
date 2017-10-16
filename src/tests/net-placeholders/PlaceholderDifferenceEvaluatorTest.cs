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
using Org.XmlUnit;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {

    [TestFixture]
    public class PlaceholderDifferenceEvaluatorTest {
        [Test]
        public void Regression_NoPlaceholder_Equal() {
            string control = "<elem1><elem11>123</elem11></elem1>";
            string test = "<elem1><elem11>123</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void Regression_NoPlaceholder_Different() {
            string control = "<elem1><elem11>123</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (Difference difference in diff.Differences) {
                count++;
                Assert.AreEqual(ComparisonResult.DIFFERENT, difference.Result);
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Regression_NoPlaceholder_Different_EmptyExpectedElement() {
            string control = "<elem1><elem11/></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (Difference difference in diff.Differences) {
                count++;
                Assert.AreEqual(ComparisonResult.DIFFERENT, difference.Result);

                Comparison comparison = difference.Comparison;
                if (count == 1) {
                    string xpath = "/elem1[1]/elem11[1]";
                    Assert.AreEqual(ComparisonType.CHILD_NODELIST_LENGTH, comparison.Type);
                    Assert.AreEqual(xpath, comparison.ControlDetails.XPath);
                    Assert.AreEqual(0, comparison.ControlDetails.Value);
                    Assert.AreEqual(xpath, comparison.TestDetails.XPath);
                    Assert.AreEqual(1, comparison.TestDetails.Value);
                } else {
                    Assert.AreEqual(ComparisonType.CHILD_LOOKUP, comparison.Type);
                    Assert.AreEqual(null, comparison.ControlDetails.XPath);
                    Assert.AreEqual(null, comparison.ControlDetails.Value);
                    Assert.AreEqual("/elem1[1]/elem11[1]/text()[1]", comparison.TestDetails.XPath);
                    Assert.AreEqual(new XmlQualifiedName("#text"), comparison.TestDetails.Value);
                }
            }
            Assert.AreEqual(2, count);
        }

        [Test]
        public void HasIgnorePlaceholder_Equal_NoWhitespaceInPlaceholder() {
            string control = "<elem1><elem11>${xmlunit.ignore}</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Equal_NoWhitespaceInPlaceholder_CDATA_Control() {
            string control = "<elem1><elem11><![CDATA[${xmlunit.ignore}]]></elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(DifferenceEvaluators.Chain(
                    DifferenceEvaluators.Default, new PlaceholderDifferenceEvaluator().Evaluate))
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Equal_NoWhitespaceInPlaceholder_CDATA_TEST() {
            string control = "<elem1><elem11>${xmlunit.ignore}</elem11></elem1>";
            string test = "<elem1><elem11><![CDATA[abc]]></elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(DifferenceEvaluators.Chain(
                    DifferenceEvaluators.Default, new PlaceholderDifferenceEvaluator().Evaluate))
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_CustomDelimiters_Equal_NoWhitespaceInPlaceholder() {
            string control = "<elem1><elem11>#{xmlunit.ignore}</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator("#\\{", null).Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Equal_StartAndEndWhitespacesInPlaceholder() {
            string control = "<elem1><elem11>${  xmlunit.ignore  }</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Equal_EmptyActualElement() {
            string control = "<elem1><elem11>${xmlunit.ignore}</elem11></elem1>";
            string test = "<elem1><elem11/></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Exception_ExclusivelyOccupy() {
            string control = "<elem1><elem11> ${xmlunit.ignore}abc</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            DiffBuilder diffBuilder = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate);

            Assert.Throws<XMLUnitException>(() => diffBuilder.Build(), "The placeholder must exclusively occupy the text node.");
        }

        [Test]
        public void Regression_NoPlaceholder_Attributes_Equal() {
            string control = "<elem1 attr='123'/>";
            string test = "<elem1 attr='123'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void Regression_NoPlaceholder_Attributes_Different() {
            string control = "<elem1 attr='123'/>";
            string test = "<elem1 attr='abc'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate).Build();

            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (Difference difference in diff.Differences) {
                count++;
                Assert.AreEqual(ComparisonResult.DIFFERENT, difference.Result);
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Regression_NoPlaceholder_Missing_Attribute() {
            string control = "<elem1/>";
            string test = "<elem1 attr='abc'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsTrue(diff.HasDifferences());
            int count = 0;
            foreach (Difference difference in diff.Differences) {
                count++;
                Assert.AreEqual(ComparisonResult.DIFFERENT, difference.Result);

                Comparison comparison = difference.Comparison;
                if (count == 1) {
                    string xpath = "/elem1[1]";
                    Assert.AreEqual(ComparisonType.ELEMENT_NUM_ATTRIBUTES, comparison.Type);
                    Assert.AreEqual(xpath, comparison.ControlDetails.XPath);
                    Assert.AreEqual(0, comparison.ControlDetails.Value);
                    Assert.AreEqual(xpath, comparison.TestDetails.XPath);
                    Assert.AreEqual(1, comparison.TestDetails.Value);
                } else {
                    Assert.AreEqual(ComparisonType.ATTR_NAME_LOOKUP, comparison.Type);
                    Assert.AreEqual("/elem1[1]", comparison.ControlDetails.XPath);
                    Assert.AreEqual(null, comparison.ControlDetails.Value);
                    Assert.AreEqual("/elem1[1]/@attr", comparison.TestDetails.XPath);
                }
            }
            Assert.AreEqual(2, count);
        }

        [Test]
        public void HasIgnorePlaceholder_Attribute_Equal_NoWhitespaceInPlaceholder() {
            string control = "<elem1 attr='${xmlunit.ignore}'/>";
            string test = "<elem1 attr='abc'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_CustomDelimiters_Attribute_Equal_NoWhitespaceInPlaceholder() {
            string control = "<elem1 attr='#{xmlunit.ignore}'/>";
            string test = "<elem1 attr='abc'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator("#\\{", null).Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Attribute_Equal_StartAndEndWhitespacesInPlaceholder() {
            string control = "<elem1 attr='${  xmlunit.ignore  }'/>";
            string test = "<elem1 attr='abc'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Attribute_Equal_MissingActualAttribute() {
            string control = "<elem1 attr='${xmlunit.ignore}'/>";
            string test = "<elem1/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void MissingAttributeWithMoreThanOneAttribute() {
            string control = "<elem1 attr='${xmlunit.ignore}' a='b'/>";
            string test = "<elem1 a='b'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void MissingAttributeWithMoreThanOneIgnore() {
            string control = "<elem1 attr='${xmlunit.ignore}' a='${xmlunit.ignore}'/>";
            string test = "<elem1/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void MissingAttributeWithMissingControlAttribute() {
            string control = "<elem1 attr='${xmlunit.ignore}' a='${xmlunit.ignore}'/>";
            string test = "<elem1 b='a'/>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate)
                .Build();

            Assert.IsTrue(diff.HasDifferences());
        }

        [Test]
        public void HasIgnorePlaceholder_Attribute_Exception_ExclusivelyOccupy() {
            string control = "<elem1 attr='${xmlunit.ignore}abc'/>";
            string test = "<elem1 attr='abc'/>";
            DiffBuilder diffBuilder = DiffBuilder.Compare(control).WithTest(test)
                .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator().Evaluate);

            Assert.Throws<XMLUnitException>(() => diffBuilder.Build(), "The placeholder must exclusively occupy the text node.");
        }

    }
}
