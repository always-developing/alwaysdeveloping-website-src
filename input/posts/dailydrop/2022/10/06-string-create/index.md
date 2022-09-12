---
title: "Culture specific strings with String.Create"
lead: "Evaluating interpolated strings to a specific culture using String.Create"
Published: "10/06/2022 01:00:00+0200"
slug: "06-string-create"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - string
   - format

---

## Daily Knowledge Drop

Introduced in C# 10 (.NET 6), the `String.Create` method can be used to evaluate an _interpolated string_ to a specific culture (including the invariant culture). This method is faster, and uses less resources than the previously available method of using `FormattableString`.

---

## FormattableString

### Invariant

Prior to C# 10 `FormattableString` was the method used, and evaluating an _interpolated string_ to _invariant culture_ was fairly straightforward:

``` csharp
private decimal decValue = 1.99m;
private DateTime dateValue = DateTime.UtcNow;

// output the default interpolated string
Console.WriteLine($"Decimal: {decValue} | Date: {dateValue}");

// output the string formatted to "invariant culture"
Console.WriteLine(FormattableString.Invariant(
    $"Decimal: {decValue} | Date: {dateValue}"));
```

The output of the above:

``` terminal
Decimal: 1,99 | Date: 2022/09/12 03:49:41
Decimal: 1.99 | Date: 09/12/2022 03:49:41
```

----

### Culture specific

Evaluating an _interpolated string_ to _a specific culture_ is slightly more complicated when using `FormattableString`:

``` csharp
private decimal decValue = 1.99m;
private DateTime dateValue = DateTime.UtcNow;

// output the default interpolated string
Console.WriteLine($"Decimal: {decValue} | Date: {dateValue}");

// output the string formatted in the specific culture
// first declare the FormattableString
FormattableString formattable = $"Decimal: {decValue} | Date: {dateValue}";
// ToString specifying the culture
Console.Write(formattable.ToString(CultureInfo.GetCultureInfo("en-US")));
```

The output of the above:

``` terminal
Decimal: 1,99 | Date: 2022/09/12 04:00:47
Decimal: 1.99 | Date: 9/12/2022 4:00:47 AM
```

---

## String.Create

### Invariant

The new `String.Create` method introduced in .NET6 with C# 10, simplifies the evaluation of a interpolated string to a specific culture:

``` csharp
private decimal decValue = 1.99m;
private DateTime dateValue = DateTime.UtcNow;

// output the default interpolated string
Console.WriteLine($"Decimal: {decValue} | Date: {dateValue}");

// output string with the specific culture (invariant)
Console.WriteLine(String.Create(CultureInfo.InvariantCulture,
    $"Decimal: {decValue} | Date: {dateValue}"));
```

The output of the above (which is the same as when using _FormattableString.Invariant_ above):

``` terminal
Decimal: 1,99 | Date: 2022/09/12 04:02:59
Decimal: 1.99 | Date: 09/12/2022 04:02:59
```

----

### Culture specific

Specifying a _specific culture_ with `String.Create` is just as simple as specifying _InvariantCulture_, and definitely simpler than the _FormattableString_ equivalent:

``` csharp
private decimal decValue = 1.99m;
private DateTime dateValue = DateTime.UtcNow;

// output the default interpolated string
Console.WriteLine($"Decimal: {decValue} | Date: {dateValue}");

// output string with the specific culture (en-US)
Console.WriteLine(String.Create(CultureInfo.GetCultureInfo("en-US"),
    $"Decimal: {decValue} | Date: {dateValue}"));
```

The output of the above (which is the same as when using _FormattableString_ with the specific culture above):

``` terminal
Decimal: 1,99 | Date: 2022/09/12 04:05:15
Decimal: 1.99 | Date: 9/12/2022 4:05:15 AM
```

---

## Performance

Using `BenchmarkDotNet` to compare the speed and memory usage of the above 4 methods, we can see that in all cases, using the _String.Create_ version is `faster, and uses less memory` than the _FormattableString versions_:

|                   Method |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|------------------------- |---------:|--------:|--------:|-------:|----------:|
| StringFormatterInvariant | 342.6 ns | 4.92 ns | 4.60 ns | 0.0367 |     232 B |
|   StringFormatterCulture | 400.2 ns | 7.81 ns | 9.87 ns | 0.0429 |     272 B |
|    StringCreateInvariant | 272.5 ns | 3.86 ns | 3.42 ns | 0.0162 |     104 B |
|      StringCreateCulture | 326.5 ns | 4.41 ns | 3.91 ns | 0.0229 |     144 B |

---

## Notes

Even though the performance and memory improvements are slight (nano-seconds and bytes), if your application is making `heavy use of FormattableString` to format strings to specific cultures, then there are gains to be had by converting to using `String.Create`.

---

## References

[Performance: string.Create vs FormattableString](https://www.meziantou.net/performance-string-create-vs-formattablestring.htm)   

<?# DailyDrop ?>176: 06-10-2022<?#/ DailyDrop ?>
