---
title: "Naming minimal endpoints"
lead: "Naming minimal endpoints for easier link generation"
Published: "01/13/2023 01:00:00+0200"
slug: "13-endpoint-withname"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - endpoint
   - name
   - linkgenerator

---

## Daily Knowledge Drop

A minimal endpoint can be `given a name`, which can then be leveraged to `automatically generate a link to the endpoint` making it easier to be invoked.

The endpoint name metadata is also treated as the _operation Id_ in the OpenAPI specification, which is used by tools which use the swagger file to generate a client.

---

## Named endpoint

The first step is to give the endpoint a name - this is very simply and involves using the _WithName_ method:

``` csharp
// the default sample endpoint
app.MapGet("/weatherforecast", () =>
{
    WeatherForecast[]? forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
// give it a name
.WithName("GetWeatherForecast");
```

After the body of the minimal API is defined, the _WithName_ method is called, with the endpoint name supplied. In this example the sample _/weatherforecast_ endpoint is given the name `GetWeatherForecast`.

---

## Link generation

This _endpoint name_ can now be used to generate a URL to the endpoint - this is done using the `LinkGenerator` class:

``` csharp
// inject LinkGenerator from DI
app.MapGet("/generateUrl", (HttpContext context, LinkGenerator generator) =>
{
    // use the name to get a link to the GetWeatherForecast endpoint
    return generator.GetUriByName(context, "GetWeatherForecast", null);
});
```

The above code defines an endpoint, which when invoked will return the URL of the `GetWeatherForecast` endpoint. Browsing to the `/generateUrl` endpoint returns the following:

``` terminal
http://localhost:5276/weatherforecast
```

The full URL (including port etc) is automatically calculated by the `LinkGenerator.GetUriByName` method, based on the _HttpContext_ and the _endpoint name_. _LinkGenerator_ is available through the dependency injection container and can just be injected into the relevent constructor/delegate.

---

## Named endpoint invocation

A practical use of this functionality is _when an endpoint is required to call another endpoint in the same application_:

``` csharp
app.MapGet("/weatherforecastproxy", async (HttpContext context, LinkGenerator generator) =>
{
    HttpClient? client = new HttpClient();

    // get the URL for the "GetWeatherForecast" endpoint
    // and create an HttpRequestMessage for it
    HttpRequestMessage? request = new HttpRequestMessage(
        new HttpMethod("GET"),
        generator.GetUriByName(context, "GetWeatherForecast", null)
    );

    // invoke it
    HttpResponseMessage? response = client.Send(request);
    return await response.Content.ReadAsStringAsync();
});
```

In this example, calling `/weatherforecastproxy` will proxy the call to the `GetWeatherForecast` endpoint and returns the result - this proxy endpoint is not adding much value in this sample, but it could do more complicated logic, such as calling multiple endpoints and reconciling the results, for example.

Using `named endpoints` and `LinkGenerator.GetUriByName` is a safer approach to generating the URL, instead of manually trying to build up the URL based on the information extracted from the _HttpContext_.

---

## Notes

A simple and easy way to name, and then generate a link using the name. If the code is required to call another endpoint in the same application, or will be used with a client generation tool - then all endpoints should be named, and _LinkGenerator_ used when generating the links.

---

## References

[Adding Experimental HTTP Methods To ASP.NET Core](https://khalidabuhakmeh.com/adding-experimental-http-methods-to-aspnet-core)  

<?# DailyDrop ?>234: 12-01-2023<?#/ DailyDrop ?>
