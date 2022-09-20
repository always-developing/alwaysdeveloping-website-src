---
title: "ReadOnlyList and runtime errors"
lead: "Compile vs runtime error occurs with readonly lists"
Published: "10/11/2022 01:00:00+0200"
slug: "11-readonly-list"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - list
   - readonly

---

## Daily Knowledge Drop

When working with a `ReadOnlyList`, depending on how the list is defined, it is possible to _accidentally try_ change a value in the list, with the exception only becoming apparent at runtime and not compile time.

---

## Runtime

Suppose we have the following code:

``` csharp
List<int> golden = new List<int> { 1, 6, 1, 8 };

// readonlyGolden is a readonly list
IList<int> readonlyGolden = golden.AsReadOnly();

// still allows for trying to change the value
readonlyGolden[1] = 7;
```

Even though _golden_ is being `converted to readonly list with the AsReadOnly method`, as it is being cast to _IList\<int\>_, the compiler does recognize _readonlyGolden_ are read only.

"Changing" the value in the readonly list is "allowed" at compiled, however at runtime a `NotSupportedException` will be thrown:

``` terminal
Collection is read-only.
```

---

## Compile time

Instead, of _IList\<int\>_ the _golden_ variable is cast to _IReadOnlyList\<int\>_:

``` csharp
List<int> golden = new List<int> { 1, 6, 1, 8 };

// readonlyGolden is a readonly list
IReadOnlyList<int> readonlyGolden = golden.AsReadOnly();
// OR
// IReadOnlyList<int> readonlyGolden = golden

// this will cause a compiler error
readonlyGolden[1] = 7;
```

Now the code will not compile, as the compile recognizes _readonlyGolden_ as read-only.

``` terminal
Property or indexer 'IReadOnlyList<int>.this[int]' cannot be assigned to -- it is read only
```

---

## Notes

The above may seem obvious - cast a list to _IList_ and it's not (necessarily) readonly, while casting it to _IReadOnlyList_ will definitely make it readonly. However, the take away is that one `cannot assume that an IList is mutable`, just because the compiler does not stop the code from compiling - the implementation could be a _ReadOnlyList_, which will only become apparent at runtime, with an exception.

---

## References

[Davide Bellone tweet](https://twitter.com/BelloneDavide/status/1567562406271885313)   

<?# DailyDrop ?>179: 11-10-2022<?#/ DailyDrop ?>
