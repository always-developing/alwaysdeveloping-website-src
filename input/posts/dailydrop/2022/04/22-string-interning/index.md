---
title: "String interning in C#"
lead: "How interning reuses memory for strings of same value"
Published: 04/22/2022
slug: "22-string-interning"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - string
    - interning

---

## Daily Knowledge Drop

`String interning` is a process whereby different string variables of the same value, can both point to the same memory location (to improve memory usage). This is done automatically in most cases, but it is also possible to force this process when not automatically done.

Below we'll look at some cases when interning is done automatically, and how to force it when not automatically done.

---

## Automatic interning

First let's look at how string interning happens automatically.

When working with two string literals:

``` csharp
var helloWorld = "hello world";
var helloWorld2 = "hello world";

Console.WriteLine(Object.ReferenceEquals(helloWorld, helloWorld2));
```

In this example, two strings are declared, both having the same value - a check is then performed compare the references (memory) of the two variables.

The output:

``` powershell
    True
```

The same occurs when appending string literals:

``` csharp
var helloWorld = "hello world";
var helloWorld3 = "hello " + "world";

Console.WriteLine(Object.ReferenceEquals(helloWorld, helloWorld3));
```

The output again indicates they point to the same memory:

``` powershell
    True
```


Now lets look at an example where two variables are set, and then combined: 

``` csharp
var helloWorld = "hello world";
var hello = "hello";
var world = "world";
var helloWorld4 = hello + " " + world;

Console.WriteLine(Object.ReferenceEquals(helloWorld, helloWorld4));
```

The output now changes: even though _helloWorld_ and _helloWorld4_ have the same string value, they no longer point to the same memory address.

``` powershell
    False
```


---

## Manual interning

The interning process can manually be triggered when its unable to automatically be determined, using the `String.Intern` method.

The `Intern` method will use an internal intern pool to search for a string equal to the specified value - if such a string exists, that referent to the intern pool is returned, otherwise a reference to the specified string is added to the intern pool, and then returned.

``` csharp
var helloWorld = "hello world";
var hello = "hello";
var world = "world";
var helloWorld5 = string.Intern(hello + " " + world);

Console.WriteLine(Object.ReferenceEquals(helloWorld, helloWorld45);
```

This is the same as the previous example, except now the `String.Intern` method is used. The output now changes to:

``` powershell
    True
```


As mentioned above, if the string specified to the `Intern` method doesn't already exists, a new reference will be returned, and no error or exceptions will occur:

``` csharp
var helloWorld = "hello world";
var hello = "hello";
var world = "world";
var helloWorld6 = string.Intern(hello + "12345" + world);

Console.WriteLine(Object.ReferenceEquals(helloWorld, helloWorld6));
```

_helloWorld_ and _helloWorld6_ definitely have two very different values now, and the memory location of each reflects that:

``` powershell
    False
```

---

## Notes

`Interning` is an internal implementation and generally not something which should be worried about. Manually using interning however can be used to improve performance in some very niche cases where micro-optimization is required.

---

## References

[String.Intern(String) Method](https://docs.microsoft.com/en-us/dotnet/api/system.string.intern?view=net-6.0)  

<?# DailyDrop ?>58: 22-04-2022<?#/ DailyDrop ?>
