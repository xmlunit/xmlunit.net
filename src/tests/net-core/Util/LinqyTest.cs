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

namespace Org.XmlUnit.Util {
    [TestFixture]
    public class LinqyTest {

        [Test]
        public void FirstOrDefaultShouldReturnFirstReferenceTypeVersion() {
            IList<object> os = new List<object> { new object(), new object() };
            object f = Linqy.FirstOrDefault(os, o => o != null, null);
            Assert.AreSame(os[0], f);
        }

        [Test]
        public void FirstOrDefaultShouldReturnFirstValueTypeVersion() {
            IList<int> os = new List<int> { 1, 2 };
            int f = Linqy.FirstOrDefaultValue(os, i => i > 1, 42);
            Assert.AreEqual(2, f);
        }

        [Test]
        public void FirstOrDefaultShouldReturnDefaultIfNothingMatchesReferenceTypeVersion() {
            IList<object> os = new List<object> { new object(), new object() };
            object def = new object();
            object f = Linqy.FirstOrDefault(os, o => o == null, def);
            Assert.AreSame(def, f);
        }

        [Test]
        public void FirstOrDefaultShouldReturnDefaultIfNothingMatchesValueTypeVersion() {
            IList<int> os = new List<int> { 1, 2 };
            int f = Linqy.FirstOrDefaultValue(os, i => i < 1, 42);
            Assert.AreEqual(42, f);
        }
    }
}
