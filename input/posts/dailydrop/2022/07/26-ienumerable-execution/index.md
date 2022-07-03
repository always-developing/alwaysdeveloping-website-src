---
title: "IEnumerable's lazy evaluation"
lead: "Defer execution of IEnumerable returning methods until iteration"
Published1: "07/26/2022 01:00:00+0200"
slug: "26-ienumerable-execution"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - ienumerable
    - lazy
    - iteration

---

## Daily Knowledge Drop

When executing a method which returns _IEnumerable_, the method body is `not execute until the result is enumerated over`. Invoking the method will not cause any of the method code to execute, including any code before the first `yield` in the body - not until enumeration.

---

## IEnumerable

A quick summary of `IEnumerable` and `yield` usage - when a method is defined to have a return type of IEnumberable\<T\>, it can be invoked and the results iterated over:

``` csharp
// call the GetNumbers method in the foreach loop
foreach(var number in GetNumbers())
{
    Console.WriteLine(number);
}

public IEnumberable<int> GetNumbers()
{
    yield return 1;
    yield return 2;
    yield return 3;
    yield return 4;
    yield return 5;
}
```

The great benefit of IEnumerable though, comes with its usage in conjunction with the `yield` keyword - this is used inside the method to return a value and (temporarily) yield control to the calling iterator. Once the iteration body (_Console.WriteLine_ in our example) is complete, control is then returned back to the method, which is executed until the next `yield` is encountered.

The output for the above would be:

``` terminal
1
2
3
4
5
```

---

## Lazy evaluation

In the above example we saw how the _GetNumbers_ method was called as part of the iterator (as part of the `foreach`) - but it is also possible to invoke the method, and store the returned _IEnumerable_ for later execution:

``` csharp
// call the GetNumbers method 
IEnumerable<int> numbers = GetNumbers();

// do more processing

// iterate over the IEnumerable<int> variable
foreach (var number in numbers)
{
    Console.WriteLine(number);
}
```

This results in the same output as the previous example above.

---

## Lazy execution

The interesting part about the lazy evaluation (and the reason for this post), is that when using `lazy evaluation`, the method body is `not executed when the method is called`, only when it's `iterated over`:

Consider the following method which returns _IEnumerable\<string\>_, but before it returns a value it will log which value is being returned:

``` csharp
IEnumerable<string> GetStringsWithLogging()
{
    Console.WriteLine("Executing iteration 1");
    yield return "Iteration 1";

    Console.WriteLine("Executing iteration 2");
    yield return "Iteration 2";

    Console.WriteLine("Executing iteration 3");
    yield return "Iteration 3";

    Console.WriteLine("Executing iteration 4");
    yield return "Iteration 4";

    Console.WriteLine("Executing iteration 5");
    yield return "Iteration 5";
}
```

The method is executed as follows:

``` csharp
Console.WriteLine($"Before '{nameof(GetStringsWithLogging)}' called");
var deferLogging = GetStringsWithLogging();
Console.WriteLine($"After '{nameof(GetStringsWithLogging)}' called");

foreach (var item in deferLogging)
{
    Console.WriteLine(item);
}
```

The output of the above is:

``` terminal
Before 'GetStringsWithLogging' called
After 'GetStringsWithLogging' called
Executing iteration 1
Iteration 1
Executing iteration 2
Iteration 2
Executing iteration 3
Iteration 3
Executing iteration 4
Iteration 4
Executing iteration 5
Iteration 5
```

From the output one can see that the body of _GetStringsWithLogging_ is `not invoked` when it is initially called - it is only when the _deferLogging_ variable is iterated over with the _foreach_ loop, that the body of _GetStringsWithLogging_ is executed.

---

## Notes

My initial gut assumption with an IEnumerable method was that the body of the method in question (_GetStringsWithLogging_ here) would execute up until the first `yield` when called, no matter if lazy or not. However working through sample examples, and understanding how the code is lowered, the deferred execution makes more sense - and I am glad my initial assumptions were incorrect.  

Having the ability to defer execution of the method allows for potentially long running processes which retrieve the results data (for example), to be deferred until/if actually needed (obviously all by design, I am sure) it very valuable. The _IEnumerable_ instance can be passed around between methods, and only materialized when required - instead of passing around the (potentially larger in size) materialized data, when it might not even be needed.

---

## References

[C#: IEnumerable, yield return, and lazy evaluation](https://stackoverflow.blog/2022/06/15/c-ienumerable-yield-return-and-lazy-evaluation/)   

---

<?# DailyDrop ?>124: 25-07-2022<?#/ DailyDrop ?>
