---
title: "Dispose vs Exception handling"
lead: "Exploring the sequence of event when using a disposable class inside a try-catch block"
Published: "09/01/2022 01:00:00+0200"
slug: "01-disposable-exceptions"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - disposable
   - idisposable
   - exception

---

## Daily Knowledge Drop

When using a class which implements `IDisposable` inside a `try-catch-finally` block, if an exception is thrown in what order are the _Dispose_ and _catch_ block executed?

The order is:
1. _Dispose_ method on the class implementing _IDisposable_
1. _Catch_ block
1. _Finally_ block

Logically this makes sense, as the `IDisposable` class is out of scope when the `catch` block is being executed. The _lowered_ code can also be viewed to get an even deeper understanding of how these two features fit together.

---

## Example

Consider the following class which implements _IDisposable_:

``` csharp
public class DisposableClass : IDisposable
{
    public DisposableClass()
    {
        Console.WriteLine($"In Constructor of {nameof(DisposableClass)}");
    }

    public void Dispose()
    {
        Console.WriteLine($"In Dispose of {nameof(DisposableClass)}");
    }
}
```

As it implements _IDisposable_, it can be used with the `using` statement:

``` csharp
try
{
    // declare instance of the disposable class
    using var disposableInstance = new DisposableClass();

    throw new Exception("Exception or dispose?");

}catch(Exception ex)
{
    Console.WriteLine("An exception occurred");
}
finally
{
    Console.WriteLine("In the finally block");
}
```

Executing the above, results in the following output:
``` terminal
In Constructor of DisposableClass
In Dispose of DisposableClass
An exception occurred
In the finally block
```

_Dispose_ called before the _catch block_.

---

## Lowered

Looking at the `lowered` code (generated using [sharplab.io](https://sharplab.io/)) it becomes more obvious as to why this is the sequence of events:

``` csharp
internal static class <Program>$
{
    private static void <Main>$(string[] args)
    {
        try
        {
            DisposableClass disposableClass = new DisposableClass();
            try
            {
                throw new Exception("Exception or dispose?");
            }
            finally
            {
                if (disposableClass != null)
                {
                    ((IDisposable)disposableClass).Dispose();
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("An exception occurred");
        }
        finally
        {
            Console.WriteLine("In the finally");
        }
    }
}
internal class DisposableClass : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine("Disposing DisposableClass");
    }
}
```

The `using` statement is converted into its own `try-finally` block, with the _Dispose_ being called inside the _finally_.

Looking at the above code, it make entire sense that the sequence of events is:
1. `Constructor` of the _disposable class_
1. The first `finally` block is called, and the `Dispose` method is invoked
1. The `catch` block is called, and the exception is handled
1. The second `finally` block is called

---

## Notes

The sequence of events in this scenario was not something I had considered before - but thinking through what each block of code does (as well as executing the sample code, and looking at the lowered code), the sequence of events makes sense, and is as one would probably expect. However, it is good to get confirmation and gain a better understanding of how one's code might operate under the hood.

---

## References

[Davide Bellone tweet](https://twitter.com/BelloneDavide/status/1547993398853767170)   

<?# DailyDrop ?>151: 01-09-2022<?#/ DailyDrop ?>
