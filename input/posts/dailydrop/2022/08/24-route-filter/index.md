---
title: "Route handler filters in .NET 7"
lead: "Learning about minimal api route handler filters coming in .NET 7"
Published: "08/24/2022 01:00:00+0200"
slug: "24-route-filter"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - minimalapi
   - .net7
   - routehandler
   - filter

---

## Daily Knowledge Drop

Coming with .NET 7 (most likely, .NET 7 is still in preview so its not 100% guaranteed to be included), is the `IRouteHandlerFilter` interface, which allows for _intercepting_  requests/response to and from a specific minimal endpoint.

This enabled `cross-cutting concerns to be coded once`, and then applied to the relevent endpoints. They operate _similar_ to the .NET `middleware` but are applied at a specific endpoint level, and not a level higher targeting all routes.

---

## IRouteHandlerFilter
### Definition

First, lets define the `IRouteHandlerFilter` implementation to later be applied to a minimal api endpoint. In the below example, a router handler filter is defined to measure how long a call to the endpoint takes:

``` csharp
public class RouteLogger : IRouteHandlerFilter
{
    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, 
        RouteHandlerFilterDelegate next)
    {
        // record the time before the endpoint (or next filter is called)
        Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
            $"RouteLogger - before endpoint called");

        var result = await next.Invoke(context);

        // record the time after the endpoint (or next filter is called)
        Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
            $"RouteLogger - after endpoint called");

        return result;
    }
}
```

The `IRouteHandlerFilter` interface only has one method to implement - _InvokeAsync_, which takes two parameters:

- `RouteHandlerInvocationContext`:  this contains a reference to the _HttpContext_, as well as a list of _Arguments_ which can be modified to be passed between filters
- `RouteHandlerFilterDelegate`: this contains a delegate to the next _IRouteHandlerFilter_ implementation if multiple have been applied, otherwise it will route to the _actual endpoint handler_

In the above sample, a message is logged when the method is entered, the _next_ delegate is invoked, and then a message logged just before the return. If _next_ is not invoked, the pipeline to the endpoint is `short-circuited` and the `endpoint handler will never be invoked`. This allows checks or validation to be performed (authentication checks for example), and _short-circuit_ if the checks fail.

### Application

Applying the filter to the endpoint is very simple:

``` csharp
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/endpoint", () =>
{
    Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
        $"Endpoint handler");

    return "Endpoint has been called";
}).AddFilter<RouteLogger>(); // add the filter

app.Run();
```

On the endpoint definition, the _AddFilter_ method is called, with the `IRouteHandlerFilter` implementation specified. Multiple implementations can be linked together to form a pipeline to the endpoint handler.

<?# InfoBlock ?>
The above was written using **.NET 7 Preview 5**. AddFilter() has been renamed to _AddRouteHandlerFilter()_ in Preview 6 and will be renamed again to _AddEndpointFilter()_ starting in Preview 7.
<?#/ InfoBlock ?>

---

## Middleware

As mentioned in the introduction, _route handler filters_ act similar to the middleware. Where `route handler filters are applied to specific endpoints`, `middleware is applied to requests coming in on any route`.

Below a middleware function is defined, which also performs logging:

``` csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// define the middleware
// The Func defined is called on every 
// request to any endpoints
app.Use(async (context, next) =>
{
    Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
        $"Middleware - before endpoint called");

    await next(context);

    Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
        $"Middleware - after endpoint called");
});

app.MapGet("/endpoint", () =>
{
    Console.WriteLine($"{DateTime.Now:MM/dd/yyyy hh:mm:ss.fff}: " +
        $"Endpoint handler");

    return "Endpoint has been called";
}).AddFilter<RouteLogger>();

app.Run();

```

The middleware has a similar structure to a _route handler filter_, with an _HttpContext_ and a _RequestDelegate_ as arguments. The _RequestDelegate_ argument `next` is also invoked, to call the next middleware component, or the endpoint handler. If not called, _short-circuiting_ will occur, just as with the _route handler filter_.

In the above example, the `middleware was defined as a lambda function` while the `filter was defined as a concrete IRouteHandlerFilter implementation` - either of these can also be defined the either way. Middleware could be defined as a concrete implementation, and a route handler filter could be defined as a lambda function.  
If there are a number of middleware or route handler filter components being used, it's usually better to use the concrete implementation method to keep the startup code cleaner, and keep all pipeline logic in one place (their own folder, for example).

Executing the above code and browsing to the `/endpoint`, results in the following output:

``` terminal
07/27/2022 08:42:27.851: Middleware - before endpoint called
07/27/2022 08:42:27.859: RouteLogger - before endpoint called
07/27/2022 08:42:27.860: Endpoint handler
07/27/2022 08:42:27.860: RouteLogger - after endpoint called
07/27/2022 08:42:27.865: Middleware - after endpoint called
```

As expected, the `middleware is called first`, before the http request is passed onto the endpoint specific `route handler filter(s)` (if any are defined), before the `actual endpoint handler` is called.

---

## Notes

As mentioned, `middleware` is applied to all endpoints, so if some middleware logic is only applicable to a certain endpoint(s), then currently filter logic needs to be specific in the middleware to determine if the middleware functionality should be applied to the request or not. 
Having the ability to granularly apply `route handler filter` "middleware" on specific endpoint(s) allows for greater flexibility and is a welcome addition which brings functionality closer to being on par with that of MVC (which has ActionFilter)

---

## References

[Minimal API Route Handler Filters](https://khalidabuhakmeh.com/minimal-api-route-handler-filters)   

---

<?# DailyDrop ?>145: 24-08-2022<?#/ DailyDrop ?>
