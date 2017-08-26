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
using System.Xml;
using NUnit.Framework;

namespace Org.XmlUnit.Input {
    [TestFixture]
    public class CommentLessSourceTest {

        [TestCase(null)]
        [TestCase("1.0")]
        [TestCase("2.0")]
        public void StripCommentsAtDifferentLevels(string xsltVersion) {
            CommentLessSource cls = GetSource("<?xml version='1.0'?>"
                                              + "<!-- comment 1 -->"
                                              + "<foo>"
                                              + "<!-- comment 2 -->"
                                              + "</foo>", xsltVersion);
            XmlDocument d = Org.XmlUnit.Util.Convert.ToDocument(cls);
            Assert.AreEqual(2, d.ChildNodes.Count);
            Assert.IsTrue(d.ChildNodes[0] is XmlDeclaration);
            Assert.IsTrue(d.ChildNodes[1] is XmlElement);
            Assert.AreEqual(0, d.ChildNodes[1].ChildNodes.Count);
        }

        [Test]
        public void CantWrapNullSource() {
            Assert.Throws<ArgumentNullException>(() =>
            new CommentLessSource(null));
        }

        [Test]
        public void CantUseNullVersion() {
            Assert.Throws<ArgumentNullException>(() =>
            new CommentLessSource(new StreamSource(new StringReader("foo")), null));
        }

        private CommentLessSource GetSource(string s, string xsltVersion) {
            StreamSource src = s == null ? null : new StreamSource(new StringReader(s));
            return xsltVersion == null ? new CommentLessSource(src)
                : new CommentLessSource(src, xsltVersion);
        }
    }
}

