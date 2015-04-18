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
using System.Collections.Generic;

namespace Org.XmlUnit.Diff {

    /// <summary>
    ///   Extension methods for ComparisonType
    /// </summary>
    public static class ComparisonTypes {
        private static readonly IDictionary<ComparisonType, string> DESCS
            = new Dictionary<ComparisonType, string>();

        static ComparisonTypes() {
            DESCS[ComparisonType.ATTR_VALUE_EXPLICITLY_SPECIFIED]
                = "attribute value explicitly specified";
            DESCS[ComparisonType.ELEMENT_NUM_ATTRIBUTES] = "number of attributes";
            DESCS[ComparisonType.ATTR_VALUE] = "attribute value";
            DESCS[ComparisonType.CHILD_LOOKUP] = "child";
            DESCS[ComparisonType.ATTR_NAME_LOOKUP] = "attribute name";
        }

        public static string GetDescription(this ComparisonType type) {
            string description;
            if (DESCS.TryGetValue(type, out description)) {
                return description;
            }
            return Enum.GetName(typeof(ComparisonType), type)
                .ToLowerInvariant().Replace('_', ' ');
        }
    }
}