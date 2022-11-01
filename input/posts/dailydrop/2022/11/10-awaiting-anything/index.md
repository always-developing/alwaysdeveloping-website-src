---
title: "Awaiting anything with GetAwaiter"
lead: "Using a GetAwaiter extension method to await any type"
Published: "11/10/2022 01:00:00+0200"
slug: "10-awaiting-anything"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - await
   - task

---

## Daily Knowledge Drop

All that is required to `await` a specific type, is that the type has a method called `GetAwaiter` which returns an instance of `TaskAwaiter`.

This `GetAwaiter` method can be an _extension method_ - which means `any type can be extended to be awaited`.

---

## Use case: double parse

In general, the below use cases are not a recommendation or proposed best technique for solving the use case - but it's interesting to discover what is possible.

In this use case, we are going to attempt to _make an async version of the double.TryParse method_.

---

### TryParseAsync

This solution doesn't require the usage of the `GetAwaiter` method - in this first option we are just going to create a method which returns a `Task` (and as such, the _Task_ is awaited):

``` csharp
// extension method on string
public static Task<bool> TryParseAsync(this string s, out double result)
{
    // perform the normal TryParse
    if(double.TryParse(s, out result))
    {
        // return a Task instead of the 
        // TryParse response
        return Task.FromResult(true);
    }
    
    // return a Task instead of the 
    // TryParse response
    return Task.FromResult(false);
}
```

This can now be used as follows:

``` csharp
// Instead of doing it this OLD way
if(double.TryParse("100", out double result))
{
    Console.WriteLine("100 is a double");
}

// This is now possible:
// The TryParseAsync is called directly 
// on the string and can be awaited
if(await "100".TryParseAsync(out double result1))
{
    Console.WriteLine("100 is a double");
}
```

Here, the `TryParseAsync` can be used on a string value directly, and as it returns a `Task`, the call can be _awaited_.

---

### await directly

However, what if there is a requirement to perform the same functionality as `TryParseAsync`, but to _await_ the string directly - not call the `TryParseAsync` method. Well, no problem!

``` csharp
public static TaskAwaiter<bool> GetAwaiter(this string s)
{
    return s.TryParseAsync(out double _).GetAwaiter();
}
```

Here, a `GetAwaiter` extension method is created on `string` - by convention, this now allows `string to be awaited directly`. In this instance, awaiting a string will try parse it as a double (using the previously created extension method).

So now this can be done:

``` csharp
// just await the string
if(await "500")
{
    Console.WriteLine("500 is a double");
}
```

As mentioned - this is not really a recommended approach at all - while the code is more concise, the readability is not great, with the code making no contextual sense. However, it is what can be done.

---

## Use Case: TimeSpan

A more useful use case, is to _expand the functionality of TimeSpan_ to make it easier to _wait for specific lengths of time_.

### Delay

Generally when a delay is required in code, a variation of the following is used:

``` csharp
await Task.Delay(TimeSpan.FromMilliseconds(100));
```

Nothing wrong with this, it works. However if used often throughout code, it's it fairly verbose.

---

### await

As before, a `GetAwaiter` method could be added to `TimeSpan` to make it _awaitable_, eliminating the need for the Task.Delay (this will still be required, it will just be wrapped up in the extension method):

``` csharp
// extension method on TimeSpan
public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
{
    return Task.Delay(timeSpan).GetAwaiter();
}
```

This method can now be used as follows:

``` csharp
await TimeSpan.FromMilliseconds(100);
```

Definitely simpler and more concise than the previous version, and I'd argue actually more readable.

---

### await extended

This could even be taken a step further with an addition extension method:

``` csharp
public static TimeSpan MilliSeconds(this int i) => TimeSpan.FromMilliseconds(i);
```

This is an extension method on an `int`, which will convert the integer to an equivalent TimeSpan object.

As we already have an existing extension method to _await_ a `TimeSpan`, the two extensions methods can be used in conjunction - _convert an integer to a TimeSpan, which can then be awaited_:

``` csharp
await 100.MilliSeconds();
```

---

## Notes

While this is a very useful and convenient technique to add the _await_ functionality to _any class_, it doesn't mean every class _should_ have this functionality. In the case of `TryParseAsync`, the code was made less readable, for no real benefit. In the case of `TimeSpan`, the _await_ extension to the class did add value to the developer.  Basically, made an informed choice to use `GetAwaiter`, and only do so where it makes sense.

---

## References

[Cursed C# - Doing shenanigans in C#](https://steven-giesel.com/blogPost/5360d1c3-89f6-4a08-9ee3-6ddbe1b44236)  

<?# DailyDrop ?>199: 010-11-2022<?#/ DailyDrop ?>
