---
title: "Lambda vs local function performance"
lead: "Exploring the performance of a lambda function vs a local function"
Published1: "06/27/2022 01:00:00+0200"
slug: "27-lambda-vs-local"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - lambda
    - local

---

## Daily Knowledge Drop

A `local function` will out perform a `lambda function` by a significant margin, which can make a different especially if the function is called from within a loop.

---

## Examples

The use case is straight forward: `iterate through a list of 500 integers, and if the number is larger than a supplied value (y), add it to a running total`.

Below we look at a number of different ways to perform the `if the number is larger than a supplied value (y)` check, and then benchmark the performance of each technique.


### Lambda

The first method is to use a `lambda function` to perform the _where_ check:

``` csharp
public int Lambda(int y)
{
    int runningTotal = 0;

    // define the lambda to compare two values
    // and return true or false based on the results
    Func<int, bool> whereFilter = x => x > y;

    for(int i = 0; i < 500; i++)
    {
        // call the lambda
        if(whereFilter(i))
        {
            runningTotal += i;
        }
    }

    return runningTotal;
}
```

A lambda function is defined which compares two values, and is then invoked to compare the iteration value with the supplied value.

---

### Local function

Next, instead of using a lambda, we will define a `local function` to perform the _where_ check:

``` csharp
public int LocalFunction(int y)
{
    int runningTotal = 0;

    // define the local function expression
    // with the same functionality as the above lambda
    bool whereFilter(int x) => x > y;

    for (int i = 0; i < 500; i++)
    {
        // invoke the local function
        if (whereFilter(i))
        {
            runningTotal += i;
        }
    }

    return runningTotal;
}
```

A local function, as with the lambda, is defined which compares two values, and is then invoked to compare the iteration value with the supplied value.

---

### Function

Next, the local function will be replaced with a normal `function` to perform the _where_ check:

``` csharp
public int Method(int y)
{
    int runningTotal = 0;

    for (int i = 0; i < 500; i++)
    {
        // call the separate expression-bodied method
        if (whereFilterMethod(i, y))
        {
            runningTotal += i;
        }
    }

    return runningTotal;
}

// expression bodied method to compare two values
private bool whereFilterMethod(int x, int y) => x > y;
```

The method body is the same as the local function, but defined outside the method in question (_Method_ in this example).

---

### Operation

Lastly, instead of using any variation of method, the operation will be put directly inline:

``` csharp
public int Operation(int y)
{
    int runningTotal = 0;

    for (int i = 0; i < 500; i++)
    {
        // straight compare of the two value
        if (i > y)
        {
            runningTotal += i;
        }
    }

    return runningTotal;
}
```

No method defined here, just the straight comparison.

---

## Benchmarks

Finally let's benchmark all of the above techniques. **BenchmarkDotNet** was used to benchmark each technique using two parameters, `17` and `472`, to give representation for the lower and upper bounds of the loop.

|        Method |   y |     Mean |    Error |   StdDev | Ratio |
|-------------- |---- |---------:|---------:|---------:|------:|
|        Lambda |  17 | 974.7 ns | 13.68 ns | 12.79 ns |  1.00 |
| LocalFunction |  17 | 135.6 ns |  1.51 ns |  1.34 ns |  0.14 |
|        Method |  17 | 136.2 ns |  2.30 ns |  2.15 ns |  0.14 |
|     Operation |  17 | 135.4 ns |  0.86 ns |  0.67 ns |  0.14 |
|               |     |          |          |          |       |
|        Lambda | 472 | 854.0 ns |  9.61 ns |  8.99 ns |  1.00 |
| LocalFunction | 472 | 255.0 ns |  3.85 ns |  3.42 ns |  0.30 |
|        Method | 472 | 263.0 ns |  2.95 ns |  2.76 ns |  0.31 |
|     Operation | 472 | 252.2 ns |  4.00 ns |  3.74 ns |  0.30 |

  

As one can see from the results, a lambda is `3-6x times` slower than any of the other methods.

---

## Notes

When calling the `lambda/local function/function/operation` once off in the hot path, there probably won't be any noticeable difference depending on which method is used.  
However if used in a loop (such as in the example), consider moving away from a `lambda` to any of the other techniques to see an improvement in performance. 

---

## References

[Lambda Optimizations Tips 1](https://leveluppp.ghost.io/content/images/size/w1000/2021/07/lambda_tips1-1.png)  

<?# DailyDrop ?>104: 27-06-2022<?#/ DailyDrop ?>
