---
title: "LINQ DistinctBy"
lead: "Using DistinctBy to get a unique collection of items"
Published: "01/06/2023 01:00:00+0200"
slug: "06-linq-distinct"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - distinctby

---

## Daily Knowledge Drop

The _Enumerable_ `DistinctBy` method, introduced in .NET 6, can be used to return _distinct elements from a sequence according to the specified key selector_. 

Prior to .NET 6, performing this operation was a bit complex, but the introduction of the `DistinctBy` method simplifies the process.

---

## Setup

In this example, a collection of 3 _Songs_ will be used:

``` csharp
List<Song> songs = new List<Song>
{
    new Song
    {
        Id = 1,
        Name = "Everlong",
        Album = "The Colour and the Shape"
    },
    new Song
    {
        Id = 2,
        Name = "Monkey Wrench",
        Album = "The Colour and the Shape"
    }
    ,
    new Song
    {
        Id = 3,
        Name = "Learn to Fly",
        Album = "There Is Nothing Left to Lose"
    }
};
```

Let's look at how we can get a _distinct list of albums from the collection of songs_.

---

## Pre .NET 6

The process for doing this (using LINQ) prior to .NET 6 was as follows:

``` csharp
IEnumerable<string> albums = songs
    .GroupBy(s => s.Album)
    .Select(g => g.First())
    .Select(a => a.Album);
```

The steps are as follows:
- Use **GroupBy** to get a collection of _Songs_, with the _Album_ value as the key
- Use **Select** and **First** to get the first record in each group
- Use **Select** to get the _Album_ of the first record

As the _Songs_ in each group will all contain the same _Album_ (as we are grouping by Album), selecting any record will yield the same Album.

This works, but is a fairly convoluted process.

---

## .NET 6 and beyond

The `DistinctBy` method greatly simplifies this:

``` csharp
IEnumerable<string> albums = songs
    .DistinctBy(s => s.Album)
    .Select(a => a.Album);
```

Here, the steps are as follows:
- Use **DistinctBy** to get a collection of _Songs_, each one having a _unique Album value_. In this example, after this is executed, the record with _id = 2_ is dropped from the collection as it has the same _Album_ value as thew previous record
- Use **Select** to get the _Album_ of each of the remaining records

Easier to read and less convoluted!

---

## Notes

The addition of the new `DistinctBy` makes a big difference to the complexity, as well as the readability of the code - instead of trying to decider some _complex_ LINQ (as was the case prior to .NET 6), the code now better describes what it is actually doing.

---


## References

[Do you know about the DistinctBy method?](https://www.reddit.com/r/csharp/comments/zpjpst/do_you_know_about_the_distinctby_method/)  

<?# DailyDrop ?>229: 06-01-2023<?#/ DailyDrop ?>
