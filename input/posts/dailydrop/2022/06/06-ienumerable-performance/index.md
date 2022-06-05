---
title: "IEnumerable performance cost"
lead: "Exploring the cost of virtualization with IEnumerable"
Published: "06/06/2022 01:00:00+0200"
slug: "06-ienumerable-performance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - ienumerable
    - performance

---

## Daily Knowledge Drop

Using `IEnumerable<>` instead of a concrete implementation (List or an array for example) can make the code more usable and concise, but comes with a performance cost.

If performance is critical, it will be worth performing a `type check` and handling accordingly.

---

## Generic method

We want to create a method which takes a collection of integers, sums them up and returns the result. We are not entirely sure what this collection will be (a list, an array) so we want to make the method generic.

We might end up with something like this:

``` csharp
public int SumItems(IEnumerable<int> enumerable)
{
    var runningSum = 0;

    foreach(var item in enumerable)
    {
        runningSum += item;
    }

    return runningSum;
}
```

This is fairly straightforward - the method takes an `IEnumerable<int>` as a parameter, will iterate through each item keeping a running total, and then return the total at the end.

Lets look at the performance of this method using various implementations of `IEnumerable<int>`.

``` csharp
IEnumerable<int> enumerableData = Enumerable.Range(0, 10000);
IEnumerable<int> listData = new List<int>(Enumerable.Range(0, 10000));
IEnumerable<int> arrayData = Enumerable.Range(0, 10000).ToArray();
```

Here three different implementations are declared and `populated with 10000 items`. We have:
- an IEnumerable\<int\>
- a List\<int\>
- an int array

As all three of these implement IEnumerable, they can all be passed to the _SumItems_ method as a parameter.

Using `BenchmarkDotNet` our method is called using each of the IEnumerable types:

|         Method | IEnumerable |      Mean |     Error |    StdDev |
|--------------- |------------ |----------:|----------:|----------:|
|       SumItems | IEnumerable | 37.400 us | 0.4611 us | 0.4313 us |
|       SumItems |        List | 57.201 us | 0.4353 us | 0.3635 us |
|       SumItems |       int[] | 37.226 us | 0.3353 us | 0.2972 us |

As one can see, the `IEnumerable and the int array are comparable`, with the `List considerably slower` (relatively).

---

## Type checking

Next, let's modify the _SumItems_ method to check the type of IEnumerable, cast to that type, and then iterate on the specific type instead of IEnumerable.

This could have also been done with method overloading, but with this approach all the code is kept in a single method:


``` csharp
public int SumListChecked(IEnumerable<int> enumerable)
{
    var runningSum = 0;

    // check for List
    if(enumerable is List<int> list)
    {
        foreach (var item in list)
        {
            runningSum += item;
        }
        return runningSum;
    }

    // check for array
    if (enumerable is int[] array)
    {
        foreach (var item in array)
        {
            runningSum += item;
        }
        return runningSum;
    }

    // all others
    foreach (var item in enumerable)
    {
        runningSum += item;
    }

    return runningSum;
}
```

Here, the type of IEnumerable parameter _enumerable_ is checked, if its a `List` or `Array`, then the IEnumerable is cast to that type, and the foreach loop is done on the `cast type` not on the original IEnumerable.

Running the benchmarks for both methods, yields the following results:

|         Method | IEnumerable |      Mean |     Error |    StdDev |
|--------------- |------------ |----------:|----------:|----------:|
|       SumItems | IEnumerable | 37.400 us | 0.4611 us | 0.4313 us |
| SumListChecked | IEnumerable | 34.846 us | 0.3551 us | 0.3148 us |
|       SumItems |        List | 57.201 us | 0.4353 us | 0.3635 us |
| SumListChecked |        List |  7.523 us | 0.0806 us | 0.0754 us |
|       SumItems |       int[] | 37.226 us | 0.3353 us | 0.2972 us |
| SumListChecked |       int[] |  3.580 us | 0.0213 us | 0.0189 us |


`IEnumerable is effectively the same`, as the code is the same between the two method, however `List and Array are considerably quicker` when performing the iterating on the concrete type and not IEnumerable.

## Notes

While this type of micro-optimization will probably not be required or noticeable in most use cases, its worth knowing about the trade-offs which come with each approach.

- `Just using IEnumerable`: simplest, most maintainable code, but slowest in most cases
- `Type checking`: less maintainable, but better performance in some cases
- `Method overloading`: even less maintainable, as the same/similar code will be in multiple methods but better performance in some cases

---

## References

[The cost of virtualization in .NET can be surprising](https://leveluppp.ghost.io/content/images/size/w1000/2021/07/net_virt1.png)  

<?# DailyDrop ?>89: 06-06-2022<?#/ DailyDrop ?>
