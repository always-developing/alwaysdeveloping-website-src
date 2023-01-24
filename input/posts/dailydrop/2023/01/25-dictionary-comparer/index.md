---
title: "Case-less Dictionary keys"
lead: "How to ignore the case of Dictionary keys when performing operations"
Published: "01/25/2023 01:00:00+0200"
slug: "25-dictionary-comparer"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - dictionary
   - comparer
   - ignorecase

---

## Daily Knowledge Drop

The `Dictionary` constructor can take a `StringComparer` parameter allowing for retrieving of keys from the dictionary while ignoring the case of the key. This removes the need to perform _ToUpper_ and _ToLower_ on the calls involving the key.


---

## Without StringComparer

Without a `StringComparer`, the case of the _key added to the dictionary, and the case of the key when performing a lookup, need to match_:

``` csharp
Dictionary<string, int>? artistAlbum = new Dictionary<string, int>();

// Call made to .ToLower() to ensure all artist 
// names are in the same standard format
artistAlbum.Add("Foo Fighters".ToLower(), 10);
artistAlbum.Add("John Mayer".ToLower(), 8);

// check will only return true if using lower case values,
// or specifically calling ToLower
Console.WriteLine(artistAlbum.ContainsKey("Foo Fighters")); // false
Console.WriteLine(artistAlbum.ContainsKey("Foo Fighters".ToLower())); // true
Console.WriteLine(artistAlbum.ContainsKey("foo fighters")); // true
```

In this example, `ToLower` is called each time an item is added to the Dictionary. When performing a check to see if the Dictionary contains a key, a match will only be found if the supplied value is all lower case, or if `ToLower` is specifically called again. This ensure that the keys are always stored, and then looked-up in a consistent format - lower case in this example.

However, all the `ToLower` (or `ToUpper`) calls do have a performance impact, and additionally they have to manually be added everywhere the Dictionary is used. A simpler and better approach is to use a `StringComparer`.

---

## With StringComparer

Using the `StringComparer` technique is as simple as passing a value into the `Dictionary` constructor:

``` csharp
Dictionary<string, int>? artistAlbum = 
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

artistAlbum.Add("Foo Fighters", 10);
artistAlbum.Add("John Mayer", 8);

Console.WriteLine(artistAlbum.ContainsKey("Foo Fighters")); // true
Console.WriteLine(artistAlbum.ContainsKey("Foo fighters")); // true
Console.WriteLine(artistAlbum.ContainsKey("foo fighters")); // true
```

With `StringComparer.OrdinalIgnoreCase` is supplied to constructor, `ToLower` is no longer required when adding or checking the existence of a key. The case is now ignored when doing the comparison - this results in cleaner code, and better overall performance.

---

## Notes

If the `Dictionary` could contain keys of various cases (maybe based on user input) then `StringComparer` should be used over _ToLower_ or _ToUpper_. There is no overhead to using this approach, and is actually cleaner and more performant.

---

## References

[Pass in StringComparer to Dictionary](https://linkdotnet.github.io/tips-and-tricks/dictionary/#pass-in-stringcomparer-to-dictionary)  

<?# DailyDrop ?>242: 25-01-2023<?#/ DailyDrop ?>
