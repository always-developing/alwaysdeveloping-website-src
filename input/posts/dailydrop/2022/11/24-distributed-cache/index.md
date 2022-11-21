---
title: "Distributed caching int ASP.NET Core"
lead: "ASP.NET Core has build in distributed caching functionality with multiple providers"
Published: "11/24/2022 01:00:00+0200"
slug: "24-distributed-cache"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - cache
   - distributed
   - distributedcache

---

## Daily Knowledge Drop

ASP.NET Core comes with the `IDistributedCache` interface, and a number of implementations to support `distributed caching`. The out of the box implementaions include _In Memory_, _SQL Server_, _Redis_ and _NCache_ - however if another implementation is required, a custom provider can also be fairly easily be written.

---

## The need for distributed cache 

Before getting to _distributed cache_ we'll have a quick look at the _non-distributed in memory cache_ implementation which also comes out of the box with ASP.NET Core. Configuring and leveraging this functionality is incredibly easy:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// configure dependency injection with the in-memory cache
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/cache", (IMemoryCache cache) =>
{
    // try get a value with the key "hello" out the cache
    if(cache.TryGetValue("hello", out string result))
    {
        Console.WriteLine("Retrieved from cache");

        return result;
    }

    // if the value was not found in the cache
    // add it to the cache
    cache.Set("hello", "world");

    return "world";
});

app.Run();
```

Here, the _IMemoryCache TryGetValue_ method is called to get get a value out of the cache by key. If no value is found, then the item is put into the cache and returned. This is a very simple example, with no cache expiry specified.

The in-memory cache will be entirely suitable if `only one instance of an application is running` - the cache is stored in the memory of that one instance of the application. However, if multiple instance of the application are running (in the cloud, or in containers) the _in-memory cache is not shared across instances_ - each instance will have its own cached. This is where a `distributed cache` can be leveraged, with each application instance sharing the cache, and benefiting from the caching done by other instances.

---

## Distributed cache 

Configuring and using a `distributed cache` in ASP.NET Core is _almost_ as easy as configuring a normal in-memory cache.

``` csharp
var builder = WebApplication.CreateBuilder(args);

// configure in-memory distributed cache
// which is good for testing, but is not truly 
// distributed
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// inject IDistributedCache and not IMemoryCache
app.MapGet("/distributedcache", async (IDistributedCache cache) =>
{
    string result;

    // (Try) get the byte array value from the cache
    byte[] encodedResult = await cache.GetAsync("hello");
    // if a value was returned
    if(encodedResult != null)
    {
        Console.WriteLine("Retrieved from cache");

        // convert byte array to string and return
        result = Encoding.UTF8.GetString(encodedResult);
        return result;
    }

    // convert string to byte array 
    encodedResult = Encoding.UTF8.GetBytes("world");
    // and configure the cache options
    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(10));

    // add the value to the cache
    await cache.SetAsync("hello", encodedResult, options);

    return "world";

});

app.Run();
```

---

### Usage

While `IMemoryCache` can store a _string_ or _object_, `IDistributedCache` will only store a _byte array_. Additional processing has to be done to convert to and from a byte array when inserting and retrieving values from the cache.
The `IDistributedCache` also doesn't have a _TryGet_ method, only a _Get_ method, which will return `null` if no value for the supplied key is found.

---

### Configuration

The configuration of the `in-memory distributed cache` cache was simple (as seen above):

``` csharp
builder.Services.AddDistributedMemoryCache();
```

However, the configuration of the other implementations is not much more complicated. The SQL Server provider for example:

``` csharp
builder.Services.AddDistributedSqlServerCache(act =>
{
   act.SchemaName = "dbo";
   act.TableName = "AppCache";
   act.ConnectionString = builder.Configuration.GetConnectionString("DefaultDatabase");
});
```
The actual logic (the endpoint delegate method in the above example) can remain exactly as is.

---

## Notes

Having a distributed cache is a key feature in many scalable applications, and the easy to configure and use, out of the box functionality provided by ASP.NET Core will be suitable for most application's needs.

---


## References

[Understanding & Implementing Caching in ASP.NET Core](https://www.mitchelsellers.com/blog/article/understanding-implementing-caching-in-asp-net-core)  

<?# DailyDrop ?>209: 24-11-2022<?#/ DailyDrop ?>
