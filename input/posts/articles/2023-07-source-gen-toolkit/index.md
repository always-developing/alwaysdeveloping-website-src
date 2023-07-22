---
title: "Introducing the Source Generator Toolkit"
lead: "Source Generators made easier!"
Published: "2023-07-24 03:00:00+0200"
slug: "2023-07-source-gen-toolkit"
draft: false
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - sourcegenerator
    - roslyn
    - toolkit
    - sourcegeneratortoolkit
---

## Introduction

Ever wanted to try leverage _Roslyn Source Generators_, but the process seemed too complex? The [`Source Generator Toolkit`](https://www.nuget.org/packages/SourceGeneratorToolkit) aims to make this process easier, but providing two pieces of core functionality:
- `Code Generation`: this can be leveraged in conjunction with the Roslyn Source Generator process, or standalone and allows for the easy generation of C# source code using a _fluent builder pattern_.
- `Code Qualification`: this is leverage as part of the Roslyn Source Generator process, and make the process of _determining if a syntax node qualifies for source generation_ easier and more stream-lined.

---

## Reason for the library

The main aim of the code generation functionality of the library is to `remove as much of the hard-coded C# strings required when creating a source generator as possible`. Some hard-coded _strings of code_ will still be required for specific logic, however the main scaffolding of the classes, methods etc can be done through the library using a _fluent builder pattern_. This pattern makes it easy to build up the source code logically, with each component building on the previous one.

While the initial idea was for the `Source Generator Toolkit` to be used with _Roslyn Source Generators_, the code generation functionality can be leveraged outside of this process to just generate a string representation of C# code.

When creating a _Roslyn Source Generators_ in most cases, before source code is generated, _qualification checks_ need to be performed to determine _if_ the source code should be generated, and to determine the values to be used in the generated code. Here the aim of the `Source Generator Toolkit` was to make working the the syntax nodes and syntax tree as easy as possible, by `providing a variety of extension methods for qualification checks performed on syntax nodes`. The qualification check functionality is done, again, through a _fluent builder_ which allows for the qualification check to logically be built up - easily to maintain and easier to understand.

---

## Generating Code - outside of the Roslyn Source Generator process

First we'll look at how to generate c# source code, outside of the Roslyn Source Generator process.

The static `SourceGenerator` class is the starting point for building up the source code. No actual _true c# code_ is generated here - just a **formatted string representation of c# code**:

``` csharp
var strCode = SourceGenerator.GenerateSource(gen =>
{
    gen.WithFile("file1", file =>
    {
        file.WithNamespace("SampleNamespace", ns =>
        {
            ns.WithClass("SampleClass", cls => { });
        });
    });
});
```

The string output of the above being (the value of `strCode`):

``` csharp
namespace SampleNamespace
{
    [System.CodeDom.Compiler.GeneratedCode("SourceGeneratorToolkit", "0.0.0.1")]
    class SampleClass
    {
    }
}
```

As you can see from this simple example, defining the code is easy:
- start with a "file" (in this case, it is an "in-memory" file which will hold the defined c# code)
- the file contains a single `namespace`, _SampleNamespace_
- the namespace contains a single `class`, _SampleClass_

Each component is defined with its required properties (such as name in the above examples), and then an optional builder Action to define its child components.

---

## Generating Code - Roslyn Source Generator process (without ISyntaxReceiver)

Next, we look at leveraging the `Source Generator Toolkit` as _part of the Roslyn Source Generator process_ - but when the `ISyntaxReceiver` is not used. This use case is for the scenarios when the generated source code must always be output and is not reliant on a _qualification check_ being done.

When used in conjunction with a Source Generator, the `GenerateSource` extension method on the `GeneratorExecutionContext` class can be leveraged. 

The below example shows how to generate source code `without any information from a SyntaxReceiver` - see further down on how the `Source Generator Toolkit` can be used to generate code in conjunction with a `ISyntaxReceiver` implementation.

``` csharp
// a ISourceGenerator implementation
public class SampleGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        context.GenerateSource("file1", fileBuilder =>
        {
            fileBuilder.WithNamespace("SampleNamespace", nsBuilder =>
            {
                ns.WithClass("SampleClass", cls => { });
            });
        });
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // no ISyntaxReceiver implementation registered here
    }
}
```

In the case of a Source Generator, an actual file named `file1.cs` will be output as part of the generation process.

The output content of the file will be the same as in the previous example:

``` csharp
namespace SampleNamespace
{
    [System.CodeDom.Compiler.GeneratedCode("SourceGeneratorToolkit", "0.0.0.1")]
    class SampleClass
    {
    }
}

```

The _fluent builder pattern_ is leveraged to build up the source code in exactly the same manner as in the previous example above.

---

## Generating Code - Configuration

There is optional configuration which can be specified when generating the code using either of the above two methods (when calling the `GenerateSource` method). If no configuration is specified, the default configuration is used.

|Configuration Name|Description|Default Value|
|---|---|---|
|OutputGeneratedCodeAttribute|Flag to indicate if the `System.CodeDom.Compiler.GeneratedCode` attribute should be output with generated code. This attribute is used as an indicator to various tools that the code was auto generated|true|
|OutputDebuggerStepThroughAttribute|Flag to indicate if the `System.Diagnostics.DebuggerStepThrough` attribute should be output with generated code. When set to true, this attribute allows stepping into the generated code when debugging|false|

---

## Code Qualification - Roslyn Source Generator process (with ISyntaxReceiver implementation)

When using the .NET Roslyn Source Generator process, the actual _generation_ of the source is only one step of the process - the other step is determining _if the source should be generated in the first place_. This _qualification check_ is done in the `OnVisitSyntaxNode` method of the `ISyntaxReceiver` implementation.

The `OnVisitSyntaxNode` method takes a `SyntaxNode` as an argument (this is part of the normal Roslyn Source Generator process) - the `Source Generator Toolkit` provides an extension method (`NodeQualifiesWhen`) which accepts a _qualification builder_ which is used to determine if the SyntaxNode qualifies to have source code generated.

The _fluent builder_ pattern is again used to build up the qualification check for for the syntax:

``` csharp
class SampleClassSyntaxReceiver : ISyntaxReceiver
{
    public List<SyntaxReceiverResult> Results { get; set; } = new List<SyntaxReceiverResult>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        syntaxNode.NodeQualifiesWhen(Results, node =>
        {
            node.IsClass(c => c
                .WithName("SampleClass")
                .IsNotStatic()
                .IsPublic()
                .Implements("ISerializable")
            );
        });
    }
}
```

In the above example, if the qualification checks determines the node is:
- a class named `SampleClass` 
- which is public
- and not static
- and also implements `ISerializable`

then the specific `SyntaxNode` qualifies, and the _Results_ list will be populated and passed to the _Execute_ method of the generator.

A most complex, but _less practical_ example:

``` csharp
syntaxNode.NodeQualifiesWhen(Results, node =>
{
    node.IsClass(c => c
        .WithName("SampleClass")
        .IsNotStatic()
        .IsNotPrivateProtected()
        .IsPublic()
        .Implements("ISerializable")
        // the class must have the Obsolete attribute 
        .WithAttribute(a =>
        {
            a.WithName("Obsolete");
        })
        .WithMethod(m =>
        {
            // the class must have a method called "SampleMethod"
            m.WithName("SampleMethod")
            // which is async
            .IsAsync()
            // with the Obsolete attribute with a parameter in position 1 supplied
            .WithAttribute(a =>
            {
                a.WithName("Obsolete")
                .WithArgument(arg =>
                {
                    arg.WithPosition(1);
                });
            })
            // method must have a return type of Task
            .WithReturnType(typeof(Task));
        })
    );
});
```

---

## Code Generation - Roslyn Source Generator process (with ISyntaxReceiver implementation)

When generating code based on the output of the _qualification process_ (`OnVisitSyntaxNode` method in the `ISyntaxReceiver` implementation, shown above), the `Results` list is populated with the qualifying `SyntaxNode(s)`, and passed to the `Execute` method of the `ISourceGenerator` implementation.

Using the same `ISyntaxReceiver` implementation as above:

``` csharp
class SampleClassSyntaxReceiver : ISyntaxReceiver
{
    public List<SyntaxReceiverResult> Results { get; set; } = new List<SyntaxReceiverResult>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        syntaxNode.NodeQualifiesWhen(Results, node =>
        {
            node.IsClass(c => c
                .WithName("SampleClass")
                .IsNotStatic()
                .IsPublic()
                .Implements("ISerializable")
            );
        });
    }
}
```

For each qualifying node, the `Results` property is populated with the qualifying `SyntaxNode` in question.

Below is a sample of a `ISourceGenerator` which used the _Results_ output from the `OnVisitSyntaxNode` method to generate source code:

``` csharp
[Generator]
public class PartialMethodGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register our custom syntax receiver
        context.RegisterForSyntaxNotifications(() => new PartialClassSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver == null)
        {
            return;
        }

        PartialClassSyntaxReceiver syntaxReceiver = (PartialClassSyntaxReceiver)context.SyntaxReceiver;

        if (syntaxReceiver != null && syntaxReceiver.Results != null && syntaxReceiver.Results.Any())
        {
            foreach (SyntaxReceiverResult result in syntaxReceiver.Results)
            {
                // based on the qualification process
                // we know the qualifying node will be a class
                ClassDeclarationSyntax cls = result.Node.AsClass();

                context.GenerateSource($"{cls.GetNamespace()}_file", fileBuilder =>
                {
                    fileBuilder.WithNamespace($"{cls.GetNamespace()}", nsBuilder =>
                    {
                        nsBuilder.WithClass($"{cls.GetName()}_generated", clsBuilder =>
                        {
                            clsBuilder.AsPublic();

                            clsBuilder.WithMethod("Hello", "void", mthBuilder =>
                            {
                                mthBuilder.AsPublic()
                                .WithBody(@"Console.WriteLine($""Generator says: Hello"");");
                            });
                        });
                    });
                });
            }
        }
    }
}
```

- The `Initialize` method is used to register the custom `ISyntaxReceiver` implementation containing the qualification rules - this is part of the normal Roslyn source generation processes (not specific to the `Source Generator Toolkit`)
- The `GeneratorExecutionContext` parameter passed to the `Execute` method contains a _ISyntaxReceiver_ implementation property - `PartialClassSyntaxReceiver` in this example, which contains the _Results_ property with the qualifying SyntaxNode(s). A number of checks are performed to ensure the _SyntaxReceiver_ is not null, and that the _Results_ property on it is not null.
- The code then iterates over each `SyntaxReceiverResult` in the _Results_ property. In other words, _iterating through each qualifying node_
- The `AsClass` extension method (part of the `Source Generator Toolkit`) will convert the generic SyntaxNode to the specific syntax type ('ClassDeclarationSyntax' in this example)
- The `GenerateSource` extension method (again, part of the `Source Generator Toolkit`) then allows for the building up of the required source code as described above. However, now, instead of explicitly supplying the values for the code (the _file name_,  _namespace_ and _class name_ in this example), the provided extension methods are used to extract the values from the qualifying syntax node.
- In this example, the `GetNamespace` and `GetName` extension methods on `ClassDeclarationSyntax` are used to get the relevent details from the syntax to populate the generated source code

---

## Custom syntax qualifiers 

The `Source Generator Toolkit` allows for custom qualification checks using the `WithQualifyingCheck` method:

``` csharp
syntaxNode.NodeQualifiesWhen(Results, node =>
{
    node.WithQualifyingCheck(customNode =>
    {
        // completely un-useful check
        return customNode.ChildNodes().Count() == 10;
    });
});
```

Here instead of checking if the node is a _class or attribute_ for example, the qualification check is to see if the node contains 10 child nodes (a not very useful check)

## Future enhancements

The library is a work in progress, with common source generator functionality added initially, but with more to come over time.  Some future enhancements include (but not limited to):

- [ ] SyntaxNode `AsAttribute` extension method
- [ ] Additional extension methods to be used on _ClassDeclarationSyntax_ and _AttributeDeclarationSyntax_ to be leverage when doing code generation in a source generator
- [ ] Ability to determine qualification with generics and generic types
- [ ] Ability to determine qualification based on a code comment

Feel free to log a request or bug if you have a specific requirement and I'll try to implement asap.

---

## Useful links
[Source Generator Toolkit](https://www.nuget.org/packages/SourceGeneratorToolkit)  
[Debugging a source generator](https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022)


