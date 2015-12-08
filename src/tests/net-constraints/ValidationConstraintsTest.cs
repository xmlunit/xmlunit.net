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
using Org.XmlUnit.Input;

namespace Org.XmlUnit.Constraints {
    [TestFixture]
    public class ValidationConstraintTest {
        [Test]
        public void ShouldSuccessfullyValidateInstance() {
            Assert.That(new StreamSource(TestResources.TESTS_DIR
                                         + "BookXsdGeneratedNoSchema.xml"),
                        new SchemaValidConstraint(
                            new StreamSource(TestResources.TESTS_DIR + "Book.xsd")));
        }

        [Test]
        public void ShouldFailOnBrokenSchema() {
            Assert.That(new StreamSource(TestResources.TESTS_DIR
                                         + "invalidBook.xml"),
                        !new SchemaValidConstraint(new StreamSource(TestResources
                                                                    .TESTS_DIR +
                                                                    "Book.xsd")));
        }

        [Test]
        public void ShouldThrowOnBrokenInstance() {
            Assert.Throws<AssertionException>(() =>
            Assert.That(new StreamSource(TestResources.TESTS_DIR
                                         + "invalidBook.xml"),
                        new SchemaValidConstraint(new StreamSource(TestResources
                                                                   .TESTS_DIR +
                                                                   "Book.xsd"))));
        }

        [Test][Ignore("Validator doesn't seem to like http URIs - at least in AppVeyor")]
        public void ShouldSuccessfullyValidateInstanceWithoutExplicitSchemaSource() {
            Assert.That(new StreamSource(TestResources.TESTS_DIR
                                         + "BookXsdGenerated.xml"),
                        new SchemaValidConstraint());
        }

        // throws for the wrong reason, but throws :-)
        [Test]
        public void ShouldThrowOnBrokenInstanceWithoutExplicitSchemaSource() {
            Assert.Throws<AssertionException>(() =>
            Assert.That(new StreamSource(TestResources.TESTS_DIR
                                         + "invalidBook.xml"),
                        new SchemaValidConstraint()));
        }

        [Test]
        public void ShouldThrowWhenSchemaSourcesContainsNull() {
            Assert.Throws<ArgumentException>(() =>
            new SchemaValidConstraint(new object[] { null }));
        }

        [Test]
        public void ShouldThrowWhenSchemaSourcesIsNull() {
            Assert.Throws<ArgumentNullException>(() =>
            new SchemaValidConstraint(null));
        }
    }
}
