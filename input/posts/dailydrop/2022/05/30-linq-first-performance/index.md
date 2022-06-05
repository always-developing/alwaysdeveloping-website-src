---
title: "LINQ First and Single performance"
lead: "Investigating LINQ performance when retrieving a single item from a enumeration"
Published: "05/30/2022 01:00:00+0200"
slug: "30-linq-first-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - linq
    - first
    - single
    - performance

---

## Daily Knowledge Drop

When using LINQ to retrieve a value from a collection, there are a number of different techniques - however not all are equal in terms of performance. 

Today we'll explore the various methods and their comparative performance.

---

## First performance

First, we'll look at the various ways to retrieve a single value using variations of `First`, when **multiple** values match the condition: 

``` csharp
// populate with 10000 values
int[] values = values = Enumerable.Range(0, 10000).ToArray();

// Only using First
var firstValue = values.First(v => v > 400);

// Only FirstOrDefault
var firstDefaultValue = values.FirstOrDefault(v => v > 400);

// Where and then First
var whereFirstValue = values.Where(v => v > 400).First();
```

In the above example, an array of 10000 items is populated, and when we look at three ways to find the _First_ value greater than 400.

Benchmarking the above scenario using BenchmarkDotNet, the results are as follows (using .NET6):

|         Method |       Mean |    Error |   StdDev | Ratio |
|--------------- |-----------:|---------:|---------:|------:|
|          First | 2,330.2 ns | 20.03 ns | 18.74 ns |  1.00 |
| FirstOrDefault | 2,330.7 ns | 19.28 ns | 18.04 ns |  1.00 |
|     WhereFirst |   722.3 ns |  8.27 ns |  7.73 ns |  0.31 |

Interestingly, the `Where().First()` approach is 3 times faster than `First` or `FirstOrDefault`.

The benchmarks were also performed using a `List<int>` as well as an `Array[100]` (with the condition being > 40). The results were roughly the same, with the `WhereFirst` approach `2-3 times` faster than _First_ or _FirstOrDefault_.

---

## Single performance

Next, we'll look at the various ways to retrieve a single value using variations of `First/Single`, when a **single** value matches the condition: 

``` csharp
// populate with 10000 values
int[] values = values = Enumerable.Range(0, 10000).ToArray();

// Only using First
var firstValue = values.First(v => v == 450);

// Only FirstOrDefault
var firstDefaultValue = values.FirstOrDefault(v => v == 450);

// Only Single
var singleValue = values.Single(v => v == 450);

// Only SingleOrDefault
var singleOrDefaultValue = values.SingleOrDefault(v => v == 450);

// Where and then First
var whereFirstValue = values.Where(v => v == 450).First();
```

In the above example, an array of 10000 items is populated, and when we look at five ways to find the _First_ or _Single_ value equal to 450.

Benchmarking the above scenario using BenchmarkDotNet, the results are as follows (using .NET6):

|                  Method |        Mean |     Error |    StdDev | Ratio |
|------------------------ |------------:|----------:|----------:|------:|
|           FirstOneValue |  2,342.4 ns |  28.89 ns |  25.61 ns |  1.00 |
|  FirstOrDefaultOneValue |  2,341.8 ns |  21.82 ns |  19.34 ns |  1.00 |
|          SingleOneValue | 53,322.5 ns | 403.26 ns | 357.48 ns | 22.77 |
| SingleOrDefaultOneValue | 55,948.0 ns | 463.01 ns | 433.10 ns | 23.87 |
|      WhereFirstOneValue |    698.9 ns |   5.24 ns |   4.90 ns |  0.30 |

As before, `Where().First()` is the quickest approach being 3 times faster than _First_ or _FirstOrDefault_, while using _Single_ is `22 times slower` than _First_ and approximately `75 times slower` than _Where().First()_

The benchmarks were also performed using a `List<int>` with 10000 items - the results were roughly the same, with the `WhereFirst` approach `2-3 times` faster than _First_ or _FirstOrDefault_ and `65 times faster` than _Single_.

However, when performed with `Array[100]` (with the condition being = 45) the differences between the various methods is not as drastic:


|                  Method |     Mean |    Error |  StdDev | Ratio | 
|------------------------ |---------:|---------:|--------:|------:|
|           FirstOneValue | 256.0 ns |  1.19 ns | 1.05 ns |  1.00 |
|  FirstOrDefaultOneValue | 258.0 ns |  2.02 ns | 1.89 ns |  1.01 |
|          SingleOneValue | 574.3 ns | 11.07 ns | 9.24 ns |  2.24 |
| SingleOrDefaultOneValue | 529.4 ns |  3.23 ns | 3.02 ns |  2.07 |
|      WhereFirstOneValue | 107.5 ns |  0.30 ns | 0.25 ns |  0.42 |


---

## Notes

We've looked at various techniques to get a value from an enumeration of varying sizes - in all use cases performed for this post `Where().First()` was the fastest approach. but this might not always be the same for all possible use cases.

Keep in mind that the size of the collection, and the method used _does_ have a performance impact (admittedly nanoseconds, but in the right hot path it could make a material difference) and the various methods should be benchmarked with your specific use case to determine the correct method.

---

## References

[LINQ optimizations in .NET can be surprising](https://leveluppp.ghost.io/content/images/size/w1000/2021/12/linq.png)  

<?# DailyDrop ?>84: 30-05-2022<?#/ DailyDrop ?>
