---
title: "LINQ improvements - FirstOrDefault defaults"
lead: "Exploring the FirstOfDefault improvements introduced in .NET6"
Published: 02/25/2022
slug: "25-firstordefault-default"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - indexer
    - array
    - extend
    
---

## Daily Knowledge Drop

A number of enhancements have been made to LINQ as part of .NET6 - one of those is the ability to `set a default value to be returned from .FirstOrDefault()`.

---

## Examples

_List<>_ is being used in the below examples, but the method is available on all compatible types, not just _List<>_.

### Simple type list

``` csharp
// list of 11 integers
var intValues = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Returns the first value greater than 4. 
// A value from the list will be returned
Console.WriteLine(intValues.FirstOrDefault(i => i > 4));

// Returns the first value greater than 20.
// No qualifying value exists in the list, 
// so the default value of type int will be returned (0 in this case)
Console.WriteLine(intValues.FirstOrDefault(i => i > 20));

// Returns the first value greater than 20. No qualifying value 
// exists in the list, and as the new method override is being used, 
// the explicitly supplied default value will be returned (-1 in this case)
Console.WriteLine(intValues.FirstOrDefault(i => i > 20, -1));
```

The output:

``` powershell
5
0
-1
```

---

### Complex type list

The same process can be applied to more complex type. The below example uses a _Person record_ (but the same would apply to a traditional class as well)

``` csharp
var personValues = new List<Person> {
    new Person("Dave",30),
    new Person("Nate",45),
    new Person("Pat",60),
    new Person("Taylor",25),
    new Person("Chris",39)
};

// Returns the first Person with age of 30. A value from the list will be returned
Console.WriteLine(personValues.FirstOrDefault(p => p.Age == 30));

// Returns the first Person whose name starts with "J". 
// No qualifying value exists in the list, so the default value of 
// Person will be returned (null in this case)
Console.WriteLine(personValues.FirstOrDefault(p => p.Name.StartsWith("J")));

// Returns the first _Person_ whose name starts with "J". 
// No qualifying value exists in the list, and as the new method override 
// is being used, the explicitly supplied default value will be returned 
// (Person initialized with "John Doe" and 0 in this case)
Console.WriteLine(personValues.FirstOrDefault(p => p.Name.StartsWith("J"), 
    new Person("John Doe", 0)));

public record Person(string Name, int Age);
```

The output. The blank line indicates the _null_ record returned from the 2nd call to _FirstOrDefault()_:

``` powershell
Person { Name = Dave, Age = 30 }

Person { Name = John Doe, Age = 0 }
```

---

### Empty list

This method can also be used to return a specific default value in the case of an empty list.

``` csharp
var stringValues = new List<string>();

// Returns the first value in the list. 
// The list is empty, so the default value of string
// will be returned (empty string in this case)
Console.WriteLine(stringValues.FirstOrDefault());

// Returns the first value in the list. 
// The list is empty, and as the new method override is 
// being used, the explicitly supplied default value
// will be returned ("empty" in this case)
Console.WriteLine(stringValues.FirstOrDefault("empty"));
```

The output is as follows, the blank line indicating the _empty string_ returned from the 1st call to _FirstOrDefault()_:

``` powershell
     
empty
```

---

## Conclusion

A small, simple update made to LINQ, but nevertheless a very useful feature which I'm sure will get a fair amount of usage.

---

<?# DailyDrop ?>19: 25-02-2022<?#/ DailyDrop ?>
