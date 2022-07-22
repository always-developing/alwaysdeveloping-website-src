---
title: "Combining enum values"
lead: "Using bit flags to combine enum values to simplify code"
Published1: "08/18/2022 01:00:00+0200"
slug: "18-enum-bit-flags"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - bit
   - flags
   - enum

---

## Daily Knowledge Drop

The `bitwise logical operator (|)` can be used to _combine enum values_ when assigning to a variable, but they can also be used when `defining the enum` to create enum values which are a `combination of other enum values`.

---

## Combine variable

This is the more common (in my experience) usage of combining enums - assigning enum value(s) to a variable.

Consider the following enum:

``` csharp
[Flags]
enum OrderStatus
{
    New = 1,
    Processing = 2,
    Reserved = 4,
    Paid = 8,
    Delivered = 16,
    Cancelled = 32
}
```

We could indicate that an order is _Paid_ but also been _Cancelled_ by doing the follow:

``` csharp
OrderStatus status = OrderStatus.Paid | OrderStatus.Cancelled;

Console.WriteLine($"Is order Paid? {status.HasFlag(OrderStatus.Paid)}");
Console.WriteLine($"Is order Cancelled? {status.HasFlag(OrderStatus.Cancelled)}");
```

With the output being:

``` terminal
Is order Paid? True
Is order Cancelled? True
```

The `bitwise logical operator (|)` can also be used when `declaring the enum` to combine statues.

---

## Combine declaration

Consider the same enum from above:

``` csharp
[Flags]
enum OrderStatus
{
    New = 1,
    Processing = 2,
    Reserved = 4,
    Paid = 8,
    Delivered = 16,
    Cancelled = 32
}
```

Suppose we wanted two methods to determine if a order is currently `Open` (still processing to do) or if it is `Closed` (no more processing to do).

With the enums as they are, something similar to this would need to be done:

``` csharp
// check all the statuses considered 'open'
bool IsOrderOpen(OrderStatus status) => status.HasFlag(OrderStatus.New) ||
        status.HasFlag(OrderStatus.Processing) ||
        status.HasFlag(OrderStatus.Reserved) ||
        status.HasFlag(OrderStatus.Paid);

// check all the statuses considered 'closed'
bool IsOrderClosed(OrderStatus status) => status.HasFlag(OrderStatus.Delivered) ||
        status.HasFlag(OrderStatus.Cancelled);
```

Each time a new status is added and the enum changes, the developer would need to remember to come in and update each method as well.

A cleaner approach, is to `define a new status, and use it to combine with other statuses`.


Consider this `updated enum`:

``` csharp
[Flags]
enum OrderStatus
{
    New = 1 | 64,
    Processing = 2 | 64,
    Reserved = 4 | 64,
    Paid = 8 | 64,
    Delivered = 16 | 128,
    Cancelled = 32 | 128,
    Open = 64,
    Closed = 128
}
```

Two new statuses, `Open` and `Closed`, have been defined, with each other status being updated to indicate it as being either _Open_ or _Closed_.
For example `Processing = 2 | 64,` effectively means: `an order with a status of Processing is Processing but also Open`.

To check if an order is `Open` or `Closed` now, all that is required is the following:

``` csharp
bool IsOrderOpen(OrderStatus status) => status.HasFlag(OrderStatus.Open);
bool IsOrderClosed(OrderStatus status) => status.HasFlag(OrderStatus.Closed);
```

Using the methods:

``` csharp
// the status is not set explicitly to 'Open' or 'Closed'
var status = OrderStatus.Reserved;

Console.WriteLine($"Is order open? {IsOrderOpen(status)}");
Console.WriteLine($"Is order closed? {IsOrderClosed(status)}");

// just to confirm a check on the original status 
// still works
Console.WriteLine($"Is order reserved? {status.HasFlag(OrderStatus.Reserved)}");
```

The output:

``` terminal
Is order open? True
Is order closed? False
Is order reserved? True
```

This version of the code is definitely `much cleaner`, and much `easier to maintain`!

---

## Notes

Using the _bitwise logical operator (|)_ on enum definition is a useful technique I was not aware of. It facilities cleaner code, and makes it easier for the developer to contain enum logic all in one place.

---

## References

[Enum bit-shifting syntax for multiple flags during enum declaration](https://www.reddit.com/r/csharp/comments/vzph36/enum_bitshifting_syntax_for_multiple_flags_during/)   

---

<?# DailyDrop ?>141: 18-08-2022<?#/ DailyDrop ?>
