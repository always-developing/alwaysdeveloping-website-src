---
title: "LINQ Any/All over Count"
lead: "Use the Any or All methods instead of the Count method"
Published: "01/20/2023 01:00:00+0200"
slug: "20-linq-count"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - count
   - any
   - all

---

## Daily Knowledge Drop

In most scenarios, the LINQ `All` or `Any` methods should be used instead of the `Count` method.

`Count` should be avoided, as it _enumerates through every single entry in the collection to get the count_, where `Any/All` will _return as soon as the predicate condition is not met anymore_.

---

## Examples

All of the below examples, use the following collection:

``` csharp
var intList = Enumerable.Range(1, 10);
```

---

### No predicate

When required to check if a list contains _any_ items, the `Any` method should be used instead of `Count`:

``` csharp
// Bad
Console.WriteLine(intList.Count() > 0);
// Good
Console.WriteLine(intList.Any());
```

In this example, when `Count` is used, 10 items needs to be enumerated to get the full count, however `Any` will return _true_ after one iteration, as soon as one item is found.  
With only 10 items, the difference is negligible, however as the number of items in the collection increased, the difference will become more noticeable.

---

### With predicate

The same logic applies when a _predicate is supplied_:

``` csharp
// Bad
Console.WriteLine(intList.Count(i => i > 5) > 0);
// Good
Console.WriteLine(intList.Any(i => i > 5));

// Bad
Console.WriteLine(intList.Count(i => i > 10) == 0);
// Good
Console.WriteLine(!intList.Any(i => i > 10));
```

The `Count` method will need to enumerable over all items in the collection, while `Any` will return _true_ as soon as the first item which satisfies the predicate is reached.

---

### All items

Similar logic applies when `All` items in the collection need to be checked:

``` csharp
// Bad
Console.WriteLine(intList.Count() == intList.Count(i => i  < 100));
// Good
Console.WriteLine(intList.All(i => i < 100));
```

Again, with `Count`, all items in the collection are enumerated over, while the `All` method will return _false_ as soon as one item is reached which does not satisfy the predicate.

---

## Notes

Generally, unless `Count` specifically needs to be used, `Any` or `All` should be preferred, especially as the number of items in the collection increases.

---

## References

[Using Count() instead of All or Any](https://linkdotnet.github.io/tips-and-tricks/linq/#using-count-instead-of-all-or-any)  

<?# DailyDrop ?>239: 20-01-2023<?#/ DailyDrop ?>
