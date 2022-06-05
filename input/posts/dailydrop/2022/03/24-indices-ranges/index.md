---
title: "Indices and ranges"
lead: "Using indices and ranges for succinct syntax when working with sequences"
Published: 03/24/2022
slug: "24-indices-ranges"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - func
    - methods
    - invoke

---

## Daily Knowledge Drop

C#8, first introduced with .NET Core 3, added support for `indices and ranges`, which provide a succinct syntax for accessing single elements or ranges in a sequence.

Two new operators were introduced to support this functionality:
- `^`: The _index from end_ operator
- `..`: The _range_ operator

---

## ^ operator

The new `^` operator is an `index from end` operator, which specifies that an index is relative to the end of the sequence.

``` csharp
var words = new string[]
{
    "This", "is", "a", "sequence", "of", 
    "word", "to", "demo", "indices", "and", "ranges"
};

Console.WriteLine(words[^1]); // last word (ranges)
Console.WriteLine(words[^2]); // 2nd last word (and)
Console.WriteLine(words[^3]); // 3rd last word (indices)
Console.WriteLine(words[^4]); // (demo)
Console.WriteLine(words[^5]); // (to)
```

The output is:

``` powershell
    ranges
    and
    indices
    demo
    to
```

`^1` indicates 1 index from the end, in other words the last item.  
`^2` indicates 2 indexes from the end, the second last item, etc.

`^0` is used to represent the length of the sequence, and is equivalent to `sequence.Length`.

---

## .. operator

The new `..` operator is a `range` operator, which specifies the start and end of the range as its operands.

### Constant values

Constant int values can be used with the `..` operator:

``` csharp
var words = new string[]
{
    "This", "is", "a", "sequence", "of", 
    "word", "to", "demo", "indices", "and", "ranges"
};

// get elements 0,1,2,3 and 4 (not 5) from the sequence
var snippet = words[0..5];
// join the items in "snippet" and separate them with a space
Console.WriteLine(string.Join(" ", snippet));
```

The output is:

``` powershell
    This is a sequence of
```

`0..5` indicates a range of items 0 to 4. `words[5]` is not included in the range.

---

### 'Index from end' values

The range (`..`) operator can also be used in conjunction with the new `^` operator:

``` csharp
var words = new string[]
{
    "This", "is", "a", "sequence", "of", 
    "word", "to", "demo", "indices", "and", "ranges"
};

// get the 3rd, 2nd and last words from the sequence
var lastThreeWord = words[^3..^0];
// join the items in "lastThreeWord" and separate them with a space
Console.WriteLine(string.Join(" ", lastThreeWord));
```

The output is:

``` powershell
    indices and ranges
```

As the last item specified by a Range, is not included in the range, the `^0` is used, to indicate the `last item in the sequence when used in a Range.`

---

### Range variable

A `Range` can also be declared as a variable, which has a value set at runtime, then used:

``` csharp
var words = new string[]
{
    "This", "is", "a", "sequence", "of", 
    "word", "to", "demo", "indices", "and", "ranges"
};

Range GetRange(int start, int end)
{
    return start..end;
}

var dynamicWords = words[GetRange(2, 8)];
Console.WriteLine(string.Join(" ", dynamicWords));
```

The output is:

``` powershell
    a sequence of word to demo
```

---

## String example

The new operators are not only supported on arrays, but also can also be used on `string`, as well as `Span<T>` and `ReadOnlySpan<T>`

A `string example`:

``` csharp
string alwaysDeveloping = "alwaysdeveloping.net";

// get the last 4 characters
Console.WriteLine(alwaysDeveloping[^4..^0]);

// get the last character
Console.WriteLine(alwaysDeveloping[^1]);
Console.WriteLine(alwaysDeveloping[alwaysDeveloping.Length - 1]);
```

The output is:

``` powershell
    .net
    t
    t
```

As you can see from the output, `alwaysDeveloping[^1]` is equilveilant to `alwaysDeveloping[alwaysDeveloping.Length - 1]` - just a lot more concise and succinct.

---

## Notes

While maybe not for everyday use, especially if not dealing with a lot of arrays (and other supported types) - the new operators can prove very useful when they are required, the resulting code being more succinct and less verbose.

---

## References

[Indices and ranges](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#indices-and-ranges)

<?# DailyDrop ?>37: 24-03-2022<?#/ DailyDrop ?>
