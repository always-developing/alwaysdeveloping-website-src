---
title: "Static constructors in C#"
lead: "Exploring the behavior of static constructors in C#"
Published: 05/17/2022
slug: "17-static-constructor"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - constructor
    - static

---

## Daily Knowledge Drop

A class can have a `static constructor` which is `only every called once`, the first time a class of the particular type is instantiated.

Today's bit of knowledge is something _I know I have known at some point in the past_, but due to effectively never using this feature, I find myself relearning it now - and find it interesting enough to feature on today's knowledge drop.

---

## Characteristics

There are some characteristics of a static constructor which makes them different to a normal constructor:
- A static constructor does not have any access modifiers or have any parameters
- A class or struct can only have one static constructor
- Static constructors cannot be inherited or overloaded
- A static constructor cannot be called directly and is automatically invoked by the runtime

There are the main characteristics, however there are others - see the references link below.

---

## Example

In the below example, we have a _FileWriter_ class which contains two constructors (one static) and a method to write a string value to file:

``` csharp
public class FileWriter
{
    private static readonly string currentFileName;

    static FileWriter()
    {
        currentFileName = $"output_{DateTime.Now.ToString("yyyyMMddhhmmss")}.txt";
        
        Console.WriteLine("Static Constructor called");
        Console.WriteLine($"Setting current file to: '{currentFileName}'");
    }

    public FileWriter()
    {
        Console.WriteLine("Constructor called");
    }

    public void WriteString(string output)
    {
        Console.WriteLine($"Writing '{output}' to '{currentFileName}'");
    }
}
```

Instead of checking for the file existence each time the _WriteString_ method is called, in the static constructor is leveraged to create the file once, and set the static _currentFileName_ string.

The first time an instance of the class is instantiated, the file will be create and the _currentFileName_ set - every other instance will now have access to the filename, and can be assured that the file exists.

We can confirm this by running the following:

``` csharp
var writer = new FileWriter();
var writer2 = new FileWriter();

writer.WriteString("Hello world");
writer2.WriteString("Hello universe");
```

The output being:

``` powershell
Static Constructor called
Setting current file to: 'output_20220420060039.txt'
Constructor called
Constructor called
Writing 'Hello world' to 'output_20220420060039.txt'
Writing 'Hello universe' to 'output_20220420060039.txt'
```

The static constructor is only called once, and both instance of the _FileWriter_ are writing to the same file.

---

## Notes

A very useful feature (which I am sure I will _forget_ about again with time) when a piece of logic needs to only be executed once. The logic can be executed once to be leveraged by all instances of the class.

---

## References

[Static Constructors (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-constructors)  

<?# DailyDrop ?>75: 17-05-2022<?#/ DailyDrop ?>
