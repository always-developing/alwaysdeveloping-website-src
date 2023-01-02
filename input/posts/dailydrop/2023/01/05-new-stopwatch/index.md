---
title: "Efficient Stopwatch usage"
lead: "New .NET 7 feature to make StopWatch usage more efficient"
Published: "01/05/2023 01:00:00+0200"
slug: "05-new-stopwatch"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - stopwatch
   - benchmark

---

## Daily Knowledge Drop

.NET 7 introduced new `StopWatch` functionality, which not only makes using the class easier.

---

## Typical usage

The `StopWatch` class is often used to perform simple benchmarks to determine how long a piece of code executes for. The _StopWatch_ is started, the code in question is executed, and then the _StopWatch_ is stopped and the ElapsedTime is captured.

Typically the `StopWatch` class is used as follows:

``` csharp
// new instance of Stopwatch
Stopwatch sw = new Stopwatch();
sw.Start();

await Task.Delay(500);

sw.Stop();
Console.WriteLine($"Time Elapsed: {sw.Elapsed}");
```

---

## Improved usage

The issue with the above is that memory is allocated for the _StopWatch_ instance declared. Instead, a more memory efficient approach is to do the following:

``` csharp
// get the start time (in ticks)
long startTime = Stopwatch.GetTimestamp();

await Task.Delay(500);

// get the end time (in ticks)
long endTime = Stopwatch.GetTimestamp();
// calculate the difference
TimeSpan elapsedTime = new TimeSpan(endTime - startTime);
Console.WriteLine($"Time Elapsed: {elapsedTime}");
```

No memory is allocated in this example, but it is slightly more complex to code than the previous example. 

---

## .NET 7 improvements

A new method, `Stopwatch.GetElapsedTime`,  was introduced with .NET 7 which makes the above approach simpler:

``` csharp
// get the start time (in ticks)
long startTime = Stopwatch.GetTimestamp();

await Task.Delay(500);

// get the elapsed time between the startTime and the currentTime
TimeSpace elapsedTime = Stopwatch.GetElapsedTime(startTime);
Console.WriteLine($"Time Elapsed: {elapsedTime}");
```

The `GetElapsedTime` method will calculate the elapsed time between the _startTime_ and the _current time_ (it is also possible to explicitly supply an _endTime_ to the method as well).

Here we get the benefit of less memory allocations, and also not having to do additional calculations. 

---

## Notes

A small, useful new method which makes using `StopWatch` cleaner and more efficient overall - one doesn't want diagnostic tools to be detrimental to the performance and readability of the application code.
Having said that, having a single StopWatch instance should really not impact application performance at all - however its always a good practice to be as efficient as possible, traded off with effort, and in this case, there is no additional effort required to be slightly more efficient.

---


## References

[Are you using the Stopwatch efficiently in .NET?](https://www.youtube.com/watch?v=NTz99yN2urc)  

<?# DailyDrop ?>228: 05-01-2023<?#/ DailyDrop ?>
