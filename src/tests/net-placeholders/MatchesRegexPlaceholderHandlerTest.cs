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
using Org.XmlUnit.Diff;
using Org.XmlUnit.Placeholder;

namespace Org.XmlUnit.Placeholder
{
    [TestFixture]
    public class MatchesRegexPlaceholderHandlerTest {

        private IPlaceholderHandler placeholderHandler = new MatchesRegexPlaceholderHandler();

        [Test]
        public void ShouldGetKeyword() {
            string expected = "matchesRegex";
            string keyword = placeholderHandler.Keyword;

            Assert.AreEqual(expected, keyword);
        }

        [Test]
        public void ShouldEvaluateGivenSimpleRegex() {
            string testTest = "1234";
            string regex = "^\\d+$";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }

        [Test]
        public void ShouldNotEvaluateGivenNull() {
            string testTest = null;
            string regex = "^\\d+$";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }

        [Test]
        public void ShouldNotEvaluateGivenEmptystring() {
            string testTest = "";
            string regex = "^\\d+$";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }

        [Test]
        public void ShouldNotEvaluatestringDoesNotMatchRegex() {
            string testTest = "not parsable as a number even though it contains 123 numbers";
            string regex = "^\\d+$";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }

        [Test]
        public void ShouldNotEvaluateWithNullRegex() {
            string testTest = "a string";
            string regex = null;
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }

        [Test]
        public void ShouldNotEvaluateWithEmptyRegex() {
            string testTest = "a string";
            string regex = "";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest, regex);

            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }
    }

}
