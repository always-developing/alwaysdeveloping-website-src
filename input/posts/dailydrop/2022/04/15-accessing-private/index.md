---
title: "Accessing private variables"
lead: "In certain scenarios private variables can be accessed externally"
Published: 04/15/2022
slug: "15-accessing-private"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - private
    - accessing

---

## Daily Knowledge Drop

Private variables of a class cannot be accessed from outside the class - except for in one specific case!  
Private variables of one instance of a class, `can be accessed from within another instance of the same class`

---

## Example

Consider the _Person_ example below:

``` csharp
public class Person
{
    public DateOnly BirthDay { get; set; }

    private int Age;
    
    public Person() 
    { }

    public Person(DateOnly birthday)
    {
        BirthDay = birthday;
        Age = DateOnly.FromDateTime(DateTime.Now).Year - birthday.Year;
    }
}
```

The _Age_ variable is private, and as such cannot be accessed externally. In this example the value of _Age_ set in the constructor and never used again.

However, if we update the class, adding two new methods: 
- One method which takes a _Person_ instance as an input parameter
- One method which returns a _Person_ instance

``` csharp
public class Person
{
    public DateOnly BirthDay { get; set; }

    private int Age;
    
    public Person() 
    { }

    public Person(DateOnly birthday)
    {
        BirthDay = birthday;
        Age = DateOnly.FromDateTime(DateTime.Now).Year - birthday.Year;
    }

    public Person Clone()
    {
        return new Person
        {
            Age = this.Age, // this variable is private!
            BirthDay = this.BirthDay
        };
    }

    public bool IsOlderThan(Person otherPerson)
    {
        return this.Age > otherPerson.Age; //otherPerson.Age is private!
    }
}
```

- **Clone method**: In this method, a new instance of _Person_ is declared, and the `private Age` variable is accessible and manually set. This is only possible as the new _Person_ instance is being declared within the _Person_ class.

- **IsOlderThan method**: In this method, an instance of _Person_ is passed in as a parameter. The `private Age` value of the parameter is compared against the Age of `this Person`. This is only possible as the _Person_ parameter is being used within the _Person_ class.

---

## Notes

There is a very narrow scope in which private variables are accessible "outside" the instance of the class - so this is not something to worry about happening on a broad scale. However when the use case arises, it is useful to know about this feature - it means the private variable doesn't unnecessarily have to be changed to public (or internal)

---

## References

[Five C# Features You Might Not Know](https://auth0.com/blog/five-csharp-features-you-dont-know/)  

<?# DailyDrop ?>53: 15-04-2022<?#/ DailyDrop ?>
