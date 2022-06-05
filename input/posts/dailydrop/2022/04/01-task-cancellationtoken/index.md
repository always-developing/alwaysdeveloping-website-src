---
title: "Cancel a task with OperationCanceledException"
lead: "Why the OperationCanceledException should be used over a soft cancellation"
Published: 04/01/2022
slug: "01-task-cancellationtoken"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - anonymous
    - with

---

## Daily Knowledge Drop

When cancelling tasks using a `CancellationTokenSource`, rather than do a _soft cancellation_, an `OperationCanceledException` should be thrown.

---

## Cancellation Token

A quick summary of _cancellation tokens_ - they "enable cooperative cancellation between threads, thread pool work items or Task objects".  

Basically _cancellation tokens_ are instantiated outside a particular thread, and then passed into the thread, to allow for cancellation from outside the thread.

---

## Examples

In each of the below examples, there is a "long" running process which runs for 10 seconds. A _cancellation token_ is passed in and checked every 1 second to see if a cancellation has been requested - if requested, then an action will be perform to abort/cancel the long running process.

### Soft cancellation

The first example will show how to do a "soft cancellation".

Here, if a cancellation is requested, the developer will decide how to manually cause the method to end its execution.

``` csharp
var cts = new CancellationTokenSource();
var token = cts.Token;

// start the long running process
var longProcessTask = Task.Run(async () =>
{
    for (var i = 0; i < 10; i++)
    {
        // If the token has been requested to cancel, break 
        / out of the loop causing the long processing and 
        // effectively cause the long running task to finish
        if (token.IsCancellationRequested)
            break; // exit from the loop

        Console.WriteLine("Processing...");
        await Task.Delay(1000);
    }
}, token);

// If the user presses a key, the token is cancelled
Console.ReadKey();
cts.Cancel();

await longProcessTask;

Console.WriteLine($"Task is completed: {longProcessTask.IsCompleted}");
```

The problem with this approach, is that there is `no indication if the task actually finished to completion or not`. 

If the _longProcessTask_ was required to finish, before another process was to kick off - we do not know if the _longProcessTask_ finished due to executing to its end, or if it finished due to be cancelled early.

---

### OperationCanceledException

Rather than a "soft cancellation, a `OperationCanceledException` should be used. This provides a clear way of determining if the task was cancelled or finished successfully.

``` csharp
var cts = new CancellationTokenSource();
var token = cts.Token;

// start the long running process
var longProcessTask = Task.Run(async () =>
{
    for (var i = 0; i < 10; i++)
    {
        // If the token has been requested to cancel, 
        // throw an _OperationCanceledException_
        if (token.IsCancellationRequested)
            throw new OperationCanceledException();

        Console.WriteLine("Processing...");
        await Task.Delay(1000);
    }
}, token);

// The longProcessTask is awaited inside a try-catch block. 
try
{
    Console.ReadKey();
    cts.Cancel();

    await longProcessTask;

    Console.WriteLine($"Task is completed: {longProcessTask.IsCompleted}");
}
catch (OperationCanceledException e)
{
    Console.WriteLine($"{nameof(OperationCanceledException)} " +
        $"thrown with message: {e.Message}");
}
```

With this method, when task is cancelled we can now tell vs the task actually finishing to completion.

---

### ThrowIfCancellationRequested

There is a slight improvement which can be made on the above, which is the recommended way of dealing with a cancelled task, and that is to use the `ThrowIfCancellationRequested` method on a token.

Looking at the [source code for the method](https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/mscorlib/system/threading/CancellationToken.cs#L466) one can see that its basically doing the same as the above, just wrapped up neatly into a method.

``` csharp
var cts = new CancellationTokenSource();
var token = cts.Token;

// start the long running process
var longProcessTask = Task.Run(async () =>
{
    for (var i = 0; i < 10; i++)
    {
        // check if a cancellation has been requested
        token.ThrowIfCancellationRequested();

        Console.WriteLine("Processing...");
        await Task.Delay(1000);
    }
}, token);

try
{
    Console.ReadKey();
    cts.Cancel();

    await longProcessTask;

    Console.WriteLine($"Task is completed: {longProcessTask.IsCompleted}");
}
catch (OperationCanceledException e)
{
    Console.WriteLine($"{nameof(OperationCanceledException)} " +
        $"thrown with message: {e.Message}");
}
```

---

## Notes

We've looked at a number of ways to cancel a task. The `soft cancellation`, while successfully cancellation a task, has some limitations and the better option is to use a `OperationCanceledException`, facilitated by the `ThrowIfCancellationRequested` method.

---

## References

[Cancellation Tokens](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-6.0)  

<?# DailyDrop ?>43: 01-04-2022<?#/ DailyDrop ?>
