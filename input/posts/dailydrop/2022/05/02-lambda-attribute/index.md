---
title: "Attributes on lambda methods"
lead: "Applying method attributes to lambda methods"
Published: 05/02/2022
slug: "02-lambda-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - attribute
    - lambda

---

## Daily Knowledge Drop

Introduced with C#10, attributes can also be applied to `lambda expressions and lambda parameters`.

---

## Attribute

For our example, a method level attribute has been created, which can be used to mark a method for logging (this is just an example, no actual logging will be demonstrated in this post):

``` csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
class LoggingEnabledAttribute : Attribute
{
    // marker attribute to flag this method should be logged
}
```

---

## Attribute application

This attribute, configured to target methods (using another attribute, _AttributeUsage_) can now be applied to any method:

``` csharp
public class AttributeDemo
{
    // Method1 has been flagged for logging
    [LoggingEnabled]
    public void Method1()
    {
        Console.WriteLine("In AttributeDemo => Method1");
    }

    public void ActionMethod(Action action)
    {
        action.Invoke();
    }
}
```

In the above, _Method1_ has had the attribute applied to it. 

## Lambda attribute application

The _AttributeDemo_ class above also has a method called _ActionMethod_, which takes in an Action. This Action can be represented by a lambda:

``` csharp
var ad = new AttributeDemo();

ad.ActionMethod(() => Console.WriteLine("In AttributeDemo => ActionMethod => action"));
```

The _LoggingEnabledAttribute_ can also now be applied to the lambda:

``` csharp
var ad = new AttributeDemo();

ad.ActionMethod(() => Console.WriteLine("In AttributeDemo => ActionMethod => no logging"));

// Logging enabled!
ad.ActionMethod([LoggingEnabled]() => 
    Console.WriteLine("In AttributeDemo => ActionMethod => with logging"));
```

Relevant attributes which target parameters can also be applied to the lambda parameters.

## Attribute confusion

To be able to use attributes on a lambda, some very minor code updates might be required. These are needed to indicate to the compiler what the attribute should be targeting:

``` csharp
// lambda takes no parameters
Action lambda1 = () => { };  

// lambda takes 1 parameter, parenthesis excluded
Action<int> lambda2 = x => x++;

// lambda takes 1 parameter, parenthesis included
Action<int> lambda3 = (x) => x++;
```

The the case of the second example in the above snippet, the lambda with the `parenthesis excluded`, if the attribute was applied, the compiler is `unable to determine` if the attribute should be applied to the method, or to the parameter:

``` csharp
// lambda takes no parameters
// Attribute applied without issue to the method
Action lambda1 = [LoggingEnabled]() => { };  

// lambda takes 1 parameter, parenthesis excluded
// NOT VALID - does the attribute apply to the method, or to the parameter x?
Action<int> lambda2 = [LoggingEnabled]x => x++;

// lambda takes 1 parameter, parenthesis included
// VALID - attribute applies to the method
Action<int> lambda3 = [LoggingEnabled](x) => x++;

// lambda takes 1 parameter, parenthesis included
// VALID - attribute LoggingEnabled applies to the method
// VALID - attribute AttributeForParameter applies to the method
Action<int> lambda4 = [LoggingEnabled]([AttributeForParameter]x) => x++;

```

If the code is using the style without the parenthesis, then to be able to use attributes, it would need to update to include the parenthesis.

---

## Notes

A small, but useful improvement to C#, especially if the application make extensive use of attributes and/or lambdas.

---

## References

[Lambda Improvements: Attributes](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/lambda-improvements#attributes)  

<?# DailyDrop ?>64: 02-05-2022<?#/ DailyDrop ?>
