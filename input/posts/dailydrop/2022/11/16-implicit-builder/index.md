---
title: "Builder pattern implicit conversions"
lead: "Leveraging implicit conversions to make using the builder pattern easier"
Published: "11/16/2022 01:00:00+0200"
slug: "16-implicit-builder"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - pattern
   - builder
   - implicit
   - conversion

---

## Daily Knowledge Drop

Usually when using the `builder pattern`, a final `Build` method (or similarly named method) is invoked to generate the _final entity_ from the _builder entity_. Using an `implicit operator`, this final step can be removed, and the _final entity_ can `automatically be generated` from the _builder entity_.


---

## Examples

In these examples, there is a simple `Order` entity:

``` csharp
public record Order(int Id, int lineItemCount, double Total);
```

There is also an `OrderBuilder` class, which is used to build an `Order`:

``` csharp
public class OrderBuilder
{
    // store some order information
    private int _id;
    private int _lineItemCount;
    private double _total;

    // method to give an order an id
    public OrderBuilder WithId(int Id)
    {
        _id = Id;
        return this;
    }

    // add a simple line item to the order
    public OrderBuilder WithLineItem(double lineItemAmount)
    {
        _lineItemCount++;
        _total += lineItemAmount;

        return this;
    }

    // create an Order from the OrderBuilder information
    public Order Build()
    {
        return new Order(_id, _lineItemCount, _total);
    }
}
```

---

### Build method

Usage of the above _builder_ is easy:

``` csharp
var order = new OrderBuilder()
    .WithId(100) // give the order an id
    .WithLineItem(99) // add line items
    .WithLineItem(149)
    .Build(); // remember to build!

Console.WriteLine(order);
```

The above is the typical usage of the _builder pattern_ - building up the entity using various methods, and then at the end calling a `Build` method to create the final entity (`Order` in this example) using the information supplied to the _builder_.

The output of the order:

``` terminal
Order { Id = 100, lineItemCount = 2, Total = 248 }
```

Nothing inherently wrong or incorrect with this code or approach - however an implicit operator can be added to the `OrderBuilder` class to make the pattern and classes even easier to use.

---

### Implicit

Adding the following `implicit operator` to the _OrderBuilder_ class, allows for the `Build` method to _automatically be invoked_ when converting the _OrderBuilder_ instance to an _Order_ instance:

``` csharp
public static implicit operator Order(OrderBuilder b) => b.Build();
```

With the above added to the _OrderBuilder_ class, the following is now possible:

``` csharp
// instead of "var", the actual type
// Order is used
Order order = new OrderBuilder()
    .WithId(101)
    .WithLineItem(100)
    .WithLineItem(29);
// No Build required!

Console.WriteLine(order);
```

With this code snippet, as the _OrderBuilder_ is being converted to _Order_ implicitly, the `implicit operator` is called which calls the _Build_ method to convert the _OrderBuilder_ to an _Order_ automatically. No need for the `Build` method to be used!


---

## Notes

This is a small update to the code, but for the developers consuming the classes, the overall _developer experience_ is improved. This gives the developer the flexibility to use the class how they would like - either using the _Build_ method, or doing the _implicit conversion_.  

---

## References

[Bonus: Builder Pattern with the implicit operator using c#](https://josef.codes/bonus-builder-pattern-with-the-implicit-operator-using-c-sharp/)  

<?# DailyDrop ?>203: 16-11-2022<?#/ DailyDrop ?>
