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
    /// Source that provides XML from a byte array.
    /// </summary>
    public sealed class ByteArraySource : ISource
    {
        private string systemId;
        private readonly byte[] content;
        /// <summary>
        /// Creates a new Source wrapping a byte array.
        /// </summary>
        /// <param name="c">the byte array to wrap</param>
        public ByteArraySource(byte[] c)
        {
            content = c;
        }

        /// <inheritdoc/>
        public XmlReader Reader
        {
            get
            {
                return XmlReader.Create(new MemoryStream(content));
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
            return string.Format("ByteArraySource with systemId {0}", SystemId);
        }
    }
}
