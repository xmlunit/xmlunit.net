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
using System.Xml;
using Org.XmlUnit.Transform;

namespace Org.XmlUnit.Input {

    /// <summary>
    /// ISource implementation that is obtained from a different
    /// source by stripping all comments.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     As of XMLUnit.NET 2.5.0 it is possible to select the XSLT
    ///     version to use for the stylesheet. The default now is 2.0,
    ///     it used to be 1.0 and you may need to change the value if
    ///     your transformer doesn't support XSLT 2.0.
    ///   </para>
    /// </remarks>
    public sealed class CommentLessSource : ISource {
        private bool disposed;
        private readonly XmlReader reader;
        private string systemId;

        private const string DEFAULT_VERSION = "2.0";

        private const string STYLE_TEMPLATE =
            "<stylesheet xmlns=\"http://www.w3.org/1999/XSL/Transform\" version=\"{0}\">"
            + "<template match=\"node()[not(self::comment())]|@*\"><copy>"
            + "<apply-templates select=\"node()[not(self::comment())]|@*\"/>"
            + "</copy></template>"
            + "</stylesheet>";

        private static readonly string STYLE = GetStylesheetContent(DEFAULT_VERSION);

        /// <summary>
        /// Creates a new Source with the same content as another
        /// source removing all comments using an XSLT stylesheet of
        /// version 2.0.
        /// </summary>
        /// <param name="originalSource">source with the original content</param>
        public CommentLessSource(ISource originalSource) : this(originalSource, DEFAULT_VERSION)
        {
        }

        /// <summary>
        /// Creates a new Source with the same content as another
        /// source removing all comments.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   since XMLUnit 2.5.0
        ///   </para>
        /// </remarks>
        /// <param name="originalSource">source with the original content</param>
        /// <param name="xsltVersion">use this version for the stylesheet</param>
        public CommentLessSource(ISource originalSource, string xsltVersion)
        {
            if (originalSource == null) {
                throw new ArgumentNullException();
            }
            if (xsltVersion == null) {
                throw new ArgumentNullException();
            }
            systemId = originalSource.SystemId;

            Transformation t = new Transformation(originalSource);
            t.Stylesheet = GetStylesheet(xsltVersion);
            reader = new XmlNodeReader(t.TransformToDocument());
        }

        /// <inheritdoc/>
        public XmlReader Reader
        {
            get {
                return reader;
            }
        }

        /// <inheritdoc/>
        public string SystemId
        {
            get {
                return systemId;
            }
            set {
                systemId = value;
            }
        }

        public void Dispose() {
            if (!disposed) {
                reader.Close();
                disposed = true;
            }
        }

        private static ISource GetStylesheet(string xsltVersion) {
            return new StreamSource(new System.IO.StringReader(GetStylesheetContentCached(xsltVersion)));
        }

        private static string GetStylesheetContentCached(string xsltVersion) {
            return DEFAULT_VERSION == xsltVersion ? STYLE : GetStylesheetContent(xsltVersion);
        }

        private static string GetStylesheetContent(string xsltVersion) {
            return string.Format(STYLE_TEMPLATE, xsltVersion);
        }
    }
}
