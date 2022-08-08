---
title: "Namespace-less types"
lead: "Reference types without a namespace and handling naming conflicts"
Published: "08/29/2022 01:00:00+0200"
slug: "29-global-namespace"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - global
   - namespace

---

## Daily Knowledge Drop

Types in C# `do not have to belong to a specific namespace` - it is just standard convention that they do, however this is not a requirement. The `global` alias, along with the namespace alias qualifier `::` can be used to reference types without a namespace in the event of a _naming conflict_.

---

## Usage

Consider the following basic example - a simple wrapper class, _belonging to a namespace_, which will output a string to the console:

``` csharp
namespace GlobalKeyword;

public class ConsoleWrapper
{
    public static void OutputToConsole(string output)
    {
        Console.WriteLine(output);
    }
}
```

The usage is straight forward:

``` csharp
namespace GlobalKeyword;

public class Program
{
    public static void Main()
    {
        ConsoleWrapper.OutputToConsole(
            $"Written to console by instance of '{nameof(ConsoleWrapper)}'");
    }
}
```

Suppose we reference a Nuget package in our project - it just so happens the require functionality is contained in the package, in a class also called `ConsoleWrapper`, however, it `doesn't contain a namespace`:

``` csharp
public  class ConsoleWrapper
{
    public static void Output(string output)
    {
        Console.WriteLine(output);
    }
}
```

If we try reference the new class in our _Main_ method:

``` csharp
namespace GlobalKeyword;

public static void Main()
{
    // old class in same namespace as Main
    ConsoleWrapper.OutputToConsole(
        $"Written to console by instance of '{nameof(ConsoleWrapper)}'");

    // new class without a namespace
    ConsoleWrapper.Output(
        $"Written to console by instance of '{nameof(GlobalKeyword.ConsoleWrapper)}'");
    
}
```

Then the following compile-time error occurs:

``` terminal
'ConsoleWrapper' does not contain a definition for 'Output'
```

As the newly introduced  _ConsoleWrapper_ has no namespace, we cannot fully qualify it to resolve the conflict. However, as it has no namespace, it gets put into the `global namespace` which can be referenced using `global::`:

``` csharp
namespace GlobalKeyword;

public static void Main()
{
    // old class in same namespace as Main
    ConsoleWrapper.OutputToConsole(
        $"Written to console by instance of '{nameof(ConsoleWrapper)}'");

    // new class in no/global namespace
    global::ConsoleWrapper.Output(
        $"Written to console by instance of '{nameof(GlobalKeyword.ConsoleWrapper)}'");
    
}
```

**Problem solved!**

---

## Notes

This is a very rare issue to encounter, as it requires two unlikely scenarios to occur:
- A type to not have a namespace
- The type without a namespace to have the same name as another type (either user defined, or reference)

The vast majority of types do have namespaces, so the chances of encountering this issue are small - however, if it is encountered, the `global` alias can be used to resolve the conflict.

---

## References

[:: operator (C# reference)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/namespace-alias-qualifier)   

---

<?# DailyDrop ?>148: 29-08-2022<?#/ DailyDrop ?>
