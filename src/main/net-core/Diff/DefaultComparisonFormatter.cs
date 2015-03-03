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

        private object GetValue(object value, ComparisonType type) {
            return type == ComparisonType.NODE_TYPE
                ? NodeType((XmlNodeType) value) : value;
        }

        private string GetShortString(object node, string xpath, ComparisonType type) {
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
            } else if (node is XmlNode) {
                XmlNode unknownNode = node as XmlNode;
                sb.Append("<!--NodeType ").Append(unknownNode.NodeType)
                    .Append(' ').Append(unknownNode.Name)
                    .Append('/').Append(unknownNode.Value)
                    .Append("-->");
            } else if (node == null) {
                sb.Append("<NULL>");
            } else {
                sb.Append("<!-- ").Append(node).Append(" -->");
            }
            if (!string.IsNullOrEmpty(xpath)) {
                sb.Append(" at ").Append(xpath);
            }
            return sb.ToString();
        }

        private bool AppendDocumentXmlDeclaration(StringBuilder sb, XmlDeclaration dec) {
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
        /// A short indication of the documents root element like "&lt;ElementName...&gt;".
        /// </summary>
        private void AppendDocumentElementIndication(StringBuilder sb, XmlDocument doc) {
            sb.Append("<")
                .Append(doc.DocumentElement.Name)
                .Append("...>");
        }

        private bool AppendDocumentType(StringBuilder sb, XmlDocumentType type) {
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

        private void AppendProcessingInstruction(StringBuilder sb,
                                                 XmlProcessingInstruction instr) {
            sb.Append("<?")
                .Append(instr.Target)
                .Append(' ').Append(instr.Data)
                .Append("?>");
        }

        private void AppendComment(StringBuilder sb, XmlComment aNode) {
            sb.Append("<!--")
                .Append(aNode.Value)
                .Append("-->");
        }

        private void AppendText(StringBuilder sb, XmlCharacterData aNode) {
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

        private void AppendElement(StringBuilder sb, XmlElement aNode) {
            sb.Append("<")
                .Append(aNode.Name).Append("...")
                .Append(">");
        }

        private void AppendAttribute(StringBuilder sb, XmlAttribute aNode) {
            sb.Append("<").Append(aNode.OwnerElement.Name);
            sb.Append(' ')
                .Append(aNode.Name).Append("=\"")
                .Append(aNode.Value).Append("\"...>");
        }

        public string GetDetails(Comparison.Detail difference, ComparisonType type,
                                 bool formatXml) {
            if (difference.Target is XmlNode) {
                return GetFullFormattedXml(difference.Target as XmlNode, type,
                                           formatXml);
            } else if (difference.Target == null) {
                return "<NULL>";
            }
            return difference.Target.ToString();
        }

        private string GetFullFormattedXml(XmlNode node, ComparisonType type,
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
            } else if (node is XmlAttribute) {
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

        private void AppendFullDocumentHeader(StringBuilder sb, XmlDocument doc) {
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

        private string GetFormattedNodeXml(XmlNode nodeToConvert, bool formatXml) {
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
        private static XmlWriter CreateXmlWriter(StringBuilder sb,
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

        private String NodeType(XmlNodeType type) {
            switch(type) {
                case XmlNodeType.DocumentType:          return "Document Type";
                case XmlNodeType.EntityReference:       return "Entity Reference";
                case XmlNodeType.CDATA:                 return "CDATA Section";
                case XmlNodeType.ProcessingInstruction: return "Processing Instruction";
            }
            return Enum.GetName(typeof(XmlNodeType), type);
        }
    }
}
