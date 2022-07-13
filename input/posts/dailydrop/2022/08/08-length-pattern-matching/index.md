---
title: "String null or empty using pattern matching"
lead: "Leveraging pattern matching instead of string.IsNullOrEmpty - with performance benchmarks!"
Published1: "08/08/2022 01:00:00+0200"
slug: "08-length-pattern-matching"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - patternmatching
    - length

---

## Daily Knowledge Drop

`Pattern matching` syntax can be used to check the nullability and length of a string, instead of the traditional `string.IsNullOrEmpty` method - with interesting (and promising) performance benchmarks!

---

## IsNullOrEmpty

Usually the `string.IsNullOrEmpty` method is used to check if a specific string has a value (other than null or empty) - a string is passed in and a bool value is returned.

Consider the following method which uses `string.IsNullOrEmpty`:

``` csharp
void IsStringNullOrEmpty(string value)
{ 
    if (!string.IsNullOrEmpty(value))
    {
        Console.WriteLine($"Using 'IsNullOrEmpty' the '{value}' is NOT empty or null");
    }
}
```

If the method is called with three different values:

``` csharp
string checkValue = "www.alwaysdeveloping.net";
IsStringNullOrEmpty(checkValue);

checkValue = "";
IsStringNullOrEmpty(checkValue);

checkValue = null;
IsStringNullOrEmpty(checkValue);
```

As expected, only one value triggers an output to the console:

``` terminal
    Using 'IsNullOrEmpty' the value 'www.alwaysdeveloping.net' is NOT empty or null
```

---

## Pattern Matching

However, instead of this, `string.IsNullOrEmpty` the following `pattern matching` syntax could be used:

``` csharp
    value is { Length: > 0 }
```

This checks if the _value_ variable has a value (not null) and has a length greater than zero.

Again, consider the following method which uses `pattern matching`:

``` csharp
void IsStringNullOrEmpty(string value)
{
    if (value is { Length: > 0 })
    {
        Console.WriteLine($"Using 'pattern matching' the value '{value}' is NOT empty or null");
    }
}
```

If the method is called with three different values (the same values as above):

``` csharp
string checkValue = "www.alwaysdeveloping.net";
IsStringNullOrEmpty(checkValue);

checkValue = "";
IsStringNullOrEmpty(checkValue);

checkValue = null;
IsStringNullOrEmpty(checkValue);
```

Again, as expected, the same values as when using `IsNullOrEmpty` are picked up as valid or not:

``` terminal
    Using 'pattern matching' the value 'www.alwaysdeveloping.net' is NOT empty or null
```

---

## Benchmarks

So there doesn't appear to be any difference in the output of the two methods. Next let's see how each performs using BenchmarkDotNet.

The two different methods were compared, using the same three values as above: a string with a `value`, and `empty` string and a `null`:

``` csharp
[MemoryDiagnoser]
public class Benchmarks
{
    [Params("alwaysdeveloping.net", "", null)]
    public string? strValue { get; set; }

    [Benchmark]
    public void IsNullOrEmpty()
    {
        _ = !string.IsNullOrEmpty(strValue);
    }

    [Benchmark]
    public void PatternMatching()
    {
        _ = strValue is { Length: > 0 };
    }
}
```

The results:

|          Method |             strValue |      Mean |     Error |    StdDev |    Median | Allocated |
|---------------- |--------------------- |----------:|----------:|----------:|----------:|----------:|
|   IsNullOrEmpty |                    ? | 0.2562 ns | 0.0367 ns | 0.0307 ns | 0.2615 ns |         - |
| PatternMatching |                    ? | 0.2592 ns | 0.0203 ns | 0.0190 ns | 0.2627 ns |         - |
|   IsNullOrEmpty |                      | 0.0158 ns | 0.0148 ns | 0.0123 ns | 0.0133 ns |         - |
| PatternMatching |                      | 0.0150 ns | 0.0144 ns | 0.0127 ns | 0.0163 ns |         - |
|   IsNullOrEmpty | alwaysdeveloping.net | 0.0355 ns | 0.0298 ns | 0.0279 ns | 0.0296 ns |         - |
| PatternMatching | alwaysdeveloping.net | 0.0093 ns | 0.0113 ns | 0.0105 ns | 0.0056 ns |         - |

    
From the result, when the string is `null or empty performance is pretty much equivalent`, however when the string `has a value, pattern matching is about 4x faster`

---

## Notes

The `pattern matching` syntax is definitely interesting and intriguing, however is definitely not as intuitive or informative to the developer as to what it does. If micro-optimization is required, this is definitely something to look into, however for most applications the performance improvement would be unnoticeable.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1545094497138360323)   

---

<?# DailyDrop ?>133: 08-08-2022<?#/ DailyDrop ?>
