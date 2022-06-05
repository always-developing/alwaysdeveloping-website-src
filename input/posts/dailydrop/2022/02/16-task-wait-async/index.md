---
title: "Task.WaitAsync in .NET6"
lead: "Exploring the new WaitAsync method introduced in .NET6"
Published: 02/16/2022
slug: "16-task-wait-async"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - task
    - wait
    - async
    - await
    
---

## Daily Knowledge Drop

A new method on `Task` called `WaitAsync` was introduced in .NET6. This method allows for _waiting on a Task for a specific period of time before throwing a timeout exception_.  

On the surface, this might not seem very useful, but lets look at some examples to see how this new method can be leveraged. 

---

## The issue
### Long running processes

Suppose we have a long running method which returns a Task - in the below example we are simulating a _download process which takes 5 seconds_

``` csharp
await LongRunningProcessAsync();

async Task LongRunningProcessAsync()
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Starting large download...");
    for (int x = 0; x < 4; x++)
    {
        await Task.Delay(1000);
    }
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Download Complete");
}
```

As probably expected, the output is a follows, with 5 seconds gap between the start and completion of the "download":

``` powershell
06:51:29.4337633 => Starting large download...
06:51:34.4755862 => Download Complete
``` 

In the above example, the long running process is capped at 5 seconds, but in a real world situation, this would be an **unknown number**, dependant the _size of the file downloading_, _network speed and availability_ etc.

So how can we `force a cap on the download time`, and if it takes `longer than the cap the process is cancelled`, and feedback can be given back to the user.

---

## Solutions
### Cancellation token

The **best** way to handle cancellation of the task would be by using a `CancellationToken`.

``` csharp

// A new CancellationTokenSource is used, with a timeout of 1 second
using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
{
    try
    {
        // The long running method is called, and has been 
        // updated to accept a CancellationToken, which is supplied
        await LongRunningProcessAsync(cts.Token);
    }
    catch (TimeoutException)
    {
         Console.WriteLine($"{DateTime.Now.TimeOfDay} => " +
            $"Download took too long and was cancelled");
    }
}

async Task LongRunningProcessAsync(CancellationToken token)
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Starting large download...");
    for (int x = 0; x < 4; x++)
    {
        // Every iteration, the token is checked to see if a cancellation 
        // was requested (which in this example would happen after 1 second)
        if(token.IsCancellationRequested)
        {
            // If a cancellation has been requested, throw an 
            // exception and abort the long running process
            throw new TimeoutException();
        }

        await Task.Delay(1000);
    }
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Download Complete");
}
```

The output is as follows:

``` powershell
07:07:28.2537474 => Starting large download...
07:07:29.3237966 => Download took too long and was cancelled
```

PROS:
- The long running process stops when the cancellation occurs

CONS:
- The method needs to change to support a cancellation token as a parameter

In this example changing the method to support the cancellation token was straightforward, and many existing third party libraries already support cancellation tokens.  

But how could the same problem be solved `when the method does not support a cancellation token and cannot be changed`?

---

### Parallel tasks

One method is to `start a second task which runs for a set period of time, in parallel to the long running process`. We wait for one of the tasks to finish and based on which finished first, we can handle how to proceed.

``` csharp
// start the process of creating cancellation token
using (var cts = new CancellationTokenSource())
{
    try
    {
        // create a timeout delay task, which will run for 1 second
        var timeout = TimeSpan.FromSeconds(1);
        var timeoutTask = Task.Delay(timeout, cts.Token);

        // wait for either the timeout task OR the 
        // long running task to finish
        var firstTask = await Task.WhenAny(new[] { timeoutTask, 
            LongRunningProcessAsync() });

        // if the first task to finish was the timeout task, 
        // we can throw a timeout exception as the long running 
        // process has now exceeded the allowed timeout
        if (firstTask == timeoutTask)
        {
            throw new TimeoutException();
        }
        else
        {
            cts.Cancel();
        }
    }
    catch (TimeoutException)
    {
        Console.WriteLine($"{DateTime.Now.TimeOfDay} => " +
            $"Download took too long and was cancelled");
    }
}

// This method is unable to change, so no CancellationToken can be used
async Task LongRunningProcessAsync()
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Starting large download...");
    for (int x = 0; x < 4; x++)
    {
        await Task.Delay(1000);
    }
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Download Complete");
}
```

**Note:** The above could have been done without the use of the _CancellationTokenSource_ and use of _cts.Token_. However, whenever possible if a method (**Task.Delay** in this case) supports a cancellation token parameter, it should be used.

The output is as follows:

``` powershell
07:56:40.5198266 => Starting large download...
07:56:41.5520772 => Download took too long and was cancelled
07:56:44.5803565 => Download Complete
```

But wait - `the download still completed?`

Unfortunately, because the method doesn't accept a cancellation token (or provide some other way) to handle cancellations, there is no way to truly cancel it.  
What the above technique does, is allow the flow of execution to continue after waiting for the task for a certain period of time. The long running process task will still run to completion in the background.

PROS:
- A long running task can be "cancelled" even without a cancellation token or any modifications to code

CONS:
- The long running task is not truly cancelled, it still executes in the background. Control is just given back to the main processing thread

---

### Task.WaitAsync

The new `Task.WaitASync` method performs the same function as the above technique, it is just much cleaner. 

``` csharp
try
{
    // start the long running process, and wait on the task 
    // for a maximum of 1 second
    await LongRunningProcessAsync()
        .WaitAsync(TimeSpan.FromSeconds(1));
}
catch (TimeoutException)
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => " +
        $"Download took too long and was cancelled");
}
}

// This method is unable to change, so no CancellationToken can be used
async Task LongRunningProcessAsync()
{
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Starting large download...");
    for (int x = 0; x < 4; x++)
    {
        await Task.Delay(1000);
    }
    Console.WriteLine($"{DateTime.Now.TimeOfDay} => Download Complete");
}
```

The output is as follows:

``` powershell
18:02:46.4930592 => Starting large download...
18:02:47.5520137 => Download took too long and was cancelled
18:02:50.5424670 => Download Complete
```

As can be seen, the resulting output is the same, however the code required is a lot more compact and cleaner. Unfortunately, even with this new method, without a cancellation token, the long running task will still execute in the background.

PROS:
- A long running task can be "cancelled" even without a cancellation token 

CONS:
- The long running task is not truly cancelled, it still executes in thee background. Control is just given back to the main processing thread

---

## Conclusion

The preferred solution for cancelling a long running task is to use a _CancellationToken_., and if this is available should always be used. However this is not always possible, and in that case the `Task.WaitAsync` is the best and cleanest way of handling a task "cancellation".

---

## References
[New Task.WaitAsync method in .NET 6](https://www.tabsoverspaces.com/233882-new-task-waitasync-method-in-net-6?utm_source=feed)


<?# DailyDrop ?>12: 16-02-2022<?#/ DailyDrop ?>
