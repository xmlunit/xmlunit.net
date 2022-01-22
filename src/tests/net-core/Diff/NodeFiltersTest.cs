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
    public class NodeFiltersTest {

        internal class TestFilter {
            private readonly bool doReturn;
            internal bool called;

            internal TestFilter(bool ret) {
                doReturn = ret;
            }

            public bool Test(XmlNode n) {
                called = true;
                return doReturn;
            }
        }


        [Test]
        public void EmptySatisfiesAllReturnsTrue() {
            Assert.IsTrue(NodeFilters.SatifiesAll()(null));
        }

        [Test]
        public void EmptySatisfiesAnyReturnsFalse() {
            Assert.IsFalse(NodeFilters.SatifiesAny()(null));
        }

        [Test]
        public void SatisfiesAllWorksAsPromised() {
            TestFilter n1 = new TestFilter(true);
            TestFilter n2 = new TestFilter(false);
            TestFilter n3 = new TestFilter(true);
            Assert.IsTrue(NodeFilters.SatifiesAll(n1.Test)(null));
            Assert.IsFalse(NodeFilters.SatifiesAll(n1.Test, n2.Test, n3.Test)(null));
            Assert.IsFalse(n3.called);
            Assert.IsTrue(NodeFilters.SatifiesAll(n1.Test, n3.Test)(null));
        }

        [Test]
        public void SatisfiesAnyWorksAsPromised() {
            TestFilter n1 = new TestFilter(false);
            TestFilter n2 = new TestFilter(true);
            TestFilter n3 = new TestFilter(false);
            Assert.IsFalse(NodeFilters.SatifiesAny(n1.Test)(null));
            Assert.IsTrue(NodeFilters.SatifiesAny(n1.Test, n2.Test, n3.Test)(null));
            Assert.IsFalse(n3.called);
            Assert.IsFalse(NodeFilters.SatifiesAny(n1.Test, n3.Test)(null));
        }
    }
}
