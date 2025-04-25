# XMLUnit.NET NUnit 3.x Constraints

XMLUnit provides you with the tools to verify the XML you emit is the
one you want to create.

This package provides Constraints on top of the XMLUnit.NET core
library to be used with NUnit 3.x. If you are using a different
version of NUUnit, please use the package for your version.

* [XMLUnit.Core - Core Library ![nuget](https://img.shields.io/nuget/v/XMLUnit.Core.svg)](https://www.nuget.org/packages/XMLUnit.Core/)
* [XMLUnit.NUnit2.Constraints - Constraints for NUnit 2.x ![nuget](https://img.shields.io/nuget/v/XMLUnit.NUnit2.Constraints.svg)](https://www.nuget.org/packages/XMLUnit.NUnit2.Constraints/)
* [XMLUnit.NUnit4.Constraints - Constraints for NUnit 4.x ![nuget](https://img.shields.io/nuget/v/XMLUnit.NUnit4.Constraints.svg)](https://www.nuget.org/packages/XMLUnit.NUnit4.Constraints/)

[![Build status](https://ci.appveyor.com/api/projects/status/am34dfbr4vbcarr3?svg=true)]

## Requirements

XMLUnit requires .NET Standard 2.0 (tested with .NET 8 rigt now) and
should still support .NET Framework 3.5 and Mono.

This Constraints package requires NUnit 3.x and XMLUnit.Core.

## Usage

These are some really small examples, more is available as part of the
[user guide](https://github.com/xmlunit/user-guide/wiki)

### Comparing Two Documents

```csharp
Assert.That(CreateTestDocument(), CompareConstraint.IsIdenticalTo(Input.FromFile("test-data/good.xml")));
```

### Asserting an XPath Value

```csharp
Assert.That("<foo>bar</foo>", HasXPathConstraint.HasXPath("/foo"));
Assert.That("<foo>bar</foo>", EvaluateXPathConstraint.HasXPath("/foo/text()",
```

### Validating a Document Against an XML Schema


```csharp
Assert.That(CreateDocument(),
            new ValidationConstraint(Input.FromFile("local.xsd")));
```

## Additional Documentation

XMLUnit.NET is developed at
[github](https://github.com/xmlunit/xmlunit.net). More documentation,
releases and an issue tracker can be found there.

## Changelog

See the [Release
Notes](https://github.com/xmlunit/xmlunit.net/blob/main/RELEASE_NOTES.md)
at github.
