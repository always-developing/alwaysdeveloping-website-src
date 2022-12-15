---
title: "Entity Framework conventions"
lead: "Customizing Entity Framework through the use of conventions"
Published1: "12/16/2022 01:00:00+0200"
slug: "16-ef-conventions"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - entityframework
   - convention


---

## Daily Knowledge Drop

Entity Framework comes out of the box with a number of default _conventions_ - however `new conventions can easily be added` to an application specific EF configuration to customize how Entity Framework operates.

---

## Scenario

By default, when a `string` property on an entity is mapped to a SQL database column, it will be generated as `nvarchar(max)`. 

Consider the following class:

``` csharp
[Table("Blog")]
public  class Blog
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime DateCreated { get; set; }
}
```

And the DbContext configuration, some additional logging for demo purposes:

``` csharp
public class DemoContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=ConfigConventions;Integrated Security=True;TrustServerCertificate=True");

        // enable additional logging for demo purposes
        optionsBuilder.EnableSensitiveDataLogging(true);
        optionsBuilder.LogTo((string query) =>
        {
            Console.WriteLine(query);
        }, LogLevel.Information);
    }
}
```

The _CREATE TABLE SQL_ generated is as follows:

``` sql
CREATE TABLE [Blog] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_Blog] PRIMARY KEY ([Id])
);
```

As one can see, the default for C# properties of type `string` is `nvarchar(max)` - next we'll go through the various steps to change the convention for _non-explicitly set string_ properties from `nvarchar(max)` to `nvarchar(256)`.

---

### Convention definition

The first step is to define the _convention_ to change the default string length. This is achieved by implementing the `IModelFinalizingConvention` interface (which inherits the `IConvention` interface). The `IModelFinalizingConvention` implementations, as the name suggests, are executed once the model has mostly been built (using the built-in and other custom conventions), and is being _finalized_. This is the "safest" time to execute custom conventions.

``` csharp
public class MaxStringLengthConvention : IModelFinalizingConvention
{
    private readonly int _maxLength;

    public MaxStringLengthConvention(int maxLength)
	{
        this._maxLength = maxLength;
    }

    // implement the only method on the interface
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, 
        IConventionContext<IConventionModelBuilder> context)
    {
        // iterate through each type defined in the EF model
        // and through each property and check if a string
        foreach (var property in modelBuilder.Metadata.GetEntityTypes()
                     .SelectMany(entityType => entityType.GetDeclaredProperties()
                                    .Where( property => property.ClrType == typeof(string))))
        {
            // set the max length based on the value passed in
            property.Builder.HasMaxLength(_maxLength);
        }
    }
}
```

The implementation for this convention effectively checks _each type_ and _each property_ on the type to see if it is of type _string_ - if so, the max length is set based on the max length specified on initialization.

Now to register the newly created convention!

---

### Convention configuration

The convention is required to be registered with Entity Framework - so EF knows to apply the convention to the model (adn when to apply it)

The `ConfigureConventions` method is _overridden_ on the _DbContext_, and the convention(s) registered:

``` csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Conventions.Add(_ => new MaxStringLengthConvention(256));
}
```

---

### Generated SQL

With the convention configured, the SQL generated now looks as follows:

``` sql
CREATE TABLE [Blog] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(256) NOT NULL,
    [Description] nvarchar(256) NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_Blog] PRIMARY KEY ([Id])
);
```

All string properties/columns are set to have a length of 256 by default. 

---


### Explicitly set properties

Entity Framework is smart enough to know that if the _max length_ has explicitly been set on a property/column, then the default length set by the convention will `not be applied`.

If the _Title_ property on the _Blog_ was set to have a max length of 500:

``` csharp
[Table("Blog")]
public  class Blog
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime DateCreated { get; set; }
}
```

The SQL generated is now as follows:

``` sql
CREATE TABLE [Blog] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(500) NOT NULL,
    [Description] nvarchar(256) NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_Blog] PRIMARY KEY ([Id])
);
```

An explicitly defined max length on a property, takes precedence over the max length specified by the convention. 

---

## Notes

While all of the above could have been achieved by overwriting the `OnModelCreating` method on the _DbContext_, the conventions approach is more flexible and reusable. The conventions can be packaged and shared across teams in an enterprise to ensure consistent database standards.

---


## References

[.NET Data Community Standup - EF7 Custom Model Conventions](https://www.youtube.com/watch?v=6apfe1L1FhY&t=2041s)  

<?# DailyDrop ?>224: 16-12-2022<?#/ DailyDrop ?>
