---
title: "Indexers - access a class as an array"
lead: "How to extend a class so that it can be accessed as an array"
Published: 02/23/2022
slug: "23-indexers"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - indexer
    - array
    - extend
    
---

## Daily Knowledge Drop

Any (relevant) class can be extended so that it can be accessed as an array, using `Indexers`, just by adding a property to the class.

---

## Without indexers

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

To access a price for a specific month, we can do this via the _Prices_ property exposed on the class

``` csharp
var pp = new ProductPrice(112, "Green t-shirt", 
    new decimal[] { 0, 0, 0, 100, 100, 80, 80, 50, 50, 100, 100, 60 });
int month = 5;

if (pp.Prices.Length >= month)
{
    Console.Write(pp.Prices[month]);
}
```

This code will function just fine - however the `ProductPrice` class can be extended to make it more intuitive and easier to work with.

---

## With indexers

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

    // The indexer has been added
    // This allows the class to be accessed as an array
    public decimal this[int index]
    {
        get
        {
            // Custom logic to check if trying to access an out of bound index, 
            // then return 0 (instead of throwing an exception)
            if(Prices.Length >= index) return Prices[index];
            return 0;
        }
    }

    //  Expose a Length property which will return the number of prices
    public int Length => Prices.Length;
}
```

The `ProductPrice` class can now be used as follows:

``` csharp
var pp = new ProductPrice(112, "Green t-shirt", 
    new decimal[] { 0, 0, 0, 100, 100, 80, 80, 50, 50, 100, 100, 60 });
int month = 5;
int invalidMonth = 100;

// The price can now be accessed directly on the class using [], 
// no need to go via Prices as before
Console.WriteLine(pp[month]);

// An invalid month can be sent without bounds checking, 
// as the class will perform the check internally
Console.WriteLine(pp[invalidMonth]);
```

The output for the above is

``` powershell
80
0
```

---

## Field indexer

Another use for `indexers` is to index the fields/properties on a class.

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

    // add indexer functionality
    public string this[int index]
    {
        // build up an array of all the properties, 
        // and then access the corresponding item
        get => (new string[] { Number, Line1, Line2, Suburb, Town, Province }) [index];
    }

    public int Length => 6;
}
```

The indexer functionality has been added, but the class `contains no arrays to index`.

In the indexer method, an array is built up of all the properties on the class, and then accessed via the index. This allows for accessing a property via an index. This can be leveraged to iteration through the properties on the class:

``` csharp
var address = new Address("11", "Main Road", "XYZ Estate", 
    "Suburb1", "Town2", "Province3");

for (int i = 0; i < address.Length; i++)
{
    Console.WriteLine(address[i]);
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

While extending a class to use `indexers` will generally not be required for everyday use, its advantageous to know that `indexers` exist and are available for the use cases where they will make working with the class a lot easier and intuitive.

---

## References
[Using indexers (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/using-indexers)  

<?# DailyDrop ?>17: 23-02-2022<?#/ DailyDrop ?>
