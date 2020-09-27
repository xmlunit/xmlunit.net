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
using System.IO;
using System.Xml;

namespace Org.XmlUnit.Input
{
    /// <summary>
    /// Source that provides XML from a string.
    /// </summary>
    public sealed class StringSource : ISource
    {
        private string systemId;
        private readonly string content;
        /// <summary>
        /// Creates a new Source wrapping a string.
        /// </summary>
        /// <param name="c">the string to wrap</param>
        public StringSource(string c)
        {
            content = c;
        }

        /// <inheritdoc/>
        public XmlReader Reader
        {
            get
            {
                return XmlReader.Create(new StringReader(content));
            }
        }

        /// <inheritdoc/>
        public string SystemId
        {
            get
            {
                return systemId;
            }
            set
            {
                systemId = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("StringSource with content{0} and systemId {1}", content,
                                 SystemId);
        }

        public void Dispose() { }
    }
}
