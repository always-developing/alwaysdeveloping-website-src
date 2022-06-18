---
title: "Enum validity with IsDefined"
lead: "Check the validity of an enum with with the IsDefined method"
Published: 03/14/2022
slug: "14-enum-isdefined"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - enum
    - switch
    - validity

---

## Daily Knowledge Drop

There is a helper `IsDefined` method on the `Enum` class, which will check if a numerical value is valid for a specific enum type.

---

## IsDefined method

Assume an enum is defined as below:

``` csharp
public enum Direction
{
    North,
    East,
    South,
    West
}
```

The `IsDefined` can be invoked as follows:

``` csharp
bool IsEnumDefined(int direction)
{
    return Enum.IsDefined(typeof(Direction), direction);
}
```

The method takes in the enum type, as well as the numerical value and validates if the enum defines a value for the numerical value.

This is pretty straightforward and useful, especially for confirming the validity of an enum is the data is being sent from a 3rd party as an int value.

--- 

## Performance

While `IsDefined` is easy to use, one negative aspect is that it is slow. `Very slow` (in comparison).

Consider this `switch statement` which effectively returns the same validation result as the `IsDefined` method:

``` csharp
bool IsEnumSwitch(int direction)
{
    switch(direction)
    {
        case 0:
        case 1:
        case 2:
        case 3:
            return true;
        default:
            return false;
    }
}
```

Here the enum values are hardcoded, and the method checks if the supplied numerical value is part the hardcoded list.

Using `BenchmarkDotNet` to run a simple benchmark, we can see that the switch is orders of magnitude faster than the `IsDefined` method.

``` csharp
[Benchmark(Baseline=true)]
public void IsEnumDefined_Benchmark()
{
    for(int i = 0; i < 100; i++)
    {
        _ = IsEnumDefined(i);
    }
}

[Benchmark]
public void SwitchStatement_Benchmark()
{
    for (int i = 0; i < 100; i++)
    {
        _ = IsEnumSwitch(i);
    }
}
```

The results:

|                    Method |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Allocated |
|-------------------------- |------------:|-----------:|-----------:|------:|-------:|----------:|
|   IsEnumDefined_Benchmark | 8,566.83 ns | 119.527 ns | 111.806 ns | 1.000 | 0.3815 |   2,400 B |
| SwitchStatement_Benchmark |    33.34 ns |   0.670 ns |   0.689 ns | 0.004 |      - |         - |

---

## Switch expression

A big negative aspect of the `switch` approach, is that if the enum is very large, the switch will end up just just as big. The `switch expression` can assist here (assuming all the enum values are consecutive):

``` csharp
bool IsEnumSwitchExp(int direction) => direction switch
{
    <= 3 => true,
    _ => false,
};
```

Here instead of listing each value of the enum, the numerical value is just checked to make sure its equal to or smaller than the maximum enum value. 

The same test is executed:

``` csharp
[Benchmark]
public void SwitchExp_Benchmark()
{
    for (int i = 0; i < 100; i++)
    {
        _ = IsEnumSwitchExp(i);
    }
}
```


The results - the switch statement and switch expression are basically identical.

|                    Method |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Allocated |
|-------------------------- |------------:|-----------:|-----------:|------:|-------:|----------:|
|   IsEnumDefined_Benchmark | 8,566.83 ns | 119.527 ns | 111.806 ns | 1.000 | 0.3815 |   2,400 B |
|       SwitchExp_Benchmark |    33.11 ns |   0.672 ns |   0.773 ns | 0.004 |      - |         - |
| SwitchStatement_Benchmark |    33.34 ns |   0.670 ns |   0.689 ns | 0.004 |      - |         - |

---

## Notes

While the `IsDefined` method is very useful, the trade off comes with (relative) slow performance. If this function is used often on a hot path, it might make more sense to trade it out for a `switch statement` or `switch expression`, even though it might take more work maintaining the code.

---

## References
[Bartosz Adamczewski tweet](https://twitter.com/badamczewski01/status/1489883708769411080)  

<?# DailyDrop ?>30: 14-04-2022<?#/ DailyDrop ?>
