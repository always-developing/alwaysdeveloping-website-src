---
title: "ObjectPool for resource management"
lead: "Using the ObjectPool class to keep objects in memory for reuse"
Published: "10/13/2022 01:00:00+0200"
slug: "13-objectpool"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - object
   - pooling
   - objectpool

---

## Daily Knowledge Drop

Th `ObjectPool` class can be leveraged to keep a _group of objects in memory for reuse_ rather than allowing the objects to be garbage collected and having to be reinitialized every time required.

This is especially useful if the object:

- is expensive to allocate/initialize.
- represent some limited resource.
- is used predictably and frequently.

---

## Usage

Usage of the `ObjectPool` consists of a few steps:

### ObjectPoolProvider

The `ObjectPoolProvider` is used to ultimately _create the ObjectPool_ based on a _policy_. The below code snippet uses the `DefaultObjectPoolProvider`.

The dependency injection container is configured as follows:

``` csharp
builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
```

When the `DefaultObjectPoolProvider` is used and the item stored in the pool (_StringBuilder_ in the following examples) implements IDisposable, Then:
- items that are not returned to the pool will be disposed
- when the pool gets disposed by the dependency injection container, all items in the pool are disposed

---

### ObjectPool

The next step is to register the `ObjectPool` itself with the dependency injection container, in this case using an implementation factory:

``` csharp
builder.Services.TryAddSingleton<ObjectPool<StringBuilder>>(serviceProvider =>
{
    // get the provider registered in the previous step
    var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
    // create a policy, using the built in string builder policy
    var policy = new StringBuilderPooledObjectPolicy();
    // return an ObjectPool<StringBuilder> instance created using the policy
    return provider.Create(policy);
});
```

---

### Injection

`ObjectPool<StringBuilder>` can now be injected into the relevent method/delegate:

``` csharp
app.MapGet("/buildstring", (ObjectPool<StringBuilder> pool) =>
{
    // get a StringBuilder instance from the pool
    var sBuilder = pool.Get();

    try
    {
        // use the string builder
        sBuilder.Append("This string has been built up using ");
        sBuilder.Append("a StringBuilder instance from an ");
        sBuilder.Append("ObjectPool<StringBuilder> instance");

        return sBuilder.ToString();
    }
    finally
    {
        // return it to the pool
        pool.Return(sBuilder);
    }
});
```

- The `Get` method on `ObjectPool` is used to get an instance of the class from the pool (_StringBuilder_ in this example)
- The `Return` method is used to return the instance back to the `ObjectPool`, making it available for reused in the future

---

## Performance

While using the `ObjectPool` can definitely increase performance, it does not always guarantee it. Object pooling usually won't improve performance:
- unless the initialization cost of an object is high, it's usually slower to get the object from the pool.
- objects managed by the pool aren't de-allocated until the pool is de-allocated.

---

## Notes

Under the right use-case object pool can definitely make a difference to the performance of an application. However, if used incorrect it can be detrimental - so if performance is a concern, benchmark and make an informed decision for the specific use case.

---

## References

[Object reuse with ObjectPool in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/objectpool?view=aspnetcore-6.0)   

<?# DailyDrop ?>181: 13-10-2022<?#/ DailyDrop ?>
