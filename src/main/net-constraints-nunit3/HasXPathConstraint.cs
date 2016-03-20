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
using NUnit.Framework.Constraints;
using Org.XmlUnit.Util;
using Org.XmlUnit.Xpath;
using InputBuilder = Org.XmlUnit.Builder.Input;

namespace Org.XmlUnit.Constraints {

    /// <summary>
    /// This NUnit3 Constraint verifies whether the provided XPath
    /// expression corresponds to at least one element in the provided
    /// object.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All types which are supported by Input.From(object) can be
    ///     used as input for the object against the constraint is
    ///     evaluated.
    ///   </para>
    ///   <para>
    ///   since XMLUnit 2.1.0  
    ///   </para>
    /// </remarks>
    /// <example>
    ///   Simple Example
    /// <code>
    /// string xml = "&lt;a&gt;&lt;b attr=\"abc\"&gt;&lt;/b&gt;&lt;/a&gt;";
    /// Assert.That(xml, HasXPathConstraint.HasXPath("//a/b/@attr"));
    /// Assert.That(xml, !HasXPathConstraint.HasXPath("//a/b/c"));
    /// </code>
    /// </example>
    /// <example>
    ///   Example with namespace mapping
    /// <code>
    ///    string xml = "&lt;?xml version=\"1.0\" encoding=\"UTF-8\"?&gt;" +
    ///          "&lt;feed xmlns=\"http://www.w3.org/2005/Atom\"&gt;" +
    ///          "   &lt;title&gt;title&lt;/title&gt;" +
    ///          "   &lt;entry&gt;" +
    ///          "       &lt;title&gt;title1&lt;/title&gt;" +
    ///          "       &lt;id&gt;id1&lt;/id&gt;" +
    ///          "   &lt;/entry&gt;" +
    ///          "&lt;/feed&gt;";
    ///    var prefix2Uri = new Dictionary&lt;string, string&gt;();
    ///    prefix2Uri["atom"] = "http://www.w3.org/2005/Atom";
    ///    Assert.That(xmlRootElement,
    ///          HasXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:id")
    ///                            .WithNamespaceContext(prefix2Uri));
    /// </code>
    /// </example>
    public class HasXPathConstraint : Constraint {

        private string xPath;
        private IDictionary<string, string> prefix2Uri;

        /// <summary>
        /// Creates a {@link HasXPathConstraint} instance with the
        /// associated XPath expression.
        /// </summary>
        /// <param name="xPath">xPath expression</param>
        public HasXPathConstraint(string xPath) {
            this.xPath = xPath;
        }

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo<TActual>(TActual actual) {
            XPathEngine engine = new XPathEngine();
            if (prefix2Uri != null) {
                engine.NamespaceContext = prefix2Uri;
            }

            var s = InputBuilder.From(actual).Build();
            var n = Convert.ToNode(s);
            var nodes = engine.SelectNodes(xPath, n);
            return new HasXPathConstraintResult(this, actual, nodes);
        }

        /// <summary>
        /// Creates a constraint that matches when the examined XML
        /// input has at least one node corresponding to the specified
        /// xPath.
        /// </summary>
        /// <param name="xPath">xPath expression</param>
        /// <returns>the xpath constraint</returns>
        /// <example>
        /// For example
        /// <code>
        /// Assert.That(xml, HasXPath("/root/cars[0]/audi"))
        /// </code>
        /// </example>
        public static HasXPathConstraint HasXPath(string xPath) {
            return new HasXPathConstraint(xPath);
        }

        /// <summary>
        /// Utility method used for creating a namespace context
        /// mapping to be used in XPath matching.
        /// </summary>
        /// <param name="prefix2Uri">maps from prefix to namespace
        /// URI. It is used to resolve XML namespace prefixes in the
        /// XPath expression</param>
         public HasXPathConstraint WithNamespaceContext(IDictionary<string, string> prefix2Uri) {
            this.prefix2Uri = prefix2Uri;
            return this;
        }

        /// <summary>
        ///   Result of a HasXPathConstraint.
        /// </summary>
        public class HasXPathConstraintResult : ConstraintResult {
            private readonly HasXPathConstraint constraint;

            /// <summary>
            /// Creates the result.
            /// </summary>
            public HasXPathConstraintResult(HasXPathConstraint constraint, object actual, IEnumerable<XmlNode> result)
                : base(constraint, actual, result.Count() > 0) {
                this.constraint = constraint;
            }
            
            /// <inheritdoc/>
            public override void WriteMessageTo(MessageWriter writer)
            {
                writer.WriteLine("XML with XPath {0} returned no results.",
                                 constraint.xPath);
            }
        }
    }
}