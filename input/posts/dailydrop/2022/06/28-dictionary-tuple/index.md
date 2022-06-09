---
title: "Destructing a Dictionary record into a Tuple"
lead: "A Dictionary key-value pair can be destructed into a Tuple"
Published: "06/28/2022 01:00:00+0200"
slug: "28-dictionary-tuple"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dictionary
    - tuple
    - destruct

---

## Daily Knowledge Drop

A `Dictionary` record can be destructed into a `Tuple` - specifically the `KeyValuePair` type representing a dictionary record can be destructed into a `Tuple`. In addition, as the `Dictionary` type contains a _GetEnumerator_ method, [it can be iterated over](../../03/03-getenumerator/) into a `Tuple`.

---

## Examples

In all of the below examples, _numberDictionary_ is of type `Dictionary<string, int>`.

--- 

### Key iteration

One way to iterate through all entries in a `Dictionary` is to loop through each `Key` and then use the _Key_ to retrieve the entry `Value`.

``` csharp
public void IterateKeyValue()
{
    var sum = 0;

    // iterate of each key in the dictionary
    foreach (var key in numberDictionary.Keys)
    {
        // get the int value based on the key
        sum += numberDictionary[key];
    }
}
```

Here the `Dictionary` is accessed twice, once to get the _Key_ and a second time to get the _Value_ for the specific Key.

---

### KeyValue iteration

The next method for iteration, is to iterate through each `Dictionary` record, a `KeyValuePair`. This is possible because Dictionary contains a _GetEnumerator_ method.

``` csharp
public void IterateKVPair()
{
    var sum = 0;

    // iterate of each KeyValuePair in the dictionary
    foreach (KeyValuePair<string, int> kv in numberDictionary)
    {
        // get the value part of the KeyValuePair
        sum += kv.Value;
    }
}
```

Simpler than the previous method, and here the Dictionary itself is only accessed only once to get the `KeyValuePair` which in turn contains all the information for the entry.

---

### Tuple

The last method for iteration, is to iterate through each `Dictionary` item, but `destruct the KeyValuePair into a Tuple`.

``` csharp
public void IterateTuple()
{
    var sum = 0;

    // iterate and destruct into a tuple
    foreach (var (key, value) in numberDictionary)
    {
        // access the value directly
        sum += value;
    }
}
```

One could argue whether this version is easier to read than the previous _KeyValuePair_ version - personally I do find this version more readable. An added benefit is that the name of the `Tuple` items can be customized to make it obvious to the reader as to what they contains. So instead of _Key_ and _Value_:

``` csharp
public void IterateProducts()
{
    var sum = 0;
    foreach (var (productName, price) in productDictionary)
    {
        sum += intValue;
    }
}
```

---

## Benchmarks

So we've looked at three ways to get the information from a Dictionary, but how to each of them perform?


|          Method |      Mean |     Error |    StdDev | Ratio |
|---------------- |----------:|----------:|----------:|------:|
| IterateKeyValue | 14.827 us | 0.1774 us | 0.1659 us |  1.00 |
|    IterateTuple |  3.529 us | 0.0468 us | 0.0438 us |  0.24 |
|   IterateKVPair |  3.519 us | 0.0296 us | 0.0262 us |  0.24 |

The `KeyValuePair` and `Tuple` versions are comparable, while iterating through each _Key_ and then getting the _Value_ is `4 times slower`.

---

## Notes

Iterating through _Keys_, to then retrieve the _Value_ shouldn't ever be the default method for iteration - especially when there are other more performant and easier to read methods available.  
_KeyValuePair_ and _Tuple_ iteration are comparable, and usage comes down to personal preference - personally I prefer the `Tuple` with its added benefit of being able to accurately name the _Key_ and _Value_.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1525885135727501312)  

<?# DailyDrop ?>105: 28-06-2022<?#/ DailyDrop ?>
