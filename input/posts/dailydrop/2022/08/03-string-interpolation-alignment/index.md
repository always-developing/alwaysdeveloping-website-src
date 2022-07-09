---
title: "String interpolation with alignment"
lead: "String interpolation provides a mechanism to align (and pad) interpolated values"
Published: "08/03/2022 01:00:00+0200"
slug: "03-string-interpolation-alignment"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - string
    - interpolation
    - alignment

---

## Daily Knowledge Drop

When using `string interpolation ($)`, the _interpolation expressions results_ (the final string resolved into the main string) can be left or right aligned, including padding to be used when aligning. 

This can be very useful when having _interpolation expressions_ which result in string of varying lengths - having these values alignment can result in a more uniform output.

---

## Examples

In all the below examples, a collection of _sale values_ of varying lengths is used:

``` csharp
var saleValues = new[]
{
    100,
    54500,
    1,
    8514,
    -500
};
```

#### Default 



If we want to output each of them without any alignment:

``` terminal
The sale amount is: 100 (for the month of August)
The sale amount is: 54500 (for the month of August)
The sale amount is: 1 (for the month of August)
The sale amount is: 8514 (for the month of August)
The sale amount is: -500 (for the month of August)
```

#### Right align 

If we require a more uniform output, a `positive number` can be used to `right align and pad` the value:

``` csharp
foreach (var sale in saleValues)
{
    Console.WriteLine($"The sale amount is: {sale, 9} (for the month of August)");
}
```

The above will `right align` the _sale_ value and make it a uniform _9 characters in length_:

``` terminal
The sale amount is:       100 (for the month of August)
The sale amount is:     54500 (for the month of August)
The sale amount is:         1 (for the month of August)
The sale amount is:      8514 (for the month of August)
The sale amount is:      -500 (for the month of August)
```

#### Left align 

To left align, a `negative number` is specified:

``` csharp
foreach (var sale in saleValues)
{
    Console.WriteLine($"The sale amount is: {sale, -9} (for the month of August)");
}
```

The above will `left align` the _sale_ value and make it a uniform _9 characters in length_:

``` terminal
The sale amount is: 100       (for the month of August)
The sale amount is: 54500     (for the month of August)
The sale amount is: 1         (for the month of August)
The sale amount is: 8514      (for the month of August)
The sale amount is: -500      (for the month of August)
```

---

#### Const value 

The value specified is required to be a constant value.

So this is valid:

``` csharp
// must be a constant
const int length = 10;

foreach (var sale in saleValues)
{
    Console.WriteLine($"The sale amount is: {sale, length} (for the month of August)");
}
```

However, this is `NOT VALID`, as _length_ is not a `const`:

``` csharp
int length = saleValues.Length;

foreach (var sale in saleValues)
{
    Console.WriteLine($"The sale amount is: {sale, length} (for the month of August)");
}
```

---

## Notes

A small lesser-known feature of string interpolation, however it can be very useful in producing uniform output when required, with very little additional effort.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1541467908169035776)   

---

<?# DailyDrop ?>130: 03-08-2022<?#/ DailyDrop ?>
