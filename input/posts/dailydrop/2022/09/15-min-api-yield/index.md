---
title: "Streaming responses from a minimal api "
lead: "How to use IAsyncEnumerable amd yield in a minimal api"
Published: "09/15/2022 01:00:00+0200"
slug: "15-min-api-yield"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - minimal
   - yield
   - iasyncenumerble

---

## Daily Knowledge Drop

Using _"usual techniques"_, it is not possible to stream a response from a minimal api, as the `yield keyword cannot be used inside an anonymous method or lambda expression`. To work around this limitation, a `local function` can be created which returns _IAsyncEnumerable_, and the local function result in turn, is returned from the minimal api.

---

## Limitation

Suppose we need to define an endpoint, which when called will stream a _bool response_ indicating if current date time minute is 13 or not (not very practical or useful, but will work for demo purposes):

``` csharp
app.MapGet("/isminute13", async () =>
{
    // when the endpoint is called, loop until
    // the user cancels
    while (true)
    {
        // return a bool indicating if the current minute is 13
        yield return (DateTime.Now.Minute == 13);

        // wait 1 second, and repeat
        await Task.Delay(1000);
    }
});
```

The issue here, is that it `does not compile`, as:

``` terminal
The yield statement cannot be used inside an anonymous method or lambda expression
```

But, there is a way around this constraint of the lambda expression!

---

## Local function

The error itself gives a clue on how to solve the problem - yield cannot be used inside a _anonymous method or lambda expression_, so `why don't not instead create and use a non-anonymous method!`:

``` csharp
app.MapGet("/isminute13", () =>
{
    // move all previous logic into this local method
    async IAsyncEnumerable<bool> DateTimeStream()
    {
        while (true)
        {
            yield return (DateTime.Now.Minute == 13);
            await Task.Delay(1000);
        }
    }

    // return the result of the method
    return DateTimeStream();
});
```

Executing the above code, and calling the endpoint will result in the following output:

``` terminal
[false,false,false,false,false,false,false,false,false,true,true,true
```

Every second a new bool will be appended to the response, indicating if the minute is 13 or not.

Simple, easy solution with minimal additional coding!

---

## Notes

Streaming responses from an api is not something I've seen leveraged much before - however with the simplicity of configuring a minimal api for streaming, it is quick and easy to get an application up and running to determine if streaming can add value to the application or business.

---

## References

[David Fowler Tweet](https://twitter.com/davidfowl/status/1436706303586410503)   

<?# DailyDrop ?>161: 15-09-2022<?#/ DailyDrop ?>
