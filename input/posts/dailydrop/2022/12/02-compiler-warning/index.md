---
title: "Compiler Warning CS8981"
lead: "Discovering the new C#11 compiler warning CS8981"
Published: "12/02/2022 01:00:00+0200"
slug: "02-compiler-warning"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - warning
   - compiler

---

## Daily Knowledge Drop

C# 11 introduced a new compiler warning CS8981, which is raised when a type is declared `consisting of only lowercase letters`.

## The warning

Considering the following class declaration:

``` csharp
public class myclass
{
}
```

When using C# 11, this will raise the following warning:

``` terminal
The type name 'myclass' only contains lower-cased ascii characters. 
    Such names may become reserved for the language.
```

The code will still compiled and execute, but the compiler is _warning_ the developer about potential future issues.

---

## The reason

There are a couple of reasons for the warning (and I am sure other's I have not mentioned here):
- Encourages best practices for class declarations, which should be declared using _PascalCase_ as a default
- As explicitly mentioned in the warning, it prevents potential future conflicts with potential reserved language names - this allows future versions of the language to include new keywords, limiting the impact on existing applications

---

## Notes

A small, almost inconsequential language feature - but personally I am big fan of these kinds of initiatives, and hope to see more in future versions. They help guide developers down the "pit of success" path (which not forcing it outright), and they also allow the developers of the language to enhance and grow it, with limited naming conflicts.

---


## References

[The Best C# 11 Feature You Don’t Need](https://newdevsguide.com/2022/11/13/the-best-csharp-11-feature/)  

<?# DailyDrop ?>215: 02-12-2022<?#/ DailyDrop ?>
