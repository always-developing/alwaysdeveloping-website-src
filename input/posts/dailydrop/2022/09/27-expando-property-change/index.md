---
title: "dynamic, ExpandoObject and INotifyPropertyChanged"
lead: "Adding a property changed event handler to a ExpandoObject object"
Published: "09/27/2022 01:00:00+0200"
slug: "27-expando-property-change"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - expando
   - property
   - propertychanged

---

## Daily Knowledge Drop

`ExpandoObject` implements _INotifyPropertyChanged_ - a callback delegate can be added to be invoked when a property value on the `ExpandoObject` instances changes.

---

## ExpandoObject

A quick overview of `ExpandoObject` - it represents an object whose members can be dynamically added and removed at run time.

In the examples below, it is being used to represent a _product_:

``` csharp
dynamic product = new ExpandoObject();
product.Name = "Green Shirt";
product.Rating = 4.3;
```

The properties _Name_ and _Rating_ are added at runtime. The properties can then be accessed as if they were traditional class properties:

``` csharp
Console.WriteLine(product.Name);
Console.WriteLine(product.Rating);
```

---

## Calculation

In the examples below, we have a collection of _Products_, each with a customer rating: 

``` csharp
public dynamic[] GetProducts()
{
    dynamic prod1 = new ExpandoObject();
    prod1.Name = "Green Shirt";
    prod1.Rating = 4.3;

    dynamic prod2 = new ExpandoObject();
    prod2.Name = "Blue Shirt";
    prod2.Rating = 4.9;

    dynamic prod3 = new ExpandoObject();
    prod3.Name = "Shoes";
    prod3.Rating = 3.8;

    dynamic prod4 = new ExpandoObject();
    prod4.Name = "Jeans";
    prod4.Rating = 5.0;

    dynamic prod5 = new ExpandoObject();
    prod5.Name = "Peak cap";
    prod5.Rating = 1.7;

    return new dynamic[] { prod1, prod2, prod3, prod4, prod5 };
}
```

We want to find the `average rating for all products`:

``` csharp
public void ComputeAverageRating()
{
    avgRating = products.Average(p => (double)p.Rating);
}
```

---

### Manual invocation

Getting the average for all products is straight forward - call the `ComputeAverageRating` method:

``` csharp
var products = GetProducts();

var avgRating = products.Average(p => (double)p.Rating);
Console.WriteLine($"Average product rating is: {avgRating}");
```

However, every time a rating on one of the _Products_ changes, the method needs to manually be called again:

``` csharp
var products = GetProducts();

var avgRating = products.Average(p => (double)p.Rating);
Console.WriteLine($"Average product rating is: {avgRating}");

// the product rating changed from 5 to 4.5
products[3].Rating = 4.5;

avgRating = products.Average(p => (double)p.Rating);
Console.WriteLine($"Average product rating is: {avgRating}");
```

The output from the above is:

``` terminal
Average product rating is: 3,94
Average product rating is: 3,84
```

While this method works without issue, as mentioned, it requires manually recalculating the average every time one of the _Rating_ property values changes. 

A different method, requiring less manual work - is to leverage the `INotifyPropertyChanged` functionality of `ExpandoObject`.

---

### INotifyPropertyChanged

With this method, once we have the list of _Products_ we can `register a method to be called whenever a property on the Product changes`:

``` csharp
void RegisterPropertyChange(dynamic[] products)
{
    foreach(var product in products)
    {
        // cast each product to INotifyPropertyChanged
        // and register a callback method to be called
        // every time a property changes.
        // The cast is valid as ExpandoObject implements
        // INotifyPropertyChanged
        ((INotifyPropertyChanged)product).PropertyChanged += 
            new PropertyChangedEventHandler(
                (sender, e) =>
                {
                    // when a property changes, call this method
                    ComputeAverageRating();
                }
            );
    }

    // calculate the average for the first time once 
    // the callbacks have all been registered
    ComputeAverageRating();
}

public void ComputeAverageRating()
{
    // avgRating is defined on the parent class
    avgRating = products.Average(p => (double)p.Rating);
}
```

This can now be utilized as follows:

``` csharp
var products = GetProducts();
RegisterPropertyChange(products);

Console.WriteLine($"Average product rating is: {avgRating}");

products[3].Rating = 4.5;

Console.WriteLine($"Average product rating is: {avgRating}");
```

With this approach, the average rating calculation doesn't have to be called manually:

``` terminal
Average product rating is: 3,94
Average product rating is: 3,84
```

---

## Notes

While the `ExpandoObject` is a very useful class, it does have it's drawbacks, performance being a big one. However, when it is required it can prove to be invaluable - with the bonus advantage of being able to leverage the `INotifyPropertyChanged` functionality if required.

---

## References

[How to Create a Class Dynamically in C#?](https://code-maze.com/create-class-dynamically-csharp/)   

<?# DailyDrop ?>169: 27-09-2022<?#/ DailyDrop ?>
