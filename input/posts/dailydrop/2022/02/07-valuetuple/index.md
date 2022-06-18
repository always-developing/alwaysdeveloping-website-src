---
title: "C# ValueTuple"
lead: "Exploring the C# ValueTuple type"
Published: 02/07/2022
slug: "07-valuetuple"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - tuple
    - valuetuple
    - .net6
    
---

## Daily Knowledge Drop

You may have heard of C# the `Tuple` type, but there is also a `ValueTuple` type available, which has existed in C# since .NET 4.7!

The post will will take a brief look at the `Tuple` type and compare its functionality to that of the `ValueTuple` type.

---

## Tuple

### Tuple usage

A Tuple is a `data structure which has a specific number and sequence of elements`. The data structure can contain up to 8 elements, but if more are required, nested tuple objects can be leveraged in the 8th element to extent the number of elements.

It is an effective, quick and simple way to create an _immutable_ data entity structure without creating an entire class entity. 

What do I mean by this?  
Assume you have a method which needs to return a number of details related to a _person_. The go to solution would be to create a _Person_ class with the relevant properties and return an instance of the class from the method.  
If this class is only going to be used in one place, it might not make sense for an entire class to be defined - in this case a `Tuple` can be used instead.

The following example uses .NET6 C# console application with minimal startup:

``` csharp
// Invoke the method and get the tuple back
var personInfo = GetPersonInformation();

// Each element in the tuple can be accessed
// The elements in the Tuple are accessed using 
// the `ItemX` property, which corresponds to the element in position X 
Console.WriteLine(personInfo.Item1);
Console.WriteLine(personInfo.Item2);
Console.WriteLine(personInfo.Item3);
Console.WriteLine(personInfo);

static Tuple<string, string, int> GetPersonInformation()
{
    return new Tuple<string, string, int>("Dave", "Grohl", 53);
}
```

The output is as follows:

``` powershell
Dave
Grohl
53
(Dave, Grohl, 53)
```

---

### Tuple creation

In the above example, the `Tuple` was created and returned using the constructor method. These is another way - the static `Tuple.Create` method.

``` csharp
// Using a generic constructor 
var constructorTuple = new Tuple<string, string, int>("Dave", "Grohl", 53);

// Using the static Tuple Create method
var createTuple = Tuple.Create("Dave", "Grohl", 53);
```

Both are equivalent, however the _Create_ method is easier to use when dealing with nested Tuples.

---

## ValueTuple

### ValueTuple usage

A `ValueTuple` operations the same as a `Tuple` on the surface, but there are a number of key differences between the two.

- **ValueTuple** is a _struct_ (a value type), while a **Tuple** is a _class_ (a reference type)
- **ValueTuple** is _mutable_ (can be changed), while a **Tuple** is _immutable_ (they are read-only)
- **ValueTuple** data members are fields, while **Tuple** data members are properties.

In the above example, `Tuple` can be replaced with `ValueTuple` and the code will still execute as before, with the same output:

``` csharp
var personInfo = GetPersonInformation();

Console.WriteLine(personInfo.Item1);
Console.WriteLine(personInfo.Item2);
Console.WriteLine(personInfo.Item3);
Console.WriteLine(personInfo);

static ValueTuple<string, string, int> GetPersonInformation()
{
    return new ValueTuple<string, string, int>("Dave", "Grohl", 53);
}
```

In addition to the differences mentioned above, the `ValueTuple` also has additional ways in which it can be created, which _allows working with the elements much easier_.

---

### ValueTuple creation

As seen above a `ValueTuple` can be created using the constructor:

``` csharp
    var consValueTuple = new ValueTuple<string, string, int>("Dave", "Grohl", 53);
```

It can also be created using the static _Create_ method:

``` csharp
    var createValueTuple = ValueTuple.Create("Dave", "Grohl", 53);
```

In addition it can be created as follows

``` csharp
    var valueTuple = ("Dave", "Grohl", 53);
```

In all three of the above, the elements are still accessed using the _Item1_, _Item2_, etc properties. With a `ValueTuple` it is possible to _give each element a name and access it by name_.  

Instead of using the _var_ keyword, we explicitly define the ValueTuple types, with names.

``` csharp

(string FirstName, string LastName, int Age) nameValueTuple = ("Dave", "Grohl", 53);

// The elements can now be accessed by name
Console.WriteLine(nameValueTuple.FirstName);
Console.WriteLine(nameValueTuple.LastName);
Console.WriteLine(nameValueTuple.Age);
Console.WriteLine(nameValueTuple);

// This will also still also work
Console.WriteLine(nameValueTuple.Item1);
Console.WriteLine(nameValueTuple.Item2);
Console.WriteLine(nameValueTuple.Item3);

```

Any of the three initialization methods can be used to create the a `ValueTuple` with names.

``` csharp
/// Create using the constructor
(string FirstName, string LastName, int Age) consValueTuple = 
    new ValueTuple<string, string, int>("Dave", "Grohl", 53);

// Create using the static method
(string FirstName, string LastName, int Age) createValueTuple = 
    ValueTuple.Create("Dave", "Grohl", 53);

// Create using the abbreviated syntax
(string FirstName, string LastName, int Age) valueTuple = ("Dave", "Grohl", 53);

```


A named ValueTuple can also be returned from a method:

``` csharp
var personInfo = GetPersonInformation();

// now accessed by name
Console.WriteLine(personInfo.FirstName);
Console.WriteLine(personInfo.LastName);
Console.WriteLine(personInfo.Age);
Console.WriteLine(personInfo);

static (string FirstName, string LastName, int Age) GetPersonInformation()
{
    return ("Dave", "Grohl", 53);
}
```

---

## Notes

Today we looked at the `Tuple` and `ValueTuple` structures, how they are different and the various ways they can be initialized.  

For additional reading, see the references below.

---

## References
[Tuple Class](https://docs.microsoft.com/en-gb/dotnet/api/system.tuple?view=net-6.0)  
[ValueTuple Class](https://docs.microsoft.com/en-us/dotnet/api/system.valuetuple?view=net-6.0)  
[Tuple in C#](https://code-maze.com/csharp-tuple/)


<?# DailyDrop ?>05: 07-02-2022<?#/ DailyDrop ?>