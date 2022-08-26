---
title: "Extend the middleware pipeline with IStartupFilter"
lead: "Automatically extend the start or end of the middleware pipeline with IStartupFilter"
Published: "09/22/2022 01:00:00+0200"
slug: "22-startup-filters"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - middleware
   - startupfilter
   - istartupfilter

---

## Daily Knowledge Drop

The `IStartupFilter` interface can be used to extend either the _start_ or the _end_ the middleware pipeline. 

Any implementations of `IStartupFilter` will automatically be called on application build `before any other middleware configuration` is called.

---

## Manual middleware

In the below example the basic _weatherforecast_ template is being extended to contain a manually added middleware component:

``` csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    // print out a message
    Console.WriteLine("Weatherforecast endpoint called");

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// add the custom middleware manually
app.UseMiddleware<ManualMiddleware>();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

The definition of `ManualMiddleware`:

``` csharp
public class ManualMiddleware
{
    private readonly RequestDelegate _next;

    public ManualMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
       // this should be output before the request to 
       // forwarded onto the next component in the pipeline
       Console.WriteLine($"In the manually added " +
            $"middleware: {nameof(ManualMiddleware)}. " +
            $"The current datetime is '{DateTime.Now}'");

        // call the next component with the context
        await _next.Invoke(httpContext);
    }
}
```

When calling the endpoint, the console output is as follows:

``` terminal
In the manually added middleware: ManualMiddleware. The current datetime is '2022/09/20 06:40:23'
Weatherforecast endpoint called
```

As expected, the component added to the middleware is invoked before forwarding the request onto the next component, and eventually the endpoint.

---

## IStartupFilter middleware

The `IStartupFilter` only has one method to implement, `Configure`, which takes an `Action<IApplicationBuilder>` as an argument, and also return an `Action<IApplicationBuilder>`. Below is our custom implementation of the class:

``` csharp
public class CustomFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            // perform the custom build configuration
            Console.WriteLine($"Configured via {nameof(CustomFilter)}");
            builder.UseMiddleware<HelloMiddleware>();

            // call the next item in the build pipeline
            next(builder);

            // add here to  change the end of the middleware pipeline
        };
    }
}
```

This `build pipeline` operates similarly to how the `middleware pipeline` operates - the custom component performs its build logic, before passing on the _build request_ to the next item in the build pipeline.

In the above, the custom logic is writing a message to the console and then adding `another custom middleware component` to the middleware pipeline:


``` csharp
public class HelloMiddleware
{
    private readonly RequestDelegate _next;

    public HelloMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        Console.WriteLine($"Hello from {nameof(HelloMiddleware)}. " +
            $"The current datetime is '{DateTime.Now}'");

        await _next.Invoke(httpContext);
    }
}
```
The final step to putting it all together is to add the `CustomFilter` implementation to the dependency injection container:

``` csharp
builder.Services.AddTransient<IStartupFilter, CustomFilter>();
```

When the application start, the following is output:

```terminal
Configured via CustomFilter
```

Then when calling the endpoint:

``` terminal
Hello from HelloMiddleware. The current datetime is '2022/08/25 06:48:35'
In the manually added middleware: ManualMiddleware. The current datetime is '2022/08/25 06:48:35'
Weatherforecast endpoint called
```

From the output we can see, that the `middleware added using the IStartupFilter is invoked before the manually added middleware`. We have used the _IStartupFilter_ implementation to add an item to the start of the middleware pipeline , achieved by only adding the _IStartupFilter_ implementation to the dependency injection container.

---

## Flow summary

A summary of what we have covered so far, and how the various pieces fit together

### Application startup

On application startup:
1. The _IStartupFilter_ implementation _CustomFilter_ is `added to the dependency injection` container.
2. When the `WebApplication instance is run`, with the _app.Run()_ command:
    1. All implementations of _IStartupFilter_ are retrieved from the dependency injection container, and the `Configure` method on the implementation invoked
    In this above sample, the `Configure` adds the _HelloMiddleware_ component to the start of the middleware pipeline.
    2. All other middleware components are configured
    In this above sample, the _ManualMiddleware_ component is added to the middleware pipeline

The middleware pipeline is now ready to receive requests.

### Request received

1. When a request is received, it will flow through the middleware pipeline, component by component in the order in which they were added to the pipeline:
    1. The request is passed to the _HelloMiddleware_ component and the "hello" message is output (added via _IStartupFilter_ implementation)
    1. The request is passed to the _ManualMiddleware_ components and the relevent message is output (added explicitly)
    1. The request is then passed to the endpoint handler, to return generate the weather data (defined explicitly)

----

## Notes

Generally most applications would not have a need for this functionality, but there are a couple of cases where it could be especially useful:
- As a library author, and the library needs to inject middleware at the beginning or the end of the middleware pipeline
- Conversely, if a library is injecting middleware at the beginning or end of the pipeline and you as the application author needs to inject a component before/after the library.

---

## References

[Exploring IStartupFilter in ASP.NET Core ](https://andrewlock.net/exploring-istartupfilter-in-asp-net-core/)   

<?# DailyDrop ?>166: 22-09-2022<?#/ DailyDrop ?>
