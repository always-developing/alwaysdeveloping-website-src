---
title: "Value type default values"
lead: "Understanding the default value for a value type"
Published: 05/24/2022
slug: "24-value-type-default-value"
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

`Value Types` by default, have to have a value and cannot be null in C# - even if uninitialized. This can lead to unexpected results if comparing a value type to null instead of the default value.

---

## Null comparison 

Consider the below example:

``` csharp
class Program
{
    static Point pointValue;
    static int IntValue;

    static void Main(string[] args)
    {
        // comparing an uninitialized Point to null
        Console.WriteLine(pointValue == null);  
        Console.WriteLine(IntValue == null);  
    }
}
```

The output for this is:

``` powershell
    False
    False
```

Both the Point and int types are `value types` and as such have default values of (0, 0) and 0 respectively (**not null**), even if uninitialized.

So instead of comparing to **null**, we need to check if they are `empty` or have the `default value` for the type.

---

## IsEmpty comparison

Some value types (not but all), have a `IsEmpty` property which will return true if uninitialized or explicitly set to the default value:

``` csharp
class Program
{
    static Point pointValue;
    static Point pointValue2 = new Point(0,0);
    static int IntValue;
    
    static void Main(string[] args)
    {
        Console.WriteLine(pointValue.IsEmpty);    
        Console.WriteLine(pointValue2.IsEmpty);   
    }
}
```

The output for this is now:

``` powershell
    True
    True
```

Both variables are considered empty as they both have the default value. `int` does not have an _IsEmpty_ property, so cannot be checked using this method.

---

## default comparison

The `default` keyword (which will produced the default value of a type) can be compared to a value type to determine if it has the default value:

``` csharp
class Program
{
    static Point pointValue;
    static Point pointValue2 = new Point(0,0);
    static int IntValue;
    
    static void Main(string[] args)
    {
        Console.WriteLine(pointValue == default);  
        Console.WriteLine(pointValue2 == default); 
        Console.WriteLine(IntValue == default); 
    }
}
```

The output for this is now:

``` powershell
    True
    True
    True
```

As you can see, `default` works for _Point_ and _int_, and in fact it will work with all types.

---

## Nullable types

A value type can made `nullable` indicating it _can_ have a null value. This is done using the `?` indicator. In the below sample, each variable is now marked with `?`:

``` csharp
class Program
{
    static Point? pointValue;
    static Point? pointValue2 = new Point(0,0);
    static int? IntValue;
    
    static void Main(string[] args)
    {
        Console.WriteLine(pointValue == null);
        Console.WriteLine(pointValue2 == null);
        Console.WriteLine(IntValue == null);    
    }
}
```

Now when comparing these values to `null` the output is as follows:

``` powershell
    True
    False
    True
```

If a nullable value type is not initialized, it will have a null value.

---

## Notes

`Value type comparison` is a small detail in a large application, but if done incorrectly can lead to unexpected results. Its important to understand how the value type variables are set (or not set) and how each one can be checked for a null or default value.

---

## References

[Common C# Programming Mistake #2: Misunderstanding default values for uninitialized variables](https://www.toptal.com/c-sharp/top-10-mistakes-that-c-sharp-programmers-make)  

<?# DailyDrop ?>80: 24-05-2022<?#/ DailyDrop ?>
