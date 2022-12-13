---
title: ".NET8 Frozen collection"
lead: "New Frozen collection (potentially) being introduced with .NET8"
Published: "12/15/2022 01:00:00+0200"
slug: "15-frozen-collection"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net8
   - collection
   - frozen


---

## Daily Knowledge Drop

.NET8 _potentially_ introduces a new collection type called a `Frozen collection` - this post will explore the new collection, and see how to compares to existing collection implementations.

As .NET8 is still currently in alpha, the `Frozen collection` as well as any specific functionality it may offer could change before the final release.

---

## Usage

To see how the `Frozen collection` operates, below it is used along with other common collection implementations:

``` csharp
// create a base list of 10 items
List<int> baseList = Enumerable
    .Range(1, 10).ToList();

ReadOnlyCollection<int> readonlyList = 
    baseList.AsReadOnly();
FrozenSet<int> frozenSet = baseList.ToFrozenSet();
ImmutableList<int> immutableList = 
    baseList.ToImmutableList();

// now add another item to the list
baseList.Add(11);

Console.WriteLine($"List count: {baseList.Count}");
Console.WriteLine($"ReadOnlyList count: {readonlyList.Count}");
Console.WriteLine($"FrozenSet count: {frozenSet.Count}");
Console.WriteLine($"ImmutableList count: {immutableList.Count}");
```

The output from the above:

``` terminal
List count: 11
ReadOnlyList count: 11
FrozenSet count: 10
ImmutableList count: 10
```

From this we can see, that when _adding an item to the underlying list_:
- The `ReadOnlyList` count also increases, as it is a readonly view into the underlying list
- The `FrozenSet` and `ImmutableList` count is _not increased_

So what's the difference between the `FrozenSet` and `ImmutableList`?

---

## Set vs List

As per their name, the `FrozenSet` is a _set_, while the `ImmutableList` is a _list_ - `a set cannot contain duplicates and is unordered`, unlike a list. Consider the following code, similar to the previous example, but now with duplicate items:

``` csharp

List<int> baseList = Enumerable.Range(1, 10)
    .ToList();
// add duplicate items to the base list
baseList.Add(1);
baseList.Add(2);
baseList.Add(3);

ReadOnlyCollection<int> readonlyList = 
    baseList.AsReadOnly();
FrozenSet<int> frozenSet = baseList.ToFrozenSet();
ImmutableList<int> immutableList = 
    baseList.ToImmutableList();

Console.WriteLine($"List count: {baseList.Count}");
Console.WriteLine($"ReadOnlyList count: {readonlyList.Count}");
Console.WriteLine($"FrozenSet count: {frozenSet.Count}");
Console.WriteLine($"ImmutableList count: {immutableList.Count}");
```

The output:

``` terminal
List count: 13
ReadOnlyList count: 13
FrozenSet count: 10
ImmutableList count: 13
```

The 3 duplicate items are automatically removed when converting the base list to a set. 

---

## Another collection type

So why the need for another specialized collection type - [Steven Giesel](https://steven-giesel.com/blogPost/34e0fd95-0b3f-40f2-ba2a-36d1d4eb5601) benchmark's the performance of the `FrozenSet` against other collection types:
- it is substantially _quicker_ vs the other types when _performing a lookup_
- it is however _slower_ when creating the set vs creating the other types

---

## Notes

A new specialized collection type, which might not see every day use by the majority, but which, with the right use case, can improve the performance of the code.

---


## References

[Frozen collections in .NET 8](https://steven-giesel.com/blogPost/34e0fd95-0b3f-40f2-ba2a-36d1d4eb5601)  

<?# DailyDrop ?>223: 15-12-2022<?#/ DailyDrop ?>
