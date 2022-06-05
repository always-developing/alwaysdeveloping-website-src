---
title: "Creating objects without calling the constructor"
lead: "Using GetUninitializedObject to create objects without calling the object constructor"
Published: "06/15/2022 01:00:00+0200"
slug: "15-get-uninitialized-object"
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

The `RuntimeHelpers.GetUninitializedObject` method can be used to create an instance of an object, without calling its constructor or property initializers.

---

## Usage

### Example class

Consider the following simple _Person_ class:

``` csharp
public class Person
{
    public string Name { get; } = "(not set)";

    public int Age { get;  }

    public Person(int age)
    {
        Age = age;
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
```

The important things to note:
- One constructor which explicitly sets both properties
- One constructor which only explicitly sets the age
- The _Name_ property has an initializer, which sets the value if not explicitly set

---

### Normal initialization

First let's look at "normal" object initializations - two instances of the _Person_ class will be created, using the two different constructors:

``` csharp
var p1 = new Person("Dave", 46);
var p2 = new Person(47);

Console.WriteLine($"p1 has a name of '{p1.Name}' and an age of {p1.Age}");
Console.WriteLine($"p2 has a name of '{p2.Name}' and an age of {p2.Age}");
```

In the first instance, both properties are set, in the second instance only the age is (explicitly) set.

The output is then as follows:

``` powershell
p1 has a name of 'Dave' and an age of 46
p2 has a name of '(not set)' and an age of 47
```

As you might have expected, _p1_ outputs both values passed into the constructor, which _p2_ outputs the _Age_ specified, and the default initialization value for the _Name_.

---

### GetUninitializedObject

Next, we'll look at the `RuntimeHelpers.GetUninitializedObject` method, which is part of the `System.Runtime.CompilerServices` namespace.

``` csharp
using System.Runtime.CompilerServices;

var p1 = new Person("Dave", 46);
var p2 = new Person(47);
var p3 = (Person)RuntimeHelpers.GetUninitializedObject(typeof(Person));

Console.WriteLine($"p1 has a name of '{p1.Name}' and an age of {p1.Age}");
Console.WriteLine($"p2 has a name of '{p2.Name}' and an age of {p2.Age}");
Console.WriteLine($"p3 has a name of '{p3.Name}' and an age of {p3.Age}");
```

In the third instance, we use the _GetUninitializedObject_ to get an `uninitialized instance of Person`.

The output is then as follows:

``` powershell
p1 has a name of 'Dave' and an age of 46
p2 has a name of '(not set)' and an age of 47
p3 has a name of '' and an age of 0
```

As the name of the method suggests, no initialization methods are called - neither the constructors nor the _Name_ initializer.

---

## Notes

A useful library to know about and leverage when the need arises. I wouldn't suggest using it to initialize objects, unless you know how the object will behave without having any constructor or initializers invoked. Doing so may cause instability in the usage of the instance.  

One useful use case could be for auto generating documentation - if the documentation is to give examples of clean uninitialized entities, then this method could be used to get an object, which can then be serialized and output.

---

## References

[Create .NET Objects without Calling The Constructor](https://khalidabuhakmeh.com/create-dotnet-objects-without-calling-the-constructor)  

<?# DailyDrop ?>96: 15-06-2022<?#/ DailyDrop ?>
