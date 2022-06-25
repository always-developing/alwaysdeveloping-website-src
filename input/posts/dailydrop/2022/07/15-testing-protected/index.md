---
title: "Unit testing a protected method"
lead: "How to unit test a protected method on a class"
Published: "07/15/2022 01:00:00+0200"
slug: "15-testing-protected"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - testing
    - unittest
    - protected

---

## Daily Knowledge Drop

To unit test a `protected` method of a class, inheritance and a public method wrapper can be used.

Consider a class, (called, for example, _CustomClass_) with a protected member - a _test_ class (_TestCustomClass_) can be created which inherits from _CustomClass_ Then a public wrapper method created on _TestCustomClass_ can be created, which in turn invokes the `protected` method on _CustomClass_.

This pattern enables the `protected` member to be accessed and tested through the _public test wrapper_ method.

---

## Protected keyword

A quick recap of the `protected` keyword - a `protected member is accessible within its class and by derived class instances`.

For testing, the `""derived class instance""` is the relevent and important piece of information.

---

## Problem

We have a library which performs some business logic, some of it `publicly` exposed, and some of it `protected`:

``` csharp
namespace CustomLibrary;

public class CustomClass
{
    public int PerformPublicLibraryLogic()
    {
        // do some processing
        return 0;
    }

    protected int PerformProtectedLibraryLogic()
    {
        // do processing
        return 0;
    }

}
```

We want to unit test these two methods, so a separate test project is created (a _MSTest_ test project in this example):


``` csharp
namespace CustomApplication.UnitTest;

[TestClass]
public class CustomLibraryUnitTests
{
    [TestMethod]
    // Tests the public method
    public void TestPublic()
    {
        // create an instance and call the public method
        var customclass = new CustomClass();
        var result = customclass.PerformPublicLibraryLogic();

        // check the results
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    // Try test the protected method
    public void TestProtectedBroken()
    {
        var customclass = new CustomClass();
        // THIS WILL NOT COMPILE
        // PerformProtectedLibraryLogic is not accessible
        var result = customclass.PerformProtectedLibraryLogic();
        
        Assert.AreEqual(0, result);
    }
}
```

The above code will `NOT compile`, as the _PerformProtectedLibraryLogic_ is `not accessible from the test project`.


---

## Solution

As mentioned above, the important piece of information related to the `protected` keyword, is that a protected member is accessible from a `derived class instance` - so let's create a derived class.

In the test project:

``` csharp
namespace CustomApplication.UnitTest;

// inherit from the class in question
public class TestCustomClass : CustomClass
{
    // create a public method, which wraps the protected method
    public int WrappedPerformProtectedLibraryLogic()
    {
        // as this code is inside a derived class, it has accessibility
        // to the PerformProtectedLibraryLogic method
        return PerformProtectedLibraryLogic();
    }
}
```

Instead of testing the protected method on the _CustomClass_ directly, it can now be tested through _public_ method on _TestCustomClass_:

``` csharp
[TestMethod]
// Tests the protected method
public void TestProtectedWorking()
{
    // used the TestCustomClass instead of CustomClass 
    var customclass = new TestCustomClass();
    // invoke the wrapper method
    var result = customclass.WrappedPerformProtectedLibraryLogic();

    Assert.AreEqual(0, result);
}
```

What's important here is that the _TestCustomClass_ and _wrapper method_ `add no additional logic or complexity`. We do not want the test class to interfere with the goal of testing the underlying protected method.

## Notes

The technique mentioned here is not a ground-breaking revelation, but it is very useful. Personally, I've recently started writing more unit tests in earnest (after not having done any for 5+ years) this was one of the first issues I encountered, I had not experienced before, which needed to be solved. 

---

<?# DailyDrop ?>117: 15-07-2022<?#/ DailyDrop ?>
