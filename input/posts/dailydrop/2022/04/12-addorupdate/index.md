---
title: "ConcurrentDictionary AddOrUpdate method"
lead: "ConcurrentDictionary (unlike Dictionary) has a useful AddOrUpdate method"
Published: 04/12/2022
slug: "12-addorupdate"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dictionary
    - concurrentdictionary

---

## Daily Knowledge Drop

The C# `ConcurrentDictionary` implementation has a convenient `AddOrUpdate` method (unlike the Dictionary implementation) - this method allows for a value to try be added to the concurrent dictionary and if the key already exists, then update with a different value.

---

## Dictionary updates

First we'll take a look at how to handle updates when using a `Dictionary`.

First, a `Dictionary<string, int>` is instantiated and a few items are added to the dictionary.

``` csharp
var dict = new Dictionary<string, int>();

dict.Add("zero", 0);
dict.Add("one", 1);
dict.Add("two", 22);
dict.Add("three", 3);
dict.Add("four", 4);

// This would result in an exception
// dict.Add("two", 2);
```

If a duplicate key is used when adding, an exception will be thrown. To cater for this scenario, when adding its best to first check if the dictionary already contains the key, or to use the `TryAdd` method:

``` csharp
// Check if the dictionary contains the key
// if not, then add the item
if (!dict.ContainsKey("two"))
{
    dict.Add("two", 2);
}

// Tries to add the item with a key
// if this fails, then the key exists
// and perform an update instead
if (!dict.TryAdd("2", 2))
{
    dict["two"] = 2;
}
```

---

## ConcurrentDictionary updates

The `ConcurrentDictionary` operates similarly to that of a Dictionary (but is not the same, and has both positive and negative aspects when compared with the Dictionary), but has a useful `AddOrUpdate` method.

First, a `ConcurrentDictionary<string, int>` is instantiated and a few items are added to the dictionary. The ConcurrentDictionary does not have an _Add_ method, only a _TryAdd_:

``` csharp
var conDict = new ConcurrentDictionary<string, int>();
conDict.TryAdd("zero", 0);
conDict.TryAdd("one", 1);
conDict.TryAdd("two", 22);
conDict.TryAdd("three", 3);
conDict.TryAdd("four", 4);
```

The two methods described above to check if the key already exists, can also be used with the ConcurrentDictionary, but now there is also a _AddOrUpdate_ method available:

``` csharp
// Check if the ConcurrentDictionary contains the key
// if not, then add the item
if (!conDict.TryAdd("2", 2))
{
    conDict["two"] = 2;
}

// Tries to add the item with a key
// if this fails, then the key exists
// and perform an update instead
if (!conDict.ContainsKey("two"))
{
    conDict.TryAdd("two", 2);
}

// Add the key "two" with a value of 2
// if it already exists, then take the old value and multiple by 2
conDict.AddOrUpdate("two", 2, (key, existingValue) => existingValue * 2);
```

The _AddOrUpdate_ method simplifies the code - the method tries to add the key and value, but if the key already exists, then the third parameter `Func` is called. In this case, the `Func` multiples the existing value by 2 (and updates the dictionary to now contain the value)


--- 

## Notes

The main benefit of the `ConcurrentDictionary` over the `Dictionary` is that the ConcurrentDictionary is `thread safe` - it can be accessed by multiple threads without running into issues with values being updated at the same time by multiple threads. However, this additional functionality comes with a performance cost.

If needing to convert code from a `Dictionary` to a `ConcurrentDictionary` (perhaps because the system has now become multi-threaded), it can be an almost straight convert - however there is additional methods available on `ConcurrentDictionary` (such as _AddOrUpdate_), which could be leveraged to simplify the code.

---

## References

[ConcurrentDictionary<TKey,TValue> Class](https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2?view=net-6.0)  

<?# DailyDrop ?>50: 12-04-2022<?#/ DailyDrop ?>
