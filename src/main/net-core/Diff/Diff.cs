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

using System.Collections.Generic;
using System.Linq;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// The Diff-Object is the result of two comparisons.
    /// </summary>
    public class Diff {

        private static readonly IComparisonFormatter DEFAULT_FORMATTER =
            new DefaultComparisonFormatter();

        private readonly IEnumerable<Difference> differences;
        private readonly ISource controlSource;
        private readonly ISource testSource;
        private readonly IComparisonFormatter formatter;

        /// <summary>
        /// Creates the result of comparing two documents.
        /// </summary>
        /// <param name="controlSource">the reference document</param>
        /// <param name="testSource">the test document</param>
        /// <param name="differences">list of differences found</param>
        public Diff(ISource controlSource, ISource testSource,
                    IEnumerable<Difference> differences)
            : this(controlSource, testSource, DEFAULT_FORMATTER, differences) {
        }

        /// <summary>
        /// Creates the result of comparing two documents.
        /// </summary>
        /// <param name="controlSource">the reference document</param>
        /// <param name="testSource">the test document</param>
        /// <param name="formatter">formatter to use</param>
        /// <param name="differences">list of differences found</param>
        public Diff(ISource controlSource, ISource testSource,
                    IComparisonFormatter formatter,
                    IEnumerable<Difference> differences) {
            this.controlSource = controlSource;
            this.testSource = testSource;
            this.formatter = formatter;
            this.differences = differences;
        }

        /// <return>true if there was at least one difference.</return>
        public bool HasDifferences() {
            return differences.GetEnumerator().MoveNext();
        }

        /// <return>all differences found before the comparison process stopped.</return>
        public IEnumerable<Difference> Differences {
            get {
                return differences;
            }
        }

        /// <summary>
        /// The reference source.
        /// </summary>
        public ISource ControlSource {
            get {
                return controlSource;
            }
        }

        /// <summary>
        /// The test source.
        /// </summary>
        public ISource TestSource {
            get {
                return testSource;
            }
        }

        /// <inheritdoc/>
        public override string ToString() {
            return ToString(formatter);
        }

        /// <summary>
        /// Stringify the outcome using the fiven formatter
        /// </summary>
        /// <param name="formatter">the formatter to use</param>
        /// <returns>a string representation of the outcome</returns>
        public string ToString(IComparisonFormatter formatter) {
            if (!HasDifferences()) {
                return "[identical]";
            }
            return differences.First().Comparison.ToString(formatter);
        }
    }
}
