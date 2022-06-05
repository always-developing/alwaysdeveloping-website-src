---
title: "Binding a query parameter to an object"
lead: "Exploring binding a query parameter to an endpoint object parameter"
Published: "06/09/2022 01:00:00+0200"
slug: "09-parameter-binding"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - .net7
    - endpoint

---

## Daily Knowledge Drop

Coming in .NET 7, endpoint query parameter can automatically me mapped to an object by adding a `TryParse` method to the object. In this post we'll compare the traditional query parameter functionality, and compare it with the newly added functionality.

This functionality is available now in the the .NET 7 preview release, and subject to change by the final .NET 7 release.

---

## Use case

In our use case, we have a search page on an e-commerce website. Query string parameters can be send as part of the URL to determine which _category_ to display, which _page number_ to display and _how many items per page_ to display.

---

### Separate parameters

The above can be achieved using three separate query string parameters. A minimal endpoint can be defined with 3 parameters, matching the query string parameter names - the query string parameters will automatically be mapped to the endpoint parameters by ASPNET Core.

``` csharp
app.MapGet("/api/searchpage", (string category, int pageIndex, int pageSize) =>
{
    return $"Returning page '{pageIndex}' of size " +
        $"'{pageSize}' for category '{category}'";

});
```

This endpoint can be access on the URL `/api/searchpage?category=pants&pageIndex=3&pageSize=50`. 

This solution will work, but there is also another cleaner (although, that is subjective) way to define the endpoint and handle the query string.

---

### Binding to an object

Let's update the endpoint to take an object instead of the three separate parameters:

``` csharp
app.MapGet("/api/searchpageoptions", (SearchPageOptions searchOptions) =>
{
    return $"Returning page '{searchOptions.PageIndex}' of size " +
        $"'{searchOptions.PageSize}' for category '{searchOptions.Category}'";
    
});
```

With `SearchPageOptions` defined as follows:

``` csharp
public class SearchPageOptions
{
    public string Category { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }
}
```

If we try run this as it, an exception will occur - this is because by `default ASPNET Core will infer and try map the body of the request to an object parameter to the endpoint`, and GET requests do not allow body parameters.  

We need to instruct ASPNET Core how to map from the query string to the object - and thankfully this is very simple.  

We are however required to make a change to the format of the query string - only a single query string parameter can be mapped to an object, so the three query string parameters need to be converted to one parameter in `json format`.

We can add a `TryParse` method to the _SearchPageOptions_ class:

``` csharp
public class SearchPageOptions
{
    public string Category { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public static bool TryParse(string value, out SearchPageOptions result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        result = JsonSerializer.Deserialize<SearchPageOptions>(value, options);

        return true;
    }
}
```

ASPNET Core will look for, and call this method with the relevent query string parameter, allowing the method to try convert the parameter to the object. With the above changes to the endpoint and _SearchPageOptions_ class, the endpoint can now be called using:  
`/api/searchpageoptions?searchOptions={"category":"pants","pageIndex":3,"pageSize":50}`

Notice the information conveyed is the same, just in a simple json parameter now, instead of three separate parameters - allowing for the conversion of the query string parameter to a _SearchPageOptions_ instance.

---

## Notes

Both methods above result in the same outcome, just using slightly different techniques - but in the end it comes down to personal preference. Personally, I prefer the binding method, with the cleaner and concise method arguments. This approach however does result in a slightly more complicated query string format - the tradeoffs of each approach should be considered, and the best one for your application chosen.

---

## References

[ASP.NET Core: Custom Controller Action Parameter Binding using TryParse in Minimal APIs ](https://jaliyaudagedara.blogspot.com/2022/04/aspnet-core-custom-controller-action.html)  

<?# DailyDrop ?>92: 09-06-2022<?#/ DailyDrop ?>
