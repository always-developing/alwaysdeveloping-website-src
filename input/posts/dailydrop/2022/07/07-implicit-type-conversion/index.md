---
title: "Type conversion with the implicit operator"
lead: "How the implicit operator can be used to convert one type to another easily"
Published: "07/07/2022 01:00:00+0200"
slug: "07-implicit-type-conversion"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - typeconversion
    - implicit

---

## Daily Knowledge Drop

The `implicit operator` keywords can be used to define a method to perform an implicit conversion from one type to another.

Effectively this is a mapper function, which is implicitly invoked without any special syntax - making its usage automatic and more natural.

---

## Setup

In this simple example, we have two types, _SourceType_ and _DestinationType_ - _SourceType_ contains an **int** field, while _DestinationType_ contains a **string** field. We require the ability to convert an instance of _SourceType_ to an instance of _DestinationType_.

``` csharp
public class SourceType
{
    public int Value { get; set; }
}

public class DestinationType
{
    public string Name { get; set; }
}
```

---

## Mapper

A conversion between the two types can be performed with a _mapper_ method. There are a number of places this method could be defined, including (but not limited to):
1. a method inside source type
1. the constructor of the destination type
1. a method outside of the source or destination types

``` csharp
public class SourceType
{
    // 1. Method inside SourceType to return
    // an instance of DestinationType
    DestinationType ToDestinationType()
    {
        return new DestinationType
        {
            Name = this.Value.ToString()
        };
    }

    public int Value { get; set; }
}

public class DestinationType
{
    public DestinationType() { }

    // 2. Constructor which takes a parameter of SourceType
    public DestinationType(SourceType sourceType)
    {
        this.Name = sourceType.Value.ToString();
    }

    public string Name { get; set; }
}

// 3. External method to do the mapping between SourceType 
// and DestinationType
DestinationType SourceToDestinationMapper(SourceType sourceType)
{
    return new DestinationType
    {
        Name = sourceType.Value.ToString()
    };
}
```

Any of the above will work, however they are required to be invoked explicitly for the conversion to take place.

---

## Implicit

Instead of one of the mapper methods described above, another technique is to use the `implicit operator` keywords. Effectively this is a mapper method - but is implicitly invoked under certain conditions, instead of having to explicitly be invoked as with a mapper method.

The operator is added to the _SourceType_:

``` csharp
public class SourceType
{
    public int Value { get; set; }

    public static implicit operator DestinationType(SourceType t) => 
        new DestinationType { Name = t.Value.ToString() };
}
```

As you can see, the operator has the same logic as the mapper methods above - a _SourceType_ is accepted as a parameter, and a _DestinationType_ is returned.

Next'll we'll look at how the operator is implicitly invoked.

---

## Usage

There are a number of scenarios in which the implicit operator is automatically invoked.

Assigning a _SourceType_ to a _DestinationType_:

``` csharp
var source = new SourceType { Value = 100 };

// implicit operator code is called for this line
DestinationType destination = source;
```

A _cast_ from one type to the other:

``` csharp
var source = new SourceType { Value = 100 };

// implicit operator code is called for this line
// x is of type DestinationType
var x = (DestinationType)source;
```

When calling a method:

``` csharp
var source = new SourceType { Value = 100 };

// pass SourceType to the method
DoProcessing(source);

// method accepts DestinationType
// implicit operator code is called for this line
public void DoProcessing(DestinationType dest)
{
    Console.WriteLine($"Doing processing on '{dest.Name}'");
}
```

As you can see, no mapper functions are required to be called - the conversion _implicitly_ occurs, resulting in cleaner code.

## Limitations

One limitation of this `implicit operator` keywords, is that is not considered by the `is` or `as` operators. A _cast_ should be invoked to the explicit conversion.

The following will **NOT** compile:

``` csharp
// NOT ALLOWED
var destination = source as DestinationType;
Console.WriteLine(destination.Name);
```

---

## Notes

Personally, I find this simplifies the usage and readability of the code and is something I will try to implement more where it makes sense.  
One potentially drawback of the `implicit operator`, depending on the specific code architecture, is that _SourceType_ now requires a reference to _DestinationType_ which may not make sense or be practical (if _SourceType_ is from a 3rd party for example).  

As always, consider your own use case, and which method will work best for that use case.


---

## References

[User-defined conversion operators (C# reference)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators)   

---

<?# DailyDrop ?>112: 07-07-2022<?#/ DailyDrop ?>
