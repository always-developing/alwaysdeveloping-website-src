---
title: "Entity Framework: IQueryable vs IEnumerable"
lead: "How different collection types impact Entity Framework queries"
Published: "08/30/2022 01:00:00+0200"
slug: "30-ef-query-enumerable"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - efcore
   - entityframework
   - iqueryable
   - ienumerable

---

## Daily Knowledge Drop

When querying data using `Entity Framework (Core)` the type of collection used before materializing the results drastically changes how the LINQ is translated and executed.

Using:
- `IQueryable`: The LINQ is translated to SQL and executing on the database (SQL Server in my examples below)
- `IEnumerable`: The data is retrieved, and then the LINQ applied to the full dataset in memory

---

## Sample

In these examples, we have SQL database containing _Blog_ information. One of the tables, is the `Tag` table which contains all the _tags_ which have been assigned to specific blog posts.

### Query

An important aspect to remember when using Entity Framework (Core) and LINQ, is that a call is only made to the database and the results materialized (stored in memory) when needed and explicitly asked for.

To query all _Tags_ in the database:

``` csharp
// usual DbContext setup
var context = new DemoContext();

// query using the DbSet itself
DbSet<Tag> dbSetTags = context.Tags;
// materialize the results. The actual database
// call is done when ToList is called
var dbSetList = dbSetTags.ToList();

// query using IQueryable
IQueryable<Tag> queryableTags = context.Tags;
var queryList = queryableTags.ToList();

// Query using IEnumerable
IEnumerable<Tag> enumerableTags = context.Tags;
var enumerableList = enumerableTags.ToList();
```

As _DbSet_ implements _IQueryable_, which implements _IEnumerable_ - all of the above casts are valid.

As mentioned above, the database is not actually queried until the data is explicitly asked for, which in the above examples, is when `ToList is called`.

Executing the above code results in all three generating the same SQL:

``` sql
SELECT [t].[Id], [t].[DateCreated], [t].[Name], [t].[Used]
FROM [Tag] AS [t]
```

Fairly straightforward and so far the results are as expected.

---

### Filter/Ordering

When _filtering_ or _data ordering_ is applied however, things can go wrong if not done correctly. The same three examples as above will be used, but with a _filter_ applied using `Where` and _ordering_ applied by using `OrderBy`:

``` csharp
// usual DbCOntext setup
var context = new DemoContext();

// query using the DbSet itself
DbSet<Tag> dbSetTagsFilter = context.Tags;
// apply filter, ordering and then materialize
var dbSetTagsFilterList = dbSetTagsFilter
    .Where(t => t.Id > 50)
    .OrderBy(t => t.DateCreated)
    .ToList();

// query using IQueryable
IQueryable<Tag> queryTagsFilter = context.Tags;
// apply filter, ordering and then materialize
var queryTagsFilterList = queryTagsFilter
    .Where(t => t.Id > 50)
    .OrderBy(t => t.DateCreated)
    .ToList();

// query using IEnumerable
IEnumerable<Tag> enumerableTagsFilter = context.Tags;
// apply filter, ordering and then materialize
var enumerableTagsFilterList = enumerableTagsFilter
    .Where(t => t.Id > 50)
    .OrderBy(t => t.DateCreated)
    .ToList();
```

On the surface, these might all look the same, however, when looking at the SQL generated there is a `fundamental difference` between them.

The first two, `DbSet` and `IQueryable` generate the following SQL:

``` sql
SELECT [t].[Id], [t].[DateCreated], [t].[Name], [t].[Used]
FROM [Tag] AS [t]
WHERE [t].[Id] > 50
ORDER BY [t].[DateCreated]
```

While the third method, using `IEnumerable` generates the following:

``` sql
SELECT [t].[Id], [t].[DateCreated], [t].[Name], [t].[Used]
FROM [Tag] AS [t]
```

When using `DBSet` or `IQueryable`:
- The LINQ is translated into SQL and executed on the database. This limits the amount of data transferred from the database to the application

When using `IEnumerable`:
- First all the data is retrieved and then the filtering/ordering applied in memory. This leads to slower performance (as more data needs is transferred) and more memory usage

---

## Notes

When working with Entity Framework (Core) make sure to understand how the collections work, and which to use, when materialization occurs and how the LINQ gets translated - if done incorrectly, one could see a massive negative performance impact.

---

<?# DailyDrop ?>149: 30-08-2022<?#/ DailyDrop ?>
