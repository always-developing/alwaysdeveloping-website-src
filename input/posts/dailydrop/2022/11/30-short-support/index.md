---
title: "Short and arithmetic operations"
lead: "How arithmetic operators convert short values when performing operations"
Published: "11/30/2022 01:00:00+0200"
slug: "30-short-support"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - short
   - operator

---

## Daily Knowledge Drop

When performing arithmetic operations on `short` data types, their values are converted to the `int` types, which is also the type of the result of the operation.

---

## Examples

When working with integral types which could be an `int` or `short`, the compiler will infer `int`:

``` csharp
// the compiler will infer than intValue is 
// an int (even though it could be a short)
// in this specific example
var intValue = 32 / 2;
```

However, when specifying the variable type explicitly, the compiler will infer the value as a `short`:

``` csharp
short shortValue = 32 / 2;
```

Here the value _32_ is inferred as a `short`, and the output of the operator is explicitly declared as a `short`. 

All good so far - but now is where the are a bit unexpected.

Operations on an _explicitly defined_ short value, will result in a `int result`

``` csharp
// short value is definitely a short
short shortValue = 32 / 2;

// int response, all good
int intOutput = shortValue / 2;

// ERROR: this gives a compiler error
//short shortOutput = (shortValue / 2);
```

In the last operation, even though it is being performed on a short value, the result is an `int` and the above results in the error:

``` terminal
Cannot implicitly convert type 'int' to 'short'. An explicit conversion exists (are you missing a cast?)
```

Thankfully, the error is very easy to resolve - as the error states, an explicit conversion needs to be done:

``` csharp
// All good!
short shortOutput = (short)(shortValue / 2);
```

---

## Notes

A small quirk of the language, which if encountered just needs to be managed and handled. Before being aware of this knowledge, I would have assumed that a `short` would be returned from an operation where the operands were defined as `short` - this assumption would be incorrect though!

---


## References

[Reddit Post](https://www.reddit.com/r/csharp/comments/ysm6lx/why_am_i_being_forced_to_cast_to_shorts_when_i_am/)

<?# DailyDrop ?>213: 30-11-2022<?#/ DailyDrop ?>
