---
title: "Succinct initialization pattern"
lead: "Succinct lazy initialization patterns with newer C# features"
Published: "11/18/2022 01:00:00+0200"
slug: "18-succinct-init"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - init
   - lazy
   - initialization

---

## Daily Knowledge Drop

Sometimes objects need to be initialized _lazily_ - two newer C# features, the _null-coalescing assignment operator_ `??=` and the _target-typed new expression_ makes the lazy initialization cleaner and more succinct.

---

## Example

In the examples, we have a `Album` class which contains an _optional_ list of `Song` instances:

``` csharp
public class Album
{
    private readonly string _name;

    private readonly List<Song> _songs;

    public Album(string name, List<Song> songs = null)
    {
        _name = name;
        _songs = songs;

        // See examples below on how to 
        // do the _songs initialization
    }
}

public class Song
{
    private readonly string _name;

    public Song(string name)
    {
        _name = name;
    }
}
```

If no `Song` list is passed into the constructor, then the `_song` variable needs to be initialized to an _empty list_. Doing this will prevent having to have checks throughout the code to determine if the Song list is null or not. There are multiple ways to do this, but this post focuses on the "more traditional" method, and the new "succinct" method.

---

### Traditional lazy init

Traditionally (before any of the newer C# features were introduced), the constructor might look something like this:

``` csharp
public Album(string name, List<Song> songs = null)
{
    _name = name;
    _songs = songs;

    // check if _songs is null
    if(_songs == null)
    {
        // initialize to an empty list
        _songs = new List<Song>();
    }
}
```

The variable is checked to determine if its _null_ or not, and if it is, then explicitly initialized to an empty list. Nothing inherently wrong with this approach - it just takes four lines of code to _lazily initialize a variable_. 

---

### Succinct lazy init

As mentioned, the two newer C# features allow for this code to be more succinct:

``` csharp
public Album(string name, List<Song> songs = null)
{
    _name = name;
    _songs = songs;

    _songs ??= new();
}
```

- The _null-coalescing assignment operator_ `??=` assigns the value of the right-hand side to the left-hand side, only if the left-hand side is null
- The `target-typed new` expression, `new` allows for the inferring of the type from the declaration, instead of having to explicitly specify the type in full

Four lines of code have been reduced to one!

---

## Notes

A very minor change, which is not a necessity or requirement when coding - however is does result in cleaner, more succinct code. Over time, it also does reduce coding time by reducing the number of key strokes required by the developer.

---

## References

[David Fowler Tweet](https://twitter.com/davidfowl/status/1586592671296290816)  

<?# DailyDrop ?>205: 18-11-2022<?#/ DailyDrop ?>
