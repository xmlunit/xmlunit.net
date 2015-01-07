XMLUnit.NET 2.x
===============

XMLUnit is a library that supports testing XML output in several ways.

This will be a work in progress for quite some time.  We are in the
process of migrating the - unpublished so far - XMLUnit 2.x from
sourceforge to github.  XMLUnit 1.x for Java and 0.x for .NET will
stay at [sourceforge](https://sourceforge.net/projects/xmlunit/).

## Help Wanted!

If you are looking for something to work on, we've compiled a
[list](https://github.com/xmlunit/xmlunit/blob/master/HELP_WANTED.md) of things that should be done before XMLUnit
2.0 can be released.

Please see the [contributing guide](CONTRIBUTING.md) for details on
how to contribute.

## Examples

These are some really small examples, more is to come in the [user guide](https://github.com/xmlunit/user-guide/wiki)

### Comparing Two Documents

```csharp
ISource control = Input.FromFile("test-data/good.xml").Build();
ISource test = Input.FromMemory(CreateTestDocument()).Build();
AbstractDifferenceEngine diff = new DOMDifferenceEngine();
diff.DifferenceListener += (comparison, outcome) => {
            Assert.Fail("found a difference: {}", comparison);
        };
diff.Compare(control, test);
```

### Asserting an XPath Value

```csharp
ISource source = Input.FromMemory("<foo>bar</foo>").Build();
IXPathEngine xpath = new XPathEngine();
IEnumerable<XmlNode> allMatches = xpath.SelectNodes("/foo", source);
string content = xpath.evaluate("/foo/text()", source);
```

### Validating a Document Against an XML Schema

```java
Validator v = Validator.ForLanguage(Languages.W3C_XML_SCHEMA_NS_URI);
v.SchemaSources = new ISource[] {
        Input.FromUri("http://example.com/some.xsd").Build(),
        Input.FromFile("local.xsd").Build()
    };
ValidationResult result = v.ValidateInstance(Input.FromDocument(CreateDocument()).Build());
bool valid = result.Valid;
IEnumerable<ValidationProblem> problems = result.Problems;
```

## Requirements

XMLUnit requires .NET 3.5 (it is known to work and actually is
developed on Mono 4).

The `core` library provides all functionality needed to test XML
output and hasn't got any dependencies.  It uses NUnit 2.x for its own
tests.  The core library is complemented by NUnit constraints.

## Building

XMLUnit for .NET builds using NAnt, run `nant -projecthelp` for the
available targets, but mainly you want to run

```sh
$ nant
```

in order to compile `core` and `constraints` and build the assemblies.

```sh
$ nant test
```

executes the NUnit tests.
