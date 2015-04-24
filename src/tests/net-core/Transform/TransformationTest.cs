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
using InputBuilder = Org.XmlUnit.Builder.Input;
using NUnit.Framework;

namespace Org.XmlUnit.Transform {
    [TestFixture]
    public class TransformationTest {
        private Transformation t;

        [SetUp]
        public void CreateTransformation() {
            t = new Transformation(InputBuilder.FromFile(TestResources.DOG_FILE)
                                   .Build());
            t.Stylesheet = InputBuilder.FromFile(TestResources.ANIMAL_XSL).Build();
        }

        [Test]
        public void TransformAnimalToString() {
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><dog />",
                            t.TransformToString().Replace("\n", string.Empty));
        }

        [Test]
        public void TransformAnimalToDocument() {
            XmlDocument doc = t.TransformToDocument();
            Assert.AreEqual("dog", doc.DocumentElement.Name);
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRejectNullSourceInSetSource() {
            Transformation t = new Transformation();
            t.Source = null;
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRejectNullExtensionObjectUri() {
            t.AddExtensionObject(null, "foo");
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRejectNullParameterName() {
            t.AddParameter(null, "foo", "bar");
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRejectNullParameterUri() {
            t.AddParameter("foo", null, "bar");
        }

        [Test][ExpectedException(typeof(InvalidOperationException))]
        public void ShouldRejectNullSourceInTransform() {
            Transformation t = new Transformation();
            t.TransformToString();
        }

        [Test][ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRejectNullTransformer() {
            t.Transform(null);
        }

        [Test][ExpectedException(typeof(XMLUnitException))]
        public void ShouldMapTransformerException() {
            t.Transform((ct, r, args) => {
                    throw new NullReferenceException();
                });
        }

        [Test]
        public void PassesThroughExtensionObjects() {
            t.AddExtensionObject("foo", "bar");
            t.Transform((ct, r, args) => {
                    Assert.AreEqual("bar", args.GetExtensionObject("foo"));
                });
        }

        [Test]
        public void ClearsExtensionObjects() {
            t.AddExtensionObject("foo", "bar");
            t.Clear();
            t.Transform((ct, r, args) => {
                    Assert.IsNull(args.GetExtensionObject("foo"));
                });
        }

        [Test]
        public void PassesThroughParameters() {
            t.AddParameter("foo", "bar", "baz");
            t.Transform((ct, r, args) => {
                    Assert.AreEqual("baz", args.GetParam("foo", "bar"));
                });
        }

        [Test]
        public void ClearsParameters() {
            t.AddParameter("foo", "bar", "baz");
            t.Clear();
            t.Transform((ct, r, args) => {
                    Assert.IsNull(args.GetParam("foo", "bar"));
                });
        }

    }
}
