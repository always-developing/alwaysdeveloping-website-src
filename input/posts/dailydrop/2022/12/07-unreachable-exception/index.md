---
title: "Unreachable code and UnreachableException"
lead: "UnreachableException - the new exception which should never be thrown"
Published: "12/07/2022 01:00:00+0200"
slug: "07-unreachable-exception"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - exception
   - .net7
   - unreachable

---

## Daily Knowledge Drop

The `UnreachableException` was introduced in .NET 7, which is used in situations _when the application executes code thought to be unreachable_. If this exception does occur, then there is an error in the flow or data of the application causing the, previous thought, unreachable code to be executed.

---

## Unreachable code setup

Consider the follow example of example - we have an `enum` of _OrderStatus_:

``` csharp
public enum OrderStatus
{
    New = 0,
    Processing = 1,
    Fulfilled = 2,
    OutOnDelivery = 3,
    Delivered = 4
}
```

As well as a switch expression to convert the `enum` value to a `string description` :

``` csharp
string currentStatusText = currentStatus switch
{
    OrderStatus.New => "Order Placed",
    OrderStatus.Processing => "Processing Order",
    OrderStatus.Fulfilled => "Processing Order",
    OrderStatus.OutOnDelivery => "Order is out on delivery",
    OrderStatus.Delivered => "Order is delivered",
    _ => throw new UnreachableException($"OrderStatus enum " +
        $"value {currentStatus} invalid")
};
```

The `UnreachableException` is used if the code tries to convert an `OrderStatus which does not exist` to a string - in theory a situation which should never occur.

We also have a method to retrieve the order status, `stored as an int`, from the database:

``` csharp
public OrderStatus GetOrderStatusFromDatabase()
{
    // simulate getting the value from the database
    return (OrderStatus)2;
}
```

---

## Executing unreachable code

If the database stores a valid _OrderStatus_ int value, everything will execute as expected.

In this example, the database `stores an OrderStatus value of 2`:

``` csharp
// currentStatus is 2
OrderStatus currentStatus = GetOrderStatusFromDatabase();

var currentStatusText = currentStatus switch
{
    OrderStatus.New => "Order Placed",
    OrderStatus.Processing => "Processing Order",
    OrderStatus.Fulfilled => "Processing Order",
    OrderStatus.OutOnDelivery => "Order is out on delivery",
    OrderStatus.Delivered => "Order is delivered",
    _ => throw new UnreachableException()
};

Console.WriteLine($"Order status: {currentStatusText}");
```

The output is:
``` terminal
Order: Processing Order
```

However, if the _OrderStatus_ was manually, incorrectly `updated to be 5` in the database - this is a situation which _should never happen, but in reality it could_:

``` csharp
// currentStatus is 5
OrderStatus currentStatus = GetOrderStatusFromDatabase();

string currentStatusText = currentStatus switch
{
    OrderStatus.New => "Order Placed",
    OrderStatus.Processing => "Processing Order",
    OrderStatus.Fulfilled => "Processing Order",
    OrderStatus.OutOnDelivery => "Order is out on delivery",
    OrderStatus.Delivered => "Order is delivered",
    _ => throw new UnreachableException($"OrderStatus enum " +
        $"value {currentStatus} invalid")
};

Console.WriteLine($"Order status: {currentStatusText}");
```

The C# code `allows the currentStatus variable to be set to a value of 5, an enum value which doesn't exist` - and only when it comes time to convert to a string (in the  switch expression), will there be no match to any of the options and the `UnreachableException` be thrown.

Monitoring tools or logs can now be checked for the presence of `UnreachableException` and if such an exception occurs, something has gone wrong.

---

## Notes

A useful tool at a developers disposal to assist in tracking down completely unexpected issues which may arise - but a tool which should hopefully never actually be reached to be used!

---


## References

[The new .NET Exception that should NEVER be thrown](https://www.youtube.com/watch?v=s_NrqRI7Gnc&t=308s)  

<?# DailyDrop ?>218: 07-12-2022<?#/ DailyDrop ?>
