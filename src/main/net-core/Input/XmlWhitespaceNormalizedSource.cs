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
    /// all empty text nodes and normalizing the non-empty ones.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// "normalized" in this context means all XML whitespace
    /// characters are replaced by space characters and
    /// consecutive XML whitespace characters are collapsed.
    ///   </para>
    ///   <para>
    /// This class is similiar to <see cref="XmlWhitespaceStrippedSource"/>
    /// but in addition "normalizes" XML whitespace.
    ///   </para>
    ///   <para>
    /// Unlike <see cref="WhitespaceNormalizedSource"/> this uses XML's idea
    /// of whitespace rather than the more extensive set considered
    /// whitespace by Unicode.
    ///   </para>
    ///   <para>
    /// since XMLUnit 2.10.0
    ///   </para>
    /// </remarks>
    public class XmlWhitespaceNormalizedSource : DOMSource {
        /// <summary>
        /// Creates a new Source with the same content as another source normalizing XML whitespace in Text nodes.
        /// </summary>
        /// <param name="originalSource">source with the original content</param>
        public XmlWhitespaceNormalizedSource(ISource originalSource) :
            base(Nodes.NormalizeXmlWhitespace(originalSource.ToDocument()))
            {
            SystemId = originalSource.SystemId;
        }
    }
}
