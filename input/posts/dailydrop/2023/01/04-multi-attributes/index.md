---
title: "Combining multiple attributes"
lead: "Combining multiple attributes into a single line"
Published: "01/04/2023 01:00:00+0200"
slug: "04-multi-attributes"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - attribute
   - attributes


---

## Daily Knowledge Drop

When multiple attributes are applied to a program entity, the attributes can either be `specified on separate lines, or on a single line`.


---

## Multiline

In this example, a _class_ is used to demonstrate an entity having multiple attributes, but the same applies to other relevent program entities:

``` csharp
[Serializable]
[Obsolete]
[DebuggerDisplay("Id={Id}, Name={Name}")]
public class MultiAttributeClass
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

Three attributes added to the class - the actual attributes themselves are not important, it could be been any number of any attributes.

---

## Single line


The multiple attributes can also be applied as follows though:

``` csharp
[Serializable, Obsolete, DebuggerDisplay("Id={Id}, Name={Name}")]
public class MultiAttributeClass
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

Does this reduce the number of lines in code? Yes
Does it make it more difficult to read? Potentially
Is this recommended? Probably not. But you can if you want


---

## Notes

A very small knowledge drop today, but something I was not aware was possible. As mentioned, as a general rule I would argue this make the code slightly less readable - however, some might find this code cleaner and more readable.

---


## References

[Reddit Post](https://www.reddit.com/r/csharp/comments/zk8oyj/did_you_know_you_could_combine_method_or_class/)  

<?# DailyDrop ?>227: 04-01-2023<?#/ DailyDrop ?>
