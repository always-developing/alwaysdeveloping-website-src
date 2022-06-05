---
title: "Setting readonly variable using init"
lead: "Readonly variable can be set using an init only setter"
Published: 04/08/2022
slug: "08-init-readonly"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - readonly
    - init

---

## Daily Knowledge Drop

Readonly variables on a class can be set, not only in the constructor, but also using the `init` keyword.

---

## Before init

Prior to the introduction of the `init` keyword in C#9, if a class had a readonly variable, its value had to be set either:
- when declared
- in the constructor of the class

Consider the following class:

``` csharp
public class Song
{
    // if not explicitly set, this variable will have 
    // default int value
    public readonly int Id;

    // explicitly set the value when declared
    public readonly DateTime DateCreated = DateTime.Now;

    public string Name { get; set; }

    // The readonly Id field is set in the constructors
    public Song()
    {
        Id = 0;
    }

    // set both values in the constructor
    public Song(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
```

When using an instance, as expected, the value cannot be set:

``` csharp
var song = new Song();
//song.Id = 100; << This results in an error
```

The issue with this, is that if you want to set the value of _Id_, you `cannot use the object initialization format`. The only way to set the value is using the constructor

``` csharp
var song2 = new Song
{
    Name = "Song2"
    // Id =.. << Id is not available to set here
};

// This is the only way to set the Id
var song3 = new Song(17, "Song3");
```

---

## The init keyword

The `init` keyword was introduced in C#9, which allows for the ability for a property to be set, but only on initialization. 

Here is the same _Song_ class, but with the _Name_ property changed to use `init` instead of _set_:

``` csharp
public class Song
{
    // if not explicitly set, this variable will have 
    // default int value
    public readonly int Id;

    // explicitly set the value
    public DateTime DateCreated { get; init; }

    public string Name { get; init; }

    // set the value in the constructor
    public Song()
    {
        Id = 0;
    }

    // set both values in the constructor
    public Song(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
```

An example of this in action:

``` csharp
// Allowed
var song = new Song(17, "Song");

// All good
var song2 = new Song
{
    Name = "Song2"
};

//song2.Name = "Song3" << NOT allowed
```

---

## Readonly and init

While readonly variables still cannot use the `init` keyword, they can still be set when _other_ properties are initialized.

In the below _Person_ class example, the readonly _Age_ is calculated and set when the _DateOfBirth_ property is set:

``` csharp
public class Person
{
    private DateTime _dateOfBirth;

    public string Name { get; set; }

    public readonly int Age;

    public DateTime DateOfBirth
    {
        get => _dateOfBirth; 
        init
        {
            _dateOfBirth = value;
            // Set the value of the readonly field
            Age = (int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 
                    int.Parse(_dateOfBirth.ToString("yyyyMMdd"))) 
                    / 10000;
        }
    }
}
```

Object initialization can now be used:

``` csharp
var person = new Person();
Console.WriteLine(person.Age);

var person2 = new Person
{
    Name = "Person1",
    DateOfBirth = new DateTime(1983, 08, 25)
};
Console.WriteLine(person2.Age);
```

With the output being:

``` powershell
0
38
```

---

## Limitations

As shown in the above example, there are some limitations when using this approach:
- The readonly field can only be set if another property has an `init only setter`
- The property with the init only setting, needs to be converted to have a `private backing field`, and the _get_ of the property now needs to be manually done

---

## Notes

While not especially useful in every use case, when the need does arise, the ability to calculate and set a readonly property based on the initialization of another field, can prove to be very useful.  
Even though there is some modifications required to the class (as mentioned in the above limitations), implementing this requirement without the `init` keyword would result in even more code.

---

## References

[Init Only Setters](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/init)  

<?# DailyDrop ?>48: 08-04-2022<?#/ DailyDrop ?>
