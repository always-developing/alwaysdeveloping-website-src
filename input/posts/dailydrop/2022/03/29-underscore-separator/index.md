---
title: "Using underscore as a digit separator"
lead: "Underscores can be used a digit separator on numeric literals for ease of reading"
Published: 03/29/2022
slug: "29-underscore-separator"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - separator
    - numeric

---

## Daily Knowledge Drop

An `underscore (_)` can be used to separate digits when dealing with numeric literals to make it easier to read.

---

## Decimal literals

When dealing with large numeric literals, it can sometimes be difficult to read them.

Consider a value representing `pi to 100 digits`:

``` csharp
    var pi = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;
```

This can instead be represented as follows, with a 3 digit separator. This has no effect on the actual value, just how it appears:

``` csharp
    var pi3 = 3.141_592_653_589_793_238_462_643_383_279_502_884_197_169_399_375_105_820_974_944_592_307_816_406_286_208_998_628_034_825_342_117_067_9;
```

The same formatting can also be applied as a `thousand separator`, to make balances easier to read, for example:

``` csharp
    var balance = 75098217932.65;
```

This instead becomes:

``` csharp
    var balanceSeparated = 75_098_217_932.63;
```

Adding the separator has no effect on the underlying values, just increases the readability of the values.

---

## Hexadecimal and binary literals

The formatting can also be applied to hexadecimal and binary literals:

A `hexadecimal literal`:

``` csharp
    var hexValue = 0x2DFDBBC31;
    var hexValueSep = 0x_2_DF_DB_BC_31;
```

The `0x` prefix indicates a hexadecimal literal, and as you can see the separator can be applied.

The same can be applied to a `binary literal`:

``` csharp
    var byteValue = 0b10011010010;
    var byteValueSep = 0b_100_1101_0010;
```

The `0b` prefix indicates a binary literal.

---

## Notes

A small feature learnt today, to assist in making code slightly more readable.

---

## References

[Integral numeric types (C# reference)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types#integer-literals)  
[What's New in C# 7.0?](https://airbrake.io/blog/csharp/digit-separators-reference-returns-and-binary-literals)

<?# DailyDrop ?>40: 29-03-2022<?#/ DailyDrop ?>
