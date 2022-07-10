---
title: "Dynamically changing minimal api return type"
lead: "Changing minimal api return content types dynamically (including to xml)"
Published: "08/04/2022 01:00:00+0200"
slug: "04-minimal-api-return-type"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - api
    - minimalapi
    - contenttype

---

## Daily Knowledge Drop

Minimal apis, by default, have a JSON return type - however a custom `IResult` implementation can be used to change this return type. This same interface can be used to dynamically decide at runtime the return type, but requires some additional effort.

---

## Default return type

With minimal api's, the default return type is JSON. Executing the following command, the _Song_ instance is automatically serialized to JSON and the response _content-type_ header automatically set to **application/json**:


``` csharp
app.MapGet("/json", () =>
{
    return new Song
    {
        ArtistName = "John Mayer",
        SongName = "Bigger than my body",
        LengthInSeconds = 245
    };
});
```

Returned JSON:

``` json
{"songName":"Bigger than my body","artistName":"John Mayer","lengthInSeconds":245}
```

WHat if a `different return type` is required?

---

## XML return type

As mentioned, to change the response type, instead of just returning the _Song_ instance (which will then serialize to JSON), an implementation of `IResult` needs to be returned.

In this endpoint an instance of the custom class `XmlResult` (full code below) is being returned:

``` csharp
app.MapGet("/xml", () =>
{
    // XML implementation of IResult 
    return new XmlResult<Song>(
        new Song
        {
            ArtistName = "John Mayer",
            SongName = "Bigger than my body",
            LengthInSeconds = 245
        });
});
```

`XmlResult` is an implementation of the `IResult` interface, which is very simple, containing only one method to implement:

``` csharp
    Task ExecuteAsync(HttpContext httpContext);
```

This method will accept the _HttpContext_ of the request as a parameter, and modify the response body and headers as required before being returned to the called.

The `XmlResult` implementation (this is by no means the most efficient method for doing XML serialization, but it's a simple demonstration for this post):

``` csharp
// implement IResult
public class XmlResult<T> : IResult
{
    // store the entity to be serialized
    private readonly T _entity;

    public XmlResult(T entity)
    {
        _entity = entity;
    }

    // method which needs implementing
    public Task ExecuteAsync(HttpContext httpContext)
    {
        // prepare for XML serialization
        XmlSerializer xmlSerializer = new(typeof(T));
        using StringWriter textWriter = new();

        // perform the serialization
        xmlSerializer.Serialize(textWriter, _entity);

        // modify the response content type, content length and 
        // write the XML to the body of the response
        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        httpContext.Response.ContentLength = 
            Encoding.UTF8.GetByteCount(textWriter.ToString());
        return httpContext.Response.WriteAsync(textWriter.ToString());
    }
}
```

Browsing to the `/xml` endpoint show above, returns the following response, with the response _content-type_ set to **application/xml**:

``` xml
<Song>
    <SongName>Bigger than my body</SongName>
    <ArtistName>John Mayer</ArtistName>
    <LengthInSeconds>245</LengthInSeconds>
</Song>
```

So far so good, however, the standard recommended way of returning an `IResult` implementation is via an extension method on `IResultExtensions`, and not a manual instantiation:

``` csharp
public static class XmlResultsExtensions
{
    public static IResult Xml<T>(this IResultExtensions resultExtensions, T entity)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new XmlResult<T>(entity);
    }
}
```

Using this extension method, the **final XML endpoint** now looks as follows:

``` csharp
app.MapGet("/xml", () =>
{
    // Use the extension method instead of 
    // explicitly using XmlResult 
    return Results.Extensions.Xml(
        new Song
        {
            ArtistName = "John Mayer",
            SongName = "Bigger than my body",
            LengthInSeconds = 245
        });
});
```


Next up we explore how to allow the `caller to decide how they would like the data returned, using a single endpoint`.

---

## Dynamic return type

Let's explore how the return type can be changed dynamically based on an indicator from the caller. For this example we'll use the `content-type` of the _request_ as the indicator (not necessarily the good option for all cases - but good enough for this demo).

We'll add a parameter to the endpoint, and instruct ASPNET Core to get the value from the `content-type` header, using the _FromHeader_ attribute:

``` csharp
app.MapGet("/dynamic", ([FromHeader(Name = "Content-Type")] string? contentType) =>
{
    var song = new Song
    {
        ArtistName = "John Mayer",
        SongName = "Bigger than my body",
        LengthInSeconds = 245
    };

    return song;
});
```

Progress! However this endpoint still only returns JSON. Next let's add the check on the request content type....

``` csharp
app.MapGet("/dynamic", ([FromHeader(Name = "Content-Type")] string? contentType) =>
{
    var song = new Song
    {
        ArtistName = "John Mayer",
        SongName = "Bigger than my body",
        LengthInSeconds = 245
    };

    // if XML return XML implementation of Song
    if (contentType == MediaTypeNames.Application.Xml)
    {
        return Results.Extensions.Xml(song);
    }

    return song;
});
```

...which `results in an error!`

``` terminal
Cannot convert lambda expression to type 'RequestDelegate' because 
    the parameter types do not match the delegate parameter types
```

Our endpoint is trying trying to return two different types - an `IResult` implementation when XML, and a `Song` instance when not. 

This is easy enough to resolve - change the endpoint to `always returns an IResult` implementation. To do this however, we now need a `custom IResult implementation for JSON`. It operates exactly the same as the XML implementation, but serializes to JSON instead of XML:

``` csharp
public class JsonResult<T> : IResult
{
    private readonly T _entity;

    public JsonResult(T entity)
    {
        _entity = entity;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        using StringWriter textWriter = new StringWriter();
        var jsonResult = System.Text.Json.JsonSerializer.Serialize<T>(_entity);

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonResult);
        return httpContext.Response.WriteAsync(jsonResult);
    }
}
```

And the corresponding extension method:

``` csharp
public static class JsonResultsExtensions
{
    public static IResult Json<T>(this IResultExtensions resultExtensions, T entity)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new JsonResult<T>(entity);
    }
}
```

Lastly, we update the _dynamic_ endpoint so the **final endpoint** now looks as follows:

``` csharp
app.MapGet("/dynamic", ([FromHeader(Name = "Content-Type")] string? contentType) =>
{
    var song = new Song
    {
        ArtistName = "John Mayer",
        SongName = "Bigger than my body",
        LengthInSeconds = 245
    };

    // if XML return XML implementation of Song
    if (contentType == MediaTypeNames.Application.Xml)
    {
        return Results.Extensions.Xml(song);
    }

    // In all other cases return JSON
    return Results.Extensions.Json(song);
});
```

The endpoint can now be invoked with a `content-type` header value `application/xml` to get the results in XML and in all other cases, get the result as JSON.

---

## Notes

This is a fair amount of code just to change the response type - but it is a piece of code which only needs to be written once and can then be reused across all endpoints. All endpoints can then benefit from bug fixes or performance improvements in the `IResult` implementation.

---

## References

[Minimal APIs overview - responses](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0#responses)   

---

<?# DailyDrop ?>131: 04-08-2022<?#/ DailyDrop ?>
