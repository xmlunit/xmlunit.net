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
using NUnit.Framework;
using InputBuilder = Org.XmlUnit.Builder.Input;

namespace Org.XmlUnit.Diff {

    [TestFixture]
    public class DifferenceEvaluatorsTest {

        internal class Evaluator {
            internal bool Called = false;
            private readonly ComparisonResult Ret;
            internal ComparisonResult Orig;
            internal Evaluator(ComparisonResult ret) {
                Ret = ret;
            }
            public ComparisonResult Evaluate(Comparison comparison,
                                             ComparisonResult orig) {
                Called = true;
                Orig = orig;
                return Ret;
            }
        }

        [Test]
        public void EmptyFirstJustWorks() {
            DifferenceEvaluator d = DifferenceEvaluators.First();
            Assert.AreEqual(ComparisonResult.DIFFERENT,
                            d(null, ComparisonResult.DIFFERENT));
        }

        [Test]
        public void FirstChangeWinsInFirst() {
            Evaluator e1 = new Evaluator(ComparisonResult.DIFFERENT);
            Evaluator e2 = new Evaluator(ComparisonResult.EQUAL);
            DifferenceEvaluator d = DifferenceEvaluators.First(e1.Evaluate,
                                                               e2.Evaluate);
            Assert.AreEqual(ComparisonResult.DIFFERENT,
                            d(null, ComparisonResult.SIMILAR));
            Assert.IsTrue(e1.Called);
            Assert.IsFalse(e2.Called);
            e1.Called = false;
            Assert.AreEqual(ComparisonResult.EQUAL,
                            d(null, ComparisonResult.DIFFERENT));
            Assert.IsTrue(e1.Called);
            Assert.IsTrue(e2.Called);
        }

        [Test]
        public void AllEvaluatorsAreCalledInSequence() {
            Evaluator e1 = new Evaluator(ComparisonResult.SIMILAR);
            Evaluator e2 = new Evaluator(ComparisonResult.EQUAL);
            DifferenceEvaluator d = DifferenceEvaluators.Chain(e1.Evaluate,
                                                               e2.Evaluate);
            Assert.AreEqual(ComparisonResult.EQUAL,
                            d(null, ComparisonResult.DIFFERENT));

            Assert.IsTrue(e1.Called);
            Assert.That(e1.Orig, Is.EqualTo(ComparisonResult.DIFFERENT)); // passed initial ComparisonResult
            Assert.IsTrue(e2.Called);
            Assert.That(e2.Orig, Is.EqualTo(ComparisonResult.SIMILAR)); // passed ComparisonResult from e1
}

        [Test]
        public void DowngradeDifferencesToEqualDowngradesMatchingTypes() {
            DifferenceEvaluator d = DifferenceEvaluators
                .DowngradeDifferencesToEqual(ComparisonType.XML_VERSION,
                                             ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.EQUAL,
                            d(new Comparison(ComparisonType.XML_VERSION,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.SIMILAR));
        }

        [Test]
        public void DowngradeDifferencesToEqualLeavesUnknownTypesAlone() {
            DifferenceEvaluator d = DifferenceEvaluators
                .DowngradeDifferencesToEqual(ComparisonType.XML_VERSION,
                                         ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.SIMILAR,
                            d(new Comparison(ComparisonType.XML_ENCODING,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.SIMILAR));
        }

        [Test]
        public void DowngradeDifferencesToSimilarDowngradesMatchingTypes() {
            DifferenceEvaluator d = DifferenceEvaluators
                .DowngradeDifferencesToSimilar(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.SIMILAR,
                            d(new Comparison(ComparisonType.XML_VERSION,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.DIFFERENT));
        }

        [Test]
        public void DowngradeDifferencesToSimilarLeavesUnknownTypesAlone() {
            DifferenceEvaluator d = DifferenceEvaluators
                .DowngradeDifferencesToSimilar(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.DIFFERENT,
                            d(new Comparison(ComparisonType.XML_ENCODING,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.DIFFERENT));
        }

        [Test]
        public void DowngradeDifferencesToSimilarLeavesEqualResultsAlone() {
            DifferenceEvaluator d = DifferenceEvaluators
                .DowngradeDifferencesToSimilar(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.EQUAL,
                            d(new Comparison(ComparisonType.XML_VERSION,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.EQUAL));
        }

        [Test]
        public void UpgradeDifferencesToDifferentUpgradesMatchingTypes() {
            DifferenceEvaluator d = DifferenceEvaluators
                .UpgradeDifferencesToDifferent(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.DIFFERENT,
                            d(new Comparison(ComparisonType.XML_VERSION,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.SIMILAR));
        }

        [Test]
        public void UpgradeDifferencesToDifferentLeavesUnknownTypesAlone() {
            DifferenceEvaluator d = DifferenceEvaluators
                .UpgradeDifferencesToDifferent(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.SIMILAR,
                            d(new Comparison(ComparisonType.XML_ENCODING,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.SIMILAR));
        }

        [Test]
        public void UpgradeDifferencesToDifferentLeavesEqualResultsAlone() {
            DifferenceEvaluator d = DifferenceEvaluators
                .UpgradeDifferencesToDifferent(ComparisonType.XML_VERSION,
                                               ComparisonType.XML_STANDALONE);
            Assert.AreEqual(ComparisonResult.EQUAL,
                            d(new Comparison(ComparisonType.XML_VERSION,
                                             null, null, null, null,
                                             null, null, null, null),
                              ComparisonResult.EQUAL));
        }

        [Test]
        public void IgnorePrologIgnoresAdditionalContentInProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologIgnoresXMLDeclarationDifferences() {
            var differences =
                Compare(
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologIgnoresPrologCommentDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<?foo some PI ?>\n"
                        + "<!-- some other comment -->"
                        + "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologIgnoresPrologProcessingInstructionDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some other PI ?>\n"
                        + "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologIgnoresPrologWhitespaceDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment --> "
                        + "<?foo some PI ?>"
                        + "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologIgnoresDoesntIgnoreElementName() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<foo/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>");
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        public void IgnorePrologDoesntIgnoreCommentsOutsideOfProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<!-- some comment -->"
                        + "</foo>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<!-- some other comment -->"
                        + "</foo>");
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        public void IgnorePrologDoesntIgnorePIsOutsideOfProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<?foo some PI ?>\n"
                        + "</foo>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<?foo some other PI ?>\n"
                        + "</foo>");
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        [Ignore("doctype tests fail 'Document Type Declaration (DTD) is prohibited in this XML.'")]
        public void IgnorePrologIgnoresPresenceOfDoctype() {
            var differences =
                Compare("<!DOCTYPE test ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>",
                        "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        [Ignore("doctype tests fail 'Document Type Declaration (DTD) is prohibited in this XML.'")]
        public void IgnorePrologIgnoresNameOfDoctype() {
            var differences =
                Compare("<!DOCTYPE foo ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>",
                        "<!DOCTYPE test ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresAdditionalContentInProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<bar/>");
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresXMLDeclarationDifferences() {
            var differences =
                Compare(
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresPrologCommentDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<?foo some PI ?>\n"
                        + "<!-- some other comment -->"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresPrologProcessingInstructionDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some other PI ?>\n"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresPrologWhitespaceDifferences() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment --> "
                        + "<?foo some PI ?>"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeIgnoresDoesntIgnoreElementName() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<foo/>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<!-- some comment -->"
                        + "<?foo some PI ?>\n"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeDoesntIgnoreCommentsOutsideOfProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<!-- some comment -->"
                        + "</foo>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<!-- some other comment -->"
                        + "</foo>",
                        false);
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        public void IgnorePrologExceptDoctypeDoesntIgnorePIsOutsideOfProlog() {
            var differences =
                Compare("<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<?foo some PI ?>\n"
                        + "</foo>",
                        "<?xml version = \"1.0\" encoding = \"UTF-8\"?>"
                        + "<foo>"
                        + "<?foo some other PI ?>\n"
                        + "</foo>",
                        false);
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        [Ignore("doctype tests fail 'Document Type Declaration (DTD) is prohibited in this XML.'")]
        public void IgnorePrologExceptDoctypeDoesntIgnorePresenceOfDoctype() {
            var differences =
                Compare("<!DOCTYPE test ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>",
                        "<bar/>",
                        false);
            Assert.That(differences, Is.Not.Empty);
        }

        [Test]
        [Ignore("doctype tests fail 'Document Type Declaration (DTD) is prohibited in this XML.'")]
        public void IgnorePrologExceptDoctypeDoesntIgnoreNameOfDoctype() {
            var differences =
                Compare("<!DOCTYPE foo ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>",
                        "<!DOCTYPE test ["
                        + "<!ELEMENT bar EMPTY>"
                        + "]>"
                        + "<bar/>",
                        false);
            Assert.That(differences, Is.Not.Empty);
        }

        private IList<Comparison> Compare(string controlXml, string testXml) {
            return Compare(controlXml, testXml, true);
        }

        private IList<Comparison> Compare(string controlXml, string testXml,
                                          bool ignoreDoctypeDeclarationAsWell) {
            ISource control = InputBuilder.From(controlXml).Build();
            ISource test = InputBuilder.From(testXml).Build();
            DOMDifferenceEngine e = new DOMDifferenceEngine();
            if (ignoreDoctypeDeclarationAsWell) {
                e.DifferenceEvaluator = DifferenceEvaluators.IgnorePrologDifferences();
            } else {
                e.DifferenceEvaluator = DifferenceEvaluators.IgnorePrologDifferencesExceptDoctype();
            }
            var differences = new List<Comparison>();
            e.DifferenceListener += (comparison, outcome) => differences.Add(comparison);
            e.Compare(control, test);
            return differences;
        }
    }
}
