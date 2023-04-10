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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.XmlUnit.Util {
    /// <summary>
    /// Utility algorithms that work on DOM nodes.
    /// </summary>
    public static class Nodes {

        /// <summary>
        /// Extracts a Node's name and namespace URI (if any).
        /// </summary>
        public static XmlQualifiedName GetQName(this XmlNode n) {
            return new XmlQualifiedName(n.LocalName, n.NamespaceURI);
        }

        /// <summary>
        /// Tries to merge all direct Text and CDATA children of the given
        /// Node and concatenates their value.
        /// </summary>
        /// <return>an empty string if the Node has no Text or CDATA
        /// children.</return>
        public static string GetMergedNestedText(XmlNode n) {
            return n.ChildNodes
                .Cast<XmlNode>()
                .Where(child => IsTextualContentNode(child))
                .Select(child => child.Value)
                .Where(s => s != null)
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s))
                .ToString();
        }

        /// <summary>
        /// Obtains an element's attributes as dictionary.
        /// </summary>
        public static IDictionary<XmlQualifiedName, string>
            GetAttributes(XmlNode n) {
            return GetAttributes(n, ignored => true);
        }

        /// <summary>
        /// Obtains an element's attributes as dictionary.
        /// </summary>
        /// <remarks>
        ///  <para>
        /// since XMLUnit 2.10.0
        ///  </para>
        /// </remarks>
        public static IDictionary<XmlQualifiedName, string>
            GetAttributes(XmlNode n, Predicate<XmlAttribute> attributeFilter) {
            XmlAttributeCollection coll = n.Attributes;
            if (coll != null) {
                return coll.Cast<XmlAttribute>()
                    .Where(a => attributeFilter(a))
                    .ToDictionary<XmlAttribute, XmlQualifiedName, string>(GetQName,
                                                                          a => a.Value);
            }
            return new Dictionary<XmlQualifiedName, string>();
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// empty text or CDATA nodes and where all textual content
        /// including attribute values or comments are trimmed.
        /// </summary>
        /// <remarks>
        ///  <para>
        /// Unlike <see cref="StripXmlWhitespace"/> this uses Unicode's idea
        /// of whitespace rather than the more restricted subset considered
        /// whitespace by XML.
        ///  </para>
        /// </remarks>
        public static XmlNode StripWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            HandleWsRec(cloned, TrimValue);
            return cloned;
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// empty text or CDATA nodes and where all textual content
        /// including attribute values or comments are trimmed of
        /// characters XML considers whitespace according to
        /// <see href="https://www.w3.org/TR/xml11/#NT-S"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        /// Unlike <see cref="StripWhitespace"/> this uses XML's idea
        /// of whitespace rather than the more extensive set considered
        /// whitespace by Unicode.
        ///  </para>
        ///   <para>
        /// since XMLUnit 2.10.0
        ///   </para>
        /// </remarks>
        public static XmlNode StripXmlWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            HandleWsRec(cloned, XmlTrimValue);
            return cloned;
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// empty text or CDATA nodes and where all textual content
        /// including attribute values or comments are normalized.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// "normalized" in this context means all whitespace
        /// characters are replaced by space characters and
        /// consecutive whitespace characaters are collapsed.
        ///   </para>
        ///   <para>
        /// This method is similiar to <see cref="StripWhitespace"/>
        /// but in addition "normalizes" whitespace.
        ///   </para>
        ///  <para>
        /// Unlike <see cref="NormalizeXmlWhitespace"/> this uses Unicode's idea
        /// of whitespace rather than the more restricted subset considered
        /// whitespace by XML.
        ///  </para>
        /// </remarks>
        public static XmlNode NormalizeWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            HandleWsRec(cloned, TrimAndNormalizeValue);
            return cloned;
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// empty text or CDATA nodes and where all textual content
        /// including attribute values or comments are normalized.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// "normalized" in this context means all XML whitespace
        /// characters are replaced by space characters and
        /// consecutive XML whitespace characaters are collapsed.
        ///   </para>
        ///   <para>
        /// This method is similiar to <see cref="StripXmlWhitespace"/>
        /// but in addition "normalizes" XML whitespace.
        ///   </para>
        ///  <para>
        /// Unlike <see cref="NormalizeWhitespace"/> this uses XML's idea
        /// of whitespace rather than the more extensive set considered
        /// whitespace by Unicode.
        ///  </para>
        ///  <para>
        /// since XMLUnit 2.10.0
        ///  </para>
        /// </remarks>
        public static XmlNode NormalizeXmlWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            HandleWsRec(cloned, XmlTrimAndNormalizeValue);
            return cloned;
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// text or CDATA nodes that only consist of whitespace.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// This doesn't have any effect if applied to a text or CDATA
        /// node itself.
        ///   </para>
        ///  <para>
        /// Unlike <see cref="StripXmlElementContentWhitespace"/> this uses Unicode's idea
        /// of whitespace rather than the more restricted subset considered
        /// whitespace by XML.
        ///  </para>
        ///   <para>
        /// since XMLUnit 2.6.0
        ///   </para>
        /// </remarks>
        public static XmlNode StripElementContentWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            StripECW(cloned, TrimValue);
            return cloned;
        }

        /// <summary>
        /// Creates a new Node (of the same type as the original node)
        /// that is similar to the orginal but doesn't contain any
        /// text or CDATA nodes that only consist of XML whitespace.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// This doesn't have any effect if applied to a text or CDATA
        /// node itself.
        ///   </para>
        ///  <para>
        /// Unlike <see cref="StripXmlElementContentWhitespace"/> this uses XML's idea
        /// of whitespace rather than the more extensive set considered
        /// whitespace by Unicode.
        ///  </para>
        ///   <para>
        /// since XMLUnit 2.10.0
        ///   </para>
        /// </remarks>
        public static XmlNode StripXmlElementContentWhitespace(XmlNode original) {
            XmlNode cloned = original.CloneNode(true);
            cloned.Normalize();
            StripECW(cloned, XmlTrimValue);
            return cloned;
        }

        /// <summary>
        /// Returns the nodes' value trimmed of all whitespace.
        /// </summary>
        private static String TrimValue(XmlNode n) {
            return n.Value.Trim();
        }

        /// <summary>
        /// Returns the nodes' value trimmed of all whitespace and Normalized
        /// </summary>
        private static String TrimAndNormalizeValue(XmlNode n) {
            return Normalize(TrimValue(n));
        }

        private static readonly char[] XML_WHITESPACE_CHARS = {
            ' ', '\r', '\n', '\t'
        };

        /// <summary>
        /// Returns the nodes' value trimmed of all characters XML considers whitespace.
        /// </summary>
        private static String XmlTrimValue(XmlNode n) {
            return n.Value.Trim(XML_WHITESPACE_CHARS);
        }

        /// <summary>
        /// Returns the nodes' value trimmed of all whitespace and Normalized
        /// </summary>
        private static String XmlTrimAndNormalizeValue(XmlNode n) {
            return XmlNormalize(XmlTrimValue(n));
        }

        /// <summary>
        /// Trims textual content of this node, removes empty text and
        /// CDATA children, recurses into its child nodes.
        /// </summary>
        private static void HandleWsRec(XmlNode n, Func<XmlNode, String> handleWs) {
            if (n is XmlCharacterData || n is XmlProcessingInstruction) {
                n.Value = handleWs(n);
            }
            LinkedList<XmlNode> toRemove = new LinkedList<XmlNode>();
            foreach (XmlNode child in n.ChildNodes) {
                HandleWsRec(child, handleWs);
                if (!(n is XmlAttribute)
                    && IsTextualContentNode(child)
                    && child.Value.Length == 0) {
                    toRemove.AddLast(child);
                }
            }
            foreach (XmlNode child in toRemove) {
                n.RemoveChild(child);
            }
            XmlNamedNodeMap attrs = n.Attributes;
            if (attrs != null) {
                foreach (XmlAttribute a in attrs) {
                    HandleWsRec(a, handleWs);
                }
            }
        }

        private const char SPACE = ' ';

        /// <summary>
        /// Normalize a string.
        /// </summary>
        /// <remarks>
        ///  <para>
        /// "normalized" in this context means all whitespace
        /// characters are replaced by space characters and
        /// consecutive whitespace characters are collapsed.
        ///  </para>
        ///  <para>
        /// Unlike <see cref="XmlNormalize"/> this uses Unicode's idea
        /// of whitespace rather than the more restricted subset considered
        /// whitespace by XML.
        ///  </para>
        /// </remarks>
        internal static string Normalize(string s) {
            return Normalize(s, c => char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Normalize a string with regard to XML whitespace.
        /// </summary>
        /// <remarks>
        ///  <para>
        /// "normalized" in this context means all XML whitespace
        /// characters are replaced by space characters and
        /// consecutive XML whitespace characters are collapsed.
        ///  </para>
        ///  <para>
        /// Unlike <see cref="Normalize"/> this uses XML's idea
        /// of whitespace rather than the more extensive set considered
        /// whitespace by Unicode.
        ///  </para>
        ///  <para>
        /// since XMLUnit 2.10.0
        ///  </para>
        /// </remarks>
        internal static string XmlNormalize(string s) {
            return Normalize(s, c => XML_WHITESPACE_CHARS.Contains(c));
        }

        private static string Normalize(string s, Predicate<char> isWhiteSpace) {
            StringBuilder sb = new StringBuilder();
            bool changed = false;
            bool lastCharWasWS = false;
            foreach (char c in s) {
                if (isWhiteSpace(c)) {
                    if (!lastCharWasWS) {
                        sb.Append(SPACE);
                        changed |= (c != SPACE);
                    } else {
                        changed = true;
                    }
                    lastCharWasWS = true;
                } else {
                    sb.Append(c);
                    lastCharWasWS = false;
                }
            }
            return changed ? sb.ToString() : s;
        }

        private static void StripECW(XmlNode n, Func<XmlNode, String> trimmer) {
            LinkedList<XmlNode> toRemove = new LinkedList<XmlNode>();
            foreach (XmlNode child in n.ChildNodes) {
                StripECW(child, trimmer);
                if (!(n is XmlAttribute)
                    && IsTextualContentNode(child)
                    && trimmer(child).Length == 0) {
                    toRemove.AddLast(child);
                }
            }
            foreach (XmlNode child in toRemove) {
                n.RemoveChild(child);
            }
        }

        private static bool IsTextualContentNode(XmlNode n) {
            return n is XmlText || n is XmlCDataSection || n is XmlWhitespace
                || n is XmlSignificantWhitespace;
        }
    }
}
