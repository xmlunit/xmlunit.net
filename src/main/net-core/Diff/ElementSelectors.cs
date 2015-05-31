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
using System.Xml;
using Org.XmlUnit.Input;
using Org.XmlUnit.Util;
using Org.XmlUnit.Xpath;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Common ElementSelector implementations.
    /// </summary>
    public static class ElementSelectors {

        /// <summary>
        /// Always returns true, i.e. each element can be compared to each
        /// other element.
        /// </summary>
        /// <remarks>
        /// Generally this means elements will be compared in document
        /// order.
        /// </remarks>
        public static bool Default(XmlElement controlElement,
                                   XmlElement testElement) {
            return true;
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// can be compared.
        /// </summary>
        public static bool ByName(XmlElement controlElement,
                                  XmlElement testElement) {
            return controlElement != null && testElement != null
                && object.Equals(controlElement.GetQName(),
                                 testElement.GetQName());
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// and nested text (if any) can be compared.
        /// </summary>
        public static bool ByNameAndText(XmlElement controlElement,
                                         XmlElement testElement) {
            return ByName(controlElement, testElement)
                && object.Equals(Nodes.GetMergedNestedText(controlElement),
                                 Nodes.GetMergedNestedText(testElement));
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// and attribute values for the given attribute names can be
        /// compared.
        /// </summary>
        /// <remarks>Attributes are only searched for in the null
        /// namespace.</remarks>
        public static ElementSelector
        ByNameAndAttributes(params string[] attribs) {
            if (attribs == null) {
                throw new ArgumentNullException("attribs");
            }
            if (attribs.Any(a => a == null)) {
                throw new ArgumentException("must not contain null values", "attribs");
            }
            return ByNameAndAttributes(attribs
                                       .Select(a => new XmlQualifiedName(a))
                                       .ToArray());
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// and attribute values for the given attribute names can be
        /// compared.
        /// </summary>
        public static ElementSelector
            ByNameAndAttributes(params XmlQualifiedName[] attribs) {
            if (attribs == null) {
                throw new ArgumentNullException("attribs");
            }
            if (attribs.Any(a => a == null)) {
                throw new ArgumentException("must not contain null values", "attribs");
            }
            XmlQualifiedName[] qs = new XmlQualifiedName[attribs.Length];
            Array.Copy(attribs, 0, qs, 0, qs.Length);
            return (controlElement, testElement) =>
                   ByName(controlElement, testElement) &&
                   MapsEqualForKeys(Nodes.GetAttributes(controlElement),
                                    Nodes.GetAttributes(testElement),
                                    qs);
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// and attribute values for the given attribute names can be
        /// compared.
        /// </summary>
        /// <remarks>
        /// Namespace URIs of attributes are those of the attributes on
        /// the control element or the null namespace if they don't
        /// exist.
        /// </remarks>
        public static ElementSelector
            ByNameAndAttributesControlNS(params string[] attribs) {
            if (attribs == null) {
                throw new ArgumentNullException("attribs");
            }
            if (attribs.Any(a => a == null)) {
                throw new ArgumentException("must not contain null values", "attribs");
            }
            ICollection<string> ats = new HashSet<string>(attribs);
            return (controlElement, testElement) => {
                if (!ByName(controlElement, testElement)) {
                    return false;
                }
                IDictionary<XmlQualifiedName, string> cAttrs =
                    Nodes.GetAttributes(controlElement);
                IDictionary<string, XmlQualifiedName> qNameByLocalName =
                    new Dictionary<string, XmlQualifiedName>();
                foreach (XmlQualifiedName q in cAttrs.Keys) {
                    string local = q.Name;
                    if (ats.Contains(local)) {
                        qNameByLocalName[local] = q;
                    }
                }
                foreach (string a in ats) {
                    if (!qNameByLocalName.ContainsKey(a)) {
                        qNameByLocalName[a] = new XmlQualifiedName(a);
                    }
                }
                return MapsEqualForKeys(cAttrs,
                                        Nodes.GetAttributes(testElement),
                                        qNameByLocalName.Values);
            };
        }

        /// <summary>
        /// Elements with the same local name (and namespace URI - if any)
        /// and attribute values for all attributes can be compared.
        /// </summary>
        public static bool ByNameAndAllAttributes(XmlElement controlElement,
                                                  XmlElement testElement) {
            if (!ByName(controlElement, testElement)) {
                return false;
            }
            IDictionary<XmlQualifiedName, string> cAttrs =
                Nodes.GetAttributes(controlElement);
            IDictionary<XmlQualifiedName, string> tAttrs =
                Nodes.GetAttributes(testElement);
            if (cAttrs.Count != tAttrs.Count) {
                return false;
            }
            return MapsEqualForKeys(cAttrs, tAttrs, cAttrs.Keys);
        }

        /// <summary>
        ///   Negates another ElementSelector
        /// </summary>
        public static ElementSelector Not(ElementSelector es) {
            if (es == null) {
                throw new ArgumentNullException("es");
            }
            return (control, test) => !es(control, test);
        }

        /// <summary>
        ///   Accepts two elements if at least one of the given ElementSelectors does.
        /// </summary>
        public static ElementSelector Or(params ElementSelector[] selectors) {
            if (selectors == null) {
                throw new ArgumentNullException("selectors");
            }
            if (selectors.Any(s => s == null)) {
                throw new ArgumentException("must not contain null values", "selectors");
            }
            return (control, test) => selectors.Any(es => es(control, test));
        }

        /// <summary>
        ///   Accepts two elements if all of the given ElementSelectors do.
        /// </summary>
        public static ElementSelector And(params ElementSelector[] selectors) {
            if (selectors == null) {
                throw new ArgumentNullException("selectors");
            }
            if (selectors.Any(s => s == null)) {
                throw new ArgumentException("must not contain null values", "selectors");
            }
            return (control, test) => selectors.All(es => es(control, test));
        }

        /// <summary>
        ///    Accepts two elements if exactly on of the given ElementSelectors does.
        /// </summary>
        public static ElementSelector Xor(ElementSelector es1, ElementSelector es2) {
            if (es1 == null) {
                throw new ArgumentNullException("es1");
            }
            if (es2 == null) {
                throw new ArgumentNullException("es2");
            }
            return (control, test) => es1(control, test) ^ es2(control, test);
        }

        /// <summary>
        /// Applies the wrapped ElementSelector's logic if and only if the
        /// control element matches the given predicate.
        /// </summary>
        public static ElementSelector ConditionalSelector(Predicate<XmlElement> predicate,
                                                          ElementSelector es) {
            if (predicate == null) {
                throw new ArgumentNullException("predicate");
            }
            if (es == null) {
                throw new ArgumentNullException("es");
            }

            return (control, test) => predicate(control) && es(control, test);
        }

        /// <summary>
        /// Applies the wrapped ElementSelector's logic if and only if the
        /// control element has the given (local) name.
        /// </summary>
        public static ElementSelector SelectorForElementNamed(String expectedName,
                                                              ElementSelector es) {
            if (expectedName == null) {
                throw new ArgumentNullException("expectedName");
            }

            return ConditionalSelector(ElementNamePredicate(expectedName), es);
        }

        /// <summary>
        /// Applies the wrapped ElementSelector's logic if and only if the
        /// control element has the given name.
        /// </summary>
        public static ElementSelector SelectorForElementNamed(XmlQualifiedName expectedName,
                                                              ElementSelector es) {
            if (expectedName == null) {
                throw new ArgumentNullException("expectedName");
            }

            return ConditionalSelector(ElementNamePredicate(expectedName), es);
        }

        /// <summary>
        /// Selects two elements as matching if the child elements selected
        /// via XPath match using the given childSelector.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The xpath expression should yield elements.  Two elements
        ///     match if a DefaultNodeMatcher applied to the selected children
        ///     finds matching pairs for all children.
        ///   </para>
        /// </remarks>
        /// <param name="xpath">XPath expression applied in the context of the
        /// elements to chose from that selects the children to compare.</param>
        /// <param name="childSelector">ElementSelector to apply to the selected children.</param>
        /// <returns>an ElementSelector</returns>
        public static ElementSelector ByXPath(string xpath, ElementSelector childSelector) {
            return ByXPath(xpath, null, childSelector);
        }

        /// <summary>
        /// Selects two elements as matching if the child elements selected
        /// via XPath match using the given childSelector.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The xpath expression should yield elements.  Two elements
        ///     match if a DefaultNodeMatcher applied to the selected children
        ///     finds matching pairs for all children.
        ///   </para>
        /// </remarks>
        /// <param name="xpath">XPath expression applied in the context of the
        /// elements to chose from that selects the children to compare.</param>
        /// <param name="childSelector">ElementSelector to apply to the selected children.</param>
        /// <param name="namespaceContext">provides prefix mapping for namespace
        /// prefixes used inside the xpath expression.</param>
        public static ElementSelector ByXPath(string xpath,
                                              IDictionary<string, string> namespaceContext,
                                              ElementSelector childSelector) {
            IXPathEngine engine = new XPathEngine();
            if (namespaceContext != null) {
                engine.NamespaceContext = namespaceContext;
            }
            INodeMatcher nm = new DefaultNodeMatcher(childSelector);
            return (control, test) => {
                IEnumerable<XmlNode> controlChildren =
                    engine.SelectNodes(xpath, new DOMSource(control));
                int expected = controlChildren.Count();
                int matched = nm.Match(controlChildren,
                                       engine.SelectNodes(xpath, new DOMSource(test)))
                    .Count();
                return expected == matched;
            };
        }

        /// <summary>
        ///   then-part of conditional ElementSelectors built
        ///   via IConditionalSelectorBuilder.
        /// </summary>
        public interface IConditionalSelectorBuilderThen {
            /// <summary>
            /// Specifies the ElementSelector to use when the condition holds true.
            /// </summary>
            IConditionalSelectorBuilder ThenUse(ElementSelector es);
        }

        /// <summary>
        ///   Allows to build complex ElementSelectors by combining simpler blocks.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     All pairs created by the when*/thenUse pairs
        ///     are evaluated in order until one returns true, finally the
        ///     default, if any, is consulted.
        ///   </para>
        /// </remarks>
        public interface IConditionalSelectorBuilder {
            /// <summary>
            /// Sets up a conditional ElementSelector.
            /// </summary>
            IConditionalSelectorBuilderThen When(Predicate<XmlElement> predicate);
            /// <summary>
            /// Sets up a conditional ElementSelector.
            /// </summary>
            IConditionalSelectorBuilderThen WhenElementIsNamed(string expectedName);
            /// <summary>
            /// Sets up a conditional ElementSelector.
            /// </summary>
            IConditionalSelectorBuilderThen WhenElementIsNamed(XmlQualifiedName expectedName);
            /// <summary>
            /// Assigns a default ElementSelector.
            /// </summary>
            IConditionalSelectorBuilder DefaultTo(ElementSelector es);
            /// <summary>
            /// Builds a conditional ElementSelector.
            /// </summary>
            ElementSelector Build();
        }

        /// <summary>
        ///   Allows to build complex ElementSelectors by combining simpler blocks.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     All pairs created by the when*/thenUse pairs
        ///     are evaluated in order until one returns true, finally the
        ///     default, if any, is consulted.
        ///   </para>
        /// </remarks>
        public static IConditionalSelectorBuilder ConditionalBuilder() {
            return new DefaultConditionalSelectorBuilder();
        }

        private class DefaultConditionalSelectorBuilder
            : IConditionalSelectorBuilder, IConditionalSelectorBuilderThen {
            private ElementSelector defaultSelector;
            private readonly IList<ElementSelector> conditionalSelectors = new List<ElementSelector>();
            private Predicate<XmlElement> pendingCondition;

            public IConditionalSelectorBuilder ThenUse(ElementSelector es) {
                if (pendingCondition == null) {
                    throw new InvalidOperationException("missing condition");
                }
                conditionalSelectors.Add(ConditionalSelector(pendingCondition, es));
                pendingCondition = null;
                return this;
            }
            public IConditionalSelectorBuilderThen When(Predicate<XmlElement> predicate) {
                if (pendingCondition != null) {
                    throw new InvalidOperationException("unbalanced conditions");
                }
                pendingCondition = predicate;
                return this;
            }
            public IConditionalSelectorBuilder DefaultTo(ElementSelector es) {
                if (defaultSelector != null) {
                    throw new InvalidOperationException("can't have more than one default selector");
                }
                defaultSelector = es;
                return this;
            }
            public IConditionalSelectorBuilderThen WhenElementIsNamed(string expectedName) {
                return When(ElementNamePredicate(expectedName));
            }
            public IConditionalSelectorBuilderThen WhenElementIsNamed(XmlQualifiedName expectedName) {
                return When(ElementNamePredicate(expectedName));
            }
            public ElementSelector Build() {
                if (pendingCondition != null) {
                    throw new InvalidOperationException("unbalanced conditions");
                }
                List<ElementSelector> es = new List<ElementSelector>();
                es.AddRange(conditionalSelectors);
                if (defaultSelector != null) {
                    es.Add(defaultSelector);
                }
                return Or(es.ToArray());
            }
        }

        private static bool
            MapsEqualForKeys(IDictionary<XmlQualifiedName, string> control,
                             IDictionary<XmlQualifiedName, string> test,
                             IEnumerable<XmlQualifiedName> keys) {
            return !keys.Any(q => {
                string c, t;
                return control.TryGetValue(q, out c) != test.TryGetValue(q, out t)
                    || !object.Equals(c, t);
            });
        }

        private static Predicate<XmlElement> ElementNamePredicate(string expectedName) {
            return e => e != null && e.LocalName == expectedName;
        }

        private static Predicate<XmlElement> ElementNamePredicate(XmlQualifiedName expectedName) {
            return e => expectedName == e.GetQName();
        }

        /// <summary>
        /// Maps Nodes to their NodeInfo equivalent.
        /// </summary>
        internal static XPathContext.INodeInfo TO_NODE_INFO(XmlNode n) {
            return new XPathContext.DOMNodeInfo(n);
        }

    }
}
