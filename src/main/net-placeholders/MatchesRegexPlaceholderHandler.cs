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
    /// Handler for the "matchesRegex()" placeholder keyword.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// since 2.8.0
    ///   </para>
    /// </remarks>
    public class MatchesRegexPlaceholderHandler : IPlaceholderHandler {
        private const string PLACEHOLDER_NAME = "matchesRegex";

        /// <inheritdoc/>
        public string Keyword { get { return PLACEHOLDER_NAME; } }
        /// <inheritdoc/>
        public ComparisonResult Evaluate(string testText, params string[] args) {
            if (args.Length > 0 && args[0] != null && args[0] != "") {
                var pattern = new Regex(args[0].Trim());
                if (testText != null && Evaluate(testText.Trim(), pattern)) {
                    return ComparisonResult.EQUAL;
                }
            }
            return ComparisonResult.DIFFERENT;
        }

        private bool Evaluate(string testText, Regex pattern) {
            return pattern.Match(testText).Success;
        }
    }
}
