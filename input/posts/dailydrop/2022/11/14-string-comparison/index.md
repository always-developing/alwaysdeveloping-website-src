---
title: "String comparison with StringComparer"
lead: "Using StringComparer.OrdinalIgnoreCase.Equals to compare strings instead of ToLower"
Published: "11/14/2022 01:00:00+0200"
slug: "14-string-comparison"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - string
   - compare
   - comparison

---

## Daily Knowledge Drop

When comparing strings, instead of using `ToLower()` (or `ToUpper`) with the comparison operator `==`, `StringComparer.OrdinalIgnoreCase.Equals` can be used, which is faster and uses less memory.

---

## Examples

There are a number of different ways to compare strings in C# - this post will explore a number of these methods, as well as compare their performance.

---

### ==

The most basic method is to use the comparison operator `==`. 

``` csharp
var value1 = "String1";
var value2 = "STRING1";
var value3 = "String2";

Console.WriteLine("Using ==");
Console.WriteLine($"value1 and value2: {value1 == value2}");
Console.WriteLine($"value1 and value3: {value1 == value3}");
```

The output:
``` terminal
Using ==
value1 and value2: False
value1 and value3: False
```

The issue with this approach is that the _case of the string is not taken into account_.

---

### ToLower/ToUpper

An often use method, is to use the comparison operator, but to `ToUpper` or `ToLower` the string for comparison:

``` csharp
var value1 = "String1";
var value2 = "STRING1";
var value3 = "String2";

Console.WriteLine("Using == and ToLower");
Console.WriteLine($"value1 and value2: {value1.ToLower() == value2.ToLower()}");
Console.WriteLine($"value1 and value3: {value1.ToLower() == value3.ToLower()}");
Console.WriteLine("-----");

Console.WriteLine("Using == and ToUpper");
Console.WriteLine($"value1 and value2: {value1.ToUpper() == value2.ToUpper()}");
Console.WriteLine($"value1 and value3: {value1.ToUpper() == value3.ToUpper()}");
```

The output:
``` terminal
Using == and ToLower
value1 and value2: True
value1 and value3: False
-----
Using == and ToUpper
value1 and value2: True
value1 and value3: False
```

Both methods yield the same, accurate result.

---

### string.Equals

The `string` class has an `Equals` method, which can also be used for comparison. One version is a _static method_, and the other is an _instance method_.

``` csharp
var value1 = "String1";
var value2 = "STRING1";
var value3 = "String2";

// instance Equals method
Console.WriteLine("Using instance Equals");
Console.WriteLine($"value1 and value2: {value1.Equals(value2)}");
Console.WriteLine($"value1 and value3: {value1.Equals(value3)}");
Console.WriteLine("-----");

// Static Equals method
Console.WriteLine("Using static Equals");
Console.WriteLine($"value1 and value2: {string.Equals(value1, value2)}");
Console.WriteLine($"value1 and value3: {string.Equals(value1, value3)}");
```

The output:

``` terminal
Using instance Equals
value1 and value2: False
value1 and value3: False
-----
Using static Equals
value1 and value2: False
value1 and value3: False
```

This basic version of `Equals` does not take the string case into account - however there is an additional parameter which can be passed to the `Equals` method to define how the comparison is done:

``` csharp
var value1 = "String1";
var value2 = "STRING1";
var value3 = "String2";

Console.WriteLine("Using static Equals OrdinalIgnoreCase");
Console.WriteLine($"value1 and value2: {string.Equals(value1, value2, 
    StringComparison.OrdinalIgnoreCase)}");
Console.WriteLine($"value1 and value3: {string.Equals(value1, value3, 
    StringComparison.OrdinalIgnoreCase)}");
Console.WriteLine("-----");
```

The output of this:

``` terminal
Using string.Equals OrdinalIgnoreCase
value1 and value2: True
value1 and value3: False
```

---

### StringComparer

The final method is using `StringComparer`:

``` csharp
var value1 = "String1";
var value2 = "STRING1";
var value3 = "String2";

Console.WriteLine("Using StringComparer");
Console.WriteLine($"value1 and value2: {StringComparer.OrdinalIgnoreCase.Equals(value1, value2)}");
Console.WriteLine($"value1 and value3: {StringComparer.OrdinalIgnoreCase.Equals(value1, value3)}");
```

The output:

``` terminal
Using StringComparer
value1 and value2: True
value1 and value3: False
```

---

## Benchmarks

Finally, we can benchmark all the different methods:

| Method | Mean | Error | StdDev | Median | Ratio | RatioSD | Gen0 | Allocated | Alloc Ratio |
| --------------------------------- | ----------:| ----------:| -----------:| ----------:| ------:| --------:| -------:| ----------:| ------------:|
| BasicComparison | 1.972 ns | 0.0392 ns | 0.0348 ns | 1.974 ns | 1.00 | 0.00 | - | - | NA |
| BasicToLower | 42.073 ns | 0.8145 ns | 0.7619 ns | 42.187 ns | 21.34 | 0.61 | 0.0127 | 80 B | NA |
| BasicToUpper | 51.095 ns | 4.6650 ns | 13.7548 ns | 42.910 ns | 21.74 | 1.32 | 0.0127 | 80 B | NA |
| StringInstanceEquals | 1.903 ns | 0.0465 ns | 0.0388 ns | 1.912 ns | 0.96 | 0.02 | - | - | NA |
| StringStaticEquals | 1.896 ns | 0.0272 ns | 0.0227 ns | 1.899 ns | 0.96 | 0.02 | - | - | NA |
| StringEqualOrdinalIgnoreCases | 6.083 ns | 0.1480 ns | 0.1584 ns | 6.078 ns | 3.09 | 0.10 | - | - | NA |
| StringComparerOrdinalIgnoreCases | 4.770 ns | 0.0669 ns | 0.0559 ns | 4.762 ns | 2.42 | 0.04 | - | - | NA |

From the results, once can see that:
- In all instances where the case of the string is taken into account, performance is slower
- The often used `ToUpper/ToLower` method is the slowest a large margin, and the only method to use memory
- `StringComparer` is the most performant technique to use when a comparison needs to be done while ignoring the case

---

## Notes

Realistically, in most applications the performance of a string comparison is not going to have any material effect on performance. However if the application does a large number of string comparisons, the accumulative effect could be slightly noticeable and it might be worth investigating the use of `StringComparer`.

---

## References

[@Tullo Tweet](https://twitter.com/Tullo/status/1583491663577481219)  

<?# DailyDrop ?>201: 14-11-2022<?#/ DailyDrop ?>
