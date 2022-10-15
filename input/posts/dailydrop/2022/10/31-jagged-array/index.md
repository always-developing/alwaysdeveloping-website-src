---
title: "Jagged array in C#"
lead: "Learning about jagged arrays in C#"
Published: "10/31/2022 01:00:00+0200"
slug: "31-jagged-array"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - array
   - jagged

---

## Daily Knowledge Drop

A `jagged array` is an _array of arrays_, where each internal array can be of varying length.

---

### Array

A quick recap on a traditional one dimensional array:

``` csharp
var array = new int[10];
int[] array1 = new int[10];
```

The array size is defined when the variable is instantiated - this array is defined to hold up to 10 int's. 

---

### Multi-dimensional array

A multiple dimensional is declared and instantiated the same way as a one-dimensional array:

``` csharp
// declare a two dimensional array
int[,] multiArray = new int[10,10];

// a three dimensional array
int[,,] multiArray1 = new int[10, 10, 10];
```

With multiple directional arrays, the size of the dimension is _fixed_. In the above two-dimensional array for example, a 10x10 array is defined - it can be thought of a _grid with 10 columns and 10 rows_. The important aspect is that each row `will always` have 10 columns at most.

---

### Jagged array

A `jagged array` is an _array of arrays_, where the internal array is of varying length.

There is a subtle difference when declaring a `jagged array` vs declaring a multiple dimensional array:

``` csharp
// Jagged array
int[][] jaggedArray1 = new int[10][];

// For comparison - multi directional array
int[,] multiArray = new int[10,10]
``` 

The jagged array _jaggedArray1_, has _10 rows, but each row has varying column length_ - a second size is not specified when declaring the array. As it stands now, each row is null by default - the array in each row needs to be initialized:

``` csharp
jaggedArray1[0] = new int[5];
jaggedArray1[1] = new int[3];
jaggedArray1[2] = new int[1];
```

The first 3 rows are being initialized to each have a different number of items. We have an _array of arrays_, with each internal array being of different sizes.

---

## Notes

This is not functionality I've personally ever had to use - but I am sure it definitely does have its practical uses cases. Just being aware the functionality exists can help make better, more informed design choices going forward.

---

## References

[Arrays (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/)  

<?# DailyDrop ?>191: 31-10-2022<?#/ DailyDrop ?>
