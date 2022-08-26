---
title: "Bulk update (and delete) in EF7"
lead: "New bulk update and delete functionality coming with Entity Framework 7"
Published: "09/23/2022 01:00:00+0200"
slug: "23-bulk-update"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - entityframework
   - bulk
   - update
   - delete

---

## Daily Knowledge Drop

New functionality is being introduced with Entity Framework 7 which enables the ability to perform `bulk updates and deletes` on the database, without having to first load the relevent records into memory. This results in `better memory usage`, as well as `increased performance`.

---

## Availability

As of the time of this post, this functionality is only available via the EF nightly builds for `Microsoft.EntityFrameworkCore` and `Microsoft.EntityFrameworkCore.SqlServer` available at the [following NuGet feed: https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet7/nuget/v3/index.json](https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet7/nuget/v3/index.json). 

This URL can be added as a package source in the _NuGet Package Manager -> Package Sources_ in Visual Studio to enable access to the nightly build NuGet packages.

---

## Examples

In the sample below, we have a table of 10 000 _Blog_ records - we want to update the _Description_ column value for all records which have an `Id value of 20 or less`.

In both EF6 and EF7 examples, the _OnConfiguring_ method of the DbContext has been updated to enable _sensitive logging_ and _console logging_:

``` csharp
optionsBuilder.EnableSensitiveDataLogging(true);
    optionsBuilder.LogTo((string query) =>
    {
        Console.WriteLine(query);
    }, LogLevel.Information);
```

---

### Prior to EF7

In existing versions of Entity Framework, the relevent records have to be selected into memory (so the change tracker is tracking them), updated, and then the changes saved back to the database:

``` csharp
// get all blogs into memory/change tracker
var updateBlogs = context.Blogs.Where(b => b.Id <= 20).ToList();

// update the value
updateBlogs.ForEach(ub => ub.Description = "EF6 Bulk Update");

// save...multiple updates
context.Blogs.UpdateRange(updateBlogs);
var result = await context.SaveChangesAsync();

Console.WriteLine($"Numbers of records updated: {result}");
```

Executing the update code, results in the following output (abbreviated in places):

``` terminal
 Executed DbCommand (90ms) [Parameters=[@p3='1', @p0='2022-07-12T06:47:24.1400000', 
 @p1='EF6 Bulk Update' (Size = 4000), @p2='DD50AA26-FB55-4985-B5D5-C9D2DE98AFB3' (Size = 4000), 
 @p7='2', @p4='2022-07-12T06:47:24.1400000', @p5='EF6 Bulk Update' (Size = 4000), 
 @p6='4BF98BB3-A4BD-4652-AC29-F6A6EE1BE524' (Size = 4000), @p11='3', @p8='2022-07-12T06:47:24.1400000', 
 @p9='EF6 Bulk Update' (Size = 4000), @p10='3C41C714-1670-4978-85DF-956F14FB8863' (Size = 4000), 
 @p15='4', @p12='2022-07-12T06:47:24.1430000', @p13='EF6 Bulk Update' (Size = 4000), 
 @p14='91929DC9-BE25-4027-9554-1B6EEF108E12' (Size = 4000), @p19='5', @p16='2022-07-12T06:47:24.1430000', 
 @p17='EF6 Bulk Update' (Size = 4000), @p18='28DAF144-2ADB-42F4-9838-64F2DC192480' (Size = 4000), @p23='6' 
 .... 
 ....
SET NOCOUNT ON;
UPDATE [Blog] SET [DateCreated] = @p0, [Description] = @p1, [Title] = @p2
OUTPUT 1
WHERE [Id] = @p3;
UPDATE [Blog] SET [DateCreated] = @p4, [Description] = @p5, [Title] = @p6
OUTPUT 1
WHERE [Id] = @p7;
UPDATE [Blog] SET [DateCreated] = @p8, [Description] = @p9, [Title] = @p10
OUTPUT 1
...
...
Numbers of records updated: 20
```

From the output, we can see that multiple _UPDATE_ statements are executed, `one UPDATE for each record`.

---

### EF7

With EF 7, an `ExecuteUpdateAsync extension method` is now available on `IQueryable`, enabling bulk updates.

The syntax is a little more complex than using the EF6 technique:

``` csharp
var updatedCount = await context
    .Blogs
    // filter records to be updated
    .Where(b => b.Id <= 20)
    // execute the bulk update
    .ExecuteUpdateAsync(record => record
        // specify which column to update
        // as well as the value
        .SetProperty(blog => blog.Description, desc => "EF7 Bulk Update")
    );

Console.WriteLine($"Numbers of records updated: {updatedCount}");
```

Looking at the output when executed:

``` terminal
UPDATE [b]
    SET [b].[Description] = N'EF7 Bulk Update'
FROM [Blog] AS [b]
WHERE [b].[Id] <= 20

Numbers of records updated: 20
```

The generated SQL is `much simpler and more concise`.

It is possible to also _update multiple columns_ at the same time by `chaining SetProperty methods together`:

``` csharp
var updatedCount = await context
    .Blogs
    .Where(b => b.Id <= 20)
    .ExecuteUpdateAsync(record => record
        // update the description and date modified
        .SetProperty(blog => blog.Description, desc => "EF7 Bulk Update")
        .SetProperty(blog => blog.DateModified, date => DateTime.Now)
    );
```

---

## Benchmarks

Benchmarking the two samples above using BenchmarkDotnet::

|    Method |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 |  Gen 1 | Allocated |
|---------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|
| EF6Update | 2,222.8 us | 126.60 us | 367.29 us |  1.00 |    0.00 |       - |      - |    302 KB |
| EF7Update |   428.9 us |   5.23 us |   5.81 us |  0.19 |    0.03 | 12.6953 | 0.9766 |     79 KB |

From the results, we can see that the EF7 methods:
- is `5 times faster than EF6`
- uses almost `4 times less memory`

----

## Bulk Delete

For completeness, a `bulk delete method` is also available as part of EF7. It can be leveraged as follows:

``` csharp
var deletedCount = await context
    .Blogs
    .Where(b => b.Id <= 20)
    .ExecuteDeleteAsync();
```

This will result in SQL similar to that of the bulk update - a _single bulk delete statement_ instead of multiple, one per record.

---

## Notes

A very welcome addition to the Entity Framework libraries, greatly speeding up specific uses cases. A specific use case I'm personally interested in, is _extracting data for publishing, and then flagging the data as "published"_. Currently this involves having to load the data into memory, so the change tracker is tracking it, updating the records, and then having EF update each record as "published" individually. 

With EF7, the data can be retrieved without tracking, thus saving on memory usage, and then updated as "published" in bulk.

---

## References

[New in Entity Framework 7: Bulk Operations with ExecuteDelete and ExecuteUpdate](https://timdeschryver.dev/blog/new-in-entity-framework-7-bulk-operations-with-executedelete-and-executeupdate#number-of-rows-affected)   

<?# DailyDrop ?>167: 23-09-2022<?#/ DailyDrop ?>
