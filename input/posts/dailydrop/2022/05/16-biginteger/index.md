---
title: "Catering for big numbers with BigInteger"
lead: "How to work with integer values great than int.MaxValue with BigInteger"
Published: 05/16/2022
slug: "16-biginteger"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - integers
    - biginteger

---

## Daily Knowledge Drop

In C#, the `int` integral type has a maximum and minimum value of 2,147,483,647 and -2,147,483,648 respectively. Going outside of these bounds will result in inaccurate output.

The `BigInteger` struct is an immutable type, which has no upper or lower bounds - and this can store values outside those of `int`.

---

## int

It was touched upon the the ["checked and unchecked keywords"](../11-checked-unchecked/) post, but lets have a look at what happens when going outside the limits of an `int`:

``` csharp
// Just confirm the max value of an int
Console.WriteLine(int.MaxValue);

// assign this max value to a variable
int maxInt = int.MaxValue;
// then increase it by 10
maxInt = maxInt + 10;
Console.WriteLine(maxInt);
```

By default, the above does not result in an exception, however the final result is not an accurate representation of the operation. The output is:

``` powershell
2147483647
-2147483639
```

Exceeding the max value caused counting to continue from the min value and increment from there. There are a number of ways to solve this:
- using `unit` (unsigned int)
- using a larger type, such as `long`

However, these both still have limits (although higher than that of `int`). If the value required could be higher or lower than the bounds of these types, then the `BigInteger` type can be used.


---

## BigInteger

The `BigInteger` type has no upper or lower bounds (in theory) and as such can store any arbitrarily large integer.

``` csharp
// Just confirm the max value of an int
Console.WriteLine(int.MaxValue);

// assign this max value to a variable
int maxInt2 = int.MaxValue;
var bigInt = new BigInteger(maxInt2);
bigInt = bigInt + 10;
Console.WriteLine(bigInt);
```

The output is now accurate:

``` powershell
2147483647
2147483657
```

---

### Caution

As mentioned, `BigInteger` is immutable, and also has no upper or lower bounds. If an operation causes it's value grows too large, it is possible that an `OutOfMemoryException`.   

Just be aware that if dealing with a number of `BigInteger` instances, storing large values, the usage might need to be considered and thought through more than if working with an `int`, for example.


---

## Notes

For most use cases `BigInteger` will probably not be required - however in the rare cases when a value needs to exceed the limits of an `int`, the `BigInteger` type can prove to be very useful.

---

## References

[BigInteger Struct](https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger?view=net-6.0)  

<?# DailyDrop ?>74: 16-05-2022<?#/ DailyDrop ?>
