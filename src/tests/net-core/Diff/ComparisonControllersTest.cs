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

using NUnit.Framework;

namespace Org.XmlUnit.Diff {

    [TestFixture]
    public class ComparisonControllersTest {

        [Test]
        public void TestDefault() {
            Difference equals = new Difference(null, ComparisonResult.EQUAL);
            Difference similar = new Difference(null, ComparisonResult.SIMILAR);
            Difference difference = new Difference(null, ComparisonResult.DIFFERENT);

            Assert.That(ComparisonControllers.Default(equals), Is.EqualTo(false));
            Assert.That(ComparisonControllers.Default(similar), Is.EqualTo(false));
            Assert.That(ComparisonControllers.Default(difference), Is.EqualTo(false));
        }

        [Test]
        public void TestStopWhenDifferent() {
            Difference equals = new Difference(null, ComparisonResult.EQUAL);
            Difference similar = new Difference(null, ComparisonResult.SIMILAR);
            Difference difference = new Difference(null, ComparisonResult.DIFFERENT);

            Assert.That(ComparisonControllers.StopWhenDifferent(equals), Is.EqualTo(false));
            Assert.That(ComparisonControllers.StopWhenDifferent(similar), Is.EqualTo(false));
            Assert.That(ComparisonControllers.StopWhenDifferent(difference), Is.EqualTo(true));
        }

        [Test]
        public void TestStopWhenSimilar() {
            Difference equals = new Difference(null, ComparisonResult.EQUAL);
            Difference similar = new Difference(null, ComparisonResult.SIMILAR);
            Difference difference = new Difference(null, ComparisonResult.DIFFERENT);

            Assert.That(ComparisonControllers.StopWhenSimilar(equals), Is.EqualTo(false));
            Assert.That(ComparisonControllers.StopWhenSimilar(similar), Is.EqualTo(true));
            Assert.That(ComparisonControllers.StopWhenSimilar(difference), Is.EqualTo(true));
        }
    }
}
