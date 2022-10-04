---
title: "Using math functions on numeric types"
lead: "With .NET 7 the static Math class is no longer required for math functions"
Published: "10/21/2022 01:00:00+0200"
slug: "20-math-update"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net7
   - math

---

## Daily Knowledge Drop

.NET 7 introduces the ability to `call math methods on numeric types` and not have to use the static _Math_ class. 

---

## Pre .NET 7

Prior to .NET 7, the static `Math` class was used:

``` csharp
double value = 100.33;

// get the absolute value
double mathAbs = Math.Abs(value);

// get the cube root
double mathCbrt = Math.Cbrt(125);
```

---

## .NET 7

In .NET 7, these methods are now available on the numeric type:

``` csharp
double value = 100.33;

double doubleAbs = double.Abs(value);
double doubleCbrt = double.Cbrt(125);
```

Under the hood, these methods are still using the `Math` static class:

``` csharp
// source code for the double Abs method
public static double Abs(double value) => Math.Abs(value);
```

---

## Notes

The two techniques effectively use the same code, and perform the same operation - so which one to use comes down to preference and readability. Personally, I find using the numeric types is more informative, because at a glance one can see the return type of the calculation (the _Abs_ method on _double_, will return a _double_) - but this will not be the preference for everyone.

---

## References

[Fons Sonnemans Tweet](https://twitter.com/fonssonnemans/status/1573274358910525442)  

<?# DailyDrop ?>187: 21-10-2022<?#/ DailyDrop ?>
