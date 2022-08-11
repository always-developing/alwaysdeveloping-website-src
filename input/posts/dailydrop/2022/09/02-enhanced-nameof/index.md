---
title: "nameof enhancement in C# 11"
lead: "A look into C# 11 enhancement to the nameof expression"
Published: "09/02/2022 01:00:00+0200"
slug: "02-enhanced-nameof"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - C#11
   - nameof

---

## Daily Knowledge Drop

With C# 11 (coming towards the end of this year) the scope of the `nameof` expression is being extended, allowing it to be used inside an `attribute expression`.

Below examples written and tested using .NET 7-preview5, and as such may differ from the final release or subsequent preview releases.

---

## Sample

In the examples below, a simple `Attribute` which takes a single string in the constructor is used:

``` csharp
[AttributeUsage(AttributeTargets.Method)]
public class MethodAttr : Attribute
{
    private readonly string _parameterName;

    // constructor just takes a string
    public MethodAttr(string parameterName)
    {
        _parameterName = parameterName;
    }
}
```

Suppose we want to apply this attribute to a method, and specify the method `parameter name as the argument to the Attribute constructor`.

Below we'll look at how this is done prior to C# 11 and how it can be improved on with C# 11.

---

### Pre C# 11

Prior to C #11 when using the attribute, the parameter name had to be _hardcoded_ into the `MethodAttr constructor`:

``` csharp
public class NameOfClass
{
    public NameOfClass()
    {
        Console.WriteLine($"In constructor of {nameof(NameOfClass)}");
    }

    public void NameOfMethod()
    {
        Console.WriteLine($"In method {nameof(NameOfMethod)}");
    }

    // method parameter name hardcoded into the MethodAttr constructor
    [MethodAttr("The parameter name is 'stringParam'")]
    public void MethodWithParam(string stringParam)
    {
        // however, in the method nameof can be used to get 
        // the name of the parameter
        Console.WriteLine($"Value of parameter '{nameof(stringParam)}' is " +
            $"is {stringParam}");
    }
}
```

The `nameof` expression can be used successfully on the parameter name _in the method_ (`nameof(stringParam)`), however when it comes to the attribute, the name has to be hardcoded - the `nameof` expression cannot be used in this context.

The issue with this approach, is that if the variable _stringParam_ is renamed (either with _F2_, or _CTRL R+R_) then all `string value representations` of the variable name (such as in the attribute constructor) need to `manually be updated`. With `nameof`, a rename will ensure that all references (including the `nameof` references) are renamed - this results in a more consistent and accurate code base.

Prior to C# 11, there was no choice but to use the string value in the attribute - however this changes in C# 11.

---

### C# 11

With C# 11, the scope of `nameof` has been increased, and it can now be used in attribute parameters:

``` csharp
public class NameOfClass
{
    public NameOfClass()
    {
        Console.WriteLine($"In constructor of {nameof(NameOfClass)}");
    }

    public void NameOfMethod()
    {
        Console.WriteLine($"In method {nameof(NameOfMethod)}");
    }

    // method parameter name hardcoded into the MethodAttr constructor
    [MethodAttr($"The parameter name is '{nameof(stringParam)}'")]
    public void MethodWithParam(string stringParam)
    {
        // however, in the method nameof can be used to get 
        // the name of the parameter
        Console.WriteLine($"Value of parameter '{nameof(stringParam)}' is " +
            $"is {stringParam}");
    }
}
```

Now, `nameof` can be used in the method as well as in the attribute parameter!

---

## Notes

A small, but welcome change - leveraging `nameof` results in more consistent, more accurate and safer code. It's not possible to change the name of the variable, without changing all reference to it also being updated - otherwise a compiler error will occur. The more places `nameof` can be utilized, the better!

---

## References

[Oleg Kyrylchuk Tweet](https://twitter.com/okyrylchuk/status/1553081565613367298)   

<?# DailyDrop ?>152: 02-09-2022<?#/ DailyDrop ?>
