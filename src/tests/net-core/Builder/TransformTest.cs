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

using System.IO;
using System.Xml;
using NUnit.Framework;

namespace Org.XmlUnit.Builder {
    [TestFixture]
    public class TransformTest {
        [Test] public void TransformAnimalToString() {
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><dog />",
                            Transform
                            .Source(Input.FromFile(TestResources.DOG_FILE)
                                    .Build())
                            .WithStylesheet(Input.FromFile(TestResources.ANIMAL_XSL)
                                            .Build())
                            .Build()
                            .ToString()
                            .Replace("\n", string.Empty));
        }

        [Test] public void TransformAnimalToDocument() {
            XmlDocument doc = Transform
                .Source(Input.FromFile(TestResources.DOG_FILE).Build())
                .WithStylesheet(Input.FromFile(TestResources.ANIMAL_XSL)
                                .Build())
                .Build()
                .ToDocument();
            Assert.AreEqual("dog", doc.DocumentElement.Name);
        }

        [Test]
        public void TransformAnimalToTextWriter() {
            StringWriter sw = new StringWriter();
            Transform
                .Source(Input.FromFile(TestResources.DOG_FILE).Build())
                .WithStylesheet(Input.FromFile(TestResources.ANIMAL_XSL).Build())
                .Build()
                .To(sw);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><dog />",
                            sw.ToString().Replace("\n", string.Empty));
        }

        [Test]
        public void TransformAnimalToXmlWriter() {
            using (MemoryStream ms = new MemoryStream()) {
                using (XmlWriter w = XmlWriter.Create(ms)) {
                    Transform
                        .Source(Input.FromFile(TestResources.DOG_FILE).Build())
                        .WithStylesheet(Input.FromFile(TestResources.ANIMAL_XSL).Build())
                        .Build()
                        .To(w);
                }
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                Assert.AreEqual("dog", doc.DocumentElement.Name);
            }
        }

        [Test]
        public void TransformAnimalToStream() {
            using (MemoryStream ms = new MemoryStream()) {
                Transform
                    .Source(Input.FromFile(TestResources.DOG_FILE).Build())
                    .WithStylesheet(Input.FromFile(TestResources.ANIMAL_XSL).Build())
                    .Build()
                    .To(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                Assert.AreEqual("dog", doc.DocumentElement.Name);
            }
        }
    }
}

