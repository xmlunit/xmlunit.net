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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Org.XmlUnit.Placeholder
{
    [TestFixture()]
    public class IsDateTimePlaceholderHandlerTests
    {
        private IPlaceholderHandler placeholderHandler = new IsDateTimePlaceholderHandler();

        [Test]
        [TestCase("01-01-2020", TestName = "ShouldEvaluateDateWithDashes", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020", TestName = "ShouldEvaluateDateWithDots", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020", TestName = "ShouldEvaluateDateWithSlashes", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("2020-01-01T15:00", TestName = "ShouldEvaluateSortableDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("2020-01-01 15:00:00Z", TestName = "ShouldEvaluateUniversalDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020 15:00", TestName = "ShouldEvaluateDateWithTime", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase(null, TestName = "ShouldNotEvaluateNull", ExpectedResult = ComparisonResult.DIFFERENT)]
        [TestCase("", TestName = "ShouldNotEvaluateEmpty", ExpectedResult = ComparisonResult.DIFFERENT)]
        [TestCase("This is a test date 01/01/2020", TestName = "ShouldNotEvaluateStringContainingDate", ExpectedResult = ComparisonResult.DIFFERENT)]
        public ComparisonResult EvaluateTest(string testText)
        {
            return placeholderHandler.Evaluate(testText);
        }

        [Test]
        [TestCase("19/06/2020", "en-GB", TestName = "ShouldEvaluateUKDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("06/19/2020", "en-US", TestName = "ShouldEvaluateUSDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("19/06/2020", "fr-FR", TestName = "ShouldEvaluateFRDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("19/06/2020", "es-ES", TestName = "ShouldEvaluateESDate", ExpectedResult = ComparisonResult.EQUAL)]
        public ComparisonResult EvaluateOtherCulturesTest(string testText, string cultureCode)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
            return placeholderHandler.Evaluate(testText);
        }

        [Test]
        public void ShouldGetKeyword()
        {
            string expected = "isDateTime";
            string keyword = placeholderHandler.Keyword;

            Assert.AreEqual(expected, keyword);
        }

        [Test]
        public void ShouldParseExplicitPattern() {
            Assert.AreEqual(ComparisonResult.EQUAL,
                            placeholderHandler.Evaluate("31 01 2020 12:34", "dd MM yyyy HH:mm"));
            Assert.AreEqual(ComparisonResult.DIFFERENT,
                            placeholderHandler.Evaluate("abc", "dd MM yyyy HH:mm"));
        }

    }
}
