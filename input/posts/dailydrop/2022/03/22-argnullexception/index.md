---
title: "ArgumentNullException improvements"
lead: "How its becoming easier to throw argument null exceptions"
Published: 03/22/2022
slug: "22-argnullexception"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - exception

---

## Daily Knowledge Drop

The way in which the validity of a method parameter, and the way the consequent exception is thrown, has evolved over time and has become a lot simpler and cleaner to do.

Today we'll look at the evolution of the `ArgumentNullException`.

---

## Manual check and throw

The first example is probably the most familiar way to do validate - check the value and then throw an exception if the validation passes.

``` csharp
void ManualCheckAndThrow(ParameterClass param)
{
    if(param == null)
    {
        throw new ArgumentNullException("param");
    }

    Console.WriteLine($"The parameter name is: {param.Name}");
}
```

---

## Check and throw

An improvement was included with .NET6, which is a method on `ArgumentNullException` itself, which performs the validation and only throws the exception if the value is null.

``` csharp
void ManualThrowIfNull(ParameterClass param)
{
    ArgumentNullException.ThrowIfNull(param, "param");

    Console.WriteLine($"The parameter name is: {param.Name}");
}
```

This code is much cleaner than the first example, however its still a manual process to initialize the check.

---

## Automatic parameter checking

`Update 20-04-2022:` This feature has been remove from .NET 7, but may be introduced in future version. See "[Remove parameter null-checking from C# 11](https://devblogs.microsoft.com/dotnet/csharp-11-preview-updates/#remove-parameter-null-checking-from-c-11)"  

.NET7 (currently only .NET7 Preview 1 - so things might still change) introduced the ability to mark the parameter as `non-nullable`. If the value is null, the `ArgumentNullException` is automatically thrown.

``` csharp
void AutoNullCheck(ParameterClass param!!)
{
    Console.WriteLine($"The parameter name is: {param.Name}");
}
```

The parameter value is marked with a double exclamation mark `!!`. This is the indicator that an `ArgumentNullException` should be thrown automatically if the value is null.

---

## !! lowering

Having a look at how the `!!` operator is lowered, using [sharplab.io](https://sharplab.io), we can see that the complier is converting the `!!` operator into an explicit check of the parameter, and throwing the exception if null.

So this method:

``` csharp
void AutoNullCheck(ParameterClass param!!)
{
    Console.WriteLine($"The parameter name is: {param.Name}");
}
```

Gets lowered to this (some non-related portions of code have been omitted):

``` csharp
private void AutoNullCheck(ParameterClass param)
{
    <PrivateImplementationDetails>.ThrowIfNull(param, "param");
    Console.WriteLine(string.Concat("The parameter name is: ", param.Name));
}

[CompilerGenerated]
internal sealed class <PrivateImplementationDetails>
{
    internal static void Throw(string paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    internal static void ThrowIfNull(object argument, string paramName)
    {
        if (argument == null)
        {
            Throw(paramName);
        }
    }
}
```

---

## Notes

There has been a fair amount of negative feedback to the introduction of the `!!` operation, as seen [here at the C#11 features post.](https://devblogs.microsoft.com/dotnet/early-peek-at-csharp-11-features/)

Personally, I don't mind the new operator - if unfamiliar with it, it is not initially obvious what it does or how it's effecting the code, but I feel this is a minor issue that can easily be overcome with online search and reading through the docs. The operator does improve the readability and cleanliness of the code.


---

<?# DailyDrop ?>35: 22-03-2022<?#/ DailyDrop ?>
