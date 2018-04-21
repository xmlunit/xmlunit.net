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
    /// A source that is obtained from a different source by removing all
    /// text nodes that only contain whitespace.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   since XMLUnit 2.6.0
    ///   </para>
    /// </remarks>
    public class ElementContentWhitespaceStrippedSource : DOMSource {
        /// <summary>
        /// Creates a new source that consists of the given source with all
        /// text nodes that only contain whitespace stripped.
        /// </summary>
        /// <param name="originalSource">source with the original content</param>
        public ElementContentWhitespaceStrippedSource(ISource originalSource) :
            base(Nodes.StripElementContentWhitespace(originalSource.ToDocument())) {
            SystemId = originalSource.SystemId;
        }
    }
}
