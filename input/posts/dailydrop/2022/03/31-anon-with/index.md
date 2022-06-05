---
title: "Anonymous types and with keyword"
lead: "Using the with keyword to support non-destructive mutations on anonymous types"
Published: 03/31/2022
slug: "31-anon-with"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - anonymous
    - with

---

## Daily Knowledge Drop

The `with` keyword can be used to create a new instance of an anonymous type where one of more properties have new values.

---

## Anonymous types

First off, a brief explanation of `anonymous types`. Anonymous types are a way to encapsulate a set of _read only_ properties into a single object without explicitly defining a type. The type name is generated internally by the compiler and the type of each property is inferred by the compiler.

Here we define an anonymous type (as you can see, no type is specified) with the instance name _song_, and a number of properties:

``` csharp
var song = new
{
    Name = "Everlong",
    Artist = "Foo Fighters",
    Released = 1997
};

Console.WriteLine(song.Name);
Console.WriteLine(song);
```

The _song_ instance can be treated and operated on as a usual type instance, with each property accessible (as read-only):

``` powershell
Everlong
{ Name = Everlong, Artist = Foo Fighters, Released = 1997 }
```

---

## Using with

As mentioned above, the `properties on an anonymous class are read-only`.

This is **NOT permitted**:

``` csharp
var song = new
{
    Name = "Everlong",
    Artist = "Foo Fighters",
    Released = 1997
};

// Compiler will not allow this!
song.Name = "Monkey Wrench";
```


This is where the `with` keyword becomes useful - `with expressions` allow for non-destructive mutation of an anonymous type!

Suppose we wanted to instantiate an anonymous type for each song on the album - most of the information (Artist and Released) would be the same across each instance. The `with` keyword can be leveraged to create a `new instance of the anonymous type, based on an existing instance, but with (some) new values`.

``` csharp
var song = new
{
    Name = "Everlong",
    Artist = "Foo Fighters",
    Released = 1997
};

var song2 = song with { Name = "Monkey Wrench" };
var song3 = song with { Name = "My Hero" };
var song4 = song with { Name = "Walking After You" };
```

The output:

``` powershell
    { Name = Everlong, Artist = Foo Fighters, Released = 1997 }
    { Name = Monkey Wrench, Artist = Foo Fighters, Released = 1997 }
    { Name = My Hero, Artist = Foo Fighters, Released = 1997 }
    { Name = Walking After You, Artist = Foo Fighters, Released = 1997 }
```

---

## Notes

When dealing with anonymous types, which share the same data across instances, this technique of instantiating using `with` and an existing instance can save time as well as keep code cleaner.

---

## References

[Anonymous Types](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types)  

<?# DailyDrop ?>42: 31-03-2022<?#/ DailyDrop ?>
