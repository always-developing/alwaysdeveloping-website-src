---
title: "Determining bytes of memory allocated"
lead: "Using GetAllocatedBytesForCurrentThread to get the allocated bytes in a thread"
Published: "12/06/2022 01:00:00+0200"
slug: "06-getallocatedbytes"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - bytes
   - memory
   - allocated

---

## Daily Knowledge Drop

The static _Garbage Collector_ class `GC` contains a `GetAllocatedBytesForCurrentThread` method which can be used to _get the number of bytes allocated_ in the current thread (as the name implies). 

This can be leveraged to discover some interesting facts about how the application is allocated _unessacary memory_.

---

## GetAllocatedBytes usage

The usage of `GetAllocatedBytesForCurrentThread` is very simple and straightforward:

``` csharp
// get the bytes allocated at the start of the process
long beforeAllocation = GC.GetAllocatedBytesForCurrentThread();

// declare a string and use it
string strValue = "This is a string";
Console.WriteLine(strValue);

// get the bytes allocated after
long afterAllocation = GC.GetAllocatedBytesForCurrentThread();

// output the results
Console.WriteLine($"Before allocation: {beforeAllocation} | " +
    $"After allocation: {afterAllocation}");
Console.WriteLine($"Bytes used: {afterAllocation - beforeAllocation}");
```

A snapshot of the bytes used is taken before the work, and then again after.

The result of the above:

``` terminal
This is a string
Before allocation: 1761952 | After allocation: 1764552
Bytes used: 2600
```

---

## Interesting findings

### Unused variable

In some instances, if a variable is defined, but never used, the compiler will remove it and no memory will be allocated.

Below is the same example as above, but _without the variable "strValue" ever being used_:

``` csharp
long beforeAllocation = GC.GetAllocatedBytesForCurrentThread();

// declared, but never used
string strValue = "This is a string";

long afterAllocation = GC.GetAllocatedBytesForCurrentThread();

Console.WriteLine($"Before allocation: {beforeAllocation} | " +
    $"After allocation: {afterAllocation}");
Console.WriteLine($"Bytes used: {afterAllocation - beforeAllocation}");
```

The output here:

``` terminal
Before allocation: 1768144 | After allocation: 1768144
Bytes used: 0
```

The compiler is performing optimizations to remove unused code.

---

### Empty array

Declaring an empty array, will still allocated memory - this is because the array itself is of type `Array` and contains information about what the array itself:

``` csharp
long beforeAllocation = GC.GetAllocatedBytesForCurrentThread();

int[] list = new int[0];

long afterAllocation = GC.GetAllocatedBytesForCurrentThread();

Console.WriteLine($"Before allocation: {beforeAllocation} | " +
    $"After allocation: {afterAllocation}");
Console.WriteLine($"Bytes used: {afterAllocation - beforeAllocation}");
```

The output:

``` terminal
Before allocation: 1768088 | After allocation: 1768112
Bytes used: 24
```

Declaring multiple empty arrays, will use memory in multiples of 24 bytes:

``` csharp
long beforeAllocation = GC.GetAllocatedBytesForCurrentThread();

int[] list = new int[0];
int[] list1 = new int[0];
int[] list2 = new int[0];

long afterAllocation = GC.GetAllocatedBytesForCurrentThread();

Console.WriteLine($"Before allocation: {beforeAllocation} | " +
    $"After allocation: {afterAllocation}");
Console.WriteLine($"Bytes used: {afterAllocation - beforeAllocation}");
```

The output for this, 24 bytes x 3:

``` terminal
Before allocation: 1768088 | After allocation: 1768160
Bytes used: 72
```

To use less memory, the static `Array.Empty` method is used. This will assign the same empty static array to each instance, thus not allocated additional memory each time:

``` csharp
long beforeAllocation = GC.GetAllocatedBytesForCurrentThread();

// use Array.Empty instead of new []
int[] list = Array.Empty<int>();
int[] list1 = Array.Empty<int>();
int[] list2 = Array.Empty<int>();

long afterAllocation = GC.GetAllocatedBytesForCurrentThread();

Console.WriteLine($"Before allocation: {beforeAllocation} | " +
    $"After allocation: {afterAllocation}");
Console.WriteLine($"Bytes used: {afterAllocation - beforeAllocation}");
```

The output, only 24 bytes used in total:

``` terminal
Before allocation: 1761896 | After allocation: 1761920
Bytes used: 24
```

---

## Notes

A very useful method which can help benchmark memory usage in an application, as well as just provide an insight into interesting aspects of how the compiler allocates (or doesn't allocated) memory.

---


## References

[How to get allocations in .NET? And how big is an empty array?](https://steven-giesel.com/blogPost/db43d6f4-4b93-415f-be03-600ee358cdfd)  

<?# DailyDrop ?>217: 06-12-2022<?#/ DailyDrop ?>
