---
title: "Primitive obsession and Value Records "
lead: "Leveraging records to assist overcoming primitive obsession"
Published: "11/03/2022 01:00:00+0200"
slug: "03-value-records"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - record
   - primitive

---

## Daily Knowledge Drop

The C# `record` type can provide a clean, quick and easy way to overcome _primitive obsession_.

_Primitive obsession_, as the name implies, is the overuse of primitive types (int, string, Guid, etc) to represent more complex business or domain concepts.

---

## Primitive 

Let's look at an example, of a (very simplified) _Order_ entity:

``` csharp
public class Order
{
    public int OrderId { get; set; }

    public decimal OrderTotal { get; set; }

    public int ItemCount { get; set; }
}
```

On the surface, this is all good. This will work and is probably what most developer are familiar with when it comes to entities. 

However, the above class structure allows for the following code:

``` csharp
var order = new Order
{
    OrderId = 100,
    ItemCount = 2,
    OrderTotal = 148.95m
};

// This doesn't make sense!
order.OrderId = order.OrderId * 2;
// No!?
order.ItemCount = -10;
```

As primitive types are used for _domain business concepts_ such as `OrderId` or `ItemCount`, these values can be manipulated in a which doesn't make sense in their business or domain context.

Of course an option is to constantly perform validation on the _Order_ to ensure it is in a valid state - but another option is to use `Value Objects`, which will be explored in the next section.

---

## Value object

A `Value object` is a simple, light-weight wrapper around the primitive type, which can also provide some validation for the internal primitive type.

### Value record

In the referenced article, Stephen Cleary introduces a concept called `Value Record` - leveraging the C# `record` and `struct` types, a simple _Value Object_ can easily be defined with one line of code.

To replace the _int OrderId_ above:

``` csharp
public readonly record struct OrderId(int Value);
```

Using this technique has a number of benefits:
- immutability
- equality, hash code and _ToString_ support built in
- value-type wrapper, with no additional memory allocated

---

### Usage

The _Order_ entity can now be updated to use the _OrderId_ `value record` (as well as other properties as well):

``` csharp
public class Order
{
    public OrderId OrderId { get; set; }

    public OrderTotal OrderTotal { get; set; }

    public ItemCount ItemCount { get; set; }
}


public readonly record struct OrderId(int Value);

public readonly record struct OrderTotal(decimal Value);

public readonly record struct ItemCount(int Value);
```

And the usage:

``` csharp
var order = new Order
{
    OrderId = new OrderId(100),
    ItemCount = new ItemCount(2),
    OrderTotal = new OrderTotal(148.95m)
};

// An added benefit of records (over a class) 
// is that they can be printed as well
// The ToString() method can also be overwritten
// if required
Console.WriteLine(order.OrderId);

// ERROR - compilation error 
// this is now not possible as the 
// OrderId is immutable

// order.OrderId.Value = order.OrderId.Value * 2;
```

The output of the above:

``` terminal
OrderId { Value = 100 }
```

The `value records` can be expanded with additional (light-weight) validation, but the _record struct_ method shown above allows for a quick, simple implementation.

---

## Notes

A very useful as well as quick easy technique to implement - but also an technique which can be misused (as it is so easy to implement). Not all properties should be or need to be converted to a value object. However where it does make sense, the `value record` method is a slick, minimal way to achieve this.

---

## References

[Modern C# Techniques, Part 2: Value Records](https://blog.stephencleary.com/2022/10/modern-csharp-techniques-2-value-records.html)  

<?# DailyDrop ?>194: 03-11-2022<?#/ DailyDrop ?>
