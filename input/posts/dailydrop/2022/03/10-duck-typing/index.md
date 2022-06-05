---
title: "Duck typing in C#"
lead: "What is duck typing and how does it work in C#?"
Published: 03/10/2022
slug: "10-duck-typing"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - ducktyping

---

## Daily Knowledge Drop

The term `duck typing` refers to the ability to _allow for an object to be passed to a method which expects a certain type, even if the object doesn't inherit from the type_. 

This is more prevalent in dynamic languages and less prevalent in strong type languages, such as C# - however it is still occasionally used.

---

## Duck typing summary

The term `duck typing` is explained by the populate phrase:
``` powershell
    If it walks like a duck, and quacks like a duck, it must be a duck
```

How does this relate to code? If a _method expects a certain object type as a parameter, and invokes a method on this parameter - with **duck typing**, a different object with the same method could be used instead of the specified type_. 

If object type B, _looks like_ object type A, then it must be of type A - and can be used instead of type A.

--- 

## Valid example

The following C# sample is valid in demonstrating `duck typing`, however it **WILL NOT COMPILE**, but it gives an example of how duck typing works:

``` csharp
public class Duck
{
    public int NumberOfWings { get; set; }

    public void MakeSound()
    {
        Console.WriteLine("Quack");
    }
}

public class Car
{
    public int NumberOfWheels { get; set; }

    public void MakeSound()
    {
        Console.WriteLine("Vroom");
    }
}

public class NoiseMaker
{
    public void MakeNoise(Duck entity)
    {
        entity.MakeSound();
    }
}

var duck = new Duck();
var car = new Car();

var noiseMaker = new NoiseMaker();

noiseMaker.MakeNoise(duck);
// a strong type lang such as C#
// does not approve of or allow this
noiseMaker.MakeNoise(car);
```

With `duck typing`, the above scenario would be **allowed**. This is because:
- Both **Duck** and **Car** entities have a method called _MakeSound()_, with the _same signature_
- The _MakeNoise()_ method only uses the common _MakeSound()_ method, and not either of the fields which are not common (_NumberOfWings_ or _NumberOfWheels_)

In this setup, _according to MakeNoise(), a Car looks like a Duck_, and therefor can be used instead.

---

## Invalid example

If the _MakeNoise_ method was altered to also access the _NumberOfWings_ property, then `duck typing` would no longer apply:

``` csharp
public class NoiseMaker
{
    public void MakeNoise(Duck entity)
    {
        entity.MakeSound();
        duck.NumberOfWings = 2;
    }
}

var duck = new Duck();
var car = new Car();

var noiseMaker = new NoiseMaker();

noiseMaker.MakeNoise(duck);
// Not allowed by C#, BUT ALSO not
// allowed according to duck typing 
// as Car no longer looks like a Duck
noiseMaker.MakeNoise(car);
```

With `duck typing`, the above scenario would NOT be **allowed**. This is because:
- Both **Duck** and **Car** entities have a method called _MakeSound()_, with the _same signature_
- The _MakeNoise()_ method uses the common _MakeSound()_ method, however is also uses the _NumberOfWings_ property, which a Car doesn't have.

In this setup a _Car does not look like a Duck, according to MakeNoise()_

---

## Valid C# example

The above examples are not valid in C#, as it's a strong-type language which doesn't do a check by **similarity**, but rather by name/type.

However there are examples of C# using `Duck Typing`. A good example of this is `GetEnumerator`, outlined in [this post](../03-getenumerator/)

To add the ability to use `foreach` on a class, all that is required is that a **GetEnumerator** method be added to the class, it's not required to be of any type or implement any interface.

If it looks like an enumerator (has a GetEnumerator method), then it is an enumerator!

---

## Notes

`Duck typing` is not especially useful or practical in C#, however its a useful general programming concept to know and also to know C# does leverage it occasionally.

---

## References
[How Duck Typing Benefits C# Developers](http://haacked.com/archive/2007/08/19/why-duck-typing-matters-to-c-developers.aspx/)  

<?# DailyDrop ?>28: 10-03-2022<?#/ DailyDrop ?>
