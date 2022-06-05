---
title: "Soft deletes with EF global filters"
lead: "How EF Core global filters can be used to simplify soft deletes"
Published: 04/26/2022
slug: "26-ef-soft-delete"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - efcore
    - entityframeworkcore
    - queryfilter
    - filter

---

## Daily Knowledge Drop

Entity Framework Core's `global filter` functionality can be used to a apply a filter automatically to all queries on a dbSet.  

This is especially useful when dealing with `soft delete` functionality, where the data is not removed from the database table, but instead just marked as deleted (or archived or retired etc)

---

## Setup

Consider a _Song_ class and _IRetirable_ interface which is as follows:


``` csharp
public interface IRetirable
{
    public bool IsRetired { get; set; }
}

[Table("Song")]
public class Song : IRetirable
{
    public int Id { get; set; } 

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int LengthInSeconds { get; set; }

    public bool IsRetired { get; set; }

    public override string ToString()
    {
        return $"Song `{Name}` by '{Artist} released " +
            $"in '{YearReleased}' and is '{LengthInSeconds}' seconds long";
    }
}
```

---

## Without global filters

Every time the _Song_ dbset is queried, only active records (records which have the _IsRetired_ field set to false) should be returned.

This results in most queries requiring the `IsRetired == false` filter added each LINQ _Where_ expression:

``` csharp
using (var db = new EFGlobalFilterContext())
{
    // get all active songs
    var activeSongs = db.Songs.Where(s => s.IsRetired == false).ToList();

    foreach (var item in activeSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }

    Console.WriteLine("=====");

    // get all active songs with a length of 260 seconds
    var timedSongs = db.Songs.Where(s => s.IsRetired == false 
        && s.LengthInSeconds == 260);
    foreach (var item in timedSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }

    Console.WriteLine("=====");

    // get all songs
    var allSongs = db.Songs.ToList();

    foreach (var item in allSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }
}
```

Having to ensure this additional condition is always added becomes tedious, and prone to error as it can easily be forgotten. It also results in a lot of duplicate filter expressions.

Enter `global filters` to simplify the entire process.

---

## With global filters

A `global filter` can be applied to all, or specific DbSets on the DBContext when the model is being created.

On the DBContext, the _OnModelCreating_ method can be overwritten and the `global filter` specified:

``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // For each entity type (DbSet) on the model
    modelBuilder.Model.GetEntityTypes()
         // if the entity type implements IRetirable
        .Where(entityType => typeof(IRetirable).IsAssignableFrom(entityType.ClrType))
        .ToList()
        // Build up the expression `IsRetired == false`. It's safe to always add this 
        // expression, as the IRetirable interface will ensure 
        // the entity always has a _IsRetired_ field
        .ForEach(entityType =>
        {
            var parameter = Expression.Parameter(entityType.ClrType, "p");
            var deletedCheck = Expression.Lambda(
                Expression.Equal(Expression.Property(parameter, "IsRetired"), 
                    Expression.Constant(false)), 
                parameter);

            // Apply the filter to the entity type
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);
        });

    base.OnModelCreating(modelBuilder);
}
```

With this filter applied, each time the _entityType_ DbSet is queried, the filter `IsRetired == false` will automatically be applied.

The EFGlobalFilterContext usage example from above, can now be simplified: 

``` csharp
using (var db = new EFGlobalFilterContext())
{
    var activeSongs = db.Songs.ToList();

    foreach (var item in activeSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }

    Console.WriteLine("=====");

    var timedSongs = db.Songs.Where(s => s.LengthInSeconds == 260);
    foreach (var item in timedSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }

    Console.WriteLine("=====");

    var allSongs = db.Songs.IgnoreQueryFilters().ToList();

    foreach (var item in allSongs)
    {
        Console.WriteLine($"Artist = {item.Artist}, Song = {item.Name}");
    }
}
```

The `IsRetired == false` does not need to explicitly be applied, but it explicitly needs to be _excluded_ if required using the _IgnoreQueryFilters_ method. An example of this is done on **line 20**, where ALL songs are retrieved.

---

## Notes

Adding `global filters` are very easy and incredibly powerful and useful. Code can be kept clean and simplified, with the repetitive filter expression offloaded to be handled by EF Core.

---

## References

[Using EF Core Global Query Filters To Ignore Soft Deleted Entities](https://dotnetcoretutorials.com/2022/03/17/using-ef-core-global-query-filters-to-ignore-soft-deleted-entities/)  

<?# DailyDrop ?>60: 26-04-2022<?#/ DailyDrop ?>
