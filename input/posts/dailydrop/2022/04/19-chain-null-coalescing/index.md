---
title: "Chaining null-coalescing operator"
lead: "Looking into how the null-coalescing operator can be chained together"
Published: 04/19/2022
slug: "19-chain-null-coalescing"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - operator
    - coalescing

---

## Daily Knowledge Drop

The C# null-coalescing operator `??` can be chained together to eventually get to a non-null value.

---

## Null-Coalescing operator

The null-coalescing operator returns the value of its left-hand operand if it isn't `null`, otherwise it will return the right-hand operand and assign it to the result.

Below is a simple example:

``` csharp
string value1 = "value1";
string? value2 = null;

string resultValue = value2 ?? value1;

Console.WriteLine(resultValue); // "value1" is output 
```

When the null-coalescing operator (??) is used:
- The left-hand side `value2` will be evaluated and if `not null will be assigned` to _resultValue_.
- If it is null, then the right-hand side `value1` will be assigned to _resultValue_

---

## Chaining

Considering how the operator works (left-hand side evaluated, then right-hand side) - it makes sense that the operator can be chained together: 

``` csharp
string value1 = "value1";
string? value2 = null;
string? value3 = null;

string resultValue = value3 ?? (value2 ?? value1);

Console.WriteLine(resultValue);
```

When the null-coalescing operator is used:
- The left-hand side `value3` will be evaluated and if `not null will be assigned` to _resultValue_. 
- If it is null, then the right-hand side will be evaluated:
    - The left-hand side `value2` of the right operand will be evaluated and if `not null will be assigned` to _resultValue_. 
    - If it is null, then the right-hand side `value1` will be assigned to _resultValue_

This can be simplified to no using parenthesis and just using the `??` operator:

``` csharp
string value1 = "value1";
string? value2 = null;
string? value3 = null;
string? value4 = null;
string? value5 = null;
string? value6 = null;

// each value is checked to see if null, and if so the next value is evaluated
string resultValue = value6 ?? value5 ?? value4 ?? value3 ?? value2 ?? value1;

Console.WriteLine(resultValue);
```

The final value output to the console being `value1`.

---

## Notes

This is not something I'd thought about, but it makes sense that the operator would work like this, and this would be possible, when you consider how the values are evaluated. It is a small simple, technique to use to simplify and reduce unnecessary code.

---

## References

[Chaining the C# ?? Operator](https://weblog.west-wind.com/posts/2008/Jan/23/Chaining-the-C-Operator)  

<?# DailyDrop ?>55: 19-04-2022<?#/ DailyDrop ?>
