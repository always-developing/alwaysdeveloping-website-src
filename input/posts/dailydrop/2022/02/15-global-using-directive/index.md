---
title: "Aliases with global using directives"
lead: "Create an alias with C#10 global usings"
Published: 02/15/2022
slug: "15-global-using-directives"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - global
    - using
    - directive
    
---

## Daily Knowledge Drop

With the introduction of `global usings` in C#10, it is now also possible to defined a `global alias` to have a shortcut to a specific type, across an entire project.

---

## Global usings

A quick intro to `global usings` - In C#10 the concept of global usings was introduced. It allows for a `using` statement to be prefixed with `global`, which then includes that _using_ in all files automatically when compiled.

Assume we have a file called **GlobalUsings.cs**:

``` csharp
global using System.Threading.Tasks;
```

In the Program.cs:

``` csharp
// No using System.Threading.Tasks here
Console.WriteLine("About to delay");
Task.Delay(1000);
Console.WriteLine("Delay finished");
```

Here we can include **all** includes needed for the project in one file (**GlobalUsings.cs**), and keep all other files free of _usings_.

---

## Global alias

This global using functionality can be leverage to create global aliases.

Assume we are writing an application for a library - we need to incorporate the [Dewey Decimal System](https://en.wikipedia.org/wiki/Dewey_Decimal_Classification) through-out the application. The Dewey Decimal System is basically used to links book(s) to a specific shelf in the library, so they can be easily found - so we decide to use a `Dictionary<string,List<string>>` to represent this.  
The dictionary key will be the _shelf reference_ and the dictionary value will be the _list of book titles_ on specific shelf

**Note:** There are a number of ways to represent the Dewey Decimal System data, and a `Dictionary<string,List<string>>` is not necessarily the best or most performant, it is just used for demo purposes.

Throughout code, if we wanted to present this, we could do the following:

``` csharp
var hds = new Dictionary<string,List<string>>();
// OR
Dictionary<string, List<string>> hds1 = new Dictionary<string, List<string>>();
```

This is a relatively long type to type out the entire time and to clutter up the code - an alternative is to create a `global alias` for this type.

In the **GlobalUsings.cs** file we create an alias called `dewey` and alias it to `System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>`

``` csharp
global using dewey = System.Collections.Generic.Dictionary<string, 
    System.Collections.Generic.List<string>>;
```

Now in our application, in any place we want to use `System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>` we can instead use the alias. Its defined as `global` so no additional _usings_ are required:

``` csharp
var hds = new dewey();
```

We can now operate on the `dewey` type as we would a `System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>`.  

The alias _dewey_ is pointing to the type _System.Collections.Generic.Dictionary\<string, System.Collections.Generic.List\<string\>\>_ **they are the same type, just with different names**.

---

## Conclusion

The concept of an `alias` is not new to C#, however the new ability to make it global to the entire project makes it a very convenient tool to create shortcuts, especially for the longer named types.

---

## References
[Global Using Directives](https://benbowen.blog/post/two_decades_of_csharp_vi/#global_using_directives)


<?# DailyDrop ?>11: 15-02-2022<?#/ DailyDrop ?>
