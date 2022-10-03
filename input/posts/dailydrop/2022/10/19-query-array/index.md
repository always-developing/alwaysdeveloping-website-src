---
title: "Binding query string values to an array"
lead: "Automatically binding a query string value to an array"
Published: "10/19/2022 01:00:00+0200"
slug: "19-query-array"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - querystring
   - array
   - binding
   - .net7

---

## Daily Knowledge Drop

.NET 7 introduces the ability to automatically `bind a query string value to an array` (or _StringValues_ class). 

When the query string value is of variable length and can contain one or more different items - this automatic binding can save time and facilitates cleaner code.

---

## Pre .NET7

Prior to .NET 7, a query string parameter with multiple values would manually need to be converted to an array:

``` csharp
app.MapGet("/getsongs", async ([FromQuery] string artists) =>
{
    async IAsyncEnumerable<string> GetSongs(string[] artists)
    {
        foreach (var name in artists)
        {
            /// simulate looking up all the 
            // songs for the artist
            yield return $"Retrieving songs for '{name}'";
        }
    }

    // the artist parameter value would be
    // comma separated of all the values supplied
    return GetSongs(artists.Split(","));
});

```

Calling the endpoint with `/getsongs?artists=foofighters&artists=johnmayer`, will result in the _artists_ parameter having a value of `foofighters,johnmayer` (all values supplied in a comma separated string). The return value:

``` terminal
["Retrieving songs for 'foofighters'","Retrieving songs for 'johnmayer'"]
```

---

## .NET7
### Array binding

Leveraging this new array binding functionality is very simple:

``` csharp
// Automatically bind the query string value "artists" to the 
// endpoint array parameter "artists"
app.MapGet("/getsongs", async ([FromQuery] string[] artists) =>
{
    // create a local function to be able to yield return
    // from the endpoint
    async IAsyncEnumerable<string> GetSongs(string[] artists)
    {
        foreach (var name in artists)
        {
            /// simulate looking up all the 
            // songs for the artist
            yield return $"Retrieving songs for '{name}'";
        }
    }

    // no conversion required
    return GetSongs(artists);

});
```

Calling the endpoint with a single value, by browsing to the endpoint `/getsongs?artists=foofighters`, gives the following result:

```terminal
["Retrieving songs for 'foofighters'"]
```

While calling the endpoint with multiple values, `/getsongs?artists=foofighters&artists=johnmayer`, results in:

``` terminal
["Retrieving songs for 'foofighters'","Retrieving songs for 'johnmayer'"]
```

---

### StringValues binding

The query string parameter can also be bound to a `StringValues` instance if required. This operates the same as the array:

``` csharp
// Automatically bind the query string value "artists" to the 
// endpoint StringValues parameter "artists"
app.MapGet("/getsongs", async ([FromQuery] StringValues[] artists) =>
{
    // create a local function to be able to yield return
    // from the endpoint
    async IAsyncEnumerable<string> GetSongs(StringValues artists)
    {
        foreach (var name in artists)
        {
            /// simulate looking up all the 
            // songs for the artist
            yield return $"Retrieving songs for '{name}'";
        }
    }

    return GetSongs(artists);

});
```

---

## Notes

This approach is significantly simpler and cleaner than having to split the query string into an array manually. While this enhancement will not drastically change the performance of an application or add significant new functionality, it does facilitate cleaner, more readable code, which day to day, is probably more important for the vast majority of application.

---

## References

[.NET 7: Microsoft Reveals New ASP.NET Core Features](https://dev.to/bytehide/net-7-microsoft-reveals-new-aspnet-core-features-24f3)  

<?# DailyDrop ?>185: 19-10-2022<?#/ DailyDrop ?>
