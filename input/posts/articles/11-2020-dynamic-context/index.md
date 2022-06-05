---
title: "Dynamic Context (for Entity Framework Core)"
lead: "A guide to returning projected, anonymous and simple type values dynamically"
Published: 12/11/2021
slug: "11-2020-dynamic-context"
draft: false
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - entity framework
    - entityframework
    - ef     
    - context
    - dynamic
    - dbset
---

## The problem
Entity Framework Core is a great go-to ORM for .NET, for any type of application provides almost all the functionality required to do successful database access out the box.  
However, there are two use cases, specifically with regards to retrieval of data, it doesn't cater for - this post and the accompanying code sample/NuGet package attempts to provides solutions for these use cases.

First, the setup - an EF DbContext which has one DbSet, for storing `Blogs` (the below is a standard DbContext configuration):

``` csharp
public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
    {
    }

    public Action<DbContextOptionsBuilder>? BlogContextOptionsBuilder { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    // The DBSet for the Blog entity
    public DbSet<Blog> Blog { get; set; }
}

public class Blog
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Owner { get; set; }
    public string Url { get; set; }
    public List<Post> Posts { get; set; }
}

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int WordCount { get; set; }
    public Guid BlogId { get; set; }
    public Blog Blog { get; set; }
}
```

<?# InfoBlock ?>
The below use cases are `fairly niche` - in most day-to-day use cases, `EF Core will do what is required out the box`. The below solutions are intended to be used to `enhance and work in conjunction` with the normal DbContext.  
If you find you are ONLY using the DbContext for the below use cases, it might make sense to investigate using another more suited ORM (such as **[Dapper](https://github.com/DapperLib/Dapper)**).

However if you are using EF Core and adding another ORM into your application doesn't make sense, hopefully this post along with the **[source code + samples](https://github.com/always-developing/AlwaysDeveloping.EntityFrameworkCore.DynamicContext)** and **[NuGet package](https://www.nuget.org/packages/AlwaysDeveloping.EntityFrameworkCore.DynamicContext/)** can assist you in resolving any issues.
<?#/ InfoBlock ?>

---

### DbSet is required

To get a list of all Blogs, one of the following two lines of code can be used:

``` csharp
var blogs1 = context.Blog.AsNoTracking().ToList();
// OR
var blogs2 = context.Set<Blog>().AsNoTracking().ToList();
```

Suppose only the Blog id and the url was required - any one of the below methods would achieve this:

``` csharp
// Define a new type, called BlogUrl, which contains just BlogId and Url properties
// Project into the new type
var blogs1 = context.Set<Blog>()
    .Select(x => new BlogUrl { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();

// Project into the new type using raw SQL
var blogs2 = context.Set<Blog>().FromSqlRaw("SELECT Id, Url FROM Blog")
    .Select(x => new BlogUrl { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();

// Project into an anonymous type
var blogs3 = context.Set<Blog>()
    .Select(x => new { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();

// Project into an anonymous type using raw SQL 
var blogs4 = context.Set<Blog>().FromSqlRaw("SELECT Id, Url FROM Blog")
    .Select(x => new { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();    
```

The issue here is that `EF Core requires the retrieval to be executed off a DbSet`. This means an entity and matching SQL statement cannot dynamically be thrown at EF Core at runtime, and have data successfully returned.

For example, the following code would `not work unless the BlogUrl type has been added as a DbSet to the DbContext`.
``` csharp
// Does not work unless a DbSet of type BlogUrl has been added
var blogs = context.Set<BlogUrl>().FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList();
```

This problem also extends to anonymous types - as their definition is only known at runtime, a `DbSet` cannot be created for them before runtime.

---

### Support for simple types

Suppose now only a list of Blog ids is required to be returned - either one of the following would work:

``` csharp
// Define a new type, called BlogId, which contains just Id
// Project into the new type
var ids1 = context.Set<Blog>()
    .Select(x => new BlogId { Id = x.Id })
    .AsNoTracking().ToList();

// Project into a list of Ids
var ids2 = context.Set<Blog>()
    .AsNoTracking()
    .Select(x => x.Id).ToList();

// Project into a list of Ids using raw sql
var ids3 = context.Set<Blog>().FromSqlRaw("SELECT Id FROM Blog")
    .AsNoTracking()
    .Select(x => x.Id).ToList();
```

The issue here is that the `DbSet type is required to be a reference type`: This means a list of simple/value types (and other identifier types such as Guid) cannot be returned directly.  
This is related to the first issue mentioned above - a simple/value type and matching SQL statement cannot dynamically be thrown at EF Core at runtime and have data returned.

For example, the following code would `NOT work`.
``` csharp
// This DOES NOT WORK (yet)
var ids = context.Set<Guid>().FromSqlRaw("SELECT Id FROM Blog")
    .AsNoTracking().ToList();
```

<?# InfoBlock ?>
Both issues this post addresses involves the retrieval data - `EF Core change tracking functionality will not work, and is not intended to work` with the proposed solutions.  
If change tracking is required, then the dynamic route outlines in this post should not be used. This is the reason why all example use **AsNoTracking()** when retrieving the data.
<?#/ InfoBlock ?>

---

## Dynamic entity results
### Projected entity

The first issue to resolve, is the ability to populate an entity `without having a Dbset added to the DbContext` for the entity.  
We cannot _really_ get around this requirement - EF Core always need the entity be added as a DbSet, however what if it was `added dynamically at runtime?`

``` csharp
// Inherits from DBContext, but takes in a generic type T
public class RuntimeContext<T> : DbContext where T : class
{
    public RuntimeContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Environment
            .GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\BlogDatabase.db");

        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Type T is added as a DbSet to the context, but without a Key. 
        // A key is not required as this will be used only for data 
        // retrieval and with AsNoTracking
        var t = modelBuilder.Entity<T>().HasNoKey();

        // Using reflection, the relevant properties of type T 
        // are added to the DbSet entity
        foreach (var prop in typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CustomAttributes
                .Any(a => a.AttributeType == typeof(NotMappedAttribute)))
            {
                t.Property(prop.Name);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
```

`The following code will now work, without BlogUrl being added` as a DbSet beforehand:
``` csharp
using var dynContext = new RuntimeContext<BlogUrl>();
var blogs = dynContext.Set<BlogUrl>().FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList()
```

As this is all being setup dynamically, EF will not know how to generate the SQL for the dynamic entity - this is why raw SQL will always need to be supplied for this solution. The solution could be expanded to include this functionality in future, but this is outside the scope of this post.

This is now a `working dynamic runtime context` - however there are still a few broader issues which need to be resolved, which we'll get to later in the post:
- `Dependency Injection` - There needs to be a way to configure the DI container with the dynamic runtime context when the underlying original context is configured.
- `Dynamic Configuration`: In the above, the configuration of the dynamic runtime context is hardcoded. Ideally this context would be initialized with the same configuration as the original context.

---

### Anonymous entity

As it stands, the core of the above code will work with anonymous types - just one small issue to resolve, and thats how to `convert the anonymous type to T`.  

To convert the anonymous object to T, we have to `inter T by example`:

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

---

## Dynamic value type results

The next issue to resolve, is the ability to get a simple type or list of simple types from EF Core dynamically. The term `simple type` is used very loosely to refer to following types:
- Value types: `int`, `bool`, `float`, `char` etc
- Common Unique Identifier: `Guid`
- Simple value classes: `string`

As mentioned previously, EF Core requires a query be executed off a DbSet - another requirement is that the `DbSet type be a reference type` (a class).

Even with the dynamic runtime context, the following code `would not work` as a DbSet of type `Guid` cannot be dynamically created (as its not a reference type):
``` csharp
// This DOES NOT WORK (yet)
var blogs = dynContext.Set<Guid>().FromSqlRaw("SELECT Id FROM Blog")
    .AsNoTracking().ToList();
```

We cannot _really_ get around the requirement that the DbSet be a reference type - however, what can be done is to `dynamically converted the simple type to a reference type, run the query and converted the results back to a simple type`

First lets create a reference type class, which will act as a wrapper:
``` csharp
public class SimpleType<TValue>
{
    public TValue Value { get; set; }
}
```

There are no constraints on TValue, as there are no generic constraints which will work for all the types we require (value types, Guid and string). 

Now we can use this reference wrapper class, and call the dynamic runtime context:
``` csharp
using var dynContext = new RuntimeContext<SimpleType<Guid>>();
var id = dynContext.Set<SimpleType<Guid>>()
    .FromSqlRaw("SELECT Id as Value FROM Blog")
    .AsNoTracking().First().Value;
```


The above will work, but we now have two more broader issues which need to be resolved:
- `Still using reference type`: We are still using a reference type for the DbSet and have to manually wrap the simple type and then unwrap it
- `Column called 'Value'`: The column in the SQL has to be called 'Value' for it to match the field on the wrapper class and successfully retrieve the data

--- 

## Resolving outstanding issues

### Code restructure
Even though we have a working dynamic runtime context, there are 4 outstanding issues to be resolved, before we have a more complete solution. First lets restructure the code a bit to make these easier to resolve.

1. Create a new class `DynamicContext` which accepts a generic DbContext. 
``` csharp
public class DynamicContext<TContext> where TContext : DbContext
{
    private readonly DbContext _originalContext;

    public DbContext Context { get { return _originalContext; } }

    public DynamicContext(TContext context)
    {
        _originalContext = context;
    }
}
```

2. Change `RuntimeContext` to accept a generic TContext of type DbContext, and make it internal instead of public. The reason for this will become more apparent as we start adding more functionality to `DynamicContext`.
``` csharp
internal class RuntimeContext<T, TContext> : 
    DbContext where T : class where TContext : DbContext
{
    // Rest of class implementation
}
```

`DynamicContext` will now become a `wrapper` and the public face of `RuntimeContext` and of the original `DbContext` - as we work through the outstanding issues below, more functionality will be added to `DynamicContext` to make use of `RuntimeContext`.

The four outstanding issues:
- `Dependency Injection`: There needs to be a way to configure the DI container with the runtime context as well as the original context
- `Dynamic Configuration`: The dynamic runtime context should use the same configuration as the original underlying DbContext
- `Reference type wrapper`: A reference type wrapper is still used for simple types, which has to manually be wrapped and unwrapped
- `Column called 'Value'`: The column in the raw SQL has to be called 'Value' as it has to match the field on the wrapper class

---

### Dependency Injection

As `DynamicContext` now takes a DbContext as a generic parameter, if a DbContext is added to the DI container `DynamicContext` should be added as well.

To do this, we'll use extension methods which correspond to the existing .NET `AddDbContext` methods. For each overloaded `AddDbContext` method, an `AddDynamicContext` method will be added (an example of one of these methods):
``` csharp
public static IServiceCollection AddDynamicContext<TContext>(
    this IServiceCollection serviceCollection, 
    Action<DbContextOptionsBuilder> optionsAction = null, 
    ServiceLifetime contextLifetime = ServiceLifetime.Scoped, 
    ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
{
    // Add the dynamic context for the original dbcontext
    serviceCollection.AddScoped<DynamicContext<TContext>>();
    // add the dbcontext using the normal AddDbContext call
    return serviceCollection.AddDbContext<TContext, TContext>(
        optionsAction, 
        contextLifetime, 
        optionsLifetime
    );
}
```

The method has the same parameters as the invoked `AddDbContext` method, and acts as a passthrough - on the way adding a record to the DI container for `DynamicContext<T>`.  
An extension method is added for each variation of the `AddDbContext` method.  

When setting up the DI container, instead of calling `AddDbContext`, `AddDynamicContext` is now called.
``` csharp
// OLD
.AddDbContext<BlogContext>(x => x
    .UseSqlite($"Data Source={Environment
        .GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\BlogDatabase.db"))

// NEW
.AddDynamicContext<BlogContext>(x => x
    .UseSqlite($"Data Source={Environment
        .GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\BlogDatabase.db"))        
```

We can now inject DynamicContext\<T\> (where T is the DbContext) into the relevant constructor, and have access to the dynamic functionality provided by `DynamicContext` as well as access to the underlying DbContext, via the `Context` property.

**Dependency injection taken care of!**

---

### Dynamic Configuration

Currently, the configuration of the RuntimeContext is hardcoded in the _OnConfiguring_ method. Next let's make it dynamic and have the same configuration as the underlying original DbContext. 

First, lets define a new type to `store the DbContext initialization Action`:

``` csharp
public class DynamicContextInitOptions<TContext> where TContext : DbContext
{
    Action<DbContextOptionsBuilder> optionsAction { get; set; }

    Action<IServiceProvider, DbContextOptionsBuilder> 
        optionsActionDependencyInjection { get; set; }
}
```
When initializing a DbContext, one of the two `Actions` could be used. The class can cater for both, but only one of the two will be used at any one time.  

Next, when a DbContext is added to the container (via the `AddDynamicContext` extension method) we'll record how the original DbContext was initialized, and add the initialization Action to the DI container as well. A private helper method `AddDynamicContent` is used to configure the DI container based on the Action passed in:
``` csharp
public static IServiceCollection AddDynamicContext<TContext>(
    this IServiceCollection serviceCollection, 
    Action<DbContextOptionsBuilder> optionsAction = null, 
    ServiceLifetime contextLifetime = ServiceLifetime.Scoped, 
    ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
{
    AddDynamicContent<TContext>(serviceCollection, optionsAction);

    return serviceCollection.AddDbContext<TContext, TContext>(
        optionsAction, 
        contextLifetime, 
        optionsLifetime
    );
}

private static void AddDynamicContent<TContext>(
    IServiceCollection serviceCollection, 
    Action<DbContextOptionsBuilder> optionsAction = null, 
    Action<IServiceProvider, DbContextOptionsBuilder> 
        optionsActionDependencyInjection = null) where TContext : DbContext
{
    serviceCollection.AddScoped<DynamicContext<TContext>>();

    // If no action, then it would need to be added manually
    if(optionsAction == null && optionsActionDependencyInjection == null)
    {
        return;
    }

    // Create an options instance with the Action populated. 
    // One of the two will always be null
    var options = new DynamicContextInitOptions<TContext>
    {
        optionsAction = optionsAction,
        optionsActionDependencyInjection = optionsActionDependencyInjection
    };

    // add the type to the DI container
    serviceCollection.AddSingleton(typeof(options);
}
```

For a specific DbContext, we now know how it was initialized. So if we inject `DynamicContextInitOptions<TContext>` into `DynamicContext` and then into `RuntimeContext`, it can be used to initialized dynamically.  

``` csharp
internal class RuntimeContext<T> 
    : DbContext where T : class 
{
    private readonly DynamicContextInitOptions<DbContext> _contextInitAction;

    private readonly IServiceProvider _serviceProvider;

    public RuntimeContext(
        DynamicContextInitOptions<DbContext> contextInitAction, 
        IServiceProvider serviceProvider)
    {
        _contextInitAction = contextInitAction;
        _serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // init the context based on how the initial TContext was initially initialized
        if (_contextInitAction.optionsAction != null)
        {
            _contextInitAction.optionsAction.Invoke(optionsBuilder);
        }
        else
        {
            _contextInitAction
                .optionsActionDependencyInjection
                .Invoke(_serviceProvider, optionsBuilder);
        }

        base.OnConfiguring(optionsBuilder);
    }

    // Class continues...
}
```

We can now inject a DynamicContext into a relevant constructor, have access to dynamic functionality as well as the underlying original DbContext, and we can dynamically initialized the dynamic context based on how the underlying original context was initialized.

**Dynamic configuration taken care of!**

---

### DynamicContext wrapper

Next we'll add a few methods to the wrapper `DynamicContext` to provide access to the internal `RuntimeContext`, as well as making using the `reference type wrapper, SimpleType<T>` easier.

- First a method to add a DbSet dynamically based on the `generic entity` _TEntity_:
``` csharp
public DbSet<TEntity> Set<TEntity>() where TEntity : class
{
    // if the type is on the original context, 
    // then don't initialize the dynamic context
    if (_originalContext.Model.FindEntityType(typeof(TEntity)) != null)
    {
        return _originalContext.Set<TEntity>();
    }

    var runtimeContext = new RuntimeContext<TEntity>(
        _runtimeInitAction, 
        _serviceProvider);
    return runtimeContext.Set<TEntity>();
}
```

To invoke this method:
``` csharp
var blogs = dynamicContext.Set<BlogUrl>()
    .FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList()
```

- Next, a method to add DbSet dynamically based on an `anonymous object`:
``` csharp
public DbSet<TEntity> Set<TEntity>(TEntity example) 
    where TEntity : class
{
    _ = example;

    // if the type is on the original context, 
    // then don't initialize the dynamic context
    if (_originalContext.Model.FindEntityType(typeof(TEntity)) != null)
    {
        return _originalContext.Set<TEntity>();
    }

    var runtimeContext = new RuntimeContext<TEntity>(
        _runtimeInitAction, 
        _serviceProvider);
    return runtimeContext.Set<TEntity>();
}
```

To invoke this method:

``` csharp
var anonBlogUrl = new { BlogId = 0, Url = "" };
var blogs = dynamicContext.Set(anonBlogUrl)
    .FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList();
```


- Next, a method to `wrap the explicit SimpleType<T> usage`. With this method SimpleType\<T\> does not need to be used explicitly:
``` csharp
public DbSet<SimpleType<TEntity>> ValueSet<TEntity>() where TEntity : struct 
{
    // as the constraint is on struct, we have this additional check 
    // just to make sure its a struct of a relevant type 
    // (int, long, float, bool etc)
    if(!IsValidType(typeof(TEntity)))
    {
        throw new InvalidOperationException(
            $"Type '{typeof(TEntity).Name}' is not supported");
    }

    var runtimeContext = GetInternalRuntimeContext(new SimpleType<TEntity>());
    return runtimeContext.Set<SimpleType<TEntity>>();
}
```

To invoke this method:
``` csharp
var blogIds = dynamicContext.ValueSet<Guid>()
    .FromSqlRaw("SELECT Id as Value FROM Blog")
    .AsNoTracking().Select(x => x.Value).ToList();
```

- Lastly, as `string is not a struct, it has to be handled separately`:
``` csharp
public DbSet<SimpleType<TEntity>> StringSet<TEntity>() 
    where TEntity : IEnumerable<char>, IEnumerable, ICloneable, 
        IComparable, IComparable<string>, IConvertible, IEquatable<string>
{
    var runtimeContext = GetInternalRuntimeContext(new SimpleType<TEntity>());
    return runtimeContext.Set<SimpleType<TEntity>>();
}
```

To invoke this method:
``` csharp
var urls = context.StringSet<string>()
    .FromSqlRaw("SELECT Url as Value FROM Blog")
    .AsNoTracking().Select(x => x.Value).ToList();
```

We can have user friendly methods which are similar to the DbContext _Set_ methods, and which wrap some of the annoyance of having to use _SimpleType\<T\>_ explicitly.

**Simple value usage taken care of!**

---

### Value column

The last issue to resolve is the fact that the `Set<>` methods for simple types (struct and string) return a `DbSet of SimpleType object and not the simple type value itself`. 

One option, is to explicitly select the value out the IQueryable, as seen in `line 4 below`:
``` csharp
var urls = context.StringSet<string>()
    .FromSqlRaw("SELECT Url as Value FROM Blog")
    .AsNoTracking()
    .Select(x => x.Value)
    .ToList();
```

The other option is to add some extension methods to easily unpack the `SimpleType<T>` into the `T`:
``` csharp
public static IQueryable<TEntity> ToSimple<TEntity>(
        this IQueryable<SimpleType<TEntity>> query) 
    where TEntity : struct, IComparable, IFormattable, 
    IComparable<TEntity>, IEquatable<TEntity>
{
    return query.Select(entity => entity.Value).AsQueryable();
}

public static IQueryable<string> ToSimple(this IQueryable<SimpleType<string>> query) 
{
    return query.Select(entity => entity.Value).AsQueryable();
}
```

To invoke this method:
``` csharp
var blogIds = context.ValueSet<Guid>()
    .FromSqlRaw("SELECT Id as Value FROM Blog")
    .AsNoTracking()
    .ToSimple()
    .ToList();
```

<?# InfoBlock ?>
The solution still requires that the SQL command have a column returned with the name of 'Value'. With some additional effort, this constraint could be resolved , but this is outside the scope of this post.
<?#/ InfoBlock ?>

**Value column partially taken care of!**

---

## Nuget Package

All the above functionality has been packed into a **[NuGet package which is ready to use, and is available here](https://www.nuget.org/packages/AlwaysDeveloping.EntityFrameworkCore.DynamicContext/)**.  

**[Full source code is also available on GitHub here](https://github.com/always-developing/AlwaysDeveloping.EntityFrameworkCore.DynamicContext)**

---

## Performance benchmarks

Some performance benchmarks of using the DynamicContext vs DbContext directly and projecting the values out (using .NET 6, EF Core 6 and Sqlite)

The first set of results benchmark `retrieving a list of ids and urls from a database of 500 records, then 100 000 records`.
``` csharp
// DirectDbSet
var blogs = context.Blog.AsNoTracking().ToList();

// GenericDbSet
var blogs1 = context.Set<Blog>().AsNoTracking().ToList();

// GenericDbSetRaw
var blogs2 = context.Set<Blog>()
    .FromSqlRaw("SELECT * FROM Blog").AsNoTracking().ToList();

// GenericDbSetProj
var blogs3 = context.Set<Blog>()
    .Select(x => new BlogUrl { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();

// GenericDbSetRawProj
var blogs4 = context.Set<Blog>()
    .FromSqlRaw("SELECT Id, Url FROM Blog")
    .Select(x => new BlogUrl { BlogId = x.Id, Url = x.Url })
    .AsNoTracking().ToList();

// DynamicDbSet
var blogs5 = dynamicContext.Set<BlogUrl>()
    .FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList();

// DynamicDbSetAnon
var anonBlogUrl = new { BlogId = 0, Url = "" };
var blogs6 = dynamicContext.Set(anonBlogUrl)
    .FromSqlRaw("SELECT Id as BlogId, Url FROM Blog")
    .AsNoTracking().ToList();
```

`500 records`:

|              Method |       Mean |    Error |   StdDev | Ratio |   Gen 0 |   Gen 1 | Allocated |
|-------------------- |-----------:|---------:|---------:|------:|--------:|--------:|----------:|
|         DirectDbSet | 1,049.3 us | 10.66 us |  9.97 us |  1.00 | 76.1719 | 25.3906 |    477 KB |
|        GenericDbSet | 1,054.4 us | 12.71 us | 11.89 us |  1.00 | 76.1719 | 25.3906 |    477 KB |
|     GenericDbSetRaw | 1,070.9 us | 18.19 us | 16.12 us |  1.02 | 78.1250 | 25.3906 |    481 KB |
|    GenericDbSetProj |   638.2 us |  9.48 us |  8.87 us |  0.61 | 47.8516 | 12.6953 |    296 KB |
| GenericDbSetRawProj |   661.1 us |  3.98 us |  3.53 us |  0.63 | 48.8281 | 14.6484 |    302 KB |
|        DynamicDbSet |   696.8 us |  3.22 us |  2.69 us |  0.66 | 54.6875 | 14.6484 |    338 KB |
|    DynamicDbSetAnon |   605.2 us |  5.51 us |  5.15 us |  0.58 | 45.8984 | 13.6719 |    287 KB |  

  
`100 000 records` (Gen 0, 1 and 2 decimals truncated for space reasons):

|              Method |     Mean |   Error |  StdDev | Ratio |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|-------------------- |---------:|--------:|--------:|------:|---------:|--------:|--------:|----------:|
|         DirectDbSet | 268.5 ms | 2.02 ms | 1.89 ms |  1.00 | 15000.00 | 5000.00 | 2000.00 |     81 MB |
|        GenericDbSet | 268.6 ms | 2.47 ms | 2.19 ms |  1.00 | 15000.00 | 5000.00 | 2000.00 |     81 MB |
|     GenericDbSetRaw | 267.9 ms | 3.11 ms | 2.91 ms |  1.00 | 15000.00 | 5000.00 | 2000.00 |     81 MB |
|    GenericDbSetProj | 143.8 ms | 2.60 ms | 3.09 ms |  0.54 |  8250.00 | 3000.00 | 1000.00 |     46 MB |
| GenericDbSetRawProj | 140.8 ms | 2.76 ms | 3.06 ms |  0.52 |  8250.00 | 3000.00 | 1000.00 |     46 MB |
|        DynamicDbSet | 143.9 ms | 2.11 ms | 1.97 ms |  0.54 |  8250.00 | 3000.00 | 1000.00 |     46 MB |
|    DynamicDbSetAnon | 118.1 ms | 2.28 ms | 2.02 ms |  0.44 |  6400.00 | 2400.00 | 1000.00 |     36 MB |

As expected, projecting the required fields out, is faster and requires less memory than retrieving all the data. Using the `DynamicContext` is comparable to projecting the specific values out - `the overhead of dynamically creating the DynamicContext is negligible`.

---

The next set of results benchmark `retrieving a list of simple types (Guid) from a database of 500 records, then 100 000 records`.
``` csharp
// DirectDbSet
var blogIds = context.Set<Blog>()
    .AsNoTracking().Select(x => x.Id).ToList();

// ValueSetSelect
var blogIds1 = dynamicContext.ValueSet<Guid>()
    .FromSqlRaw("SELECT Id as Value FROM Blog")
    .AsNoTracking().Select(x => x.Value).ToList();

// ValueSetToSimple
var blogIds2 = dynamicContext.ValueSet<Guid>()
    .FromSqlRaw("SELECT Id as Value FROM Blog")
    .AsNoTracking().ToSimple().ToList();

// DirectDbSetString
var urls = context.Set<Blog>()
    .FromSqlRaw("SELECT Url FROM Blog")
    .AsNoTracking().Select(x => x.Url).ToList();

// StringSetSelect
var urls1 = dynamicContext.StringSet<string>()
    .FromSqlRaw("SELECT Url as Value FROM Blog")
    .AsNoTracking().Select(x => x.Value).ToList();

// StringSetToSimple
var urls2 = dynamicContext.StringSet<string>()
    .FromSqlRaw("SELECT Url as Value FROM Blog")
    .AsNoTracking().ToSimple().ToList();
```

`500 records`:

|            Method |     Mean |    Error |    StdDev | Ratio |   Gen 0 |   Gen 1 | Allocated |
|------------------ |---------:|---------:|----------:|------:|--------:|--------:|----------:|
|       DirectDbSet | 864.1 us | 38.04 us | 111.55 us |  1.00 |       - |       - |    226 KB |
|    ValueSetSelect | 439.1 us |  8.32 us |  14.12 us |  0.52 | 37.1094 |  8.7891 |    233 KB |
|  ValueSetToSimple | 435.7 us |  7.98 us |   8.20 us |  0.51 | 37.1094 |  6.8359 |    232 KB |
| DirectDbSetString | 610.5 us | 12.65 us |  14.06 us |  0.71 | 38.0859 |  1.9531 |    233 KB |
|   StringSetSelect | 437.9 us |  8.53 us |  11.67 us |  0.50 | 39.0625 | 10.7422 |    240 KB |
| StringSetToSimple | 439.3 us |  8.56 us |  12.28 us |  0.51 | 39.0625 |  9.7656 |    240 KB |

`100 000 records` (Gen 0, 1 and 2 decimals truncated for space reasons):

|            Method |     Mean |    Error |   StdDev | Ratio |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------ |---------:|---------:|---------:|------:|--------:|--------:|--------:|----------:|
|       DirectDbSet | 58.67 ms | 1.167 ms | 2.276 ms |  1.00 | 4000.00 | 1000.00 | 1000.00 |     32 MB |
|    ValueSetSelect | 54.48 ms | 1.058 ms | 1.132 ms |  0.91 | 5500.00 | 1100.00 | 1000.00 |     32 MB |
|  ValueSetToSimple | 51.66 ms | 0.399 ms | 0.354 ms |  0.86 | 5444.44 | 1111.11 | 1000.00 |     32 MB |
| DirectDbSetString | 81.04 ms | 1.153 ms | 0.963 ms |  1.00 | 6000.00 | 2000.00 | 1000.00 |     33 MB |
|   StringSetSelect | 92.20 ms | 1.359 ms | 1.271 ms |  1.14 | 5666.67 | 2000.00 |  833.33 |     33 MB |
| StringSetToSimple | 92.26 ms | 1.789 ms | 1.915 ms |  1.14 | 5666.67 | 2000.00 |  833.33 |     33 MB |

For the `non-string values, using DynamicContext is faster`, while using roughly the same memory, especially with more records. For `string values DynamicContext is slower` - but with the tradeoff of it being more dynamic.

---

## Closing

The post outlines solutions to be able to:
- Retrieve data into an entity (or collection of entities) without a DbSet, using raw SQL
- Retrieve data into an entity or collection of entities) based on an anonymous object, using raw SQL
- Retrieve data into a simple type without having to define a reference type to use as a DbSet

The performance of the library is either faster or comparable to using a DbContext, but as always, test and benchmark and make an informed decision in your specific use case.

Full source code available on [Github](https://github.com/always-developing/AlwaysDeveloping.EntityFrameworkCore.DynamicContex) and fully functionality package available on [NuGet](https://www.nuget.org/packages/AlwaysDeveloping.EntityFrameworkCore.DynamicContext/).
