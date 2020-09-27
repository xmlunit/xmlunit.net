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
using Org.XmlUnit.Util;

namespace Org.XmlUnit.Input {
    /// <summary>
    /// Performs XML normalization on a given ISource or DOM Node.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This means adjacent text nodes are merged to single nodes
    ///   and empty Text nodes removed (recursively).  See the linked
    ///   API docs for details.
    ///   </para>
    ///   <para>
    ///   When reading documents a parser usually puts the document
    ///   into normalized form anway.  You will only need to perform
    ///   XML normalization on DOM trees you have created
    ///   programmatically.
    ///   </para>
    ///   <para>
    ///     https://msdn.microsoft.com/en-us/library/system.xml.xmlnode.normalize%28v=vs.110%29.aspx
    ///   </para>
    /// </remarks>
    public class NormalizedSource : ISource {
        private bool disposed;
        private string systemId;
        private readonly XmlReader reader;
        private readonly XmlNode node;

        /// <summary>
        /// Creates a new Source wrapping a XmlNode after normalizing it.
        /// </summary>
        /// <param name="node">source with the original content</param>
        public NormalizedSource(XmlNode node)
            : this(node, null)
        {
        }

        /// <summary>
        /// Creates a new Source wrapping a XmlNode after normalizing it.
        /// </summary>
        /// <param name="node">source with the original content</param>
        /// <param name="systemId">the systemId to use</param>
        public NormalizedSource(XmlNode node, string systemId)
        {
            if (node == null) {
                throw new ArgumentNullException("node");
            }
            node.Normalize();
            this.node = node;
            this.reader = new XmlNodeReader(node);
            SystemId = systemId;
        }

        /// <summary>
        /// Creates a new Source with the same content as another source normalizing the document.
        /// </summary>
        /// <param name="originalSource">source with the original content</param>
        public NormalizedSource(ISource originalSource)
            : this(originalSource.ToDocument(), originalSource.SystemId) {
        }

        /// <summary>
        /// The node this source is wrapping
        /// </summary>
        public XmlNode Node {
            get {
                return node;
            }
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} with systemId {1}", GetType().Name,
                                 SystemId);
        }

        public void Dispose() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                reader.Close();
                disposed = true;
            }
        }
    }
}
