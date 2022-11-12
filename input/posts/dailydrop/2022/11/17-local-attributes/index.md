---
title: "Local functions and attributes"
lead: "Since C#9, local functions are permitted to be decorated with attributes"
Published: "11/17/2022 01:00:00+0200"
slug: "17-local-attributes"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - attributes
   - local
   - localfunction

---

## Daily Knowledge Drop

When using `local functions`, it is possible to decorate them, as well as their parameters, with `attributes`.


---

## Local function

`Local functions` are methods of a type that are nested in another member. They can only be called from their containing member:

``` csharp
public class Processor
{

    // Containing method
    public void DoProcessing()
    {
        // local function/method
        void PerformInternalLogic()
        {
            Console.WriteLine("Logic being performed...");
        }

        PerformInternalLogic();
    }
}
```

In the above sample, the _PerformInternalLogic_ method is a `local method` nested in the _DoProcessing_ method, and can only be called from within the method.

---

## Attributes

### Method attribute

_Method targeting_ attributes can be used on local functions:

``` csharp
public class Processor
{
    // Containing method
    public void DoProcessing()
    {
        // local function/method
        [Obsolete("This will be deprecated. Not useful for consumers of your method")]
        void PerformInternalLogic()
        {
            Console.WriteLine("Logic being performed...");
        }

        PerformInternalLogic();
    }
}
```

In the above, the `Obsolete` attribute was used to decorate the _PerformInternalLogic_ method (as an indicator that the local method is obsolete and will be removed in future)

---

### Parameter attribute

_Parameter targeting_ attributes can also be used on _local function parameters_:

``` csharp
public class Processor
{
    // Containing method
    public void DoProcessing()
    {
        // local function/method
        [Obsolete("This will be deprecated. Not useful for consumers of your method")]
        void PerformInternalLogic()
        {
            Console.WriteLine("Logic being performed...");
        }

        // local function/method
        void PerformOtherInternalLogic([CallerMemberName] string memberName = "")
        {
            Console.WriteLine($"Logic being performed called from {memberName}");
        }

        PerformInternalLogic();

        PerformOtherInternalLogic();
    }

}
```

Here, the `CallerMemberName` attribute us applied to the parameter of the _PerformOtherInternalLogic_ method.

---

## Notes

A fairly niche use case, but if required, it is useful to know it is possible to add attributes to local functions. 

---

## References

[8. Attributes on Local Functions](https://blog.okyrylchuk.dev/a-comprehensive-overview-of-c-9-features#heading-8-attributes-on-local-functions)  

<?# DailyDrop ?>204: 17-11-2022<?#/ DailyDrop ?>
