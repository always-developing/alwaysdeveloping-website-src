---
title: "Automatic redirect on http status code"
lead: "Using UseStatusCodePagesWithRedirects to automatically redirect to an error endpoint"
Published1: "10/10/2022 01:00:00+0200"
slug: "10-status-code-pages"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - httpstatus
   - statuscode

---

## Daily Knowledge Drop

The `UseStatusCodePagesWithRedirects` method can be used to redirect the client to an error page in the case of a non-successful request (400-599 range)

This method returns a _301-Found_ to the client, and then redirects to the _redirect endpoint_ which will return a status code of _200-Success_.

This is often used when the app:
- Should redirect the client to a different endpoint, perhaps to have a different application process the error. In the browser, the redirect endpoint will be reflected
- Doesn't need to preserve and return the original status code with the initial redirect response back to the client

---

## Middleware setup

First step is to enable the automatic redirect functionality - this step is very straight-froward, and entails adding a single middleware components:

``` csharp
var app = builder.Build();

// other middleware setup goes here
app.UseStatusCodePagesWithRedirects("/error?status={0}");
// other middleware setup goes here
```

Here, the middleware is instructed to redirect any responses (which are not successful) to the `/error` endpoint. The endpoint can include a `{0}` placeholder which will contain the _http status code_.

---

## Error endpoint

Next, up the `/error` endpoint is defined.

Here the _Map_ method is used - this takes an _IApplicationBuilder_ as a parameter:

``` csharp
app.Map("/error", errorApp =>
{
    errorApp.Run(async context =>
    {
        await context.Response.WriteAsync($"This is a redirected " +
            $"error message status {context.Request.Query["status"]}");
    });
});
```

For all _error_ responses, the message will be output along with the _http status code_.

---

## Execution

A number of other endpoints were also defined to simulate different status code responses:

``` csharp
app.MapGet("/500response", context =>
{
    context.Response.StatusCode = 500;
    return Task.CompletedTask;
});

app.MapGet("/401response", context =>
{
    context.Response.StatusCode = 401;
    return Task.CompletedTask;
});

app.MapGet("/200response", context =>
{
    context.Response.StatusCode = 200;
    return Task.CompletedTask;
});
```

Browsing to the `/500response` from a browser, for example, will return the following:

``` terminal
This is a redirected error message status 500
```

And looking at the _Network_ tab of the browse, one can see that the initial status code returned is `302`, followed by the `200 on redirect`

![HTTP Status code](statuscode.png)

Now, on error (500 status code), the caller is routed to a generic error page, with a _200_ response, and the actual error in the content of the page.

---

## Notes

For a lot of situations this might not be especially useful, as the return message and status code is not especially useful to the calling application to act on. 

For a customer facing application this could be useful (providing the actual error is logged and recorded somewhere) as the customer doesn't care about the details of the eror, but for a backend api-to-api call, the string response is not especially useful.

---

## References

[Status Pages](https://github.com/dodyg/practical-aspnetcore/tree/net6.0/projects/diagnostics/diagnostics-5)   
[UseStatusCodePages](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-6.0#usestatuscodepageswithredirects)   

<?# DailyDrop ?>178: 10-10-2022<?#/ DailyDrop ?>
