---
title: "Exploring the new required properties"
lead: "Learning about the new required keyword potentially coming with C#11"
Published: "07/22/2022 01:00:00+0200"
slug: "22-required-property"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - required
    - property
    - C#11

---

## Daily Knowledge Drop

As part of C#11 release later this year (2022), is a potential new keyword, `required` (potential as it's still currently in preview and is not guaranteed to be included in the final release).

The `required` keyword is applied to a _property_ and will force the property value to be set in the _object initializer_. The compiler will generate an error and prevent compilation if the property is not set.

---

## Current

In our example we have a _Song_ class, which has three fields. We require that these three fields always be set, and cannot be updated:

``` csharp
public class Song
{
    public string Name { get; init; }

    public string Artist { get; init; }

    public int LengthInSeconds { get; init; }
}
```

The `init` keyword is used to specify that the values of the properties can only be set on _initialization_ of the class, and after that they are read only. However, just doing the above `does not enforce the values be set`.

The following is completely valid:

``` csharp
var song = new Song
{
    Artist = "Foo Fighters",
    LengthInSeconds = 250
};

// Name will be NULL here.
Console.WriteLine(song.Name);
```

The only way with the current versions of C#, is to use a `constructor` to enforce the property values be set:

``` csharp
public class Song
{
    public string Name { get; init; }

    public string Artist { get; init; }

    public int LengthInSeconds { get; init; }

    // new constructor taking all three required arguments
    public Song(string name, string artist, int lengthInSeconds)
    {
        Name = name;
        Artist = artist;
        LengthInSeconds = lengthInSeconds;
    }
}
```

Using the _object initializer syntax_ (the syntax in the previous initialization of _Song_) is now invalid - the compiler error:

``` terminal
There is no argument given that corresponds to the required formal parameter 'name' of 'Song.Song(string, string, int)'	
```

The `constructor` now needs to be used to initialize an instance of _Song_:

``` csharp
var song = new Song("Foo Fighters", "Everlong", 250);
Console.WriteLine(song.Name);
```

So we've fulfilled our requirement - the three fields always be set, and cannot be updated? Yes! ...but....

Imagine you have a class with `20 require properties`, in conjunction with `10 optional properties` - you'd end up with at least one constructor with at least 20 parameters. In addition, the 10 optional properties can either be set in a constructor resulting in a 30 parameter constructor, or using the _object initializer syntax_, which forces the use of two different methods to set values when initializing a class.

In the below, the optional _YearReleased_ `init` only property has been added (but not added to the constructor):

``` csharp
var song = new Song("Foo Fighters", "Everlong", 250)
{
    YearReleased = 1997
};
```

All of this will _function_, but it is not very intuitive or developer friendly - enter the `required` keyword.

---

## Required keyword

The `required` keyword is applied to the property:

``` csharp
public class Song
{
    public required string Name { get; init; }

    public required string Artist { get; init; }

    public required int LengthInSeconds { get; init; }
}
```

And enforces that the property is set on object initialization.

The following does **NOT** compile:

``` csharp
var song = new Song
{
    Artist = "Foo Fighters",
    LengthInSeconds = 250
};
```

Generating the compiler error:

``` terminal
Required member 'Song.Name' must be set in the object initializer or attribute constructor.
```

Now there is `no need for the constructor` as the compiler will enforce that `required` properties are set:

``` csharp
var song = new Song
{
    Artist = "Foo Fighters",
    Name = "Everlong",
    LengthInSeconds = 250
};
```

This results in cleaner code, as there are no constructors with numerous arguments, and there is a single standard way in which required and optional properties can be initialized.

---

## Notes

I'm looking forward to this feature, and hoping that it is included in C#11. It will be interesting to explore how this can replace or mostly likely, work with the `Required` attribute data annotation to perform the necessary checks at compile time, and not only at runtime.

## References

[Nick Chapsas - Why I won’t need constructors anymore in C# 11](https://www.youtube.com/watch?v=9CDgPgWF9IY)   

<?# DailyDrop ?>122: 22-07-2022<?#/ DailyDrop ?>
