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
using System.Linq;
using System.Xml;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Common NodeFilter implementations.
    /// </summary>
    public static class NodeFilters {
        /// <summary>
        /// Suppresses document-type and XML declaration nodes.
        /// </summary>
        /// <remarks>
        ///   <para>
        /// This is the default used by AbstractDifferenceEngine and
        /// thus DOMDifferenceEngine.
        ///   </para>
        /// </remarks>
        public static bool Default(XmlNode n) {
            return n.NodeType != XmlNodeType.DocumentType
                && n.NodeType != XmlNodeType.XmlDeclaration;
         }

        /// <summary>
        /// Suppresses document-type and XML declaration nodes.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     since XMLUnit 2.6.0
        ///   </para>
        /// </remarks>
        public static bool AcceptAll(XmlNode n) {
            return true;
         }

        /// <summary>
        ///   Accepts nodes that are accepted by all given filters.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This short-circuits the given list of predicates and
        ///     returns false as soon as the first predicate does.
        ///   </para>
        ///   <para>
        ///     since XMLUnit 2.9.2
        ///   </para>
        /// </remarks>
        public static Predicate<XmlNode> SatifiesAll(params Predicate<XmlNode>[] predicates) {
            return node => predicates.All(p => p(node));
        }

        /// <summary>
        ///   Accepts nodes that are accepted by at least on of the given filters.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This short-circuits the given list of predicates and
        ///     returns true as soon as the first predicate does.
        ///   </para>
        ///   <para>
        ///     since XMLUnit 2.9.2
        ///   </para>
        /// </remarks>
        public static Predicate<XmlNode> SatifiesAny(params Predicate<XmlNode>[] predicates) {
            return node => predicates.Any(p => p(node));
        }

    }
}
