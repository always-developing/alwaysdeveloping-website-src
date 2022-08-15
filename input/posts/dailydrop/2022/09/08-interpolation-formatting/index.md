---
title: "Formatting interpolated strings"
lead: "How interpolated strings can be formatted without using the ToString method"
Published1: "09/08/2022 01:00:00+0200"
slug: "08-interpolation-formatting"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - interpolation
   - formatting
   - string

---

## Daily Knowledge Drop

When using `string interpolation`, the colon `:` operator can be used, followed by the `format string` to specify how the string should be formatted. This can be used instead of the `ToString` method with a specified format.

This post is about `interpolated strings` however the technique will also work when using the `index component` (using _String.Format_ with `{0}` instead the string).

---

## Format

In a previous post, we learnt how to [align the string when performing string interpolation](../../08/03-string-interpolation-alignment/) - but when using this method (and other composite string techniques) there also an optional _formatString_ component. 

The full syntax for:
- `string interpolation` is: `{<interpolationExpression>[,<alignment>][:<formatString>]}`
- `composite formatting` is: `{index[,alignment][:formatString]}`

This post will focus on the _formatString_ portion - this provides a shortcut to using the `.ToString(format)` method on the relevent entity.

---

## Examples

In the examples below, the _ToString_ method, and the _interpolated format string_ method are compared, with the same format string, and shown to produce the same output. It is also shown when no format string is specified, the `general ("G") format specifier is used (for numeric, datetime and enumeration).

### DateTime

Below we see how to format a `DateTime` using the more traditional _ToString_ method, and then the _format string_ method:

``` csharp
DateTime current  = DateTime.Now;
Console.WriteLine($"Current Datetime:{current}");
Console.WriteLine($"Current Datetime:{current.ToString("MM/dd/yyyy hh:mm:ss.fff")}");
Console.WriteLine($"Current Datetime:{current:MM/dd/yyyy hh:mm:ss.fff}");

Console.WriteLine($"Current Datetime:{current.ToString("hh:mm")}");
Console.WriteLine($"Current Datetime:{current:hh:mm}");
```

Executing the above:

``` terminal
Current Datetime:2022/08/15 05:56:53
Current Datetime:09/07/2022 05:54:25.527
Current Datetime:09/07/2022 05:54:25.527
Current Datetime:05:54
Current Datetime:05:54
```

Same output with the two methods, but when using the _interpolated format string_ method, the code is more concise and (arguably) cleaner.

---

### Guid

A `Guid` example:

``` csharp
Guid newGuid = Guid.NewGuid();
Console.WriteLine($"Guid value: {newGuid}");
Console.WriteLine($"Guid value: {newGuid.ToString("B")}");
Console.WriteLine($"Guid value: {newGuid:B}");
```

Executing the above:

``` terminal
Guid value: bcaf5b5e-14be-4156-a480-a87614a900f4
Guid value: {bcaf5b5e-14be-4156-a480-a87614a900f4}
Guid value: {bcaf5b5e-14be-4156-a480-a87614a900f4}
```

---

### TimeSpan

A `TimeSpan` example:

``` csharp
TimeSpan travelTime = new TimeSpan(1, 6, 24, 1);
Console.WriteLine($"Travel time: {travelTime}");
Console.WriteLine($"Travel time: {travelTime.ToString("g")}");
Console.WriteLine($"Travel time: {travelTime:g}");
```

Executing the above:

``` terminal
Travel time: 1.06:24:01
Travel time: 1:6:24:01
Travel time: 1:6:24:01
```

---

### Numeric

A `numeric` example:

``` csharp
int cost = 5699;
Console.WriteLine($"Cost: {cost}");
Console.WriteLine($"Cost: {cost.ToString("c")}");
Console.WriteLine($"Cost: {cost:c}");
```

Executing the above:

``` terminal
Cost: 5699
Cost: R5 699,00
Cost: R5 699,00
```

---

### Enum

A `Enum` example:

``` csharp
ConsoleColor drawColor = ConsoleColor.Green;
Console.WriteLine($"Draw color: {drawColor}");
Console.WriteLine($"Draw color: {drawColor.ToString("D")}");
Console.WriteLine($"Draw color: {drawColor:D}");
```

Executing the above:

``` terminal
Draw color: Green
Draw color: 10
Draw color: 10
```

---

## Notes

This is `not a new or revolutionary feature`. I tend to use the `ToString` method when formatting strings, not really aware that there was an alternative. Using the `:formatString` method is not going to drastically change the maintainability, readability or performance of the code - but it does require slightly less typing and removes one additional method call (_ToString_) so personally I will be using this method more frequently going forward.

---

## References

[Composite formatting: Format string component](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting#format-string-component)   

<?# DailyDrop ?>156: 08-09-2022<?#/ DailyDrop ?>
