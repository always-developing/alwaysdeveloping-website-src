---
title: "Capture exception state"
lead: "Using ExceptionDispatchInfo to capture exception state"
Published: 05/05/2022
slug: "05-eventdispatchinfo"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - exception
    - exceptiondispatchinfo

---

## Daily Knowledge Drop

When catching an exception, the `ExceptionDispatchInfo` class can be used to capture the state of the exception. This can then later be used to throw the original exception, preserving its original state.

---

## Use case

The `ExceptionDispatchInfo` class is used in the situation where an exception occurs and is caught, but _additional_ processing has to occur before the exception can be thrown up the call stack. 

The following examples will clear when and why to use `ExceptionDispatchInfo`.

## Using Exception

First we'll look at an example `without using ExceptionDispatchInfo`.

``` csharp
NullReferenceException tempException = null;

try
{
    List<int> nullList = null;

    nullList.Add(100);
}
catch(NullReferenceException exception)
{
    tempException = exception;
}

// Do additional processing here
// maybe save information to database
// perhaps some logging

if(tempException != null)
{
    // Maybe do some cleanup processing here

    throw tempException;
}
```

In this sample, an exception occurs and is caught, but additional processing needs to be done before exiting the function. Here the exception is saved into a variable (_tempException_), which is then thrown when the additional processing has been done.

The output of the above highlights the issue with this approach:

``` powershell
Unhandled exception. System.NullReferenceException: 
        Object reference not set to an instance of an object.
   at Program.<Main>$(String[] args) in 
    C:\Development\Blog\ExceptionDispatchInfoDemo\
            ExceptionDispatchInfoDemo\Program.cs:line 22
```

According to the output the exception occurred on line 22. **This is NOT accurate** - the exception occurred on line 7, but when the exception is thrown on line 22, the state of the original exception is not being preserved. 

Using this approach, its difficult to track down the actual root cause of the exception. `ExceptionDispatchInfo` helps solve this problem.

---

## Using ExceptionDispatchInfo

This sample is very similar to the above one, except the exception is captured into an instance of `ExceptionDispatchInfo` instead of saved into a variable.

``` csharp
using System.Runtime.ExceptionServices;

ExceptionDispatchInfo exceptionDispatchInfo = null;

try
{
    List<int> nullList = null;

    nullList.Add(100);
}
catch (NullReferenceException exception)
{
    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
}

// Do additional processing here
// maybe save information to database
// perhaps some logging

if (exceptionDispatchInfo != null)
{
    // Maybe do some cleanup processing here

    exceptionDispatchInfo.Throw();
}
```

When the exception occurs, it is captured into the instance of `ExceptionDispatchInfo`. Then on line 24, the instance of `ExceptionDispatchInfo` is used to throw the captured exception.

The output now looks as follows:

``` powershell
Unhandled exception. System.NullReferenceException: 
        Object reference not set to an instance of an object.
   at Program.<Main>$(String[] args) in C:\Development\Blog\
        ExceptionDispatchInfoDemo\ExceptionDispatchInfoDemo\Program.cs:line 9
--- End of stack trace from previous location ---
   at Program.<Main>$(String[] args) in C:\Development\Blog\
        ExceptionDispatchInfoDemo\ExceptionDispatchInfoDemo\Program.cs:line 24
```

Here we can see that the exception originated on line 9 - we now have accurate information, allowing the exception root cause to easily be traced!

---

## Notes

Having to always do additional processing after an exception is caught, but before the exception is thrown, is not a very common use case. However when it is required, the `ExceptionDispatchInfo` class can easily be used to ensure no valuable exception information is lost.

---

## References

[11. ExceptionDispatchInfo](https://www.automatetheplanet.com/top-15-underutilized-features-dotnet/#11)  

<?# DailyDrop ?>67: 05-05-2022<?#/ DailyDrop ?>
