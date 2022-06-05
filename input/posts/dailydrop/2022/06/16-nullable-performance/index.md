---
title: "The cost of Nullable variables"
lead: "Nullable variables are a useful feature, but come with a performance cost"
Published: "06/16/2022 01:00:00+0200"
slug: "16-nullable-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - performance
    - nullable

---

## Daily Knowledge Drop

The ability to mark a variable as `Nullable` (using the ? syntax) is a useful feature in C#, but does come with a large (relative to non-nullable) performance cost.  

Today we'll have a look at some simple benchmarks and how making a variable nullable impacts performance.

---

## Benchmark

The benchmarks were be run on .NET6, using BenchmarkDotNet.

``` csharp
[Benchmark(Baseline =true)]
[Arguments(1, 2)]
[Arguments(2, 4)]
[Arguments(0, 0)]
public void NotNullableParameters(int a, int b)
{
    int result = 0;

    for (int i = 0; i < 1000; i++)
    {
        result += a * b;
    }
}

[Benchmark]
[Arguments(1, 2)]
[Arguments(2, 4)]
[Arguments(0, 0)]
[Arguments(null, null)]
public void NullableParameters(int? a, int? b)
{
    int? result = 0;

    for (int i = 0; i < 1000; i++)
    {
        result += a * b;
    }
}
```

The logic of each method is the same, and is straight-forward - multiple two int values, 1000 times, and keep a running total of the result.

The first benchmark will us non-nullable int values, while the second will use nullable int values.

---

## Results

Running the benchmarks yields the following results:

|                Method | a | b |       Mean |    Error |   StdDev | Ratio | RatioSD |
|---------------------- |-- |-- |-----------:|---------:|---------:|------:|--------:|
|    NullableParameters | ? | ? | 1,196.4 ns | 21.79 ns | 20.38 ns |     ? |       ? |
| NotNullableParameters | 0 | 0 |   240.0 ns |  3.53 ns |  3.13 ns |  1.00 |    0.00 |
|    NullableParameters | 0 | 0 | 1,051.0 ns |  5.70 ns |  4.45 ns |  4.37 |    0.06 |
| NotNullableParameters | 1 | 2 |   241.4 ns |  3.74 ns |  3.50 ns |  1.00 |    0.00 |
|    NullableParameters | 1 | 2 | 1,054.5 ns |  8.60 ns |  7.18 ns |  4.37 |    0.07 |
| NotNullableParameters | 2 | 4 |   240.8 ns |  2.92 ns |  2.73 ns |  1.00 |    0.00 |
|    NullableParameters | 2 | 4 | 1,053.6 ns |  8.73 ns |  6.81 ns |  4.38 |    0.06 |

In all cases, the `non-nullable version is approximately 4.4x faster than the nullable version`.

Bear in mind, this is measured in nano-seconds, so probably won't make a material different to performance, unless the logic is computational heavy.

---

## Lowered code

Using [sharplab.io](https://sharplab.io) to have a look at the lowered code, there a number of additional checks performed with the nullable version, all of which adds to the performance difference.

The `non-nullable` version is lowered to the following:

``` csharp
public void NotNullableParameters(int a, int b)
{
    int num = 0;
    int num2 = 0;
    while (num2 < 1000)
    {
        num += a * b;
        num2++;
    }
}
```

While the `nullable` version is lowered to this:

``` csharp
public void NullableParameters(Nullable<int> a, Nullable<int> b)
{
    Nullable<int> num = 0;
    int num2 = 0;
    while (num2 < 1000)
    {
        Nullable<int> num3 = num;
        Nullable<int> num4 = a;
        Nullable<int> num5 = b;
        Nullable<int> num6 = ((num4.HasValue & num5.HasValue) ? 
            new Nullable<int>(num4.GetValueOrDefault() * num5.GetValueOrDefault()) : 
            null);
        num = ((num3.HasValue & num6.HasValue) ? 
            new Nullable<int>(num3.GetValueOrDefault() + num6.GetValueOrDefault()) : 
            null);
        num2++;
    }
}
```

---

## Notes

As with most things, there is a trade-off, in this case - the convenience of nullable types vs the performance impact their usage brings. 

The performance penalty when using nullable types is measured in nano-seconds, and for the most part won't have any noticeable impact on performance of the application. However if a large number of operations are being performed on nullable types, and performance is to be improved, then one can look at converting the nullable types to non-nullable.

---

## References

[What is the cost of Nullable in .NET](https://leveluppp.ghost.io/content/images/size/w1000/2021/12/nullable_b.png)  

<?# DailyDrop ?>97: 16-06-2022<?#/ DailyDrop ?>
