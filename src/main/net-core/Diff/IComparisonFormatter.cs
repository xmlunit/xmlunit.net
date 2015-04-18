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

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Formatter methods for a Comparison Object.
    /// </summary>
    public interface IComparisonFormatter {
        /// <summary>
        /// Return a short String of the Comparison including the
        /// XPath and the shorten value of the effected control and
        /// test Node.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///      This is used for Diff#ToString().
        ///   </para>
        ///   </remarks>
        string GetDescription(Comparison comparison);

        /// <summary>
        /// Return the xml node from Detail#Target as formatted String.
        /// </summary>
        /// <param name="details">The Comparison#ControlDetails or
        /// Comparison#TestDetails.</param>
        /// <param name="type">The implementation can return different
        /// details depending on the ComparisonType.</param>
        /// <param name="formatXml">set this to true if the Comparison
        /// was generated with DiffBuilder#IgnoreWhitespace.</param>
        /// <return>the full xml node</return>
        string GetDetails(Comparison.Detail details, ComparisonType type,
                          bool formatXml);
    }
}
