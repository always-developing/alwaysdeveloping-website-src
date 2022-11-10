---
title: "Foreach loops and indexes"
lead: "Using a tuple to keep track of an item index in a foreach loop"
Published: "11/15/2022 01:00:00+0200"
slug: "15-foreach-index"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - foreach
   - index

---

## Daily Knowledge Drop

Often when iterating through a list using a `foreach` loop, an index to the item's position in the list is also required - instead of using a separate _index variable_, a `tuple can be used to keep track of the item and it's index`

---

## Manually tracking index

Sometimes while iterating, the index of the item in the list is required - usually this is done by creating a separate variable and manually increasing it in each iteration:

``` csharp
// list with 5 items
var list = new List<int>();
list.AddRange(Enumerable.Range(1, 5));

// variable used to keep track of the index
int loopIndex = 0;
foreach(var item in list)
{
    // access the item and the index
    Console.WriteLine($"ItemValue: {item}");
    Console.WriteLine($"index: {loopIndex}");

    // don't forget to increase the index
    loopIndex++;
}
```

With this approach, the item from the list can be accessed, as well as the index (position) of the item in the list. The output:

``` terminal
ItemValue: 1
index: 0
ItemValue: 2
index: 1
ItemValue: 3
index: 2
ItemValue: 4
index: 3
ItemValue: 5
index: 4
```

This approach is entirely valid, however it does require additional effort, and the developer needs to remember to increase the index variable. There is a cleaner, simpler approach (however, it _might_ come with a performance cost)

---

## Tuple index

Instead of manually creating and incrementing an index variable, this can be done automatically _with the foreach loop_:


``` csharp
// list with 5 items
var list = new List<int>();
list.AddRange(Enumerable.Range(1, 5));

// instead of iterating through the list directly
// select the items of the list into a tuple, along
// with the index
foreach(var (item, index) in 
    list.Select((item, index) => (item, index)))
{
    Console.WriteLine($"ItemValue: {item}");
    Console.WriteLine($"index: {index}");
}
```

Here, the items in the list are select into a list of _tuples_, each tuple containing the item itself, as well as the index. Iterating over this new list gives us access to both these values.

The output is the same as before:

``` terminal
ItemValue: 1
index: 0
ItemValue: 2
index: 1
ItemValue: 3
index: 2
ItemValue: 4
index: 3
ItemValue: 5
index: 4
```

Cleaner, simpler and less prone to errors (forgetting to increment the index manually)

---

## Notes

A relatively simple tip, but one which does make things easier as a developer -  however depending on the list size and the list type, could result in a performance hit. If the list is generally relatively small, the performance hit shouldn't be noticeable and the code is cleaner. But still perform benchmark and use the best solution for your use case

---

## References

[David Pine Tweet](https://twitter.com/davidpine7/status/1584553974236971008)  

<?# DailyDrop ?>202: 15-11-2022<?#/ DailyDrop ?>
