---
title: "Using with IDisposable"
lead: "Use using by implementing a single interface"
Published: 02/17/2022
slug: "17-using-idisposable"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - idisposable
    - using
    - disposable
    
---

## Daily Knowledge Drop

To make use of the `using` statement in C#, all you need to do is implement the `IDisposable` interface on a class.  

_The `using` statement provides a convenient syntax to ensure the correct use of IDisposable objects_. The object in question will exists for the scope of the `using` and then automatically be _disposed_ once out of scope.  

This functionality can also be leveraged to create scoped helper instances for certain use cases.

---

## Examples

All the example below make use of a simple **_Order_** with multiple **_OrderLines_** entity structure.

``` csharp
class Order
{
    public Guid Id { get; set; }

    public DateTime DateCreated { get; set; }

    public OrderLine[] Lines { get; set; }

    // Provide an convenient way to output the order
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, 
            new JsonSerializerOptions { WriteIndented = true });
    }
}

class OrderLine
{
    public Guid Id { get; set; }

    public DateTime DateCreated { get; set; }

    public Guid OrderId { get; set; }
}
```

When an _Order_ is created, the _OrderLines_ should be created at the same time. Both entities are required to have the `same DateCreated` value and we also require the `OrderLine.OrderId to be set to the Order.Id`

For something so relatively simple, there are a couple of issues.  

See the code below which **DOES NOT COMPILE**:

``` csharp
var order = new Order
{
    Id = Guid.NewGuid(),
    DateCreated = DateTime.Now,
    Lines = new[]
    {
        new OrderLine
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.Now,
            // How to set this???
            OrderId = ???
        }
    }
};
```

The issues:
1. The two _DateCreated_ values will not be the same (off by probably only a few nano or milliseconds, but still off)
1. There is no way to easily set the OrderId value on the _OrderLine_ entity


### Variable snapshot

These issues can quite easily be solved by using variables to snapshot values before the entity creation:

``` csharp
// snapshot values to be used later
var datetimeCreated = DateTime.Now;
var orderId = Guid.NewGuid();

var order = new Order
{
    // use the snapshot orderId
    Id = orderId,
    // use the snapshot date created
    DateCreated = datetimeCreated,
    Lines = new[]
    {
        new OrderLine
        {
            Id = Guid.NewGuid(),
            // use the snapshot date created
            DateCreated = datetimeCreated,
            // use the snapshot orderId
            OrderId = orderId
        }
    }
};
```

This will 100% work and is completely acceptable to do - however a cleaner way, which makes use of the `using` statement.

---

### Using snapshot

First thing is to define a _OrderInfoSnapshot_ class which implements _IDisposable_:

``` csharp

// Implement IDisposable
class OrderInfoSnapshot : IDisposable
{
    private DateTime _created;
    private Guid _orderId;

    public DateTime Created => _created;

    public Guid OrderId => _orderId;

    // On creation of an instance, we 
    // snapshot the values we are interested in
    public OrderInfoSnapshot()
    {
        _created = DateTime.Now;
        _orderId = Guid.NewGuid();
    }

    // Implement the IDisposable Dispose method. This is a very 
    // simple example, but more sophisticated cleanup can happen here
    public void Dispose()
    {
        // do cleanup etc here
        // look at resources for correct
        // Dispose usage
    }
}
```

Now instead of using a number of variables for the various snapshots required, _OrderInfoSnapshot_ can be used with a `using` statement:

``` csharp
// create the snapshot
using (var orderInfo = new OrderInfoSnapshot())
{
    var order = new Order
    {
        // grab the OrderId from the snapshot
        Id = orderInfo.OrderId,
        // Create from the snapshot
        DateCreated = orderInfo.Created,
        Lines = new[]
        {
            new OrderLine
            {
                Id = Guid.NewGuid(),
                // Create from the snapshot
                DateCreated = orderInfo.Created,
                // OrderId from the snapshot
                OrderId = orderInfo.OrderId
            }
        }
    };

    Console.WriteLine(order);
}
```

The _OrderInfoSnapshot_ will automatically be disposed (which in this case doesn't need to do much cleanup) once the end of the `using` scope is reached.

As mentioned, this is a very simple example, but if the _OrderInfoSnapshot_ was required to access multiple other resources, all the connections could be cleaned up in the _Dispose_ method.

---

## Alternative syntax

Introduced in C# 8, there is an alternative syntax for `using`s - the end result and operation is exactly the same though.

``` csharp
// create the snapshot
using var orderInfo = new OrderInfoSnapshot();

var order = new Order
{
    Id = orderInfo.OrderId,
    DateCreated = orderInfo.Created,
    Lines = new[]
    {
        new OrderLine
        {
            Id = Guid.NewGuid(),
            DateCreated = orderInfo.Created,
            OrderId = orderInfo.OrderId
        }
    }
};

Console.WriteLine(order);
```

The scope of the _OrderInfoSnapshot_ instance is now the method it was created in, and it will still automatically be disposed when going out of scope.

---

## Notes

Implementing IDisposable _correctly_ is a bit more complicated than shown above - see the references for more details.

---

## Conclusion

The _IDisposable_ is not required on every class, but it makes sense and can be useful when you want to:
1. Explicitly dispose of resources used when going out of scope
1. Make use of the `using` syntax to be able to create a create a scoped entity (as in the above examples)

---

## References
[IDisposable Interface](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0)

<?# DailyDrop ?>13: 17-02-2022<?#/ DailyDrop ?>
