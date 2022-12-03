---
title: "EF7 GroupBy support"
lead: "Entity Framework 7 adds supports for GroupBy as the final operator"
Published: "12/08/2022 01:00:00+0200"
slug: "08-ef-group-by"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - entityframework
   - ef7
   - groupby

---

## Daily Knowledge Drop

Prior to Entity Framework 7, having a LINQ `GroupBy` as the final operator in a query would result in an exception being thrown - EF7 adds support for this scenario, allowing `GroupBy` to be the final operator.

---

## Scenario

In this scenario, we have a simple _Blog_ class, setup as a _DbSet_ in a _DbContext_, configured to connect to SQL Server:

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

Suppose we want to _select all Blogs_, but `group them by the DateCreated` - lets see how this can be done.

---

### Before EF7

If we try perform the _GroupBy_ as the final operation `prior to EF7`:

``` csharp
// get an instance of the DBContext
DemoContext context = new DemoContext();

// we're not querying the actual database
// just getting the query which would be 
// generated
var query = context
    .Blogs
    .GroupBy(b => b.DateCreated)
    .ToQueryString();

Console.WriteLine(query);
```

An exception is thrown!

``` terminal
The LINQ expression 'DbSet<Blog>().GroupBy(b => b.DateCreated)' could not be translated. 
Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable
```

To perform this, the _group by_ logic would need to be done in code:

``` csharp
var context = new DemoContext();

// get all relevent blogs, ungrouped
var List<Blog> = context.Blogs.ToList();

// group the blogs by date in code
List<IGrouping<DateTime, Blog>>? blogByDate = 
    blogs
    .GroupBy(b => b.DateCreated)
    .ToList();
```

So the requirement to group by _DateCreated_ is definitely do-able, just maybe not as simple and intuitive to do as it should be. 

---

### EF7

EF7 makes implementing the same requirement a little bit easier. Upgrading to EF7, and executing the exact same code as before:

``` csharp
// get an instance of the DBContext
DemoContext context = new DemoContext();

// we not querying the actual database
// just getting the query the following would
// generate
var query = context
    .Blogs
    .GroupBy(b => b.DateCreated)
    .ToQueryString();

```

Now yields the following output:

``` terminal
SELECT [b].[DateCreated], [b].[Id], [b].[Description], [b].[Title]
FROM [Blog] AS [b]
ORDER BY [b].[DateCreated]
```

The _GroupBy_ is not perform on the database itself, but is again, performed in code - just this time it is done automatically by EF.

The output type is the same as when the operation is performed manually:

``` csharp
var context = new DemoContext();
List<IGrouping<DateTime, Blog>>? query = context
    .Blogs
    .GroupBy(b => b.DateCreated)
    .ToList();
```


---

## Notes

A relatively small enhancement to EF, but one which makes building up queries in EF using LINQ more intuitive, and results in the developer having to do slightly less work overall.

---


## References

[Oleg Kyrylchuk Tweet](https://twitter.com/okyrylchuk/status/1595887786535575554)  

<?# DailyDrop ?>218: 08-12-2022<?#/ DailyDrop ?>
