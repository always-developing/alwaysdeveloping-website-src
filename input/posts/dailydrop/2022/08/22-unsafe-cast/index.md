---
title: "Unsafe cast for performance"
lead: "Performing unsafe casts for performance improvements"
Published: "08/22/2022 01:00:00+0200"
slug: "22-unsafe-cast"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - unsafe
   - cast

---

## Daily Knowledge Drop

The `UnSafe static class` can be used to perform an `Unsafe` cast, which can lead to performance improvements - however it is only recommended in instances when a `safe cast ((T)o) would have guaranteed to be successful`.  Performing an unsafe cast when a safe cast would have been unsuccessful could result in application instability.

---

## Safe casting

When performing a `safe cast`, runtime safety checks are performed, and if the cast is invalid then an `InvalidCastException` will be thrown.

The below method is a `safe cast`, as the runtime will prevent the cast from taking place (and throw the _InvalidCastException_) if _object o_ is not compatible with type Product:

``` csharp
public Product GetProduct(object o)
{
    return (Product)o;
}
```

Another approach is to use the `is` keyword, which will not thrown the exception, but return `false` if the cast is not compatible (and actually perform the cast if it is compatible):

``` csharp
public Product GetProductIs(object o)
{
    if (o is Product p)
        return p;

    return null;
}
```

For almost all use cases, one of the above techniques will be suitable.

---

## UnSafe casting

The `static Unsafe class` can be used to perform a cast, but without any of the safety checks being performed. Performing an invalid cast will not lead to the `InvalidCastException` being thrown, just to potential system instability.

Performing an `unsafe cast` is almost as simple as performing a _safe cast_:

``` csharp
public Product GetProductUnsafe(object o)
{
    // case object o to Product
    return Unsafe.As<Product>(o);
}

```

As no safety checks are performed by the runtime, some performance gains can be achieved by using this approach - but let's test just how big of a gain.

---

## Performance

Benchmarking the three different techniques, each with a `null value` and with a `Product instance` (a valid cast):

|      Method |       o |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|------------ |-------- |----------:|----------:|----------:|----------:|------:|--------:|
|  NormalCast |       ? | 0.2752 ns | 0.0331 ns | 0.0294 ns | 0.2763 ns |  1.00 |    0.00 |
| CastUsingIs |       ? | 1.4600 ns | 0.0369 ns | 0.0327 ns | 1.4688 ns |  5.37 |    0.67 |
|  UnsafeCast |       ? | 0.0447 ns | 0.0428 ns | 0.0761 ns | 0.0000 ns |  0.16 |    0.20 |
|             |         |           |           |           |           |       |         |
|  NormalCast | Product | 0.1189 ns | 0.0353 ns | 0.0275 ns | 0.1164 ns | 1.000 |    0.00 |
| CastUsingIs | Product | 0.7099 ns | 0.0511 ns | 0.0453 ns | 0.7091 ns | 6.290 |    1.30 |
|  UnsafeCast | Product | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.000 |    0.00 |

Results may vary depending on the use case, but here, `using Unsafe is effectively is free` and takes no time at all.

---

## Notes

Using `Unsafe` is only recommended in the narrow use case where one knows using a `safe cast will succeed` - leveraging `Unsafe` in this case will result in performance gains.
However, the gain is on the _nanoseconds_ scale, so is not likely to be noticeable - results may vary depending on the use ase though, and if performing a large number of slow _casts_, then potentially a safe `Unsafe` cast result in a noticeable difference.

---

<?# DailyDrop ?>143: 22-08-2022<?#/ DailyDrop ?>
