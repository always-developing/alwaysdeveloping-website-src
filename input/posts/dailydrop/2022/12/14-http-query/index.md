---
title: "New HTTP methods"
lead: "How to handle new or custom HTTP methods"
Published: "12/14/2022 01:00:00+0200"
slug: "14-http-query"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - http
   - query
   - httpmethod


---

## Daily Knowledge Drop

Standards have been published for a new HTTP method, `QUERY`, which aims to effectively be a merge between the GET and POST methods.

While this post will focus on handling the `QUERY` HTTP method, the same configuration and logic could be applied to any custom HTTP method (although not advised)

---

## The need for QUERY

First, a quick word on the need for the QUERY method. When using the GET method, all information to be passed to the server is contained in the URL as a _GET method does not have a body_. The problem is that URL's have a finite length and when this limit is reached, often developers will switch to using a POST method which allows for more complex and lengthy information to be _passed in the body_. 
While this technique will work, the two different methods will generally have different caching mechanisms and a POST method may not offer the same performance as a GET method.

The QUERY method aims to address this, by effectively providing a method which operates like the "GET" method, but which allows for a body to be posted to the server.

---

## Defining the endpoint

Defining an endpoint which uses a custom or new HTTP method is surprisingly easy - the `MapMethods` method can be used, with a list of _httpMethods_ the endpoint supports:

``` csharp
// define an endpoint with QUERY http method
app.MapMethods("/query", new[] { "QUERY" }, async (HttpContext context) =>
{
    string? text = null;
    var request = context.Request;

    // the body has to be read manually
    if (!request.Body.CanSeek)
    {
        request.EnableBuffering();
    }

    if (request.Body.CanRead)
    {
        // read the body as a stream, and then set the
        // reader position back to the start
        request.Body.Position = 0;
        var reader = new StreamReader(request.Body, Encoding.UTF8);
        text = await reader.ReadToEndAsync();
        request.Body.Position = 0;
    }

    // return the body
    return Results.Ok($"Querying data using body: {text}");

})
// give the endpoint a name, just making it easier to 
// find later. Not required
.WithName("query"); 
```

Here an endpoint `/query` is defined, which uses the HTTP method `QUERY`. The endpoint will read the contents of the body supplied with the HTTP request, and return it back to the caller.

The _WithName_ method is used to name the endpoint, which will be used in the next step to easily build up the URL to be invoked - this is not strictly required.

---

## Calling the endpoint

Calling the endpoint is a _little_ tricker, as the typical tools one might use (e.g. Postman) will not support the new or custom HTTP method. In this example, the C# HttpClient will be used to call the `QUERY` endpoint - we are going to _create a GET endpoint, which will call the QUERY endpoint_:

``` csharp
app.MapGet("/callquery", async (HttpContext context, 
    LinkGenerator generator) =>
{
    var client = new HttpClient();

    // set the http method as QUERY
    var request = new HttpRequestMessage(
        new HttpMethod("QUERY"),
        // get the URL by the name specified
        generator.GetUriByName(context, "query", null)
    );

    // set the body of the QUERY call
    request.Content = new StringContent("This is a body of a QUERY call", 
        Encoding.UTF8, "text/plain");

    // call the QUERY endpoint
    var response = client.Send(request);
    var result = await response.Content.ReadAsStringAsync();

    // the expected result should be the same 
    // as the body specified
    return result;
});
```

Invoking the `/callquery` endpoint yields the following result:

``` terminal
"Querying data using body: This is a body of a QUERY call"
```

Success! We have successfully defined an endpoint which uses the `QUERY` HTTP method, which was successfully invoked with a body supplied.

---

## Simplifying the code

The above is a very rough implementation of the `QUERY` (or any custom HTTP method) implementation - in [Khalid Abuhakmeh's post](https://khalidabuhakmeh.com/adding-experimental-http-methods-to-aspnet-core), he has some good ways to simplify the code, and make it more generic. I suggest having a look at the techniques mentioned in his post on how to streamline the code for reuse.

---

## Notes

A useful _experiment_ to see how support for any HTTP method support can be added relatively easily. There are limitation of custom HTTP methods - including the above mention support from 3rd party applications (Postman) as well as libraries (Swashbuckle), as well as limited or no support from caching or load balancing mechanisms.

With reference to the `QUERY` HTTP method - personally I like this, and will make sue of this, so I hope it does get adopted in the mainstream in future.

---


## References

[Adding Experimental HTTP Methods To ASP.NET Core](https://khalidabuhakmeh.com/adding-experimental-http-methods-to-aspnet-core)  

<?# DailyDrop ?>222: 14-12-2022<?#/ DailyDrop ?>
