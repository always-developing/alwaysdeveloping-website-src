---
title: "Anonymous type property name inference"
lead: "How anonymous types can infer property names from variable names"
Published: 05/18/2022
slug: "18-anon-type-infer"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - anonymous
    - inference

---

## Daily Knowledge Drop

When creating an anonymous type and setting properties based on other variables, the `name of the property can be inferred from the name of the variable` setting the property.

---

## Example

Below is a very simple example:

``` csharp
int Age = 45;
string Name = "John Doe";

var anonType = new
{
    Age,
    Name
};

Console.WriteLine(anonType);
```

Here, two variables (one int and one string) are being declared with values. An anonymous type is then being declared, using the two variables - `without the name of the properties being set`.

The output is as follows:

``` powershell
    { Age = 45, Name = John Doe }
```

The anonymous type `inferred the name of the properties from the variable names`.

---

The names can also be explicitly set if the name of the variable isn't meaningful:

``` csharp
int Age = 45;
string str_col1 = "John Doe";

var anonType = new
{
    Age,
    FullName = str_col1 // Property name explicitly set
};

Console.WriteLine(anonType);
```

Now the name of the property is not inferred:

``` powershell
    { Age = 45, FullName = John Doe }
```

---

The name inference also works with more complex types (such as classes, and other anonymous types):

``` csharp
int Age = 45;
string Name = "John Doe";

var anonType = new
{
    Age,
    FullName = Name
};

var AnotherAnonType = new
{
    anonType,
    Index = 0
};

Console.WriteLine(AnotherAnonType);
```

With the output now being:

``` powershell
    { anonType = { Age = 45, FullName = John Doe }, Index = 0 }
```


---

## Notes

When working with anonymous types, ensure to name variables accurately and meaningfully. The ability to automatically infer the anonymous type property names will save on additional type, and unnecessary code as well as make the code naming standards more consistent.

---

## References

[StackOverflow comment](https://stackoverflow.com/questions/9033/hidden-features-of-c/212905#212905)  

<?# DailyDrop ?>76: 18-05-2022<?#/ DailyDrop ?>
