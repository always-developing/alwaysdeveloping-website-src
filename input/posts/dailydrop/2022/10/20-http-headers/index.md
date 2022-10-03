---
title: "Built in HTTP header properties"
lead: "ASP.NET Core has built-in properties for commonly used HTTP headers"
Published: "10/20/2022 01:00:00+0200"
slug: "20-http-headers"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - http
   - headers

---

## Daily Knowledge Drop

ASP.NET Core has `built in properties for common HTTP headers`, which are _faster and better_ than using an indexer manually.

---

## Manual indexers

In this example, the _ContextType_, _Date_ and _Link_ headers are manually set using indexers on the endpoint response:

``` csharp
app.MapGet("/getheadersmanual", async context =>
{
    context.Response.Headers.ContentType = MediaTypeNames.Application.Json;
    context.Response.Headers.Date = DateTime.UtcNow.ToString();
    context.Response.Headers.Link = "/getheadersmanual";

    await context.Response.WriteAsync(@"{""Message"" : ""Look at the headers""}");
});
```

This works without issue, and calling the endpoint will result in the following response and headers:

```terminal
{"Message" : "Look at the headers"}
```

![Response Headers](manualheaders.png)

---

## Built-in properties

The other option is to use the built-in header properties:

``` csharp
app.MapGet("/getheaders", async context =>
{
    context.Response.Headers.ContentType = MediaTypeNames.Application.Json;
    context.Response.Headers.Date = DateTime.UtcNow.ToString();
    context.Response.Headers.Link = "/getheaders";

    await context.Response.WriteAsync(@"{""Message"" : ""Look at the headers""}");
});
```

This version will same response and headers as the above technique:

```terminal
{"Message" : "Look at the headers"}
```

![Response Headers](autoheaders.png)

---

## Notes

While the output from both techniques are effectively the same - in addition to the property method being faster than using indexers (although probably not effectively noticeable), using the property method will results in less errors and safer code, as the HTTP header name cannot be misspelt.

---

## References

[James Newton-King Tweet](https://twitter.com/JamesNK/status/1572876608553517056)  

<?# DailyDrop ?>186: 20-10-2022<?#/ DailyDrop ?>
