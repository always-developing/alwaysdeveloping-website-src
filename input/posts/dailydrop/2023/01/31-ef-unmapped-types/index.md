---
title: "EF queries on unmapped types"
lead: "Introducing the ability to execute raw queries on unmapped types"
Published: "01/31/2023 01:00:00+0200"
slug: "31-ef-unmapped-types"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - entityframework
   - unmapped

---

## Daily Knowledge Drop

Entity Framework 8 is (_potentially_) introducing new functionality allowing `raw queries to be executed on unmapped types`.

At the time of this post, the functionality is only available on the [daily builds](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet8/nuget/v3/index.json), and may change before the EF8 release.

---

## SqlQuery Example

Making use of the new functionality is fairly straight-forward. To use the new functionality, we need a `type`:

``` csharp
public  class Blog
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string Title { get; set; }

    public DateTime DateCreated { get; set; }
}
```

A `DbContext` is also required:

``` csharp
public class DemoContext : DbContext
{
    public DemoContext() : base()
    {
    }

    public DemoContext(DbContextOptions<DemoContext> options) : base(options)
    {        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=Demo;Integrated Security=True;TrustServerCertificate=True");
    }
}
```

The important thing to note here, is that the `Blog type is NOT mapped to a DBSet in the DBContext`


With the new functionality, _raw SQL queries_ can now be executed against the `unmapped type` (_Blog_ in this example):

``` csharp
using var context = new DemoContext();

// Blog type is not mapped
var blogs = await context.Database.SqlQuery<Blog>(
        @$"SELECT b.Id, b.Title, b.DateCreated
            FROM Blog b")
    .ToListAsync();

Console.WriteLine($"Blogs returned: {blogs.Count}");
```

The same functionality can be leveraged for _simple types_:

``` csharp
using var context = new DemoContext();

var titles = await context.Database.SqlQuery<string>(
        @$"SELECT b.Title
            FROM Blog b")
    .ToListAsync();
```


This functionality can also be abstracted to a generic method to make calling it multiple times, for multiple entities easier:

``` csharp
async Task<List<T>> ExecuteSqlQuery<T>(DbContext context, string query)
{
    return await context.Database.SqlQuery<T>(
            FormattableStringFactory.Create(query)
        ).ToListAsync();
}
```

This method can now be called as follows:

``` csharp
var results = await ExecuteSqlQuery<Blog>(context, 
    "SELECT b.Id, b.Title, b.DateCreated FROM Blog b");
```

---

## DynamicContext

I have previous written a [NuGet package called DynamicContext](https://www.nuget.org/packages/AlwaysDeveloping.EntityFrameworkCore.DynamicContext) which effectively provides the same functionality. A detailed [blog post is also available](https://www.alwaysdeveloping.net/p/11-2020-dynamic-context/).

So will this new EF functionality replace `DynamicContext`? Yes, and no.

For the most part, yes it does replace `DynamicContext`. However there is one bit of functionality available in `DynamicContext` that is not available with the new functionality - the `ability to run queries against anonymous types`. This is possible with `DynamicContext`:

``` csharp
// Declare an example anonymous object with 
// the relevant properties, using default values 
var anon = new { BlogId = 0, Url = "" };

// Invoke the method just using the 
// example object, and not specifying T
var blog = CallWithAnon(anon);

static T CallWithAnon<T>(T example) where T: class
{
    // T is inferred from the example parameter (which is not used in the method, 
    // it is only used for the inference) and can successfully 
    // call into the dynamic runtime context
    using var dynContext = new RuntimeContext<T>();
    return dynContext.Set<T>().FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
        .AsNoTracking().First();
}
```

Trying the same technique with the new EF functionality results in the following error:

``` terminal
No suitable constructor was found for entity type '<>f__AnonymousType0<int, string>'. 
he following constructors had parameters that could not be bound to properties of the entity type: 
    Cannot bind 'BlogId', 'Title' in '<>f__AnonymousType0<int, string>(int BlogId, string Title)'
```

This is a fairly niche use case though, so for the majority of the time, the `new functionally can replace DynamicContext`.

---

## Notes

If the ability to query unmapped entities is required - this new functionality is the way to go (vs custom libraries). I'm happy to see additional _required_ functionality such as this being introduced into the base EF libraries, removing the reliance on additional libraries.

---

## References

[Entity Framework 8: Raw SQL queries on unmapped types](https://steven-giesel.com/blogPost/d1f069fb-7f6d-4f80-a98f-734755474ae1)  
[Arthur Vickers Tweet](https://twitter.com/ajcvickers/status/1616203415637618688)  

<?# DailyDrop ?>246: 31-01-2023<?#/ DailyDrop ?>
