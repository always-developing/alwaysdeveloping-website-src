---
title: "Generic attributes with C#11"
lead: "Starting with C# 11 attributes can now contain generic parameters"
Published: "05/31/2022 01:00:00+0200"
slug: "31-generic-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - attribute
    - generic

---

## Daily Knowledge Drop

Coming in C# 11 (later this year with .NET 7) is the `generic attributes` feature - the ability to define an attribute which takes a generic parameter. This is a more convenient style for attributes which require a `Type` parameter.

Generic attribute code samples below were written using the .NET 7 preview release.

---

## Pre C# 11 - Type parameter

Sometimes an attribute needs to take a `Type` parameter - currently (using any version prior to C# 11) the only way to do this is passing a `Type` parameter to the constructor.

``` csharp
[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class TypeAttribute : Attribute
{
    readonly Type _classType;

    public TypeAttribute(Type classType)
    {
        if(classType is not IMarkerInterface)
        {
             throw new Exception($"Parameter '{classType}'" +
                $" must implement `IMarkerInterface`");
        }

        _classType = classType;
    }

    public Type ClassType
    {
        get { return _classType; }
    }
}
```

A drawback of this approach is that if there are any constraints on the type of `Type` then a check needs to explicitly be done in the constructor - as is done above, ensuring that the type implements _IMarkerInterface_. However this check is _only done at runtime_, quite late in the development loop. 

Usage of the above attribute:

``` csharp
[TypeParam(typeof(ImplementationType))]
public class UsingTypeAttribute
{
}
```


---

## C# 11 - Generic attribute

C# 11 introduces generic attributes - which use the same syntax as generic classes or methods. Below is the same attribute as above, but implemented using generics:

``` csharp
[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class GenericTypeAttribute<T> : Attribute where T : IMarkerInterface
{
    public GenericTypeAttribute()
    {
    }

    public Type ClassType
    {
        get { return typeof(T); }
    }
}
``

As one can see, the code is a lot cleaner, with the type of `Type` being constrained as part of the generics - no need to manually check the type in the constructor. The check is also automatically performed at compile time so any issues with the `Type` are picked up earlier.

Usage of the above attribute is also cleaner:

``` csharp
[GenericType<ImplementationType>]
public class UsingGenericAttribute
{
}
```

---

## Notes

While this may not be something used every day by everyone, for those which do use it, it will be an incredibly useful upgrade - bringing more standardization across the various parts of the C# language.

---

## References

[Generic attributes](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11#generic-attributes)  

<?# DailyDrop ?>85: 31-05-2022<?#/ DailyDrop ?>
