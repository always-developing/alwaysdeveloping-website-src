---
title: "ActivatorUtilities to create instances"
lead: "The ActivatorUtilities class can be used to create instances of classes, in conjunction with DI"
Published: "08/02/2022 01:00:00+0200"
slug: "02-activatorutilities"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - activatorutilities
    - DI
---

## Daily Knowledge Drop

The `ActivatorUtilities` static class can be used to create instances of classes `outside of the dependency injection (DI) container`, while still `leveraging the DI container` to create instances of the dependencies.

---

## Examples

Consider a business logic class, which has one dependency on `IConfiguration`:

``` csharp
public class MyBusinessLogic
{
    private readonly IConfiguration _configuration;

    public MyBusinessLogic(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public int PerformBusinessLogic()
    {
        Console.WriteLine("Performing business logic");

        return 1;
    }
}
```

It consists of a _Constructor_ and a single method, _PerformBusinessLogic_, which performs some business logic, and returns the value `1` once completed.

In all of the below examples, the _MyBusinessLogic_ class `has NOT been registered with the dependency injection container`.

The code for all the endpoints shown below is as follows:

``` csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// endpoint definition show below goes here!

app.Run();

```

Thats it - nothing else being registered or configured on startup.

---

### Default DI

First, we'll try getting an instance of class directly from the DI container, two different ways:

``` csharp
// inject directly
app.MapGet("/direct", ([FromServices]MyBusinessLogic logic) =>
{
    return logic.PerformBusinessLogic();
});

// inject the service provider (the DI container)
// and try get the service from there
app.MapGet("/provider", (IServiceProvider provider) =>
{
    var logic = provider.GetRequiredService<MyBusinessLogic>();

    return logic.PerformBusinessLogic();
});
```

If you've worked with DI before, you would be unsurprised to know that the above does not work:

``` terminal
InvalidOperationException: No service for type 'MyBusinessLogic' has been registered.
```

The runtime doesn't know how to instantiate the _MyBusinessLogic_ class. To resolve we could register _MyBusinessLogic_ with the DI container, but for this post the assumption is this is not an option.

---

### Manual

Next we'll look at the ways to manually get an instance of the _MyBusinessLogic_ class:

The first way is to just manually instantiate the class directly. The constructor of _MyBusinessLogic_ requires an instance of `IConfiguration`. In the below, `IConfiguration` is injected via DI, and then passed to the constructor of _MyBusinessLogic_:

``` csharp
app.MapGet("/manual", (IConfiguration config) =>
{
    var logic = new MyBusinessLogic(config);

    return logic.PerformBusinessLogic();
});

```

`This will work`, but if _MyBusinessLogic_ had a long list of dependencies and it's constructor required many parameters, this can become messy. The endpoint would need to be modified to accept items from the DI container when it doesn't actual require them directly - they are only used to pass to _MyBusinessLogic_.

As slight improvement is to only inject the _IServiceProvider_ implementation into the endpoint and then use that to get the required items for the _MyBusinessLogic_ constructor:

``` csharp
app.MapGet("/manualprovider", (IServiceProvider provider) =>
{
    var logic = new MyBusinessLogic(provider.GetService<IConfiguration>());

    return logic.PerformBusinessLogic();
});
```

Again, this `will work`, but is still not ideal, as each type required needs to manually retrieved from the DI container.

---

### ActivatorUtilities

Thankfully, there is a class to assist with this exact requirement - the `ActivatorUtilities` static class. Its usage is very simple - the _CreateInstance_ method is called with the required class passed in as a generic parameter, along with the _IServiceProvider_ implementation as a parameter (any other parameters which might be required, but are not part of the DI container).  
`ActivatorUtilities` will then return an instance of the require class, using the _IServiceProvider_ implementation to resolve any dependencies automatically - as would happen when using the DI container implicitly.

As the required _IConfiguration_ parameter is already in the DI container, no additional parameters need to be passed in: 

``` csharp
app.MapGet("/activatorutils", (IServiceProvider provider) =>
{
    var logic = ActivatorUtilities.CreateInstance<MyBusinessLogic>(provider);

    return logic.PerformBusinessLogic();
});
```

Here an `instance of MyBusinessLogic is created`, and all its dependencies are `automatically resolved from the IServiceProvider instance`, provider.

---

## Notes

An incredibly useful method when working with dependency injection, but not all classes have been added to the DI container (for example, if in the processes of porting a legacy app, one controller at a time).

There are other ways of doing this using reflection (_Activator.CreateInstance_ for example) not mentioned here - this post focuses on instantiating a class when the type wanted is known at compile time.

---

## References

[Activator utilities: activate anything!](https://onthedrift.com/posts/activator-utilities/)   

---

<?# DailyDrop ?>129: 02-08-2022<?#/ DailyDrop ?>
