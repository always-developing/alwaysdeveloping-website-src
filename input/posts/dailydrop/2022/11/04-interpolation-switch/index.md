---
title: "Switch expression and string interpolation"
lead: "Switch expressions inside string interpolation expression with C#11"
Published: "11/04/2022 01:00:00+0200"
slug: "04-interpolation-switch"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - string
   - interpolation
   - switch
   - .net7

---

## Daily Knowledge Drop

C# 11 is introducing the ability to `use switch expressions inside a string interpolation expression`. 

This allows for more simplified code, as instead of creating a separate method to be used once-off when generating the string, all the _string building_ logic can be contained in a single place.

---

## Use case

In our use case, we want to convert a numerical grade (e.g. 89%), to a string representation ("A" in this case). We also want to be able to output both of these values in a string.

---

### Separate expression - pre C# 11

Prior to C# 11 it was not possible for _interpolation expressions_ to be _switch expressions_ directly - the expression had to be a separate method:

``` csharp
// method which converts a numeric grade value
// to a string representation
public string GetGradeAsString(int grade) => grade switch
{
    > 90 => "A+",
    > 80 => "A",
    > 70 => "B",
    > 60 => "C",
    > 50 => "D",
    > 40 => "E",
    _ => "F"
};

var studentGrade = 89;

// using string interpolation, and calling the method defined
Console.WriteLine($"{studentGrade} converted to " +
    $"a string is {GetGradeAsString(studentGrade)}");
```

There is nothing wrong with this approach, and it will continue to work with C# 11 - however the _issue_ here, is that if the conversion is only ever required once (when outputting the data), a method was required to be defined.

Is defining a method a big deal? No
Will it create massive overhead? No
Is there a slightly easier way in C# 11? Yes.

---

### Embedded expression - C# 11

C# 11 allows for `switch expressions to be used directly in interpolated strings`:

``` csharp
var studentGrade = 89;

// a separate method is not required
Console.WriteLine($"{studentGrade} converted to a string is {
    studentGrade switch
    {
        > 90 => "A+",
        > 80 => "A",
        > 70 => "B",
        > 60 => "C",
        > 50 => "D",
        > 40 => "E",
        _ => "F"
    }}");
```

The same outcome as below, just slightly more concise and contained code. 

---

## Notes

This is not a massive enhancement, but it does help reduce unnecessary ceremony (creating a method) when not required - ultimately resulting in cleaner, more concise and quicker to write code when the use case applies.

---

## References

[What’s new in C# 11? Dev friendly features](https://tomaszs2.medium.com/c-11-wants-to-be-your-friend-db4a31ed9710)  

<?# DailyDrop ?>195: 04-11-2022<?#/ DailyDrop ?>
