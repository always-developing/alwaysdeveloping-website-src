---
title: "Deconstructing a class instance into multiple variables"
lead: "Investigating how a class instance can be easily deconstructed into multiple variables"
Published: "07/29/2022 01:00:00+0200"
slug: "29-deconstruct-method"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - deconstruct
    - tuple

---

## Daily Knowledge Drop

One or many `Deconstruct` methods can be added to a class allowing the class properties to be deconstructed into one or more variables (depending on the `Deconstruct` methods available). This helps created cleaner, more concise code.

---

## Non-Deconstruct

Consider a _Song_ class, with three properties:

``` csharp
public class Song
{
    public string SongName { get; init; }

    public string ArtistName { get; init; }

    public int LengthInSeconds { get; init; }
}
```

If there is a requirement to print out all the properties of a _Song_ instance, it could be done like this:

``` csharp
var song1 = new Song
{
    ArtistName = "Foo Fighters",
    SongName = "Everlong",
    LengthInSeconds = 250
};

Console.WriteLine($"The song '{song1.SongName}' by '{song1.ArtistName}' " +
    $"is {song1.LengthInSeconds} seconds long");

```

Nothing especially wrong with this, but if the _Song_ class had more properties, with long names, the interpolated string could get long and unwieldy.

This could be simplified by doing the following:

``` csharp
var song1 = new Song
{
    ArtistName = "Foo Fighters",
    SongName = "Everlong",
    LengthInSeconds = 250
};

var name = song1.SongName;
var artist = song1.ArtistName;
var length = song1.LengthInSeconds;

Console.WriteLine($"The song '{name}' by '{artist}' is {length} seconds long");
```

The interpolated string is definitely more concise now, but thee extra variables have been defined and assigned. This would make sense if the values are being reused numerous times in the code - but if this is only used once-off, a lot of vertical space has been taken up for no real "value".

However, the `Deconstruct` method makes this process even more concise and simpler.

---

## Deconstruct

One or many `Deconstruct` method can be added to the _Song_ class to allow for the deconstruction of the instance into variables. 

A method called `Deconstruct` is defined on the class, with one or more `out` parameters in the method signature:

``` csharp
public class Song
{
    public string SongName { get; init; }

    public string ArtistName { get; init; }

    public int LengthInSeconds { get; init; }

    // deconstruct all three properties
    public void Deconstruct(out string artist, out string name, out int length)
    {
        name = SongName;
        artist = ArtistName;
        length = LengthInSeconds;
    }

    // deconstruct into a string combination of song and artist
    // as well as the length
    public void Deconstruct(out string output, out int length)
    {
        output = $"'{SongName}' by {ArtistName}";
        length = LengthInSeconds;
    }
}
```

The `Deconstruct` methods can now be used as follows:

``` csharp
var song1 = new Song
{
    ArtistName = "Foo Fighters",
    SongName = "Everlong",
    LengthInSeconds = 250
};

// deconstruct song1, into three variables
var (artist, name, length) = song1;
Console.WriteLine($"The song '{name}' by '{artist}' is {length} long");
```

Here, the three variables _artist_, _name_ and _length_ are automatically defined and assigned to the `out` parameter values of the matching corresponding `Deconstruct` method. Definitely cleaner and more concise that previous techniques.

The `Deconstruct` methods also work with the `discard` character. In the above example there is a second `Deconstruct` method which returns the string _output_ and the song _length_. If only interested in the _output_ value, but not the _length_, then the following can be done:

``` csharp
var song1 = new Song
{
    ArtistName = "Foo Fighters",
    SongName = "Everlong",
    LengthInSeconds = 250
};

// deconstruct song1, into only one variable
// discarding the length
var (output, _) = song1;
Console.WriteLine(output);
```

Here no memory is allocated for the _length_ out parameter where the `discard` is used.

---

## Notes

There are not too many practical use cases for the `Deconstruct` method - but where it can be applied (in cases such as the ones described above), it will definitely assist with creating cleaner, more concise code.

---

## References

[Deconstructing tuples and other types](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types)   

---

<?# DailyDrop ?>127: 29-07-2022<?#/ DailyDrop ?>
