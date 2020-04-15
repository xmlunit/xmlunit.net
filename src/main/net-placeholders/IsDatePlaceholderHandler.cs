﻿/*
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

using Org.XmlUnit.Diff;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.XmlUnit.Placeholder
{
    /// <summary>
    /// Handler for the "isDate" handler placeholder keyword
    /// </summary>
    public class IsDatePlaceholderHandler : IPlaceholderHandler
    {
        private const string _keyword = "isDate";

        /// <inheritdoc/>
        public string Keyword => _keyword;

        /// <inheritdoc/>
        public ComparisonResult Evaluate(string testText)
        {
            var result = DateTime.TryParse(testText, out _);
            return result
                ? ComparisonResult.EQUAL
                : ComparisonResult.DIFFERENT;
        }
    }
}