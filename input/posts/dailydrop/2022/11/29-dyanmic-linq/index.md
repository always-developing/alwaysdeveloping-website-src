---
title: "Dynamic LINQ with System.Linq.Dynamic.Core"
lead: "How System.Linq.Dynamic.Core can be used to parse strings in LINQ dynamically"
Published: "11/29/2022 01:00:00+0200"
slug: "29-dynamic-linq"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - dynamic

---

## Daily Knowledge Drop

The third-party, _open source_ library `System.Linq.Dynamic.Core` extends LINQ functionality allowing _dynamic, string based LINQ queries_ to be parsed, resulting in operations identical to regular LINQ.

## Examples

In all of the below examples, the following snippet of code is used to get a sample collection of _Song_ entities:

``` csharp
IEnumerable<Song> GetSongs()
{
    return new[]
    {
        new Song("Learn to Fly", "Foo Fighters", 245),
        new Song("Everlong", "Foo Fighters", 312),
        new Song("Bigger than my Body", "John Mayer", 281),
    };
};

public record Song(string Name, string Artist, int LengthInSeconds);
```

The extension methods `System.Linq.Dynamic.Core` offer are available on the _IQueryable_ interface. As such in the examples the _IEnumerable\<Song\>_ returned from the `GetSongs` method is required to be converted to _IQueryable\<Song\>_ using the `AsQueryable` method before the extension methods are available.

Below come common use cases are shown, but the library does offer a lot more functionality - see the references links below for more information on the library.

---

### Select

`System.Linq.Dynamic.Core` can be used to dynamically _Select_ a property value from a collection of entities:

``` csharp
// using traditional LINQ
List<string> namesLinq = songs
  .Select(s => s.Name)
  .ToList();

// dynamically specifying the property to be returned
List<string> nameDynamic = songs
  .AsQueryable()
  .Select("Name")
  .ToDynamicList<string>();
```

A _anonymous or dynamic_ entity can also be _Selected_ out of the collection dynamically:

``` csharp
// traditional
var nameArtistLinq = songs
  .Select(s => new { s.Name, s.Artist })
  .ToList();

// dynamic
List<dynamic> nameArtistDynamic = songs
  .AsQueryable()
  .Select("new { Name, Artist}")
  .ToDynamicList();
```

This is very powerful, allowing the columns to be selected to be determined _at runtime_.

---

### Filtering

The library also provides the ability to dynamically filter a collection of records:

``` csharp
// traditional
List<Song> filterLinq = songs
  .Where(x => x.Artist == "Foo Fighters")
  .ToList();

// dynamic
string column = "Artist";
string value = "Foo Fighters";
List<Song> filterDynamic = songs
  .AsQueryable()
  .Where($"{column} == \"{value}\"")
  .ToList();
```

Again, very powerful as it allows the filter criteria to be be generated _at runtime_.

Outputting the result of the filtering for each technique yields the same result:

``` csharp
Console.WriteLine(filterLinq.Count());
Console.WriteLine(filterDynamic.Count());

// ouput:
// 2
// 2
```

---

### Ordering

The library also offers the ability to dynamically order a collection:

``` csharp
// traditional LINQ ordering
List<Song> orderLinq = songs
  .OrderBy(s => s.LengthInSeconds)
  .ToList();
  
List<Song> orderDescLinq = songs
  .OrderByDescending(s => s.LengthInSeconds)
  .ToList();

// dynamic ordering
List<Song> orderDynamic = songs
  .AsQueryable()
  .OrderBy("LengthInSeconds")
  .ToList();

List<Song> orderDescDynamic = songs
  .AsQueryable()
  .OrderBy("LengthInSeconds desc")
  .ToList();
```

The functionality the library offers is very easy and intuitive to use, as one can see from the above examples.

---

## Notes

This is a very powerful library, and more functionality than described here. If dynamic LINQ is not required, I do not recommend replacing traditional strongly-typed LINQ with this as a based, however in cases where flexibility to change the LINQ at runtime is required, this library is incredibly powerful.

If this is of interest it is definitely recommended to check out the _Dynamic LInq_ reference link below for more information.

---


## References

[Using Dynamic LINQ With System.Linq.Dynamic.Core Library](https://code-maze.com/using-dynamic-linq/)  
[Dynamic LINQ](https://dynamic-linq.net)

<?# DailyDrop ?>212: 29-11-2022<?#/ DailyDrop ?>
