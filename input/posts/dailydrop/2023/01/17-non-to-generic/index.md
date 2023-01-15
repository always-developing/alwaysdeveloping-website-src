---
title: "Non-generic to generic method call"
lead: "How to call into a generic method from a non-generic method"
Published: "01/17/2023 01:00:00+0200"
slug: "17-non-to-generic"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - generic
   - conversion

---

## Daily Knowledge Drop

The `dynamic` type can be used in the use case when required to call into a _generic method, from a non-generic method_. This is a fairly niche use case, but when required, the `dynamic` technique explained below can be of great benefit.

---

## Generic method

Assume we have the following simple generic method:

``` csharp
public static void GenericMethod<T>(T parameter)
{
    Console.WriteLine($"T is of type: {typeof(T).Name}");
}
```

This method needs to be called from another non-generic method which contains an `object` variable.

---

### Object technique

Calling into the generic method with an `object`:

``` csharp
public static class Converter
{
    public static void ObjectMethod(object randomObj)
    {
        // call into the generic method with the
        // object type
        GenericMethod(randomObj);
    }
}
```

While this will compile and "work", calling the `ObjectMethod` method with various types, will result in the same output:

``` csharp
Converter.ObjectMethod("string value");
Converter.ObjectMethod(1001);
```

The output:

``` terminal
T is of type: Object
T is of type: Object
```

As an `Object` is being passed to the generic method (even though the underlying types are different), the values are _boxed_ into that type, and that is what is output.

If we would like to know the _actual_ type output, then the `dynamic technique` can be used.

---

### Dynamic technique

The `dynamic technique` involves casting the `object to dynamic` before calling the generic method:

``` csharp
public static class Converter
{
    public static void ObjectMethod(object randomObj)
    {
        // cast the object to dynamic
        dynamic dynObj = randomObj;
        GenericMethod(dynObj);
    }
}
```

Now, calling the generic method with the same variables:

``` csharp
Converter.ObjectMethod("string value");
Converter.ObjectMethod(1001);
```

Will result in the following output:

``` terminal
T is of type: String
T is of type: Int32
```


---

## Notes

A fairly niche use case, and a small difference in code but one which can make a big difference when the use case is encountered. There might be a performance impact with the _dynamic technique_ (as dynamic generally is less performant), but this might be out-weighed by the benefit the technique gives. As always, if performance is an issue, benchmark and and make an informed decision which technique to use.

---

## References

[Roger Johansson Tweet](https://twitter.com/RogerAlsing/status/1609955500363333632)  

<?# DailyDrop ?>236: 17-01-2023<?#/ DailyDrop ?>
