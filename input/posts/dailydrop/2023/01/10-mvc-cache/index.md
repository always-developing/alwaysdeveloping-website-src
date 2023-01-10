---
title: "Caching MVC responses"
lead: "How to cache the response from an MVC controller to increase service performance"
Published: "01/10/2023 01:00:00+0200"
slug: "10-mvc-cache"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - mvc
   - controller
   - cache

---

## Daily Knowledge Drop

ASP.NET Core has out of the box `configurable response caching` functionality which can be leveraged to improve the performance of a service using `controllers`.

The functionality described in this post is only for caching responses from _an MVC controller_ - to cache responses from a _minimal API_, have a look at [this post which details output caching on minimal APIs](../../../2022/09/07-output-cache/).

---

## Code

### Setup

The default template _Weather API_ project is used for the below example, with the `Use Controllers` options checked.

---

### Startup

First step, `enabling the caching functionality` in the service - this is done in the _program.cs_, and involves adding two lines of code:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// setup caching with the DI container
builder.Services.AddResponseCaching();

var app = builder.Build();

// setup caching in the pipeline
app.UseResponseCaching();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

This follows a fairly standard pattern when it comes to adding functionality to an ASP.NET Core application:
- The `AddResponseCaching` method is called to register the required services with the dependency injection container
- The `UseResponseCaching` method is called to insert the caching logic into the middleware pipeline


Next step, enabling caching on a controller.

---

### Controller

Now that we have the base caching functionality configured in the service, the next step is to actual enable caching for a specific endpoint/controller. This is done by `adding an attribute to the relevent controller method`. In the _WeatherForecastController.cs_ class:

``` csharp
[HttpGet]
[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
public IEnumerable<WeatherForecast> Get()
{
    Console.WriteLine($"{nameof(Get)} method in {nameof(WeatherForecastController)} called");

    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    })
    .ToArray();
}
```

The `ResponseCache` attribute is added to the _Get_ method (the method called when the endpoint is called). In this example, the cache is set to expire every 30 seconds.

And thats it! We now have basic caching functionality working on the service.

---

### Output

If we run the service and browse to the `/weatherforecast` endpoint, the weather payload will be returned, and logging at the console, the following will be output:

``` terminal
Get method in WeatherForecastController called
```

Refreshing the endpoint within 30 seconds (the duration of the cache), will yield the same payload, and cause no output to the console - the `results are returned from the cache, and the controller is never called.`

---

### Vary

Not explicitly show in the above example, but it is also possible to `vary the cache response` by a specific key.

If the attribute parameters were changed to include the `VaryByHeader` parameter:

``` csharp
[HttpGet]
[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
public IEnumerable<WeatherForecast> Get()
{
    Console.WriteLine($"{nameof(Get)} method in {nameof(WeatherForecastController)} called");

    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    })
    .ToArray();
}
```

Then a different cache is created for each distinct `User-Agent` header value sent on a request, each cache independent of one another. This way, the cache of one called will operate independently from the cache of a different caller. It is also possible to vary the cache by a specific `query string key`.

---

## Notes

When simple caching is required, the built-in functionality is an easy to implement, low effort option to enhance the performance of a service.

---


## References

[ASP.NET Core Response Caching](https://ryansouthgate.com/asp.net-core-response-caching/)  

<?# DailyDrop ?>231: 10-01-2023<?#/ DailyDrop ?>
