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
    /// Controllers used for the base cases.
    /// </summary>
    public static class ComparisonControllers {

        /// <summary>
        ///   Does not stop the comparison at all.
        /// </summary>
        public static bool Default(Difference diff) {
            return false;
        }

        /// <summary>
        ///   Makes the comparison stop as soon as the first "real"
        ///   difference is encountered.
        /// </summary>
        public static bool StopWhenDifferent(Difference diff) {
            return diff.Result == ComparisonResult.DIFFERENT;
        }
    }
}