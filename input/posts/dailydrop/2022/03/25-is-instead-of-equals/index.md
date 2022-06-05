---
title: "Null checking with the is keyword"
lead: "Why you should do null checking with is keyword instead of =="
Published: 03/25/2022
slug: "25-is-instead-of-equals"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - check
    - is

---

## Daily Knowledge Drop

When checking if an instances of a object is null, the `is` keyword should be used instead of the double equals `==` operator.

This is because the `==` operator can be overloaded to change its meaning, while the `is` keyword cannot.

---

## Comparing using ==

Consider a `Person` class, which contains _Name_ and _Age_ properties.

``` csharp

// define 3 person instances
Person p1 = null;
Person p2 = new Person();
Person p3 = new Person { Name = "John", Age = 33 };

if (p1 == null)
{
    Console.WriteLine($"p1 == null");
}

if (p2 == null)
{
    Console.WriteLine($"p2 == null");
}

if(p3 == null)
{
    Console.WriteLine($"p3 == null");
}
```

In the above example, three `Person` instances are defined:
- One explicitly set to null 
- One set to a default instance of _Person_
- One set with explicit values

Each of these is checked to see if they are `null`, using the `==` operator, with the output as follows:

``` powershell
    p1 == null
    p2 == null
```

Hold on... The output is showing that `p2 == null` is true, when `p2 is clearly not null`! (is was instantiated with _Person p2 = new Person();_) How can this be?

The reason for this, is unknown to us, the author of the _Person_ class has overloaded the `==` operator to function differently to the default expected behavior.

---

## Operator overloading

If we take a look at the contents of the `Person` class in full:

``` csharp
public class Person
{
    public string Name { get; set; }

    public int Age { get; set; }

    public static bool operator == (Person p, Person p1)
    {
        if((p?.Age == 0 && p1 is null) || (p1?.Age == 0 && p is null))
        {
            return true;
        }

        return p?.Name == p1?.Name && p?.Age == p1?.Age;
    }

    public static bool operator !=(Person p, Person p1)
    {
        return !(p?.Name == p1?.Name && p?.Age == p1?.Age);
    }
}
```

The `==` operator has been overloaded to change its meaning - **Line 9** states that `if either of the two Person instances being compared have an Age of 0`, then no matter what, `return false` (meaning the two instances are not equal).

In the previous example the _Person_ instance _p2_ has an `Age of 0`. Therefor according to the new logic for the `==` operator, no matter what the value of the _Person_ being compared to, it will return `false`.

---

## Comparing using is

The `is` operator should be used to do comparison, as it cannot be overloaded and have its definition changed:

``` csharp
Person p1 = null;
Person p2 = new Person();
Person p3 = new Person { Name = "John", Age = 33 };

if (p1 is null)
{
    Console.WriteLine($"p1 is null");
}

if (p2 is null)
{
    Console.WriteLine($"p2 is null");
}

if (p3 is null)
{
    Console.WriteLine($"p3 is null");
}
```

The output is as follows:

``` powershell
    p1 == null
```

This is the expected output, as _p1_ is the only instance which is truly `null` - the output is now accurate.

---

## Notes

This is a small distinction between the two operators (`==` and `is`), and might not ever be an issue if you the author of the classes as well as the code using them. However in the case when using 3rd party classes, its safer to use the `is` keyword and ensure the code operates as expected.

---

## References

[Operator overloading](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading)

<?# DailyDrop ?>38: 25-03-2022<?#/ DailyDrop ?>
