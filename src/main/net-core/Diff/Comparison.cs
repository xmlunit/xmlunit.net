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

using System.Xml;

namespace Org.XmlUnit.Diff {

    /// <summary>
    /// Details of a single comparison XMLUnit has performed.
    /// </summary>
    public class Comparison {

        /// <summary>
        /// The details of a target (usually a representation of an
        /// XML node) that took part in the comparison.
        /// </summary>
        public sealed class Detail {
            private readonly XmlNode target;
            private readonly string xpath;
            private readonly string parentXpath;
            private readonly object value;

            internal Detail(XmlNode t, string x, object v, string parentX) {
                target = t;
                xpath = x;
                value = v;
                parentXpath = parentX;
            }

            /// <summary>
            /// The actual target.
            /// </summary>
            public XmlNode Target { get { return target; } }
            /// <summary>
            /// XPath leading to the target.
            /// </summary>
            public string XPath { get { return xpath; } }
            /// <summary>
            /// XPath leading to the target's parent.
            /// </summary>
            public string ParentXPath { get { return parentXpath; } }
            /// <summary>
            /// The value for comparison found at the current target.
            /// </summary>
            public object Value { get { return value; } }
        }

        private readonly Detail control, test;
        private readonly ComparisonType type;

        /// <summary>
        /// Encapsulates a comparison of two parts of the pieces of XML to compare.
        /// </summary>
        /// <param name="t">the type of comparison</param>
        /// <param name="controlTarget">part inside the control document</param>
        /// <param name="controlXPath">XPath of the part inside the control document</param>
        /// <param name="controlValue">value inside the control document</param>
        /// <param name="controlParentXPath">Parent XPath of the part inside the control document</param>
        /// <param name="testTarget">part inside the test document</param>
        /// <param name="testXPath">XPath of the part inside the test document</param>
        /// <param name="testValue">value inside the test document</param>
        /// <param name="testParentXPath">Parent XPath of the part inside the test document</param>
        public Comparison(ComparisonType t, XmlNode controlTarget,
                          string controlXPath, object controlValue, string controlParentXPath, 
                          XmlNode testTarget, string testXPath,
                          object testValue, string testParentXPath) {
            type = t;
            control = new Detail(controlTarget, controlXPath, controlValue, controlParentXPath);
            test = new Detail(testTarget, testXPath, testValue, testParentXPath);
        }

        /// <summary>
        /// The kind of comparison performed.
        /// </summary>
        public ComparisonType Type {
            get {
                return type;
            }
        }

        /// <summary>
        /// Details of the control target.
        /// </summary>
        public Detail ControlDetails {
            get {
                return control;
            }
        }

        /// <summary>
        /// Details of the test target.
        /// </summary>
        public Detail TestDetails {
            get {
                return test;
            }
        }


        /// <summary>
        /// Returns a string representation of this comparison using the
        /// given IComparisonFormatter.
        /// <param name="formatter"> the IComparisonFormatter to use</param>
        /// <return>a string representation of this comparison</return>
        /// </summary>
        public string ToString(IComparisonFormatter formatter) {
            return formatter.GetDescription(this);
        }

        /// <summary>
        /// Returns a string representation of this comparison using
        /// DefaultComparisonFormatter.
        /// <return>a string representation of this comparison</return>
        /// </summary>
        public override string ToString() {
            return ToString(new DefaultComparisonFormatter());
        }
    }
}
