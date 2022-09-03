---
title: "Minimal api's and cancellation tokens"
lead: "How to cancel aborted api calls with cancellation tokens"
Published1: "09/30/2022 01:00:00+0200"
slug: "30-api-cancellation"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - minimalapi
   - cancellation
   - token

---

## Daily Knowledge Drop

A `CancellationToken` can be passed into a minimal api endpoint to be used to _cancel processing executed by the endpoint_ - the cancellation token will automatically be marked as cancelled if the caller either cancels the request or navigates away from the endpoint (if being accessed from the browser)

---

## Process cancellation

### No token

Consider the following minimal api endpoint:

``` csharp
app.MapGet("/long", async () =>
{
    Console.WriteLine($"{DateTime.Now.ToString("ss")}: " +
        $"Starting long process");

    // simulate a long running process
    await Task.Delay(5000);

    Console.WriteLine($"{DateTime.Now.ToString("ss")}: " +
        $"Finishing long process");
});
```

If the application is executed, and the endpoint called, the output would be something like this:

```terminal
02: Starting long process
07: Finishing long process
```

As expected, it takes approximately 5 seconds for the endpoint to execute and finish.

If the endpoint is called again, and during those 5 seconds of "processing", the _"Stop loading this page"_ button is clicked on the browser (wording may differ slightly depending on browser) we see the following:

``` terminal
52: Starting long process
57: Finishing long process
```

Even though the `request from the browser to the endpoint is cancelled, the processing done by the endpoint is not cancelled!`. The endpoint isn't aware of the fact the request was cancelled, which results in unnecessary CPU and memory usage, no longer required as the user has decided to cancel the request.

---

### CancellationToken

`CancellationToken` to the rescue - a _cancellation token_ can be used to convey information to the endpoint about the status of the request.  

Let's update the endpoint to use a cancellation token:

``` csharp
// add cancellation token, which is automatically populated
app.MapGet("/long", async (CancellationToken token) =>
{
    Console.WriteLine($"{DateTime.Now.ToString("ss")}: " +
        $"Starting long process");

    //forward the token onto the method
    await Task.Delay(5000, token);

    Console.WriteLine($"{DateTime.Now.ToString("ss")}: " +
        $"Finishing long process");
});
```

Here, a `CancellationToken` is specified to be passed into the endpoint - this automatically gets instantiated and is _"linked"_ to the HTTP context of the request. If the request is cancelled by the caller, the `CancellationToken` will indicate this fact.

Most _async_ methods (such as `Task.Delay` in this instance) have an optional `CancellationToken` parameter which can be used to pass a token all the way down the call stack to the code doing the actual work/processing, which will then in turn monitor and check if the token has been cancelled.

Invoking this endpoint now, and clicking the _"Stop loading this page"_ button on the browser while the endpoint is still processing will result in the following:

``` terminal
27: Starting long process
fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
      An unhandled exception has occurred while executing the request.
      System.Threading.Tasks.TaskCanceledException: A task was canceled.
```

When a `CancellationToken` is used to cancel a process, it is up to the library author to decide if cancelling means an exception be thrown (such as in the above example with _Task.Delay_), or if processing simply just stops.

While the endpoint `process is now successfully` cancelled when the request is cancelled, not using unnecessary resources - we now have to deal with and cater for the `TaskCanceledException`.

---

### Cancellation middleware

A _custom middleware_ component can be written to catch any exceptions of type `TaskCanceledException`, log it and return a different response to the caller (which they will not care about, as they have cancelled the request):

``` csharp
public class CancellationMiddleware
{
    private readonly RequestDelegate _next;

    public CancellationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // catch any canceled exceptions
        // TaskCanceledException inherits from OperationCanceledException
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Cancelled request handled by {nameof(CancellationMiddleware)}");
            // the called has cancelled the request
            // so doesn't care about the response
            // Totally legit HTTP response code
            context.Response.StatusCode = StatusCodes.Status418ImATeapot;
        }
    }
}
```

This middleware is then registered on startup:

``` csharp
app.UseMiddleware<CancellationMiddleware>();
```

And now when invoking the endpoint and cancelling we see the following output:

``` terminal
18: Starting long process
Cancelled request handled by CancellationMiddleware
```

`Process successfully cancelled` and `no more unhandled exception errors`.

---

## Notes

Adding `CancellationToken` support to a minimal endpoint is a fairly low effort improvement, with a (depending on how the endpoint gets used) potential high reward - processing which is no longer required by the caller gets cancelled, freeing up CPU and memory usage.

---

## References

[Using CancellationTokens in ASP.NET Core minimal APIs ](https://andrewlock.net/using-cancellationtokens-in-asp-net-core-minimal-apis/)   

<?# DailyDrop ?>172: 30-09-2022<?#/ DailyDrop ?>
