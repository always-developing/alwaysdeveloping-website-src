---
title: "Accurate string length"
lead: "String.Length for inaccurate string length and how to get the correct length"
Published: "01/18/2023 01:00:00+0200"
slug: "18-string-length"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - string
   - length

---

## Daily Knowledge Drop

`String.Length` does _not always return a "true" accurate length_ of the string - instead, the `StringInfo.LengthInTextElements` method should be used.

---

## String Length

Generally, for most common string usage, the `String.Length` will return the correct length of the string - the number of characters in the string object. However, the number of characters in the string object `does not always correspond to the number of characters which reflect on the screen`.

For example:

``` csharp
Console.Write("This string '👍' has the length of: ");
Console.WriteLine("👍".Length);
```

The output of the above is:

``` terminal
This string '??' has the length of: 2
```

The terminal window is unable to render the 👍 emoji, so it is reflected as '??' - but this also gives insight into its length (two question marks, and no one). Even though 👍 reflects as `one character when output, it actually consists of two characters in the string object`.

---

## StringInfo LengthInTextElements

To get a more accurate string length, the `StringInfo.LengthInTextElements` property can be used:

``` csharp
Console.Write("This string '👍' has the true length of: ");
Console.WriteLine(new StringInfo("👍").LengthInTextElements);
```

The output of the above is:

``` terminal
This string '??' has the length of: 1
```

The _LengthInTextElements_ property will return the number of text elements displayed in the terminal.


---

## Notes

If the length of the string is required for display purposes, and the string could contain "non-traditional" characters, such as emoji's - then the _StringInfo.LengthInTextElements_ property should be used for more accurate results.

---

## References

[Getting the printable length of a string or character](https://linkdotnet.github.io/tips-and-tricks/strings/#getting-the-printable-length-of-a-string-or-character)  

<?# DailyDrop ?>237: 18-01-2023<?#/ DailyDrop ?>
