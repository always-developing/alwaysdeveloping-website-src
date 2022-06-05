---
title: "Indexers with multiple arguments"
lead: "Writing custom indexers which accept multiple arguments"
Published: "06/14/2022 01:00:00+0200"
slug: "14-indexer-arguments"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - indexer

---

## Daily Knowledge Drop

Previously we have look at a method to [add an indexer and access class as an array](../../02/23-indexers/). Today we explore indexers again, and how custom indexers can be written which accept not only integers, but other types as well as `multiple parameters`, to access data in a variety of ways.

---

## List example

First as a benchmark, we'll have a look at the `List` class:

``` csharp
var strList = new List<string>();

strList.Add("one");
strList.Add("two");
strList.Add("three");
strList.Add("four");
strList.Add("five");

Console.WriteLine(strList[3]);
```

The items added to the List can be accessed using an int indexer. In the above example _strList[3]_ will return the 4th item in the list. This is standard built-in functionality.

---

## EnhancedList example

Next we'll create our own `EnhancedList`, which inherits from the `List` class, but provides additional functionality through custom indexers.

The base `EnhancedList` looks as follows and operates exactly the same as a normal `List`:

``` csharp
    public class EnhancedList<T> : List<T> { }
```

---

### Access index

First, let's create an indexer to `get the index, based on the value`. This is basically exactly what the `IndexOf` method does, but as an indexer:

``` csharp
public class EnhancedList<T> : List<T>
{
    public int this[T value] => this.Contains(value) ? this.IndexOf(value) : -1;
}
```

The method checks if the list contains the value passed in, and if it does will return the value's index, otherwise -1 will be returned.

The usage is now as follows:

``` csharp
var enhancedList = new EnhancedList<string>();
enhancedList.Add("one");
enhancedList.Add("two");
enhancedList.Add("three");
enhancedList.Add("four");
enhancedList.Add("five");

// access the value based on the index
Console.WriteLine(enhancedList[3]);

// access the index based on the value
Console.WriteLine(enhancedList["two"]);

```

The output of the above is:

``` powershell
    four
    1
```

The built in indexer for `List` accepts an int as a parameter, and we've created an indexer which accepts type T (the type contained in the _EnhancedList_), in this example, a string.

---

### Multiple index lookup

As we've seen, `EnhancedList[index]` can be used to get the value at the specified index. Let's update the `EnhancedList` to accept `multiple indexes and return multiple values`:

``` csharp
public class EnhancedList<T> : List<T>
{
    public int this[T value] => this.Contains(value) ? this.IndexOf(value) : -1;

    public IEnumerable<T> this[bool rangeLookup, params int[] indexes] => 
        indexes.Select(i => (T)this[i]);
}
```

A new indexer has been added, this time taking a bool and an array of integers as arguments. The bool parameter is required to differentiate between _EnhancedList[index]_ and _EnhancedList[params]_ - without the bool forcing a difference, there is no way of specifying which indexer is being called.

The usage is now as follows:

``` csharp
var enhancedList = new EnhancedList<string>();
enhancedList.Add("one");
enhancedList.Add("two");
enhancedList.Add("three");
enhancedList.Add("four");
enhancedList.Add("five");

// get the value for index 2 and 4
foreach (var lookupItem in enhancedList[true, 2, 4])
{
    Console.WriteLine(lookupItem);
}

```

In the above, we get the values at index 2 and 4. The output being:

``` powershell
    three
    five
```

---

### T modification

The generic type T contained in the `EnhancedList` can also be modified before being returned by the indexer. In the last example, we are going to create an indexer which returned the items in the list as a string, ready for output to the Console:

``` csharp
public class EnhancedList<T> : List<T>
{
    public int this[T value] => this.Contains(value) ? this.IndexOf(value) : -1;

    public IEnumerable<T> this[bool rangeLookup, params int[] indexes] => 
        indexes.Select(i => (T)this[i]);

    public IEnumerable<string> this[string prefixMessage, params int[] indexes] => 
        indexes.Select(i => $"{prefixMessage} {(T)this[i]}");
}
```

The new indexer takes a string prefix message, and an array of indexes. Instead of returning just the values at the position of the indexes (as in the previous example), now the _prefixMessage_ and the _value_ are combined before being returned.

The usage is now as follows:

``` csharp
var enhancedList = new EnhancedList<string>();
enhancedList.Add("one");
enhancedList.Add("two");
enhancedList.Add("three");
enhancedList.Add("four");
enhancedList.Add("five");

// return value at index 0, 2 and 4
// with the supplied message
foreach (var lookupItem in enhancedList["Printing item ...", 0, 2, 4])
{
    Console.WriteLine(lookupItem);
}

```

In the above, we get the values at index 2 and 4. The output being:

``` powershell
    Printing item ... one
    Printing item ... three
    Printing item ... five
```

---

## Notes

While the examples shown above are not necessarily production ready or practical, they do show how indexers can be created which accept multiple arguments, allowing for some innovative possibilities depending on your specific use case.

---

## References

[Maarten Balliauw Tweet](https://twitter.com/maartenballiauw/status/1527280915092742144)  

<?# DailyDrop ?>95: 14-06-2022<?#/ DailyDrop ?>
