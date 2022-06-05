---
title: "Roslyn Analyzer - testing an analyzer and code fix (Part 4)"
lead: "A step by step guide to testing an analyzer and code fix"
Published: 11/27/2021
slug: "analyzer-test"
draft: false
toc: true
categories:
    - Blog
    - Series
tags:
    - c#
    - .net
    - roslyn
    - codefix
    - code fix
    - analyser
    - analyzer
    - guide
    - entity framework
    - entityframework
    - ef
    - analyzer-series    
---

All posts in the series:  
**Part 1:** [Roslyn Analyzer - explained](../analyzer-explained)    
**Part 2:** [Roslyn Analyzer - writing an analyzer](../analyzer-write/)  
**Part 3:** [Roslyn Analyzer - writing a code fix](../analyzer-code-fix/)  
**Part 4:** Roslyn Analyzer - testing an analyzer and code fix (this post)   
**Part 5:** [Roslyn Analyzer - tips and tricks](../analyzer-extra/) 

All code in the posts, including the sample project and working `analyzer` and `code fix` are [available on Github](https://github.com/always-developing/CodeAnalysis.EntityFrameworkCore.Sample).

## Analyzer unit test introduction

The previous posts in the series detail how to [write an analyzer](../analyzer-write) and [code fix](../analyzer-code-fix).

This post details writing unit tests to help ensure the stability of the code, but also aid in the development process by providing a quick and easy way to debug and test the `analyzer` and `code fix`.

---

## Why write unit tests?

`Analyzers` are not simple to test - to "run" an analyzer, a new instance of Visual Studio starts up with the `analyzer` installed as an extension. An application (which has the code needed to test the `analyzer`) then needs to be opened to cause the `analyzer` trigger.   
While this definitely has a place when testing (hence the suggestion of [creating a sample application for the analyzer](../analyzer-write/#a-sample-application)), this process to often be inconsistent, with the updated `analyzer` not always being installed in the new instance of Visual Studio, or the breakpoints in the `analyzer` not being hit.  

Unit tests provide a convenient and comparatively quick way to debug and iterate while coding the `analyzer` and `code fix`.

Luckily, writing unit tests are fairly straight forward. In addition a test framework is available for the testing of `analyzers` and `code fixes`.

---

## Default unit tests

### Wrapper classes
As part of the `analyzer` template, a unit test project is automatically created.  
This template has:
- A sample `analyzer` test, using the a VerifyCS._VerifyAnalyzerAsync_ method
- A sample verify `code fix` test, using a VerifyCS._VerifyCodeFixAsync_ method

The _VerifyCS_ class is an auto-generated class, which wraps a lot of the complexity of the underlying testing framework classes - while this is great when first working with `analyzers` and is easy to use for simple use cases, more complex use cases require more configuration and its generally easier to just use the underlying wrapped classes directly.  

Using the _VerifyCS_ class to test an `analyzer` is straightforward though:
1. Define a block of code as a string
1. Define the list of diagnostic result the code should produce (and the location in the code)
1. Call _VerifyCS.VerifyAnalyzerAsync()_

``` csharp
//No diagnostics expected to show up
[TestMethod]
public async Task TestMethod1()
{
    var test = @"";

    // No code, so no diagnostic will be triggered
    await VerifyCS.VerifyAnalyzerAsync(test);
}
```

Using the _VerifyCS_ class to test a `code fix`:
1. Define a initial state block of code as a string
1. Define the list of diagnostic result the code should produce (and the location in the code)
1. Define a final state block of code as a string (what the code would look like after the code fix has been applied)
1. Call _VerifyCS.VerifyCodeFixAsync()_

``` csharp
//Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task TestMethod2()
    {
        // define the initial code block
        var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class {|#0:TypeName|}
    {   
    }
}";

        // define the final state code block
        var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class TYPENAME
    {   
    }
}";
        // Expected diagnostics to be triggered
        var expected = VerifyCS.Diagnostic("Analyzer1")
            .WithLocation(0).WithArguments("TypeName");

        // Verify the diagnostic will be triggered, 
        // and that the code fix applies successfully
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
```

---

### Complex use cases

There are a few use cases where I've found __NOT__ using _VerifyCS_ easier (although it is still possible to use it):
- If the code block requires external dependencies to compile (such as a NuGet package)
- If the code block is using some of the .NET 6 features (such as the minimal startup with the implicit main method)
- If the `analyzer` requires additional files, such as an appsettings.json file.
- If the build config (or any preprocessor symbol) makes a different to the analyzer

For these reasons, I generally `do not use VerifyCS`, but use the underlying framework classes directly instead.

---

## Enhanced unit tests

The steps for using the framework classes directly (_CSharpAnalyzerTest_) are similar to using the wrapper class:

Using the _CSharpAnalyzerTest_ class to test an analyzer:
1. Define a block of code as a string
1. Define the list of diagnostic result the code should produce (and the location in the code)
1. Define any additional configuration
1. Call _RunAsync()_

To test a code fix using _CSharpCodeFixTest_:
1. Define a initial state block of code as a string
1. Define the list of diagnostic result the code should produce (and the location in the code)
1. Define a final state block of code as a string (what the code would look like after the code fix has been applied)
1. Define any additional configuration for both the initial and final state
1. Call _RunAsync()_

Lets go through these steps in details in the next sections.

---

### Defining the code
#### Analyzer test code
Although the analyzer and code fix test use different test classes, the setup is very similar.

With the configuration for an `analyzer`, the _TestState_ is set - the __sourceCode__ variable is a string with C# code as text.
``` csharp
var analyzerTest =  new CSharpAnalyzerTest<DevOnlyMigrateAnalyzer, MSTestVerifier>
{
    TestState =
    {
        Sources = { sourceCode }
    }
};
```    

#### Code fix test code
With the configuration for a `code fix`, the _TestState_ is set, as well as the source code for the expected _FinalState_. The final state is the expected code after the code fix has been applied. Again, both __sourceCode__ and __fixedCode__ are C# code as text.
``` csharp
var analyzerFix = new CSharpCodeFixTest<DevOnlyMigrateAnalyzer, 
    DevOnlyMigrateCodeFixProvider, MSTestVerifier>
{
    TestState =
    {
        Sources = { sourceCode }
    },
    FixedState =
    {
        Sources = { fixCode }
    }
};
```     

---

### Defining the diagnostics

Next up is to define the diagnostics we expect the code to trigger.  

#### Analyzer diagnostics
With an `analyzer` test, the diagnostic id, severity and location of the expected diagnostics is specified:
``` csharp
analyzerTest.ExpectedDiagnostics.Add(
    new DiagnosticResult(
        "ADEF001", 
        Microsoft.CodeAnalysis.DiagnosticSeverity.Warning
    ).WithLocation(18, 27));
```   

#### Code fix diagnostics
With a `code fix`, if the expectation is that there will still be diagnostics after the code fix has been applied, the ExpectedDiagnostics is set on the __FixedState__:
``` csharp
analyzerFix.FixedState.ExpectedDiagnostics.Add(
    new DiagnosticResult(
        "ADEF001", 
        Microsoft.CodeAnalysis.DiagnosticSeverity.Warning
    ).WithLocation(18, 27));
```

No or multiple expected diagnostics can be specified.

---

### Additional configuration

#### .NET6.0 support

If the __sourceCode__ (a string representation of C# code) contains any features specific to .NET6.0 (such as the no longer required Main method), the setup below needs to be done.  

This specifies to the testing framework to include the additional package as part of the code when executing the analyzer:
``` csharp
var analyzerTest =  new CSharpAnalyzerTest<ConfigConnectionStringAnalyzer, 
    MSTestVerifier>
{
    TestState =
    {
        Sources = { sourceCode },
        ReferenceAssemblies = new ReferenceAssemblies(
            "net6.0", 
            new PackageIdentity(
                "Microsoft.NETCore.App.Ref", "6.0.0"), 
                Path.Combine("ref", "net6.0"))
    }
};
``` 

For a `code fix` test, the same needs to be applied to the __FinalState__ if it makes use of the same .NET6.0 specific functionality.

#### Nuget Packages

Sometimes additional packages are required for the _sourceCode_ to successfully compile. In the sample code, for example, the EntityFramework Core references.

The required package names and version are specified and then added to the _TestState_.
``` csharp

// include any nuget packages to reduce the number of errors
var packages = new[] {
    new PackageIdentity("Microsoft.Extensions.Hosting", "6.0.0"),
    new PackageIdentity("Microsoft.Extensions.Configuration", "6.0.0"),
    new PackageIdentity("Microsoft.EntityFrameworkCore", "6.0.0"),
    new PackageIdentity("Microsoft.EntityFrameworkCore.Sqlite", "6.0.0")
}
.ToImmutableArray();

var analyzerTest =  new CSharpAnalyzerTest<DevOnlyMigrateAnalyzer, MSTestVerifier>
{
    TestState =
    {
        Sources = { sourceCode },
        ReferenceAssemblies = new ReferenceAssemblies(
            "net6.0", 
            new PackageIdentity(
                "Microsoft.NETCore.App.Ref", "6.0.0"), 
                Path.Combine("ref", "net6.0"))
            .AddPackages(packages)
    }
};

```

<?# InfoBlock ?>
Adding the packages to the tests is NOT required - if not added, the code simply wont compile, with the error: **_The type or namespace name 'XXX' does not exist in the namespace_** ...  
These errors could be added to the _ExpectedDiagnostics_ collection and as the test now expects these to occur, the test will pass.  

However the easier and more complete solution, is to rather just add the required packages instead of trying to cater for diagnostics not related to the `analyzer` or `code fix` being tested.
<?#/ InfoBlock ?>


#### Additional files

Sometimes an analyzer will require additional files to successfully perform its function - such as checking the contents of the appsettings.json. To successfully be able to test this, additional files (names, and content) can be configured as part of the test.

This is done on the _TestState_ or _FixedState_:

In the below sample, an additional file called `appsettings.json`, with `empty json` contents, is added to the test state.

``` csharp
analyzerTest.TestState
    .AdditionalFiles.Add(("appsettings.json", "{}"));
```

#### Build configuration

In some use cases, such as one in the sample, the build configuration of the project makes a difference to how the analyzer performs.  

To specify the build configuration, or any preprocessor symbols, the following is used:

``` csharp
analyzerTest.SolutionTransforms.Add((s, p) =>
{
    return s.WithProjectParseOptions(p, 
        new CSharpParseOptions()
            .WithPreprocessorSymbols("DEBUG"));
});
```

---

## Next steps: Tips and tricks

The next and final [part in the series](../analyzer-extra/) will provider some collection of tips and tricks collected while working with `analyzers`.

## Useful links
[Roslyn repository](https://github.com/dotnet/roslyn)  
[Sample analyzer and code fix repository](https://github.com/always-developing/CodeAnalysis.EntityFrameworkCore.Sample)




