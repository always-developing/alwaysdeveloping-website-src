---
title: "Delaying injected dependency instantiation"
lead: "Using Lazy to delay the instantiation of injected dependencies"
Published: "09/26/2022 01:00:00+0200"
slug: "26-lazy-injection"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - dependencyinjection
   - lazy
   - DI

---

## Daily Knowledge Drop

The `Lazy` class can be leveraged to delay instantiation of implementations `injected via dependency injection`. This is especially useful if the _instantiation is high cost_ (it has a nigh number of dependencies) and the _functionality of the dependency is not used in every code path_.

We have previously looked at the [Lazy class](../../03/09-lazy-class/), but not how its functionality can be used in conjunction with _dependency injection_, covered by this post.

---

## Methods

### Normal injection

In the examples below, we have a `IDatabaseAccess` interface, and a `SqlDatabaseAccess` implementation:

``` csharp
// basic interface
public interface IDatabaseAccess 
{
    void PerformDatabaseUpdate();
}

// the SQL implementation of the interface
public class SqlDatabaseAccess : IDatabaseAccess 
{
    public SqlDatabaseAccess()
    {
        Console.WriteLine($"In constructor of '{nameof(SqlDatabaseAccess)}'");
    }

    public void PerformDatabaseUpdate()
    {
        Console.WriteLine($"Database update performed");
    }
}
```

We also have an endpoint, where only on _some paths_ is the database accessed:

``` csharp
app.MapGet("/maybe-do-work", (IDatabaseAccess databaseAccess) =>
{
    // perform a random check
    var rando = new Random();
    if(rando.Next(10) > 5)
    {
         databaseAccess.PerformDatabaseUpdate();
    }

    return "'maybe-do-work' endpoint successfully called";
});
```

With the dependency injection container setup as follows:

``` csharp
builder.Services.AddTransient<IDatabaseAccess, SqlDatabaseAccess>();
```

The problem here is that the `IDatabaseAccess implementation is always initialized even if not used`. This might be fine if the implementation construction is low cost, but if the _IDatabaseAccess_ implementation is high cost, and and itself has many dependencies, this could have an impact on performance.

Let's look at how the `Lazy` class can be used to achieve a delayed instantiation.

---

### Lazy injection

To leverage the `Lazy` class, the first step involves setting up the dependency injection container with `Lazy<IDatabaseAccess>`:

``` csharp
builder.Services.AddTransient<IDatabaseAccess, SqlDatabaseAccess>();
builder.Services.AddTransient<Lazy<IDatabaseAccess>>(
    provider =>
    {
        Console.WriteLine("In 'Lazy<IDatabaseAccess>' implementation factory");

        // be sure to use the "valueFactory" constructor and 
        // NOT the "value" constructor
        return new Lazy<IDatabaseAccess>(
            () => provider.GetRequiredService<IDatabaseAccess>());
    });
```

And then injecting `Lazy<IDatabaseAccess>` into the delegate, instead of `IDatabaseAccess`:

``` csharp
app.MapGet("/maybe-do-work-lazy", (Lazy<IDatabaseAccess> databaseAccess) =>
{
    var rando = new Random();

    if (rando.Next(10) > 5)
    {
        databaseAccess.Value.PerformDatabaseUpdate();
    }

    return "'maybe-do-work-lazy' endpoint successfully called";
});
```

Executing the code and calling the endpoint results in the following (example output):

``` terminal
In Lazy<IDatabaseAccess> implementation factory
In Lazy<IDatabaseAccess> implementation factory
In Lazy<IDatabaseAccess> implementation factory
In constructor of 'SqlDatabaseAccess'
Database update performed
```

As we can see, `SqlDatabaseAccess is only initialized when it is being used` - exactly what we are after.

---

### ServiceProvider injection

Another option is to `inject IServiceProvider directly` and get the service only when required - however this is considered by some to be an _anti-pattern_.

The above example is updated to look as follows:

``` csharp
app.MapGet("/maybe-do-work-provider", (IServiceProvider provider) =>
{
    Console.WriteLine("In `maybe-do-work-provider` endpoint");
    var rando = new Random();

    if (rando.Next(10) > 5)
    {
        var databaseAccess = provider.GetRequiredService<IDatabaseAccess>();
        databaseAccess.PerformDatabaseUpdate();
    }

    return "'maybe-do-work-provider' endpoint successfully called";
});
```

And calling the endpoint:

``` terminal
In `maybe-do-work-provider` endpoint
In `maybe-do-work-provider` endpoint
In constructor of 'SqlDatabaseAccess'
Database update performed
In `maybe-do-work-provider` endpoint
In `maybe-do-work-provider` endpoint
```

Again, `SqlDatabaseAccess is only initialized when it is being used`.

---

## Notes

A very useful technique to use when the instantiation of an object has a large overhead, but it not used in all code paths. The constructor in general should be kept as quick and performant as possible, but sometimes its unavoidable to have to use a _high cost_ constructor (if using a 3rd party package, for example).

---

## References

[Delayed Instantiation Using Dependency Injection In .NET](https://thecodeblogger.com/2021/04/28/delayed-instantiation-using-dependency-injection-in-net/)   

<?# DailyDrop ?>168: 26-09-2022<?#/ DailyDrop ?>
