---
title: "Complex Pattern Matching"
lead: "Pattern matching can contain IS, NOT, AND and OR keywords for more complex statements"
Published: "08/15/2022 01:00:00+0200"
slug: "15-pattern-keywords"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - patternmatching
   - keywords

---

## Daily Knowledge Drop

`Pattern matching` can use the `is`, `not`, `and` and `or` keywords individually in a statement, but also use the keywords together to form an enhanced, more complex statement.

---

## Individual

### is keyword

Common usages of the `is` keyword is for `null checks` and `type tests`:

A `null check` being performed:

``` csharp
int? age = 12;

// check if the value of age
// is a valid number (i.e. not null)
if (age is int number)
{
    Console.WriteLine($"'age' has the value of:{number}");
}
else
{
    Console.WriteLine("'age' has no value");
}
```

A `type check` being performed (for performance improvements in this example):

``` csharp
public int SumListChecked(IEnumerable<int> enumerable)
{
    var runningSum = 0;

    // Check the type 
    if (enumerable is List<int> list)
    {
        foreach (var item in list)
        {
            runningSum += item;
        }
        return runningSum;
    }
    
    // all others
    foreach (var item in enumerable)
    {
        runningSum += item;
    }

    return runningSum;
}
```

### not keyword

The `not` keyword can be used in conjunction with the `is` keyword to perform a `null check`:

``` csharp
string? outputMessage = "This message should be output!";

// explicitly check that the string is not null
if (outputMessage is not null)
{
    Console.WriteLine(outputMessage);
}
```

---

### and, or keywords

The `and` and `or` keywords are commonly used in a `switch expression` for `pattern matching`:

``` csharp
string GetGrade(int percentage) =>
    percentage switch
    {
        (< 0) or (> 100) => "Invalid",
        (>= 0) and (< 40) => "Fail",
        (>= 40) and (< 70) => "Average",
        (>= 70) => "Excellent",
    };
```

---

## Enhanced matching

All the keywords can be used together for a more complex pattern matching expression - in this example to `validate an email address` (there are definitely better, more comprehensive ways to validate an email address than using pattern matching, and although the below is not a complete and entirely valid way to validate an email address, it is sufficient for this demo)

The first step is to loop through each character in the email address to determine if its valid for an email address:

``` csharp
bool IsValidEmailAddress(string email)
{
    foreach (var c in email)
    {
        var result = IsValidChar(c);
        if (!result)
        {
            return false;
        }
    }

    return true;
}
```

If any of the characters are invalid according to the `IsValidChar` method, then the _email string_ as a whole is invalid.

The `IsValidChar` is where the enhanced, more complex pattern matching takes place. The method is an `expression-bodied method` which in turn used `pattern matching`:

``` csharp
bool IsValidChar(char c) =>
    c is ((>= 'A' and <= 'Z') // capital letters
      or (>= 'a' and <= 'z') // lower case
      or (>= '0' and <= '9') // numbers
      or '@' // allowed symbols
      or '.'
      or '-'
      or '+'
      and not ' ');
```

Executing the above code with the following example:

``` csharp
var emailToValidate = "learn@alwaysdeveloping.com";
Console.WriteLine($"'{emailToValidate}' a valid email " +
    $"address?: {IsValidEmailAddress(emailToValidate)}");

var anotherEmailToValidate = "invalid123, invalid";
Console.WriteLine($"'{anotherEmailToValidate}' a valid email " +
    $"address?: {IsValidEmailAddress(anotherEmailToValidate)}");
```

The output:

``` terminal
'learn@alwaysdeveloping.com' a valid email address?: True
'invalid123, invalid' a valid email address?: False
```

---

## Notes

As `Pattern matching` is a fairly new addition to C#, I have not seen it extensively used, and have no extensively used it myself. However the more I learn about the usages of it, the more it gets incorporated into my coding daily.  
In this demo `pattern matching` is certainly more readable and easier to understand than some other methods (regex for example) - but what one gains in maintainability and readability, one might lose in performance. However, the important thing to know is there are options, and being better informed about each option allows for the best technique to be chosen for each use case.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1543278994069544962)   

---

<?# DailyDrop ?>138: 15-08-2022<?#/ DailyDrop ?>
