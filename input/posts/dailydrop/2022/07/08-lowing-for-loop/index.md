---
title: "For loop lowering in C#"
lead: "Discovering how C# lowers for a loop to a while loop"
Published: "07/08/2022 01:00:00+0200"
slug: "08-lowing-for-loop"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - lowing
    - forloop

---

## Daily Knowledge Drop

In C#, a `for loop` is _lowered_ to a `while loop` by the compiler.  

At a lower level, the `for` loop _does not exist_ - it gets translated into a `while` loop by the C# compiler, and then is executed as such.

---

## Lowering

Done by the C# compiler, `lowering` is the process of rewriting high-level language features, into low-level language features in the _same language_.  

There are a number of C# features, which are just _syntactic sugar_ over (often) more complex lower-level features - the compiler will take the high-level feature and _lower_ it to the lower-level feature. 

Lowering is done for a number of reasons, including:
- Working with high-level features, are easier and simpler to work with 
- Optimization

This post is specifically about the `for` loop, not an especially complex feature - however it is more complex than the `while` loop, its lower-level equivalent.

---

## For loop

Using [sharplab.io](https://sharplab.io), one is able to see the lowered code the compiler generates.

Here is an example of some code written in Visual Studio:

``` csharp
public void ForLoopExample() 
{
    for(int i = 0; i < 100; i++)
    {
        Console.WriteLine(i);
    }
}
```

And the lowered code the compiler generates:

``` csharp
public void ForLoopExample()
{
    int num = 0;
    while (num < 100)
    {
        Console.WriteLine(num);
        num++;
    }
}
```

As you can see - the `for` loop is converted to a `while` loop!

Another example, in a previous post we learnt how the _iterator section_ of a for loop could [contain multiple statements](../../06/30-for-multi-operations/). Looking at the lowered code for that example, the `while` loop is controlled only by the _condition section_ of the `for` loop - this is why this portion of the for loop can only contain one statement, while the other sections can contain multiple.

Original C# code:

``` csharp
public void ForLoopExample2() 
{
    int countDown = 100;
    for(int i = 0; i < 100; i++, countDown--)
    {
        Console.WriteLine(i);
    }
}
```

And the lowered code the compiler generates:

``` csharp
public void ForLoopExample2()
{
    int num = 100;
    int num2 = 0;
    while (num2 < 100)
    {
        Console.WriteLine(num2);
        num2++;
        num--;
    }
}
```

---

## Foreach

Just for reference, the `foreach` does not operate the same way as the `for` loop - even though both iterate through a list of items, they do it very differently.

`Foreach` works using the _[GetEnumerator](../../03/03-getenumerator/)_ method on a class, and is not lowered to a `while` loop:

Original C# code:

``` csharp
public void ForEachLoopExample() 
{
    var list = new List<int> { 1, 2, 3, 4, 5, };

    foreach (var item in list)
    {
        Console.WriteLine(item);
    }
}
```

And the lowered code the compiler generates:

``` csharp
public void ForEachLoopExample()
{
    List<int> list = new List<int>();
    list.Add(1);
    list.Add(2);
    list.Add(3);
    list.Add(4);
    list.Add(5);
    List<int> list2 = list;
    List<int>.Enumerator enumerator = list2.GetEnumerator();
    try
    {
        while (enumerator.MoveNext())
        {
            int current = enumerator.Current;
            Console.WriteLine(current);
        }
    }
    finally
    {
        ((IDisposable)enumerator).Dispose();
    }
}
```

While the developer written code for a `for` and `foreach` loop are fairly similar - the lowered code for the `foreach` is a bit more complex than a simple `while` loop.

---

## Notes

While this may not be something one has to worry or think about when coding - its always good to have a general knowledge of how the compiler works, and what its doing (even at a high level), and how the code written impacts the lowered code which gets executed.

---

## References

[sharplab.io](https://sharplab.io/)   

<?# DailyDrop ?>113: 07-08-2022<?#/ DailyDrop ?>
