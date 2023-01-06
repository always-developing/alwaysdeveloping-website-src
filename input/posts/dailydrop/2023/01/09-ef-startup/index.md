---
title: "Database creation on startup"
lead: "Using dependency injection to create an EF database on startup"
Published: "01/09/2023 01:00:00+0200"
slug: "09-ef-startup"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - entityframework
   - startup

---

## Daily Knowledge Drop

When configuring an Entity Framework `DbContext` with the _dependency injection container_, it is possible to make use of the container to create an instance of the context on startup, to either create the database or apply database migrations.

---

## Configuration

The `AddDbContext` extension method is used to configure the _Entity Framework DbContext_ on startup, registering the context with the dependency injection container:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// configure the context and add to the DI Container
builder.Services.AddDbContext<DemoContext>(options =>
    options.UseSqlServer(@"Server=.\SQLEXPRESS;Database=EFStartup;
        Integrated Security=True;TrustServerCertificate=True"));

var app = builder.Build();

app.MapGet("/blogs", (DemoContext context) =>
{
    return context.Blogs.ToList();
});

app.Run();
```

When the `/blogs` endpoint is called and a `DemoContext` is instantiated, the above will work `provided that the database and relevent tables have already been created` - if not, and exception will be thrown, as the database is not automatically created.

Next we'll have a look at how to ensure the database is created on startup, using the services registered with the dependency injection container.

---

## Startup

The process of ensuring the database is created, is relatively straightforward - Entity Framework does most of the heavy lifting for us:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DemoContext>(options =>
    options.UseSqlServer(@"Server=.\SQLEXPRESS;Database=EFStartup;
        Integrated Security=True;TrustServerCertificate=True"));

var app = builder.Build();

// this block ensures the database is created
#if DEBUG
using (var scope = app.Services.CreateScope())
{
    var startupContext = scope.ServiceProvider
        .GetRequiredService<DemoContext>();
    startupContext.Database.EnsureCreated();
}
#endif

app.MapGet("/blogs", (DemoContext context) =>
{
    return context.Blogs.ToList();
});

app.Run();
```

Some notes on the above code:
- the `#if conditional` is used to ensure the code is only executed when debugging. It is NOT recommended to automatically create the database or apply migrations in non-development environments. The recommended approach is to use a CI/CD pipeline.
- a new dependency injection scope is created (as the application host is technically not running yet) and a `DbContext` instance instantiated
- the _EnsureCreated_ method is called to create the database with the configured schema. This **bypasses** migrations - if the migrations are required to be run, then `startupContext.Database.Migrate()` should be called instead of `startupContext.Database.EnsureCreated()`
- the _EnsureCreated_ is designed to be used when doing `testing or prototyping` where the database is dropped and recreated with each execution

---

## Notes

A useful and time-saving way to use the _already configured_ dependency injection container to instantiate the database context, and have EF do the heavy lifting to create the database with the correct schema.

---


## References

[Dependency Injection of an Entity Framework Context within Program.cs Using Top Level Statements](https://nodogmablog.bryanhogan.net/2022/09/dependency-injection-of-an-entity-framework-context-within-program-cs-using-top-level-statements/)  

<?# DailyDrop ?>230: 09-01-2023<?#/ DailyDrop ?>
