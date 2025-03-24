XMLUnit.NET 2.x
===============

[![nuget](https://img.shields.io/nuget/v/XMLUnit.Core.svg)](https://www.nuget.org/packages/XMLUnit.Core/)

Builds:
  * AppVeyor using .NET Framework 3.5 and .NET > .NET Core >= 2.0 on
    Windows, .NET > .NET Core >= 2.0 on Linux: [![Build
    status](https://ci.appveyor.com/api/projects/status/am34dfbr4vbcarr3?svg=true)](https://ci.appveyor.com/project/bodewig/xmlunit-net)

XMLUnit is a library that supports testing XML output in several ways.

Some goals for XMLUnit 2.x:

* create .NET and Java versions that are compatible in design while
  trying to be idiomatic for each platform
* remove all static configuration (the old XMLUnit class setter methods)
* focus on the parts that are useful for testing
  - XPath
  - (Schema) validation
  - comparisons
* be independent of any test framework

## Documentation

* [Developer Guide](https://github.com/xmlunit/xmlunit/wiki)
* [User's Guide](https://github.com/xmlunit/user-guide/wiki)

## Help Wanted!

If you are looking for something to work on, we've compiled a
[list](https://github.com/xmlunit/xmlunit/blob/main/HELP_WANTED.md) of known issues.

Please see the [contributing guide](CONTRIBUTING.md) for details on
how to contribute.

## Latest Release

The latest releases are available as
[GitHub releases](https://github.com/xmlunit/xmlunit.net/releases) or
via [nuget](https://www.nuget.org/packages/XmlUnit.Core/).  *Note:*
Due to a glitch in the `nuspec` files the package ids for the alpha-02
release are wrong, we began using XMLUnit.Core and XMLUnit.Constraints
(with capital M and L) with the subsequent releases.

## SNAPSHOT builds

NuGet packages are available from out CI builds at
[AppVeyor](https://ci.appveyor.com/project/bodewig/xmlunit-net).
Follow the link to the Debug or Release Configuration and then the
link to Artifacts.

## Examples

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

or using the `CompareConstraint`

```csharp
Assert.That(CreateTestDocument(), CompareConstraint.IsIdenticalTo(Input.FromFile("test-data/good.xml")));
```

### Asserting an XPath Value

```csharp
ISource source = Input.FromString("<foo>bar</foo>").Build();
IXPathEngine xpath = new XPathEngine();
IEnumerable<XmlNode> allMatches = xpath.SelectNodes("/foo", source);
string content = xpath.evaluate("/foo/text()", source);
```

or using `HasXPathConstraint` and `EvaluateXPathConstraint`

```csharp
Assert.That("<foo>bar</foo>", HasXPathConstraint.HasXPath("/foo"));
Assert.That("<foo>bar</foo>", EvaluateXPathConstraint.HasXPath("/foo/text()",
                                                               Is.EqualTo("bar")));
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

or using `ValidationConstraint`

```csharp
Assert.That(CreateDocument(),
            new ValidationConstraint(Input.FromFile("local.xsd")));
```

## Requirements

XMLUnit requires .NET Standard 2.0 (tested with .NET 8 rigt now) and
should still support .NET Framework 3.5 and Mono.

The `core` library provides all functionality needed to test XML
output and hasn't got any dependencies.  It uses NUnit 3.x for its own
tests.  The core library is complemented by two libaries of NUnit
constraints targeting NUnit 2.x and 3.x respectively.

## Checking out XMLUnit.NET

XMLUnit.NET uses a git submodule for test resources it shares with
XMLUnit for Java.  You can either clone this repository using `git
clone --recursive` or run `git submodule update --init` inside
your fresh working copy after cloning normally.

If you have checked out a working copy before we added the submodule,
you'll need to run `git submodule update --init` once.

## Building

Currently there are two different build setups for .NET Framework 3.5
and .NET Standard 2.0. Both build from the same sources and run the
same tests but use separate solution and project files. The
constraints for NUnit 2.x are ony built as part of the .NET Framework
/ Mono build.

### .NET Framework / Mono 3.5

XMLUnit for .NET uses NuGet and `msbuild`/`xbuild` - or Visual Studio
2013. The correspondig solution and project files are suffixed with
`.NetFramework`.

When using Visual Studio the build should automatically refresh the
NuGet packages, build the `core` and `constraints` assemblies as well
as the unit test projects and run all NUnit tests.

When not using Visual Studio you need to [install
nuget](https://docs.microsoft.com/en-us/nuget/guides/install-nuget) as
well as `msbuild` or `xbuild`<sup>[1](#nuget-linux)</sup> and run

```sh
$ nuget restore XMLUnit.NET.NetFramework.sln
```

once to download the packages used by XMLUnit during the build (really
only NUnit right now).  After that you can run `msbuild` or `xbuild`
like

```sh
> msbuild /p:Configuration=Debug XMLUnit.NET.NetFramework.sln
```
```sh
$ xbuild /p:Configuration=Debug XMLUnit.NET.NetFramework.sln
```

which compiles `core` and `constraints`, builds the assemblies and
executes the NUnit tests.

<a name="nuget-linux">1</a>: In order to run `nuget` and `xbuild` on
Linux (or any other platform supported by Mono) you may need to install
Mono itself and the xbuild package (the deb packages are
`mono-complete` and `mono-xbuild`).  You'll need to download `nuget`
and finally, if you encounter "System.Net.WebException: Error getting
response stream" when running `nuget.exe`, you'll need to execute

```sh
$ mozroots --import --sync
```

to install the same certificates into your Mono truststore that are
trusted by Firefox' default installation.

### .NET Standard 2.0

XMLUnit for .NET uses the `dotnet` CLI - or Visual Studio
2027. In order to run the tests, .NET > .NET Core >= 2.0 is required
(see the section about DOTNET_ROLL_FORWARD at the bottom of this page
when using .NET later than Core 2.0).

When using Visual Studio the build should automatically refresh the
NuGet packages, build the `core` and `constraints` assemblies as well
as the unit test projects.

When not using Visual Studio you need to run

```sh
$ dotnet restore XMLUnit.NET.sln
```

once to download the packages used by XMLUnit during the build (really
only NUnit right now).  After that you can run `dotnet` like

```sh
> dotnet build XMLUnit.NET.sln -c Debug
```

which compiles `core`, `constraints` and `placeholder` and builds the
assemblies. In order to run the tests use

```sh
> dotnet test src/tests/net-core/XMLUnit.Core.Tests.csproj
> dotnet test src/tests/net-constraints-nunit3/XMLUnit.NUnit3.Constraints.Test.csproj
> dotnet test src/tests/net-placeholders/XMLUnit.Placeholders.Tests.csproj
```

You may need to specify
[`--roll-forward`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet#rollforward)
or
[`DOTNET_ROLL_FORWARD`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-environment-variables#dotnet_roll_forward)
to run the tests when using recent versions of .NET.
