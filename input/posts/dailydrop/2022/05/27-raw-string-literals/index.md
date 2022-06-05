---
title: "Raw string literals in .NET7"
lead: "Exploring the useful .NET7 raw string literal feature"
Published: "05/27/2022 01:00:00+0200"
slug: "27-raw-string-literals"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - string
    - literals
    - .net7

---

## Daily Knowledge Drop

The new `raw string literals` feature coming as part of .NET7 (now available through the .NET7 preview) is a new format for dealing with strings, which allows for literals to contain  whitespace, new lines, embedded quotes and other special characters `without escape sequences`.

This new feature is especially useful when working with embedded language strings JSON, XML, HTML etc.

---

## Pre raw string literals

Prior to the raw string literal functionality (before C#11), if building up a JSON string, especially when using string interpolation, a large number of escape characters were required:

``` csharp
var name = "Dave Grohl";
var age = 42;
var isActive = true;
var addressLine1 = "1 Main Street";
var addressLine2 = "Cape Town";
var postalCode = "8000";

var jsonString =
$@"{{
    ""Name"": ""{name}"",
    ""Age"": {age},
    ""IsActive"": {isActive.ToString().ToLower()},
    ""DateCreated"": ""{DateTime.Now.ToString()}"",
    ""Address"" : {{
        ""AddressLine1"": ""{addressLine1}"",
        ""AddressLine2"": ""{addressLine2}"",
        ""PostalCode"": ""{postalCode}""
    }}
}}";
```

As you can see, each time a quote is required, it needs to be escaped with another quote. Each time a brace is required, it needs to be escaped with another brace. Depending on how complicated the embedded string is, this can lead to an unwieldy mess of escape characters.

---

## Raw string literals

A raw string literal starts with at least three double-quotes `"""`, and ends with the same number of double quotes. Within these three double-quotes, single quotes `"` are now considered content and included in the string.  

Any number of quotes `less than the number that opened the raw string literal` will be considered content.

A similar approach is used for string interpolation - the number of `$` which prefixes the string, is the number of braces required to to indicate a nested code expression. `Any number less will be considered content.`

Let's look at a few examples. 

The same example from above, but now using raw string literals:

``` csharp
var name = "Dave Grohl";
var age = 42;
var isActive = true;
var addressLine1 = "1 Main Street";
var addressLine2 = "Cape Town";
var postalCode = "8000";

var rawLiteralJsonString = $$"""
{
    "Name": "{{name}}",
    "Age": {{age}},
    "IsActive": {{isActive.ToString().ToLower()}},
    "DateCreated": "{{DateTime.Now.ToString()}}",
    "Address" : {
        "AddressLine1": "{{addressLine1}}",
        "AddressLine2": "{{addressLine2}}",
        "PostalCode": "{{postalCode}}"
    }
}
""";
```

- The literal is opened with 3 double quotes, and hence 1 or 2 double-quotes (any number less than the number used to open the literal) inside the literal will be considered content
- The literal is prefixed with 2 dollar signs `$`, and hence 1 brace will be considered content, while 2 will be considered the indication for nested code

---

If more than 1 or 2 double-quotes are required in the string, then the number of opening double-quotes can be increased:

``` csharp
var singleQuotes = """This string contains a "quoted" word""";
var doubleQuotes = """This string contains a doubled ""quoted"" word""";
var tripleQuotes = """"This string contains a triple """quoted""" word"""";

Console.WriteLine(singleQuotes);
Console.WriteLine(doubleQuotes);
Console.WriteLine(tripleQuotes);
```

In the above example, the first two strings only required 1 and 2 double-quotes respectively, so opening with 3 double-quotes works. In the third string, 3 double-quotes are required in the content, so the raw string literal is opened with 4 double-quotes.

---

A similar process can be used for braces:

``` csharp
var singleBraces = $$"""This string contains a {braced} word""";
var doubleBraces = $$$"""This string contains a doubled {{braced}} word""";
var tripleBraces = $$$$"""This string contains a triple {{{braced}}} word""";

Console.WriteLine(singleBraces);
Console.WriteLine(doubleBraces);
Console.WriteLine(tripleBraces);
```

If braces are required in the string content, the number of `$` prefixing the string needs to be `1 more` than the number of consecutive braces in the string. In the third string example above, 3 braces are requires, so 4 `$` are required. To use string interpolation and a nested code expression, 4 `$` would need to be used to break out the string.

For example: 

``` csharp
var tripleValue = "Triple";
var tripleBracesInterpolation = $$$$"""{{{{tripleValue}}}} {{{braced}}} word""";
Console.WriteLine(tripleBracesInterpolation);
```

---

## Notes

While is may seem a bit convoluted at first, once the basics are understood, this is an incredibly powerful and useful feature, especially dealing with embedded string (such as JSON in the above example). It makes figuring out the correct sequence of escape characters much easier, as well as make the code more readable.

---

## References

[Raw string literals](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11#raw-string-literals)  

<?# DailyDrop ?>83: 27-05-2022<?#/ DailyDrop ?>
