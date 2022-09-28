---
title: "Access cache stats with MemoryCacheStatistics"
lead: "Using MemoryCacheStatistics to better understand MemoryCache usage in an application"
Published: "10/17/2022 01:00:00+0200"
slug: "17-cache-stats"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - cache
   - memorycache
   - statistics

---

## Daily Knowledge Drop

Coming with .NET 7, the `MemoryCacheStatistics` class, accessed through `IMemoryCache.GetCurrentStatistics` can provider useful statistics regarding the state of the memory cache. These include:
- how many items are currently in the cache
- the current estimate size of the cache
- the number of times the cache has been queried for a value, and the value was not present (misses)
- the number of times the cache has been queried for a value, and the value was present (hits)

These stats can be leveraged to better understand how/if the cache mechanism is correctly configured and being used optimally.

---

## Cache configuration

The first step is to register the require cache implementation instances with the dependency injection container. This is done using the _AddMemoryCache_ extension method:

``` csharp
builder.Services
    .AddMemoryCache(memCacheOptions => 
        { 
            memCacheOptions.TrackStatistics = true; 
            memCacheOptions.SizeLimit = 1024; 
        });
```

- For stats to be tracked, the `MemoryCacheOptions.TrackStatistics` value (_memCacheOptions.TrackStatistics_ in the above example) needs to be set to `true`
- For the `EstimateSize` property of the statistics to be tracked, the cache needs to have a _SizeLimit_ specified. The _SizeLimit_ does not represent memory size, but _number of units_ - in the above example, the limit has been set to _1024 units_, and when items are added to the cache _one byte will be treated as a unit_ (this is not a required, its up to the developer to decide how many "units" each item added to the cache is worth)

---

## Cache usage

### Set values

Now that the cache is configured, the next step is to start using it to store values. A simple `getnumber` endpoint is defined, which will _return the number passed in_:  
- if the number does not exist in the cache, it will be added to the cache and returned
- if the number already exists in the cache, it will be returned from the cache

``` csharp
// get the number from the route
// get the IMemoryCache from the DI container
app.MapGet("/getnumber/{number}", ([FromRoute]int number, 
    [FromServices] IMemoryCache cache) =>
{
    // first check the cache to see if the number
    // exists there
    if(cache.TryGetValue(number, out var value))
    {
        return $"'{value}' retrieved from cache";
    }

    // if it didn't exist, add it to the cache
    cache.Set(number, number, new MemoryCacheEntryOptions { Size = sizeof(int)});

    // return
    return $"'{number}' added to cache";
});
```

Invoking the endpoint with a number for the first time `/getnumber/1`, will result in:

``` terminal
'1' added to cache
```

While invoking subsequent times with the same number will result in:

``` terminal
'1' retrieved from cache
```

The cache has been configured and the _endpoint called a number of times, with unique and duplicate numbers_, so next let's have a look at the stats.

---

### Get statistics

To get a _snapshot of the IMemoryCache statistics_, the `GetCurrentStatistics` method is used:

``` csharp
app.MapGet("/getstats", ([FromServices] IMemoryCache cache) =>
{
    return cache.GetCurrentStatistics();
});
```

A sample return snapshot:

``` json
{
    "currentEntryCount":6,
    "currentEstimatedSize":24,
    "totalMisses":6,
    "totalHits":2
}
```

In this example:
- `6 items have been added` to the cache
- the 6 items take up `24 of the available 1024 units` available (6 x 4 bytes)
- the cache has been checked and did `not contain the queried value 6 times`
- the cache has been checked and `did contain the queried value twice`

---

## Notes

This information is very valuable and can be leveraged to confirm if the cache configured for an application is successfully being used - over time, one would want to see a high number of hits and relatively low number of misses. This information can be used to determine if the cache strategy needs to be modified (cache based on a different key) or maybe increase or decrease the cache size.

---

<?# DailyDrop ?>183: 17-10-2022<?#/ DailyDrop ?>
