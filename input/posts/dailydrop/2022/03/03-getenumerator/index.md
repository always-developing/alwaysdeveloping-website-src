---
title: "Enumeration on a custom class"
lead: "How to make a custom class enumerable (work with foreach)"
Published: 03/03/2022
slug: "03-getenumerator"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - iteration
    - enumeration
    - getenumerator
    - foreach
    
---

## Daily Knowledge Drop

To enable `enumeration` on a class (ability to `foreach` on the class), the only requirement is for a method named `GetEnumerator` to exist on the class.

There is no requirement for the class to implement any interface (IEnumerable, IEnumerator etc), just the presence of the method is enough.  

---

## Without GetEnumerator

Last week we looked at [adding indexing to a class](../../02/23-indexers) - adding `enumeration` is similar, so we'll use similar examples.

In the examples below, we are using a `ProductPrice` entity. This class keeps some basic details of a product, as well as an array of prices, one price for each month of the year. So _Prices[0]_ is the price for January, _Prices[1]_ is the price for February and so on.

``` csharp
public class ProductPrice
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public decimal[] Prices { get; set; }

    public ProductPrice(int productId, string productName, decimal[] prices)
    {
        ProductId = productId;
        ProductName = productName;
        Prices = prices;
    }
}
```

To iterate through each price for the product, we can do this via the _Prices_ property exposed on the class.

``` csharp
var pp = new ProductPrice(112, "Green t-shirt", 
    new decimal[] { 0, 0, 0, 100, 100, 80, 80, 50, 50, 100, 100, 60 });

foreach (var price in pp.Prices)
{
    Console.WriteLine(price);
}
```

This code will function just fine - however the `ProductPrice` class can be extended to make it more intuitive and easier to work with.

---

## With GetEnumerator

The main purpose of the `ProductPrice` class is to store price information related to a product. So let's update the class so that we can access a price directly, without going via the _Price_ property.

`ProductPrice` class update:

``` csharp
public class ProductPrice
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    // Now private
    private decimal[] Prices { get; }

    public ProductPrice(int productId, string productName, decimal[] prices)
    {
        ProductId = productId;
        ProductName = productName;
        Prices = prices;
    }

    // New method called GetEnumerator added. 
    // The return type of the method is IEnumerator<T>, 
    // where T is the specific type being returned
    public IEnumerator<decimal> GetEnumerator()
    {
        //  The items in the underlying Prices array are 
        // `yield returned` one at a time
        foreach (var price in Prices)
            yield return price;
    }
}
```

If unfamiliar with the `yield` keyword, in short it indicates that the method in which it appears is an iterator, and will return one item at a time, one for each iteration.

The `ProductPrice` class can now be used as follows:

``` csharp
var pp = new ProductPrice(112, "Green t-shirt", 
    new decimal[] { 0, 0, 0, 100, 100, 80, 80, 50, 50, 100, 100, 60 });

// No need to go via Prices
foreach (var price in pp)
{
    Console.WriteLine(price);
}
```

---

## Field enumeration

Another use for `GetEnumerator` method is to be able to enumerate through each fields/property on a class.

In the below example, we have an `Address` class which has a number of string fields:

``` csharp
public class Address
{
    public string Number { get; set; }

    public string Line1 { get; set; }

    public string Line2 { get; set; }

    public string Suburb { get; set; }

    public string Town { get; set; }

    public string Province { get; set; }

    public Address(string number, string line1, string line2, 
        string suburb, string town, string province)
    {
        Number = number;
        Line1 = line1;
        Line2 = line2;
        Suburb = suburb;
        Town = town;
        Province = province;
    }

    public IEnumerator<string> GetEnumerator()
    {
        yield return Number;
        yield return Line1;
        yield return Line2;
        yield return Suburb;
        yield return Town;
        yield return Province;
    }
}
```

The `GetEnumerator` method has been added even though this class doesn't have an internal array of items to iterate over. Instead we will iterate over each property of the class.  
Each property of the class is `yield returned` one at a time (so each time the iterator is called, the next item will be returned)

This allows for accessing a property via an index. This can be leveraged to iteration through the properties on the class:

``` csharp
var address = new Address("11", "Main Road", "XYZ Estate", 
    "Suburb1", "Town2", "Province3");

foreach (var field in address)
{
    Console.WriteLine(field);
}
```

The output:

``` powershell
11
Main Road
XYZ Estate
Suburb1
Town2
Province3
```

---

## Notes

Similarly to working with **indexers**, adding your own `enumeration` will generally not be required for everyday use. However its advantageous to know that the option exists, and is as simple as adding a `GetEnumerator` method to the class in question.  

---

<?# DailyDrop ?>23: 03-03-2022<?#/ DailyDrop ?>
