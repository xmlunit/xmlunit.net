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
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {

    [TestFixture]
    public class PlaceholderSupportTest {

        [Test]
        public void IgnoreWithDefaultDelimiter() {
            string control = "<elem1><elem11>${xmlunit.ignore}</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control).WithTest(test)
                .WithPlaceholderSupport().Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void IgnoreWithCustomDelimters() {
            string control = "<elem1><elem11>#[xmlunit.ignore]</elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control)
                .WithPlaceholderSupportUsingDelimiters("#\\[", "\\]")
                .WithTest(test).Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void IgnoreChainedWithDefaultDelimters() {
            string control = "<elem1><elem11><![CDATA[${xmlunit.ignore}]]></elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control)
                .WithPlaceholderSupportChainedAfter(DifferenceEvaluators.Default)
                .WithTest(test).Build();

            Assert.IsFalse(diff.HasDifferences());
        }

        [Test]
        public void IgnoreChainedWithCustomDelimters() {
            string control = "<elem1><elem11><![CDATA[_xmlunit.ignore_]]></elem11></elem1>";
            string test = "<elem1><elem11>abc</elem11></elem1>";
            var diff = DiffBuilder.Compare(control)
                .WithPlaceholderSupportUsingDelimitersChainedAfter("_", "_", DifferenceEvaluators.Default)
                .WithTest(test).Build();
            Assert.IsFalse(diff.HasDifferences());
        }
    }
}
