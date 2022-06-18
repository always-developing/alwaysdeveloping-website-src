---
title: "EF Core 6 EntityTypeConfiguration Attribute"
lead: "Discover the new EntityTypeConfiguration attribute added in EF Core 6"
Published: 02/21/2022
slug: "21-ef-core6-entitytypeconfig"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - efcore
    - efcore6
    - entityframework
    - entitytypeconfiguration
    - dbcontext
    
---

## Daily Knowledge Drop

A new attribute called `EntityTypeConfiguration` was introduced in Entity Framework 6, which allows for easier configuration of custom entity configurations.

Let's take a look at how is simplifies the configuration of an entity.

---

## Configurations previously

There are a number of steps performed when manually configuring an entity using Entity Framework Core (EF).

Note: _The entity structure below is not optimized or suitable for a production system, it is used just for sample purposes_

### Define the entity

In this example, we are going to be using a `Song entity`.  

This is straight forward and nothing EF related here.

``` csharp
public class Song
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int LengthInSeconds { get; set; }
}
```

---

### Add the DbSet

The next step is to make EF aware of this entity. We do this by creating a `Dbset (using the entity) on the DbContext`:

``` csharp
public class EntityTypeConfigContext : DbContext
{
    // This links the entity to EF
    public DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=EntityTypeConfig;Integrated Security=True");
    }
}
```

---

### Customize entity configuration

The next step is to create a `configuration for the entity`.  

The default SQL column size for strings is _nvarchar(max)_ - the below configuration changes this to 250 chars for the entity, for example.

``` csharp
public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasIndex(p => p.Id, "Id");
        builder.Property(p => p.Name).HasMaxLength(250);
        builder.Property(p => p.Artist).HasMaxLength(250);
    }
}
```

---

### Apply the configuration

The final step is to `apply the configuration` to the DbContext:

``` csharp
public class EntityTypeConfigContext : DbContext
{
    public DbSet<Song> Songs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // when the model is creating, specify
        // the configuration should be used
        modelBuilder.ApplyConfiguration(new SongConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=EntityTypeConfig;Integrated Security=True");
    }
}
```

---

## EntityTypeConfiguration attribute

The above 4 steps will still work with EF Core 6, however the last step can now be simplified.

Instead of manually adding each entity configuration to the _OnModelCreating_ method on the context, the entity itself can be tagged with the `EntityTypeConfiguration` attribute.

``` csharp
// This attribute will ensure the "SongConfiguration" is used 
// when configuring the "Song" entity in EF
[EntityTypeConfiguration(typeof(SongConfiguration))]
public class Song
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int LengthInSeconds { get; set; }
}
```

The `SongConfiguration` configuration will now automatically be used for the `Song` entity by the EF when creating the database model.

---

## Notes

While using the `EntityTypeConfiguration` attribute is cleaner and easier, and probably likely less to be overlooked by the developer, the technique will not work for everyone. Depending on the application design/architecture it might not make sense to "pollute" the entity wth any EF specific references.

What is important though, is that there are _now multiple options to leverage_ depending on what sense for your specific use case.

---

## References
[Entity Framework Core 6 features - Part 1](https://blog.okyrylchuk.dev/entity-framework-core-6-features-part-1)

<?# DailyDrop ?>15: 21-02-2022<?#/ DailyDrop ?>
