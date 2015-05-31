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
using System.Xml;

namespace Org.XmlUnit.Diff {
    /// <summary>
    /// ElementSelector that allows two elements to be compared if
    /// their name (including namespace URI, if any) and textual
    /// content is the same and the same is true for all child
    /// elements recursively.
    /// </summary>
    ///
    /// <remarks>
    /// This ElementSelector helps with structures nested more deeply
    /// but may need to be combined inside a
    /// ElementSelectors.ConditionalSelector in order to be useful for
    /// the document as a whole.
    /// </remarks>
    public static class ByNameAndTextRecSelector {

        /// <inheritdoc cref="ElementSelector"/>
        public static bool CanBeCompared(XmlElement controlElement,
                                         XmlElement testElement) {
            if (!ElementSelectors.ByNameAndText(controlElement, testElement)) {
                return false;
            }

            XmlNodeList controlChildren = controlElement.ChildNodes;
            XmlNodeList testChildren = testElement.ChildNodes;
            int controlLen = controlChildren.Count;
            int testLen = testChildren.Count;
            int controlIndex, testIndex;
            for (controlIndex = testIndex = 0;
                 controlIndex < controlLen && testIndex < testLen;
                 ) {
                // find next non-text child nodes
                XmlNode c = FindNonText(controlChildren, ref controlIndex, controlLen);
                if (IsText(c)) {
                    break;
                }
                XmlNode t = FindNonText(testChildren, ref testIndex, testLen);
                if (IsText(t)) {
                    break;
                }

                // different types of children make elements
                // non-comparable
                if (c.NodeType != t.NodeType) {
                    return false;
                }
                // recurse for child elements
                if (c is XmlElement
                    && !CanBeCompared(c as XmlElement, t as XmlElement)) {
                    return false;
                }

                controlIndex++;
                testIndex++;
            }

            // child lists exhausted?
            if (controlIndex < controlLen) {
                FindNonText(controlChildren, ref controlIndex, controlLen);
                // some non-Text children remained
                if (controlIndex < controlLen) {
                    return false;
                }
            }
            if (testIndex < testLen) {
                FindNonText(testChildren, ref testIndex, testLen);
                // some non-Text children remained
                if (testIndex < testLen) {
                    return false;
                }
            }
            return true;
        }

        private static XmlNode FindNonText(XmlNodeList nl, ref int current, int len) {
            XmlNode n = nl[current];
            while (IsText(n) && ++current < len) {
                n = nl[current];
            }
            return n;
        }

        private static bool IsText(XmlNode n) {
            return n is XmlText || n is XmlCDataSection;
        }

    }
}
