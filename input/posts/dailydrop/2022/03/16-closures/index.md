---
title: "Closures explained"
lead: "What is a closure, and how does it work?"
Published: 03/16/2022
slug: "16-closures"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - closure
    - method
    - function

---

## Daily Knowledge Drop

A **closure** is a particular type of `first class function`, which is `linked to the environment` in which it was declared, and as such can `reference variables in this environment, even if outside the scope` of the function.


---

## First class function

A closure is a `first class function` - this basically means C# treats the function as a data type, and as such can be used as if it were a data type. It can be assigned to a variable, passed as a parameter etc.

``` csharp
// GetFunction is called and the returned function is assigned to variable func1
var func1 = GetFunction();
// func1 is invoked, and the returning string output to the console
Console.WriteLine(func1());

// A method is defined, which returns a Func<string>. Func\<string\> 
// is a shortcut for a method, which takes no parameters and returns a string. 
// So the GetFunction method, will return another method (as a variable), 
// which takes no parameters and returns as string
Func<string> GetFunction()
{
    // The Func<string> is defined, an anonymous method, 
    // which takes no parameters and returns a string
    return () => "String created by GetFunction";
}
```

The output is as follows:

``` powershell
    String created by GetFunction
```

---

## Variables outside scope

Looks look at some more examples, this time where the function `uses a variable outside of its scope`:

``` csharp
var func2 = GetFunction2();
Console.WriteLine(func2());

var func3 = GetFunction3(17);
Console.WriteLine(func3());

Func<string> GetFunction2()
{
    int intValue = 12;

    return () => $"String created by GetFunction, value of {intValue}";
}

Func<string> GetFunction3(int intValue)
{
    return () => $"String created by GetFunction, value of {intValue}";
}
```

Both of the methods, _GetFunction2_ and _GetFunction3_ return a _Func\<string\>_ which makes use of the intValue variable.  
When the _Func_ is finally invoked, the variable _intValue_ is no longer in scope of the Func (it was defined inside the scope of _GetFunction2_ and _GetFunction3_, which are both now out of scope) - however the value is accessed and output correctly - this is `a closure`.

``` powershell
    String created by GetFunction, value of 12
    String created by GetFunction, value of 17
```

As mention in the intro, the `Closure` is `linked to the environment` in which it was declared, and as such has access to variables in that environment (intValue), even if outside of its direct scope.

---

## How it works

So what does the compiler do to make this work?  
We can use [sharplab.io](https://sharplab.io) to see exactly how this code is lowered:

``` csharp
var func2 = GetFunction2();
Console.WriteLine(func2());

Func<string> GetFunction2()
{
    int intValue = 12;

    return () => $"String created by GetFunction, value of {intValue}";
}
```


The lowered code is as follows:

``` csharp
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
[CompilerGenerated]
internal static class <Program>$
{
    private sealed class <>c__DisplayClass0_0
    {
        public int intValue;

        internal string <<Main>$>b__1()
        {
            return string.Format("String created by GetFunction, value of {0}", intValue);
        }
    }

    private static void <Main>$(string[] args)
    {
        Console.WriteLine(<<Main>$>g__GetFunction2|0_0()());
    }

    internal static Func<string> <<Main>$>g__GetFunction2|0_0()
    {
        <>c__DisplayClass0_0 <>c__DisplayClass0_ = new <>c__DisplayClass0_0();
        <>c__DisplayClass0_.intValue = 12;
        return new Func<string>(<>c__DisplayClass0_.<<Main>$>b__1);
    }
}
```

Obviously this can get more complicated and complex depending on the use case, but essentially what the compiler is doing is as follows:
- A "dummy" class (<>c__DisplayClass0_0) is created which contains the variable required, and the _Func_ is converted to a method on the class
- A method is created (\<\<Main>$>g__GetFunction2|0_0) which instantiates the above class, sets the variable value and then invokes the method (which was the _Func_)
- The above method is invoked, creating the class, setting the value and outputting the result

---

## Variable not value

One final example, show that the `closure uses the variable, not the value`:

``` csharp
int localInt = 100;

// A Func is defined, which outputs the value of _localInt_. 
// At the time the function is created, the `localInt value is 100`
Func<string> localFunc = () => $"The value of localInt is: {localInt}";

localInt = 150;

// The Func is invoked, with the `localInt value at 150`
Console.WriteLine(localFunc());
```

The output:

``` powershell
    The value of x is: 150
```

As mentioned, the closure uses the variable (which can change value) and not the value when it was created.

---

## Notes

`Closures` are easy to use and implement, but this is enabled by the compiler doing lots of work behind the scenes. They are another useful tool to be aware of when coding and thinking through how the code might fit together.

---

## References
[A Simple Explanation of C# Closures](https://www.simplethread.com/c-closures-explained/)  
[How to use closures in C#](https://www.infoworld.com/article/3620248/how-to-use-closures-in-csharp.html)

<?# DailyDrop ?>32: 16-03-2022<?#/ DailyDrop ?>
