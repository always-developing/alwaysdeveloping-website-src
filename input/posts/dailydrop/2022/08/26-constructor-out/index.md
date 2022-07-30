---
title: "Constructor out parameters"
lead: "A constructor can contain out and ref parameters"
Published: "08/26/2022 01:00:00+0200"
slug: "26-constructor-out"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - constructor
   - out
   - parameter

---

## Daily Knowledge Drop

Class constructors can contain `out or ref parameters`, and operate just as they would on a normal method.

---

## Usages

While not a very common feature to make use of in most code, below we'll look at a few use cases when an `out` parameter on a constructor _might_ be useful. There may definitely be better ways of handling these specific use cases, but using an `out` parameter is _a way to handle them_.

---

### Dependent entity 

Consider the situation when `instantiating a class, another class is created at the same time using the same information`. 

In the below example, we have a `Product` class to store product information (including the current price) and a `ProductPrice` class to store the history of prices for the product.

When creating a `Product` record, `ProductPrice` record should be created at the same time:

``` csharp
var product = new Product(Guid.NewGuid(), "Green pants", 99.99m);
var price = new ProductPrice
{
    Id = product1.Id,
    Price = product1.Price
};
```

This could however also be done using an `out` parameter. Consider the following _constructor_:

``` csharp
public Product(Guid id, string name, decimal price, out ProductPrice productPrice)
{
    Id = id;
    Name = name;
    Price = price;

    productPrice = new ProductPrice 
    { 
        Id = id, 
        Price = price 
    };
}
```

Which can then be called as follows:

``` csharp
var product = new Product(Guid.NewGuid(), "Green pants", 99.99m, out ProductPrice price);
```

More concise? Yes. A better solution? Debatably.

---

### Generated information

Expanding on the above example, maybe the `Id of the Product is generated internally to the class`, in the constructor. An `out` parameter could be used to return the instance _Id_ from the constructor.

Here the _Id_ is generated in the constructor, and assigned to the `out` parameter as well as the _Id_ property of thd class itself:

``` csharp
public Product(string name, decimal price, out Guid id)
{
    id = Id = Guid.NewGuid();
    Name = name;
    Price = price;
}
```

The usage of the constructor:

``` csharp
var product = new Product("Green pants", 99.99m, out Guid productId);
```

Again - more concise than some alternatives? Yes. A better solution? Debatably.

---

## Notes

This is not something I'd ever really considered, or ever required in 20 years of development - however it is an interesting feature. Maybe one day I might have a  a practical use to use it in production code.

---

## References

[Roman Marusyk Tweet](https://twitter.com/MarusykRoman/status/1550995942576476162)   

---

<?# DailyDrop ?>147: 26-08-2022<?#/ DailyDrop ?>
