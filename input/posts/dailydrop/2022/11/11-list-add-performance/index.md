---
title: "List AddRange performance"
lead: "Comparing the performance of the List AddRange and Add methods"
Published: "11/11/2022 01:00:00+0200"
slug: "11-list-add-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - list
   - add
   - addrange

---

## Daily Knowledge Drop

When multiple items need to be added to a `List`, using the `AddRange` method is generally significantly more performant than using the `Add` method - however, in some scenarios it might actually be slower!

---

## Benchmarks

In all of the below examples, an initial List of 10, 1000 and 10000 items is used as a source and transferred transferer into another destination List.

### One at a time

In this benchmark, the `Add` method is used to add items to a destination list, one at a time:

``` csharp
public void AllOneAtTime()
{
    var newList = new List<int>();

    foreach(var item in items)
    {
        newList.Add(item);
    }
}
```

---

### Filtered one at a time

Here, only certain items are added to the destination list (this divisible by 2). A check is performed, and only if it passes is the specific item added to the list:

``` csharp
public void FilteredOneAtTime()
{
    var newList = new List<int>();

    foreach (var item in items)
    {
        // only if divisible by 2
        if (item % 2 == 0)
        {
            newList.Add(item);
        }
    }
}
```

---

### Add range

In this benchmark, the entire source list is copied to the destination list using the `AddRange` method:

``` csharp
public void AllOnce()
{
    var newList = new List<int>();

    newList.AddRange(items);
}
```

---

### Filtered add range

In the final benchmark, the entire source list is filtered using LINQ, and then `AddRange` is used:

``` csharp
public void FilteredOnce()
{
    var newList = new List<int>();

    newList.AddRange(items.Where(i => i % 2 == 0).ToList());
}
```

---

## Performance

The full results are below, but in summary:
- `AddRange` is 2-3 times faster than using `Add` multiple times (depending on the list size)
- When required to filter the list, its quicker to iterate through each item, perform the check and use `Add`. This is in comparison to using LINQ to filter and then `AddRange` (the bottleneck here is LINQ, not the _AddRange_ call)

### 10 items

|              Method |     Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
|        AllOneAtTime | 59.21 ns | 0.873 ns | 0.817 ns |  1.00 |    0.00 | 0.0343 |     216 B |        1.00 |
|   FilteredOneAtTime | 40.12 ns | 0.738 ns | 0.690 ns |  0.68 |    0.01 | 0.0204 |     128 B |        0.59 |
|             AllOnce | 23.10 ns | 0.458 ns | 0.450 ns |  0.39 |    0.01 | 0.0153 |      96 B |        0.44 |
|        FilteredOnce | 97.60 ns | 1.395 ns | 1.305 ns |  1.65 |    0.04 | 0.0446 |     280 B |        1.30 |

---

### 1000 items

|              Method |       Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|-------------------- |-----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
|        AllOneAtTime | 2,155.3 ns | 40.86 ns | 43.72 ns |  1.00 |    0.00 | 1.3390 | 0.0229 |   8.23 KB |        1.00 |
|   FilteredOneAtTime | 1,554.8 ns | 30.15 ns | 38.13 ns |  0.72 |    0.02 | 0.6847 | 0.0057 |    4.2 KB |        0.51 |
|             AllOnce |   216.5 ns |  3.27 ns |  3.06 ns |  0.10 |    0.00 | 0.6464 | 0.0098 |   3.96 KB |        0.48 |
|        FilteredOnce | 2,870.1 ns | 36.82 ns | 34.44 ns |  1.33 |    0.03 | 1.0223 | 0.0153 |   6.28 KB |        0.76 |

### 10000 items

---

|              Method |      Mean |     Error |    StdDev | Ratio | RatioSD |    Gen0 |   Gen1 | Allocated | Alloc Ratio |
|-------------------- |----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
|        AllOneAtTime | 21.498 us | 0.4191 us | 0.4659 us |  1.00 |    0.00 | 20.8130 | 4.1504 | 128.32 KB |        1.00 |
|   FilteredOneAtTime | 15.165 us | 0.1404 us | 0.1172 us |  0.71 |    0.02 | 10.4675 | 1.2817 |   64.3 KB |        0.50 |
|             AllOnce |  2.127 us | 0.0408 us | 0.0382 us |  0.10 |    0.00 |  6.3667 | 0.7935 |  39.12 KB |        0.30 |
|        FilteredOnce | 28.335 us | 0.3079 us | 0.2729 us |  1.32 |    0.03 | 13.6414 | 2.2583 |  83.95 KB |        0.65 |

---

## Notes

Generally, `AddRange` is much quicker than multiple iterations of `Add` - however, if using `AddRange` in conjunction with a slower operation (such as LINQ's _where_), then it doesn't really matter which is used, and to improve performance the bottleneck (LINQ) should be optimized.

---

## References

[Sabig Gasim Tweet](https://twitter.com/SY7K9/status/1580163986347405312)  

<?# DailyDrop ?>200: 11-11-2022<?#/ DailyDrop ?>
