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
using System.Text;
using System.Xml;
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Mutable representation of an XPath.
    /// </summary>
    public class XPathContext : ICloneable {
        private LinkedList<Level> path = new LinkedList<Level>();
        private readonly IDictionary<string, string> uri2Prefix;

        private const string COMMENT = "comment()";
        private const string PI = "processing-instruction()";
        private const string TEXT = "text()";
        private const string OPEN = "[";
        private const string CLOSE = "]";
        private const string SEP = "/";
        private const string ATTR = "@";

        /// <summary>
        /// Creates a new empty context without namespace context.
        /// </summary>
        public XPathContext() : this(null, null) {
        }

        /// <summary>
        /// Creates a new empty context with a given root without namespace context.
        /// </summary>
        /// <param name="root">the root of the XPath</param>
        public XPathContext(XmlNode root)
            : this(null, root)
        {
        }

        /// <summary>
        /// Creates a new empty context with namespace context.
        /// </summary>
        /// <param name="uri2Prefix">map from namespace URI to prefix to use when building the XPath expression</param>
        public XPathContext(IDictionary<string, string> uri2Prefix)
            : this(uri2Prefix, null) {
        }

        /// <summary>
        /// Creates a new empty context with a given root and namespace context.
        /// </summary>
        /// <param name="root">the root of the XPath</param>
        /// <param name="uri2Prefix">map from namespace URI to prefix to use when building the XPath expression</param>
        public XPathContext(IDictionary<string, string> uri2Prefix,
                            XmlNode root) {
                if (uri2Prefix == null) {
                this.uri2Prefix = new Dictionary<string, string>();
            } else {
                this.uri2Prefix = new Dictionary<string, string>(uri2Prefix);
            }
            path.AddLast(new Level(string.Empty));
            if (root != null) {
                SetChildren(new DOMNodeInfo(root).Singleton());
                NavigateToChild(0);
            }
        }

        /// <summary>
        /// Positions the XPath at the nth child of the current context.
        /// </summary>
        /// <param name="n">index of child to navigate to</param>
        public void NavigateToChild(int n) {
            path.AddLast(path.Last.Value.Children[n]);
        }

        /// <summary>
        /// Positions the XPath at the named attribute of the current element.
        /// </summary>
        /// <param name="attribute">name of the attribute</param>
        public void NavigateToAttribute(XmlQualifiedName attribute) {
            path.AddLast(path.Last.Value.Attributes[attribute]);
        }

        /// <summary>
        /// Positions the XPath at the parent of the current context.
        /// </summary>
        public void NavigateToParent()
        {
            path.RemoveLast();
        }

        /// <summary>
        /// Makes the list of attributes known to this context.
        /// </summary>
        /// <param name="attributes">the attributes to learn</param>
        public void AddAttributes<Q>(IEnumerable<Q> attributes)
            where Q : XmlQualifiedName {
            Level current = path.Last.Value;
            foreach (XmlQualifiedName attribute in attributes) {
                current.Attributes[attribute] =
                    new Level(ATTR + GetName(attribute));
            }
        }

        /// <summary>
        /// Makes the an attribute known to this context.
        /// </summary>
        /// <param name="attribute">the attribute to learn</param>
        public void AddAttribute(XmlQualifiedName attribute)
        {
            Level current = path.Last.Value;
            current.Attributes[attribute] =
                new Level(ATTR + GetName(attribute));
        }

        /// <summary>
        /// Replaces knowledge about children of the current context with the new list.
        /// </summary>
        /// <typeparam name="N">abstract representation of a child</typeparam>
        /// <param name="children">list of children to learn</param>
        public void SetChildren<N>(IEnumerable<N> children) 
            where N : INodeInfo {
            Level current = path.Last.Value;
            current.Children.Clear();
            AppendChildren(children);
        }

        /// <summary>
        /// Adds knowledge about children of the current context with the new list - adds to the children already known.
        /// </summary>
        /// <typeparam name="N">abstract representation of a child</typeparam>
        /// <param name="children">list of children to learn</param>
        public void AppendChildren<N>(IEnumerable<N> children) 
            where N : INodeInfo {
            Level current = path.Last.Value;
            int comments, pis, texts;
            comments = pis = texts = 0;
            IDictionary<string, int> elements = new Dictionary<string, int>();

            foreach (Level l in current.Children) {
                string childName = l.Expression;
                if (childName.StartsWith(COMMENT)) {
                    comments++;
                } else if (childName.StartsWith(PI)) {
                    pis++;
                } else if (childName.StartsWith(TEXT)) {
                    texts++;
                } else {
                    childName = childName.Substring(0, childName.IndexOf(OPEN));
                    Add1OrIncrement(childName, elements);
                }
            }

            foreach (INodeInfo child in children) {
                Level l = null;
                switch (child.Type) {
                case XmlNodeType.Comment:
                    l = new Level(COMMENT + OPEN + (++comments) + CLOSE);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    l = new Level(PI + OPEN + (++pis) + CLOSE);
                    break;
                case XmlNodeType.CDATA:
                case XmlNodeType.Text:
                    l = new Level(TEXT + OPEN + (++texts) + CLOSE);
                    break;
                case XmlNodeType.Element:
                    string name = GetName(child.Name);
                    l = new Level(name + OPEN + Add1OrIncrement(name, elements)
                                  + CLOSE);
                    break;
                default:
                    // more or less ignore
                    // FIXME: is this a good thing?
                    l = new Level(string.Empty);
                    break;
                }
                current.Children.Add(l);
            }
        }

        /// <summary>
        /// A stringified XPath describing the current context.
        /// </summary>
        public string XPath {
            get {
                return GetXPath(path.Last);
            }
        }

        /// <summary>
        /// Creates a deep copy of this XPathContext.
        /// </summary>
        public object Clone() {
            XPathContext c = (XPathContext) MemberwiseClone();
            c.path = new LinkedList<Level>();
            foreach (Level l in path) {
                c.path.AddLast((Level) l.Clone());
            }
            return c;
        }

        private static string GetXPath(LinkedListNode<Level> l) {
            if (l == null) {
                return string.Empty;
            }
            Level level = l.Value;
            if (null == level.XPath) {
                string previous = GetXPath(l.Previous);
                if (previous != SEP) {
                    previous += SEP;
                }
                level.XPath = previous + level.Expression;
            }
            return level.XPath;
        }

        private string GetName(XmlQualifiedName name) {
            string ns = name.Namespace;
            string p = null;
            if (ns != null) {
                uri2Prefix.TryGetValue(ns, out p);
            }
            return (p == null ? string.Empty : p + ":") + name.Name;
        }

        /// <summary>
        /// Increments the value name maps to or adds 1 as value if name
        /// isn't present inside the map.
        /// </summary>
        /// <returns>the new mapping for name</returns>
        private static int Add1OrIncrement(string name,
                                           IDictionary<string, int> map) {
            int index = 0;
            map.TryGetValue(name, out index);
            map[name] = ++index;
            return index;
        }

        internal class Level : ICloneable {
            private string xpath;
            internal readonly string Expression;
            internal IList<Level> Children = new List<Level>();
            internal IDictionary<XmlQualifiedName, Level> Attributes =
                new Dictionary<XmlQualifiedName, Level>();
            internal string XPath {
                get { return xpath; }
                set { xpath = value; }
            }
            internal Level(string expression) {
                this.Expression = expression;
            }
            public object Clone() {
                Level l = (Level) MemberwiseClone();
                l.Children = new List<Level>();
                foreach (Level c in Children) {
                    l.Children.Add((Level) c.Clone());
                }
                l.Attributes = new Dictionary<XmlQualifiedName, Level>();
                foreach (var e in Attributes) {
                    l.Attributes[e.Key] = (Level) e.Value.Clone();
                }
                return l;
            }
        }

        /// <summary>
        /// Abstract representation of a node inside the XPathContext.
        /// </summary>
        public interface INodeInfo {
            /// <summary>
            /// The fully qualified name of a node.
            /// </summary>
            XmlQualifiedName Name { get; }
            /// <summary>
            /// The type of a node.
            /// </summary>
            XmlNodeType Type { get; }
        }

        /// <summary>
        /// DOM based implementation of <see cref="INodeInfo"/>.
        /// </summary>
        public class DOMNodeInfo : INodeInfo {
            private readonly XmlQualifiedName name;
            private readonly XmlNodeType type;
            /// <summary>
            /// Obtains information from the given XmlNode
            /// </summary>
            /// <param name="n">node to read information from</param>
            public DOMNodeInfo(XmlNode n) {
                name = n.GetQName();
                type = n.NodeType;
            }

            /// <inheritdoc/>
            public XmlQualifiedName Name { get { return name; } }
            /// <inheritdoc/>
            public XmlNodeType Type { get { return type; } }
        }
    }
}