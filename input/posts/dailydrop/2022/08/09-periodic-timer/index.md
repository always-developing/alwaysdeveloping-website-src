---
title: "Accurate, regular scheduling with PeriodicTimer"
lead: "Using PeriodicTimer to build an accurate, reliable background service"
Published: "08/09/2022 01:00:00+0200"
slug: "09-periodic-timer"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - timer
    - schedule
    - interval

---

## Daily Knowledge Drop

.NET 6 introduced a new timer, the `PeriodicTimer` which provides a more regular, accurate and reliable way to run code on a specific interval when compared with previous methods.

---

## Task.Delay 

Consider a simple background process which needs to run every second:

``` csharp
async Task StartJobAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        Console.WriteLine(DateTime.Now.ToString("O"));
        
        // Do work
        
        await Task.Delay(1000, token);
    }
}
```

The method is started, and while a cancellation is not requested via the _CancellationToken_, the date time is output, and _work_ is done.

Executing the above as follows:

``` csharp
var cancelSource = new CancellationTokenSource();
var jobTask = StartJobAsync(cancelSource.Token);

Console.ReadKey();
cancelSource.Cancel();
Console.ReadKey();
```

Provides the following output:

``` terminal
2022-07-13T19:48:38.9682679+02:00
2022-07-13T19:48:39.9743395+02:00
2022-07-13T19:48:40.9755190+02:00
2022-07-13T19:48:41.9913331+02:00
2022-07-13T19:48:42.9977149+02:00
2022-07-13T19:48:44.0130672+02:00
2022-07-13T19:48:45.0289959+02:00
```

Even without any work being done in the iteration, the loop does not iterate every second - in `second 43 no work is done at all`. At time goes on the divergence from the start time will just grow larger.

This divergence is even more evident if a job is executed and takes longer than the interval:

``` csharp
async Task StartJobAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        Console.WriteLine(DateTime.Now.ToString("O"));

        // generate a random number between 0 and 3
        var random = new Random();
        if (random.Next(3) == 1)
        {
            // if the random number was 1, simulate a long 
            // running job
            Console.WriteLine("Running long");
            await Task.Delay(1500);
        }

        await Task.Delay(1000, token);
    }
}
```

Here the random _work_ being performed takes `1.5 second`, so instead of the loop taking `1 second`, it becomes `2.5 seconds`. If the work being done is variable in time, then the interval also becomes varied and irregular:

``` terminal
2022-07-13T19:54:18.5637838+02:00
2022-07-13T19:54:19.6099211+02:00
2022-07-13T19:54:20.6106040+02:00
Running long
2022-07-13T19:54:23.1278907+02:00
2022-07-13T19:54:24.1338937+02:00
2022-07-13T19:54:25.1360588+02:00
2022-07-13T19:54:26.1519535+02:00
Running long
2022-07-13T19:54:28.6815481+02:00
2022-07-13T19:54:29.6844306+02:00
```

This might be sufficient, however if a more regular, consistent interval is required, look no further than the `PeriodicTimer`.

---

## Periodic Timer

.NET 6 introduce the `PeriodicTimer` which runs on a more regular, predictable schedule, using more modern practices:

``` csharp
async Task StartJobAsync(CancellationToken token)
{
    // define the timer to tick every second
    var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

    // loop every tick while the token has not been cancelled
    while (await timer.WaitForNextTickAsync() && !token.IsCancellationRequested)
    {
        Console.WriteLine(DateTime.Now.ToString("O"));
    }
}
```

With the `PeriodicTimer` the interval period is defined when the timer is declared, and the _WaitForNextTickAsync_ method will return true every interval period.

Executing the above as follows:

``` csharp
var cancelSource = new CancellationTokenSource();
var jobTask = StartJobAsync(cancelSource.Token);

Console.ReadKey();
cancelSource.Cancel();
Console.ReadKey();
```

Provides the following output:

``` terminal
2022-07-14T07:23:29.1553510+02:00
2022-07-14T07:23:30.1430203+02:00
2022-07-14T07:23:31.1502777+02:00
2022-07-14T07:23:32.1522676+02:00
2022-07-14T07:23:33.1541686+02:00
2022-07-14T07:23:34.1603689+02:00
2022-07-14T07:23:35.1475597+02:00
2022-07-14T07:23:36.1498775+02:00
2022-07-14T07:23:37.1561782+02:00
2022-07-14T07:23:38.1425797+02:00
```

There are variations on the milliseconds, but the `PeriodicTimer` will adjust to keep it as regular as possible.

When introducing _work_ which exceeds the interval period, the `PeriodTimer` will still try to adjust to keep the ticks as regular as it can:

``` csharp
async Task StartJobAsync(CancellationToken token)
{
    var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

    while (await timer.WaitForNextTickAsync() && !token.IsCancellationRequested)
    {
        Console.WriteLine(DateTime.Now.ToString("O"));

        // randomly perform some work which takes 1.5 seconds
        var random = new Random();
        if (random.Next(3) == 1)
        {
            Console.WriteLine("Running long");
            await Task.Delay(1500);
        }
    }
}
```

From the results one can see that if a tick gets skipped due to the work running long, the next tick will `align with the starting tick` and not when the previous tick completed:

``` terminal
2022-07-14T07:27:22.2437461+02:00
Running long
2022-07-14T07:27:23.7708051+02:00
2022-07-14T07:27:24.2487034+02:00
2022-07-14T07:27:25.2491709+02:00
Running long
2022-07-14T07:27:26.7629854+02:00
2022-07-14T07:27:27.2392793+02:00
Running long
2022-07-14T07:27:28.7631133+02:00
2022-07-14T07:27:29.2430825+02:00
Running long
2022-07-14T07:27:30.7529474+02:00
2022-07-14T07:27:31.2606374+02:00
Running long
2022-07-14T07:27:32.7737681+02:00
2022-07-14T07:27:33.2500507+02:00
```

The timer will always execute on a regular, predictable schedule, even if the work runs long.

---

## Notes

The `Periodic Timer` is a great addition to the ecosystem. Reliable and easy to use, for simple background processing, this should be the go-to mechanism going forward. For more complex scheduling and processing, something like [Hangfire](https://www.hangfire.io/) will still have its place for a while to come.

---

## References

[Scheduling repeating tasks with .NET 6’s NEW Timer](https://www.youtube.com/watch?v=J4JL4zR_l-0)   

---

<?# DailyDrop ?>134: 09-08-2022<?#/ DailyDrop ?>
