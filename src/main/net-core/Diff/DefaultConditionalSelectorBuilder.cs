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
using System.Linq;
using System.Xml;

namespace Org.XmlUnit.Diff {

    internal class DefaultConditionalSelectorBuilder
        : ElementSelectors.IConditionalSelectorBuilder,
        ElementSelectors.IConditionalSelectorBuilderThen {

        private ElementSelector defaultSelector;
        private readonly IList<KeyValuePair<Predicate<XmlElement>, ElementSelector>> conditionalSelectors =
            new List<KeyValuePair<Predicate<XmlElement>, ElementSelector>>();
        private Predicate<XmlElement> pendingCondition;

        public ElementSelectors.IConditionalSelectorBuilder ThenUse(ElementSelector es) {
            if (pendingCondition == null) {
                throw new InvalidOperationException("missing condition");
            }
            conditionalSelectors.Add(new KeyValuePair<Predicate<XmlElement>, ElementSelector>(pendingCondition, es));
            pendingCondition = null;
            return this;
        }
        public ElementSelectors.IConditionalSelectorBuilderThen When(Predicate<XmlElement> predicate) {
            if (pendingCondition != null) {
                throw new InvalidOperationException("unbalanced conditions");
            }
            pendingCondition = predicate;
            return this;
        }
        public ElementSelectors.IConditionalSelectorBuilder ElseUse(ElementSelector es) {
            if (defaultSelector != null) {
                throw new InvalidOperationException("can't have more than one default selector");
            }
            defaultSelector = es;
            return this;
        }
        public ElementSelectors.IConditionalSelectorBuilderThen WhenElementIsNamed(string expectedName) {
            return When(ElementSelectors.ElementNamePredicate(expectedName));
        }
        public ElementSelectors.IConditionalSelectorBuilderThen WhenElementIsNamed(XmlQualifiedName expectedName) {
            return When(ElementSelectors.ElementNamePredicate(expectedName));
        }
        public ElementSelector Build() {
            if (pendingCondition != null) {
                throw new InvalidOperationException("unbalanced conditions");
            }
            return new DefaultCondionalSelector(conditionalSelectors, defaultSelector).CanBeCompared;
        }

        internal class DefaultCondionalSelector {
            private readonly IEnumerable<KeyValuePair<Predicate<XmlElement>, ElementSelector>> conditionalSelectors;
            private readonly ElementSelector defaultSelector;

            internal DefaultCondionalSelector(IEnumerable<KeyValuePair<Predicate<XmlElement>,
                                              ElementSelector>> conditionalSelectors,
                                              ElementSelector defaultSelector) {
                this.conditionalSelectors = conditionalSelectors.ToArray();
                this.defaultSelector = defaultSelector;
            }

            public bool CanBeCompared(XmlElement control, XmlElement test) {
                foreach (KeyValuePair<Predicate<XmlElement>, ElementSelector> p in
                         conditionalSelectors) {
                    if (p.Key(control)) {
                        return p.Value(control, test);
                    }
                }
                if (defaultSelector != null) {
                    return defaultSelector(control, test);
                }
                return false;
            }
        }
    }
}

