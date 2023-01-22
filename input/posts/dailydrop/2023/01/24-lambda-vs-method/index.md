---
title: "LINQ lambda vs method group"
lead: "Comparing lambda and method groups when using LINQ"
Published: "01/24/2023 01:00:00+0200"
slug: "24-lambda-vs-method"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - lambda
   - methodgroup
   - performance

---

## Daily Knowledge Drop

When using an _expression_ with LINQ, `a lambda should be preferred over a method group` as the performance is slightly better. Lambda expressions can be cached by the runtime resulting in the increased performance.

---

## Examples

In the below example we'll be filtering a collection of integers, to return only the values which are _greater than 100_.

The _Where_ method on `IEnumerable<int>` accepts a `Func<int, bool>` - this can be defined as an actual method, or as a lambda method. We'll have a look at each technique, and then compare performance.

---

### Method group

With a collection of integers, to filter using the _Where_ method with a `method group`, a method need to be defined which _accepts an int and returns a bool_:

``` csharp
// simple method which accepts an int
// and returns true if the value is 
// greater than 100
static bool GreaterThan100(int value)
{
    return value > 100;
}
```

This method can then be used in a _Where_ method call:


``` csharp
// collection of 100 integers
IEnumerable<int>? items = Enumerable.Range(50, 150);

// filter using a method group
IEnumerable<int>? filteredItems = items.Where(GreaterThan100);
```

---

### Lambda

With the `lambda technique` the separate defined method is invoked manually with the parameter specified:

``` csharp
IEnumerable<int>? items = Enumerable.Range(50, 150);

// "manually" call the method, sending the int value
// "manually" to the method
var filteredItems1 = items.Where(i => GreaterThan100(i));
```

Both techniques will yield the same results, and on the surface look (and are) very similar. However, next let's look at the performance of each technique to see the main difference.

---

## Performance

For the performance benchmarking, the following _lambda_ and _method group_ were tested:

``` csharp
public  class Benchmarks
{
    private IEnumerable<int> items = Enumerable.Range(1, 10000);

    [Benchmark(Baseline = true)]
    public List<int> MethodGroup() => items.Where(IsDivisibleBy5).ToList();

    [Benchmark]
    public List<int> Lambda() => items.Where(i => IsDivisibleBy5(i)).ToList();

    private static bool IsDivisibleBy5(int i) => i % 5 == 0;
}
```

- a collection of _10000 integers_ was used
- each item was checked to determine if it was _division by 5 or not_

The results:

|      Method |     Mean |    Error |   StdDev | Ratio | RatioSD |
|------------ |---------:|---------:|---------:|------:|--------:|
| MethodGroup | 75.04 us | 1.483 us | 2.079 us |  1.00 |    0.00 |
|      Lambda | 66.44 us | 1.306 us | 2.146 us |  0.89 |    0.04 |

From the results, we can see that the `lambda technique is approx. 10% faster than the method group technique`.

---

## Notes

The difference between the two methods is this example (10%) may seem fairly significant, but the timescale is in _nanoseconds_ - a 10% difference at this scale will not be noticeable. However, depending on the collection size and the complexity of the calculation, the difference could be more noticeable.  
In short - for most scenarios using either technique will be fine, however if performance is an issue, or there is a specific bottleneck, then consider explicitly using the lambda technique.

---

## References

[Lambda vs method group](https://linkdotnet.github.io/tips-and-tricks/advanced/lambda_methodgroup/)  

<?# DailyDrop ?>241: 24-01-2023<?#/ DailyDrop ?>
