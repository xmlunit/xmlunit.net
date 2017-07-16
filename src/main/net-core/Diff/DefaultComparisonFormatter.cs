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
using System.Text;
using System.Xml;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Formatter methods for a Comparison Object.
    /// </summary>
    public class DefaultComparisonFormatter : IComparisonFormatter {

        /// <summary>
        /// Return a short String of the Comparison including the
        /// XPath and the shorten value of the effected control and
        /// test Node.
        /// </summary>
        /// <param name="comparison">The Comparison to describe.</param>
        /// <return>a short description of the comparison</return>
        /// <remarks>
        ///   <para>
        /// In general the String will look like "Expected X 'Y' but
        /// was 'Z' - comparing A to B" where A and B are the result
        /// of invoking GetShortString on the target and XPath of the
        /// control and test details of the comparison. A is the
        /// description of the comparison and B and C are the control
        /// and test values (passed through GetValue) respectively.
        ///   </para>
        ///   <para>
        /// For missing attributes the string has a slightly different
        /// format.
        ///   </para>
        /// </remarks>
        public string GetDescription(Comparison difference) {
            ComparisonType type = difference.Type;
            string description = type.GetDescription();
            Comparison.Detail controlDetails = difference.ControlDetails;
            Comparison.Detail testDetails = difference.TestDetails;
            string controlTarget = GetShortString(controlDetails.Target,
                                                  controlDetails.XPath, type);
            string testTarget = GetShortString(testDetails.Target,
                                               testDetails.XPath, type);

            if (type == ComparisonType.ATTR_NAME_LOOKUP ) {
                return string.Format("Expected {0} '{1}' - comparing {2} to {3}",
                                     description,
                                     controlDetails.XPath,
                                     controlTarget, testTarget);
            }
            return string.Format("Expected {0} '{1}' but was '{2}' - comparing {3} to {4}",
                                 description,
                                 GetValue(controlDetails.Value, type),
                                 GetValue(testDetails.Value, type),
                                 controlTarget, testTarget);
        }

        /// <summary>
        /// May alter the display of a comparison value for
        /// GetShortString based on the comparison type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This implementation returns value unless it is a
        /// comparison of node types in which case the numeric value
        /// (one of the values of the XmlNodeType enum) is mapped to a
        /// more useful string.
        /// </para>
        /// </remarks>
        /// <param name="value">the value to display</param>
        /// <param name="type">the comparison type</param>
        /// <return>the display value</return>
        /// since XMLUnit 2.4.0
        protected virtual object GetValue(object value, ComparisonType type) {
            return type == ComparisonType.NODE_TYPE
                ? NodeType((XmlNodeType) value) : value;
        }

        /// <summary>
        /// Return a string representation for GetShortString that
        /// describes the "thing" that has been compared so users know
        /// how to locate it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Examples are "&lt;bar ...&gt; at /foo[1]/bar[1]" for a
        /// comparison of elements or "&lt;!-- Comment Text --&gt; at
        /// /foo[2]/comment()[1]" for a comment.
        /// </para>
        /// <para>
        /// This implementation dispatches to several AppendX methods
        /// based on the comparison type or the type of the node.
        /// </para>
        /// </remarks>
        /// <param name="node">the node to describe</param>
        /// <param name="xpath">xpath of the node if applicable</param>
        /// <param name="type">the comparison type</param>
        /// <return>the formatted result</return>
        /// since XMLUnit 2.4.0
        protected virtual string GetShortString(XmlNode node, string xpath, ComparisonType type) {
            StringBuilder sb = new StringBuilder();
            if (type == ComparisonType.HAS_DOCTYPE_DECLARATION) {
                XmlDocument doc = node as XmlDocument;
                AppendDocumentType(sb, doc.DocumentType);
                AppendDocumentElementIndication(sb, doc);
            } else if (node is XmlDocument) {
                XmlDocument doc = node as XmlDocument;
                AppendDocumentXmlDeclaration(sb, doc.FirstChild as XmlDeclaration);
                AppendDocumentElementIndication(sb, doc);
            } else if (node is XmlDeclaration) {
                XmlDeclaration dec = node as XmlDeclaration;
                AppendDocumentXmlDeclaration(sb, dec);
                AppendDocumentElementIndication(sb, dec.OwnerDocument);
            } else if (node is XmlDocumentType) {
                XmlDocumentType docType = node as XmlDocumentType;
                AppendDocumentType(sb, docType);
                AppendDocumentElementIndication(sb, docType.OwnerDocument);
            } else if (node is XmlAttribute) {
                AppendAttribute(sb, node as XmlAttribute);
            } else if (node is XmlElement) {
                AppendElement(sb, node as XmlElement);
            } else if (node is XmlComment) {
                AppendComment(sb, node as XmlComment);
            } else if (node is XmlCharacterData) {
                AppendText(sb, node as XmlCharacterData);
            } else if (node is XmlProcessingInstruction) {
                AppendProcessingInstruction(sb, node as XmlProcessingInstruction);
            } else if (node == null) {
                sb.Append("<NULL>");
            } else {
                sb.Append("<!--NodeType ").Append(node.NodeType)
                    .Append(' ').Append(node.Name)
                    .Append('/').Append(node.Value)
                    .Append("-->");
            }
            AppendXPath(sb, xpath);
            return sb.ToString();
        }

        /// <summary>
        /// Appends the XPath information for GetShortString if present.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="xpath">the xpath to append, if any</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendXPath(StringBuilder sb, string xpath) {
            if (!string.IsNullOrEmpty(xpath)) {
                sb.Append(" at ").Append(xpath);
            }
        }

        /// <summary>
        /// Appends the XML declaration for GetShortString or
        /// AppendFullDocumentHeader if it contains non-default
        /// values.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <return>true if the XML declaration has been appended</return>
        /// since XMLUnit 2.4.0
        protected virtual bool AppendDocumentXmlDeclaration(StringBuilder sb, XmlDeclaration dec) {
            string version = dec == null ? "1.0" : dec.Version;
            string encoding = dec == null ? string.Empty : dec.Encoding;
            string standalone = dec == null ? string.Empty : dec.Standalone;
            if (version == "1.0" && string.IsNullOrEmpty(encoding)
                && string.IsNullOrEmpty(standalone)) {
                // only default values => ignore
                return false;
            }
            sb.Append("<?xml version=\"")
                .Append(version)
                .Append("\"");
            if (!string.IsNullOrEmpty(encoding)) {
                sb.Append(" encoding=\"")
                    .Append(encoding)
                    .Append("\"");
            }
            if (!string.IsNullOrEmpty(standalone)) {
                sb.AppendFormat(" standalone=\"{0}\"", standalone);
            }
            sb.Append("?>");
            return true;
        }

        /// <summary>
        /// Appends a short indication of the document's root element
        /// like "&lt;ElementName...&gt;" for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="doc">the XML document node</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendDocumentElementIndication(StringBuilder sb, XmlDocument doc) {
            sb.Append("<")
                .Append(doc.DocumentElement.Name)
                .Append("...>");
        }

        /// <summary>
        /// Appends the XML DOCTYPE for GetShortString or
        /// AppendFullDocumentHeader if present.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="type">the document type</param>
        /// <return>true if the DOCTPYE has been appended</return>
        /// since XMLUnit 2.4.0
        protected virtual bool AppendDocumentType(StringBuilder sb, XmlDocumentType type) {
            if (type == null) {
                return false;
            }
            sb.Append("<!DOCTYPE ").Append(type.Name);
            bool hasNoPublicId = true;
            if (!string.IsNullOrEmpty(type.PublicId)) {
                sb.Append(" PUBLIC \"").Append(type.PublicId).Append('"');
                hasNoPublicId = false;
            }
            if (!string.IsNullOrEmpty(type.SystemId)) {
                if (hasNoPublicId) {
                    sb.Append(" SYSTEM");
                }
                sb.Append(" \"").Append(type.SystemId).Append("\"");
            }
            sb.Append(">");
            return true;
        }

        /// <summary>
        /// Formats a processing instruction for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="instr">the processing instruction</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendProcessingInstruction(StringBuilder sb,
                                                        XmlProcessingInstruction instr) {
            sb.Append("<?")
                .Append(instr.Target)
                .Append(' ').Append(instr.Data)
                .Append("?>");
        }

        /// <summary>
        /// Formats a comment for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="aNode">the comment</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendComment(StringBuilder sb, XmlComment aNode) {
            sb.Append("<!--")
                .Append(aNode.Value)
                .Append("-->");
        }

        /// <summary>
        /// Formats a text or CDATA node for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="aNode">the text or CDATA node</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendText(StringBuilder sb, XmlCharacterData aNode) {
            sb.Append("<")
                .Append(aNode.ParentNode.Name)
                .Append(" ...>");

            if (aNode is XmlCDataSection) {
                sb.Append("<![CDATA[")
                    .Append(aNode.Value)
                    .Append("]]>");
            } else {
                sb.Append(aNode.Value);
            }

            sb.Append("</")
                .Append(aNode.ParentNode.Name)
                .Append(">");
        }

        /// <summary>
        /// Formats a placeholder for an element for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="aNode">the element</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendElement(StringBuilder sb, XmlElement aNode) {
            sb.Append("<")
                .Append(aNode.Name).Append("...")
                .Append(">");
        }

        /// <summary>
        /// Formats a placeholder for an attribute for GetShortString.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="aNode">the attribute</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendAttribute(StringBuilder sb, XmlAttribute aNode) {
            sb.Append("<").Append(aNode.OwnerElement.Name);
            sb.Append(' ')
                .Append(aNode.Name).Append("=\"")
                .Append(aNode.Value).Append("\"...>");
        }

        /// <summary>
        /// Return the xml node from Detail#Target as formatted String.
        /// </summary>
        /// <param name="details">The Comparison#ControlDetails or
        /// Comparison#TestDetails.</param>
        /// <param name="type">The implementation can return different
        /// details depending on the ComparisonType.</param>
        /// <param name="formatXml">set this to true if the Comparison
        /// was generated with DiffBuilder#IgnoreWhitespace.</param>
        /// <return>the full xml node</return>
        /// <remarks>
        ///   <para>
        /// Delegates to GetFullFormattedXml unless the
        /// Comparison.Detail's Target is null.
        ///   </para>
        /// </remarks>
        public string GetDetails(Comparison.Detail difference, ComparisonType type,
                                 bool formatXml) {
            if (difference.Target == null) {
                return "<NULL>";
            }
            return GetFullFormattedXml(difference.Target, type, formatXml);
        }

        /// <summary>
        /// Formats the node using a format suitable for the node type
        /// and comparison.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The implementation outputs the document prolog and start
        /// element for Document and DocumentType nodes and may elect
        /// to format the node's parent element rather than just the
        /// node depending on the node and comparison type. It
        /// delegates to AppendFullDocumentHeader or
        /// GetFormattedNodeXml.
        /// </para>
        /// </remarks>
        /// <param name="node">the node to format</param>
        /// <param name="type">the comparison type</param>
        /// <param name="formatXml">true if the Comparison was generated with IgnoreWhitespace - this affects the indentation of the generated output</param>
        /// <return>the fomatted XML</return>
        /// since XMLUnit 2.4.0
        protected virtual string GetFullFormattedXml(XmlNode node, ComparisonType type,
                                           bool formatXml) {
            StringBuilder sb = new StringBuilder();
            XmlNode nodeToConvert;
            if (type == ComparisonType.CHILD_NODELIST_SEQUENCE) {
                nodeToConvert = node.ParentNode;
            } else if (node is XmlDocument) {
                AppendFullDocumentHeader(sb, node as XmlDocument);
                return sb.ToString();
            } else if (node is XmlDocumentType) {
                XmlDocument doc = node.OwnerDocument;
                AppendFullDocumentHeader(sb, doc);
                return sb.ToString();
            }
            else if (node is XmlDeclaration)
            {
                XmlDeclaration dec = node as XmlDeclaration;
                AppendDocumentXmlDeclaration(sb, dec);
                return sb.ToString();
            }
            else if (node is XmlAttribute)
            {
                nodeToConvert = ((XmlAttribute) node).OwnerElement;
            } else if (node is XmlCharacterData) {
                // in case of a simple text node, show the parent
                // TAGs: "<a>xy</a>" instead "xy".
                nodeToConvert = node.ParentNode;
            } else {
                nodeToConvert = node;
            }
            sb.Append(GetFormattedNodeXml(nodeToConvert, formatXml));
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Appends the XML declaration and DOCTYPE if present as well
        /// as the document's root element for GetFullFormattedXml.
        /// </summary>
        /// <param name="sb">the builder to append to</param>
        /// <param name="doc">the document to format</param>
        /// since XMLUnit 2.4.0
        protected virtual void AppendFullDocumentHeader(StringBuilder sb, XmlDocument doc) {
            if (AppendDocumentXmlDeclaration(sb, doc.FirstChild as XmlDeclaration)) {
                sb.Append("\n");
            }
            if (AppendDocumentType(sb, doc.DocumentType)) {
                sb.Append("\n");
            }
            AppendOnlyElementStartTagWithAttributes(sb, doc.DocumentElement);
        }

        private void AppendOnlyElementStartTagWithAttributes(StringBuilder sb,
                                                             XmlElement element) {
            sb.Append("<")
                .Append(element.Name);
            XmlAttributeCollection attributes = element.Attributes;

            foreach (XmlAttribute attr in attributes) {
                sb.Append(" ")
                    .Append(attr.ToString());
            }
            if (element.HasChildNodes) {
                sb.Append(">\n  ...");
            } else {
                sb.Append("/>");
            }
        }

        /// <summary>
        /// Formats a node with the help of XmlWriter.
        /// </summary>
        /// <param name="nodeToConvert">the node to format</param>
        /// <param name="formatXml">true if the Comparison was generated with IgnoreWhitespace - this affects the indentation of the generated output</param>
        /// <return>the fomatted XML</return>
        /// since XMLUnit 2.4.0
        protected virtual string GetFormattedNodeXml(XmlNode nodeToConvert, bool formatXml) {
            try {
                int numberOfBlanksToIndent = formatXml ? 2 : -1;
                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = CreateXmlWriter(sb, numberOfBlanksToIndent)) {
                    nodeToConvert.WriteTo(writer);
                }
                return sb.ToString();
            } catch (Exception e) {
                return string.Format("ERROR {0}", e.Message);
            }
        }


        /// <summary>
        /// Create a default Writer to format a XML-Node to a String.
        /// </summary>
        /// <param name="numberOfBlanksToIndent">the number of spaces
        /// which is used for indent the XML-structure</param>
        /// <param name="sb">StringBuilder to wrap as writer</param>
        /// since XMLUnit 2.4.0
        protected virtual XmlWriter CreateXmlWriter(StringBuilder sb,
                                                    int numberOfBlanksToIndent) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.CloseOutput = true;
            if (numberOfBlanksToIndent >= 0) {
                settings.Indent = true;
                settings.IndentChars = string.Empty.PadLeft(numberOfBlanksToIndent);
            } else {
                settings.Indent = false;
            }
            return XmlWriter.Create(sb, settings);
        }

        /// <summary>
        /// Provides a display text for the constant values of the
        /// XmlNodeType enum.
        /// </summary>
        /// <param name="type">the node type</param>
        /// <return>the display text</return>
        /// since XMLUnit 2.4.0
        protected virtual String NodeType(XmlNodeType type) {
            switch(type) {
                case XmlNodeType.DocumentType:          return "Document Type";
                case XmlNodeType.EntityReference:       return "Entity Reference";
                case XmlNodeType.CDATA:                 return "CDATA Section";
                case XmlNodeType.ProcessingInstruction: return "Processing Instruction";
                default: break;
            }
            return Enum.GetName(typeof(XmlNodeType), type);
        }
    }
}
