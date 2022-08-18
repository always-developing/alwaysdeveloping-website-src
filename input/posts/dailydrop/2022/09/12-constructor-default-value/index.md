---
title: "Constructors with default values"
lead: "Constructors with default values and dependency injection behavior"
Published: "09/12/2022 01:00:00+0200"
slug: "12-constructor-default-value"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - default
   - dependencyinjection
   - constructor

---

## Daily Knowledge Drop

`Constructor and minimal api parameters` can have `default values`, allowing for dependency injection to work - while not actually working. Specifying a default value allows for the application to _run with a default implementation_ even if the services has not been registered with the dependency injection container. 

This is not something one would usually do as a standard practice, but in specific use cases it does have a place.

---

## Example
### Setup

In the below examples, we have an interface, and a couple of implementations:

The interface:

``` csharp
public interface IDependencyInterface 
{
    string GetName();
}
```

And the implementations:

``` csharp
// a default implementation
public class DefaultImplementation : IDependencyInterface
{
    public string GetName()
    {
        return nameof(DefaultImplementation);
    }
}

// and another implementation
public class OtherImplementation : IDependencyInterface
{
    public string GetName()
    {
        return nameof(OtherImplementation);
    }
}
```

---

### Injection

Usually to make use of the functionality provided by the interface and the associated implementation, the service is registered with the dependency injection container:

``` csharp
// Depending on the implementation the application requires:
builder.Services.AddTransient<IDependencyInterface, DefaultImplementation>();
// OR
builder.Services.AddTransient<IDependencyInterface, OtherImplementation>();
```

and then is injected into the relevent constructor, or minimal api delegate in the below sample:

``` csharp
// explicitly tell it to get the IDependencyInterface implementation
// from the DI service collection
app.MapGet("/name", ([FromServices]IDependencyInterface injected) =>
{
    return injected.GetName();
});
```

---

### DI Assumption

In the above setup, the runtime assumes that the necessary registrations with the dependency injection container have take place. If neither of the following registrations are  done:

``` csharp
builder.Services.AddTransient<IDependencyInterface, DefaultImplementation>();
builder.Services.AddTransient<IDependencyInterface, OtherImplementation>();
```

when trying to inject _IDependencyInterface_, the following error will be experienced:

``` terminal
InvalidOperationException: No service for type 'IDependencyInterface' has been registered.
```

We've told the runtime and dependency injection container to _inject the IDependencyInterface implementation, but have not made it aware of any implementations!_

---

### Default value

Generally if in control of the entire dependency injection container registration, one should ensure that the required registrations are performed, and the error is resolved.  

However, in some cases this might not be possible - for example if developing a library package, there is no direct control over what the developer configures with the dependency injection container.  
As a developer of the library package, one could just allow the exception to occur, which directs the developer to configure the dependency injection container correctly. Another, arguably more developer friendly technique, is to `automatically set a default implementation if one is not explicitly set`. This does require a few updates to the code:

``` csharp
// make the parameter nullable, the default value will be null.
// In the case of a constructor (vs this minimal api delegate)
// the value can explicitly be set to null if desired:
// IDependencyInterface? injected = null
app.MapGet("/name", ([FromServices]IDependencyInterface? injected) =>
{
    // if it is null, set it to the default implementation
    injected ??= new DefaultImplementation();

    return injected.GetName();
});
```

Two changes are made to code:
- Set the parameter as `nullable` using the `?` operator
- Instantiate the parameter to the `default implementation` if the value of the parameter is null

Now if `no implementation is registered` for IDependencyInterface, the application will still function and use the `default implementation` specified.

---

## Notes

As mentioned, this is not a practice which is generally recommended - however in the case of developing an external library (which requires a dependency), there is no guarantee the host application has injected the required dependencies. By making use of `default values` the library will _still function with default configuration_, but allows for `specific implementations to be overwritten` by the developer if required.

---

## References

[Christian Findlay Tweet](https://twitter.com/CFDevelop/status/1556055501661929472)   

<?# DailyDrop ?>158: 12-09-2022<?#/ DailyDrop ?>
