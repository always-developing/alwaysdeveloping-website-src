---
title: "Readonly parameters with the in modifier"
lead: "C# has a lesser used in parameter modifier which prevents parameter value modification"
Published: "08/01/2022 01:00:00+0200"
slug: "01-in-modifier"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - in
    - parameter
    - modifier

---

## Daily Knowledge Drop

The `in` parameter modifying keyword is used to cause a parameter be passed by reference, and ensure that cannot be modified in the method.

The `in` modifier's usage is similar to the `ref` and `out` keywords, except `ref` parameters can be modified and `out` parameters must be modified/set, while the `in` modifier effectively makes the parameter read-only.

---

## Types
### Simple value types

First lets have a look at how value types are handled in a few simple examples:

``` csharp
int originalValue = 1001;
InKeywordMethod(originalValue);

// parameter used in keyword
void InKeywordMethod(in int theValue)
{
    // this is not allowed and 
    // will not compile if uncommented
    // theValue = 999;
}

```

If uncommented, the application will not compile with the error:

``` terminal
Cannot assign to variable 'in int' because it is a readonly variable
```

As expected based on the introduction, with the use of the `in` keyword, the _theValue_ variable is read-only inside the scope of the method, and cannot be modified.

---

### Structure value types

The `in` keyword applied to a `struct` (a value type) parameter, yields in the same results as in the previous example with sample value types.

Consider the following `struct`:

``` csharp
public struct StructOptions
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
}
```

And it's usage:

``` csharp
var sOptions = new StructOptions
{
    IntValue = 759,
    StringValue = "StringValue"
};
InStructKeywordMethod(sOptions);

void InStructKeywordMethod(in StructOptions options)
{
    // this is not allowed and will not compile if uncommented
    // options.IntValue = 100;

    // this is also not allowed and will result in an error
    /*
    options = new StructOptions
    {
        IntValue = 123,
        StringValue = "NewStringValue!"
    }
    */
}

```

The properties of the `struct` as well as the `struct` itself are both read-only.

---

### Reference types

Reference types however operate slightly differently when used with the `in` keyword.

We will use the same _Options_ data structure as in the previous example, however this time define it as a `class` instead of a `struct`:

``` csharp
public class Options
{
    public int IntValue { get; set; }

    public string StringValue { get; set; }
}
```

And it's usage:

``` csharp
var options = new Options
{
    IntValue = 759,
    StringValue = "StringValue"
};

Console.WriteLine(options.IntValue);
InClassKeywordMethod(options);
Console.WriteLine(options.IntValue);

void InClassKeywordMethod(in Options options)
{
    // This is allowed!
    options.IntValue = 123;
}
```

With the reference type, modifications of its properties are allowed. Running the above code does not result in any compiler errors, with the output as follows:

``` terminal
759
123
```

However, modification of the `class` instance (not it's properties) is **NOT** allowed, and will result in a **compiler error**:

``` csharp
void InClassKeywordMethod(in Options options)
{
    // Cannot assign to variable 'in Options' because it is a readonly variable
    /*
    options = new Options
    {
        IntValue = 123,
        StringValue = "NewStringValue"
    };
    */
}
```
---

## Notes

Not a modifier which will see everyday use, but interesting all the same. If using the `in` keyword, keep in mind that reference types properties can still be modified, and consider other options to make them readonly (removing the _set_ accessor, for example)

---

## References

[in parameter modifier](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-parameter-modifier)   

---

<?# DailyDrop ?>128: 01-08-2022<?#/ DailyDrop ?>
