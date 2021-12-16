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
using System.Xml.Schema;
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Diff{

    /// <summary>
    /// Difference engine based on DOM.
    /// </summary>
    public sealed class DOMDifferenceEngine : AbstractDifferenceEngine {

        /// <inheritdoc/>
        public override void Compare(ISource control, ISource test) {
            if (control == null) {
                throw new ArgumentNullException("control");
            }
            if (test == null) {
                throw new ArgumentNullException("test");
            }
            try {
                XmlNode controlNode = control.ToNode();
                XmlNode testNode = test.ToNode();
                CompareNodes(controlNode, XPathContextFor(controlNode),
                             testNode, XPathContextFor(testNode));
            } catch (Exception ex) {
                throw new XMLUnitException("Caught exception during comparison",
                                           ex);
            }
        }

        private XPathContext XPathContextFor(XmlNode n) {
            return new XPathContext(NamespaceContext, n);
        }

        /// <summary>
        /// Recursively compares two XML nodes.
        /// </summary>
        /// <remarks>
        /// Performs comparisons common to all node types, then performs
        /// the node type specific comparisons and finally recurses into
        /// the node's child lists.
        ///
        /// Stops as soon as any comparison returns ComparisonResult.CRITICAL.
        /// </remarks>
        internal ComparisonState CompareNodes(XmlNode control,
                                              XPathContext controlContext,
                                              XmlNode test,
                                              XPathContext testContext) {
            IEnumerable<XmlNode> allControlChildren =
                control.ChildNodes.Cast<XmlNode>();
            IEnumerable<XmlNode> controlChildren =
                allControlChildren.Where(n => NodeFilter(n));
            IEnumerable<XmlNode> allTestChildren =
                test.ChildNodes.Cast<XmlNode>();
            IEnumerable<XmlNode> testChildren =
                allTestChildren.Where(n => NodeFilter(n));

            return Compare(new Comparison(ComparisonType.NODE_TYPE,
                                          control, GetXPath(controlContext),
                                          control.NodeType, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          test.NodeType, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.NAMESPACE_URI,
                                        control, GetXPath(controlContext),
                                        control.NamespaceURI, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.NamespaceURI, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.NAMESPACE_PREFIX,
                                        control, GetXPath(controlContext),
                                        control.Prefix, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.Prefix, GetParentXPath(testContext)))
                .AndIfTrueThen(control.NodeType != XmlNodeType.Attribute,
                               new Comparison(ComparisonType.CHILD_NODELIST_LENGTH,
                                              control, GetXPath(controlContext),
                                              controlChildren.Count(), GetParentXPath(controlContext),
                                              test, GetXPath(testContext),
                                              testChildren.Count(), GetParentXPath(testContext)))
                .AndThen(() => NodeTypeSpecificComparison(control, controlContext,
                                                          test, testContext))
                // and finally recurse into children
                .AndIfTrueThen(control.NodeType != XmlNodeType.Attribute,
                               CompareChildren(controlContext,
                                               allControlChildren,
                                               controlChildren,
                                               testContext,
                                               allTestChildren,
                                               testChildren));
        }

        /// <summary>
        /// Dispatches to the node type specific comparison if one is
        /// defined for the given combination of nodes.
        /// </summary>
        private ComparisonState NodeTypeSpecificComparison(XmlNode control,
                                                           XPathContext controlContext,
                                                           XmlNode test,
                                                           XPathContext testContext) {
            switch (control.NodeType) {
            case XmlNodeType.CDATA:
            case XmlNodeType.Comment:
            case XmlNodeType.Text:
                if (test is XmlCharacterData) {
                    return CompareCharacterData((XmlCharacterData) control,
                                                controlContext,
                                                (XmlCharacterData) test,
                                                testContext);
                }
                break;
            case XmlNodeType.Document:
                if (test is XmlDocument) {
                    return CompareDocuments((XmlDocument) control,
                                            controlContext,
                                            (XmlDocument) test, testContext);
                }
                break;
            case XmlNodeType.Element:
                if (test is XmlElement) {
                    return CompareElements((XmlElement) control,
                                           controlContext,
                                           (XmlElement) test,
                                           testContext);
                }
                break;
            case XmlNodeType.ProcessingInstruction:
                if (test is XmlProcessingInstruction) {
                    return
                        CompareProcessingInstructions((XmlProcessingInstruction) control,
                                                      controlContext,
                                                      (XmlProcessingInstruction) test,
                                                      testContext);
                }
                break;
            case XmlNodeType.DocumentType:
                if (test is XmlDocumentType) {
                    return CompareDocTypes((XmlDocumentType) control,
                                           controlContext,
                                           (XmlDocumentType) test, testContext);
                }
                break;
            case XmlNodeType.Attribute:
                if (test is XmlAttribute) {
                    return CompareAttributes((XmlAttribute) control,
                                             controlContext,
                                             (XmlAttribute) test, testContext);
                }
                break;
            default:
                break;
            }
            return new OngoingComparisonState(this);
        }

        private Func<ComparisonState> CompareChildren(XPathContext controlContext,
                                                      IEnumerable<XmlNode> allControlChildren,
                                                      IEnumerable<XmlNode> controlChildren,
                                                      XPathContext testContext,
                                                      IEnumerable<XmlNode> allTestChildren,
                                                      IEnumerable<XmlNode> testChildren) {

            return () => {
                controlContext
                    .SetChildren(allControlChildren.Select<XmlNode, XPathContext.INodeInfo>
                                 (ElementSelectors.TO_NODE_INFO));
                testContext
                    .SetChildren(allTestChildren.Select<XmlNode, XPathContext.INodeInfo>
                                 (ElementSelectors.TO_NODE_INFO));
                return CompareNodeLists(allControlChildren, controlChildren, controlContext,
                                        allTestChildren, testChildren, testContext);
            };
        }

        /// <summary>
        /// Compares textual content.
        /// </summary>
        private ComparisonState CompareCharacterData(XmlCharacterData control,
                                                     XPathContext controlContext,
                                                     XmlCharacterData test,
                                                     XPathContext testContext) {
            return Compare(new Comparison(ComparisonType.TEXT_VALUE, control,
                                          GetXPath(controlContext),
                                          control.Data, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          test.Data, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Compares document node, doctype and XML declaration properties
        /// </summary>
        private ComparisonState CompareDocuments(XmlDocument control,
                                                 XPathContext controlContext,
                                                 XmlDocument test,
                                                 XPathContext testContext) {
            XmlDocumentType controlDt = FilterNode(control.DocumentType);
            XmlDocumentType testDt = FilterNode(test.DocumentType);

            return Compare(new Comparison(ComparisonType.HAS_DOCTYPE_DECLARATION,
                                          control, GetXPath(controlContext),
                                          controlDt != null, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          testDt != null, GetParentXPath(testContext)))
                .AndIfTrueThen(controlDt != null && testDt != null,
                               () => CompareNodes(controlDt, controlContext,
                                                  testDt, testContext))
                .AndThen(() => CompareDeclarations(control.FirstChild as XmlDeclaration,
                                                   controlContext,
                                                   test.FirstChild as XmlDeclaration,
                                                   testContext));
        }

        private T FilterNode<T>(T n) where T : XmlNode {
            return n != null && NodeFilter(n) ? n : null;
        }

        /// <summary>
        /// Compares properties of the doctype declaration.
        /// </summary>
        private ComparisonState CompareDocTypes(XmlDocumentType control,
                                                XPathContext controlContext,
                                                XmlDocumentType test,
                                                XPathContext testContext) {
            return Compare(new Comparison(ComparisonType.DOCTYPE_NAME,
                                          control, GetXPath(controlContext),
                                          control.Name, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          test.Name, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.DOCTYPE_PUBLIC_ID,
                                        control, GetXPath(controlContext),
                                        control.PublicId, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.PublicId, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.DOCTYPE_SYSTEM_ID,
                                        control, GetXPath(controlContext),
                                        control.SystemId, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.SystemId, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Compares properties of XML declaration.
        /// </summary>
        private ComparisonState CompareDeclarations(XmlDeclaration control,
                                                    XPathContext controlContext,
                                                    XmlDeclaration test,
                                                    XPathContext testContext) {
            string controlVersion =
                control == null ? "1.0" : control.Version;
            string testVersion =
                test == null ? "1.0" : test.Version;
            string controlStandalone =
                control == null ? string.Empty : control.Standalone;
            string testStandalone =
                test == null ? string.Empty : test.Standalone;
            string controlEncoding =
                control != null ? control.Encoding : string.Empty;
            string testEncoding = test != null ? test.Encoding : string.Empty;

            return Compare(new Comparison(ComparisonType.XML_VERSION,
                                          control, GetXPath(controlContext),
                                          controlVersion, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          testVersion, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.XML_STANDALONE,
                                        control, GetXPath(controlContext),
                                        controlStandalone, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        testStandalone, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.XML_ENCODING,
                                        control, GetXPath(controlContext),
                                        controlEncoding, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        testEncoding, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Compares element's node properties, in particular the
        /// element's name and its attributes.
        /// </summary>
        private ComparisonState CompareElements(XmlElement control,
                                                XPathContext controlContext,
                                                XmlElement test,
                                                XPathContext testContext) {
            return Compare(new Comparison(ComparisonType.ELEMENT_TAG_NAME,
                                          control, GetXPath(controlContext),
                                          Nodes.GetQName(control).Name, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          Nodes.GetQName(test).Name, GetParentXPath(testContext)))
                .AndThen(() => CompareElementAttributes(control, controlContext,
                                                        test, testContext));
        }

        /// <summary>
        /// Compares element's attributes.
        /// </summary>
        private ComparisonState CompareElementAttributes(XmlElement control,
                                                         XPathContext controlContext,
                                                         XmlElement test,
                                                         XPathContext testContext) {
            Attributes controlAttributes = SplitAttributes(control.Attributes);
            controlContext
                .AddAttributes(controlAttributes.RemainingAttributes
                               .Select<XmlAttribute, XmlQualifiedName>(Nodes.GetQName));
            Attributes testAttributes = SplitAttributes(test.Attributes);
            testContext
                .AddAttributes(testAttributes.RemainingAttributes
                               .Select<XmlAttribute, XmlQualifiedName>(Nodes.GetQName));

            return Compare(new Comparison(ComparisonType.ELEMENT_NUM_ATTRIBUTES,
                                          control, GetXPath(controlContext),
                                          controlAttributes.RemainingAttributes.Count,
                                          GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          testAttributes.RemainingAttributes.Count,
                                          GetParentXPath(testContext)))
                .AndThen(() => CompareXsiType(controlAttributes.Type, controlContext,
                                              testAttributes.Type, testContext))
                .AndThen(new Comparison(ComparisonType.SCHEMA_LOCATION,
                                        control, GetXPath(controlContext),
                                        controlAttributes.SchemaLocation != null 
                                        ? controlAttributes.SchemaLocation.Value : null,
                                        GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        testAttributes.SchemaLocation != null
                                        ? testAttributes.SchemaLocation.Value : null,
                                        GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.NO_NAMESPACE_SCHEMA_LOCATION,
                                        control, GetXPath(controlContext),
                                        controlAttributes.NoNamespaceSchemaLocation != null
                                        ? controlAttributes.NoNamespaceSchemaLocation.Value
                                        : null, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        testAttributes.NoNamespaceSchemaLocation != null
                                        ? testAttributes.NoNamespaceSchemaLocation.Value
                                        : null, GetParentXPath(testContext)))
                .AndThen(NormalAttributeComparer(control, controlContext,
                                                 controlAttributes,
                                                 test, testContext,
                                                 testAttributes));
        }

        private Func<ComparisonState> NormalAttributeComparer(XmlElement control,
                                                              XPathContext controlContext,
                                                              Attributes controlAttributes,
                                                              XmlElement test,
                                                              XPathContext testContext,
                                                              Attributes testAttributes) {
            return () => {
                ComparisonState chain = new OngoingComparisonState(this);
                ICollection<XmlAttribute> foundTestAttributes = new HashSet<XmlAttribute>();

                foreach (XmlAttribute controlAttr
                         in controlAttributes.RemainingAttributes) {
                             XmlQualifiedName controlAttrName = controlAttr.GetQName();
                    XmlAttribute testAttr =
                        FindMatchingAttr(testAttributes.RemainingAttributes,
                                         controlAttr);
                    XmlQualifiedName testAttrName = testAttr != null
                        ? testAttr.GetQName() : null;

                    controlContext.NavigateToAttribute(controlAttrName);
                    try {
                        chain =
                            chain.AndThen(new Comparison(ComparisonType.ATTR_NAME_LOOKUP,
                                                         control, GetXPath(controlContext),
                                                         controlAttrName,
                                                         GetParentXPath(controlContext),
                                                         test, GetXPath(testContext),
                                                         testAttrName,
                                                         GetParentXPath(testContext)));

                        if (testAttr != null) {
                            testContext.NavigateToAttribute(testAttrName);
                            try {
                                chain =
                                    chain.AndThen(() =>
                                                  CompareNodes(controlAttr, controlContext,
                                                               testAttr, testContext));

                                foundTestAttributes.Add(testAttr);
                            } finally {
                                testContext.NavigateToParent();
                            }
                        }
                    } finally {
                        controlContext.NavigateToParent();
                    }
                }
                return chain.AndThen(() => {
                        ComparisonState secondChain = new OngoingComparisonState(this);
                        foreach (XmlAttribute testAttr
                                 in testAttributes.RemainingAttributes) {
                            if (!foundTestAttributes.Contains(testAttr)) {
                                XmlQualifiedName testAttrName = testAttr.GetQName();
                                testContext.NavigateToAttribute(testAttrName);
                                try {
                                    secondChain =
                                        secondChain
                                        .AndThen(new Comparison(ComparisonType.ATTR_NAME_LOOKUP,
                                                                control,
                                                                GetXPath(controlContext),
                                                                null, GetParentXPath(controlContext),
                                                                test,
                                                                GetXPath(testContext),
                                                                testAttrName,
                                                                GetParentXPath(testContext)));
                                } finally {
                                    testContext.NavigateToParent();
                                }
                            }
                        }
                        return secondChain;
                    });
            };
        }

        /// <summary>
        /// Compares properties of a processing instruction.
        /// </summary>
        private ComparisonState CompareProcessingInstructions(XmlProcessingInstruction control,
                                                              XPathContext controlContext,
                                                              XmlProcessingInstruction test,
                                                              XPathContext testContext) {
            return Compare(new Comparison(ComparisonType.PROCESSING_INSTRUCTION_TARGET,
                                          control, GetXPath(controlContext),
                                          control.Target, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          test.Target, GetParentXPath(testContext)))
                .AndThen(new Comparison(ComparisonType.PROCESSING_INSTRUCTION_DATA,
                                        control, GetXPath(controlContext),
                                        control.Data, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.Data, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Matches nodes of two node lists and invokes compareNode on
        /// each pair.
        /// </summary>
        /// <remarks>
        /// Also performs CHILD_LOOKUP comparisons for each node that
        /// couldn't be matched to one of the "other" list.
        /// </remarks>
        private ComparisonState CompareNodeLists(IEnumerable<XmlNode> allControlChildren,
                                                 IEnumerable<XmlNode> controlSeq,
                                                 XPathContext controlContext,
                                                 IEnumerable<XmlNode> allTestChildren,
                                                 IEnumerable<XmlNode> testSeq,
                                                 XPathContext testContext) {

            ComparisonState chain = new OngoingComparisonState(this);

            IEnumerable<KeyValuePair<XmlNode, XmlNode>> matches =
                NodeMatcher.Match(controlSeq, testSeq);
            IList<XmlNode> controlList = new List<XmlNode>(controlSeq);
            IList<XmlNode> testList = new List<XmlNode>(testSeq);

            IDictionary<XmlNode, int> controlListForXpathIndex = Index(allControlChildren);
            IDictionary<XmlNode, int> testListForXpathIndex = Index(allTestChildren);
            IDictionary<XmlNode, int> controlListIndex = Index(controlList);
            IDictionary<XmlNode, int> testListIndex = Index(testList);

            ICollection<XmlNode> seen = new HashSet<XmlNode>();
            foreach (KeyValuePair<XmlNode, XmlNode> pair in matches) {
                XmlNode control = pair.Key;
                seen.Add(control);
                XmlNode test = pair.Value;
                seen.Add(test);
                int controlIndexForXpath = controlListForXpathIndex[control];
                int testIndexForXpath = testListForXpathIndex[test];
                int controlIndex = controlListIndex[control];
                int testIndex = testListIndex[test];
                controlContext.NavigateToChild(controlIndexForXpath);
                testContext.NavigateToChild(testIndexForXpath);
                try {
                    chain =
                        chain.AndThen(new Comparison(ComparisonType.CHILD_NODELIST_SEQUENCE,
                                                     control, GetXPath(controlContext),
                                                     controlIndex, GetParentXPath(controlContext),
                                                     test, GetXPath(testContext),
                                                     testIndex, GetParentXPath(testContext)))
                        .AndThen(() => CompareNodes(control, controlContext,
                                                    test, testContext));
                } finally {
                    testContext.NavigateToParent();
                    controlContext.NavigateToParent();
                }
            }

            return chain
                .AndThen(UnmatchedControlNodes(controlListForXpathIndex, controlList, controlContext,
                    seen, testContext))
                .AndThen(UnmatchedTestNodes(testListForXpathIndex, testList, testContext,
                    seen, controlContext));
        }

        private Func<ComparisonState> UnmatchedControlNodes(IDictionary<XmlNode, int> controlListForXpathIndex,
                                                            IList<XmlNode> controlList,
                                                            XPathContext controlContext,
                                                            ICollection<XmlNode> seen,
                                                            XPathContext testContext) {
            return () => {
                ComparisonState chain = new OngoingComparisonState(this);
                int controlSize = controlList.Count;
                for (int i = 0; i < controlSize; i++) {
                    if (!seen.Contains(controlList[i])) {
                        controlContext
                            .NavigateToChild(controlListForXpathIndex[controlList[i]]);
                        try {
                            chain = chain
                                .AndThen(new Comparison(ComparisonType.CHILD_LOOKUP,
                                                        controlList[i],
                                                        GetXPath(controlContext),
                                                        controlList[i].GetQName(),
                                                        GetParentXPath(controlContext),
                                                        null, null, null,
                                                        GetXPath(testContext)));
                        } finally {
                            controlContext.NavigateToParent();
                        }
                    }
                }
                return chain;
            };
        }

        private Func<ComparisonState> UnmatchedTestNodes(IDictionary<XmlNode, int>  testListForXpathIndex,
                                                         IList<XmlNode> testList,
                                                         XPathContext testContext,
                                                         ICollection<XmlNode> seen,
                                                         XPathContext controlContext) {
            return () => {
                ComparisonState chain = new OngoingComparisonState(this);
                int testSize = testList.Count;
                for (int i = 0; i < testSize; i++) {
                    if (!seen.Contains(testList[i])) {
                        testContext.NavigateToChild(testListForXpathIndex[testList[i]]);
                        try {
                            chain = chain
                                .AndThen(new Comparison(ComparisonType.CHILD_LOOKUP,
                                                        null, null, null,
                                                        GetXPath(controlContext),
                                                        testList[i],
                                                        GetXPath(testContext),
                                                        testList[i].GetQName(),
                                                        GetParentXPath(testContext)));
                        } finally {
                            testContext.NavigateToParent();
                        }
                    }
                }
                return chain;
            };
        }

        /// <summary>
        /// Compares xsi:type attribute values
        /// </summary>
        private ComparisonState CompareXsiType(XmlAttribute control,
                                               XPathContext controlContext,
                                               XmlAttribute test,
                                               XPathContext testContext) {
            bool mustChangeControlContext = control != null;
            bool mustChangeTestContext = test != null;
            if (!mustChangeControlContext && !mustChangeTestContext) {
                return new OngoingComparisonState(this);
            }
            bool attributePresentOnBothSides = mustChangeControlContext
                && mustChangeTestContext;

            try {
                XmlQualifiedName controlAttrName = null;
                if (mustChangeControlContext) {
                    controlAttrName = control.GetQName();
                    controlContext.AddAttribute(controlAttrName);
                    controlContext.NavigateToAttribute(controlAttrName);
                }
                XmlQualifiedName testAttrName = null;
                if (mustChangeTestContext) {
                    testAttrName = test.GetQName();
                    testContext.AddAttribute(testAttrName);
                    testContext.NavigateToAttribute(testAttrName);
                }
                return Compare(new Comparison(ComparisonType.ATTR_NAME_LOOKUP,
                                              control, GetXPath(controlContext),
                                              controlAttrName,
                                              GetParentXPath(controlContext),
                                              test, GetXPath(testContext),
                                              testAttrName, GetParentXPath(testContext)))
                    .AndIfTrueThen(attributePresentOnBothSides,
                                   () => CompareAttributeExplicitness(control, controlContext,
                                                                      test, testContext))
                    .AndIfTrueThen(attributePresentOnBothSides,
                                   new Comparison(ComparisonType.ATTR_VALUE,
                                                  control, GetXPath(controlContext),
                                                  ValueAsQName(control),
                                                  GetParentXPath(controlContext),
                                                  test, GetXPath(testContext),
                                                  ValueAsQName(test),
                                                  GetParentXPath(testContext)));
            } finally {
                if (mustChangeControlContext) {
                    controlContext.NavigateToParent();
                }
                if (mustChangeTestContext) {
                    testContext.NavigateToParent();
                }
            }
        }

        /// <summary>
        /// Compares properties of an attribute.
        /// </summary>
        private ComparisonState CompareAttributes(XmlAttribute control,
                              XPathContext controlContext,
                              XmlAttribute test,
                              XPathContext testContext) {
            return CompareAttributeExplicitness(control, controlContext,
                                                test, testContext)
                .AndThen(new Comparison(ComparisonType.ATTR_VALUE,
                                        control, GetXPath(controlContext),
                                        control.Value, GetParentXPath(controlContext),
                                        test, GetXPath(testContext),
                                        test.Value, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Compares whether two attributes are specified explicitly.
        /// </summary>
        private ComparisonState CompareAttributeExplicitness(XmlAttribute control,
                                                             XPathContext controlContext,
                                                             XmlAttribute test,
                                                             XPathContext testContext) {
            return Compare(new Comparison(ComparisonType.ATTR_VALUE_EXPLICITLY_SPECIFIED,
                                          control, GetXPath(controlContext),
                                          control.Specified, GetParentXPath(controlContext),
                                          test, GetXPath(testContext),
                                          test.Specified, GetParentXPath(testContext)));
        }

        /// <summary>
        /// Separates XML namespace related attributes from "normal"
        /// attributes.
        /// </summary>
        private Attributes SplitAttributes(XmlAttributeCollection map) {
            XmlAttribute sLoc = map.GetNamedItem("schemaLocation",
                                                 XmlSchema.InstanceNamespace)
                as XmlAttribute;
            XmlAttribute nNsLoc = map.GetNamedItem("noNamespaceSchemaLocation",
                                                   XmlSchema.InstanceNamespace)
                as XmlAttribute;
            XmlAttribute type = map.GetNamedItem("type",
                                                   XmlSchema.InstanceNamespace)
                as XmlAttribute;
            List<XmlAttribute> rest = new List<XmlAttribute>();
            foreach (XmlAttribute a in map) {
                if ("http://www.w3.org/2000/xmlns/" != a.NamespaceURI
                    && a != sLoc && a != nNsLoc && a != type
                    && AttributeFilter(a)) {
                    rest.Add(a);
                }
            }
            return new Attributes(sLoc, nNsLoc, type, rest);
        }

        private static XmlQualifiedName ValueAsQName(XmlAttribute attribute) {
            if (attribute == null) {
                return null;
            }
            // split QName into prefix and local name
            string[] pieces = attribute.Value.Split(':');
            if (pieces.Length < 2) {
                // unprefixed name
                pieces = new string[] { string.Empty, pieces[0] };
            } else if (pieces.Length > 2) {
                // actually, this is not a valid QName - be lenient
                pieces = new string[] {
                    pieces[0],
                    attribute.Value.Substring(pieces[0].Length + 1)
                };
            }
            return new XmlQualifiedName(pieces[1] ?? string.Empty,
                                        attribute
                                        .GetNamespaceOfPrefix(pieces[0]));
        }

        internal class Attributes {
            internal readonly XmlAttribute SchemaLocation;
            internal readonly XmlAttribute NoNamespaceSchemaLocation;
            internal readonly XmlAttribute Type;
            internal readonly IList<XmlAttribute> RemainingAttributes;
            internal Attributes(XmlAttribute schemaLocation,
                                XmlAttribute noNamespaceSchemaLocation,
                                XmlAttribute type,
                                IList<XmlAttribute> remainingAttributes) {
                this.SchemaLocation = schemaLocation;
                this.NoNamespaceSchemaLocation = noNamespaceSchemaLocation;
                this.Type = type;
                this.RemainingAttributes = remainingAttributes;
            }
        }

        /// <summary>
        /// Find the attribute with the same namespace and local name
        /// as a given attribute in a list of attributes.
        /// </summary>
        private static XmlAttribute FindMatchingAttr(IList<XmlAttribute> attrs,
                                                     XmlAttribute attrToMatch) {
            bool hasNs = !string.IsNullOrEmpty(attrToMatch.NamespaceURI);
            string nsToMatch = attrToMatch.NamespaceURI;
            string nameToMatch = hasNs ? attrToMatch.LocalName
                : attrToMatch.Name;
            foreach (XmlAttribute a in attrs) {
                if (((!hasNs && string.IsNullOrEmpty(a.NamespaceURI))
                     ||
                     (hasNs && nsToMatch == a.NamespaceURI))
                    &&
                    ((hasNs && nameToMatch == a.LocalName)
                     ||
                     (!hasNs && nameToMatch == a.Name))
                    ) {
                    return a;
                }
            }
            return null;
        }

        private static IDictionary<XmlNode, int> Index(IEnumerable<XmlNode> nodes) {
            IDictionary<XmlNode, int> indices = new Dictionary<XmlNode, int>();
            int idx = 0;
            foreach (XmlNode n in nodes) {
                indices[n] = idx++;
            }
            return indices;
        }
    }
}
