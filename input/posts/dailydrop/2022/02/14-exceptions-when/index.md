---
title: "Filtering try-catch statement"
lead: "Apply filtering to catch statements with a when clause"
Published: 02/14/2022
slug: "14-exceptions-when"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - exception
    - when
    - filter
    
---

## Daily Knowledge Drop

It is possible to `filter the catch portion of the try-catch statement`, as well as catch `multiple exceptions at the same time` using the C# _when_ keyword.

---

## Multiple exceptions

In our use case, there are two exception types we are particularly interested in `IndexOutOfRangeException` and `DivideByZeroException`.  
If either of these exceptions occur, we want to **log the exception**, and carry on with the workflow. However if any other exception occurs, it should be logged and re-thrown to be handled higher up the call stack.

### Multiple catch

Usually this would be done with multiple catch statements:

``` csharp
try
{
    // array only has 2 elements
    var array = new int[2];
    // trying to access 3rd element. not allowed
    // this will be caught by the first catch statement
    array[3] = 100;
}
catch (IndexOutOfRangeException ex) 
{
    Console.WriteLine("Caught IndexOutOfRangeException: Log and ignore" );
}
catch (DivideByZeroException ex)
{
    Console.WriteLine("Caught DivideByZeroException: Log and ignore");
}
catch (Exception ex)
{
    Console.WriteLine("Caught all other exceptions: Log and throw");
    throw;
}
```

Here there are three different catch statements each handling a different type of exception. However for the first two types of exception, the `same logic needs to take place`, so lets `group these two exceptions together`. 

---

### When clause

The _when_ clause can be used to group multiple exceptions together:

``` csharp
try
{
    // divide by zero. not allow
    // this will be caught by the first catch statement
    // along with any IndexOutOfRange exceptions
    var zero = 0;
    var value = 100 / zero;
}
catch (Exception ex) when (ex is IndexOutOfRangeException ||
                            ex is DivideByZeroException)
{
    Console.WriteLine("Caught IndexOutOfRangeException or " +
        "DivideByZeroException: Log and ignore");
}
catch (Exception ex)
{
    Console.WriteLine("Caught all other exceptions: Log and throw");
    throw;
}
```

The type of the exception being caught is checked. If the exception type is one of the two we are interested in, the first catch block is entered, otherwise the exception falls through to the second catch block.

---

## Exception filtering

In our use case, we have defined a custom exception, with a _Status_ field. Based on the value of the _Status_ field, one of three things should happen:
- **log** the exception
- **log and re-thrown** the exception
- **ignore** the exception.

### Switch

Usually for filtering a switch could be done within the catch statement:

``` csharp
try
{
    throw new StatusException(3);
}
catch (StatusException ex)
{
    switch (ex.Status)
    {
        case 0:
        case 1:
            Console.WriteLine($"Logging exception with status: {ex.Status}");
            Console.WriteLine(ex.Message);
            break;
        case 3:
        case 5:
            Console.WriteLine($"Ignoring exception with status: {ex.Status}");
            break;
        default:
            Console.WriteLine($"Throwing exception with status: {ex.Status}");
            throw;
    }
}

class StatusException : Exception
{
    public int Status;

    public StatusException(int status)
    {
        Status = status;
    }
}
```

---

The _when_ clause can also be used to `filter the catch statement, based on properties of the exception`.

### When clause

Below we are filtering which catch block should handling the exception based on the _Status_ field value:

``` csharp
try
{
    throw new StatusException(1);
}
catch (StatusException ex) when (ex.Status == 0 || ex.Status == 1)
{
    Console.WriteLine($"Logging exception with status: {ex.Status}");
    Console.WriteLine(ex.Message);
}
catch (StatusException ex) when (ex.Status == 3 || ex.Status == 5)
{
    Console.WriteLine($"Ignoring exception with status: {ex.Status}");
}
catch (StatusException ex)
{
    Console.WriteLine($"Throwing exception with status: {ex.Status}");
    throw;
}

class StatusException : Exception
{
    public int Status;

    public StatusException(int status)
    {
        Status = status;
    }
}
```

---

## Conclusion

The _when_ clause is a useful addition to the existing techniques to handle multiple exceptions and allow for decision making based on exception values. 

---

## References
[Catch Multiple Exceptions in C#](https://code-maze.com/csharp-catch-multiple-exceptions/)


<?# DailyDrop ?>10: 14-02-2022<?#/ DailyDrop ?>
