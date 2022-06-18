---
title: "Dependency Injection with Func and delegates"
lead: "Injecting a Func or Delegate using dependency injection"
Published: 03/07/2022
slug: "07-di-func-delegate"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - DI
    - dependencyinjection
    - injection
    - dependency
    - delegate
    - func

---

## Daily Knowledge Drop

When requiring to abstract and inject a simple single method interface using dependency injection (DI), its possible to instead use a `Func<>` or a `Delegate` instead.

---

## Examples

Suppose we want to inject the date time using dependency injection, instead of using _DateTime.Now_ or _DateTimeOffSet.Now_.

There are a few reasons to do this:
- _DateTime.Now_ returns the current system datetime. This does not account for users in different regions, daylight savings etc (this is a complex subject, see the NodaTime reference below for additional information)
- The current datetime is an external dependency - best practice is for all external dependencies to be abstracted, so that they can be mocked and successfully unit tested

There are a number of ways to tackle this requirement. In the below examples, we'll take a look at the various approaches using _System clock_ as well as a 3rd party library, _NodaTime_

### Interface

The first approach is using an interface and implementation(s). This is the more _traditional_ approach:

``` csharp
// define the base interface
public interface IClock
{
    DateTimeOffset GetNow();
}

// implementation using system clock
public class SystemClock : IClock
{
    public DateTimeOffset GetNow()
    {
        return DateTimeOffset.Now;
    }
}

// implementation using 3rd party NodaTime library
public class NodaTimeClock : IClock
{
    public DateTimeOffset GetNow()
    {
        return NodaTime.SystemClock.Instance
            .InUtc().GetCurrentOffsetDateTime().ToDateTimeOffset();
    }
}
```

This can then be added to the DI container and injected into the relevant constructor:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// The relevant IClock implementation is added to the DI container.
builder.Services.AddTransient<IClock, NodaTimeClock>();
// OR
// builder.Services.AddTransient<IClock, SystemClock>();

var app = builder.Build();

// The interface is injected
app.MapGet("/currentdatetime", (IClock clock) =>
{
    return clock.GetNow();
});

app.Run();

```

This will certainly work, and perhaps is even the "best" approach - but it does seem like overkill to create an interface and implementation for something as simple as _getting the datetime_.

---

### Func

The next approach is injecting a `Func<>` into the DI container. A `Func<>` is a pointer to a method, which accepts zero or many parameters and has a return value.

We don't need to interface and implementation, it can all be done while adding to the DI container (although you can certainly create a method separately and use that when adding to the DI container)

``` csharp
var builder = WebApplication.CreateBuilder(args);

// Add a Func which returns a DateTimeOffset
// and define it as a lambda expression
builder.Services.AddTransient<Func<DateTimeOffset>>(
    dt => () => { return DateTimeOffset.Now; });
// OR
// builder.Services.AddTransient<Func<DateTimeOffset>>(dt => 
//    () => { return NodaTime.SystemClock.Instance
//        .InUtc().GetCurrentOffsetDateTime().ToDateTimeOffset(); });

var app = builder.Build();

// the Func can be injected
app.MapGet("/currentdatetime", (Func<DateTimeOffset> now) =>
{
    // call the Func like a method
    return now();
});

app.Run();

```

Again , this approach will work - one drawback of this though, is the `Func` doesn't give any context as to what the method actually does. `Func<DateTimeOffset>` doesn't convey that when called, the _current date time is being returned_.

---

## Delegate

The final approach is to use a `delegate`. A `delegate` is a type, which can be "instantiated" (just like other types), but points to a method not a value as such.

First the `delegate` is defined:

``` csharp
    public delegate DateTimeOffset GetCurrentDateTime();
```

Next we inject into the DI container and tell it how to "instantiate" the type _GetCurrentDateTime_:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// GetCurrentDateTime is defined using the lambda expression, 
// and injected into the DI container
builder.Services.AddTransient<GetCurrentDateTime>(dt =>
    () => { return NodaTime.SystemClock.Instance
        .InUtc().GetCurrentOffsetDateTime().ToDateTimeOffset(); });
// OR
//builder.Services.AddTransient<GetCurrentDateTime>(dt => 
//    () => { return DateTimeOffset.Now; });

var app = builder.Build();

// The `delegate` type is injected
app.MapGet("/currentdatetime", (GetCurrentDateTime now) =>
{
    return now();
});

app.Run();

```

This approach has the benefit of the `delegate` being a type with a name which conveys what the method is doing -`GetCurrentDateTime` is informative.

---

## Notes

Three different approaches to solving the same problem - there is no "right or wrong" (perhaps some are consider better best practice when compared with others). Each has their own pros and cons, which need to be evaluated and an informed decision made for each use case.

---

## References
[Dustin Moris Gorski tweet](https://twitter.com/dustinmoris/status/1488609307621044230)  
[Why does NodaTime exist?](https://nodatime.org/3.0.x/userguide/rationale)

<?# DailyDrop ?>25: 07-03-2022<?#/ DailyDrop ?>
