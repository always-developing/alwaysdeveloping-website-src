---
title: "Cancelling a collection iteration"
lead: "Using a CancellationToken to exit a collection iteration"
Published: "01/16/2023 01:00:00+0200"
slug: "16-for-cancellation"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - foreach
   - cancellation

---

## Daily Knowledge Drop

When iterating over a collection or items, the `TakeWhile` method can be used in conjunction with a `CancellationToken` to successfully stop iterating if the parent process is cancelled.

---

## No CancellationToken

Generally, when iteration through a collection, a `foreach` (or `for`) loop is used:

``` csharp
async Task ExecuteLoopNoCancel()
{
    // generate a collection of 100 items
    var iterationItems = Enumerable.Range(0, 100);

    // iterate through the 100 items
    foreach (var item in iterationItems)
    {
        Console.WriteLine($"Processing item: {item}");
        await Task.Delay(500);
    }
}
```

Calling the above `/ExecuteLoopNoCancel` endpoint has the following output:

```terminal
Processing item: 0
Processing item: 1
Processing item: 2
Processing item: 3
.
.
```

The issue with the above, and _especially in an endpoint which can be cancelled at any time_, is that if the parent process is cancelled, the `iteration of the collection will continue`.

If the endpoint is called (from Postman, for example), and the request is `cancelled`, the _loop will continue to execute in the background_ (and output to the console), as it is being executed `asynchronously`.

---

## With CancellationToken

To correctly `cancel the iteration` when the parent process is cancelled, a `CancellationToken` should be passed from the top of the stack all the way down to the loop:

``` csharp
// pass in a CancellationToken
async Task ExecuteLoop(CancellationToken cnlTkn)
{
    var iterationItems = Enumerable.Range(0, 100);

    // only loop while the CancellationToken is not cancelled
    foreach(var item in iterationItems
        .TakeWhile(_ => !cnlTkn.IsCancellationRequested))
    {
        Console.WriteLine($"Processing item: {item}");
        await Task.Delay(500);
    }
}
```

- A `CancellationToken` token is passed into the endpoint - this is _linked_ to client, so if the request is _cancelled from the client side, the CancellationToken is "notified"_ of the cancellation
- In the _foreach_ loop, the `TakeWhile` method is called, with a `CancellationToken` passed in as a parameter


Now, when the endpoint is called (from Postman, for example), and the request is `cancelled`, the _CancellationToken is marked as cancelled (IsCancellationRequested is set to true), and the iteration will stop_.


---

## Notes

When performing iterations over a collection, where it makes sense (in situations when the calling process can be cancelled, such as in an endpoint), a `CancellationToken` instance should always be used. This ensures any iterations being done asynchronously will be cancelled, ensuring resources are not being unessacary used.

---

## References

[@vekzdran@hachyderm.io Tweet](https://twitter.com/vekzdran/status/1610197203901091840)  

<?# DailyDrop ?>235: 16-01-2023<?#/ DailyDrop ?>
