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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Org.XmlUnit.Input;

namespace Org.XmlUnit.Builder {
    /// <summary>
    /// Fluent API to create ISource instances.
    /// </summary>
    public sealed class Input {
        public interface IBuilder {
            /// <summary>
            /// build the actual ISource instance.
            /// </summary>
            ISource Build();
        }

        internal class SourceHoldingBuilder : IBuilder {
            protected readonly ISource source;
            internal SourceHoldingBuilder(ISource source) {
                this.source = source;
            }
            public ISource Build() {
                return source;
            }
        }

        /// <summary>
        /// Build an ISource from a DOM Document.
        /// </summary>
        public static IBuilder FromDocument(XmlDocument d) {
            return new SourceHoldingBuilder(new DOMSource(d));
        }

        /// <summary>
        /// Build an ISource from a DOM Node.
        /// </summary>
        public static IBuilder FromNode(XmlNode n) {
            return new SourceHoldingBuilder(new DOMSource(n));
        }

        internal class StreamBuilder : SourceHoldingBuilder {
            internal StreamBuilder(string s) : base(new StreamSource(s)) {
            }
            internal StreamBuilder(Stream s) : base(new StreamSource(s)) {
            }
            internal StreamBuilder(TextReader r) : base(new StreamSource(r)) {
            }
            internal string SystemId {
                set {
                    source.SystemId = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Build an ISource from a named file.
        /// </summary>
        public static IBuilder FromFile(string name) {
            return new StreamBuilder(name);
        }

        /// <summary>
        /// Build an ISource from a stream.
        /// </summary>
        public static IBuilder FromStream(Stream s) {
            StreamBuilder b = new StreamBuilder(s);
            if (s is FileStream) {
                b.SystemId = new Uri(Path.GetFullPath((s as FileStream).Name))
                    .ToString();
            }
            return b;
        }

        /// <summary>
        /// Build an ISource from a reader.
        /// </summary>
        public static IBuilder FromReader(TextReader r) {
            StreamBuilder b = new StreamBuilder(r);
            StreamReader s = r as StreamReader;
            if (s != null && s.BaseStream is FileStream) {
                b.SystemId =
                    new Uri(Path.GetFullPath((s.BaseStream as FileStream).Name))
                    .ToString();
            }
            return b;
        }

        /// <summary>
        /// Build an ISource from a string.
        /// </summary>
        public static IBuilder FromString(string s) {
            return FromReader(new StringReader(s));
        }

        /// <summary>
        /// Build an ISource from an array of bytes.
        /// </summary>
        public static IBuilder FromByteArray(byte[] b) {
            return FromStream(new MemoryStream(b));
        }

        /// <summary>
        /// Build an ISource from an URI.
        /// <param name="uri">must represent a valid URL</param>
        /// </summary>
        public static IBuilder FromURI(string uri) {
            return new StreamBuilder(uri);
        }

        /// <summary>
        /// Build an ISource from an URI.
        /// <param name="uri">must represent a valid URL</param>
        /// </summary>
        public static IBuilder FromURI(System.Uri uri) {
            return new StreamBuilder(uri.AbsoluteUri);
        }

        public interface ITransformationBuilder
            : ITransformationBuilderBase<ITransformationBuilder>, IBuilder {
            /// <summary>
            /// Sets the stylesheet to use.
            /// </summary>
            ITransformationBuilder WithStylesheet(IBuilder b);
        }

        internal class Transformation
            : AbstractTransformationBuilder<ITransformationBuilder>,
              ITransformationBuilder {

            internal Transformation(ISource s) : base(s) {
            }
            public ITransformationBuilder WithStylesheet(IBuilder b) {
                return WithStylesheet(b.Build());
            }
            public ISource Build() {
                return new DOMSource(Helper.TransformToDocument());
            }
        }

        /// <summary>
        /// Build an ISource by XSLT transforming a different ISource.
        /// </summary>
        public static ITransformationBuilder ByTransforming(ISource s) {
            return new Transformation(s);
        }
        /// <summary>
        /// Build an ISource by XSLT transforming a different ISource.
        /// </summary>
        public static ITransformationBuilder ByTransforming(IBuilder b) {
            return ByTransforming(b.Build());
        }

        /// <summary>
        /// Build an ISource from a System.Xml.Linq Document.
        /// </summary>
        public static IBuilder FromDocument(XDocument d) {
            return new SourceHoldingBuilder(new LinqSource(d));
        }

        /// <summary>
        /// Build an ISource from a System.Xml.Linq Node.
        /// </summary>
        public static IBuilder FromNode(XNode n) {
            return new SourceHoldingBuilder(new LinqSource(n));
        }

        /// <summary>
        /// Return the matching Builder for the supported types:
        /// ISource, IBuilder, XmlDocument, XmlNode, byte[] (XML as
        /// byte[]), string (XML as String) Uri (to an XML-Document),
        /// Stream, TextReader, XDocument, XNode
        /// </summary>
        public static IBuilder From(object source) {
            IBuilder xml;
            if (source is ISource) {
                xml = new SourceHoldingBuilder((ISource) source);
            } else if (source is IBuilder) {
                xml = (IBuilder) source;
            } else if (source is XmlDocument) {
                xml = Input.FromDocument((XmlDocument) source);
            } else if (source is XmlNode) {
                xml = Input.FromNode((XmlNode) source);
            } else if (source is byte[]) {
                xml = Input.FromByteArray((byte[]) source);
            } else if (source is string) {
                xml = Input.FromString((string) source);
            } else if (source is Uri) {
                xml = Input.FromURI((Uri) source);
            } else if (source is Stream) {
                xml = Input.FromStream((Stream) source);
            } else if (source is TextReader) {
                xml = Input.FromReader((TextReader) source);
            } else if (source is XDocument) {
                xml = Input.FromDocument((XDocument) source);
            } else if (source is XNode) {
                xml = Input.FromNode((XNode) source);
            } else {
                throw new ArgumentException("unsupported type", "source");
            }
            return xml;
        }
    }
}
