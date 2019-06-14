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
using Org.XmlUnit;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {

    [TestFixture]
    public class IsNumberPlaceholderHandlerTest {
        private IPlaceholderHandler placeholderHandler = new IsNumberPlaceholderHandler();

        [Test]
        public void ShouldGetKeyword() {
            string expected = "isNumber";
            string keyword = placeholderHandler.Keyword;
    
            Assert.AreEqual(expected, keyword);
        }
    
        [Test]
        public void ShouldEvaluateGivenSimpleNumber() {
            string testTest = "1234";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldEvaluateGivenFloatingPointNumber() {
            string testTest = "12.34";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldEvaluateGivenNegativeNumber() {
            string testTest = "-1234";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldEvaluateGivenNegativeFloatingPointNumber() {
            string testTest = "-12.34";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldEvaluateGivenEngineeringNotationFloatingPointNumber() {
            string testTest = "1.7E+3";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldEvaluateGivenNegativeEngineeringNotationFloatingPointNumber() {
            string testTest = "-1.7E+3";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.EQUAL, comparisonResult);
        }
    
        [Test]
        public void ShouldNotEvaluateGivenNull() {
            string testTest = null;
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }
    
        [Test]
        public void ShouldNotEvaluateGivenEmptystring() {
            string testTest = "";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }
    
        [Test]
        public void ShouldNotEvaluateGivenNonNumberstring() {
            string testTest = "not parsable as a number even though it contains 123 numbers";
            ComparisonResult comparisonResult = placeholderHandler.Evaluate(testTest);
    
            Assert.AreEqual(ComparisonResult.DIFFERENT, comparisonResult);
        }

    }
}
