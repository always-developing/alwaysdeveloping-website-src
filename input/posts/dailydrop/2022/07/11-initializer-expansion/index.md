---
title: "Expanding on an property initializer"
lead: "Adding additional values to a Dictionary property initializer"
Published: "07/11/2022 01:00:00+0200"
slug: "11-initializer-expansion"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dictionary
    - initializer

---

## Daily Knowledge Drop

A class `property initializer can be expanded on in the class declaration` (assuming the property type supports it, such as Dictionary) to a have additional values added.

This technique (explained further below) does have fairly limited practical application and is quite niche - but it does have its place, and practical or not, is very interesting.

---

## Use case

In our use case, when an exception occurs, we want to capture some generic information about the PC on which the exception occurred (machine name, operating system), as well as the actual exception.

If we have an `ExceptionInformation` class to capture all this information:
- Static information such as the _machine name_ can be `automatically initialized` on instantiation of an _ExceptionInformation_ instance
- Dynamic information such as the _exception_ must be `manually supplied` to our capture class

### Constructor

One way of _manually_ supplying the exception information is pass it to the `ExceptionInformation` instance in the constructor:

``` csharp
public class ExceptionInformation
{
    // string representation of the exception
    private readonly string _exceptionString;

    // set the exception in the constructor
    public ExceptionInformation(string exceptionString)
    {
        _exceptionString = exceptionString;
    }

    // A dictionary to contain all the relevent information about the exception
    // initialize it with the information on class initialization
    public Dictionary<string, object> Configuration { get; } = 
        new Dictionary<string, object>
        {
            ["MachineName"] = Environment.MachineName,
            ["OsVersion"] = Environment.OSVersion
        };

    // override method to be able to output a representation of the class
    public override string ToString()
    {
        return $"{_exceptionString}{Environment.NewLine}" +
            $"{String.Join(Environment.NewLine, Configuration.Select(d => $"{d.Key}: { d.Value}"))}";
    }
}

```

The usage of the class would be as follows (with an exception being forced to occur):

``` csharp
try
{
    // force a divide by zero exception
    var intValue = 100;
    _ = intValue / 0;
}
catch(Exception ex)
{
    // capture the exception
    var ei = new ExceptionInformation(ex.ToString());
    Console.WriteLine(ei.ToString());
}
```

And the output:

``` terminal
System.DivideByZeroException: Attempted to divide by zero.
   at Program.<Main>$(String[] args) in 
    C:\Development\Projects\InitializerExpansion\Program.cs:line 5
MachineName: T800
OsVersion: Microsoft Windows NT 10.0.22000.0
```

There are other options to achieve the same result - instead of a setting the private _\_exceptionString_ variable, the exception string value could have been added directly to the dictionary in the constructor:

``` csharp
public ExceptionInformation(string exceptionString)
{
    this.Configuration.Add("ExceptionString", exceptionString);
}
```

Both of the above approaches are valid and will achieve the desired result, however another interesting approach is to _expand on the property initializer_.

---

### Initializer expansion

The initializer can be expanded to include adding custom information to the _ExceptionInformation_ instance. Similar to how the _exceptionString_ was added to the dictionary in the constructor in the above example, except this method is more dynamic and allows for any values to be added.

If we remove all references to _exceptionString_ from the _ExceptionInformation_ class, including from the constructor:

``` csharp
public class ExceptionInformation
{
    // No constructor which takes the exception
   
    // A dictionary to contain all the relevent information about the exception
    // initialize it with the information on class initialization
    public Dictionary<string, object> Configuration { get; } = 
        new Dictionary<string, object>
        {
            ["MachineName"] = Environment.MachineName,
            ["OsVersion"] = Environment.OSVersion
        };

    // override method to be able to output a representation of the class
    // Now ONLY outputs the dictionary
    public override string ToString()
    {
        return String.Join(Environment.NewLine, Configuration.Select(d => $"{d.Key}: {d.Value}"));
    }
}
```

This usage of the class is now as follows:

``` csharp
try
{
    // force a divide by zero exception
    var intValue = 100;
    _ = intValue / 0;
}
catch(Exception ex)
{
    // capture the exception by expanding the Configuration initialization
    var ei = new ExceptionInformation
    {
        Configuration =
        {
            ["ExceptionString"] = ex.ToString()
            // any other data can be added here
        }
    };
    Console.WriteLine(ei.ToString());
}
```

Here the _Configuration_ values specified when an instance of _ExceptionInformation_ is initialized, `are added` to the values initialized internally in the class.

The output of the above is:

``` terminal
MachineName: T800
OsVersion: Microsoft Windows NT 10.0.22000.0
ExceptionString: System.DivideByZeroException: Attempted to divide by zero.
   at Program.<Main>$(String[] args) in 
    C:\Development\Projects\InitializerExpansion\Program.cs:line 5
```

One `advantage` of the initializer expansion method, is that all data is now contained in a single place (the dictionary) and more values can be added dynamically.  
However, on the `negative side`, its not apparently obvious to the developer using the _ExceptionInformation_ class, that additional items can be added to the dictionary in this manner.

---

## Notes

As mentioned, this has a fairly niche use case - and even though there are other methods to do the achieve the same outcome, I find this method especially appealing. Even if I never have a practical need for it in a real application - I still find it an interesting usage of the language features.

---

## References

[8 Bit Ventilator tweet](https://twitter.com/8BitVentilator/status/1524685108606914564)   

<?# DailyDrop ?>114: 11-08-2022<?#/ DailyDrop ?>
