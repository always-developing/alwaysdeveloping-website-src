---
title: "Eliding await keyword"
lead: "How and why the await keyword should be elided"
Published: "01/19/2023 01:00:00+0200"
slug: "19-elide-await"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - async
   - await
   - elide

---

## Daily Knowledge Drop

When multiple _async method_ are called in a sequence, the `async methods should be elided` and the _Tasks_ should be _awaited_ and not passed up the call stack.

If a _Task_ is passed up the stack, and an exception occurs - the Task which is not _awaited_ will not be part of the error stack trace.

---

## No await exception

In the below code snippet, we have a call stack where a _Task_ is `not awaited immediately`, but passed up the call stack to be awaited:

``` csharp
await CallStackStart();

static async Task CallStackStart()
{
    try
    {
        // call a method which returns a task
        await NoAwaitMethod();
    }
    catch (Exception e)
    {
        Console.WriteLine("Stacktrace for the exception:");
        Console.WriteLine(e);
    }
}

// return a Task
static Task NoAwaitMethod()
{
    // call a method which returns a Task but 
    // do NOT await
    return ThrowExceptionAsync();
}

static async Task ThrowExceptionAsync()
{
    await Task.Delay(1);
    throw new Exception("Manual exception has been thrown");
}
```

Starting from the bottom of the call stack:
- `ThrowExceptionAsync` is an _async_ method in which an exception could (will in this example) be thrown.
- `NoAwaitMethod` calls into _ThrowExceptionAsync_, but **does not await the call**. The method returns the _Task_ returned from _ThrowExceptionAsync_
- `CallStackStart` calls into _NoAwaitMethod_ and _awaits_ the _Task_ the method returns - which is the _Task_ returned from _ThrowExceptionAsync_.

Running the above code, the call stack generated from the exception is:

``` terminal
Stacktrace for the exception:
System.Exception: Manual exception has been thrown
   at Program.<<Main>$>g__ThrowExceptionAsync|0_2() in C:\Development\Blog\ElideAwait\Program.cs:line 28
   at Program.<Main>$(String[] args) in C:\Development\Blog\ElideAwait\Program.cs:line 5
```

The stack trace has `no mention that the call stack went through the NoAwaitMethod method!`. This is due to the fact that the method is basically just a _pass through method_ for the _Task_.

To get a more accurate stack trace, the _async method needs to be awaited_.

---

## await exception

As mentioned above, instead of a method like _NoAwaitMethod_, which serves as a pass through for the _Task_, the `Task should be awaited`:

``` csharp
wait CallStackStart();

static async Task CallStackStart()
{
    try
    {
        // call a method which returns a task
        await AwaitMethod();
    }
    catch (Exception e)
    {
        Console.WriteLine("Stacktrace for the exception:");
        Console.WriteLine(e);
    }
}

static async Task AwaitMethod()
{
    // await the task returned instead
    // of just returning the Task
    await ThrowExceptionAsync();
}

static async Task ThrowExceptionAsync()
{
    await Task.Delay(1);
    throw new Exception("Manual exception has been thrown");
}
```

Again, starting from the bottom of the call stack:
- `ThrowExceptionAsync` is an _async_ method in which an exception could (will in this example) be thrown.
- `AwaitMethod` calls into _ThrowExceptionAsync_, and **awaits the call**.
- `CallStackStart` calls into _NoAwaitMethod_ and _awaits_ the _Task_ the method returns.

Now running the above code, the full complete stack trace is part of the exception:

``` terminal
Stacktrace for the exception:
System.Exception: Manual exception has been thrown
   at Program.<<Main>$>g__ThrowExceptionAsync|0_2() in C:\Development\Blog\ElideAwait\Program.cs:line 28
   at Program.<<Main>$>g__AwaitMethod|0_1() in C:\Development\Blog\ElideAwait\Program.cs:line 21
   at Program.<Main>$(String[] args) in C:\Development\Blog\ElideAwait\Program.cs:line 6 
```

This time we have a `full, complete stack trace!`

---

## Notes

When _await async_ is used in conjunction with any _exceptions_, the Tasks in question should be elided and _awaited_ and not passed up the stack trace as it can cause certain methods to be omitted from exception stack traces.

---

## References

[Elide await keyword - Exceptions](tps://linkdotnet.github.io/tips-and-tricks/async_await/)  

<?# DailyDrop ?>238: 19-01-2023<?#/ DailyDrop ?>
