---
title: "Enum iteration"
lead: "Using Enum.GetValues to iteration through enum values"
Published: "12/12/2022 01:00:00+0200"
slug: "12-enum-iteration"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - enum
   - iteration

---

## Daily Knowledge Drop

The static `Enum.GetValues` method value be used to get an array of the enum values, which can be output as a _string_ or corresponding compatible _underlying value_

---

## Enum.GetValues

The function and usage of `Enum.GetValues` is very simple - it _converts an enum into an array of the enum values_.

Consider the following enum:

``` csharp
public enum OrderStatus
{
    New = 0,
    Processing = 1,
    Fulfilled = 2,
    OutOnDelivery = 3,
    Delivered = 4
}
```

This can be converted to an array as follows:

``` csharp
OrderStatus[] enumItems = Enum.GetValues<OrderStatus>();
```

This array can now be iterated over, and output either as a _string_ or _compatible underlying type_:

``` csharp
foreach (var item in Enum.GetValues<OrderStatus>())
{
    // output the string value
    Console.WriteLine(item);

    // output the underlying value
    Console.WriteLine((int)item);
    // this would also work
    //Console.WriteLine((double)item);
}
```

The output of the above is:

``` terminal
New
0
Processing
1
Fulfilled
2
OutOnDelivery
3
Delivered
4
```

Simple, easy and very useful.

---

## Notes

A small but useful piece of knowledge for today.  This can be leveraged to automatically populate a UI dropdown with available options, for example, instead of having multiple places with a list of possible values.

---


## References

[How to Enumerate an Enum in C#](https://code-maze.com/enumerate-enum-csharp/)  

<?# DailyDrop ?>220: 12-12-2022<?#/ DailyDrop ?>
