---
title: "Delimit an array with ArraySegment"
lead: "Using ArraySegment to represent only a portion of a larger array"
Published: "08/25/2022 01:00:00+0200"
slug: "25-arraysegment"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - array
   - delimit
   - arraysegment

---

## Daily Knowledge Drop

The `ArraySegment` structure can be used to represent only a portion of a larger array. A few use cases when `ArraySegment` is useful:
- when only a portion of a larger array needs to be passed as an argument to a method, `ArraySegment` can be used to pass only the require portion of elements
- when wanting to perform operations on the array from multiple threads, `ArraySegment` can be used to have each thread operation on a portion of the array

An important aspect of the `ArraySegment` is that a `new array is not created` - the `ArraySegment` structure still points back to the original array.

---

## Examples

In the following examples, a line from a `csv file` is being read into a `string array`:

``` csharp
var artistCSVLine = "1,John,Mayer,1976,United States,www.johnmayer.com";
var fullArtistArray = artistCSVLine.Split(",");
```

We also have a method to output each item in the array:

``` csharp
public void OutputArray(string[] items)
{
    for (var i = 0; i < items.Length; i++)
    {
        Console.WriteLine(items[i]);
    }

    Console.WriteLine("------");
}
```

Calling the method, with the _fullArtistArray_ array:

``` terminal
1
John
Mayer
1976
United States
www.johnmayer.com
```

But suppose in our application we are only interested in the _artist Names and year of birth_, how do we output only those values?

(this example is very simplified for the sample, and not necessarily a production ready way of dealing with CSV records in this manner)

---

### Without ArraySegment

Without leveraging `ArraySegment`, there are two common ways (in my experience) to get only a portion of an array:

- Creating a separate array:

    ``` csharp
    // define a new array
    var portionArray = new string[3];
    // set the values of the new array from the
    // values of the original array
    portionArray[0] = fullArtistArray[1];
    portionArray[1] = fullArtistArray[2];
    portionArray[2] = fullArtistArray[3];

    // print out only the new array
    OutputArray(portionArray);
    ```

    With this approach, an additional array is created in memory and the information duplicated, so will use more memory.
  
    

- Manual checks for specific elements in the array:

    ``` csharp
    public void OutputArrayPortion(string[] items)
    {
        for (var i = 0; i < items.Length; i++)
        {
            // only output items in specific
            // positions in the array
            if (i == 1 || i == 2 || i == 3)
            {
                Console.WriteLine(items[i]);
            }
        }
        Console.WriteLine("------");
    }   
    ```

    With this approach, the original full array can passed to the method and a manual check is performed to only output items in specific positions in the array. While this does use no additional memory, its not sustainable. If the checks needs to be performed in multiple places in code (when outputting the data, when saving to a database etc) this approach makes the code not very maintainable.

While either of these approaches could still work, a better approach is to leverage the `ArraySegment` class.

---

### With ArraySegment

The `ArraySegment` class can be used to represent only a portion (or segment) of the original array:

``` csharp
// create a segment, from fullArtistArray, starting at position 1
// and include 3 items from the original array
var artistSegment = new ArraySegment<string>(fullArtistArray, 1, 3);
OutputArraySegment(artistSegment);

// ------

public void OutputArraySegment(ArraySegment<string> items)
{
    for (var i = 0; i < items.Count; i++)
    {
        Console.WriteLine(items[i]);
    }
    Console.WriteLine("------");
}

```

The output:

``` terminal
John
Mayer
1976
```

As mentioned, an important feature of the `ArraySegment` is that a `new array is not created`, but the class is a `structure which points to a segment of the original array`.

This can be demonstrated as follows:

``` csharp
var artistCSVLine = "1,John,Mayer,1976,United States,www.johnmayer.com";
var fullArtistArray = artistCSVLine.Split(",");

// create a new array segment
var artistSegment = new ArraySegment<string>(fullArtistArray, 1, 3);

// change the item at index 3 in the ORIGINAL array
fullArtistArray[3] = "1977";

// print out the array segment
OutputArraySegment(artistSegment);
```

The output is as follows:

``` terminal
John
Mayer
1977
```

The year value output is the `updated value`, even though it `was updated on the original array`.

---

## Notes

The `ArraySegment` provides an easy, memory efficient way of working with portions of a larger original array, and should be leveraged whenever possible of the other _non-array-segment_ techniques mentioned above.

---

## References

[ArraySegment Struct](https://docs.microsoft.com/en-us/dotnet/api/system.arraysegment-1?view=net-6.0)   

---

<?# DailyDrop ?>146: 25-08-2022<?#/ DailyDrop ?>
