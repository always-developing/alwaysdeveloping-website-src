---
title: "Creating scopes with braces"
lead: "Creating scopes with braces without any statements"
Published: 04/13/2022
slug: "13-scope-braces"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dictionary
    - concurrentdictionary

---

## Daily Knowledge Drop

Usually braces `{}` are uses to define the scope of a specific statement (method, if statement, for loop etc) - but braces can also be used to `define a scope without specifying a statement`

---

## Example

Below is a simple code snippet to demonstrate the scopes:

``` csharp

// instantiated in the main outer scope
var outerVariable = 1;

// if statement creates its own scope
if(outerVariable == 1)
{
    // instantiated within the scope of the if statement
    var innerVariable = 100;
    Console.WriteLine($"In the if statement, innerVariable: {innerVariable}");
}

// a scope is created with the use of braces and no specific statement
{
    // cannot be declared here, as a variable of the same name is already 
    // declared in the outer scope
    //var outerVariable = 2; NOT allowed

    // instantiated within the scope
    // same name as used above, and allowed
    var innerVariable = 101;
    Console.WriteLine($"In scoped statement, innerVariable: {innerVariable}");
}

// Cannot be accessed, as the visibility of this variable is confined to 
// the scope in which it was created
//inIfVariable = 102; NOT allowed

Console.WriteLine($"In the main scope, outerVariable: {outerVariable}");
```

--- 

## Notes

While not encouraging the reusing of variables names in the same scope, if it is required, braces can be used to create smaller scopes. This will limit the visibility of variables so that they can be reused within multiple smaller scopes.  
However this should not be abused, as it can make the code harder to read - often it is a better idea to declare a variable to share across all scopes (i.e a method), or convert a smaller scope into its own method.

---

## References

[Five C# Features You Might Not Know](https://auth0.com/blog/five-csharp-features-you-dont-know/)  

<?# DailyDrop ?>51: 13-04-2022<?#/ DailyDrop ?>
