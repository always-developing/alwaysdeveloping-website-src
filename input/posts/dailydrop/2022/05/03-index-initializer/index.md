---
title: "Index initializer behavior"
lead: "How different collection initialization styles behavior differs"
Published: 05/03/2022
slug: "03-index-initializer"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - collection
    - dictionary
    - index
    - initialization

---

## Daily Knowledge Drop

The `index initialization` style of collection initialization has slightly different (but important) behavior to that of the "traditional" initialization method.

---

## Initialization

Let's look at a few different ways a `Dictionary<int, string>` can be initialized, and have 5 values set:

``` csharp
// initialize a empty dictionary, then add the values
var normalDictionary = new Dictionary<int, string>();
normalDictionary.Add(1, "one");
normalDictionary.Add(2, "two");
normalDictionary.Add(3, "three");
normalDictionary.Add(4, "four");
normalDictionary.Add(5, "five");

// initialize using the "traditional" method of collection initialization
var normalInitDictionary = new Dictionary<int, string>
{
    { 1, "one" },
    { 2, "two" },
    { 3, "three" },
    { 4, "four" },
    { 5, "five" }
};

// initialize using the index initialization
var indexInitDictionary = new Dictionary<int, string>
{
    [1] = "one",
    [2] = "two",
    [3] = "three",
    [4] = "four",
    [5] = "five"
};
```

All 3 of the above example will result in the same outcome - a dictionary with 5 items. 

Apart from the difference in style and ease of reading (which is subjective, but personally I prefer the index initialization style), there is a subtle but very important difference to how the two methods operate internally.

---

## Differences

The two difference ways of initializing the dictionary are lowered completely differently by the compiler:

Traditional initialization: 

``` csharp
// initialize using the "traditional" method of collection initialization
var normalInitDictionary = new Dictionary<int, string>
{
    { 1, "one" },
    { 2, "two" },
    { 3, "three" },
    { 4, "four" },
    { 5, "five" }
};

// THE ABOVE GETS LOWERED TO:

Dictionary<int, string> dictionary = new Dictionary<int, string>();
dictionary.Add(1, "one");
dictionary.Add(2, "two");
dictionary.Add(3, "three");
dictionary.Add(4, "four");
dictionary.Add(5, "five");


```

While using index initialization:

``` csharp
// initialize using the index initialization
var indexInitDictionary = new Dictionary<int, string>
{
    [1] = "one",
    [2] = "two",
    [3] = "three",
    [4] = "four",
    [5] = "five"
};

// THE ABOVE GETS LOWERED TO:

Dictionary<int, string> dictionary = new Dictionary<int, string>();
dictionary[1] = "one";
dictionary[2] = "two";
dictionary[3] = "three";
dictionary[4] = "four";
dictionary[5] = "five";
```

---

## Why the differences matter

The way in which the code gets lowered makes a difference, as it results in `different behavior when having duplicate keys`:

Traditional initialization: 

``` csharp
// initialize using the "traditional" method of collection initialization
var normalInitDictionary = new Dictionary<int, string>
{
    { 1, "one" },
    { 1, "two" }
};

// THE ABOVE GETS LOWERED TO:

// This will result in an exception being thrown
Dictionary<int, string> dictionary = new Dictionary<int, string>();
dictionary.Add(1, "one");
dictionary.Add(1, "two");
```

Initialization on a _Dictionary_ with duplicate keys will result in an exception as the code gets lowered to having two _Add_ method with the same key. This is not allowed.

With index initialization:

``` csharp
// initialize using the index initialization
var indexInitDictionary = new Dictionary<int, string>
{
    [1] = "one",
    [1] = "two",
};

// The above gets lower to:

Dictionary<int, string> dictionary = new Dictionary<int, string>();
dictionary[1] = "one";
dictionary[1] = "two";
```

Duplicate keys with index initialization will NOT result in an exception as the code gets lowered to use the indexers, which will just result in the existing value ("one" in this case) being overwritten.

---

## Notes

Subtle difference in how the two initialization methods operate, but can come with consequences if used without being aware of the differences.  

Ignoring the operational differences, then it comes down to preference of style and ease of readability - performance difference between the two is negligible (less than 1% difference when tested)

---

## References

[7 New Cool Features in C# 6.0](https://www.automatetheplanet.com/7-new-cool-features-csharp-6-0/)  

<?# DailyDrop ?>65: 03-05-2022<?#/ DailyDrop ?>
