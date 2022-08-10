---
title: ".NET 7 Rate limiting"
lead: "An introduction into rate limiting coming with .NET 7"
Published: "08/31/2022 01:00:00+0200"
slug: "31-rate-limiting"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net7
   - ratelimiter
   - ratelimiting

---

## Daily Knowledge Drop

Built in `rate limiting support` is being introduced with .NET 7, and provides a way to protect a resource from being overwhelmed with requests.

The rate limiting can be applied to an endpoint(s) to automatically control how often it can be called (API rate limiting), but in addition it can also be manually leveraged to control how often any resource in the applications gets used (such as a database connection)

All below examples were written using .NET 7-preview5, and as such may differ from the final release or subsequent preview releases.

---

## Rate limiter types

There are a number of different rate limiting algorithms available in .NET 7 to control the flow of requests. We will not go into detail in this post, but full details on each one can be found in the references link below:
- `Concurrency` limit
- `Token bucket` limit
- `Fixed window` limit
- `Sliding window` limit

In the examples below, we make use of, and explore in a little more depth, the `fixed window` and `token bucket` limiters.

---

## Limiting an API 

The most obvious and well-known usage for rate limiting, is to apply it to an API endpoint to control how often it can get called. A few examples on why one might want to do this:
- To prevent API being overwhelmed with requests from any source
- To control how often a specific user can call the API based on account balance (or account tier)
- To limit the number of requests from a specific IP address (in the case of endpoint abuse)

In the example below, a `fixed windows` rate limiter will be applied to an endpoint. Two NuGet package references are required:
- `System.Threading.RateLimiting` - provides the based rate limiting functionality
- `Microsoft.AspNetCore.RateLimiting` - provides functionality to integrate rate limiting into AspNetCore middleware

Two steps are involved when using rate limiting with an endpoint:
- Defined the rate limit policy
- Apply the policy to an endpoint(s)

---

### Define the policy

The policy is defined on startup by using the `UseRateLimiter` extension method:

``` csharp

app.UseRateLimiter(new RateLimiterOptions()
    .AddFixedWindowLimiter("getlimiter", 
        new FixedWindowRateLimiterOptions(1, 
            QueueProcessingOrder.OldestFirst, 
            0, 
            TimeSpan.FromSeconds(2)))
    );
```

Here the `FixedWindowLimiter` is being used, with the policy name being _"getlimiter"_. Effectively the policy states that: `During the 2 second window, only 1 request may be served, with no requests allowed to queue`.

Now that the policy is defined, the next step is to apply it to an endpoint.

---

### Apply the policy

In the below example, we have a minimal api endpoint which returns the current datetime. Applying the rate limiter is as easy as invoking the `RequireRateLimiting` method:

``` csharp
app.MapGet("/getwithlimit", () =>
{
    return DateTime.Now;

}).RequireRateLimiting("getlimiter");
```

The _RequireRateLimiting_ method is called with the policy name to apply specified.

If the endpoint is now invoked more than once in a 2 second window, the called will receive a `503: Service unavailable` response.

---

## Limiting a resource

It is also possible to control the flow of requests to any part of the code or resource using the rate limiting functionality.

In the following example, we have an endpoint which can either `return data from a cache or from the database`. We only want to _limit the number of requests which go to the database_ using a rate limiter.

### Define the policy

First step again, is to define the policy.

``` csharp
TokenBucketRateLimiter tokenLimiter = 
    new TokenBucketRateLimiter(
        new TokenBucketRateLimiterOptions(0, 
            QueueProcessingOrder.OldestFirst,
            0, 
            TimeSpan.FromSeconds(10), 
            5));

builder.Services.AddSingleton(tokenLimiter);
```

Here the _TokenBucketRateLimiter_ is being used - it is defined and added as a singleton to the dependency injection container. Effectively the policy states that: `Every 10 second period, 5 tokens will be added to the bucket (with a max of 5 tokens in the bucket), with no requests allowed to queue`. Once the tokens are all taken, no requests will be allowed until the bucket is replenished.

---

### Apply the policy

We can now inject the policy into a constructor, or API endpoint

``` csharp
// inject the limiter as well as the DatabaseResource to limit
// the number of requests to
app.MapGet("/limitresource", (TokenBucketRateLimiter tokenLimiter, 
    DatabaseResource databaseAccess) =>
{
    // use Random to simulate if the database should be 
    // called or if the cache should be used
    var random = new Random();
    var shouldCallDatabase = random.Next(0, 2);

    // database should be called, rate limiting applies
    if (shouldCallDatabase == 1)
    {
        // try acquire a token
        var lease = tokenLimiter.Acquire();
        // if there was a token available
        // then we can call the database
        if (lease.IsAcquired)
        {
            return databaseAccess.GetData();
        }

        // otherwise, unable to call at this time
        return "Unable to retrieve data at this time";
    }

    // from cache, no rate limiting
    return "Retrieved from cache";

});
```

The basic flow is to try _acquire_ a lease on a token from the rate limiter instance - if no token is available, the lease is not acquired and a relevent message is returned.  
In this specific example, a `200 Success` will still be returned in all branches of the flow, but this could be manually changed handled to change the HTTP response code based on the branch taken by the code.

---

## Notes

This is a very useful and easy to configure feature coming with .NET 7. If already using an _API Management (APIM)_ tool, which usually has rate limiting functionality included, this feature might not be _as_ useful, but can still play its part. Often some internal traffic doesn't always go through the APIM, so the .NET rate limiting feature could be leveraged for those cases, and used in conjunction with the APIM.

---

## References

[Announcing Rate Limiting for .NET](https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/)   

<?# DailyDrop ?>150: 31-08-2022<?#/ DailyDrop ?>
