---
title: "Override vs New behavior"
lead: "A look into polymorphism with override and new and their differing behavior"
Published: "08/23/2022 01:00:00+0200"
slug: "23-override-new"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - new
   - virtual
   - polymorphism

---

## Daily Knowledge Drop

When using `polymorphism`, even though on the surface they appear to be doing the same thing, _hiding_ the functionality of a parent, the `override` and `new` keywords behave very differently.

`Override` will _hide_ the parent method invoked, and the _overridden_ method on the child will be called. However when `new` is used on a method, when invoked, the _parent_ method will be called.

---

## Definition recap

A lot of keywords used in the introduction above, so a quick recap on some of the definitions of the keywords.

### Polymorphism

`Polymorphism` allows for objects of a _derived class_ can be treated as objects of the _parent class_.

For example, when a _Child_ class inherits from a _Parent_ class, then the following is valid:

``` csharp
// defined as type Parent
// but assigned a Child
Parent childEntity = new Child();
```

---

### Virtual

Often the _Parent_ class will defined methods as `virtual` (which can also be applied to properties, indexers), which allows for it to be explicitly overridden in the _Child_ class.

``` csharp
public class Parent
{
    public virtual void Hello()
    {
        Console.WriteLine("Hello, I am Parent");
    }
}
```

---

### Override

The `override` keyword is used on a _Child_ class, to `override` the behavior defined by the _Parent_ class:

``` csharp
public class Child : Parent
{
    // Override the Parent Hello method
    public override void Hello()
    {
        Console.WriteLine("Hello, I am Child");
    }
}
```

---

### New

The `new` keyword can be used as a declaration modifier to explicitly _hide_ the base class functionality:

``` csharp
public class Child : Parent
{
    // Hide the Parent Hello method
    public new void Hello()
    {
        Console.WriteLine("Hello, I am Child");
    }
}
```

---

## New vs override

There is a subtle difference between the definitions of the `override` and `new` keywords:
- `override` will **override** the parent method
- `new` will **hide** the parent method

What all this comes down to, is the following - consider a _Parent_ and two different _Child_ classes, one using `new` and one using `override`:

``` csharp
public class Parent
{
    public virtual string Ping()
    {
        return $"Response from {nameof(Parent)}";
    }
}

public class OverrideChild : Parent
{
    public override string Ping()
    {
        return $"Response from {nameof(OverrideChild)}";
    }
}

public class NewChild : Parent
{
    public new string Ping()
    {
        return $"Response from {nameof(NewChild)}";
    }
}
```

When we declare an instance of each of the three types and call the _Ping_ method, we get a response from the method on the respective type:

``` csharp
Parent parent = new Parent();
NewChild newChild = new NewChild();
OverrideChild overChild = new OverrideChild();

Console.WriteLine(parent.Ping());
Console.WriteLine(newChild.Ping());
Console.WriteLine(overChild.Ping());
```

The resulting output being:

``` terminal
Response from Parent
Response from NewChild
Response from OverrideChild
```

So far, so good - all is as expected.

---

### Polymorphism

However, when using `polymorphism` then behavior starts to change.

In the below, each type is `declare of type Parent by assigned one of the three different types` (this is allowed, because of _polymorphism_):

``` csharp
// all type Parent
// but assigned different types
Parent parent = new Parent();
Parent overChild = new OverrideChild();
Parent newChild = new NewChild();

Console.WriteLine(parent.Ping()); 
Console.WriteLine(overChild.Ping()); 
Console.WriteLine(newChild.Ping()); 

```

Now the output is as follows:

``` terminal
Response from Parent
Response from OverrideChild
Response from Parent
```

From this we can see that with:
- `override`: there is a _link_ between the child and parent entities, so when the method is called the runtime knows to call the child entity method (even though the type is _Parent_)
- `new`: the parent method is hidden by the child method, but there is no _link_ between them, so calling the method on a _Parent_ entity (even though it was assigned a _Child_) will call the parent entity method

---

## Notes

Interesting behavior from the runtime which one should be aware of, otherwise unexpended results might occur. There is no _right_ or _wrong_ modifier to use, it will depend on the use case, how the classes are defined and used, and how they should behave.

---

## References

[Override vs New Polymorphism In C# .NET](https://dotnetcoretutorials.com/2022/05/13/override-vs-new-polymorphism-in-c-net/)   

---

<?# DailyDrop ?>144: 23-08-2022<?#/ DailyDrop ?>
