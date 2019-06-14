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

using System.Text.RegularExpressions;

using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {
    /// <summary>
    /// Handler for the "isNumber" placeholder keyword.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// since 2.7.1
    ///   </para>
    /// </remarks>
    public class IsNumberPlaceholderHandler : IPlaceholderHandler {
        private const string PLACEHOLDER_NAME = "isNumber";
        private const string NUMBER_PATTERN = "^[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?$";
        private static readonly Regex NUMBER_PATTERN_REGEX = new Regex(NUMBER_PATTERN);

        /// <inheritdoc/>
        public string Keyword { get { return PLACEHOLDER_NAME; } }
        /// <inheritdoc/>
        public ComparisonResult Evaluate(string testText) {
            return testText != null && NUMBER_PATTERN_REGEX.Match(testText).Success
                ? ComparisonResult.EQUAL : ComparisonResult.DIFFERENT;
        }
    }
}
