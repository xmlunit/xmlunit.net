# XMLUnit.NET

XMLUnit provides you with the tools to verify the XML you emit is the
one you want to create.

It provides helpers to validate against an XML Schema, assert the
values of XPath queries or compare XML documents against expected
outcomes.

This package provides the core functionality and can be used
stand-alone. In addition there are libraries providing NUnit
constraints and a "placeholders" package that may simplify writing
comparison tests in certain cases.

* [XMLUnit.NUnit2.Constraints - Constraints for NUnit 2.x ![nuget](https://img.shields.io/nuget/v/XMLUnit.NUnit2.Constraints.svg)](https://www.nuget.org/packages/XMLUnit.NUnit2.Constraints/)
* [XMLUnit.NUnit3.Constraints - Constraints for NUnit 3.x ![nuget](https://img.shields.io/nuget/v/XMLUnit.NUnit3.Constraints.svg)](https://www.nuget.org/packages/XMLUnit.NUnit3.Constraints/)
* [XMLUnit.NUnit4.Constraints - Constraints for NUnit 4.x ![nuget](https://img.shields.io/nuget/v/XMLUnit.NUnit4.Constraints.svg)](https://www.nuget.org/packages/XMLUnit.NUnit4.Constraints/)
* [XMLUnit.Placeholders - simplifies comparisons for special cases ![nuget](https://img.shields.io/nuget/v/XMLUnit.Placeholders.svg)](https://www.nuget.org/packages/XMLUnit.Placeholders/)

[![Build status](https://ci.appveyor.com/api/projects/status/am34dfbr4vbcarr3?svg=true)]

## Requirements

XMLUnit requires .NET Standard 2.0 (tested with .NET 8 rigt now) and
should still support .NET Framework 3.5 and Mono.

The core library hasn't got any dependencies itself.

## Usage

These are some really small examples, more is available as part of the
[user guide](https://github.com/xmlunit/user-guide/wiki)

### Comparing Two Documents

```csharp
ISource control = Input.FromFile("test-data/good.xml").Build();
ISource test = Input.FromByteArray(CreateTestDocument()).Build();
IDifferenceEngine diff = new DOMDifferenceEngine();
diff.DifferenceListener += (comparison, outcome) => {
            Assert.Fail("found a difference: {}", comparison);
        };
diff.Compare(control, test);
```

or using the fluent builder API

```csharp
Diff d = DiffBuilder.Compare(Input.FromFile("test-data/good.xml"))
             .WithTest(CreateTestDocument()).Build();
Assert.IsFalse(d.HasDifferences());
```

### Asserting an XPath Value

```csharp
ISource source = Input.FromString("<foo>bar</foo>").Build();
IXPathEngine xpath = new XPathEngine();
IEnumerable<XmlNode> allMatches = xpath.SelectNodes("/foo", source);
string content = xpath.evaluate("/foo/text()", source);
```

### Validating a Document Against an XML Schema


```csharp
Validator v = Validator.ForLanguage(Languages.W3C_XML_SCHEMA_NS_URI);
v.SchemaSources = new ISource[] {
        Input.FromUri("http://example.com/some.xsd").Build(),
        Input.FromFile("local.xsd").Build()
    };
ValidationResult result = v.ValidateInstance(Input.FromDocument(CreateDocument()).Build());
bool valid = result.Valid;
IEnumerable<ValidationProblem> problems = result.Problems;
```

## Additional Documentation

XMLUnit.NET is developed at
[github](https://github.com/xmlunit/xmlunit.net). More documentation,
releases and an issue tracker can be found there.

## Changelog

See the [Release
Notes](https://github.com/xmlunit/xmlunit.net/blob/main/RELEASE_NOTES.md)
at github.
