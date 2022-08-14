---
title: "Dynamic EF operations with EF.Property"
lead: "Using EF.Property to dynamically reference entity properties using a string"
Published: "09/06/2022 01:00:00+0200"
slug: "06-ef-property"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - property

---

## Daily Knowledge Drop

The `EF.Property` static method can be used to `reference an entity property dynamically` using a `string representation of the property name`. This can be leveraged to perform dynamic ordering or filtering on the DbSet, for example.

---

## Usage

In all of the examples below, a simple _Blog_ table is being used which contains a list of 10 000 blog records.

---

### Selection

Suppose we wanted to get a list of all _Blog Titles_ - a simple enough query using Entity Framework:

``` csharp
var titles = context.Blogs
    .Select(e => e.Title)
    .ToList();
```

Suppose now we have a requirement to allow the user to choose which specific column data should be returned. This could be done with a `switch statement` or `multiple if statements`, but this is not a sustainable approach. The better option is to use `Ef.Property` - this allows for the `column name to be specified as a string`:

``` csharp
var dynTitle = context.Blogs
    .Select(e => EF.Property<string>(e, "Title"))
    .ToList();
```

Here we get all _Titles_, without ever referencing the _Title_ property on the _Blog_ entity directly.

This can be enhanced, and a generic method created to `select values from the specified table and column`:

``` csharp
// get all ids from the Blog table
var ids = GetColumnValues<Blog, int>("Id");
Console.WriteLine($"Num Ids in list: {ids.Count}");
Console.WriteLine($"First Id in list: {ids.First()}");

// the entity type and the colum value type are specific
// as well as the column name
public List<TColumn> GetColumnValues<TEntity, TColumn>(
    string columnName) where TEntity : class
{
    using var context = new DemoContext();

    return context.Set<TEntity>()
        .Select(e => EF.Property<TColumn>(e, columnName))
        .ToList();
}
```

The output is as follows:

``` terminal
Num Ids in list: 10000
First Id in list: 1
```

We now have a dynamic method to retrieve all values from the specified column.  

The `EF.Property` can be used anywhere a entity property would traditionally be used - this means it could also be used for dynamic ordering and filtering!

---

### Ordering

We can use the above `GetColumnValues` method as a template for a new method to perform _dynamic ordering_ on the _Blog_ entity:

``` csharp
List<Blog> GetBlogs(string sortProperty, int pageSize)
{
    using var context = new DemoContext();

    // instead of specifying the column to order by 
    // directly, EF.Property is used
    return context.Blogs
        .OrderBy(e => EF.Property<object>(e, sortProperty))
        .Take(pageSize)
        .ToList();
}
```

This can now be invoked as follows:

``` csharp
// get 5 blogs sorted by Id
var sortedById = GetBlogs("Id", 5);
Console.WriteLine($"Sorted by Id, the first Id " +
    $"is: {sortedById.First().Id}");

// get 5 blogs sorted by Title
var sortedByTitle = GetBlogs("Title", 5);
Console.WriteLine($"Sorted by Title, the first Id " +
    $"is: {sortedByTitle.First().Id}");
```

The output:

``` terminal
Sorted by Id, the first Id is: 1
Sorted by Title, the first Id is: 665
```

---

### Filtering

One final example of how `EF.Property` can be leveraged, is to perform _dynamic filtering_:

``` csharp
public List<Blog> GetFilterBlogs<T>(string sortProperty, T value, int pageSize)
{
    using var context = new DemoContext();

    return context.Blogs
        .Where(e => EF.Property<T>(e, sortProperty).Equals(value))
        .Take(pageSize)
        .ToList();
}
```

This allows for a _column name_ and _column value_ to be `specified dynamically and have filtering applied`:

``` csharp
// filter by id
var filteredById = GetFilterBlogs<int>("Id", 1011, 1);
Console.WriteLine($"Filtered count: {filteredById.Count}");

// filter by title
var filteredByTitle = GetFilterBlogs<string>("Title", 
    "Dynamic EF operations with EF.Property", 1);
Console.WriteLine($"Filtered count: {filteredByTitle.Count}");
```

Executing the above:

``` terminal
Filtered count: 1
Filtered count: 1
```

---

## Notes

`Ef.Property` is a very useful utility method when required to change the target column of operations at runtime, such as when ordering or filtering dynamically (from a user interface grid, for example). There may be a performance impact in using this over referencing the column directly (but this should be benchmarked to confirm for the specific use case) - however these are the tradeoffs which need to be considered when deciding which method to use.

---

## References

[EF.Property<TProperty>(Object, String) Method](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.ef.property?view=efcore-6.0)   

<?# DailyDrop ?>154: 06-09-2022<?#/ DailyDrop ?>
