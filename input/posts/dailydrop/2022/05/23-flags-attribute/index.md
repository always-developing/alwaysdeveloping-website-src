---
title: "Flags attribute for enums"
lead: "Using the Flags attribute to treat an enum as a set of flags"
Published: 05/23/2022
slug: "23-flags-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - enum
    - attribute
    - flags

---

## Daily Knowledge Drop

The `Flags` attribute can be used to automatically treat an enum as a set of flags (a bit field) - flags are useful when elements in the enum might occur in combinations.

---

## Bitwise operations

Often an enum will have values of `0, 1, 2, 4, 8, 16 ..` instead of `0, 1, 2, 3, 4, 5 ..` - this is because it allows bitwise operations to be performed on the enum. Consider the following example enum used to indicate which destination(s) data should be published to: 

``` csharp
enum PublishDestination : short
{
    None = 0,
    File = 1,
    Api = 2,
    Database = 4,
    Console = 8
};
```

To indicated that data should be published to multiple destinations, multiple values can be combined together - for example: `7 = 1 + 2 + 4 = File & Api & Database`.

If **0, 1, 2, 3, 4, 5, 6 ..** had been used, then **7 could be one of a few combinations**:
- 7 = 1 + 2 + 4
- 7 = 1 + 6

Using `0, 1, 2, 4, 8, 16, ..` allows for each number and each sum of numbers to **uniquely identity a combination of values**.

---

## Non Flags example

Consider the following enum, used to indicate which destinations data should be published to:

``` csharp
enum PublishDestination : short
{
    None = 0,
    File = 1,
    Api = 2,
    Database = 4,
    Console = 8
};
```

Iterating through each possible combination of values as follows:

``` csharp
for (int pd = 0; pd < 16; pd++)
    Console.WriteLine("{0,3} - {1:G}", pd, (PublishDestination)pd);
```

Results in the following output:

``` powershell
0 - None
1 - File
2 - Api
3 - 3
4 - Database
5 - 5
6 - 6
7 - 7
8 - Console
9 - 9
10 - 10
11 - 11
12 - 12
13 - 13
14 - 14
15 - 15
```

As you can see, only the values represented in the enum (0, 1, 2, 4, 8) are converted. The runtime doesn't know, for instance, that `3  = 1 + 2 = File + Api`

Introducing the `Flags` attributes indicates that the enum can be treated as a set of flags.

---


## Flags example

Consider the same enum example from above, except now the `Flags` attribute has been introduced:

``` csharp
[Flags]
enum PublishDestination : short
{
    None = 0,
    File = 1,
    Api = 2,
    Database = 4,
    Console = 8
};
```

Iterating through each possible combination of values as follows:

``` csharp
for (int pd = 0; pd < 16; pd++)
    Console.WriteLine("{0,3} - {1:G}", pd, (PublishDestination)pd);
```

Now results in the following output:

``` powershell
0 - None
1 - File
2 - Api
3 - File, Api
4 - Database
5 - File, Database
6 - Api, Database
7 - File, Api, Database
8 - Console
9 - File, Console
10 - Api, Console
11 - File, Api, Console
12 - Database, Console
13 - File, Database, Console
14 - Api, Database, Console
15 - File, Api, Database, Console
```

All values are now accurately interpreted, either as a single enum value, or as a combination of enum values.

---

## Notes

Adding the `Flags` attribute doesn't effect the behavior drastically, but it does make slight differences. With and without `Flags` the following behavior is the same:
- A variable can still contain a combination of enum values
- _Enum.Parse_ operates the same in both cases
- The _HasFlag_ method on the enum operates the same in both cases
- Bitwise operations behavior is the same in both cases

Adding the `Flags` attribute:
- Changes the _ToString()_ result
- Indicates to readers of the code, that the enum is used as a flag and an enum variable could contain a combination of values

So while not drastic, it is best practice and a good idea to add the `Flags` attribute if the enum is intended to be used as flags.

---

## References

[FlagsAttribute Class](https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute?view=net-6.0)  

<?# DailyDrop ?>79: 23-05-2022<?#/ DailyDrop ?>
