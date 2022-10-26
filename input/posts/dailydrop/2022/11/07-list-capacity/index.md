---
title: "List capacity and performance"
lead: "How setting a Lists capacity explicitly can improve performance"
Published: "11/07/2022 01:00:00+0200"
slug: "07-list-capacity"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - list
   - capacity
   - collection

---

## Daily Knowledge Drop

When declaring an instance of `List`, explicitly specifying the `expected capacity can improve performance` when adding items to the list. However, specifying the capacity incorrectly could be detrimental to performance.

---

## Example

### No capacity

In the first example, a new `List` is declared _without specifying a capacity_, and then 20 items are added to the the list:

``` csharp
var list = new List<int>();

for (int i = 0; i < 20; i++)
{
    list.Add(i);
        Console.WriteLine($"List count: {list.Count} and " +
        $"the capacity is: {list.Capacity}");
}
```

Looking at the output, the default capacity when not explicitly specified is `4`, and when this capacity is reached, `the capacity is doubled`:

``` terminal
List count: 1 and the capacity is: 4
List count: 2 and the capacity is: 4
List count: 3 and the capacity is: 4
List count: 4 and the capacity is: 4
List count: 5 and the capacity is: 8
List count: 6 and the capacity is: 8
List count: 7 and the capacity is: 8
List count: 8 and the capacity is: 8
List count: 9 and the capacity is: 16
List count: 10 and the capacity is: 16
List count: 11 and the capacity is: 16
List count: 12 and the capacity is: 16
List count: 13 and the capacity is: 16
List count: 14 and the capacity is: 16
List count: 15 and the capacity is: 16
List count: 16 and the capacity is: 16
List count: 17 and the capacity is: 32
List count: 18 and the capacity is: 32
List count: 19 and the capacity is: 32
List count: 20 and the capacity is: 32
```

---

### With capacity

To specify the capacity, the value is passed to the constructor of the `List`:

``` csharp
// set the capacity to 10. Half the expected capacity
var list = new List<int>(10);

for (int i = 0; i < 20; i++)
{
    list.Add(i);
    Console.WriteLine($"List count: {list.Count} and " +
        $"the capacity is: {list.Capacity}");
}
```

Looking at the output, the capacity starts at 10, and is only doubled once:

``` terminal
List count: 1 and the capacity is: 10
List count: 2 and the capacity is: 10
List count: 3 and the capacity is: 10
List count: 4 and the capacity is: 10
List count: 5 and the capacity is: 10
List count: 6 and the capacity is: 10
List count: 7 and the capacity is: 10
List count: 8 and the capacity is: 10
List count: 9 and the capacity is: 10
List count: 10 and the capacity is: 10
List count: 11 and the capacity is: 20
List count: 12 and the capacity is: 20
List count: 13 and the capacity is: 20
List count: 14 and the capacity is: 20
List count: 15 and the capacity is: 20
List count: 16 and the capacity is: 20
List count: 17 and the capacity is: 20
List count: 18 and the capacity is: 20
List count: 19 and the capacity is: 20
List count: 20 and the capacity is: 20
```


---

## Benchmarks

So how does specifying the capacity effect performance?

In the below examples, a `List` instance is declared, with the capacity explicitly set to varying sizes and then filled with values. The benchmarks evaluated performance when:
- no capacity specified
- the capacity was defined as exactly the expected size
- the capacity was defined as a half of the expected size
- the capacity was defined as a quarter of the expected size
- the capacity was defined as a tenth of the expected size

The code snippets for the benchmarks were as follows:

``` csharp

[Benchmark(Baseline = true)]
public void NoCapacity()
{
    /// no capacity specified
    var list = new List<int>();

    // Iterations was either 10, 1000, or 100000
    for (int i = 0; i < Iterations; i++)
    {
        list.Add(i);
    }
}

[Benchmark]
public void WithExactCapacity()
{
    // here the list defined with exact capacity
    var list = new List<int>(Iterations);

    for (int i = 0; i < Iterations; i++)
    {
        list.Add(i);
    }
}

[Benchmark]
public void WithHalfCapacity()
{
    // for the fractional tests, Iterations
    // was divided by 2, 4 and 10 (not all shown here)
    var list = new List<int>(Iterations / 2);

    for (int i = 0; i < Iterations; i++)
    {
        list.Add(i);
    }
}

// other tests trimmed

```

---

### 10 items

Benchmarking adding 10 items to the list:

|              Method | Iterations |          Mean | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------- |----------- |--------------:|------:|---------:|---------:|---------:|----------:|------------:|
|          NoCapacity |         10 |      51.72 ns |  1.00 |   0.0344 |        - |        - |     216 B |        1.00 |
|   WithExactCapacity |         10 |      22.74 ns |  0.44 |   0.0153 |        - |        - |      96 B |        0.44 |
|    WithHalfCapacity |         10 |      34.05 ns |  0.66 |   0.0229 |        - |        - |     144 B |        0.67 |
| WithQuarterCapacity |         10 |      57.53 ns |  1.11 |   0.0395 |        - |        - |     248 B |        1.15 |
|   WithTenthCapacity |         10 |      69.32 ns |  1.34 |   0.0446 |        - |        - |     280 B |        1.30 |

---

Benchmarking adding 1000 items to the list:

### 1000 items

|              Method | Iterations |          Mean | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------- |----------- |--------------:|------:|---------:|---------:|---------:|----------:|------------:|
|          NoCapacity |       1000 |   1,714.12 ns |  1.00 |   1.3409 |   0.0248 |        - |    8424 B |        1.00 |
|   WithExactCapacity |       1000 |   1,297.63 ns |  0.76 |   0.6447 |   0.0095 |        - |    4056 B |        0.48 |
|    WithHalfCapacity |       1000 |   1,435.71 ns |  0.84 |   0.9689 |   0.0210 |        - |    6080 B |        0.72 |
| WithQuarterCapacity |       1000 |   1,525.41 ns |  0.89 |   1.1311 |   0.0210 |        - |    7104 B |        0.84 |
|   WithTenthCapacity |       1000 |   1,779.44 ns |  1.04 |   1.9989 |   0.0687 |        - |   12552 B |        1.49 |


---

Benchmarking adding 100 000 items to the list:

### 100 000 items

|              Method | Iterations |          Mean | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------- |----------- |--------------:|------:|---------:|---------:|---------:|----------:|------------:|
|          NoCapacity |     100000 | 344,487.97 ns |  1.00 | 285.6445 | 285.6445 | 285.6445 | 1049072 B |        1.00 |
|   WithExactCapacity |     100000 | 227,840.62 ns |  0.66 | 124.7559 | 124.7559 | 124.7559 |  400098 B |        0.38 |
|    WithHalfCapacity |     100000 | 236,859.59 ns |  0.69 | 181.6406 | 181.6406 | 181.6406 |  600141 B |        0.57 |
| WithQuarterCapacity |     100000 | 344,779.88 ns |  1.00 | 199.7070 | 199.7070 | 199.7070 |  700171 B |        0.67 |
|   WithTenthCapacity |     100000 | 466,319.55 ns |  1.37 | 333.0078 | 333.0078 | 333.0078 | 1240264 B |        1.18 |

From the results, one can see:
- In all cases, setting the list capacity to the `exact expected size is twice as fast` as not setting a capacity
- In most cases, setting the list capacity `explicitly can result in significantly faster performance` over not setting the capacity
- In some cases however, explicitly setting the `list capacity to a value which is too small, is slower` than not setting a capacity at all - in the benchmarks, setting the capacity to 1/10th of the expected values

---

## Notes

When instantiating a list, it can be beneficial to set the capacity to the estimated final size. Even setting it to a fraction of the final expected size can be beneficial to performance.  
However setting the capacity to a value too small can be detrimental to performance, in which case its better to not set a capacity at all. Benchmark the specific use case, with the expected capacity and make an informed decision on how to set (or not set) the capacity.

---

## References

[What’s new in C# 11? Dev friendly features](https://tomaszs2.medium.com/c-11-wants-to-be-your-friend-db4a31ed9710)  

<?# DailyDrop ?>196: 07-11-2022<?#/ DailyDrop ?>
