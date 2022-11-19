---
title: "List ConvertAll"
lead: "Converting list data types with ConvertAll"
Published: "11/21/2022 01:00:00+0200"
slug: "21-list-convertall"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - list
   - convert
   - select

---

## Daily Knowledge Drop

The `List` class has a `ConvertAll` method which allows for the conversion of items in a list from one type to another. It operates similar to the LINQ `Select` method, but in some use cases will out-perform the LINQ counterpart.

---

## Example

In the below example, we have a List of `TypeA`, and would like to convert this list to a a list of `TypeB`:

The types:
``` csharp
public class TypeA
{
    public string TypeAValue { get; set; }
}

public class TypeB
{
    public string TypeBValue { get; set; }
}
```

Building up a list:

``` csharp
List<TypeA> types = new()
{
  new TypeA(),
  new TypeA(),
  new TypeA(),
  new TypeA(),
};

```

Now that we have a list ot _TypeA_, we'll look at a couple of ways to convert to a list of _TypeB_.

---

### LINQ Select

This is the more "traditional" way of converting a list (or IEnumerable) from one type to another:

``` csharp
var typeBList = types
    .Select(a => new TypeB { TypeBValue = a.TypeAValue })
    .ToList();
```

Here, the _Select_ will operate on _TypeA_ and instantiate a _TypeB_ for each _TypeA_, setting the instance properties. The output of this is a list of _TypeB_.

---

### List ConvertAll

The `ConvertAll` usage is similar to that of the _Select_ method:

``` csharp
var typeBList = types
    .ConvertAll(type => new TypeB { TypeBValue = type.TypeAValue });
```

For each item in the list, a _TypeB_ instance is instantiated and the properties set with the values from the _TypeA_ instance.

A note: `ConvertAll` is only available on _List_, so if working with an IEnumerable implementation, the _ToList_ method would need to be called to convert the collection to a list.

---

## Benchmarks

### 10 Items

|            Method |     Mean |   Error |   StdDev |   Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|------------------ |---------:|--------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
|        LinqSelect | 194.9 ns | 9.99 ns | 29.46 ns | 177.4 ns |  1.00 |    0.00 | 0.0713 |     448 B |        1.00 |
| GenericConvertAll | 156.5 ns | 7.43 ns | 21.92 ns | 143.9 ns |  0.82 |    0.16 | 0.0598 |     376 B |        0.84 |

With only 10 items in the list, the the `ConvertAll` method is faster, and more memory performant than the LINQ `Select` method.

---

### 10 000 Items

As the number of items in the list increases, the performance metrics of the two two methods converge:

|            Method |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |    Gen0 |    Gen1 | Allocated | Alloc Ratio |
|------------------ |---------:|---------:|---------:|---------:|------:|--------:|--------:|--------:|----------:|------------:|
|        LinqSelect | 175.6 us | 10.90 us | 32.15 us | 189.5 us |  1.00 |    0.00 | 50.7813 | 25.3906 | 312.63 KB |        1.00 |
| GenericConvertAll | 174.5 us |  3.42 us |  4.07 us | 173.5 us |  0.97 |    0.15 | 50.7813 | 25.3906 | 312.55 KB |        1.00 |

With 10 000 items, the two methods effectively perform the same, and both use the same amount of memory.

---

### 100 000 Items

The same results can be seen with 100 000 records, as was seen with 10 000 record:

|            Method |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|------------------ |---------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
|        LinqSelect | 6.586 ms | 0.1298 ms | 0.2561 ms |  1.00 |    0.00 | 515.6250 | 328.1250 | 179.6875 |   3.05 MB |        1.00 |
| GenericConvertAll | 6.236 ms | 0.1199 ms | 0.1970 ms |  0.96 |    0.05 | 515.6250 | 320.3125 | 179.6875 |   3.05 MB |        1.00 |

The two methods effectively perform the same, and both use the same amount of memory.

---

## Notes

If currently using `Select`, there is not much reason to update code to switch to `ConvertAll` - except if micro performance improvements are critical to your application. However if performance is so critical for the application, LINQ is probably not even being used in the first place.

Having said all that, it is interesting the `ConvertAll` method is available and does have performance improvements over using LINQ `Select`.

---

## References

[Advanced LINQ](https://code-maze.com/advanced-linq/)  

<?# DailyDrop ?>206: 21-11-2022<?#/ DailyDrop ?>
