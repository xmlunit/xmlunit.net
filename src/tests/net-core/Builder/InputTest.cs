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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Org.XmlUnit.Xpath;

namespace Org.XmlUnit.Builder {

    [TestFixture]
    public class InputTest {

        private static XmlDocument Parse(ISource s) {
            XmlDocument d = new XmlDocument();
            d.Load(s.Reader);
            return d;
        }

        private static XDocument XParse(ISource s) {
            return XDocument.Load(s.Reader);
        }

        [Test] public void ShouldParseADocument() {
            XmlDocument d = Parse(Input.FromFile(TestResources.ANIMAL_FILE)
                                  .Build());
            AllIsWellFor(Input.FromDocument(d).Build());
        }

        [Test] public void ShouldParseAnXDocument() {
            XDocument d = XParse(Input.FromFile(TestResources.ANIMAL_FILE)
                                 .Build());
            AllIsWellFor(Input.FromDocument(d).Build());
        }

        [Test] public void ShouldParseAnExistingFileByName() {
            ISource s = Input.FromFile(TestResources.ANIMAL_FILE).Build();
            AllIsWellFor(s);
            Assert.AreEqual(ToFileUri(TestResources.ANIMAL_FILE), s.SystemId);
        }

        [Test] public void ShouldParseAnExistingFileFromStream() {
            using (FileStream fs = new FileStream(TestResources.ANIMAL_FILE,
                                                  FileMode.Open,
                                                  FileAccess.Read)) {
                ISource s = Input.FromStream(fs).Build();
                AllIsWellFor(s);
                Assert.AreEqual(ToFileUri(TestResources.ANIMAL_FILE),
                                s.SystemId);
            }
        }

        [Test] public void ShouldParseAnExistingFileFromReader() {
            using (StreamReader r = new StreamReader(TestResources.ANIMAL_FILE)) {
                ISource s = Input.FromReader(r).Build();
                AllIsWellFor(s);
                Assert.AreEqual(ToFileUri(TestResources.ANIMAL_FILE),
                                s.SystemId);
            }
        }

        [Test] public void ShouldParseString() {
            AllIsWellFor(Input.FromString(Encoding.UTF8.GetString(ReadTestFile()))
                         .Build());
        }

        [Test] public void ShouldParseBytes() {
            AllIsWellFor(Input.FromByteArray(ReadTestFile()).Build());
        }

        [Test] public void ShouldParseFileFromURIString() {
            ISource s = Input.FromURI(ToFileUri(TestResources.ANIMAL_FILE))
                .Build();
            AllIsWellFor(s);
            Assert.AreEqual(ToFileUri(TestResources.ANIMAL_FILE),
                            s.SystemId);
        }

        [Test] public void ShouldParseFileFromURI() {
            ISource s = Input.FromURI(new Uri(ToFileUri(TestResources.ANIMAL_FILE)))
                .Build();
            AllIsWellFor(s);
            Assert.AreEqual(ToFileUri(TestResources.ANIMAL_FILE),
                            s.SystemId);
        }

        [Test] public void ShouldParseATransformationFromSource() {
            ISource input = Input.FromString("<animal>furry</animal>").Build();
            ISource s = Input.ByTransforming(input)
                .WithStylesheet(Input.FromFile(TestResources.TESTS_DIR + "animal.xsl")
                                .Build())
                .Build();
            Assert.That(s, Is.Not.Null);
            XmlDocument d = Parse(s);
            Assert.That(d, Is.Not.Null);
            Assert.That(d.DocumentElement.Name, Is.EqualTo("furry"));
        }

        [Test] public void ShouldParseATransformationFromBuilder() {
            Input.IBuilder input = Input.FromString("<animal>furry</animal>");
            ISource s = Input.ByTransforming(input)
                .WithStylesheet(Input.FromFile(TestResources.TESTS_DIR + "animal.xsl"))
                .Build();
            Assert.That(s, Is.Not.Null);
            XmlDocument d = Parse(s);
            Assert.That(d, Is.Not.Null);
            Assert.That(d.DocumentElement.Name, Is.EqualTo("furry"));
        }

        [Test] public void ShouldParseUnknownToSource() {
            // from ISource
            AllIsWellFor(Input.From(Input.FromByteArray(ReadTestFile()).Build()).Build());
            // from IBuilder
            AllIsWellFor(Input.From(Input.FromByteArray(ReadTestFile())).Build());
            // From XmlDocument
            AllIsWellFor(Input.From(Parse(Input.FromFile(TestResources.ANIMAL_FILE).Build()))
                         .Build());
            // From XDocument
            AllIsWellFor(Input.From(XParse(Input.FromFile(TestResources.ANIMAL_FILE).Build()))
                         .Build());
            // From string
            AllIsWellFor(Input.From(Encoding.UTF8.GetString(ReadTestFile())).Build());
            // From byte[]
            AllIsWellFor(Input.From(ReadTestFile()).Build());
            // From Uri
            AllIsWellFor(Input.From(new Uri(ToFileUri(TestResources.ANIMAL_FILE))).Build());
            // From Stream
            using (FileStream fs = new FileStream(TestResources.ANIMAL_FILE,
                                                  FileMode.Open,
                                                  FileAccess.Read)) {
                AllIsWellFor(Input.From(fs).Build());
            }
            // From Reader
            using (StreamReader r = new StreamReader(TestResources.ANIMAL_FILE)) {
                AllIsWellFor(Input.From(r).Build());
            }
            Assert.That(Input.From(new XmlDocument().CreateElement("foo")).Build(),
                        Is.Not.Null);
        }

        [Test]
        public void CanCreateInputFromNode() {
            Assert.That(Input.FromNode(new XmlDocument().CreateElement("foo")).Build(),
                        Is.Not.Null);
        }

        // see "https://github.com/xmlunit/xmlunit/issues/84"
        [Test]
        public void TestStringSourceCanBeUsedMoreThanOnce() {
            ISource xml = Input.FromString("<a><b>bvalue</b><c>cvalue</c></a>").Build();

            XPathEngine xpath = new XPathEngine();

            Assert.That(xpath.Evaluate("//a/b", xml), Is.EqualTo("bvalue"));
            Assert.That(xpath.Evaluate("//a/c", xml), Is.EqualTo("cvalue"));
        }

        private static void AllIsWellFor(ISource s) {
            Assert.That(s, Is.Not.Null);
            XmlDocument d = Parse(s);
            Assert.That(d, Is.Not.Null);
            Assert.That(d.DocumentElement.Name, Is.EqualTo("animal"));
        }

        private static byte[] ReadTestFile() {
            using (FileStream fs = new FileStream(TestResources.ANIMAL_FILE, FileMode.Open,
                                                  FileAccess.Read))
            using (MemoryStream ms = new MemoryStream()) {
                byte[] buffer = new byte[1024];
                int read = -1;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static string ToFileUri(string path) {
            return new Uri(Path.GetFullPath(path)).ToString();
        }
    }
}
