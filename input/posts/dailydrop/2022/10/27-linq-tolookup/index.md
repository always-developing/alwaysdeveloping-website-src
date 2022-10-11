---
title: "LINQ's ToLookup method"
lead: "Discovering LINQ has a ToLookup method for grouping collection items"
Published: "10/27/2022 01:00:00+0200"
slug: "27-linq-tolookup"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - linq
   - lookup
   - tolookup

---

## Daily Knowledge Drop

LINQ has a `ToLookup` method which which is used to create a _lookup_ - `a key which points to a list of (1 or more) objects`.

---

## Example

Suppose we have a collections of _Songs_:

``` csharp
// Song record
record Song(string Artist, string Name, int LengthInSeconds);

// an array of songs
Song[]? songs = new[]
{
    new Song("Foo Fighters", "EverLong", 250),
    new Song("Foo Fighters", "My Hero", 283),
    new Song("Foo Fighters", "All My Life", 250),
    new Song("John Mayer", "Clarity", 238),
    new Song("John Mayer", "Daughters", 238),
};

```

### Simple lookup

The `ToLookup` can be used to `group songs together by artist`:

``` csharp
// perform the lookup, using Artist as the key
ILookup<string, Song>? lookupByArtist = 
    songs.ToLookup(s => s.Artist, item => item);

// iterate through each key (artist) grouping
foreach(IGrouping<string, Song> artist in lookupByArtist)
{
    Console.WriteLine($"Artist: {artist.Key}");
    // iterate through each item in the grouping
    foreach(Song song in artist.ToList())
    {
        Console.WriteLine($"- Song name: {song.Name}");
    }
}
```

The output of the above is as follows:

``` terminal
Artist: Foo Fighters
- Song name: EverLong
- Song name: My Hero
- Song name: All My Life
Artist: John Mayer
- Song name: Clarity
- Song name: Daughters
```

---

### Complex lookup

In the above example, the _lookup was done on a single field, Artist_, however this doesn't have to be the case. The `grouping can be done on multiple fields`:

``` csharp
// group the data by artist AND song length
var lookupByArtistTime = 
    songs.ToLookup(s => new { s.Artist, s.LengthInSeconds}, item => item);

foreach (var artistTime in lookupByArtistTime)
{
    Console.WriteLine($"Artist/Time: {artistTime.Key}");
    foreach (Song song in artistTime.ToList())
    {
        Console.WriteLine($"- Song name: {song.Name}");
    }
}
```

In the above, the key was the `Artist + LengthInSeconds anonymous object`. The output:

``` terminal
Artist/Time: { Artist = Foo Fighters, LengthInSeconds = 250 }
- Song name: EverLong
- Song name: All My Life
Artist/Time: { Artist = Foo Fighters, LengthInSeconds = 283 }
- Song name: My Hero
Artist/Time: { Artist = John Mayer, LengthInSeconds = 238 }
- Song name: Clarity
- Song name: Daughters
```

---

## ToLookup vs GroupBy

A note on `ToLookup`, which on the surface appears to be the same as `GroupBy`:

- `ToLookup` performs execution immediately and returns an _ILookup_ implementation
- `GroupBy` defers execution until a _ToList_ (or similar) is called on the returned _IEnumerable_

So while the final result of the operations are the same, the timing of the execution is different.

---

## Notes

This is not new functionality and has been part of LINQ from the beginning - having said that, I personally wasn't aware that the method existed, and under the right use cases, can be used in place of _GroupBy_.

---

## References

[.NET: Learn LINQ as you never have before - page 27](https://anthonygiretti.com/2022/09/29/net-learn-linq-as-you-never-have-before/)  

<?# DailyDrop ?>189: 27-10-2022<?#/ DailyDrop ?>
