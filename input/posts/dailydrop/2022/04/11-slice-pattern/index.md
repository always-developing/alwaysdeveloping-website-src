---
title: "Exploring the new list pattern"
lead: "Exploring the new list and slice pattern coming with C# 11"
Published: 04/11/2022
slug: "11-slice-pattern"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - array
    - listpattern
    - slicepattern

---

## Daily Knowledge Drop

C# 11 (being released towards the end of 2022 with.NET7) is introducing the new `list pattern` which `allows for matching against lists and arrays`.

One can also match to zero or more elements (which can then be captured or discarded) in the list pattern, using the new `slice pattern, two single dots` (..) 

The below examples are using C# 11 preview 2, so may change by final release.

---

## Pattern matching

The `list matching` allows for pattern matching on an array or list.

See the below switch expression performing list matching:

``` csharp
static string CheckSwitch(char[] values)
    => values switch
    {
        ['0', ..] => "<1",
        ['1', ',',..] => ">=1 && <2",
        ['2', '5'] => "25",
        ['7', _ ] => "starting with 7. Length 2",
        ['7', .. ] => "starting with 7. Length >2",
        [.., '9'] => "ending with 9",
        ['5', '0', '0', ',', .., '6'] => " >500 && <500.1 && ends in 6",
        [..] => "unclassified"
    };
```
Starting from the top, the checks are as follows:
- `['0', ..]`: The array starts with '0' then has zero or more elements (using the slice pattern syntax)
- `['1', ',',..]`: The array starts with '1' then the decimal point, followed by zero or more elements
- `['2', '5']`: The array starts with '2' and is followed by a '5'
- `['7', _ ]`: The array starts with '7' and is followed by one element
- `['7', .. ]`: The array starts with '7' and is followed by zero or more elements
- `[.., '9']`: The array is of any length, but ends with '9'
- `['5', '0', '0', ',', .., '6']`: The array starts with the 4 characters '5', '0', '0' and the decimal point, followed by zero or more elements, and then ending in a '6' 
- `[..]`: The array pattern is not any of the above

As you can see, the `slice pattern` can be used as a "wild card" match for zero or more elements.

The above switch expression being used:

``` csharp
char[] arr1 = { '0', ',', '1', };
char[] arr2 = { '1', ',', '2', '9' };
char[] arr3 = { '2', '5'};
char[] arr4 = { '7', '0', ',', '4', '8', '1' };
char[] arr5 = { '7', '1' };
char[] arr6 = { '1', '0', '9' };
char[] arr7 = { '0', '0', '9' };
char[] arr8 = { '5', '0', '0', ',', '5', '3', '1' , '6' };
char[] arr9 = { '3', ',', '1', '4' };

Console.WriteLine($"{ string.Join("", arr1) } is {CheckSwitch(arr1)}");
Console.WriteLine($"{ string.Join("", arr2) } is {CheckSwitch(arr2)}");
Console.WriteLine($"{ string.Join("", arr3) } is {CheckSwitch(arr3)}");
Console.WriteLine($"{ string.Join("", arr4) } is {CheckSwitch(arr4)}");
Console.WriteLine($"{ string.Join("", arr5) } is {CheckSwitch(arr5)}");
Console.WriteLine($"{ string.Join("", arr6) } is {CheckSwitch(arr6)}");
Console.WriteLine($"{ string.Join("", arr7) } is {CheckSwitch(arr7)}");
Console.WriteLine($"{ string.Join("", arr8) } is {CheckSwitch(arr8)}");
Console.WriteLine($"{ string.Join("", arr9) } is {CheckSwitch(arr9)}");
```

Results in the following:

``` powershell
0,1 is <1
1,29 is >=1 && <2
25 is 25
70,481 is starting with 7. Length >2
71 is starting with 7. Length 2
109 is ending with 9
009 is <1
500,5316 is  >500 && <500.1 && ends in 6
3,14 is unclassified
```

The switch expression matches from the top down. For example, '109' and '009' both end in '9', however only '109' is classified as 'ending with 9'. This is because the '009' is classified as '<0' which is the first match to be made, before the 'ending in 9' check is performed.

---

## Capturing elements

The `slice pattern` can also be used to capture, and then access the elements which were matched.

In the below example the `list pattern` matching is used to check if the list conform to a certain pattern - but the elements matched using the `slice pattern` are then captured in the _digits_ variable:

``` csharp
char[] arr10 = { '5', '0', '0', ',', '5', '3', '1', '6' };

if (arr10 is ['5', '0', '0', ',', .. var digits, '6'])
{
    Console.WriteLine($"The full value is: { string.Join("", arr10) }");
    Console.WriteLine($"The wildcard digits are: { string.Join("", digits) }");
}
```

The output:

``` powershell
The full value is: 500,5316
The wildcard digits are: 531
```

---

## References

[C# 11 Preview: List patterns](https://devblogs.microsoft.com/dotnet/early-peek-at-csharp-11-features/#c-11-preview-list-patterns)  

<?# DailyDrop ?>49: 11-04-2022<?#/ DailyDrop ?>
