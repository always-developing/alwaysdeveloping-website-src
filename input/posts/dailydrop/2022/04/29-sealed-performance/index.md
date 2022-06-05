---
title: "Sealed class performance"
lead: "Improve performance by making classes sealed"
Published: 04/29/2022
slug: "29-sealed-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - sealed
    - performance

---

## Daily Knowledge Drop

There are a number of situations in which performance gains can be had by marking a class as `sealed`.

---

## Sealed class

A `sealed` class is a class (marked with the `sealed` keyword) which cannot be inherited from. If the intention is to not have a class available for inheritance (probably the majority of classes by default) the class should be marked as `sealed` to prevent unwanted effects of it being inherited - but there is the benefit of some performance improvements.

---

## Performance gains

The list of performance gains were outlined in the Git issue (link below in references), while performance benchmarking was done by Gérald Barré in his blog post (also linked below in references). Also bear in mind, the performance gains here are measured in nano-seconds, so in most cases would not be obviously apparent without benchmarking and accurate measurement.


### Virtual methods

When calling an overwritten virtual method on a class, marking a class as sealed will result in a `5000% performance improvement`, and approx `250% memory usage improvement`

``` csharp
public class ParentClass
{
    public virtual void Method1() { }
}

public class ChildClass : ParentClass
{
    public override void Method1() { }
}

public sealed class SealedChildClass : ParentClass
{
    // calling this method will result in 50x improvement
    public override void Method1() { }
}
```

This improvement is gained by bypassing the need to use the `vtable` (_virtual function table_ or _virtual method table_). The vtable contains a list of pointers to the virtual methods of a class.
An instance of the class will have a pointer to the table, which is then used when the method is invoked. This is needed as the invocation should call the method on the actual class, not the class referenced. 

In the below example, _Method1_ on _SealedChildClass_ should be called, even though the type was referenced as _ParentClass_

``` csharp
ParentClass entity = new SealedChildClass();
// this should call Method1 on SealedChildClass
// not on ParentClass
entity.Method1();
```

When the compiler is unable to determine the type (in the above example _entity_ could have been changed to reference an instance of _ChildClass_), then the vtable is used. In the case of a sealed class, it cannot be inherited from, so the compiler can determine the type directly and the vtable lookup can be avoided.

### as/is casting

When casting to a sealed class, vs a non sealed class, there is an approximate `1000% performance improvement`.

``` csharp
ParentClass entity = new();

var isNotSealed = entity is ChildClass;
// this is approx 10x faster than the above
var isSealed = entity is SealedChildClass;

//---

class ParentClass { }

class ChildClass : ParentClass { }

sealed class SealedChildClass : ParentClass { }
```

When casting or checking if "_entity is of type ChildClass_", the runtime must check the full hierarchy to see if _entity is of a type which is inherited from ChildClass_. With a sealed class, the hierarchy does not need to be checked, as there will be no hierarchy.

---

### Arrays

Arrays in .NET are covariant ([read more about covariance here)](../../03/18-co-contravariance/). Basically, this means that a child type can be assigned to a parent type, for example:

``` csharp
ParentClass[] parentArray = new ParentClass[10];
SealedChildClass[] sealedArray = new SealedChildClass[10];

// child assigned to parent - covariant check done
parentArray[0] = new ChildClass();

//---

class ParentClass { }

class ChildClass : ParentClass { }

sealed class SealedChildClass : ParentClass { }
```

The covariance has an impact on performance, as the compiler will need to perform a check to ensure the assignment for a _child class to parent class is valid_.
If the class is sealed, a child class cannot be used (as there will never be a child class), the compiler check can be removed and will result in a an approximate `15% performance improvement`.

---

### Converting arrays to Span<T>

Arrays can also be converted to `Span<T>` or `ReadOnlySpan<T>` - for the same reason when assigning arrays, a _covariance check needs to be performed_. Having an array on a sealed type can result in a `200% performance improvement`.

---

## Notes

So should you go to an existing project and now change all classes to `sealed`? Probably not, unless micro-optimizing is required. There are performance gains in some scenarios, but as mentioned, these improvements are on the nano-seconds scale.

However for new development, it might be worth defaulting classes as sealed, and then make a conscious decisions to make it unsealed if inheritance is required.

---

## References

[Performance benefits of sealed class in .NET](https://www.meziantou.net/performance-benefits-of-sealed-class.htm)  
[Analyzer Proposal: Seal internal/private types #49944](https://github.com/dotnet/runtime/issues/49944)

<?# DailyDrop ?>63: 29-04-2022<?#/ DailyDrop ?>
