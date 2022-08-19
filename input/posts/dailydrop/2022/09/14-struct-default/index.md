---
title: "Auto-default struct propety values"
lead: "Struct properties auto defaulting coming in C# 11"
Published: "09/14/2022 01:00:00+0200"
slug: "14-struct-default"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - C#11
   - struct
   - defaults

---

## Daily Knowledge Drop

Coming with C# 11 (being released later this year, coinciding with the .NET 7 release) the compiler will now ensure that `fields on a struct will be initialized to their default value` if not explicitly set.

With the current, and prior C# versions, all fields on a struct need to explicitly be set when a struct instance is initialized using a defined constructor.

---

## C# 10 and prior

Consider a _Song_ struct, which contains a number of properties related to a Song:

``` csharp
public struct Song
{
    public int Id { get; init; }

    public string Name { get; init; }

    // this property is required to have
    // a default value
    public string Artist { get; init; } = "Unavailable";

    // this property is required to have
    // a default value
    public int SongLength { get; init; } = 0;

    // As this constructor doesn't set all properties
    // those unset properties need an explicit default value
    public Song(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Song(int id, string name, string artist, int length)
    {
        Id = id;
        Name = name;
        Artist = artist;
        SongLength = length;
    }

    public override string ToString() => $"{Id}: Song with name '{Name}' " +
        $"by '{Artist}' is {SongLength} seconds long";
}
```

In the above, the _Artist_ and _SongLength_ properties need to `explicitly have a default value set`, as there is a constructor which does not set them. 

If the default values are removed:

``` csharp
public string Artist { get; init; } 

public int SongLength { get; init; }
```

then a compiler error will occur:

``` terminal
Auto-implemented property 'Song.Artist' must be fully assigned before 
    control is returned to the caller. Consider updating to language 
    version '11.0' to auto-default the property.	
Auto-implemented property 'Song.SongLength' must be fully assigned 
    before control is returned to the caller. Consider updating to language 
    version '11.0' to auto-default the property.	
```

The error even has a suggestion on how to resolve the issue (assuming you have C# 11 preview version installed).

For completeness, here is an example of using the above struct:

``` csharp
var song = new Song(1, "Everlong", "Foo Fighters", 469);
Console.WriteLine(song);

var halfSong = new Song(1, "Everlong");
Console.WriteLine(halfSong);

var defaultSong = default(Song);
Console.WriteLine(defaultSong);

var blankSong = new Song();
Console.WriteLine(blankSong);
```

with the output:

```terminal
1: Song with name 'Everlong' by 'Foo Fighters' is 469 seconds long
1: Song with name 'Everlong' by 'Unavailable' is 0 seconds long
0: Song with name '' by '' is 0 seconds long
0: Song with name '' by '' is 0 seconds long
```

From the output, one can see that when calling one of the `defined constructors`, all the properties need to be `explicitly set` - either in the constructor itself, or with a default value. However when using one of the `other techniques to instantiate`, the property values `will be set to the type's default value`.

---

## C# 11

When using C# 11 (currently this means having a preview version of .NET7 installer, and updating the csproj file to contain `<LangVersion>preview</LangVersion>`), the struct can be updated to be as follows:

``` csharp
public struct Song
{
    public int Id { get; init; }

    public string Name { get; init; }

    // now these properties don't require
    // a default value
    public string Artist { get; init; }

    public int SongLength { get; init; }

    public Song(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Song(int id, string name, string artist, int length)
    {
        Id = id;
        Name = name;
        Artist = artist;
        SongLength = length;
    }

    public override string ToString() => $"{Id}: Song with name '{Name}' " +
        $"by '{Artist}' is {SongLength} seconds long";
}
```

Now, when instantiated, `not all properties of the struct need to be explicitly set`. 

Executing the same sample code as above:

``` csharp
var song = new Song(1, "Everlong", "Foo Fighters", 469);
Console.WriteLine(song);

var halfSong = new Song(1, "Everlong");
Console.WriteLine(halfSong);

var defaultSong = default(Song);
Console.WriteLine(defaultSong);

var blankSong = new Song();
Console.WriteLine(blankSong);
```

with the output:

```terminal
1: Song with name 'Everlong' by 'Foo Fighters' is 469 seconds long
1: Song with name 'Everlong' by '' is 0 seconds long
0: Song with name '' by '' is 0 seconds long
0: Song with name '' by '' is 0 seconds long
```

When calling a defined constructor which doesn't set all properties, `the unset properties will be set to the type's default value`.

---

## Notes

A minor update to the language, but which will result in a more familiar, intuitive and expected experience for the experienced C# developer, which have worked with previous versions of the the runtime, and are familiar with the behavior of classes.

---

## References

[Auto-default struct](https://dev.to/dotnetsafer/5-new-c-11-features-you-might-have-missed-32d8)   

<?# DailyDrop ?>160: 14-09-2022<?#/ DailyDrop ?>
