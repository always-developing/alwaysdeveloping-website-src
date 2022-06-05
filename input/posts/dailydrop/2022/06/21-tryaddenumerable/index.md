---
title: "TryAddEnumerable to configure dependency injection"
lead: "How TryAddEnumerable operates differently to the other DI methods"
Published: "06/21/2022 01:00:00+0200"
slug: "21-tryaddenumerable"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dependencyinjection
    - tryaddenumerable

---

## Daily Knowledge Drop

When using the C# dependency injection container, the:
- `Add` methods (_AddSingleton_, _AddScoped_, _AddTransient_) will always add the interface and implementation to the container, even if it results in duplicate registrations
- `TryAdd` methods (_TryAddSingleton_, _TryAddScoped_, _TryAddTransient_) will only add the interface and implementation to the container if the _interface_ has not already been registered

Today we are looking specifically at `TryAddEnumerable`:
- `TryAddEnumerable` will only add the interface and implementation to the container if the _combination of the interface and implementation_ has not already been registered

---

## Endpoint example

In the three examples below, we have a simple setup, with an interface and a couple of implementations of the interface:

``` csharp
public interface IServiceInterface { }

public class ServiceImplementationOne: IServiceInterface { }

public class ServiceImplementationTwo : IServiceInterface { }
```

And then a minimal endpoint which has _IEnumerable\<IServiceInterface\>_ injected, and returns all implementations:

``` csharp
app.MapGet("/endpoint", (IEnumerable<IServiceInterface> service) =>
{
    return service.Select(s => s.GetType().Name);
});
```

---

### Add methods

First, lets look at the behavior of the _Add_ methods. These methods will always add the interface and implementation, even if its already been added.

In the dependency injection configuration, in this example, a duplicate implementation of _ServiceImplementationTwo_ is added:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationOne));
builder.Services.AddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo));
builder.Services.AddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo));

var app = builder.Build();
```

The endpoint will return the following - _ServiceImplementationTwo_ has been added to the dependency injection container twice, and is therefor the duplicate was injected into and returned from the endpoint:

``` json
[
  "ServiceImplementationOne",
  "ServiceImplementationTwo",
  "ServiceImplementationTwo"
]
```

---

### TryAdd methods

The _TryAdd_ methods operate differently, and will ensure that only one implementation of the interface is added to the container.

In the dependency injection configuration, in this example, we use the same configuration as in the first example, but with the _TryAddTransient_ method instead of the _AddTransient_ method:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.TryAddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationOne));
builder.Services.TryAddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo));
builder.Services.TryAddTransient(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo));

var app = builder.Build();
```

The endpoint will return the following - only the first implementation of _IServiceInterface_ added to the dependency injection container is registered.

``` json
[
  "ServiceImplementationOne"
]
```

Neither of the two registrations of _ServiceImplementationTwo_ are actioned, because an implementation of _IServiceInterface_ (ServiceImplementationOne) was already added.

---

### TryAddEnumerable method

The _TryAddEnumerable_ method is used slightly differently - this method takes a _ServiceDescriptor_ instance, and not a interface and implementation directly.

In the dependency injection configuration, in this example, we use the the same configuration as in the above example, with a slightly different setup. We define three instances of _ServiceDescriptor_, two of them are describing implementations of _ServiceImplementationTwo_:

``` csharp
var builder = WebApplication.CreateBuilder(args);

var descriptorOne = new ServiceDescriptor(typeof(IServiceInterface), 
    typeof(ServiceImplementationOne), ServiceLifetime.Transient);
var descriptorTwo = new ServiceDescriptor(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo), ServiceLifetime.Transient);
var descriptorThree = new ServiceDescriptor(typeof(IServiceInterface), 
    typeof(ServiceImplementationTwo), ServiceLifetime.Transient);

builder.Services.TryAddEnumerable(new[] { descriptorOne, descriptorTwo, descriptorThree });

var app = builder.Build();
```

The endpoint will return the following - only the first implementation of each unique _interface_ + _implementation_ combination:

``` json
[
  "ServiceImplementationOne",
  "ServiceImplementationTwo"
]
```

---

## Notes

The _TryAddEnumerable_ method is incredible useful, especially as a library author where your library might need to add items to the dependency injection container, but without knowing if another library has already added it. _TryAddEnumerable_ can be used to ensure that the dependency injection container is not "polluted" with duplicate identical implementations, which could potentially cause issues.

---

## References

[The .NET dependency injection methods you are not using](https://www.youtube.com/watch?v=iQ8cNI7a6mk)  

<?# DailyDrop ?>100: 21-06-2022<?#/ DailyDrop ?>
