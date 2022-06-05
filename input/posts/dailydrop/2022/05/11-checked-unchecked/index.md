---
title: "checked and unchecked keywords"
lead: "Using the checked and unchecked keywords to control overflow checking"
Published: 05/11/2022
slug: "11-checked-unchecked"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - checked
    - unchecked
    - overflow

---

## Daily Knowledge Drop

The `checked` and `unchecked` C# keywords, can be used to control overflow checking when working with integral type arithmetic operations and conversions:

- `checked`: is used to explicitly enable overflow checking
- `unchecked`:  is used to suppress overflow-checking

---

## checked

The `checked` keyword is used to explicitly enable overflow checking. Consider the following:

``` csharp
int maxInt = Int32.MaxValue;
Console.WriteLine(maxInt);

maxInt = maxInt + 10;
Console.WriteLine(maxInt);
```

Here an `int` is being defined with its value set to the `maximum possible value` an int can have. It's then being increased by 10.

The output to the above is as follows:

``` csharp
2147483647
-2147483639
```

By default the overflow does not cause an exception, and the addition operation does not produce a accurate outcome.

To force an exception, the `checked` keyword can be used:

``` csharp
// wrap the entire block
checked
{
    int maxInt = Int32.MaxValue;
    Console.WriteLine(maxInt);

    maxInt = maxInt + 10;
    Console.WriteLine(maxInt);
}

// wrap just the specific operation
int maxInt2 = Int32.MaxValue;
Console.WriteLine(maxInt2);

maxInt2 = checked(maxInt2 + 10);
Console.WriteLine(maxInt2);
```

Now when either of the above sections of `checked` code is executed, an exception occurs:

``` powershell
2147483647
Unhandled exception. System.OverflowException: 
        Arithmetic operation resulted in an overflow.
   at Program.<Main>$(String[] args) in C:\Development\Projects\
        Blog\CheckedUnchecked\CheckedUnchecked\Program.cs:line 9
```

`checked` can be used to explicitly force operations which cause an overflow to throw an exception.

---

## unchecked

The unchecked keyword is used to suppress overflow checking. Consider the following:

``` csharp
int int1 = 2147483647 + 10;
Console.WriteLine(int1);
```

The above `will not compile`, as the compiler can determine that this will result in an overflow. The error received when trying to compile is:

``` powershell
    error CS0220: The operation overflows at compile time in checked mode
```

The compiler can be instructed to ignore this, using the `unchecked` keyword:

``` csharp
// wrap the entire block
unchecked
{
    int int1 = 2147483647 + 10;
    Console.WriteLine(int1);
}

// wrap just the specific operation
int int2 = unchecked(2147483647 + 10);
Console.WriteLine(int2);
```

Now when either of the above sections of `unchecked` code is compiled, it will be successful, but result in the overflow occurring when executed:

``` powershell
-2147483639
-2147483639
```

`unchecked` can be used to explicitly suppress operations which cause an overflow from throwing an exception.

---

## Notes

Very niche keywords, but the `check` specifically could be of use if the application is dealing with integral types who's value frequently approaches the limits. Rather have an exception be thrown, and handle it, than have calculation which result in overflow and incorrect results.

---

## References

[Checked and Unchecked (C# Reference)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/checked-and-unchecked)  

<?# DailyDrop ?>71: 11-05-2022<?#/ DailyDrop ?>
