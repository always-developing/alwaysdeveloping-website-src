---
title: "Goto and switch statements"
lead: "Using goto statements with switch statements"
Published: 04/25/2022
slug: "25-goto-switch"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - goto
    - switch

---

## Daily Knowledge Drop

The `goto` statement can be used to transfer control to a statement marked with a label, but it can also be used in a switch statement to transfer control to a different switch section `using the switch case label`.

<?# InfoBlock ?>
This post, nor I, am endorsing the actual use of the goto statement - this post is for information purposes.  

The `goto` statement is an unstructured code flow statement, making code difficult to read and maintain. For the most part, a structured control flow statement (if, continue, break, for) should be used.
<?#/ InfoBlock ?>

---

## goto

First we'll look at how the `goto` statement can be used normally, outside of a switch statement.

In this example:
- An int array is filled with random numbers between 0 and 100
- The array is search for any numbers between 50 and 60
- A relevant output message is displayed if a number was found or not

``` csharp
var randomNumbers = new int[20];

// populate the array with random numbers
Random randGen = new Random();
for (int i = 0; i < randomNumbers.Length; i++)
{
    randomNumbers[i] = randGen.Next(0, 100);
}

// iterate through each item and check the value
for (int i = 0; i < randomNumbers.Length; i++)
{
    if (randomNumbers[i] > 50 && randomNumbers[i] < 60)
    {
        // The goto statement is used to break out the loop 
        // if a relevant number is found and direct the 
        // flow to the `NumFound` label
        goto NumFound;
    }
}

Console.WriteLine("Number between 50 and 60 NOT FOUND");
// The goto statement is used to circumvent Console.WriteLine 
// method below, and direct the flow to the `Exit` label
goto Exit;

// Below, two labels are defined, each with a unique name

NumFound:
Console.WriteLine("Number between 50 and 60 FOUND");

Exit:
Console.WriteLine("Application closing");
```

---

For completeness, below is code sample (one of a possible many) on how the same can be achieved without using the `goto` statement:

``` csharp
var randomNumbers = new int[20];

Random randGen = new Random();
for (int i = 0; i < randomNumbers.Length; i++)
{
    randomNumbers[i] = randGen.Next(0, 100);
}

bool found = false;
for (int i = 0; i < randomNumbers.Length; i++)
{
    if (randomNumbers[i] > 50 && randomNumbers[i] < 60)
    {
        found = true;
        break;
    }
}

Console.WriteLine($"Number between 50 and 60{(found ? "" : " NOT")} FOUND");
Console.WriteLine("Application closing");
```

---

## Switch goto 

The `goto` statement can also be used inside a switch statement, to jump to any of the other switch cases. The `switch case label can be used in the same was as a label` in the above example.

In this example, the price of a service is determined by the service type, which will adjust the base price accordingly:

``` csharp
public enum ServiceType
{
    Basic,
    Premium,
    UltraFast,
    Enterprise
}

public int CalculatePrice(ServiceType serviceType)
{
    int price = 0;

    switch (serviceType)
    {
        case ServiceType.Basic:
            price += 10;
            break;

        case ServiceType.Premium:
            price += 10;
            goto case ServiceType.Basic;

        case ServiceType.UltraFast:
            price += 20;
            goto case ServiceType.Premium;

        case ServiceType.Enterprise:
            price += 10;
            goto case ServiceType.Premium;
    }

    return price;
}
```

As you can see, the `goto case` is used to just from one switch case to another, using the `switch case label`, adjusting the price as required.

---

## Notes

The `goto` statement should almost never be used, or be required to be used - however in the small niche uses cases where it is required, an understanding of how and when it can be used will be essential.

---

## References

[The goto statement](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/jump-statements#the-goto-statement)  
["goto" statement should not be used](https://rules.sonarsource.com/csharp/RSPEC-907)  

<?# DailyDrop ?>59: 25-04-2022<?#/ DailyDrop ?>
