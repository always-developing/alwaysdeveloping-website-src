---
title: "Primitive collection performance"
lead: "Having a look at .NET 7 Linq sort updates and collection sorting performance"
Published: "10/12/2022 01:00:00+0200"
slug: "12-list-sort-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - list
   - array
   - sort

---

## Daily Knowledge Drop

.NET 7 introduces a small update when using LINQ to perform `sorting on a collection` to slightly `simplifying the code` - however, when working with arrays, the `Array.Sort` method is the most performant method to use to perform the sort.

---

## .NET 7 updates

Prior to .NET 7, when performing a _sort_ on a collection of items, the `OrderBy` or `OrderByDescending` method is generally used to sort the items (_OrderBy_ and _OrderByDescending_ are extension methods on IEnumerable):

``` csharp
// generate a random list of 500 entities into an item array
var rando = new Random(1001);
var items = Enumerable.Range(1, 500).Select(i => rando.Next()).ToArray();

Console.WriteLine(items[0]);
// sort the items
var sortedItems = items.OrderBy(item => item).ToArray();
Console.WriteLine(sortedItems[0]);
```

The output being:

``` terminal
First item before sort: 1447366984
First item after sort: 1250278
```

When working with primitives, in the _OrderBy_ method the `keySelector` (_item_ in the above example) is required to be specified, even though there is only one selector - the item in the collection. This is versus a collection of _classes_, where the `keySelector` could be any property on the class.

With .Net 7, the `keySelector` is no longer required:

``` csharp
var rando = new Random(1001);
var items = Enumerable.Range(1, 500).Select(i => rando.Next()).ToArray();

Console.WriteLine($"First item before sort: {items[0]}");
// sort the items, NO SELECTOR
var newSortedItems = items.Order().ToArray();
Console.WriteLine($"First item after sort: {newSortedItems[0]}");
```

The output being:

``` terminal
First item before sort: 1447366984
First item after sort: 1250278
```

The same output, however `slightly more readable and concise code`.

---

## Array.Sort

Using the LINQ `OrderBy` or `OrderByDescending` methods is the default go-to when wanting to sort a collection - however there is another option when _working with arrays_ - the static `Array.Sort` method.

The method usage is straight forward - it takes an array as a parameter, and will `sort the supplied array, NOT return a sorted copy`, as with the LINQ methods:

``` csharp
var rando = new Random(1001);
var items = Enumerable.Range(1, 500).Select(i => rando.Next()).ToArray();

Console.WriteLine($"First item before sort: {items[0]}");
// Perform the sort
Array.Sort(items);
// use the SAME array
Console.WriteLine($"First item after sort: {items[0]}");
```

One again, the same output:

``` terminal
First item before sort: 1447366984
First item after sort: 1250278
```

---

## Performance

Let's benchmark the three methods:
- the pre-dotnet 7 `OrderBy` with a `keySelector`
- the new .NET 7 `OrderBy` without a `keySelector`
- `Array.Sort`

The below was run on the same 500 item collection as shown in the above examples:


|    Method |     Mean |    Error |   StdDev | Ratio | Allocated | Alloc Ratio |
|---------- |---------:|---------:|---------:|------:|----------:|------------:|
|   OrderBy | 41.56 us | 0.216 us | 0.181 us |  1.00 |   10.2 KB |        1.00 |
|     Order | 40.75 us | 0.304 us | 0.285 us |  0.98 |   8.23 KB |        0.81 |
| ArraySort | 18.10 us | 0.180 us | 0.168 us |  0.44 |   2.13 KB |        0.21 |

The new and old version of `OrderBy` practically have the same performance, however for this use case, the `Array.Sort` is `over 200% faster`, and uses only `20% of the memory`!

---

## Notes

The .NET 7 LINQ _OrderBy_ enhancements definitely make the code slightly more readable and concise - however if performance is critical, consider using `AArray.Sort` and avoiding _OrderBy_ entirely. Result may vary based on the size of the array, so as always benchmark with your specific expected workload to get a comparison and make an informed choice.

---

## References

[Oleg Kyrylchuk tweet](https://twitter.com/okyrylchuk/status/1567620600599908352)   
[Stop using LINQ to order your primitive collections in C#](https://www.youtube.com/watch?v=K1Ye_QEpAq8)   

<?# DailyDrop ?>180: 12-10-2022<?#/ DailyDrop ?>
