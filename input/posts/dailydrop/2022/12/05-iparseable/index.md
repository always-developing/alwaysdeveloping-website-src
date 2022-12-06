---
title: "Parse a string to anything"
lead: "Parsing a string to any object type with the IParseable interface"
Published: "12/05/2022 01:00:00+0200"
slug: "05-iparseable"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net7
   - parseable
   - interface

---

## Daily Knowledge Drop

C# 11 introduced the ability to define `static abstract member` (read more about those [here](https://blog.ndepend.com/c-11-static-abstract-members/)) - this allowed for the introduction of the `IParseable<TSelf>`, which allows for a `string to be parsed into any type`.

---

## IParseable interface

A quick look at the interface:

``` csharp
public interface IParsable<TSelf> where TSelf : IParsable<TSelf>?
{
    static abstract TSelf Parse(string s, IFormatProvider? provider);
    static abstract bool TryParse([NotNullWhen(true)] string? s, 
        IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result);
}
```

As one can see, the interface defines two `Parse` methods - both for _parsing a string_ into the `TSelf` type. To leverage this functionality, as with other interfaces, the class which is to be _converted from a string needs to implement IParsable_.

---

## IParseable implementation

In this example we have a simple `Song` class which implements `IParsable<Song>`:

``` csharp
public class Song : IParsable<Song>
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public int LengthInSeconds { get; set; }

    private Song(string name, string artist, int lengthInSeconds)
    {
        Name = name;
        Artist = artist;
        LengthInSeconds = lengthInSeconds;
    }

    public static Song Parse(string s, IFormatProvider? provider)
    {
        // implementation detailed below
    }

    public static bool TryParse([NotNullWhen(true)] string? s, 
        IFormatProvider? provider, [MaybeNullWhen(false)] out Song result)
    {
        // implementation detailed below
    }
}
```

Next, we'll write the methods to do the parsing.

---

### Parse implementation

The simple `Parse` method implementation will perform the conversion from _string to song_:

``` csharp
public static Song Parse(string s, IFormatProvider? provider)
{
    string[] songPortions = s.Split(new[] { '|' });

    // make sure the string is in the correct format
    if (songPortions.Length != 3) 
    { 
        throw new OverflowException("Expect format: Name|Artist|LengthInSeconds"); 
    }

    return new Song(songPortions[0], songPortions[1], Int32.Parse(songPortions[2]));
}
```

The method is straight-forward - the string representation is expected in a specific format, and once it is confirmed all information is present, a new `Song` instance is declared and returned based on this information.

---

### TryParse implementation

The `TryParse` implementation leveraged the `Parse` method to _try_ parse the string, and returns _true_ or _false_ indicating if the parsing was successful or not, as well as a `Song` instance if the string was able to be parsed:

``` csharp
public static bool TryParse([NotNullWhen(true)] string? s, 
    IFormatProvider? provider, [MaybeNullWhen(false)] out Song result)
{
    // do some checks
    result = null;
    if (s == null) 
    { 
        return false; 
    }

    // parse the string 
    try
    {
        result = Parse(s, provider);
        return true;
    }
    catch { return false; }
}
```

---

## Summary

The `IParsable<TSelf>` interface allows for any class to use a generic interface to define how it can be instantiated from a string. Leveraging the interface and functionality is simple and easy to implement (the parsing logic might not always be though).

---

## Notes

This may seem like a relatively small enhancement (and it is) - but it provides a very convenient way to parse string information. The new C# 11 `static abstract member` functionality also opens the door for new techniques and coding possibilities.

---


## References

[The new .NET 7.0 IParsable<TSelf> interface](https://blog.ndepend.com/the-new-net-7-0-iparsable-interface/)  
[C# 11 static abstract members](https://blog.ndepend.com/c-11-static-abstract-members/)

<?# DailyDrop ?>216: 05-12-2022<?#/ DailyDrop ?>
