---
title: "Optional method parameters"
lead: "Use the Optional attribute to make parameters optional"
Published: 03/02/2022
slug: "02-optional-param-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - optional
    - attribute
    - parameters

---

## Daily Knowledge Drop

In additional to giving a parameter a default value, so that it can be omitted when calling a method, the `Optional` attribute can also be used allow for parameter omission.

---

## Parameter default values

When defining a method, the parameters can be given a `default value`. This allows for the parameter to be omitted when calling the method:

``` csharp
// both required
MethodNoDefaults("John", 50);

// all parameters are required when calling this method
void MethodNoDefaults(string name, int age)
{
    Console.WriteLine($"Parameters: Name='{name}', Age='{age}'");
}

//----------------------------------------

// name required, age optional
MethodOneDefaultValue("John");

// only the name parameter is required, age can be omitted
// if omitted, the value of age will be -1
void MethodOneDefaultValue(string name, int age = -1)
{
    Console.WriteLine($"Parameters: Name='{name}', Age='{age}'");
}

//----------------------------------------

// all parameters are optional
MethodAllDefaultValues("John");
MethodAllDefaultValues(age:20);
MethodAllDefaultValues();

// both parameters are optional and can be omitted
void MethodAllDefaultValues(string name = "", int age = -1)
{
    Console.WriteLine($"Parameters: Name='{name}', Age='{age}'");
}
```

The output for the above method calls:

``` powershell
Parameters: Name='John', Age='50'
Parameters: Name='John', Age='-1'
Parameters: Name='John', Age='-1'
Parameters: Name='', Age='20'
Parameters: Name='', Age='-1'
```


One limitation of using the default value approach, is that `optional parameters must appear after all required parameters`. This might force the order of the parameters into an unwanted sequence.

---

## Optional attribute

Instead of using default values, the `Optional` attribute can also be used to mark a parameter as optional:

``` csharp
MethodOneOptional("John", 20);
MethodOneOptional(age: 40);

// first parameter is optional
void MethodOneOptional([Optional]string name, int age)
{
    Console.WriteLine($"Parameters: Name='{name}', Age='{age}'");
}

//----------------------------------------

MethodAllOptional();

// all parameters are optional
void MethodAllOptional([Optional] string name, [Optional] int age)
{
    Console.WriteLine($"Parameters: Name='{name}', Age='{age}'");
}
```

The output being:

``` powershell
Parameters: Name='John', Age='20'
Parameters: Name='', Age='40'
Parameters: Name='', Age='0'
```


One advantage of using the `Optional` attribute, is that `optional parameters can appear anywhere in the sequence of attributes`, even before required parameters.

The disadvantage is that a `default value cannot be specified for optional parameters` and the type default value will be used.

---

## Notes

The `Optional` attribute is another great option to know about, to be leveraged in the appropriate use case.

---

<?# DailyDrop ?>22: 02-03-2022<?#/ DailyDrop ?>
