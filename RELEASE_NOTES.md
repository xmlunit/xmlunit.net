# Release Notes

## XMLUnit.NET 2.9.0 - /Released 2020-10-30/

* `ISource` now extends `IDisposable` to allow releasing unmanaged
  resources used when building sources from files or URIs.

  This change is backwards incompatible if you are providing `ISource`
  implementations of your own.
  [#33](https://github.com/xmlunit/xmlunit.net/pull/33).

* `DefaultNodeMatcher` with multiple `ElementSelector`s could fail to
  find the best matches as the order of `ElementSelector`s should
  select them.
  Issue similar to [xmlunit/#197](https://github.com/xmlunit/xmlunit/issues/197)

## XMLUnit.NET 2.8.0 - /Released 2020-05-12/

This version contains a backwards incompatible change to the
`IPlaceholderHandler` interface that is part of the experimental
placeholders module: The `Evaluate` method now receives a variable
number of string arguments in addition to the textual content of the
element/attribute. This allows placeholders like
`${xmlunit.matchesRegex(some\s*regex)}`.

* add a new `${xmlunit.isDateTime}` placeholder
  [#31](https://github.com/xmlunit/xmlunit.net/pull/31) and
  [#32](https://github.com/xmlunit/xmlunit.net/pull/32) by
  [MilkyWare](https://github.com/MilkyWare)

* added a new `${xmlunit.matchesRegex(regex)}` placeholder
  based on Java PR [xmlunit/#178](https://github.com/xmlunit/xmlunit/issues/178) by
  [@Jazzyekim](https://github.com/Jazzyekim).

## XMLUnit.NET 2.7.2 - /Released 2020-03-08/

* the XPath values for comparisons resulting in `CHILD_LOOKUP`
  differences could be wrong when `NodeFilter`s were present.
  Issue [#29](https://github.com/xmlunit/xmlunit.net/issues/29)

## XMLUnit.NET 2.7.1 - /Released 2019-06-21/

* add a new `${xmlunit.isNumber}` placeholder
  Based on the Java PR
  [xmlunit/#154](https://github.com/xmlunit/xmlunit/pull/154) by
  [@NathanAtClarity](https://github.com/NathanAtClarity).

* the XPath values of a comparison should not be affected by any
  `NodeFilter` being in effect.
  Issue similar to [xmlunit/#156](https://github.com/xmlunit/xmlunit/issues/156)

## XMLUnit.NET 2.7.0 - /Released 2019-04-13/

This release is identical to 2.7.0-beta-01 with only the version
numbers changed.

## XMLUnit.NET 2.7.0-beta-01 - /Released 2019-04-10/

This is the first release supporting .NET Standard 2.0. If you still
want/need the assemblies targeting .NET Framework 3.5 they are still
part of the nuget packages as well and you shouldn't see any
differences when compared to XMLUnit.NET 2.6.0.

The NUnit 2.x constraints still only support the .NET Framework as
NUnit 2.x itself doesn't support .NET Standard either.

* The nuspec files now use `license` rather than the deprecated
  `licenseUrl`.

* The `core` and `placeholders` as well as the NUnit 3.x constraints
  nuget packages now contain assemblies built for .NET Standard 2.0.

  Issue [#21](https://github.com/xmlunit/xmlunit.net/issues/21) based
  on Pull Request [#28](https://github.com/xmlunit/xmlunit.net/pull/28)
  by [@shatl](https://github.com/shatl).

  As part of this the dependency of the NUnit 3.x constraints on NUnit
  has been bumped to 3.6.1 which was the first version supporting .NET
  Standard 2.0.

## XMLUnit.NET 2.6.0 - /Released 2018-04-22/

* add a new experimental module xmlunit-placeholders which aims to
  use `${xmlunit.FOO}` expressions inside of the control document to
  allow for a DSL-like approach of defining more complex tests.
  This initial seed only supports `${xmlunit.ignore}` which can be
  used to make XMLUnit ignore the element containing this text.

* fixed the message when `CompareConstraint` or
  `ValidationConstraints` (both NUnit 2.x and 3.x) as well as
  `EvaluateXPathConstraint` or `HasXPathConstraint` (only the NUnit
  3.x versions) pass but the assertion fails because the constraint
  itself was wrapped in a `Not` constraint.

* the NUnit 3.x `EvaluateXPathConstraint` failed to resolve the nested
  constraint, leading to erroneous messages if the assertion failed.
  Issue [#25](https://github.com/xmlunit/xmlunit.net/issues/25)

* the `XmlDocument` instances used internally in `Convert` and
  `Transformation` now get their `XmlResolver` property set to `null`
  by default - which happens to be the default value of .NET 4.5.2 and
  later anyway. This is in accordance with the [OWASP recommendations
  for XML eXternal Entity injection
  preventions](https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Prevention_Cheat_Sheet#.NET).

  This may be a breaking change and you may need to provide an
  explicit `XmlResolver` instance of your own if you need to load
  external entities.

  Issue [#27](https://github.com/xmlunit/xmlunit.net/issues/27).

* added a new `ISource` implementation
  `ElementContentWhitespaceStrippedSource` which is similar to
  `WhitespaceStrippedSource` but only affects text nodes that solely
  consist of whitespace and doesn't affect any other text nodes. Also
  added convenience `IgnoreElementContentWhitespace` methods to
  `DiffBuilder` and `CompareConstraint`.
  Issue similar to [xmlunit/#119](https://github.com/xmlunit/xmlunit/issues/119).

* the configured `NodeFilter` is now applied before comparing
  `XmlDocumentType` nodes.

  This change breaks backwards compatibility as the old behavior was
  to ignore `XmlDocumentType` when counting the children of the
  `XmlDocument` node but not when actually comparing the
  `XmlDocumentType`. Prior to this change if one document had a document
  type declaration but the other didn't, a `HAS_DOCTYPE_DECLARATION`
  difference was detected, this will no longer be the case now. If you
  want to detect this difference, you need to use a more lenient
  `NodeFilter` than `NodeFilters.Default`
  (i.e. `NodeFilters.AcceptAll`) but then you will see an additional
  `CHILD_NODELIST_LENGTH` difference.

  Issue [#26](https://github.com/xmlunit/xmlunit.net/issues/26).

## XMLUnit.NET 2.5.1 - /Released 2017-10-20/

* elements that only differed in namespace prefix resulted in a false
  `ELEMENT_TAG_NAME` difference when compared.
  Issue [#22](https://github.com/xmlunit/xmlunit.net/issues/22)

## XMLUnit.NET 2.5.0 - /Released 2017-09-03/

* `CommentLessSource`, `DiffBuilder#IgnoreComments` and
  `CompareConstraint#IgnoreComments` now all use XSLT version 2.0
  stylesheets in order to strip comments. New constructors and methods
  have been added if you need a different version of XSLT (in
  particular if you need 1.0 which used to be the default up to
  XMLUnit 2.4.0).
  Issue similar to [xmlunit/#99](https://github.com/xmlunit/xmlunit/issues/99).

## XMLUnit.NET 2.4.0 - /Released 2017-07-23/

* made `DefaultComparisonFormatter` more subclass friendly.
  Issue similar to [xmlunit/#93](https://github.com/xmlunit/xmlunit/issues/93).

## XMLUnit.NET 2.3.1 - /Released 2017-03-23/

* provided xml doc files for the release version and inside the nuget
  package.
  [user-guide/#11](https://github.com/xmlunit/user-guide/issues/11)

## XMLUnit.NET 2.3.0 - /Released 2016-11-12/

* `Validator` and `SchemaValidConstraint` now accept using `XmlSchema`
  instances for the schema when validating instance documents.
  Issue similar to [xmlunit/#89](https://github.com/xmlunit/xmlunit/issues/89).

## XMLUnit.NET 2.2.0 - /Released 2016-06-04/

* `Input.FromByteArray` and `Input.FromString` now return `ISource`s
  that can be used multiple times.
  Issue similar to [xmlunit/#84](https://github.com/xmlunit/xmlunit/issues/84).

## XMLUnit.NET 2.1.1 - /Released 2016-04-09/

* `CompareConstraint` and `ValidationConstraint` for NUnit2 threw
  `NullReferenceException`s when combined with another failing
  `Constraint`.
  Issue similar to [xmlunit/#81](https://github.com/xmlunit/xmlunit/issues/81).

## XMLUnit.NET 2.1.0 - /Released 2016-03-26/

* added `CompareConstraint.WithNamespaceContext`
  port of PR [#54](https://github.com/xmlunit/xmlunit/pull/54) by
  [@cboehme](https://github.com/cboehme).

* added new implementations inside `DifferenceEvaluators` for common
  tasks like changing the outcome for specific differences or ignoring
  changes inside the XML prolog.

* new `HasXPath` constraints that check for the existence of an XPath
  inside of a piece of XML or verify additional assertions on the
  XPath's stringified result.
  Port of corresponding matchers in XMLUnit for Java by
  [@mariusneo](https://github.com/mariusneo).

* `DiffBuilder.WithComparisonFormatter` now also fully applies to the
  `Difference`s contained within the `Diff`.
  Issue [xmlunit/#55](https://github.com/xmlunit/xmlunit/issues/55)

## XMLUnit.NET 2.0.0 - /Released 2016-03-06/

* implemented `DiffBuilder.WithComparisonFormatter` mentioned in user
  guide.
  Issue [xmlunit/#51](https://github.com/xmlunit/xmlunit/issues/51)

## XMLUnit.NET 2.0.0-alpha-04 - /Released 2016-02-06/

* the unused `SchemaURI` property of `Validator` has been removed.
* the mapping of `IDifferenceEngine.NamespaceContext` has been
  inverted from prefix -> URI to URI -> prefix in order to be
  consistent with the same concept in `IXPathEngine`.
* `Comparison` now also contains the XPath of the parent of the
  compared nodes or attributes which is most useful in cases of
  missing nodes/attributes because the XPath on one side is `null` in
  these cases.
  Issue [xmlunit/#48](https://github.com/xmlunit/xmlunit/issues/48)
  ported from PR [xmlunit/#50](https://github.com/xmlunit/xmlunit/pull/50)
  by [@eguib](https://github.com/eguib).

## XMLUnit.NET 2.0.0-alpha-03 - /Released 2015-12-13/

* fixed the nuget package name of the core library to now use
  XMLUnit.Core
* switched core tests to use to NUnit 3.x and provided a new library
  to support NUnit 3.x constraints.
  [#19](https://github.com/xmlunit/xmlunit.net/pull/19) by
  [@e-tobi](https://github.com/e-tobi)
* The XMLUnit.Constraints nuget package has been replaced with
  XMLUnit.NUnit2.Constraints and XMLUnit.NUnit3.Constraints
* The XMLUnit.NUnit2.Constraints nuget package now depends on NUNit 2.6.4 -
  which it has been compiled against - rather than 2.5.10.
* added new overloads to `IXPathEngine`
* fixed the XPath context used by the `ByXPath` element selector so
  that "." now refers to the current element.
  Issue [xmlunit/#39](https://github.com/xmlunit/xmlunit/issues/39)
* `ElementSelectors.ConditionalBuilder` now stops at the first
  predicate returning `true`, even if the associated `ElementSelector`
  returns false.
  Issue [xmlunit/#40](https://github.com/xmlunit/xmlunit/issues/40)

## XMLUnit.NET 2.0.0-alpha-02 - /Released 2015-11-21/

This is the initial alpha release of XMLUnit.NET.  We expect the API
to change for the next release based on user feedback.
