---
title: "Tuples for class value equality"
lead: "Using a tuple to check the equality of two class instances"
Published: "07/18/2022 01:00:00+0200"
slug: "18-tuple-equality"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - equality
    - equals
    - tuple

---

## Daily Knowledge Drop

A `tuple` can be used to check the `value equality` of two instances of the same class. 

When comparing classes, the equality operator (`==`) will check that the two instances are actually the same instance, not that the values of the two instances are the same. There are a [number of different techniques](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type) to define value equality for a class, but they all ultimately have the same underlying logic - each property of the instances being compared need to individually be compared. 

The usage of `tuples` can be used to simplify this comparison.

---

## Setup

In this post we will be using a `Song` class, which has three properties:

``` csharp
public class Song
{
    public string Name { get; init; }

    public string Artist { get; init; }

    public int LengthInSeconds { get; init; }
}
```

For two _Song_ instances to be equal, each of the three properties for the two instances need to be equal.

---

## Equality

### Equality operator

As mentioned above, using the default equality operator for classes will test if the two instances are actually the same instance, not check the values of the two instances:

``` csharp

// two Song instances, both with the same values
var song = new Song
{
    Artist = "Foo Fighters",
    Name = "Everlong",
    LengthInSeconds = 250
};

var song1 = new Song
{
    Artist = "Foo Fighters",
    Name = "Everlong",
    LengthInSeconds = 250
};

// a 3rd instance, set to the first instance
var song2 = song;

// comparing two different instances
Console.WriteLine(song == song1);
// comparing two instances, which are the "same" instance
Console.WriteLine(song == song2);

// comparing two different instances
Console.WriteLine(song.Equals(song1));
// comparing two instances, which are the "same" instance
Console.WriteLine(song.Equals(song2));

```

The output of the above is:

``` terminal
False
True
False
True
```

---

### Manual comparison

To compare a value comparison of the two classes (instead of using one of [these techniques](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type)) we are going to write a method to do the comparison:

``` csharp
bool Compare(Song s1, Song s2) =>
    s1.Artist == s2.Artist && // compare artist
    s1.Name == s2.Name &&  // and Name
    s1.LengthInSeconds == s2.LengthInSeconds; // and song Length
```

Only if all three properties are equal, are the two instances equal:

``` csharp
Console.WriteLine(Compare(song, song1));
Console.WriteLine(Compare(song, song2));
Console.WriteLine(Compare(song1, song2));
```

The output being:

``` terminal
True
True
True
```

The above will function correctly, adequately doing a value comparison of the two instance - however a simpler and cleaner technique is to use a `tuple` for the value comparison.

---

### Tuple equality

One feature of the `Tuple` is that using the equality operator on it **does** compare the values:

``` csharp
var tuple = (1, 2, 3);
var tuple1 = (1, 2, 3);

Console.WriteLine(tuple == tuple1);
Console.WriteLine(tuple.Equals(tuple1));
```

Output: 

``` terminal
True
True
```

This fact can be leverage to compare two class instances by `creating tuples with the class values, and then comparing the tuples!`

---

### Tuple comparison

If we rework the _Compare_ method defined above, to use a `Tuple` instead:

``` csharp
// Use the properties of each instance to create a tuple
// and compare the two tuples
bool CompareTuple(Song s1, Song s2) =>
    (s1.Artist, s1.Name, s1.LengthInSeconds) == (s2.Artist, s2.Name, s2.LengthInSeconds);
```

Executing the same comparisons as above using the new method:

``` csharp
Console.WriteLine(CompareTuple(song, song1));
Console.WriteLine(CompareTuple(song, song2));
Console.WriteLine(CompareTuple(song1, song1));
```

Yields the following output:

``` terminal
True
True
True
```

---

## Notes

While the results of the two methods are the same, the tuple version is more _concise, cleaner and less prone to developer error_, especially if there are a large number of fields being compare.

The tuple method can also be used in conjunction with the [techniques already mentioned](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type) - for example by overriding the _equals operator_ (==) and performing the tuples equality check in that method.

---

## References

[Change your habits: Modern techniques for modern C# - Bill Wagner](https://www.youtube.com/watch?v=aUbXGs7YTGo&t=569s)   

---

<?# DailyDrop ?>118: 18-07-2022<?#/ DailyDrop ?>
