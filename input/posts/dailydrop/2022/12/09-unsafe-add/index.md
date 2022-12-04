---
title: "Fast, potentially unsafe iteration"
lead: "Using CollectionsMarshal, MemoryMarshal and Unsafe for fast looping"
Published: "12/09/2022 01:00:00+0200"
slug: "09-unsafe-add"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - unsafe
   - loop
   - iteration

---

## Daily Knowledge Drop

`CollectionsMarshal`, `MemoryMarshal` and `Unsafe` can be used in conjunction to create a _very fast_ `method of iteration` (possibly the fastest method of performing for loops in C#)

---

## Iteration

The technique may look a bit complex and complicated at first glance (or at least more complex than a normal `for` loop) - but going through it step-by-step it is actually relatively simple. This technique is also not as `safe` as the traditional method, due to the way memory is being handled in a potentially `unsafe` way.

The full code snippet is at the bottom of the section, but for now we'll have a look at it step-by-step.

Assume we have a list of 50 items we want to iterate through:

``` csharp
List<int> loopItems = Enumerable.Range(1, 50).ToList();
```

---

### List to Span

The first step is to convert the `List` to a `Span`, using the _System.Runtime.InteropServices.CollectionsMarshal.AsSpan_ method:

``` csharp
Span<int> itemsAsSpan = CollectionsMarshal.AsSpan(loopItems);
```

Once the `Span` is in use, (as per the documentation) _items should not be added or removed from List_.

--- 

### Span reference

The next step is to get a reference to the _element of the span at index 0_:

``` csharp
ref int searchLocation = ref MemoryMarshal.GetReference(itemsAsSpan);
```

The variable _searchLocation_ is effectively now pointing to the first item in the _Span_.

---

### Iterate

Next, we can perform the actual iteration:

``` csharp
// for loop as per usual
for (int i = 0; i < itemsAsSpan.Length; i++)
{
    // Instead of using itemsAsSpan[i], which is still fast
    // we start with the first item (searchLocation)
    // and offset it by i items
    var item = Unsafe.Add(ref searchLocation, i);
    Console.WriteLine(item);
}
```

The `for` loop is defined as per normal, but instead of accessing the `Span` item at position _i_ as one usually would (_itemsAsSpan[i]_), the `Unsafe.Add` method is used.

`Unsafe.Add` _adds an element offset to the given reference_ - in this case it will use _searchLocation_, the first item in the Span as the given reference, and offset by _i items_ each time.
Each iteration, the offset is larger (as i increases) and as the given reference, _searchLocation_, stays the same the item being referenced each loop is different.


---

### Code snippet

The full code snippet:

``` csharp
List<int> loopItems = Enumerable.Range(1, 50).ToList();
Span<int> itemsAsSpan = CollectionsMarshal.AsSpan(loopItems);

for (var i = 0; i < itemsAsSpan.Length; i++)
{
    var item = Unsafe.Add(ref searchLocation, i);
    Console.WriteLine(item);
}
```

---

## Benchmarks

A full breakdown of the performance can be seen on Nick Chapsas's video, [right here](https://www.youtube.com/watch?v=cwBrWn4m9y8&t=490s).

But in short, using `the above method is the fastest way to iterate`, slightly beating out using the index on the span (_itemsAsSpan[i]_), but significantly faster than all other methods (for loop, foreach loop etc)

---

## Notes

This should probably not be the go-to iteration method for all applications in all use cases - however, when performance is critical and every fraction of a second is important, then this kind of optimization could make a difference.
As mentioned, this technique is less "safe" than the usual _for_ or _foreach_ methods, so should be used with caution.

---


## References

[The weirdest way to loop in C# is also the fastest](https://www.youtube.com/watch?v=cwBrWn4m9y8)  

<?# DailyDrop ?>219: 09-12-2022<?#/ DailyDrop ?>
