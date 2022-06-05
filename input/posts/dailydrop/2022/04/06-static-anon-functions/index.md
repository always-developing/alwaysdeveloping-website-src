---
title: "Static anonymous functions"
llead: "How, when and why to use static anonymous functions"
Published: 04/06/2022
slug: "06-static-anon-functions"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - anonymous
    - function
    - static

---

## Daily Knowledge Drop

When using `anonymous functions` (lambda functions) - in certain use cases `static anonymous functions` can be used instead to improve performance of the application.

---

## Anonymous function

### Code

In the below sample, a function _OutputDatetime_ is called to output the current datetime.  
However the formatting of the output is determined by a _Func (an anonymous function)_ in conjunction with either the _formattedTime_ string or _slimTime_ string (or any other string format which can be specified).

``` csharp
string formattedTime = "The current time is: {0}";
string slimTime = "{0}";

OutputDatetime(inputText => string.Format(formattedTime, inputText));
OutputDatetime(inputText => string.Format(slimTime, inputText));

void OutputDatetime(Func<string, string> func)
{
    Console.WriteLine(func(DateTime.Now.ToString()));
}
```

When the _Console.WriteLine_ is executed on line 9, the _OutputDatetime_ method `doesn't have visibility` of either _formattedTime_ or _slimTime_, which are used by _func_ - they are not within the scope of _OutputDatetime_.

The compiler gets around this by creating a `closure` - [more information can be found here](../../03/16-closures)

---

### Lowered

When the code is lowered y the compiler, a class is created to encapsulate the local values needed by the function, in this case _formattedTime_ and _slimTime_.

``` csharp
[CompilerGenerated]
private sealed class <>c__DisplayClass0_0
{
    public string formattedTime;

    public string slimTime;

    internal string <<Main>$>b__0(string inputText)
    {
        return string.Format(formattedTime, inputText);
    }

    internal string <<Main>$>b__1(string inputText)
    {
        return string.Format(slimTime, inputText);
    }
}

private static void <Main>$(string[] args)
{
    <>c__DisplayClass0_0 <>c__DisplayClass0_ = new <>c__DisplayClass0_0();
    <>c__DisplayClass0_.formattedTime = "The current time is: {0}";
    <>c__DisplayClass0_.slimTime = "{0}";
    <<Main>$>g__OutputDatetime|0_2(new Func<string, string>(<>c__DisplayClass0_.<<Main>$>b__0));
    <<Main>$>g__OutputDatetime|0_2(new Func<string, string>(<>c__DisplayClass0_.<<Main>$>b__1));
}

[CompilerGenerated]
internal static void <<Main>$>g__OutputDatetime|0_2(Func<string, string> func)
{
    Console.WriteLine(func(DateTime.Now.ToString()));
}
```

The important parts to note are:
- A private class is created which contains the two anonymous methods (Func) as methods
- The values required by the methods are declared are values on the class
- The values on the class are set to the required values in the \<Main\>$(string[] args) method

The anonymous functions now have access to the values it requires, which are outside its scope.

---

## Issue at hand

The problem with the above is that the two strings used in the anonymous function need to be captured and stored in the private class. This results in additional allocations which are potentially not required.

C#9 introduced the ability to be able to set an `anonymous function as static` - however static functions are unable to capture state from the local (declaring) function, so any variables the static function uses must be declared as `const`.

If the use case allows for the making the use local variables constant, then the anonymous function can also be made static which will reduced unnecessary memory allocations.

---

## Static anonymous function

### Code

Let's convert the above example to make use of a static anonymous function:

``` csharp
// The two local variables have been declared as const
const string formattedTime = "The current time is: {0}";
const string slimTime = "{0}";

// The anonymous functions passed in has been declared as static
OutputDatetime(static inputText => string.Format(formattedTime, inputText));
OutputDatetime(static inputText => string.Format(slimTime, inputText));

void OutputDatetime(Func<string, string> func)
{
    Console.WriteLine(func(DateTime.Now.ToString()));
}
```

---

### Lowered

Taking a look at the lowered code, the benefits of the static anonymous method can be seen:

``` csharp
[Serializable]
[CompilerGenerated]
private sealed class <>c
{
    public static readonly <>c <>9 = new <>c();

    public static Func<string, string> <>9__0_0;

    public static Func<string, string> <>9__0_1;

    internal string <<Main>$>b__0_0(string inputText)
    {
        return string.Format("The current time is: {0}", inputText);
    }

    internal string <<Main>$>b__0_1(string inputText)
    {
        return string.Format("{0}", inputText);
    }
}

private static void <Main>$(string[] args)
{
    <<Main>$>g__OutputDatetime|0_2(<>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Func<string, string>(<>c.<>9.<<Main>$>b__0_0)));
    <<Main>$>g__OutputDatetime|0_2(<>c.<>9__0_1 ?? (<>c.<>9__0_1 = new Func<string, string>(<>c.<>9.<<Main>$>b__0_1)));
}

[CompilerGenerated]
internal static void <<Main>$>g__OutputDatetime|0_2(Func<string, string> func)
{
    Console.WriteLine(func(DateTime.Now.ToString()));
}
```

This version of the private class, doesn't contain the two string variables - thats two less allocations compared to the non-static version. 

---

## Notes

Wherever possible, `static anonymous functions` should be used over non-static anonymous functions to avoid the unnecessary memory allocations.

---

## References

[Static anonymous functions: New with C# 9](https://paulsebastian.codes/static-anonymous-functions-new-with-c-9)  

<?# DailyDrop ?>46: 06-04-2022<?#/ DailyDrop ?>
