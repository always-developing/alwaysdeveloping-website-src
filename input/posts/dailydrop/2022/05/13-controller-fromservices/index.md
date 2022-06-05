---
title: "Controller FromServices change in .NET 7"
lead: "How the FromServices requirement for controllers is changing in.NET 7"
Published: 05/13/2022
slug: "13-controller-fromservices"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - api
    - controller
    - minimal

---

## Daily Knowledge Drop

When using minimal apis in .NET6, a class can be injected into the relevant endpoint handler method, `without explicitly specifying where the instance is coming from` - it is determined by the runtime (and an error thrown if it could not determined).  

However when using controllers, and a class is being injected, it has to `explicably be stated` where the instance is coming from.

This changes in .NET7, where the controller behavior is changing to be more in-line with the minimal api behavior.


---

## Setup

In the below example, we have a _RandomNumberGenerator_ class, with one method which returns a random number:

``` csharp
public class RandomNumberGenerator
{
    public int GetRandomNumber(int max)
    {
        var generator = new Random();
        return generator.Next(max);
    }
}
```

In both examples below, this class is added to the dependency injection container as a singleton:

``` csharp
    builder.Services.AddSingleton<RandomNumberGenerator>();
```


## Minimal api

In the minimal api, the endpoint pattern is specified, as well as the lambda handler - any number of parameters can be passed to the handler. In the below case, an instance of _RandomNumberGenerator_ is passed in, a random number between 0 and 100 generated, and then returned:

``` csharp
app.MapGet("/randomnumber", (RandomNumberGenerator generator) =>
{
    return generator.GetRandomNumber(100);
})
.WithName("GetRandomNumber");
```

In the above example, the runtime can determine that a service of type `RandomNumberGenerator` has been registered with the dependency injection container, so this is most likely what needs to be injected into the endpoint handler.

## Controller api
### Before .NET 7

If we look at the same setup as above with the minimal api, but this time using controllers:

``` csharp
[ApiController]
[Route("[controller]")]
public class RandomNumberController : ControllerBase
{
    [HttpGet(Name = "RandomNumber")]
    public int Get(RandomNumberGenerator generator)
    {
        return generator.GetRandomNumber(100);
    }
}
```

However, when executing this, the following error is received:

``` json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.13",
  "title": "Unsupported Media Type",
  "status": 415,
  "traceId": "00-fd8fe900bf366defa058b51f685d11f1-896d209554184932-00"
}
```

This occurs because when using controllers before .NET7, if a parameter is coming from the dependency injection container, it has to `explicably be stated` using the `FromServices` attribute. 

``` csharp
[ApiController]
[Route("[controller]")]
public class RandomNumberController : ControllerBase
{
    [HttpGet(Name = "RandomNumber")]
    public int Get([FromServices]RandomNumberGenerator generator)
    {
        return generator.GetRandomNumber(100);
    }
}
```

With this attribute added on line 6, the controller endpoint can now be called successfully with a random number successfully being returned.

### .NET 7

This changes with .NET7 - when using controllers, the `FromServices` attribute can now also be  (optionally) omitted. The following **will work**:

``` csharp
[ApiController]
[Route("[controller]")]
public class RandomNumberController : ControllerBase
{
    [HttpGet(Name = "RandomNumber")]
    public int Get(RandomNumberGenerator generator)
    {
        return generator.GetRandomNumber(100);
    }
}
```

---

## Notes

This is a relatively minor change, but a welcome one - more closely aligning the behavior of minimal apis and controllers.

---

<?# DailyDrop ?>73: 13-05-2022<?#/ DailyDrop ?>
