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
using System.Xml.Schema;
using NUnit.Framework;

namespace Org.XmlUnit.Validation {
    [TestFixture]
    public class ValidationProblemTest {
        [Test]
        public void TrivialToStringTest() {
            ValidationProblem p = new ValidationProblem("foo", 1, 2, XmlSeverityType.Error);
            String s = p.ToString();
            Assert.That(s, Is.StringContaining("line=1"));
            Assert.That(s, Is.StringContaining("column=2"));
            Assert.That(s, Is.StringContaining("type=Error"));
            Assert.That(s, Is.StringContaining("message='foo'"));
        }
    }
}
