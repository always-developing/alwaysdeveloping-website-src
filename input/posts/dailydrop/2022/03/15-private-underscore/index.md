---
title: "Private variables prefixed with underscore?"
lead: "Why is the convention that private variables should start with an underscore?"
Published: 03/15/2022
slug: "15-private-underscore"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - private
    - underscore

---

## Daily Knowledge Drop

The current convention/standard/best practice when it comes to naming private fields on a class, is to prefix the name with an underscore. Turns out, that this `convention is all due to a misunderstanding`!

---

## Explained

In version 1.0 of the C# language specification, there was a reference to  `this` and `underscore`. From page 213 of the spec:

``` csharp
public class Nested {
    C this_c;
    public Nested(C c) {
        this_c = c;
    }
    public void G() {
        Console.WriteLine(this_c.i);
    }
}
```

At the time, C# didn't have the ability to uniquely identify private class members, so the initial convention was `this_privateMember`, as seen on **line 2, 4 and 7**.

Over time as the language evolved, the `this` portion was dropped, and `_privateMember` was being used. No real reason for this, apart from a misunderstanding by some in thinking that the underscore was the main key in identifying a private class member.

Over time, this has been widely adopted and is now the convention - but in reality the underscore was just used as a separator between `this` and the `privateMember`, to uniquely identify the variable as private.

---

## Notes

Although the convention may have it roots in a misunderstanding, I still prefer it over using `this.privateMember` (or any other convention), so will continue to prefix my private variables with underscore.

---

## References
[Github Issue](https://github.com/hassanhabib/CSharpCodingStandard/issues/6)  
[C# 1.0 specification](https://download.microsoft.com/download/a/9/e/a9e229b9-fee5-4c3e-8476-917dee385062/CSharp%20Language%20Specification%20v1.0.doc)

<?# DailyDrop ?>31: 15-03-2022<?#/ DailyDrop ?>
