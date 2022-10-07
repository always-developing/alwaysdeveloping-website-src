---
title: "File access modifier"
lead: "Discovering the file access modifier in C#11"
Published: "10/25/2022 01:00:00+0200"
slug: "25-file-modifier"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - .net7
   - filemodifier

---

## Daily Knowledge Drop

A new access modifier is being introduced with C#11, the `file` modifier - this will limit the accessibility of the type to the _file in which it is declared_.

---

## Modifier recap

A quick recap of the existing access modifiers, along with the new `file` modifier (as they relate to _classes_).

Existing modifiers:
- `private` - the class is only accessible inside the class in which it was defined
- `public` - the class is accessible from everywhere in the project
- `protected` - the class is accessible from within the class and all types which derive from the class
- `internal` - the class is accessible from within its own assembly, but not other assemblies

The new modifier:
- `file` - the class is accessible only from within the _file_ in which it was defined

---

## Example

When a class is defined, the `file` modifier is applied (as opposed to public, private, etc.)

``` csharp
file class Class1
{
    public string Name { get; set; }
}
```

This class now cannot be used anywhere outside the file in which is was defined. Trying to use in a different class defined in a different file, will result in a compilation error:

``` csharp
public class Class3
{
    public Class3()
    {
        // NOT allowed
        Class1 ca = new Class1();
    }
}
```

```terminal
The type or namespace name 'Class1' could not be found
```

However, as mentioned, it can be used in the same file:

``` csharp
// all in the same physical file
file class Class1
{
    public string Name { get; set; }
}

file class Class2
{
    public Class2()
    {
        // Totally okay
        Class1 c1 = new Class1();
    }
}
```

The `file` access modifier can be applied to `classes, records, structs and enums`, but cannot be used for _methods_ or _properties_.

---

## Why?

This functionality was primarily introduced to assist `source generator` authors. When a _source generator_ generates a class, its tricky to ensure it's not going to conflict with a file already in the consumer's code base. The `file` modifier helps solves this by ensuring (where specified) the the _source generated_ file is only visible where defined, and not in conflict with any developer specified class name.


---

## Lowered code

One can use [sharplab.io](https://sharplab.io) to see how the `file` keyword is lowered:

Original code:

``` csharp
file class FileScopedClass { }
```

Lowered code:

``` csharp
internal class <_>FD2E2ADF7177B7A8AFDDBC12D1634CF23EA1A71020F6A1308070A16400FB68FDE__FileScopedClass { }
```

From this we can see that the `file` keyword is translated into an `internal class with a uniquely generated name`, as to not conflict with any existing name

---


## Notes

While the intended target audience for this feature is very narrow (source generator authors), it can be leveraged by anyone if the use case calls for it. Even then, it may not get wide usage in general applications - however it is useful to know it exists and is an option if required.

---

## References

[What is the NEW "file" keyword of C# 11?](https://www.youtube.com/watch?v=xm8eQenL7wA)  

<?# DailyDrop ?>189: 25-10-2022<?#/ DailyDrop ?>
