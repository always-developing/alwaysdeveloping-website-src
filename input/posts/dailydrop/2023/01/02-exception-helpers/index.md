---
title: ".NET 8 Exception helpers"
lead: "Throwing exceptions is becoming much easier in .NET 8"
Published: "01/02/2023 01:00:00+0200"
slug: "01-exception-helpers"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net8
   - exception
   - helpers


---

## Daily Knowledge Drop

A number of _helper_ methods are being introduced with .NET 8, which make the process of `checking values and throwing exception` a lot simpler.

Keep in mind .NET 8 is in pre-release, so the functionality could potentially change or be removed before the final .NET8 release.

---

## Existing helper

An example of an existing (prior to .NET 8) exception helper method, is the `ArgumentException.ThrowIfNullOrEmpty` method.

Instead of having to do this:

``` csharp
void ManualThrowException(string strParam)
{
    if (string.IsNullOrEmpty(strParam))
    {
        throw new Exception($"{nameof(strParam)} is null or empty");
    }
}
```

The `ThrowIfNullOrEmpty` method can be used, which checks the value of the parameter and throws the exception:

``` csharp
void ThrowNullOrEmptyException(string strParam)
{
    ArgumentException.ThrowIfNullOrEmpty(strParam);
}
```

---

## New helpers

There are a number of new static helper methods (on _ArgumentOutOfRangeException_) being introduced, which include:

- ThrowIfZero
- ThrowIfNegative
- ThrowIfNegativeOrZero
- ThrowIfGreaterThan

The usage and functionality of these are the same as `ThrowIfNullOrEmpty` - the parameter is checked, and an exception is throw if the check fails.

To check if a int value is _zero or negative_, instead of this:

``` csharp
void ManualThrowZeroNegativeException(int intParam)
{
    if (intParam <= 0)
    {
        throw new Exception($"{nameof(intParam)} is less than or equal to zero");
    }
}
```

This can be done:

``` csharp
void EnhancedThrowZeroNegativeException(int intParam)
{
    ArgumentOutOfRangeException.ThrowIfNegative(intParam);
}
```

If thrown, the exception generated is also more informative than before, including the parameter name in the exception text:

``` terminal
'intParam' must be a non-negative value.
```

The result - more readable code, and quicker and easier for a developer to thrown an exception.

---

## Notes

A relatively small and simple helper method update - but one will facilitate cleaner, simpler and more readable code.

---


## References

[New ArgumentException and ArgumentOutOfRangeException helpers in .NET 8](https://steven-giesel.com/blogPost/f4bc6fcc-5691-4f72-b9bb-75aeeb59230a)  

<?# DailyDrop ?>225: 02-01-2023<?#/ DailyDrop ?>
