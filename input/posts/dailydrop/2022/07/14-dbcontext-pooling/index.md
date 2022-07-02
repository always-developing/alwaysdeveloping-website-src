---
title: "Entity Framework DbContext Pooling"
lead: "Using DbContext pooling to improve the performance of aan application"
Published: "07/14/2022 01:00:00+0200"
slug: "14-dbcontext-pooling"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - efcore
    - entityframework
    - dbcontext
    - pooling

---

## Daily Knowledge Drop

Entity Framework Core's `DbContext pooling` functionality can be used to improve the performance of an application.

While generally a lightweight object, each context instance does require some internal setup of various services, which does have an overhead. `Context pooling`, as the name implies, create a pool of DbContext instances, setup during startup of the application and reused, thus paying the setup cost only once.

---

## Configuration

Configuring an application to use `DbContext pooling` instead of the non-pooling option is incredibly simple - in fact it requires just one small change to the usual DbContext setup.

On startup of an application:

- **Default non-pooling configuration**:
    ``` csharp
    builder.Services.AddDbContext<DemoContext>(o => o
        .UseSqlServer(builder.Configuration.GetConnectionString("DemoContext")));
    ```

- **DbContext pooling configuration**:
    ``` csharp
    builder.Services.AddDbContextPool<DemoContext>(o => o
        .UseSqlServer(builder.Configuration.GetConnectionString("DemoContext")));
    ```

Instead of the `AddDbContext` method being called, the `AddDbContextPool` method is called - thats all the configuration required.

The default pool size can also be manually specified as part of the _AddDbContextPool_ call (with the default being 1024 in EF Core 6.0 and 128 in previous versions)

---

## Usage

The usage is `exactly the same` awith our without the context pooling - inject the application _DbContext_ (DemoContext in the above examples) into the relevent constructor and use it as per normal.

Entity Framework will internally handle everything related to the pooling functionality.

---

## Benchmarks

For the benchmarks, we have a database table with `50` records, and will be comparing retrieving a single row (using the primary key) using:
- Manually created `DbContext` instance each time
- A `DbContext` retrieved from the context pool

In the below code, as dependency injection was not used, `PooledDbContextFactory` is used to control getting an instance of a DbContext from the pool:

``` csharp

private DbContextOptions<DemoContext> _options;
private PooledDbContextFactory<DemoContext> _poolingFactory;

public Benchmarks()
{
    // confiture the dbcontext options
    _options = new DbContextOptionsBuilder<DemoContext>()
        .UseSqlServer(@"Server=.\SQLEXPRESS;Database=EFPool;Integrated Security=True")
        .Options;

    // setup the pooling factory using the options
    _poolingFactory = new PooledDbContextFactory<DemoContext>(_options);
}

[Benchmark]
public Song WithoutContextPooling()
{
    // new DbContext using the options
    using var context = new DemoContext(_options);

    return context.Songs.First(s => s.Id == 1);
}

[Benchmark]
public Song WithContextPooling()
{
    // new DbContext using the PooledDbContextFactory which uses the options
    using var context = _poolingFactory.CreateDbContext();

    return context.Songs.First(s => s.Id == 1);
}
```

Results:

|                Method |     Mean |    Error |    StdDev |  Gen 0 | Allocated |
|---------------------- |---------:|---------:|----------:|-------:|----------:|
| WithoutContextPooling | 701.3 us | 51.09 us | 147.42 us |      - |     96 KB |
|    WithContextPooling | 124.9 us |  2.15 us |   2.02 us | 1.4648 |      9 KB |

The `context pooling results in a 5.5x speed improvement` and `10x memory improvement!`

---

## Notes

Improvements such as the ones shown above in the benchmarks above might not be applicable in every application and use case - however my recommendation is to `default to using DbContext pooling`. If performance is not where it should be, then benchmark the two options and revert to the option with the best performance.

---

## References

[DbContext pooling](https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cwith-constant#dbcontext-pooling)   

---

<?# DailyDrop ?>116: 14-07-2022<?#/ DailyDrop ?>
