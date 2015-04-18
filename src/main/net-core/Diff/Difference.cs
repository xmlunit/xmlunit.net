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
    /// Combines a {@link Comparison} and its {@link ComparisonResult result}.
    /// </summary>
    /// <remarks>
    ///   <para>As the name implies, the implicit assumption is that
    ///   the result is not ComparisonResult#EQUAL.</para>
    /// </remarks>
    public class Difference {
        private readonly ComparisonResult result;
        private readonly Comparison comparison;

        /// <summary>
        ///   Combines comparison and result.
        /// </summary>
        public Difference(Comparison comparison, ComparisonResult result) {
            this.result = result;
            this.comparison = comparison;
        }

        /// <summary>
        ///   The result of the difference.
        /// </summary>
        public ComparisonResult Result {
            get {
                return result;
            }
        }

        /// <summary>
        ///   The details of the comparison.
        /// </summary>
        public Comparison Comparison {
            get {
                return comparison;
            }
        }

        /// <summary>
        /// Returns a string representation of this difference using the
        /// given IComparisonFormatter.
        /// <param name="formatter">the IComparisonFormatter to use</param>
        /// <return>a string representation of this difference</return>
        /// </summary>
        public string ToString(IComparisonFormatter formatter) {
            return string.Format("{0} ({1})", Comparison.ToString(formatter), Result);
        }

        /// <summary>
        /// Returns a string representation of this difference using
        /// DefaultComparisonFormatter.
        /// <return>a string representation of this difference</return>
        /// </summary>
        public override string ToString() {
            return ToString(new DefaultComparisonFormatter());
        }
    }
}