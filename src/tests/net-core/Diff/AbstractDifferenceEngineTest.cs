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
using NUnit.Framework;

namespace Org.XmlUnit.Diff {

    public abstract class AbstractDifferenceEngineTest {

        protected abstract AbstractDifferenceEngine DifferenceEngine {
            get;
        }

        private ComparisonResult outcome = ComparisonResult.SIMILAR;
        private ComparisonResult ResultGrabber(Comparison comparison,
                                               ComparisonResult outcome) {
            this.outcome = outcome;
            return outcome;
        }

        [Test]
        public void CompareTwoNulls() {
            AbstractDifferenceEngine d = DifferenceEngine;
            d.DifferenceEvaluator = ResultGrabber;
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null, null,
                                                     null, null, null)));
            Assert.AreEqual(ComparisonResult.EQUAL, outcome);
        }

        [Test]
        public void CompareControlNullTestNonNull() {
            AbstractDifferenceEngine d = DifferenceEngine;
            d.DifferenceEvaluator = ResultGrabber;
            Assert.AreEqual(Wrap(ComparisonResult.DIFFERENT),
                         d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                  null, null, null,
                                                  null, null, "")));
            Assert.AreEqual(ComparisonResult.DIFFERENT, outcome);
        }

        [Test]
        public void CompareControlNonNullTestNull() {
            AbstractDifferenceEngine d = DifferenceEngine;
            d.DifferenceEvaluator = ResultGrabber;
            Assert.AreEqual(Wrap(ComparisonResult.DIFFERENT),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null, "",
                                                     null, null, null)));
            Assert.AreEqual(ComparisonResult.DIFFERENT, outcome);
        }

        [Test]
        public void CompareTwoDifferentNonNulls() {
            AbstractDifferenceEngine d = DifferenceEngine;
            d.DifferenceEvaluator = ResultGrabber;
            Assert.AreEqual(Wrap(ComparisonResult.DIFFERENT),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("1"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(ComparisonResult.DIFFERENT, outcome);
        }

        [Test]
        public void CompareTwoEqualNonNulls() {
            AbstractDifferenceEngine d = DifferenceEngine;
            d.DifferenceEvaluator = ResultGrabber;
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("2"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(ComparisonResult.EQUAL, outcome);
        }

        [Test]
        public void CompareNotifiesComparisonListener() {
            AbstractDifferenceEngine d = DifferenceEngine;
            int invocations = 0;
            d.ComparisonListener += delegate(Comparison comp,
                                             ComparisonResult r) {
                invocations++;
                Assert.AreEqual(ComparisonResult.EQUAL, r);
            };
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("2"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(1, invocations);
        }

        [Test]
        public void CompareNotifiesMatchListener() {
            AbstractDifferenceEngine d = DifferenceEngine;
            int invocations = 0;
            d.MatchListener += delegate(Comparison comp,
                                             ComparisonResult r) {
                invocations++;
                Assert.AreEqual(ComparisonResult.EQUAL, r);
            };
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("2"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(1, invocations);
        }

        [Test]
        public void CompareNotifiesDifferenceListener() {
            AbstractDifferenceEngine d = DifferenceEngine;
            int invocations = 0;
            d.DifferenceListener += delegate(Comparison comp,
                                             ComparisonResult r) {
                invocations++;
                Assert.AreEqual(ComparisonResult.SIMILAR, r);
            };
            Assert.AreEqual(Wrap(ComparisonResult.SIMILAR),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("2"),
                                                     null, null,
                                                     Convert.ToInt16("3"))));
            Assert.AreEqual(1, invocations);
        }

        [Test]
        public void CompareUsesResultOfEvaluator() {
            AbstractDifferenceEngine d = DifferenceEngine;
            int invocations = 0;
            d.ComparisonListener += delegate(Comparison comp,
                                             ComparisonResult r) {
                invocations++;
                Assert.AreEqual(ComparisonResult.SIMILAR, r);
            };
            d.DifferenceEvaluator = delegate(Comparison comparison,
                                             ComparisonResult outcome) {
                return ComparisonResult.SIMILAR;
            };
            Assert.AreEqual(Wrap(ComparisonResult.SIMILAR),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("2"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(1, invocations);
        }

        [Test]
        public void CompareUsesResultOfController() {
            AbstractDifferenceEngine d = DifferenceEngine;
            int invocations = 0;
            d.ComparisonListener += delegate(Comparison comp,
                                             ComparisonResult r) {
                invocations++;
                Assert.AreEqual(ComparisonResult.SIMILAR, r);
            };
            d.ComparisonController = _ => true;
            Assert.AreEqual(WrapAndStop(ComparisonResult.SIMILAR),
                            d.Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                                     null, null,
                                                     Convert.ToInt16("1"),
                                                     null, null,
                                                     Convert.ToInt16("2"))));
            Assert.AreEqual(1, invocations);
        }
        [Test]
        public void OngoingComparisonStateBasics() {
            AbstractDifferenceEngine.ComparisonState cs = Wrap(ComparisonResult.EQUAL);
            Assert.AreEqual(cs, new AbstractDifferenceEngine.OngoingComparisonState(null));
        }

        [Test]
        public void AndThenUsesCurrentFinishedFlag() {
            AbstractDifferenceEngine.ComparisonState cs = WrapAndStop(ComparisonResult.SIMILAR);
            Assert.AreEqual(WrapAndStop(ComparisonResult.SIMILAR),
                            cs.AndThen(() => Wrap(ComparisonResult.EQUAL)));
            cs = Wrap(ComparisonResult.SIMILAR);
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            cs.AndThen(() => Wrap(ComparisonResult.EQUAL)));
        }

        [Test]
        public void AndIfTrueThenUsesCurrentFinishedFlag() {
            AbstractDifferenceEngine.ComparisonState cs = WrapAndStop(ComparisonResult.SIMILAR);
            Assert.AreEqual(WrapAndStop(ComparisonResult.SIMILAR),
                            cs.AndIfTrueThen(true, () => Wrap(ComparisonResult.EQUAL)));
            cs = Wrap(ComparisonResult.SIMILAR);
            Assert.AreEqual(Wrap(ComparisonResult.EQUAL),
                            cs.AndIfTrueThen(true, () => Wrap(ComparisonResult.EQUAL)));
        }

        [Test]
        public void AndIfTrueThenIsNoopIfFirstArgIsFalse() {
            AbstractDifferenceEngine.ComparisonState cs = WrapAndStop(ComparisonResult.SIMILAR);
            Assert.AreEqual(WrapAndStop(ComparisonResult.SIMILAR),
                            cs.AndIfTrueThen(false, () => Wrap(ComparisonResult.EQUAL)));
            cs = Wrap(ComparisonResult.SIMILAR);
            Assert.AreEqual(Wrap(ComparisonResult.SIMILAR),
                            cs.AndIfTrueThen(false, () => Wrap(ComparisonResult.EQUAL)));
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantSetNullNodeMatcher() {
            DifferenceEngine.NodeMatcher = null;
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantSetNullComparisonController() {
            DifferenceEngine.ComparisonController = null;
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantSetNullDifferenceEvaluator() {
            DifferenceEngine.DifferenceEvaluator = null;
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantSetNullAttributeFilter() {
            DifferenceEngine.AttributeFilter = null;
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void CantSetNullNodeFilter() {
            DifferenceEngine.NodeFilter = null;
        }

        [Test]
        public void ComparisonStateEqualsLooksAtType() {
            Assert.AreNotEqual(Wrap(ComparisonResult.SIMILAR), new MyOngoing());
        }

        [Test]
        public void ComparisonStateEqualsLooksAtResult() {
            Assert.AreNotEqual(Wrap(ComparisonResult.SIMILAR),
                               Wrap(ComparisonResult.DIFFERENT));
        }

        [Test]
        public void HashCodeLooksAtFinished() {
            Assert.AreNotEqual(Wrap(ComparisonResult.SIMILAR).GetHashCode(),
                               WrapAndStop(ComparisonResult.SIMILAR).GetHashCode());
        }

        [Test]
        public void TrivialComparisonStateToString() {
            string s = Wrap(ComparisonResult.SIMILAR).ToString();
            Assert.That(s, Is.StringContaining("OngoingComparisonState"));
            Assert.That(s, Is.StringContaining("SIMILAR"));
        }

        protected static AbstractDifferenceEngine.ComparisonState Wrap(ComparisonResult c) {
            return new AbstractDifferenceEngine.OngoingComparisonState(null, c);
        }

        protected static AbstractDifferenceEngine.ComparisonState WrapAndStop(ComparisonResult c) {
            return new AbstractDifferenceEngine.FinishedComparisonState(null, c);
        }

        internal class MyOngoing : AbstractDifferenceEngine.ComparisonState {
            internal MyOngoing() : base(null, false, ComparisonResult.SIMILAR) {
            }
        }
    }
}
