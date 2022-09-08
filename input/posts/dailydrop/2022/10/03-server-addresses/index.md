---
title: "Obtaining a list of app URLs"
lead: "Using IServerAddressesFeature to get a list of URLs your app is responding to"
Published: "10/03/2022 01:00:00+0200"
slug: "03-server-addresses"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - server
   - addresses
   - features

---

## Daily Knowledge Drop

The `IServerAddressesFeature` or `IServer` implementations can be used to get a list of URLs your application is responding to. In a landscape with a large number of applications which serve on multiple URLs, this could be leveraged to easily self-document all URLs currently in use.

---

## Multiple urls

For the samples below, the application has been configured to use multiple urls using the `UseUrls` method:

``` csharp
builder.WebHost.UseUrls("http://*:5096;http://*:5097;http://localhost:5098");
```

If we had a _"hello"_ endpoint:

``` csharp
app.MapGet("/hello", () =>
{
    return "world";
});
```

We would get a _"world"_ response by browsing to any of the below:
- http://localhost:5096/hello
- http://localhost:5097/hello
- http://localhost:5098/hello

---

## Address list

### IServerAddressesFeature

The first method to get a list of addresses is by casting the `WebApplication` instance to `IApplicationBuilder` and leveraging the _ServerFeatures_ method available:

``` csharp
// app is obtained from var app = builder.Build();
// in this example, serverAddress is a free variable
var serverAddress = (app as IApplicationBuilder)
    .ServerFeatures.Get<IServerAddressesFeature>();

app.MapGet("/addresses", (context) =>
{
    // get all addresses and output
    foreach(var address in serverAddress.Addresses)
    {
        context.Response.WriteAsync($"- {address}{Environment.NewLine}");
    }

    return Task.CompletedTask;
});
```

The output from browsing to the above endpoint:

``` terminal
- http://[::]:5096
- http://[::]:5097
- http://localhost:5098
```

---

### IServer

When `IApplicationBuilder` is not available (outside of application startup), the information is also available by injecting `IServer` into the required constructor or delegate:

``` csharp
app.MapGet("/iserver", (HttpContext context, IServer server) =>
{
    // get the address from IServer instead of IApplicationBuilder
    // otherwise everything else is the same
    var addressFeature = server.Features.Get<IServerAddressesFeature>();

    foreach (var address in addressFeature.Addresses)
    {
        context.Response.WriteAsync($"- {address}{Environment.NewLine}");
    }

    return Task.CompletedTask;
});
```

The output from browsing to the above endpoint:

``` terminal
- http://[::]:5096
- http://[::]:5097
- http://localhost:5098
```

---

## Notes

We've looked at two different ways to get a list of addresses the application is serving on. For most applications, this might never be useful as most applications serve on a single address. However in the case where there _are_ multiple addresses, this could be used to self document and easily keep track of application's addresses.

---

## References

[Server Addresses Feature](https://github.com/dodyg/practical-aspnetcore/blob/net6.0/projects/features/features-server-addresses)   
[Server Addresses Feature - 2](https://github.com/dodyg/practical-aspnetcore/tree/net6.0/projects/features/features-server-addresses-2)   

<?# DailyDrop ?>173: 03-10-2022<?#/ DailyDrop ?>
