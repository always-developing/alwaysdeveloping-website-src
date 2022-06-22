---
title: "MaybeNullWhen attribute usage"
lead: "How the MaybeNullWhen attribute can be used to give more information to the compiler about your code"
Published: "07/12/2022 01:00:00+0200"
slug: "12-maybenullwhen-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - maybenullwhen
    - attribute

---

## Daily Knowledge Drop

The `MaybeNullWhen` attribute can be used to specify that when a method returns "ReturnValue", an `out` parameter may be `null` even if the corresponding type does not allow it.  

This is especially applicable in the `TryGet*` methods, such as the _TryGetValue_ method on _Dictionary_, for example. 

---

## Setup

In all the examples below, the project has been set to allow nullable types. In the csproj file, the following is set:

``` xml
<Nullable>enable</Nullable>
```

We have also defined a _Person_ class, with a single _Name_ property:

``` csharp
public class Person
{
    public string Name { get; set; } 
}
```

A dictionary is then created of _Person_'s, and _id_ value set for a `person which does not exist` in the dictionary:

``` csharp
var people = new Dictionary<int, Person>();

people.Add(1, new Person { Name = "Dave" });
people.Add(2, new Person { Name = "John" });
people.Add(3, new Person { Name = "Mike" });
people.Add(4, new Person { Name = "Chris" });

// This Id does not exist in the dictionary
int id = 5;
```

---

## Compiler warning

Next, we'll write a method to `TryGet` a person out of the dictionary - the function will return `true/false` if the dictionary contains the id, as well as having an `out` Person parameter which will contain the _Person_ record, if found. 

This is a standard pattern used often by many core .NET libraries - _Dictionary_ already has a _TryGetValue_ method following this pattern, but for demo purposes we will write a custom method.

``` csharp
// return false if the record not found in the dictionary
// return true if found AND set the out Person to the found record
public bool TryGetPerson(Dictionary<int, Person> d, int id, out Person p)
{
    // check if the dictionary contains the key
    if (!d.ContainsKey(id))
    {
        // set the out parameter value to null
        p = null;
        return false;
    }

    // set the out parameter value to the item in the dictionary
    p = d[id];
    return true;
}
```

The above method will work, and performs as expected - however, the compiler gives us a warning for the line `p = null`:

``` terminal
Cannot convert null literal to non-nullable reference type.
```

This is because the code is assigning a `null` value to the out _Person_ p object, when the parameter hasn't been marked as explicitly allowing nulls.

---

## Nullable type

Let's address the warning - the compiler is telling us we are assigning a `null` value to a `non-nullable` type, so let's make the type `nullable`. This is done by adding a question mark (`?`) after the type:

``` csharp
// Person has changed to nullable, Person?
public bool TryGetPerson(Dictionary<int, Person> d, int id, out Person? p)
{
    // check if the dictionary contains the key
    if (!d.ContainsKey(id))
    {
        // set the out parameter value to null
        p = null;
        return false;
    }

    // set the out parameter value to the item in the dictionary
    p = d[id];
    return true;
}
```

Making the above update will resolve the initial warning - however the usage of the above method has now introduced another warning:

``` csharp
if(!TryGetPerson(people, id, out var person))
{
    return;
}

Console.WriteLine(person.Name);
```

The `Console.WriteLine(person.Name);` line of code results in the warning:

``` terminal
Dereference of a possibly null reference.
```

The compiler is informing us that the code is referencing the _Name_ property on a possibly null object (resulting in an exception).

This can be solved by checking if _person_ is null, again using the question mark:

``` csharp
Console.WriteLine(person?.Name);
```

---

## MaybeNullWhen usage

Another option is, instead of making the _Person_ out parameter nullable, to make use of the `MaybeNullWhen` attribute:

``` csharp
// The attribute is added to the parameter
public bool TryGetPerson(Dictionary<int, Person> d, int id, 
    [MaybeNullWhen(returnValue: false)]out Person p)
{
    // check if the dictionary contains the key
    if (!d.ContainsKey(id))
    {
        // set the out parameter value to null
        p = null;
        return false;
    }

    // set the out parameter value to the item in the dictionary
    p = d[id];
    return true;
}
```

The addition of the parameter is an indicator to the compiler that `the out Person parameter can maybe be NULL when the return value is false`.

The usage of the method is the same as before, but now no longer results in any warnings! 

``` csharp
if(!TryGetPerson(people, id, out var person))
{
    return;
}

Console.WriteLine(person.Name);
```

If however _person_ is reference in a path where it could be null, then the warning occurs again:

``` csharp
if(!TryGetPerson(people, id, out var person))
{
    // person is null here
    Console.WriteLine(person.Name);

    return;
}
```

In the above code snippet, _person.Name_ is being used in a path where _person_ is null (when _TryGetPerson_ returns false) - thanks to the `MaybeNullWhen` attribute, the compiler has enough information to know this will most likely result in a null reference and so the warning occurs.

---

## Notes

Whether writing a line of business application or a reusable library, if applicable, the `MaybeNullWhen` attribute should be used to give more information to the compiler about the intention of the code, thus improving developer experience.

---

## References

[Nullable types, dictionaries and magic](https://www.reddit.com/r/csharp/comments/uxlkub/nullable_types_dictionaries_and_magic/)   

<?# DailyDrop ?>115: 12-07-2022<?#/ DailyDrop ?>
