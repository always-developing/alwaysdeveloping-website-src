---
title: "Null-coalescing operator and associativity"
lead: "Chaining the null-coalescing operators together"
Published: "11/28/2022 01:00:00+0200"
slug: "28-null-coalescing-assoc"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - nullcoalescing
   - associativity

---

## Daily Knowledge Drop

The `null-coalescing operator` (`??`) is right-associative, and can be chained together to check multiple values, in order, to eventually arrive at a non-null value.

---

## Example

The example for this situation is fairly simple - the operator `??` can be used in a chain to check (and return) the _first non-null value_:

``` csharp
int? GetValue(int? value1, int? value2, int? value3, int defaultValue)
{
  // chain the operator and return the first non-null value
    return value1 ?? value2 ?? value3 ?? defaultValue;
}
```

The values are evaluated left to right to check if they are `null`, and the first non-null value is returned. In this example _defaultValue_ is an `int` and as such cannot be null, so the method will always return a value.

Invoking this with a variety of permutations:

``` csharp
Console.WriteLine(GetValue(null, null, 0, 0));
Console.WriteLine(GetValue(null, 1, 2, 0));
Console.WriteLine(GetValue(2, 5, null, 0));
```

Results in the following, expected output:

``` terminal
0
1
2
```

Each each case the first non-null value is returned. If all _nullable_ values are null, then the default is returned. 

---

## Notes

This small, but useful piece of information for today might seem obvious, and it is once you think about it - personally I've just never encountered the technique or the need to chain together multiple null checks using the `null-coalescing` operator. However, if I ever do, I now know that chaining is possible.

---


## References

[Null-Coalescing Operators’ Associativity](https://code-maze.com/null-coalescing-operator-null-coalescing-assignment-operator-csharp/)  

<?# DailyDrop ?>211: 28-11-2022<?#/ DailyDrop ?>
