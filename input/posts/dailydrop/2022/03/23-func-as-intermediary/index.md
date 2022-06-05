---
title: "Using Func<> as an intermediary"
lead: "How Func or Action can be used as an intermediary when calling methods"
Published: 03/23/2022
slug: "23-func-as-intermediary"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - func
    - methods
    - invoke

---

## Daily Knowledge Drop

A `Func (or Action) can be used as an intermediary` to keep code cleaner when dealing with multiple methods or delegates of the same signature.

---

## The setup

The root of this post stems from a real-world situation I'd encountered. The examples below will be a simplified example of the situation but the setup is as follows:

- A handler is obtained from the dependency injection container
- A collection of none or many interceptors are obtained from the dependency injection container
- If no interceptors are obtained, then Invoke a _HandleOperation_ method on the handler
- If any interceptors are obtained, then build up a "pipeline" of all interceptors and then the handler at the end of the pipeline.
- Each interceptor would perform any logic it might need to, and proceed to the next interceptor or handler, if at the end of the pipeline (similar to the ASPNET Core middleware pipeline functions)

---

## The Code

### Non-working example

Below is a simplified version of the above situation. 

If there is an `Interceptor` supplied, then its _InterceptValue_ method is called, otherwise the `Handler` _HandleOperation_ method is called:

This will **NOT compile**:

``` csharp
void PerformOperation(int x, IInterceptor interceptor = null)
{
    var handler = new Handler();

    var start = interceptor != null ? interceptor.InterceptValue : handler.HandleOperation;

    Console.WriteLine($"Log: About to perform the operation");

    var result = start.Invoke(x);

    Console.WriteLine($"Log: The result of the operation is: {x}");
}
```

The error originating on line 5 of the above code is: `Type of conditional expression cannot be determined because there is no implicit conversion between 'method group' and 'method group'`

Even though both methods, _InterceptValue_ and _HandleOperation_ have the same signature, they are considered completely different types.  
As per the error message, the compiled can't determine the type of _var start_ because the two methods are different types and either could be assigned to `var start`.

To fix this, the compiler need to be explicitly how to convert the two different methods to one common underlying type.

---

### Working example

To tell the compiler how to convert the two different methods to the same common type, the `var` just needs to be replaced with the common type.

The commonality between _InterceptValue_ and _HandleOperation_ is that they are both methods, which accept one int parameter, and return an int - luckily C# has a way to represent a method as a variable, using`Func<int, int>` (in this example).

**For reference** - just like an int represents a numerical value, a Func represents a method value. The generic parameters \<int, int\> refer to the parameter type and return type of the method.


``` csharp
void PerformOperation(int x, IInterceptor interceptor = null)
{
    var handler = new Handler();

    Func<int, int> start = interceptor != null ? 
        interceptor.InterceptValue : handler.HandleOperation;

    Console.WriteLine($"Log: About to perform the operation");

    var result = start.Invoke(x);

    Console.WriteLine($"Log: The result of the operation is: {x}");
}
```

Above `declares a variable of type "Method", a method which takes one parameter of type int and returns an int - and the value of this variable is either InterceptValue or HandleOperation`. This Func (method) is then invoked, with the parameter value being passed in.

This will now compile and work!

---

## Notes

Using `Func` or `Action` allow for the reduction of methods, with the same signatures, to a common type - this is useful in the situation where one might have to run through a long list of logic to determine which method to invoke. The method can be treated as a variable and assigned and reassigned as the logic is executed.

---

<?# DailyDrop ?>36: 23-03-2022<?#/ DailyDrop ?>
