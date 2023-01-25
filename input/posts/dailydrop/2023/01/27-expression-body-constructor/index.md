---
title: "Expression-body constructor"
lead: "How to write a constructor body as an expression"
Published: "01/27/2023 01:00:00+0200"
slug: "27-expression-body-constructor"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - tuple
   - constructor
   - expressionbody

---

## Daily Knowledge Drop

An `expression` can be used as the body of a constructor, providing a more concise (but arguably, more complex) style.


---

## Traditional constructor

Traditionally, the constructor for an entity would have a parameter for each property of the entity and set the property value to the parameter value (or to a default):

``` csharp
public class Song
{
    public string Artist { get; set; }

    public string Name { get; set; }

    public int LengthInSeconds { get; set; }

    // set all property values based on parameters
    public Song(string artist, string name, int lengthInSeconds)
    {
        Artist = artist;
        Name = name;
        LengthInSeconds = lengthInSeconds;
    }

    // set property values based on parameters and
    // default values
    public Song(string artist, string name)
    {
        Artist = artist;
        Name = name;
        LengthInSeconds = 0;
    }
}
```

While there is absolutely _nothing wrong with this approach_, it is a fair number of lines of code just to set a few properties. A more concise approach, would be to use an `expression` for the body of the constructor.

--

## Expression-bodied constructor

The constructors can be rewritten as follows, using expressions for the body of the constructor:

``` csharp
public class Song
{
    public string Artist { get; set; }

    public string Name { get; set; }

    public int LengthInSeconds { get; set; }

    // set all property values based on parameters
    public Song(string artist, string name, int lengthInSeconds) =>
        (Artist, Name, LengthInSeconds) = (artist, name, lengthInSeconds);

    // set property values based on parameters and
    // default values
    public Song(string artist, string name) =>
        (Artist, Name, LengthInSeconds) = (artist, name, 0);
}
```

Here, a powerful feature of `Tuples` is leveraged. A _Tuple_ created from the values passed into the constructor (or default values), is assigned to a _Tuple_ created using the properties of the class. This will essentially `assign the parameter values to the property values`.

Definitely a more concise approach, although if unfamiliar with the syntax and how _Tuples_ work, can definitely be a more confusing approach.

---


## Notes

If concise, lean and clean code is the goal, then `expression-body constructors` is the way to go. As mentioned, the syntax can be more confusing when compared with the traditional approach - especially to those unfamiliar with _Tuples_ and their properties. 

---

## References

[Milan Jovanović Tweet](https://twitter.com/mjovanovictech/status/1617071212504440832)  

<?# DailyDrop ?>244: 27-01-2023<?#/ DailyDrop ?>
