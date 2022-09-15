---
title: "Performant LINQ type checks"
lead: "Comparing the performance of type checking items using LINQ"
Published: "10/07/2022 01:00:00+0200"
slug: "07-linq-type-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - type

---

## Daily Knowledge Drop

In a previous post, we had a look at [LINQ's OfType](../../08/19-linq-oftype/) method - while personally this is still my preferred method (for readability) to do _type comparisons_ using LINQ, it is not the most performant.

If performing _type checks_ using LINQ, a `GetType() == typeof` check is the most performant.

---

## Type checks

First, we'll have a look at the various methods for doing _type comparisons_ in LINQ which will be benchmarked.

In all of the below example, as well as the benchmarks, a `List<object>` is used, which contains `100 items` either a _TypeA_ class, or a _TypeB_ class:

``` csharp
// sample of the items - in the actual benchmark, _items
// contained 100 items
private List<object> _items = new List<object> {
    new TypeA(), new TypeA(), new TypeB(), new TypeA(), new TypeB()
}
```

Where applicable,  different variations on the techniques, one using `Select` method to _cast_ the cast, and the other using the `Cast` method, were also benchmarked.

---

### OfType

The first method is by using the `OfType` method, explored in a [previous post](../../08/19-linq-oftype/):

``` csharp
    var filter = _items
            .OfType<TypeA>()
            .ToList();
```

I still find this to be the cleanest and easiest to read method.

---

### Cast with is

The next approach is to use the `is` keyword to check the _type_ and then use the `Cast` method to cast the filtered items to the desired type:

``` csharp
    var filter = _items
        .Where(t => t is TypeA)
        .Cast<TypeA>()
        .ToList();
```

The `Where` method filters down the list to items of only _TypeA_ however, the filtered list is still of type _object_, and hence the `Cast` method needs to be used to convert to the correct type.

The variation is the same as the above, but instead using the `Select` method to _cast_:

``` csharp
    var filter = _items
        .Where(t => t is TypeA)
        .Select(t => (TypeA)t)
        .ToList();
```

---

### Cast with is

Next up we use the `as` keyword to check the _type_. This casts the object to _TypeA_, and checks that the result is not null (the cast is possible). Then like the previous example, the `Cast` method to cast the filtered items to the desired type:

``` csharp
    var filter = _items
        .Where(t => t as TypeA is not null)
        .Cast<TypeA>()
        .ToList();
```

As before, the `Where` method filters down the list to items of only _TypeA_ however, the filtered list is still of type _object_, and hence the `Cast` method needs to be used to convert to the correct type.

As above, variation is the same as the above technique, but instead using the `Select` method to _cast_:

``` csharp
    var filter = _items
        .Where(t => t.GetType() == typeof(TypeA))
        .Select(t => (TypeA)t)
        .ToList();
```

---

### GetType with typeof

The last method is the most _raw_ method - manually get the type of the item in the list, and check if its of the same type we desire. Then as before, perform the `Cast`:

``` csharp
    var filter = _items
        .Where(t => t.GetType() == typeof(TypeA))
        .Cast<TypeA>()
        .ToList();
```

The variation:

``` csharp
    var filter = _items
        .Where(t => t.GetType() == typeof(TypeA))
        .Select(t => (TypeA)t)
        .ToList();
```

---

## Performance Results

Running the above queries against a list of 100 items, the result are:

|              Method |     Mean |     Error |    StdDev |   Median | Allocated |
|-------------------- |---------:|----------:|----------:|---------:|----------:|
|              IsType | 2.759 us | 0.0334 us | 0.0279 us | 2.749 us |   2.27 KB |
|        IsTypeSelect | 1.430 us | 0.0275 us | 0.0609 us | 1.416 us |   2.29 KB |
|              AsType | 2.862 us | 0.0570 us | 0.1071 us | 2.844 us |   2.27 KB |
|        AsTypeSelect | 1.265 us | 0.0298 us | 0.0846 us | 1.296 us |   2.29 KB |
|              OfType | 2.873 us | 0.0320 us | 0.0284 us | 2.870 us |   2.23 KB |
|       GetTypeTypeOf | 2.520 us | 0.0495 us | 0.0463 us | 2.510 us |   2.27 KB |
| GetTypeTypeOfSelect | 1.121 us | 0.0100 us | 0.0093 us | 1.124 us |   2.29 KB |


From the result, using `GetType == typeof` is the `fastest method` (while also very slightly using the most memory), while using `OfType` is the `slowest method`.

Using _Select_ to cast is also approximately `twice as fast` as using _Cast_.

---

## Notes

While the _GetTypeTypeOfSelect_ method was my far the fastest method, when performing a single LINQ command, the difference is not going to be noticeable (nano-seconds scale) - however if the collections are larger, and the time frames start moving into the milliseconds or seconds realm, then the method used starts to make a difference. However as usual, benchmark your specific use case and result may vary based on the type of entity, as well as the collection size.

---

## References

[The fastest way to cast objects in C# is not so obvious](https://www.youtube.com/watch?v=dIu5EisoB_s)   

<?# DailyDrop ?>177: 07-10-2022<?#/ DailyDrop ?>
