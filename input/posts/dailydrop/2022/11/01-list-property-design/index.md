---
title: "Generic List property wrapping"
lead: "Putting a generic List property behind a class to created cleaner code"
Published: "11/01/2022 01:00:00+0200"
slug: "01-list-property-design"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - array
   - jagged

---

## Daily Knowledge Drop

When a class has a `generic list property`, a good idea is to put it behind a class to encapsulate all the functionality specific to the list in a single place, creating cleaner, more maintainable code.

---

## Non-clean version

_Non-clean_ is not entirely accurate, as there is nothing inherently wrong with this approach, its just not _as clean_ as the class-wrapped version detailed in the next section.

In this example we have a `Album` class, which contains a `list of Song titles`:

``` csharp
public class Album
{
    public string Artist { get; set; }

    public List<string> Songs { get; set; }

    public int ReleaseYear { get; set; }
}
```

Suppose throughout the code we are required to get the `top 5 songs in the list` (perhaps to display in a summary on various screens within the application).

---

### LINQ everywhere

One option is to perform the LINQ query to get the top 5 songs whenever it is required:

``` csharp
// This LINQ will be used every time the top 5 is required
IEnumerable<string>? top5Songs = album.Songs.OrderBy(s => s).Take(5);
```

This will work, but leads to duplication of code, and is difficult to maintain.

### Helper method

Another option it to add a method into a _helper_ class, or even into the _Album_ class

``` csharp
public class Album
{
    public string Artist { get; set; }

    public List<string> Songs { get; set; }

    public int ReleaseYear { get; set; }

    public IEnumerable<string> GetTop5Songs()
    {
        return Songs.OrderBy(s => s).Take(5);
    }
}
```

Again, this will work and solves the code duplication issue. However the _helper_ class, or _Album_ class might end up containing numerous methods for numerous properties and ends up containing a wide range of functionality not directly related to it.

---

## Clean version

### Class wrapper

Another cleaner option, is to wrap the generic list in its own class, which can then hold all the methods related to the list:

``` csharp
// inherit from List
public class Songs : List<string>
{
    public IEnumerable<string> GetTop5Songs()
    {
        return this.OrderBy(s => s).Take(5);
    }

    // other methods related to Songs
}
```

The wrapper class inherits from `List` so automatically gets all the same functionality which was available when using `List<string>` explicitly.

The _Album_ class will then use the `Songs` class, instead of `List<String>` to represent the songs of an album:

``` csharp
public class Album
{
    public string Artist { get; set; }

    public Songs Songs { get; set; }

    public int ReleaseYear { get; set; }
}
```

The relevent methods are now available on the property themselves:

``` csharp
var top5Songs = album.Songs.GetTop5Songs();
```

Cleaner, more maintainable and easier to find!

---

## Notes

This is a relatively small change in code structure, but can definitely make a difference in the readability and maintainability of the code, bring related methods together in one place. Easier for a developer to find when using them, as well as when required to make changes to them!

---

## References

[Raw Coding-put generics behind classes](https://www.youtube.com/shorts/FL0qKaumcxo)  

<?# DailyDrop ?>192: 01-11-2022<?#/ DailyDrop ?>
