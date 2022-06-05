---
title: "Line breaks in string interpolation"
lead: "Line breaks allowed in interpolation expressions with C#11"
Published: 04/21/2022
slug: "21-interpolation-line-break"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - C#11
    - .net7
    - interpolation
    - linebreak

---

## Daily Knowledge Drop

Coming with C#11 (.NET7), line breaks will now be allowed in the interpolation expressions in interpolated strings.

---

## Prior to C#11

Let's look at a simple example:

``` csharp
var aCollectionOfStringValues = new string[]
{
    "String1",
    "String2",
    "String3"
};

// Before C#11
Console.WriteLine($"The first letter of the 2nd item is {aCollectionOfStringValues[1].ToLower().First()}");
```

The interpolation expression of `aCollectionOfStringValues[1].ToLower().First()` is fairly long however, unlike normal code, it cannot be split across different lines.

---

## C#11

C#11 introduced the ability to do the following:

``` csharp
var aCollectionOfStringValues = new string[]
{
    "String1",
    "String2",
    "String3"
};

// With C#11
Console.WriteLine($"The first letter of the 2nd item is {
    aCollectionOfStringValues[1]
    .ToLower()
    .First()}");
```

The interpolation expression can now contain line breaks and be split across lines, making the code easier to read.

---

## Notes

A very small change being introduced with C#11, but one which will add to the quality of life as a developer, removing to need to excessive horizontal scrolling, and making code easier to read.

---

## References

[C# 11 - New Features in .NET 7](https://www.youtube.com/watch?v=ljwz-YZZZ7g)  

<?# DailyDrop ?>57: 21-04-2022<?#/ DailyDrop ?>
