---
title: "Map all urls with IApplicationBuilder.Run"
lead: "Leveraging the IApplicationBuilder.Run method to map all URLs to an endpoint delegate"
Published: "10/04/2022 01:00:00+0200"
slug: "04-app-run"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - app
   - run
   - map

---

## Daily Knowledge Drop

The `Run` method on the `WebApplication` (which implements _IApplicationBuilder_) can be used to map **any and all** urls to the specific delegate.  

This can be leveraged in the case where the api doesn't expose any functional endpoints (such as with a _Background Service_) to always display a consistent message to the caller.

---

## Root endpoint

Usually when a web application doesn't expose any endpoints, the root endpoint might be mapped so when browsing to the url, a message is returned to at least inform the caller that the service is up and running:

``` csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<RunMiddleware>();

WebApplication? app = builder.Build();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Sample application - no endpoints exposed");
});

app.Run();
```

Browsing to `/` will display:

``` terminal
Sample application - no endpoints exposed
```

The downside of this approach, is that if the user browses to any URL other than `/`, nothing will be returned. For example, browsing to `/api` just returns a blank page.

---

## Run method

The `Run` method can be leveraged to map a _delegate_ to **all** endpoints:

``` csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<RunMiddleware>();

WebApplication? app = builder.Build();

app.Run(async context =>
{
   await context.Response.WriteAsync("Sample application - no endpoints exposed");
});

app.Run();
```

With this approach, browsing to `/` or `/api` or **any** other url, will return the following:

``` terminal
Sample application - no endpoints exposed
```

Now, no matter the endpoint called - the `same message` is returned.

---

## Endpoint override

A word of warning - using the `Run` method will cause the specific delegate to be called on all endpoints - even if another endpoint has specifically been defined:

``` csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<RunMiddleware>();

WebApplication? app = builder.Build();

app.MapGet("hello", async context =>
{
    await context.Response.WriteAsync("World");
});

app.Run(async context =>
{
    await context.Response.WriteAsync("Sample application - no endpoints exposed");
});

app.Run();
```

In the above, browsing to `/hello` will return `Sample application - no endpoints exposed` and **NOT** `World` - the _Run_ method delegate overrides all over endpoint delegates.

---

## Notes

This is not a technique to be used for every application, but in the cases where the application is running as a background service (for example) which doesn't expose any specific endpoints, this technique can be leveraged to always display _something_ to the user.

---

## References

[Application Environment](https://github.com/dodyg/practical-aspnetcore/blob/net6.0/projects/application-environment/Program.cs)   

<?# DailyDrop ?>174: 04-10-2022<?#/ DailyDrop ?>
