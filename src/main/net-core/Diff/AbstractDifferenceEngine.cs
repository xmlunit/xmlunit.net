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
    /// Useful base-implementation of some parts of the
    /// IDifferenceEngine interface.
    /// </summary>
    public abstract class AbstractDifferenceEngine : IDifferenceEngine {
        public event ComparisonListener ComparisonListener;
        public event ComparisonListener MatchListener;
        public event ComparisonListener DifferenceListener;

        private INodeMatcher nodeMatcher = new DefaultNodeMatcher();
        public virtual INodeMatcher NodeMatcher {
            set {
                if (value == null) {
                    throw new ArgumentNullException("node matcher");
                }
                nodeMatcher = value;
            }
            get {
                return nodeMatcher;
            }
        }

        private DifferenceEvaluator diffEvaluator = DifferenceEvaluators.Default;
        public virtual DifferenceEvaluator DifferenceEvaluator {
            set {
                if (value == null) {
                    throw new ArgumentNullException("difference evaluator");
                }
                diffEvaluator = value;
            }
            get {
                return diffEvaluator;
            }
        }

        private ComparisonController comparisonController = ComparisonControllers.Default;
        public virtual ComparisonController ComparisonController {
            set {
                if (value == null) {
                    throw new ArgumentNullException("comparison controller");
                }
                comparisonController = value;
            }
            get {
                return comparisonController;
            }
        }

        public abstract void Compare(ISource control, ISource test);

        private IDictionary<string, string> namespaceContext;

        public IDictionary<string, string> NamespaceContext {
            set {
                namespaceContext = value == null ? value
                    : new Dictionary<string, string>(value);
            }
            protected get {
                return namespaceContext == null ? namespaceContext
                    : new Dictionary<string, string>(namespaceContext);
            }
        }

        /// <summary>
        /// Compares the detail values for object equality, lets the
        /// difference evaluator evaluate the result, notifies all
        /// listeners and returns the outcome.
        /// </summary>
        protected internal KeyValuePair<ComparisonResult, bool> Compare(Comparison comp) {
            object controlValue = comp.ControlDetails.Value;
            object testValue = comp.TestDetails.Value;
            bool equal = controlValue == null
                ? testValue == null : controlValue.Equals(testValue);
            ComparisonResult initial =
                equal ? ComparisonResult.EQUAL : ComparisonResult.DIFFERENT;
            ComparisonResult altered = DifferenceEvaluator(comp, initial);
            FireComparisonPerformed(comp, altered);
            bool stop = false;
            if (altered != ComparisonResult.EQUAL) {
                stop = ComparisonController(new Difference(comp, altered));
            }
            return new KeyValuePair<ComparisonResult, bool>(altered, stop);
        }

        /// <summary>
        /// Returns a function that compares the detail values for
        /// object equality, lets the difference evaluator evaluate
        /// the result, notifies all listeners and returns the
        /// outcome.
        /// </summary>
        protected internal Func<KeyValuePair<ComparisonResult, bool>>
            Comparer(Comparison comp) {
            return () => Compare(comp);
        }

        private void FireComparisonPerformed(Comparison comp,
                                             ComparisonResult outcome) {
            ComparisonListener cl = ComparisonListener;
            ComparisonListener ml = MatchListener;
            ComparisonListener dl = DifferenceListener;
            if (cl != null) {
                cl(comp, outcome);
            }
            if (outcome == ComparisonResult.EQUAL && ml != null) {
                ml(comp, outcome);
            } else if (outcome != ComparisonResult.EQUAL && dl != null) {
                dl(comp, outcome);
            }
        }

        protected static string GetXPath(XPathContext ctx) {
            return ctx == null ? null : ctx.XPath;
        }

        /// <summary>
        /// Chain of comparisons where the last comparision performed
        /// determines the final result but the chain stops as soon as the
        /// comparison controller says so.
        /// </summary>
        protected class ComparisonChain {
            private KeyValuePair<ComparisonResult, bool> currentResult;
            internal ComparisonChain()
                : this(new KeyValuePair<ComparisonResult, bool>(ComparisonResult.EQUAL, false)) {
            }
            internal ComparisonChain(KeyValuePair<ComparisonResult, bool> firstResult) {
                currentResult = firstResult;
            }
            internal ComparisonChain AndThen(Func<KeyValuePair<ComparisonResult, bool>> next) {
                if (!currentResult.Value) {
                    currentResult = next();
                }
                return this;
            }
            internal ComparisonChain AndIfTrueThen(bool evalNext,
                                                   Func<KeyValuePair<ComparisonResult, bool>> next) {
                return evalNext ? AndThen(next) : this;
            }
            internal KeyValuePair<ComparisonResult, bool> FinalResult {
                get {
                    return currentResult;
                }
            }
        }
    }
}
