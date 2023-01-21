---
title: "Task.Delay accuracy"
lead: "Learning about the accuracy of Task.Delay with small timee frames"
Published: "01/23/2023 01:00:00+0200"
slug: "23-task-delay"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - task
   - delay
   - accuracy

---

## Daily Knowledge Drop

`Task.Delay` relies on the underlying operating system's internal timer, which for most Windows environments, takes about 15ms to resolve. This means that the _minimum amount of time that can be accurately used with Task.Delay is approximately 15ms (on Windows)_

---

## Sample

`System.Diagnostics.StopWatch` can be used to benchmark how long a `Task.Delay` call actually takes:

``` csharp
Stopwatch? watch = Stopwatch.StartNew();

await Task.Delay(100);

watch.Stop(); 

Console.WriteLine($"Actual time delayed: {watch.ElapsedMilliseconds} milliseconds");
```

Executing the above, the result (which may vary each execution and per machine):

``` terminal
Actual time delayed: 110 milliseconds
```

Even though the code is specifying a 100ms delay, and actual delay is close to 110ms.

The same _inaccurate_ delay is seen when trying to delay for a small precise time:

``` csharp
Stopwatch? watch = Stopwatch.StartNew();

await Task.Delay(5);

watch.Stop(); 

Console.WriteLine($"Actual time delayed: {watch.ElapsedMilliseconds} milliseconds");
```

The output:

``` terminal
Actual time delayed: 19 milliseconds
```

Results may vary, but (in my case) the true delay was never less than 17ms. As mentioned, this is due to the underlying operating system's internal timer.

---

## Notes

If requiring small precise waiting times, `Task.Delay` is not the way to go. In fact there are no "easy" ways to wait for such small precise times - there are ways to do it (which will not be shown here), but they are involved and are often not very performant when it comes to resource usage.

---

## References

[Don’t use Task.Delay for small precise waiting times](https://linkdotnet.github.io/tips-and-tricks/async_await/#dont-use-taskdelay-for-small-precise-waiting-times)  

<?# DailyDrop ?>240: 23-01-2023<?#/ DailyDrop ?>
