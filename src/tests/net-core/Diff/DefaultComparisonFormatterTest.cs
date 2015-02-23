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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Util;
using NUnit.Framework;

namespace Org.XmlUnit.Diff {
    [TestFixture]
    public class DefaultComparisonFormatterTest {

        private DefaultComparisonFormatter compFormatter = new DefaultComparisonFormatter();

#if false
        [Test]
        public void TestComparisonType_XML_VERSION() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<?xml version=\"1.0\"?><a/>").WithTest("<?xml version=\"1.1\"?><a/>").Build();
            AssertPreRequirements(diff, ComparisonType.XML_VERSION);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected xml version '1.0' but was '1.1' - "
                            + "comparing <a...> at / to <?xml version=\"1.1\"?><a...> at /", description);

            AssertEquals("<a>\n</a>", controlDetails);
            AssertEquals("<?xml version=\"1.1\"?>\n<a>\n</a>", testDetails);
        }
#endif

        [Test]
        public void TestComparisonType_XML_STANDALONE() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<?xml version=\"1.0\" standalone=\"yes\"?><a b=\"x\"><b/></a>")
                .WithTest("<?xml version=\"1.0\" standalone=\"no\"?><a b=\"x\"><b/></a>")
                .Build();
            AssertPreRequirements(diff, ComparisonType.XML_STANDALONE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected xml standalone 'yes' but was 'no' - "
                            + "comparing <?xml version=\"1.0\" standalone=\"yes\"?><a...> at / to <?xml version=\"1.0\" standalone=\"no\"?><a...> at /", description);

            AssertEquals("<?xml version=\"1.0\" standalone=\"yes\"?>", controlDetails);
            AssertEquals("<?xml version=\"1.0\" standalone=\"no\"?>", testDetails);
        }

        [Test]
        public void TestComparisonType_XML_ENCODING() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<?xml version=\"1.0\" encoding=\"UTF-8\"?><a/>")
                .WithTest("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><a/>").Build();
            AssertPreRequirements(diff, ComparisonType.XML_ENCODING);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected xml encoding 'UTF-8' but was 'ISO-8859-1' - "
                            + "comparing <?xml version=\"1.0\" encoding=\"UTF-8\"?><a...> at / "
                            + "to <?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><a...> at /", description);

            AssertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", controlDetails);
            AssertEquals("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>", testDetails);
        }

#if false
        [Test]
        public void TestComparisonType_HAS_DOCTYPE_DECLARATION() {
            // prepare data
            XmlDocument controlDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString("<!DOCTYPE Book><a/>").Build());

            Diff diff = DiffBuilder.Compare(controlDoc).WithTest("<a/>").Build();
            AssertPreRequirements(diff, ComparisonType.HAS_DOCTYPE_DECLARATION);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected has doctype declaration 'true' but was 'false' - "
                            + "comparing <!DOCTYPE Book><a...> at / to <a...> at /", description);

            AssertEquals("<!DOCTYPE Book>\n<a>\n</a>", controlDetails);
            AssertEquals("<a>\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_DOCTYPE_NAME() {
            // prepare data
            XmlDocument controlDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString("<!DOCTYPE Book ><a/>").Build());
            XmlDocument testDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString("<!DOCTYPE XY ><a/>").Build());

            Diff diff = DiffBuilder.Compare(controlDoc).WithTest(testDoc).Build();
            AssertPreRequirements(diff, ComparisonType.DOCTYPE_NAME);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected doctype name 'Book' but was 'XY' - "
                            + "comparing <!DOCTYPE Book><a...> at / to <!DOCTYPE XY><a...> at /", description);

            AssertEquals("<!DOCTYPE Book>\n<a>\n</a>", controlDetails);
            AssertEquals("<!DOCTYPE XY>\n<a>\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_DOCTYPE_PUBLIC_ID() {
            // prepare data
            XmlDocument controlDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString(
                "<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\"><a/>").Build());
            XmlDocument testDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString(
                "<!DOCTYPE Book SYSTEM \"http://example.org/nonsense\"><a/>").Build());

            Diff diff = DiffBuilder.Compare(controlDoc).WithTest(testDoc).Build();
            AssertPreRequirements(diff, ComparisonType.DOCTYPE_PUBLIC_ID);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected doctype public id 'XMLUNIT/TEST/PUB' but was 'null' - "
                            + "comparing <!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\"><a...> at / "
                            + "to <!DOCTYPE Book SYSTEM \"http://example.org/nonsense\"><a...> at /", description);

            AssertEquals("<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\">\n<a>\n</a>", controlDetails);
            AssertEquals("<!DOCTYPE Book SYSTEM \"http://example.org/nonsense\">\n<a>\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_DOCTYPE_SYSTEM_ID() {
            // prepare data
            XmlDocument controlDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString(
                "<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\"><a/>").Build());
            XmlDocument testDoc = Convert.ToDocument(Org.XmlUnit.Builder.Input.FromString(
                "<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/404\"><a/>").Build());

            Diff diff = DiffBuilder.Compare(controlDoc).WithTest(testDoc).Build();
            AssertPreRequirements(diff, ComparisonType.DOCTYPE_SYSTEM_ID);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual(
                            "Expected doctype system id 'http://example.org/nonsense' but was 'http://example.org/404' - "
                            + "comparing <!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\"><a...> "
                            + "to <!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/404\"><a...>", description);

            AssertEquals("<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/nonsense\">\n<a>\n</a>", controlDetails);
                            AssertEquals("<!DOCTYPE Book PUBLIC \"XMLUNIT/TEST/PUB\" \"http://example.org/404\">\n<a>\n</a>", testDetails);
        }
#endif

        [Test]
        public void TestComparisonType_SCHEMA_LOCATION() {
            // prepare data
            Diff diff = DiffBuilder
                .Compare("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                         + "xsi:schemaLocation=\"http://www.publishing.org Book.xsd\"/>")
                .WithTest("<a />").Build();
            AssertPreRequirements(diff, ComparisonType.SCHEMA_LOCATION);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected schema location 'http://www.publishing.org Book.xsd' but was '' - "
                    + "comparing <a...> at /a[1] to <a...> at /a[1]", description);

            AssertEquals("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                            + "xsi:schemaLocation=\"http://www.publishing.org Book.xsd\" />", controlDetails);
            AssertEquals("<a />", testDetails);
        }

        [Test]
        public void TestComparisonType_NO_NAMESPACE_SCHEMA_LOCATION() {
            // prepare data
            Diff diff = DiffBuilder.Compare(
                                            "<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                                            + "xsi:noNamespaceSchemaLocation=\"Book.xsd\"/>")
                .WithTest("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                          + "xsi:noNamespaceSchemaLocation=\"Telephone.xsd\"/>")
                .Build();
            AssertPreRequirements(diff, ComparisonType.NO_NAMESPACE_SCHEMA_LOCATION);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected no namespace schema location 'Book.xsd' but was 'Telephone.xsd' - "
                    + "comparing <a...> at /a[1] to <a...> at /a[1]", description);

            AssertEquals("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                                + "xsi:noNamespaceSchemaLocation=\"Book.xsd\" />", controlDetails);
            AssertEquals("<a xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" "
                                + "xsi:noNamespaceSchemaLocation=\"Telephone.xsd\" />", testDetails);
        }

        [Test]
        public void TestComparisonType_NODE_TYPE_similar() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a>Text</a>").WithTest("<a><![CDATA[Text]]></a>").Build();
            AssertPreRequirements(diff, ComparisonType.NODE_TYPE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected node type 'Text' but was 'CDATA Section' - "
                            + "comparing <a ...>Text</a> at /a[1]/text()[1] "
                            + "to <a ...><![CDATA[Text]]></a> at /a[1]/text()[1]",
                            description);

            AssertEquals("<a>Text</a>", controlDetails);
            AssertEquals("<a><![CDATA[Text]]></a>", testDetails);
        }

        [Test]
        public void TestComparisonType_NAMESPACE_PREFIX() {
            // prepare data
            Diff diff = DiffBuilder.Compare(
                                            "<ns1:a xmlns:ns1=\"test\">Text</ns1:a>").WithTest("<test:a xmlns:test=\"test\">Text</test:a>").Build();
            AssertPreRequirements(diff, ComparisonType.NAMESPACE_PREFIX);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected namespace prefix 'ns1' but was 'test' - "
                            + "comparing <ns1:a...> at /a[1] to <test:a...> at /a[1]", description);

            AssertEquals("<ns1:a xmlns:ns1=\"test\">Text</ns1:a>", controlDetails);
            AssertEquals("<test:a xmlns:test=\"test\">Text</test:a>", testDetails);
        }

        [Test]
        public void TestComparisonType_NAMESPACE_URI() {
            // prepare data
            Diff diff = DiffBuilder.Compare(
                                            "<test:a xmlns:test=\"test.org\">Text</test:a>")
            .WithTest("<test:a xmlns:test=\"test.net\">Text</test:a>").Build();
            AssertPreRequirements(diff, ComparisonType.NAMESPACE_URI);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected namespace uri 'test.org' but was 'test.net' - "
                            + "comparing <test:a...> at /a[1] to <test:a...> at /a[1]", description);

            AssertEquals("<test:a xmlns:test=\"test.org\">Text</test:a>", controlDetails);
            AssertEquals("<test:a xmlns:test=\"test.net\">Text</test:a>", testDetails);
        }

        [Test]
        public void TestComparisonType_TEXT_VALUE() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a>Text one</a>").WithTest("<a>Text two</a>").Build();
            AssertPreRequirements(diff, ComparisonType.TEXT_VALUE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected text value 'Text one' but was 'Text two' - "
                            + "comparing <a ...>Text one</a> at /a[1]/text()[1] "
                            + "to <a ...>Text two</a> at /a[1]/text()[1]", description);

            AssertEquals("<a>Text one</a>", controlDetails);
            AssertEquals("<a>Text two</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_PROCESSING_INSTRUCTION_TARGET() {
            // prepare data
            Diff diff = DiffBuilder.Compare(
                "<?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?><a>Text one</a>")
                .WithTest("<?xml-xy type=\"text/xsl\" href=\"animal.xsl\" ?><a>Text one</a>").Build();
            AssertPreRequirements(diff, ComparisonType.PROCESSING_INSTRUCTION_TARGET);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected processing instruction target 'xml-stylesheet' but was 'xml-xy' - "
                            + "comparing <?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?> at /processing-instruction()[1] "
                            + "to <?xml-xy type=\"text/xsl\" href=\"animal.xsl\" ?> at /processing-instruction()[1]", description);

            AssertEquals("<?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?>", controlDetails);
            AssertEquals("<?xml-xy type=\"text/xsl\" href=\"animal.xsl\" ?>", testDetails);
        }

        [Test]
        public void TestComparisonType_PROCESSING_INSTRUCTION_DATA() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?><a>Text one</a>")
                .WithTest("<?xml-stylesheet type=\"text/xsl\" href=\"animal.css\" ?><a>Text one</a>")
                .Build();
            AssertPreRequirements(diff, ComparisonType.PROCESSING_INSTRUCTION_DATA);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected processing instruction data 'type=\"text/xsl\" href=\"animal.xsl\" ' "
                            + "but was 'type=\"text/xsl\" href=\"animal.css\" ' - "
                            + "comparing <?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?> at /processing-instruction()[1] "
                            + "to <?xml-stylesheet type=\"text/xsl\" href=\"animal.css\" ?> at /processing-instruction()[1]", description);

            AssertEquals("<?xml-stylesheet type=\"text/xsl\" href=\"animal.xsl\" ?>", controlDetails);
            AssertEquals("<?xml-stylesheet type=\"text/xsl\" href=\"animal.css\" ?>", testDetails);
        }

        [Test]
        public void TestComparisonType_ELEMENT_TAG_NAME() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a></a>").WithTest("<b></b>").Build();
            AssertPreRequirements(diff, ComparisonType.ELEMENT_TAG_NAME);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected element tag name 'a' but was 'b' - "
                            + "comparing <a...> at /a[1] to <b...> at /b[1]", description);

            AssertEquals("<a>\n</a>", controlDetails);
            AssertEquals("<b>\n</b>", testDetails);
        }

        [Test]
        public void TestComparisonType_ELEMENT_NUM_ATTRIBUTES() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a b=\"xxx\"></a>")
                .WithTest("<a b=\"xxx\" c=\"xxx\"></a>").Build();
            AssertPreRequirements(diff, ComparisonType.ELEMENT_NUM_ATTRIBUTES);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected number of attributes '1' but was '2' - "
                            + "comparing <a...> at /a[1] to <a...> at /a[1]", description);

            AssertEquals("<a b=\"xxx\">\n</a>", controlDetails);
            AssertEquals("<a b=\"xxx\" c=\"xxx\">\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_ATTR_VALUE() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a b=\"xxx\"></a>").WithTest("<a b=\"yyy\"></a>").Build();
            AssertPreRequirements(diff, ComparisonType.ATTR_VALUE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected attribute value 'xxx' but was 'yyy' - "
                            + "comparing <a b=\"xxx\"...> at /a[1]/@b to <a b=\"yyy\"...> at /a[1]/@b", description);

            AssertEquals("<a b=\"xxx\">\n</a>", controlDetails);
            AssertEquals("<a b=\"yyy\">\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_CHILD_NODELIST_LENGTH() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a><b/></a>").WithTest("<a><b/><c/></a>").Build();
            AssertPreRequirements(diff, ComparisonType.CHILD_NODELIST_LENGTH);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected child nodelist length '1' but was '2' - "
                            + "comparing <a...> at /a[1] to <a...> at /a[1]", description);

            AssertEquals("<a>\n  <b />\n</a>", controlDetails);
            AssertEquals("<a>\n  <b />\n  <c />\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_CHILD_NODELIST_SEQUENCE() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a><b>XXX</b><b>YYY</b></a>").WithTest("<a><b>YYY</b><b>XXX</b></a>")
                .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndText))
                .Build();
            AssertPreRequirements(diff, ComparisonType.CHILD_NODELIST_SEQUENCE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected child nodelist sequence '0' but was '1' - "
                            + "comparing <b...> at /a[1]/b[1] to <b...> at /a[1]/b[2]", description);

            AssertEquals("<a>\n  <b>XXX</b>\n  <b>YYY</b>\n</a>", controlDetails);
            AssertEquals("<a>\n  <b>YYY</b>\n  <b>XXX</b>\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_CHILD_LOOKUP() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a>Text</a>").WithTest("<a><Element/></a>").Build();
            AssertPreRequirements(diff, ComparisonType.CHILD_LOOKUP);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected child '#text' but was '' - "
                            + "comparing <a ...>Text</a> at /a[1]/text()[1] to <NULL>",
                            description);

            AssertEquals("<a>Text</a>", controlDetails);
            AssertEquals("<NULL>", testDetails);
        }

        [Test]
        public void TestComparisonType_ATTR_NAME_LOOKUP() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a b=\"xxx\"></a>").WithTest("<a c=\"yyy\"></a>").Build();
            AssertPreRequirements(diff, ComparisonType.ATTR_NAME_LOOKUP);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected attribute name '/a[1]/@b' - "
                            + "comparing <a...> at /a[1]/@b to <a...> at /a[1]", description);

            AssertEquals("<a b=\"xxx\">\n</a>", controlDetails);
            AssertEquals("<a c=\"yyy\">\n</a>", testDetails);
        }

        [Test]
        public void TestComparisonType_Comment() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a><!--XXX--></a>").WithTest("<a><!--YYY--></a>").Build();
            AssertPreRequirements(diff, ComparisonType.TEXT_VALUE);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);

            // validate result
            Assert.AreEqual("Expected text value 'XXX' but was 'YYY' - "
                            + "comparing <!--XXX--> at /a[1]/comment()[1] to <!--YYY--> at /a[1]/comment()[1]", description);

            AssertEquals("<a>\n  <!--XXX-->\n</a>", controlDetails);
            AssertEquals("<a>\n  <!--YYY-->\n</a>", testDetails);
        }

#if false
        [Test]
        public void TestComparisonType_WhitespacesAndUnformattedDetails() {
            // prepare data
            Diff diff = DiffBuilder.Compare("<a><b/></a>").WithTest("<a>\n  <b/>\n</a>").Build();
            AssertPreRequirements(diff, ComparisonType.CHILD_NODELIST_LENGTH);
            Comparison firstDiff = diff.Differences.First().Comparison;

            // run test
            string description = compFormatter.GetDescription(firstDiff);
            string controlDetails =  GetDetails(firstDiff.ControlDetails, firstDiff.Type);
            string testDetails =  GetDetails(firstDiff.TestDetails, firstDiff.Type);
            string controlDetailsUnformatted =  compFormatter
                .GetDetails(firstDiff.ControlDetails, firstDiff.Type, false);
            string testDetailsUnformatted =  compFormatter
                .GetDetails(firstDiff.TestDetails, firstDiff.Type, false);

            // validate result
            Assert.AreEqual("Expected child nodelist length '1' but was '3' - "
                            + "comparing <a...> at /a[1] to <a...> at /a[1]", description);

            AssertEquals("<a>\n  <b>\n</b>\n</a>", controlDetails);
            AssertEquals("<a>\n  <b>\n</b>\n</a>", testDetails);

            AssertEquals("<a><b>\n</b></a>", controlDetailsUnformatted);
            AssertEquals("<a>\n  <b>\n</b>\n</a>", testDetailsUnformatted);
        }
#endif

        /// <summary>
        ///   Assert Equals for two strings where carriage returns removed.
        /// </summary>
        public static void AssertEquals(string expected, string actual) {
            Assert.AreEqual(expected, actual.Replace("\r", ""));
        }

        private void AssertPreRequirements(Diff diff, ComparisonType comparisonType) {
            Assert.That(diff.Differences.FirstOrDefault(), Is.Not.Null);
            Assert.That(diff.Differences.First().Comparison.Type, Is.EqualTo(comparisonType));
        }

        private string GetDetails(Comparison.Detail difference, ComparisonType type) {
            return compFormatter.GetDetails(difference, type, true);
        }
    }
}
