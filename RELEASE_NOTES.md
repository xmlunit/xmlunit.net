# Release Notes

## XMLUnit for Java 2.1.0 - /No released, yet/

* added `CompareConstraint.WithNamespaceContext`
  port of PR [#54](https://github.com/xmlunit/xmlunit/pull/54) by
  [@cboehme](https://github.com/cboehme).

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
