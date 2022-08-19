---
title: "Correlation using the Activity class"
lead: "Using the Activity class to correlate requests across distributed systems"
Published: "09/13/2022 01:00:00+0200"
slug: "13-activity-correlation"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - correlation
   - activity
   - distributed

---

## Daily Knowledge Drop

When working with `distributed systems`, the built in `Activity` class can be used to automatically correlate requests across the various systems.  

For each unique request to an api (for example) a new activity root id is generated. If however, the specific api makes a call into another api, the activity root id is persisted across the http call, allowing the two calls to be linked together and related together when querying or reporting on the data.

---

## Single endpoint

First, we'll look at how to get access to the `activity id`. To do this we generate a simple minimal endpoint:

``` csharp
app.MapGet("/endpoint1", (IHttpClientFactory httpFactory) =>
{
    // Output the root Id, and the current Id
     Console.WriteLine($"Root Id: {Activity.Current.RootId} | " +
        $"Id: {Activity.Current.Id}");
});
```

Here the static `Activity` class is used, to access the _Current_ activity, and output the _Root Id_ as well as the _Id_.

Calling the endpoint a few times, results in the following:

``` terminal
Root Id: 319d4ff500ce3100c2a3017531e023e4 | Id: 00-319d4ff500ce3100c2a3017531e023e4-e4c21e8cfae4db47-00
Root Id: 7de97ec9692106503aaabab105951bdf | Id: 00-7de97ec9692106503aaabab105951bdf-fa3c7b807eda8800-00
Root Id: e1868729b9f3db8c40942ffb1daf24c9 | Id: 00-e1868729b9f3db8c40942ffb1daf24c9-79de739f0d38d58c-00
```

Each time the endpoint is invoked, and new _Root Id and Id_ is generated, with the _Id_ containing the _Root Id_.

---

## Multiple endpoints

Next, we define a second endpoint - for this example the second endpoint is defined in the same project as the first endpoint, but the exact same behavior would be experienced if the endpoint was contained in a separate application.

This second endpoint will return it's _Root Id and Id_ as a string:

``` csharp
app.MapGet("/endpoint2", () =>
{
    return $"Second Root Id: {Activity.Current.RootId} | " +
        $"Second Id: {Activity.Current.Id}";
});
```

Then we update the first endpoint to call the second endpoint:

``` csharp
app.MapGet("/endpoint1", async (IHttpClientFactory httpFactory) =>
{
        Console.WriteLine($"Root Id: {Activity.Current.RootId} | " +
        $"Id: {Activity.Current.Id}");

    // call the second endpoint
    var client = httpFactory.CreateClient();
    client.BaseAddress = new Uri("http://localhost:5065");
    var response = await client.GetAsync("endpoint2");

    // output the response (which contains the Id's)
    // from the second endpoint
    Console.WriteLine(await response.Content.ReadAsStringAsync());
    Console.WriteLine("------");

    
});
```

Now, when the first endpoint is called, we see the following (formatted to make it easier to compare):

```terminal
Root Id:        8f1e949168197f1185135e963eab68bc | Unit Id:   00-8f1e949168197f1185135e963eab68bc-2adc30476d372b95-00
Second Root Id: 8f1e949168197f1185135e963eab68bc | Second Id: 00-8f1e949168197f1185135e963eab68bc-d18ddb27c96b1a1b-00
------
Root Id:        2b39afb72f3773289d8d141b2ef030d4 | Unit Id:   00-2b39afb72f3773289d8d141b2ef030d4-77be0c303ee8028a-00
Second Root Id: 2b39afb72f3773289d8d141b2ef030d4 | Second Id: 00-2b39afb72f3773289d8d141b2ef030d4-66e8aa928caf5619-00
------
Root Id:        46d2eedeacdfe46a89888598886a5186 | Unit Id:   00-46d2eedeacdfe46a89888598886a5186-f7e42f6b6d868069-00
Second Root Id: 46d2eedeacdfe46a89888598886a5186 | Second Id: 00-46d2eedeacdfe46a89888598886a5186-1af4f5b27e45f9f7-00
------
```

As you can see, the `root id is the same across the http call`, even though it is being returned from a separate endpoint in another service. The _Unit Id_ portion of the _Id_ changes though, indicating a smaller unit of work is being performed as part of the larger _root_ piece of work.

---

## Child activity

We've seen how the the _Root Id_ is shared across http calls - now we look at how to get the same functionality when performing smaller units of work not involving http calls.

Once again we update first endpoint, this time to start a child activity:

``` csharp
app.MapGet("/endpoint1", async (IHttpClientFactory httpFactory) =>
{
    Console.WriteLine($"Root Id: {Activity.Current.RootId} | " +
        $"Id: {Activity.Current.Id}");

    var client = httpFactory.CreateClient();

    client.BaseAddress = new Uri("http://localhost:5065");
    var response = await client.GetAsync("endpoint2");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
    
    // start with the child activity
    using var childActivity = new Activity("MessagePublishing");
    childActivity.Start();

    // a message is published to a message broker here, with the 
    // Id as metadata/correlationId
    Console.WriteLine($"Child Root Id: {Activity.Current.RootId} | " +
        $"Child Id: {Activity.Current.Id}");

    childActivity.Stop();
    Console.WriteLine("------");

});
```

Here a child activity is manually started, and within the scope of the child activity - a message is published to a message broker (for example).

Invoking the endpoint now results in the following:

``` terminal
Root Id:        00a695a71d140a4105750a0cb04d9408 | Id:        00-00a695a71d140a4105750a0cb04d9408-4aba3488957c9a75-00
Second Root Id: 00a695a71d140a4105750a0cb04d9408 | Second Id: 00-00a695a71d140a4105750a0cb04d9408-e29b9eb28fe6d34f-00
Child Root Id:  00a695a71d140a4105750a0cb04d9408 | Child Id:  00-00a695a71d140a4105750a0cb04d9408-2a6e2ad5d7916142-00
------
```

From the above, we can see that manually declaring and _starting_ an activity will result in a _new Id_ to be generated, but using the same _Root Id_ as the parent.

---

## Why the need?

So why the need for a _Root Id_ and _correlation_ - all of this is to `gain better observability into how an application is performing`. This data can be output and collected, either using industry standard tools and formatting ([for example OpenTelemetry](https://opentelemetry.io/)), or by rolling our one's own reporting database - either way though, this provides insight into how each portion of a larger distributed transaction are linked together, and how each portion is performing.

---

## Notes

Distributed systems can become very complex, and observability is key in managing the stability and performance of the various systems - the `Activity` class provides an easy, simple way to manage the correlation between the various systems and processes.

---

## References

[Activity Class](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.activity?view=net-6.0)   

<?# DailyDrop ?>158: 13-09-2022<?#/ DailyDrop ?>
