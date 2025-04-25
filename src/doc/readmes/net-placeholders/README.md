# XMLUnit.NET

XMLUnit provides you with the tools to verify the XML you emit is the
one you want to create.

This package provides a way to simplify comparisons by using a control
document with a simple expression language to compare against.

## Requirements

XMLUnit requires .NET Standard 2.0 (tested with .NET 8 rigt now) and
should still support .NET Framework 3.5 and Mono.

The placeholders package only depends on XMLUnit.Core.

## Usage

More documentaion is available as part of
the [user guide](https://github.com/xmlunit/user-guide/wiki).

If you are creating documents with a structure like

```xml
<message>
  <id>12345</id>
  <content>Hello</content>
</message>
```

and can't predict the `id` inside your tests but still want to assert
it is a number, using just the core library will require some custom
code as a `IDifferenceEvaluator`

Using the placeholders package you can write a control document like

```xml
<message>
  <id>${xmlunit.isNumber}</id>
  <content>Hello</content>
</message>
```

and run the test like

```csharp
string control = <the above>;
string test = <the program output>;
Diff diff = DiffBuilder.Compare(control).WithTest(test)
    .WithDifferenceEvaluator(new PlaceholderDifferenceEvaluator()).build();
Assert.IsFalse(d.HasDifferences());
```

Currently the fillowing placeholders are defined:

* `${xmlunit.ignore}` to completely ignore the element
* `${xmlunit.isNumber}`
* `${xmlunit.matchesRegex()}` with regex parameter
* `${xmlunit.isDateTime()}` with optional format parameter

## Additional Documentation

XMLUnit.NET is developed at
[github](https://github.com/xmlunit/xmlunit.net). More documentation,
releases and an issue tracker can be found there.

## Changelog

See the [Release
Notes](https://github.com/xmlunit/xmlunit.net/blob/main/RELEASE_NOTES.md)
at github.
