---
title: "Filter IEnumerable with OfType"
lead: "LINQ has a built in method to automatically filter a collection by a specific Type"
Published: "08/19/2022 01:00:00+0200"
slug: "19-linq-oftype"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - typeof

---

## Daily Knowledge Drop

LINQ provides a built in method to filter collection contents _by type_, `OfType<>`. This is considerably easier and more streamline than the alternative of using a `Where` in combination with a `Select` (which I had previously been using)

---

## Setup

In the below example, the setup is as follows:

We have three interfaces:

``` csharp
public interface IBaseType {}

public interface ITypeA
{
    void ExecuteTypeAProcessing();
}

public interface ITypeB
{
    void ExecuteTypeBProcessing();
}

```

And two classes which implement these interfaces:

``` csharp
// implement the base interface and TypeA interface
public class TypeA : IBaseType, ITypeA
{
    public void ExecuteTypeAProcessing()
    {
        Console.WriteLine($"{nameof(ExecuteTypeAProcessing)} has been called");
    }
}

// implement the base interface and TypeB interface
public class TypeB : IBaseType, ITypeB
{
    public void ExecuteTypeBProcessing()
    {
        Console.WriteLine($"{nameof(ExecuteTypeBProcessing)} has been called");
    }
}
```

A list of `IBaseType` has also been initialized, and contains a combination of _TypeA_ and _TypeB_ instances (this is possible, as both types implement _IBaseType_):

``` csharp
List<IBaseType> types = new()
{
  new TypeA(),
  new TypeA(),
  new TypeB(),
  new TypeA(),
  new TypeB(),
  new TypeB(),
  new TypeA(),
};
```

---

## Where with Select

This is the method I've been using prior to knowing there was an alternative. It does work, but is very verbose.

Suppose we want to execute the _ExecuteTypeAProcessing_ method on instances of _ITypeA_:

``` csharp
foreach(var type in types
    .Where(t => t is ITypeA)
    .Select(t => t as ITypeA))
{
    type.ExecuteTypeAProcessing();
}
```

Another method, this time executing _ExecuteTypeBProcessing_ on instances of _ITypeB_:

``` csharp
types.Where(t => t is ITypeB)
    .Select(t => t as ITypeB)
    .ToList()
    .ForEach(t => t.ExecuteTypeBProcessing());
```

---

## OfType

Only recently did I come across the `OfType` method on _IEnumerable_ - not that it was hidden in any way, I just never looked for it, as the above `Where + Select` method worked for my needs. However `OfType` greatly simplifies the above code.

Executing _ExecuteTypeAProcessing_ on instances of _ITypeA_:

``` csharp
foreach(var type in types.OfType<ITypeA>())
{
    type.ExecuteTypeAProcessing();
}
```

Executing _ExecuteTypeBProcessing_ on instances of _ITypeB_:

``` csharp
types.OfType<ITypeB>()
    .ToList()
    .ForEach(t => t.ExecuteTypeBProcessing());
```

Either option is simpler, more concise and more readable than the _Where + Select_ method.

---

## Notes

Nothing groundbreaking learnt today - but never-the-less something very interesting and useful, which will definitely see frequent usage.

---

<?# DailyDrop ?>142: 19-08-2022<?#/ DailyDrop ?>
