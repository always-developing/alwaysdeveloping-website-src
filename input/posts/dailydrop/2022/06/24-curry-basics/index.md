---
title: "Method currying basics in C#"
lead: "A simple introduction to method currying in C#"
Published: "06/24/2022 01:00:00+0200"
slug: "24-curry-basics"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - currying

---

## Daily Knowledge Drop

`Method Currying` is entails breaking a single method (which takes multiple parameters), into a sequence of single-parameter methods.  
For example, instead of a method which takes 3 parameters `method(a, b, c)`, currying the method would instead turn it into `method(a)(b)(c)`.

This offers a different syntax, which allows for complex composite methods to be built up from simpler methods. Currying is more commonly used in functional programming, but can be utilized in C#.

---

## Currying

The below examples contain a _very simple_ example of currying, but it will demonstrate the basics of how currying a method will break it down into a sequence of single argument methods.

---

### Multiply method

Let's consider a method which `multiples three integers together`, and returns the result. This can be written as follows:

``` csharp
int Multiply(int a, int b, int c)
{
    return a * b * c;
}
```

So far so good.

---

### Multiply Lambda

This _Multiply_ method, can also be written as a `lambda Func` instead of a full method:

``` csharp
Func<int, int, int, int> multiply = (a, b, c) => a * b * c;
```

Now we start with the currying process.

---

### Multiply Curry part 1

As mentioned, `currying` entails breaking down a single method, with three parameters in this example, into a sequence of single-parameters methods. Let's start small and build on it until we have the desired result.

First, we'll change the above _multiply_ lambda to only take one argument: 

``` csharp
Func<int, Func<int, int, int>> curryMultipleBase = a => ((b, c) => a * b * c);
```

It looks a bit complicated, but basically the initial lambda has been updated to instead of having three arguments, it now `takes one argument, and returns a Func<>, which takes 2 arguments, and returns an int`.

Overall, the entire composition of methods still takes 3 parameters. This can now be invoked as follows:

``` csharp
Func<int, Func<int, int, int>> curryMultipleBase = a => ((b, c) => a * b * c);

// invoke the lambda, passing in one value
// getting back a lambda which takes two arguments
Func<int, int, int> result1 = curryMultipleBase(2);

// invoke the lambda which takes two arguments
// and returns an int
int result2 = result1(3, 4);

Console.WriteLine(result2);

```

Or more succinctly expressed as follows:

``` csharp
Func<int, Func<int, int, int>> curryMultipleBase = a => ((b, c) => a * b * c);

// instead of having the intermediate Func<int, int, int>
// just invoke the result directly
int result = curryMultipleBase(2)(3, 4);
Console.WriteLine(result);

```

Both of the above yield a result of `24`.

We've converted a portion to be single parameter, but a portion still has 2 parameters.

---

### Multiply Curry part 2

The _curryMultipleBase_ returns a lambda, which has two parameters - the objective to too only have single parameter methods. Let's convert the two parameter Func into two single parameter Func's:

``` csharp
// Old
Func<int, Func<int, int, int>> curryMultipleBase = a => ((b, c) => a * b * c);

// New
Func<int, Func<int, Func<int, int>>> curryMultiple = a => (b => c => a * b * c;
```

Instead of returning a Func which takes 2 arguments and returns an int, the lambda now returns a `Func, which takes 1 parameter, and returns a Func which takes one parameter and returns an int`.

The new version of the lambda can be invoked as follows:

``` csharp
Func<int, Func<int, Func<int, int>>> curryMultiple = a => (b => (c => a * b * c));

Func<int, Func<int, int>> result1 = curryMultiple(2);
Func<int, int> result2 = result1(3);
int result3 = result2(3);

Console.WriteLine(result3);

```

Or more succinctly expressed as follows:

``` csharp
Func<int, Func<int, int, int>> curryMultipleBase = a => ((b, c) => a * b * c);

// instead of having the intermediate Func(s)
// just invoke the result directly
int result = curryMultiple(2)(3)(4);
Console.WriteLine(result);

```

And that's it! We've successfully converted the initial three parameter method, into a sequence of one parameter methods - `int result = curryMultiple(2)(3)(4);`!

---

### Logging example

Suppose we have to call a _Log_ method, with a message and the type of message to log (Error, Warning or Information). There are a number of techniques to achieve this, especially if the method is called often.

The method could be called directly:

``` csharp
    Log("error", "An Exception occurred");
```

The "issue" with this approach is that the constant type parameter "error" (or "warning" or "info") is repeated with every call.

We could create an `Action` for each log type, which can then be reused:

``` csharp
Action<string> logError = message => Log("error", message);
Action<string> logWarning = message => Log("warning", message);
Action<string> logInfo = message => Log("info", message);

logWarning("Validation failed, skipping record");

// logWarning can be reused
logWarning("An exception occurred, but the record was saved successfully");

```

The constant type parameter is now only specified once, instead of once per call.

Yet another option, is too make use of `method currying`:

``` csharp
Action<string> CurryLog(string type) => message => Log(type, message);

var curryLogInfo = CurryLog("info");
curryLogInfo("Record processed successfully");

// curryLogInfo can be reused
curryLogInfo("Saving record to database");

```

The `currying` approach is very similar to the `Action` technique mentioned above, but it comes down to syntax preference.

---

## Notes

I found the idea of `method currying` very interesting and intriguing, but I'm not sure of its benefits in C# over other techniques. Perhaps a preference for the syntax style is enough of a benefit to use it instead of the "traditional" way. 

Either way though, its useful to have knowledge of this technique, just incase it offers a unique solution to a problem in the future.

---

## References

[Currying and partial application](https://weblogs.asp.net/dixin/lambda-calculus-via-c-sharp-1-fundamentals-closure-currying-and-partial-application)  
[What Is Currying in Programming?](https://towardsdatascience.com/what-is-currying-in-programming-56fd57103431)  

<?# DailyDrop ?>103: 24-06-2022<?#/ DailyDrop ?>
