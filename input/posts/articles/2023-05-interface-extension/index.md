---
title: "Sharing a common method across classes"
lead: "Leverage interfaces and extension methods and to share a common method"
Published: "2023-05-26 03:00:00+0200"
slug: "2023-05-interface-extensions"
draft: false
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - extensionmethod
    - extension
    - interface
    - generics
---

## Introduction

Have you needed to add the same method to multiple classes? To have a single method in the code applied to multiple classes, instead of duplicating the code in multiple places? 

This article will detail a technique (which I have personally been using extensively) which `allows for a single extension method to easily be added to any class`.

---

## Inheritance

Before getting to the extension method technique, we are going to have a quick look at using inherence to solve the problem. A common method in a base class can be used to add the functionality to child classes:

``` csharp
// BaseClass which has common functionality
public class BaseClass
{
    public BaseClass ExecuteBaseFunctionality()
    {
        Console.WriteLine("Executing Common functionality");
        return this;
    }
}

// Class1 which can execute Class1 specific functionality
// as well as the common functionality
public class Class1 : BaseClass
{
    public Class1 ExecuteClass1Functionality()
    {
        Console.WriteLine("Executing Class1 functionality");
        return this;
    }
}

// Class2 which can execute Class2 specific functionality
// as well as the common functionality
public class Class2 : BaseClass
{
    public Class2 ExecuteClass2Functionality()
    {
        Console.WriteLine("Executing Class2 functionality");
        return this;
    }
}
```

The reason for returning the **class type** from the methods, is to allow for **method chaining** to take place:

``` csharp
Class1 c1 = new Class1();

c1.ExecuteClass1Functionality()
    .ExecuteBaseFunctionality();

```

However, if the common functionality (a call to the base class method) is required to be executed first, then a cast is required:

``` csharp
Class1 c1 = new Class1();

((Class1)c1.ExecuteBaseFunctionality())
            .ExecuteBaseFunctionality();
```

Not ideal, but also not a big issue.

One big limitation of the inheritance approach is that if the class (`Class1` or `Class2` in this example) already inherits from another class, then the inheritance method simple will not work as C# does not allow for multiple inheritance:

``` csharp
// Already inherits, cannot inherit from "BaseClass"
public class Class1 : AnotherBaseClass
{
    public Class1 ExecuteClass1Functionality()
    {
        Console.WriteLine("Executing Class1 functionality");
        return this;
    }
}
```

`Interfaces combined with extension methods` to the rescue!

---

## Interfaces + Extension Methods

This technique may seem a bit complicated initially, and once the foundations have been setup it is incredibly easy and quick to add the common method to any class.

### Defining the interface

The first step is to define a `marker interface` which will be used as the base for the extension method:

``` csharp
public interface IHasCommonFunctionality<TParent> 
    where TParent : class
{    
}
```

The interface uses a *generic parameter* to indicate the parent type - this is so the specific type can be returned from the extension methods to _enable method chaining_ (shown further down in the post). If method chaining is not required, the generic parameter can be removed:

``` csharp
public interface IHasCommonFunctionality
{
}
```

---

### Defining the extension method

The next step, is to define an extension method, which `extends the functionality of the interface`:

``` csharp
public static class BaseFunctionalityExtensions
{
    // Extension method on IHasCommonFunctionality with a specific generic type parameter
    public static TParent ExecuteBaseFunctionality<TParent>(this IHasCommonFunctionality<TParent> parent)
        where TParent : class
    {
        Console.WriteLine("Executing Common functionality");

        // returns the parent, cast to the actual parent type
        return (TParent)parent;
    }
}
```

At first glace, there is a lot going on - but broken down:
- This is an extension method on the `IHasCommonFunctionality<>` interface. Any class which implements this interface, will have the `ExecuteBaseFunctionality` method available
- The method uses the generic parameter `TParent` - this is the same parameter used when implementing the interface on a class, `IHasCommonFunctionality<TParent>`
- The method returns the generic parameter type (allowing for _method chaining_). As the returned _parent_ variable is of type `IHasCommonFunctionality<TParent>`, and we want to return `TParent`, _parent_ is cast to `TParent`.
- The above cast will _always be valid_ as the type represented by `TParent`, will always implement `IHasCommonFunctionality<TParent>`

A practical example might make it easier to see how the pieces fit together.

---

### Interface implementation

The interface can now me implemented on one or many classes:

``` csharp
public class Class1 : IHasCommonFunctionality<Class1>
{
    public Class1 ExecuteClass1Functionality()
    {
        Console.WriteLine("Executing Class1 functionality");
        return this;
    }
}

public class Class2 : IHasCommonFunctionality<Class2>
{
    public Class2 ExecuteClass2Functionality()
    {
        Console.WriteLine("Executing Class2 functionality");
        return this;
    }
}
```

As you can see, the generic parameter for `IHasCommonFunctionality<>` is the _class the interface is being implemented on_. This gives the extension method visibility of the type that is leveraging the interface.

The extension method can now be leveraged from multiple classes::

``` csharp
Class1 c1 = new Class1();

// ExecuteBaseFunctionality called on Class1, and the 
// Class1 instance is returned allowing 
// ExecuteClass1Functionality to be called
c1.ExecuteBaseFunctionality()
    .ExecuteClass1Functionality();

//-----------

Class2 c2 = new Class2();

// ExecuteBaseFunctionality called on Class2, and the 
// Class2 instance is returned allowing 
// ExecuteClass2Functionality to be called
c2.ExecuteBaseFunctionality()
    .ExecuteClass2Functionality();
```

In the two example above, calling `ExecuteBaseFunctionality` on an instance of `Class1` returns the _Class1 instance_ will calling the method on `Class2` returns the _Class2 instance_, allowing method chaining to still be possible.

---

## Conclusion

When it's not possible to use inheritance to share a common method across multiple classes, the `interface + extension method` technique can prove to be very valuable. Once the ground work has been done, introducing the method to a class is as simple as having the class implement the required interface. In addition, as multiple interface implementations are allowed its possible add different extension methods to different classes independently (not possible with the inheritance technique)

I'm interested to know if this same problem has been solved in any other ways - please comment below with any additional techniques to leverage to solve the problem.

