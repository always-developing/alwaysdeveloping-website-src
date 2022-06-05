---
title: "Binary and hexadecimal conversion"
lead: "Using the Convert class to convert a integer to binary or hexadecimal"
Published: "06/20/2022 01:00:00+0200"
slug: "20-base-conversion"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - binary
    - hexadecimal

---

## Daily Knowledge Drop

The `Convert` class can be used to convert any integer value to it's corresponding value of a `different base`.  

`Binary` (base 2), `Octal` (base 8), `Decimal` (base 10) and `Hexadecimal` (base 16) are supported.

---

## Convert class

Using the `Convert` class to perform the conversion is incredibly simple:

``` csharp
var intValue = 642;

// use the ToString method, specifying the new base
var binary = Convert.ToString(intValue, 2);
var octal = Convert.ToString(intValue, 8);
var hex = Convert.ToString(intValue, 16);

Console.WriteLine(binary);
Console.WriteLine(octal);
Console.WriteLine(hex);
```

The output is as follows:

``` powershell
1010000010
1202
282
```

Negative values are also supported:

``` csharp
var intValue = 642;

// use the ToString method, specifying the new base
var binaryNeg = Convert.ToString(intValue * 1, 2);
var octalNeg = Convert.ToString(intValue * -1, 8);
var hexNeg = Convert.ToString(intValue * -1, 16);

Console.WriteLine(binaryNeg);
Console.WriteLine(octalNeg);
Console.WriteLine(hexNeg);
```

The output is as follows:

``` powershell
11111111111111111111110101111110
37777776576
fffffd7e
```

And that's all there is to it - simple and occasionally useful!

---

## Notes

In my almost 20 years of programming, I don't think I've ever had to perform these kinds of conversions (outside of assignments at university), so this functionality is probably not useful for every day development for most applications - however when the need does arise, it's useful to know it can be easily implemented.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1523685323875635200)  

<?# DailyDrop ?>99: 20-06-2022<?#/ DailyDrop ?>
