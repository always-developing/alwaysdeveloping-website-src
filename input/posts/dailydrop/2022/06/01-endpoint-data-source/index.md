---
title: "Listing all ASP.NET Core routes"
lead: "How to display access and display all routes in an ASP.NET application"
Published: "06/01/2022 01:00:00+0200"
slug: "01-endpoint-data-source"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - aspnetcore
    - route

---

## Daily Knowledge Drop

ASP.NET Core has a build in class, `EndpointDataSource`, which contains information about a route, while the `IEnumerable<EndpointDataSource>` collection, available through dependency injection contains information about all endpoints of an application.

Information about a specific endpoint can also be retrieved from the `HttpContext` for a specific request.

---

## Listing routes

First, lets look at getting all the routes (endpoints) for an application - this can be done by injecting `IEnumerable<EndpointDataSource>` into the relevant constructor (or endpoint delegate in the below example):

``` csharp
app.MapGet("/routes", (IEnumerable<EndpointDataSource> routes) =>
        string.Join(Environment.NewLine, routes.SelectMany(es => es.Endpoints)));
```

Browsing to the `/routes` endpoint, the following is returned (the 4 endpoints which make up the sample api):

``` powershell
    HTTP: GET /routes
    HTTP: GET /user/{userId}
    HTTP: GET /routewithid/{id}
    HTTP: GET /routes/info
```

---

## Additional route data

Additional route metadata can also be retrieved from `EndpointDataSource`. The below `/routes/info` endpoint extends on the above basic endpoint adding additional data for each endpoint:

``` csharp
app.MapGet("/routes/info", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var sb = new StringBuilder();
    var endpoints = endpointSources.SelectMany(es => es.Endpoints);

    // iterate through each endpoint
    foreach (var endpoint in endpoints)
    {
        // get the display name
        sb.AppendLine($"Endpoint: {endpoint.DisplayName}");

        // check if the endpoint is a RouteEndpoint
        if (endpoint is RouteEndpoint routeEndpoint)
        {
            // output route pattern information
            sb.AppendLine($"        - Segment Count: " +
                $"{routeEndpoint.RoutePattern.PathSegments.Count}");
            
            sb.AppendLine($"        - Parameters: ");
            foreach(var param in routeEndpoint.RoutePattern.Parameters)
            {
                sb.AppendLine($"            - {param.Name}");
            }
            
            sb.AppendLine($"        - Inbound Precedence: " +
                $"{routeEndpoint.RoutePattern.InboundPrecedence}");
            sb.AppendLine($"        - Outbound Precedence: " +
                $"{routeEndpoint.RoutePattern.OutboundPrecedence}");
        }

        // output meta data
        sb.AppendLine($"        - Meta Count: {endpoint.Metadata.Count()}");
        foreach (var meta in endpoint.Metadata)
        {
            sb.AppendLine($"            - Meta Type: {meta}");
        }
    }

    return sb.ToString();
});
```

Browsing to the `/routes/info` endpoint, one can see the additional information:

``` powershell
    Endpoint: HTTP: GET /routes
            - Segment Count: 1
            - Parameters: 
            - Inbound Precedence: 1
            - Outbound Precedence: 5
            - Meta Count: 2
                - Meta Type: System.String <<Main>$>b__0_0(System.Collections.Generic.IEnumerable`1[Microsoft.AspNetCore.Routing.EndpointDataSource])
                - Meta Type: Microsoft.AspNetCore.Routing.HttpMethodMetadata
    Endpoint: HTTP: GET /user/{userId}
            - Segment Count: 2
            - Parameters: 
                - userId
            - Inbound Precedence: 1,3
            - Outbound Precedence: 5,3
            - Meta Count: 2
                - Meta Type: System.String <<Main>$>b__0_1(System.String)
                - Meta Type: Microsoft.AspNetCore.Routing.HttpMethodMetadata
    Endpoint: HTTP: GET /routewithid/{id}
            - Segment Count: 2
            - Parameters: 
                - id
            - Inbound Precedence: 1,3
            - Outbound Precedence: 5,3
            - Meta Count: 3
                - Meta Type: System.String <<Main>$>b__0_2(Microsoft.AspNetCore.Http.HttpContext, System.String)
                - Meta Type: System.Runtime.CompilerServices.NullableContextAttribute
                - Meta Type: Microsoft.AspNetCore.Routing.HttpMethodMetadata
    Endpoint: HTTP: GET /routes/info
            - Segment Count: 2
            - Parameters: 
            - Inbound Precedence: 1,1
            - Outbound Precedence: 5,5
            - Meta Count: 2
                - Meta Type: System.String <<Main>$>b__0_3(System.Collections.Generic.IEnumerable`1[Microsoft.AspNetCore.Routing.EndpointDataSource])
                - Meta Type: Microsoft.AspNetCore.Routing.HttpMethodMetadata
```

Some examples of the type of information available:
- The count and type of each segment making up the endpoint url (e.g. lines 2 and 10)
- The list of parameters for an endpoint (e.g. line 12)
- The lambda delegate for each endpoint (e.g lines 7 and 16)

The metadata information would need to be checked and cast to the specific type to retrieve additional information - but additional information is available.

---

## HttpContext route information

Information can also be received from the `HttpContext` of a specific request, for the endpoint being called. The information is the same as is contained in the `EndpointDataSource.Endpoints` collection used in the above examples:

``` csharp
app.MapGet("/routewithid/{id}", (HttpContext context, string id) => 
        $"Route `{context.GetEndpoint()?.DisplayName}` with id = '{id}'"); 
```

Calling this endpoint with an _id_ of _"sampleId"_ (`/routewithid/sampleId`) results in the following output:

``` powershell
    Route `HTTP: GET /routewithid/{id}` with id = 'sampleid'
```

---

## Notes

There are a number of useful outputs which could be done using this metadata information, such as outputting the information to be consumed into external api documentation, or [adding an endpoint graph to the application](https://andrewlock.net/adding-an-endpoint-graph-to-your-aspnetcore-application/). 

---

## References

[How to list all routes in an ASP.NET Core application](https://www.meziantou.net/list-all-routes-in-an-asp-net-core-application.htm)  
[Routing in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-6.0)

<?# DailyDrop ?>86: 01-06-2022<?#/ DailyDrop ?>
