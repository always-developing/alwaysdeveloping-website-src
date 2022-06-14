---
title: "EF Find vs First performance "
lead: "Comparing the performance of Entity Framework's Find and First methods"
Published: "07/04/2022 01:00:00+0200"
slug: "04-ef-find-vs-single"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - ef
    - entityframework
    - performance

---

## Daily Knowledge Drop

When retrieving a single record from the database using Entity Framework, the `Single (or SingleOrDefault)` method can be used - however the `Find` method on the DbSet is potentially more efficient, however it comes with some limitations.

---

## First

The first method is using `First (or FirstOrDefault)` - this is an extension method on _Queryable_ (an important factor when comparing it to the _Find_ method)

As _DbSet_ implement _IQueryable_, this method can be use directly on the _DbSet_.

Assuming we have a _DbContext_ with a "Song" _DbSet_, a single record can be retrieved as follows:

``` csharp
using var context = new DemoContext();

// using Single
var song = context.Songs.Single(x => x.Id == 43);

// using SingleOrDefault
var song2 = context.Songs.SingleOrDefault(x => x.Id == 43);
```

---

## Find

The next method is using `Find` - this is an extension method on _DBset_ itself.

Again, assuming we have a _DbContext_ with a "Song" _DbSet_, a record can be retrieved as follows using _Find_:

``` csharp
using var context = new DemoContext();

var song = context.Songs.Find(43);
```

---

### Limitation

As mentioned above, having `Find` as an extension method on **DbSet** does limit its usage. It cannot be used in conjunction with other _IQueryable_ extension methods.

The following examples are **NOT** valid and will **NOT** compile:

``` csharp
// Find() returns a 'Song' and as such Include()
// cannot be used in this example
var song = context.Songs.Find(43).Include("Artist");

// Include() returns a 'IQueryable' and as such Find()
// cannot be used in this example
var song2 = context.Songs.Include("Artist").Find(43);
```

However, the following using `Single` **is VALID**:

``` csharp
// Include() returns a 'IQueryable' and as Single is
// an extension method on 'IQueryable' it CAN be used
var song = context.Songs.Include("Artist").Single(x => x.Id == 43);
```

---

## Benchmark

The different methods where benchmarked, with a:
- `Shared DbContext` - defined in the constructor once, and reused
- `Single-use DbContext` - a new DbContext is declared for each database call

|                     Method |      Mean |    Error |    StdDev |    Median | Ratio | RatioSD |
|--------------------------- |----------:|---------:|----------:|----------:|------:|--------:|
|          FindSharedContext |  14.93 us | 2.845 us |  8.071 us |  12.00 us |  0.14 |    0.07 |
| SingleDefaultSharedContext | 123.26 us | 2.294 us |  2.146 us | 123.22 us |  0.98 |    0.05 |
|        SingleSharedContext | 123.78 us | 2.320 us |  4.242 us | 122.09 us |  1.00 |    0.00 |
|          FindOneUseContext | 488.81 us | 9.539 us | 10.207 us | 487.06 us |  3.91 |    0.20 |
| SingleDefaultOneUseContext | 261.49 us | 5.098 us |  5.236 us | 260.91 us |  2.09 |    0.11 |
|        SingleOneUseContext | 259.93 us | 5.166 us | 10.317 us | 257.45 us |  2.11 |    0.11 |

<br>

The _Find_ method is `substantially quicker` when reusing the DbContext (as one should).

However, interestingly, with a single-use DbContext, the _Single/SingleOrDefault_ method is almost twice as quick as the _Find_ method.

---

## Notes

While _Find_ is approximately `7 times` faster than the equivalent _Single_ method, it does come with considerable limitations. If performance is critical, it might be worth finding a way to work around the limitations - however keep in mind that the differences are being measured in microseconds, so the performance gain might not be worth the potential additional effort.

Be aware of the performance differences, and the limitations - and for each specific use case, apply the more appropriate technique.

---

## References

[Daniel Lawson tweet](https://twitter.com/danylaws/status/1524284247049216000)  

<?# DailyDrop ?>109: 04-07-2022<?#/ DailyDrop ?>
