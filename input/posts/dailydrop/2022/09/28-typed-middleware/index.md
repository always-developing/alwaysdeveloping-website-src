---
title: "Strongly typed middleware with IMiddleware"
lead: "Using IMiddleware to ensure correctly configured middleware"
Published: "09/28/2022 01:00:00+0200"
slug: "28-typed-middleware"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - middleware
   - typed
   - stronglytyped

---

## Daily Knowledge Drop

To create `custom ASP.NET Core middleware`, all that is required is a class with an `Invoke or InvokeAsync method`. However, if configured incorrectly, the error will not be apparent until _runtime_.

The `IMiddleware` interface can be used to ensure custom middleware contains the correct method, through the implementation of the interface, ensuring any configuration errors are apparent at _compile time_.

---

## Weakly typed

To define a _working_ middleware component, a class with an `Invoke` or `InvokeAsync` method needs to be defined:

``` csharp
public class WeakTypeMiddleware
{
    private readonly RequestDelegate _next;

    public WeakTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        Console.WriteLine($"Hello from {nameof(WeakTypeMiddleware)}");

        await _next.Invoke(httpContext);
    }
}
```

And the components must be added to the middleware pipeline:

``` csharp
app.UseMiddleware<WeakTypeMiddleware>();
```

However, `nothing is preventing the middleware component from accidentally being configured incorrectly` - in the below example the class doesn't contain a method with the correct name:

``` csharp
public class WeakTypeMiddleware
{
    private readonly RequestDelegate _next;

    public WeakTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // incorrect method name used
    public async Task InvokePipeline(HttpContext httpContext)
    {
        Console.WriteLine($"Hello from {nameof(WeakTypeMiddleware)}");

        await _next.Invoke(httpContext);
    }
}
```

The above class is a _valid class_, but not a valid _middleware component_. The above code will compile, but an exception will occur when trying to call an endpoint which makes use of the component:

``` terminal
System.InvalidOperationException: 'No public 'Invoke' or 'InvokeAsync' 
    method found for middleware of type 'WeakTypeMiddleware'.'
``` 

The exception tells us exactly what the problem is, and how to resolve it - it is easily fixed. However a better and cleaner approach is to _never encounter the exception in the first place_.

---

## Strongly typed

A middleware component can be strongly typed by using the `IMiddleware` interface:

``` csharp
public class StrongTypeMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine($"Hello from {nameof(StrongTypeMiddleware)}");

        await next.Invoke(context);
    }
}
```

The interface enforces the `InvokeAsync` method on the implementation, ensuring it will be configured correctly. 

A slight difference between the two,is that with the _weakly typed_ implementation, the `next delegate is passed into the constructor`, however with the _strongly typed_ implementation, the `next delegate is passed into the InvokeAsync method`.

---

## Notes

Implementing _strongly typed_ middleware using `IMiddleware` instead of having the components _weakly typed_ is a minor code change, and may not offer any immediately obvious benefits - however, using the interface will ensure code uniformity across all middleware components, as well as make it easier to find and identify middleware components in a large code base.


---

## References

[Strongly Typed Middleware in ASP.NET Core](https://www.mikesdotnetting.com/article/359/strongly-typed-middleware-in-asp-net-core)   

<?# DailyDrop ?>170: 28-09-2022<?#/ DailyDrop ?>
