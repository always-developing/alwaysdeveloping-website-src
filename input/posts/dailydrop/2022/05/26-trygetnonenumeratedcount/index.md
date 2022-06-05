---
title: "IEnumerable count without enumeration"
lead: "How to (possibility) get an IEnumerable count with TryGetNonEnumeratedCount"
Published: 05/26/2022
slug: "26-trygetnonenumeratedcount"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - ienumerable
    - enumeration
    - count

---

## Daily Knowledge Drop

The `TryGetNonEnumeratedCount` method (introduced in .NET6) can be used to _attempt_ to determine the number of elements in a sequence without forcing an enumeration over the sequence.

Some implementations of IEnumerable<> can have the count determined without enumerating over all the items, while other implementations require an enumeration. `TryGetNonEnumeratedCount` will perform a series of type tests, identifying common types whose count can be determined without enumerating.

---

## Example

Consider the following sample - first, a method which will return an `IEnumerable<string>` of shipping options:

``` csharp
public IEnumerable<string> GetShippingOptions()
{
    yield return "Pickup";
    yield return "Express";
    yield return "Overnight";
    yield return "Standard";
    yield return "Overseas shipping";
}
```

Next, we have a method which accepts an `IEnumerable<string>` implementation, and will:
- Try to get the count without enumerating using the `TryGetNonEnumeratedCount` method
- If unable to get the count without enumerating, then enumerate the sequence using the `Count` method
- Return the count of items in the sequence

``` csharp
public int GetCount(IEnumerable<string> options)
{
    if (options.TryGetNonEnumeratedCount(out var count))
    {
        Console.WriteLine($"TryGetNonEnumeratedCount success! Count => {count}");
        return count;
    }
    else
    {
        var enumerateCount = options.Count();
        Console.WriteLine($"TryGetNonEnumeratedCount fail! " +
            $"Have to enumerate. Count => {enumerateCount}");
        return enumerateCount;
    }
}
```

Lastly, lets call the _GetCount_ method with different implementations of `IEnumerable<string>`:

``` csharp
// options is IEnumerable<string> as per 
// GetShippingOptions return type
var options = GetShippingOptions();

var ienumerableCount = GetCount(options);
var listCount = GetCount(options.ToList());
var arrayCount = GetCount(options.ToArray());
```

Executing this, the output is:

``` powershell
TryGetNonEnumeratedCount fail! Have to enumerate. Count => 5
TryGetNonEnumeratedCount success! Count => 5
TryGetNonEnumeratedCount success! Count => 5
```


---

## Notes

When working with IEnumerable, and various implementations, it might be advantageous (with regards to performance) to first check if the count can be retrieved without enumerating, using the `TryGetNonEnumeratedCount` method, and only if that's not possible then using a method (such as `Count`) which enumerates over the sequence.

As always, benchmark your specific use case and expected IEnumerable size, to determine which method makes sense and results in better performance.


---

## References

[Avoiding enumeration with 'TryGetNonEnumeratedCount'](https://twitter.com/okyrylchuk/status/1445465841491795975)  

<?# DailyDrop ?>82: 26-05-2022<?#/ DailyDrop ?>
