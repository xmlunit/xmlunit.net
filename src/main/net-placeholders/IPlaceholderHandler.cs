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
using Org.XmlUnit.Diff;

namespace Org.XmlUnit.Placeholder {

    /// <summary>
    /// Interface implemented by classes that are responsible for a
    /// placeholder keyword.
    /// </summary>
    /// <remarks>
    ///   <para>
    /// This interface and the whole module are considered experimental
    /// and any API may change between releases of XMLUnit.
    ///   </para>
    ///   <para>
    /// Implementations are expected to be thread-safe, the Eevaluate
    /// method may be invoked by multiple threads in parallel.
    ///   </para>
    ///   <para>
    /// since 2.8.0
    ///   </para>
    /// </remarks>
    public interface IPlaceholderHandler {

        /// <summary>
        /// The placeholder keyword this handler is responsible for.
        /// </summary>
        string Keyword { get; }
        /// <summary>
        /// Evaluate the test value when control contained the
        /// placeholder handled by this class.
        /// </summary>
        ComparisonResult Evaluate(string testText, params string[] args);
    }
}
