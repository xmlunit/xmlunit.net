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

using Org.XmlUnit.Util;

namespace Org.XmlUnit.Input {

    /// <summary>
    /// A source that is obtained from a different source by removing
    /// all empty text nodes and removing all characters XML considers
    /// whitespace at the start and end of the text content of the
    /// non-empty ones. <see href="https://www.w3.org/TR/xml11/#NT-S"/>
    /// </summary>
    /// <remarks>
    ///   <para>
    /// If you only want to remove text nodes consisting solely of
    /// whitespace (AKA element content whitespace) but leave all
    /// other text nodes alone you should use
    /// ElementContentWhitespaceStrippedSource instead.
    ///   </para>
    ///   <para>
    /// since XMLUnit 2.10.0
    ///   </para>
    /// </remarks>
    public class XmlWhitespaceStrippedSource : DOMSource {
        /// <summary>
        /// Creates a new Source with the same content as another source trimming whitespace from Text nodes.
        /// </summary>
        /// <param name="originalSource">source with the original content</param>
        public XmlWhitespaceStrippedSource(ISource originalSource) :
            base(Nodes.StripXmlWhitespace(originalSource.ToDocument())) {
            SystemId = originalSource.SystemId;
        }
    }
}
