---
title: "Converting ExpandObject"
lead: "How ExpandObject can be converted to IEnumerable and a Dictionary"
Published: "06/22/2022 01:00:00+0200"
slug: "22-expando-conversion"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - expandobject
    - dictionary
    - ienumerable

---

## Daily Knowledge Drop

The `ExpandoObject` class implements `IEnumberable` and `IDictionary` - this means its fields can be iterated over, and that it can be converted to, and operated on as a Dictionary.

---

## ExpandoObject

First some information on the `ExpandoObject` class - it enables one to add and deleted members of its instance at runtime. 

``` csharp
dynamic infoObject = new ExpandoObject();
infoObject.Name = "Always Developing";
infoObject.Url = "www.alwaysdeveloping.net";

Console.WriteLine(infoObject.Name);
Console.WriteLine(infoObject.Url);
```

In the above example, the properties _Name_ and _Age_ are dynamically added at runtime. Any number of properties, of any type can dynamically be added.

The `ExpandoObject` can also have methods (Action and Func) added dynamically, and then invoked:

``` csharp
dynamic infoObject = new ExpandoObject();
infoObject.Name = "Always Developing";
infoObject.Url = "www.alwaysdeveloping.net";
infoObject.Visitors = 0;
infoObject.IncreaseVisitorCount = (Action)(() => { infoObject.Visitors++; });
infoObject.GetVisitorCount = (Func<int>)(() => { return infoObject.Visitors; });

infoObject.IncreaseVisitorCount();
infoObject.IncreaseVisitorCount();

Console.WriteLine(infoObject.GetVisitorCount());
```

The _IncreaseVisitorCount_ Action and _GetVisitorCount_ Func are dynamically added, and then invoked.

The output of the above being `2`, as expected, as the _IncreaseVisitorCount_ method is invoked twice.

---

## IEnumerable conversion

As `ExpandoObject` implement IEnumberable, specifically `IEnumerable<KeyValuePair<string, object?>>` and can be iterated over, with a `KeyValuePair<string, object?>` item returned for each iteration:

``` csharp
dynamic infoObject = new ExpandoObject();
infoObject.Name = "Always Developing";
infoObject.Url = "www.alwaysdeveloping.net";
infoObject.Visitors = 0;
infoObject.IncreaseVisitorCount = (Action)(() => { infoObject.Visitors++; });
infoObject.GetVisitorCount = (Func<int>)(() => { return infoObject.Visitors; });

// prop is of type KeyValuePair<string, object?>
foreach (var prop in infoObject)
{
    Console.WriteLine($"Key: '{prop.Key}' with value '{prop.Value}'");
}
```

In the above, iterating over _infoObject_ will return each property dynamically added to the `ExpandoObject` instance, including methods:

``` powershell
Key: 'Name' with value 'Always Developing'
Key: 'Url' with value 'www.alwaysdeveloping.net'
Key: 'Visitors' with value '2'
Key: 'IncreaseVisitorCount' with value 'System.Action'
Key: 'GetVisitorCount' with value 'System.Func`1[System.Int32]'
```

---

## Dictionary conversion

`ExpandoObject` also implements `IDictionary<string, object?>`, so can directly be assigned to this type:

``` csharp
dynamic infoObject = new ExpandoObject();
infoObject.Name = "Always Developing";
infoObject.Url = "www.alwaysdeveloping.net";
infoObject.Visitors = 0;
infoObject.IncreaseVisitorCount = (Action)(() => { infoObject.Visitors++; });
infoObject.GetVisitorCount = (Func<int>)(() => { return infoObject.Visitors; });

// cast ExpandoObject to IDictionary<string, object?>
IDictionary<string, object?> dictionary = infoObject;
foreach (var item in dictionary)
{
    Console.WriteLine($"Key: '{item.Key}' with value '{item.Value}'");
}
```

The output of the above the same as the first example:

``` powershell
Key: 'Name' with value 'Always Developing'
Key: 'Url' with value 'www.alwaysdeveloping.net'
Key: 'Visitors' with value '2'
Key: 'IncreaseVisitorCount' with value 'System.Action'
Key: 'GetVisitorCount' with value 'System.Func`1[System.Int32]'
```

Casting to `Dictionary` allows one to check if a property/key has been added to the `ExpandoObject`:

``` csharp
IDictionary<string, object?> dictionary = infoObject;
foreach (var item in dictionary)
{
    Console.WriteLine($"Key: '{item.Key}' with value '{item.Value}'");
}

// This would result in an exception
// var value = infoObject["Name"];

// On a dictionary, this is allowed
if (dictionary.ContainsKey("Name"))
{
    Console.WriteLine(dictionary["Name"]);
}
```

---

## Notes

This is a small, but useful piece of knowledge to be aware of if working with `ExpandoObject` as the ability to easily convert to other types opens up new operations and possibilities on the instance.

---

## References

[C# Tip: Convert ExpandoObjects to IDictionary](https://www.code4it.dev/csharptips/expandoobject-to-dictionary)  

<?# DailyDrop ?>101: 22-06-2022<?#/ DailyDrop ?>
