---
title: "Evolution of multi case switch statements"
lead: "Various ways of handling multiple case switch expressions in C#"
Published: 03/11/2022
slug: "11-switch-multi-case"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - switch
    - expression
    - switchexpression

---

## Daily Knowledge Drop

There are `numerous ways of handling the multiple case switch statement/expression in C#`, and these various methods have evolved over time as C# language features have been introduced and enhanced.

Today we'll look at a few of these ways, specifically around having `multiple cases returning the same result`.

---

## Switch statement

With the traditional `switch statement`, each case needs to be specified explicitly (except for the default case). This can become very long and tedious if there are many different cases. Prior to C#7, this was the only option:

``` csharp
public string TraditionalSwitch(int temperature)
{
    if (temperature < 0)
    {
        return "Unnaturally cold";
    }

    switch (temperature)
    {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
            return "Cold";
        case 11:
        case 12:
        case 13:
        case 14:
        case 15:
        case 16:
        case 17:
        case 18:
        case 19:
        case 20:
            return "Moderate";
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
        case 30:
            return "Hot";
        case 31:
        case 32:
        case 33:
        case 34:
        case 35:
        case 36:
        case 37:
        case 38:
        case 39:
        case 40:
            return "Very Hot";
        default:
            return "Surface of the sun";
    }
}
```

--- 

## Switch with when

C#7 introduced the ability to use the `when` keyword, in conjunction with `and (&&) and or (||) operators`, which simplified and reduced the amount of code:

``` csharp
public string WhenExpressionSwitch(int temperature)
{
    switch (temperature)
    {
        case var t when temperature < 0:
            return "Unnaturally cold";
        case var t when temperature > 0 && temperature < 11:
            return "Cold";
        case var t when temperature > 11 && temperature < 21:
            return "Moderate";
        case var t when temperature > 21 && temperature < 31:
            return "Hot";
        case var t when temperature > 31:
            return "Very Hot";
        default:
            return "Surface of the sun";
    }
}
```

--- 

## Switch assignment

C#8 introduced the ability to `assign the value returned from a switch`, instead of just using it to control logic flow:

``` csharp
public string AssignmentSwitchExpression(int temperature)
{
    var result = temperature switch
    {
        var t when temperature < 0 => "Unnaturally cold",
        var t when temperature > 0 && temperature < 11 => "Cold",
        var t when temperature > 11 && temperature < 21 => "Moderate",
        var t when temperature > 21 && temperature < 31 => "Hot",
        var t when temperature > 31 => "Very Hot",
        _ => "Surface of the sun"
    };

    return $"It is {result}";
}
```

--- 

## Switch expression

C#8 also introduced the ability to `create a switch expression from a method`, again to simplify and reduce code:

``` csharp
public string SwitchExpression(int temperature) => temperature switch
{
    _ when temperature < 0 => "Unnaturally cold",
    _ when temperature > 0 && temperature < 11 => "Cold",
    _ when temperature > 11 && temperature < 21 => "Moderate",
    _ when temperature > 21 && temperature < 31 => "Hot",
    _ when temperature > 31 => "Very Hot",
    _ => "Surface of the sun"
};
```

In this example, the discard character (_), is also used instead of an unused variable as in the previous example.

## Multi value switch expression

With C#8, multiple values can also be used when checking the switch condition. In the below example, the `two values are converted to a tuple` and then the `tuple is used in the switch expression`:

``` csharp
public string SwitchExpression(int temperature, SkyOutlook sky) 
    => (temperature, sky) switch
    {
        _ when temperature < 0 && sky == SkyOutlook.Sunny => "Cold but hot",
        _ when temperature < 0 && sky == SkyOutlook.Rainy => "Actually snowing",
        _ when temperature > 31 && sky == SkyOutlook.Sunny =>  "Get to air-con",
        _ => "Unable to determine"
    };
```

---

## Switch on a class - part 1

Another C#8 enhancement, was the ability to `use a class in the switch expression and switch on a class property(s)`.

Consider a `Person` class which contains a person's name and age:

``` csharp
public string ClassSwitchExpression(Person person) => person switch
{
    { Age: < 10 } => "Younger than 10",
    { Age: < 20 } => "Younger than 20",
    { Age: < 30 } => "Younger than 30",
    { Age: < 50 } => "Younger than 50",
    _ => "Older than 50"
};
```

---

## Switch on a class - part 2

When using a class to switch on, its also possible to use properties of a child class in the condition.

Consider the `Person` class has a _Country_ property, which is the `Country` class. The _Country_ class has a _Hemisphere_ property indicating northern or southern hemisphere:

``` csharp
public string EnhancedClassSwitchExpressionCountry(Person person) => person switch
{
    { Age: < 10 } => "Younger than 10",
    { Age: >= 10 and < 20 } => "Between 10 and 20",
    { Age: >= 20 and < 30, Country: { Hemisphere: "North" } } => 
        "Between 19 and 30, living in north hemisphere",
    { Age: >= 20 and < 30, Country: { Hemisphere: "South" } } => 
        "Between 19 and 30, living in south hemisphere",
    { Age: >= 30 and < 50 } => "Between 29 and 40",
    _ => "Older than 50"
};
```

---

## Switch on a class - part 3

C#10 cleaned up the syntax, which allowed for `easier access to the properties of the child class`:

``` csharp
public string EnhancedClassSwitchExpressionCountry2(Person person) => person switch
{
    { Age: < 10 } => "Younger than 10",
    { Age: >= 10 and < 20 } => "Between 10 and 20",
    { Age: >= 20 and < 30, Country.Hemisphere: "North" } => 
        "Between 19 and 30, living in north hemisphere",
    { Age: >= 20 and < 30, Country.Hemisphere: "South" } => 
        "Between 19 and 30, living in south hemisphere",
    { Age: >= 30 and < 50 } => "Between 29 and 40",
    _ => "Older than 50"
};
```

---

## Notes

From a basic switch statement to a switch expression, with multiple conditional enhancement the switch functionality has evolved and continues to evolve to make it more feature rich and easier to use for developers.

---

## References
[What's new in C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)  
[What's new in C# 9.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9)  
[C#10 Extended property patterns](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#extended-property-patterns)

<?# DailyDrop ?>29: 11-03-2022<?#/ DailyDrop ?>
