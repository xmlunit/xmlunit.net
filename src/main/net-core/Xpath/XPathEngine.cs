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
using System.Xml.XPath;
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Xpath {

    /// <summary>
    /// Simplified access to System.Xml.XPath API.
    /// </summary>
    public class XPathEngine : IXPathEngine {
        private XmlNamespaceManager nsContext;

        /// <summary>
        /// Returns a potentially empty collection of Nodes matching an
        /// XPath expression.
        /// </summary>
        public IEnumerable<XmlNode> SelectNodes(string xPath, ISource s) {
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(s.Reader);
                return SelectNodes(xPath, doc);
            } catch (XPathException ex) {
                throw new XMLUnitException(ex);
            }
        }

        /// <summary>
        /// Evaluates an XPath expression and stringifies the result.
        /// </summary>
        public string Evaluate(string xPath, ISource s) {
            XPathDocument doc = new XPathDocument(s.Reader);
            return Evaluate(xPath, doc.CreateNavigator());
        }

        /// <summary>
        /// Returns a potentially empty collection of Nodes matching an
        /// XPath expression.
        /// </summary>
        public IEnumerable<XmlNode> SelectNodes(string xPath, XmlNode n) {
            try {
                return n.SelectNodes(xPath, nsContext).Cast<XmlNode>();
            } catch (XPathException ex) {
                throw new XMLUnitException(ex);
            }
        }

        /// <summary>
        /// Evaluates an XPath expression and stringifies the result.
        /// </summary>
        public string Evaluate(string xPath, XmlNode n) {
            return Evaluate(xPath, n.CreateNavigator());
        }

        /// <summary>
        /// Establish a namespace context mapping from URI to prefix
        /// that will be used in Comparison.Detail.XPath.
        /// </summary>
        /// <remarks>
        /// Without a namespace context (or with an empty context) the
        /// XPath expressions will only use local names for elements and
        /// attributes.
        /// </remarks>
        public IDictionary<string, string> NamespaceContext
        {
            set {
                nsContext = Convert.ToNamespaceContext(value);
            }
        }

        private string Evaluate(string xPath, XPathNavigator navigator) {
            try {
                object v = navigator.Evaluate(xPath, nsContext);
                if (v == null) {
                    return string.Empty;
                } else if (v is XPathNodeIterator) {
                    XPathNodeIterator it = v as XPathNodeIterator;
                    if (it.MoveNext()) {
                        return (string) it.Current
                            .ValueAs(typeof(string), nsContext);
                    }
                    return string.Empty;
                }
                return v.ToString();
            } catch (XPathException ex) {
                throw new XMLUnitException(ex);
            }
        }
    }
}