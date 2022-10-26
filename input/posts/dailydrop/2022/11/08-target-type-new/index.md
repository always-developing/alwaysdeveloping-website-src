---
title: "Target-type new expressions"
lead: "Omitting the target-type when .NET can infer the type"
Published: "11/08/2022 01:00:00+0200"
slug: "08-target-type-new"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - new
   - targettype

---

## Daily Knowledge Drop

The target-type `new` keyword can be used when declaring a new variable instance without having to specify the type, if .NET can infer the instance type (i.e. the `var` keyword is not used).

In addition, the `new keyword can also be used in other instances where .NET is able to infer the type - such as passing object instances to methods`.

---

## Instance declaration

The `new` keyword can be used when declaring an instance of an object, as long as .NET can infer the type:

``` csharp
// var is used here,so the CreateRequest type
// needs to explicitly be stated so .NET knows the var type
var request = new CreateRequest(1);

// OR

// Explicitly define the type, but then just
// use the new keyword as the type can be inferred 
CreateRequest request1 = new(2);

// This is not valid. What is the type?
// var request2 = new(2);
```

The above usage is however not what this post is above - there is another more interesting technique using `new` and that is using it when passing variables to a method.

---

## Method invocation

As mentioned, this technique about to be shown is the the _new and interesting_ part of this post. The explicit usage of the type can be ignored when .NET can infer the type (as above) - so it makes sense this extends to method parameters, where the parameter type is know.

Consider this _CreateEntity_ method which takes a _CreateRequest_ as a parameter:

``` csharp
void CreateEntity(CreateRequest request)
{
    // do stuff
}
```

The parameter type is fixed, and known by .NET, so instead of doing this:

``` csharp
CreateEntity(new CreateRequest(3));
```

One can do this:

``` csharp
CreateEntity(new (4));
```

More concise, but not necessarily more readable.

This technique can also be extended for nested parameters.

Consider this absurd setup of nested parameters:

``` csharp
// takes an instance of DeleteRequestOne
record DeleteRequest(DeleteRequestOne Request);
// takes an instance of DeleteRequestTwo
record DeleteRequestOne(DeleteRequestTwo Request);
// takes an instance of DeleteRequestTwo
record DeleteRequestTwo(DeleteRequestThree request);
// takes an instance of DeleteRequestFour
record DeleteRequestThree(DeleteRequestFour request);
// takes an instance of DeleteRequestTwo
record DeleteRequestFour(DeleteRequestFive request);
// takes an int value
record DeleteRequestFive(int Id);
```

The following is possible, as all the types are known and inferred:

``` csharp
DeleteEntity(new(new(new(new(new(new(6)))))));
```

A very interesting concept, but again, not very readable at all.

---

## Notes

As mentioned, this is an interesting technique which does require less typing and results in more concise code - at the cost of readability. Use this technique sparingly where it makes sense.
Having said that, there are options in IDEs (in Visual Studio a the very least) to _display inline parameter hints_ which indicate the inferred types, so in the end the code is still readable, but with less typing. However this might not be available in all applications where the code is read (such as in Github or Azure DevOps code viewer).

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1580969384696217605)  

<?# DailyDrop ?>197: 08-11-2022<?#/ DailyDrop ?>
