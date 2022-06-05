---
title: "Conditional attribute to exclude method execution"
lead: "The Conditional attribute can be used to conditionally execute a method"
Published: 03/30/2022
slug: "30-conditional-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - attribute
    - conditional

---

## Daily Knowledge Drop

A method can be marked with the `Conditional` attribute to have its invocation excluded by the compiler under the specified condition.

---

## Example

Consider the following code:

``` csharp

// A ProcessPerson method is called to perform 
// operations on the person instance
ProcessPerson(new Person
{
    Id = Guid.NewGuid(),
    Name = "John",
    Age = 41
});

static void ProcessPerson(Person p)
{
    // Two methods are called, one to perform sparse logging 
    // and another to perform full logging
    SparseLogging(p);
    FullLogging(p);

    // Additional process of the Person p
}

// The SparseLogging method will just output the person id
static void SparseLogging(Person p)
{
    Console.WriteLine($"Processing person record id: {p.Id}");
}

// The FullLogging method will output all full set of person information. 
// his method has also been marked with the `Conditional` attribute, 
// with the parameter `DEBUG`.
[Conditional("DEBUG")]
static void FullLogging(Person p)
{
    Console.WriteLine($"Processing person record");
    Console.WriteLine($"Id: {p.Id}");
    Console.WriteLine($"Name: {p.Name}");
    Console.WriteLine($"Age: {p.Age}");
}

class Person
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }
}
```

The parameter supplied to the attribute needs to be a `preprocessor directives` (conditional complication symbol). In this case the `DEBUG` symbol is only defined and set to true when compiling the code in DEBUG mode, otherwise the symbol is not defined.

The output for the above code is now different depending on if the code is running in debug vs release mode:

Debug:

``` powershell
    Processing person record id: f6aae86e-68c7-46b6-afcf-ba0d1051a6dc
    Processing person record
    Id: f6aae86e-68c7-46b6-afcf-ba0d1051a6dc
    Name: John
    Age: 41
```

Release:

``` csharp
    Processing person record id: 4914e0ea-8664-47af-b2d0-581584e3095e
```

The method marked with the attribute `does not get executed if the condition in the attribute is not met`.

---

## Lowered code

Using [sharplab.io](sharplap.io), we can see what the compiler is doing with the above code. 

Only the relevant portion of code has been included - when compiled in debug mode, the _ProcessPerson_ method looks as follows:

``` csharp
private static void ProcessPerson(Person p)
{
    SparseLogging(p);
    FullLogging(p);
}
```

However, when compiled in release mode:

``` csharp
private static void ProcessPerson(Person p)
{
    FullLogging(p);
}
```

The compiler excludes the invocation of the method completely from the code!

---

## Limitation

There is a limitation to the use of the `Conditional` attribute - as the compiler is removing parts of code, the code left behind cannot rely on the removed code. 

As such, methods have to return `void` to be marked with the `Conditional` attribute.

---

## Notes

Often additional logging is performed locally, while development is taking place - using this attribute the logging can automatically be removed from the output when compiling and deploying for a production environment - a very useful feature to be aware of.

---

## References

[ConditionalAttribute Class](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.conditionalattribute?view=net-6.0)  

<?# DailyDrop ?>41: 30-03-2022<?#/ DailyDrop ?>
