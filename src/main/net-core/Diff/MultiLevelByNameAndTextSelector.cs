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
    /// content is the same at a certain level of nesting.
    /// </summary>
    ///
    /// <remarks>
    /// <para>This means ElementSelectors.ByNameAndText and
    /// MultiLevelByNameAndTextSelector(1).CanBeCompared should lead
    /// to the same results.</para>
    /// <para>Any attribute values are completely ignored.  Only works
    /// on elements with exactly one child element at each
    /// level.</para>
    /// <para>This class mostly exists as an example for custom
    /// ElementSelectors and may need to be combined inside a
    /// ConditionalSelector in order to be useful for the document as
    /// a whole.</para>
    /// </remarks>
    public class MultiLevelByNameAndTextSelector {

        private readonly int levels;
        private readonly bool ignoreEmptyTexts;

        /// <summary>
        /// Uses element names and the text nested levels
        /// child elements deeper into the element to compare
        /// elements.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   Does not ignore empty text nodes.
        ///   </para>
        /// </remarks>
        /// <param name="levels">number of levels to traverse before the text content is encountered</param>
        public MultiLevelByNameAndTextSelector(int levels) : this(levels, false) {
        }

        /// <summary>
        /// Uses element names and the text nested levels
        /// child elements deeper into the element to compare
        /// elements.
        /// </summary>
        /// <param name="ignoreEmptyTexts">whether whitespace-only
        /// textnodes should be ignored.</param>
        /// <param name="levels">number of levels to traverse before the text content is encountered</param>
        public MultiLevelByNameAndTextSelector(int levels, bool ignoreEmptyTexts)
        {
            if (levels < 1) {
                throw new ArgumentException("levels must be equal or"
                                            + " greater than one", "levels");
            }
            this.levels = levels;
            this.ignoreEmptyTexts = ignoreEmptyTexts;
        }


        /// <inheritdoc cref="ElementSelector"/>
        public bool CanBeCompared(XmlElement controlElement,
                                  XmlElement testElement) {
            XmlElement currentControl = controlElement;
            XmlElement currentTest = testElement;

            // match on element names only for leading levels
            for (int currentLevel = 0; currentLevel <= levels - 2; currentLevel++) {
                if (!ElementSelectors.ByName(currentControl, currentTest)
                    || !currentControl.HasChildNodes
                    || !currentTest.HasChildNodes) {
                    return false;
                }
                XmlNode n1 = GetFirstEligibleChild(currentControl);
                XmlNode n2 = GetFirstEligibleChild(currentTest);
                if (n1 is XmlElement && n2 is XmlElement) {
                    currentControl = n1 as XmlElement;
                    currentTest = n2 as XmlElement;
                } else {
                    return false;
                }
            }

            // finally compare the level containing the text child node
            return ElementSelectors.ByNameAndText(currentControl,
                                                  currentTest);
        }

        private XmlNode GetFirstEligibleChild(XmlNode parent) {
            XmlNode n1 = parent.FirstChild;
            if (ignoreEmptyTexts) {
                while (IsText(n1) && string.Empty == n1.Value.Trim()) {
                    XmlNode n2 = n1.NextSibling;
                    if (n2 == null) {
                        break;
                    }
                    n1 = n2;
                }
            }
            return n1;
        }

        private static bool IsText(XmlNode n) {
            return n is XmlText || n is XmlCDataSection;
        }
    }
}
