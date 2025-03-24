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
using System.Xml;
using NUnit.Framework.Constraints;
using Org.XmlUnit.Util;
using Org.XmlUnit.Xpath;
using InputBuilder = Org.XmlUnit.Builder.Input;

namespace Org.XmlUnit.Constraints
{

    /// <summary>
    /// This NUnit3 Constraint verifies whether the evaluation of the
    /// provided XPath expression corresponds to the value constraint
    /// specified for the provided input XML object.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All types which are supported by Input.From(object)} can
    ///     be used as input for the XML object against the constraint
    ///     is evaluated.
    ///   </para>
    ///   <para>
    /// since XMLUnit 2.1.0
    ///   </para>
    /// </remarks>
    /// <example>
    ///   Simple Example
    /// <code>
    /// final String xml = "&lt;a&gt;&lt;b attr=\"abc\"&gt;&lt;/b&gt;&lt;/a&gt;";
    
    /// Assert.That(xml, EvaluateXPathConstraint.HasXPath("//a/b/@attr", Is.EqualTo("abc")));
    /// Assert.That(xml, EvaluateXPathConstraint.HasXPath("count(//a/b/c)", Is.EqualTo("0")));
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
    ///
    ///    var prefix2Uri = new Dictionary&lt;string, string&gt;();
    ///    prefix2Uri["atom"] = "http://www.w3.org/2005/Atom";
    ///    Assert.That(xml,
    ///          EvaluateXPathConstraint.HasXPath("//atom:feed/atom:entry/atom:id/text()",
    ///                                           Is.EqualTo("id1"))
    ///                                 .WithNamespaceContext(prefix2Uri));
    /// </code>
    /// </example>
    public class EvaluateXPathConstraint : Constraint
    {
        private readonly string xPath;
        private readonly IConstraint valueConstraint;
        private IDictionary<string, string> prefix2Uri;

        /// <summary>
        /// Creates a <see cref="EvaluateXPathConstraint"/> instance
        /// with the associated XPath expression and the value
        /// constraint corresponding to the XPath evaluation.
        /// </summary>
        /// <param name="xPath">xPath expression</param>
        /// <param name="valueConstraint">constraint for the value at the specified xpath</param>
        public EvaluateXPathConstraint(string xPath, IConstraint valueConstraint)
        {
            this.xPath = xPath;
            this.valueConstraint = valueConstraint;
        }

        /// <summary>
        /// Creates a constraint that matches when the examined XML
        /// input has a value at the specified <c>xPath</c> that
        /// satisfies the specified <c>valueConstraint</c>.
        /// </summary>
        /// <example>
        /// For example
        ///   <code>
        /// Assert.That(xml, EvaluateXPathConstraint.HasXPath("//fruits/fruit/@name",
        ///                                                   Is.EqualTo("apple"))
        ///   </code>
        /// </example>
        /// <param name="xPath">xPath expression</param>
        /// <param name="valueConstraint">constraint for the value at the specified xpath</param>
        /// <returns>the xpath constraint</returns>
        public static EvaluateXPathConstraint HasXPath(string xPath, IConstraint valueConstraint)
        {
            return new EvaluateXPathConstraint(xPath, valueConstraint);
        }

        // for compatibility with NUnit 4.x
        /// <inheritdoc/>
        public override string Description { get; protected set; }

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            ConstraintResult nested = valueConstraint.Resolve().ApplyTo(XPathEvaluate(actual));
            Description = "XML with XPath " + xPath + " evaluated to " + nested.ActualValue;
            return new HasXPathConstraintResult(this, actual, nested);
        }
        
        /// <summary>
        /// Utility method used for creating a namespace context
        /// mapping to be used in XPath matching.
        /// </summary>
        /// <param name="prefix2Uri">maps from prefix to namespace
        /// URI. It is used to resolve XML namespace prefixes in the
        /// XPath expression</param>
        public EvaluateXPathConstraint WithNamespaceContext(IDictionary<string, string> prefix2Uri)
        {
            this.prefix2Uri = prefix2Uri;
            return this;
        }

        private string XPathEvaluate(object input)
        {
            XPathEngine engine = new XPathEngine();
            if (prefix2Uri != null)
            {
                engine.NamespaceContext = prefix2Uri;
            }

            ISource s = InputBuilder.From(input).Build();
            XmlNode n = Convert.ToNode(s);
            return engine.Evaluate(xPath, n);
        }

        /// <summary>
        ///   Result of a EvaluateXPathConstraint.
        /// </summary>
        public class HasXPathConstraintResult : ConstraintResult
        {
            private readonly EvaluateXPathConstraint constraint;
            private readonly ConstraintResult nestedResult;

            /// <summary>
            /// Creates the result.
            /// </summary>
            public HasXPathConstraintResult(EvaluateXPathConstraint constraint, object actual, ConstraintResult nestedResult)
                : base(constraint, actual, nestedResult.IsSuccess)
            {
                this.constraint = constraint;
                this.nestedResult = nestedResult;
            }
            
            /// <inheritdoc/>
            public override void WriteMessageTo(MessageWriter writer)
            {
                writer.Write("XML with XPath {0}", constraint.xPath);
                nestedResult.WriteMessageTo(writer);
            }
        }

    }
}
