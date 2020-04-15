using NUnit.Framework;
using Org.XmlUnit.Diff;
using Org.XmlUnit.Placeholder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.XmlUnit.Placeholder
{
    [TestFixture()]
    public class IsDatePlaceholderHandlerTests
    {
        private IPlaceholderHandler placeholderHandler = new IsDatePlaceholderHandler();

        [Test()]
        [TestCase("01-01-2020", TestName = "ShouldEvaluateDateWithDashes", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020", TestName = "ShouldEvaluateDateWithDots", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020", TestName = "ShouldEvaluateDateWithSlashes", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("2020-01-01T15:00", TestName = "ShouldEvaluateSortableDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("2020-01-01 15:00:00Z", TestName = "ShouldEvaluateUniversalDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("19/06/2020", TestName = "ShouldEvaluateUKDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("06/19/2020", TestName = "ShouldEvaluateUSDate", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase("01/01/2020 15:00", TestName = "ShouldEvaluateDateWithTime", ExpectedResult = ComparisonResult.EQUAL)]
        [TestCase(null, TestName = "ShouldNotEvaluateNull", ExpectedResult = ComparisonResult.DIFFERENT)]
        [TestCase("", TestName = "ShouldNotEvaluateEmpty", ExpectedResult = ComparisonResult.DIFFERENT)]
        [TestCase("This is a test date 01/01/2020", TestName = "ShouldNotEvaluateStringContainingDate", ExpectedResult = ComparisonResult.DIFFERENT)]
        public ComparisonResult EvaluateTest(string testText)
        {
            return placeholderHandler.Evaluate(testText);
        }

        [Test]
        public void ShouldGetKeyword()
        {
            string expected = "isDate";
            string keyword = placeholderHandler.Keyword;

            Assert.AreEqual(expected, keyword);
        }
    }
}