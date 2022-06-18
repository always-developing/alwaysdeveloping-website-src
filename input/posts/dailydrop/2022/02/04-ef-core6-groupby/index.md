---
title: "EF Core 6: GroupBy enhancements"
lead: "Improvements made to the Entity Framework Core 6 GroupBy functionality"
Published: 02/04/2022
slug: "04-ef-core6-groupby"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - efcore
    - efcore6
    - ef
    - entityframeworkcore
    - groupby
    - .net6
---

## Daily Knowledge Drop

The `GroupBy` support in Entity Framework Core 6 got an update, and has made things a lot easier.  
In short, the following is now supported:

- Translate GroupBy followed by FirstOrDefault (or similar) over a group
- Supports selecting the top N results from a group
- Expands navigation's after the GroupBy operator has been applied


## Examples

### Setup

In all the example below the setup is very simple - a single `Song` entity and corresponding database table. All sample below were tested with SQL Server.

``` csharp
public class Song
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Artist { get; set; }
    public int YearReleased { get; set; }
    public int LengthInSeconds { get; set; }
    public override string ToString()
    {
        return $"Song `{Name}` by '{Artist} released " +
                $"in '{YearReleased}' and is '{LengthInSeconds}' seconds long";
    }
}
```

The results are based on data in the example dataset, and not necessarily reflective of real world values.

The below are examples in Entity Framework Core 3.1, where possible, and Entity Framework Core 6 (both are LTS versions).

---

### Count GroupBy 

`Retrieve the number of songs per artist.`

#### EFCore 3.1

Code:

``` csharp
var query = db.Songs
    .GroupBy(s => s.Artist)
    .Select(e => new { e.Key, Count = e.Count() });

foreach (var item in query.ToList())
{
    Console.WriteLine(item);
}
```

SQL generated:

``` sql
SELECT [s].[Artist] AS [Key], COUNT(*) AS [Count]
FROM [Song] AS [s]
GROUP BY [s].[Artist]
```

Output:

``` powershell
{ Key = Foo Fighters, Count = 4 }
{ Key = John Mayer, Count = 3 }
```


#### EFCore 6

Code and resulting SQL generated are exactly the same.

---

### Top 1 GroupBy

`Retrieve an artist and the first year a song of theirs was released`

#### EFCore 3.1

Code:

``` csharp
var query = db.Songs
    .Select(s => s.Artist).Distinct()
    .Select(e => new
    {
        Artist = e,
        Year = db.Songs
            .Where(s => s.Artist == e)
            .OrderBy(s => s.YearReleased)
            .Select(y => y.YearReleased)
            .FirstOrDefault()
    });

foreach (var item in query.ToList())
{
    Console.WriteLine(item);
}
```

SQL generated:

``` sql
SELECT [t].[Artist], (
    SELECT TOP(1) [s].[YearReleased]
    FROM [Song] AS [s]
    WHERE ([s].[Artist] = [t].[Artist]) 
        OR ([s].[Artist] IS NULL AND [t].[Artist] IS NULL)
    ORDER BY [s].[YearReleased]) AS [Year]
FROM (
    SELECT DISTINCT [s0].[Artist]
    FROM [Song] AS [s0]
) AS [t]
```

Output:

``` powershell
{ Artist = Foo Fighters, Year = 1997 }
{ Artist = John Mayer, Year = 2003 }
```


#### EFCore 6

Code:

``` csharp
var query = db.Songs
    .GroupBy(a => a.Artist)
    .Select(g => g.OrderBy(a => a.YearReleased)
                .Select(s => new { Artist = s.Artist, Year = s.YearReleased })
                .FirstOrDefault()
        );

foreach (var item in query.ToList())
{
    Console.WriteLine(item);
}
```

SQL generated:

``` sql
SELECT [t0].[Artist], [t0].[Year], [t0].[c]
FROM (
    SELECT [s].[Artist]
    FROM [Song] AS [s]
    GROUP BY [s].[Artist]
) AS [t]
LEFT JOIN (
    SELECT [t1].[Artist], [t1].[Year], [t1].[c]
    FROM (
        SELECT [s0].[Artist], [s0].[YearReleased] AS [Year], 1 AS [c], 
            ROW_NUMBER() OVER(PARTITION BY [s0].[Artist] 
            ORDER BY [s0].[YearReleased]) AS [row]
        FROM [Song] AS [s0]
    ) AS [t1]
    WHERE [t1].[row] <= 1
) AS [t0] ON [t].[Artist] = [t0].[Artist]
```

Output:

``` powershell
{ Artist = Foo Fighters, Year = 1997 }
{ Artist = John Mayer, Year = 2003 }
```


The LINQ is more intuitive and concise, while the SQL appears to be more complex. More complex SQL does not necessarily mean less performant though - as with everything it should be benchmarked with your expected data volume.

---

### Top N GroupBy

`Retrieve an artist and the first N years a song of theirs was released`

#### EFCore 3.1

It was a struggle to even get an example working for EF Core 3.1 for this scenario. It could be possible, but the fact it was so difficult to even try get it to work directly speaks to the need for the enhancements made in EF Core 6.

#### EFCore 6

Code:

``` csharp
var query = db.Songs
    .GroupBy(a => a.Artist)
    .Select(g => new
    {
        Artist = g.Key,
        Years = g.OrderBy(a => a.YearReleased)
                .Distinct()
                .Take(2)
                .Select(s => s.YearReleased)
    });

foreach (var item in query.ToList())
{
    Console.WriteLine($"Artist = {item.Artist}, Years = {string.Join(',', item.Years)}");
}
```

SQL generated:

``` sql
SELECT [t].[Artist], [t0].[YearReleased], [t0].[Id]
FROM (
    SELECT [s].[Artist]
    FROM [Song] AS [s]
    GROUP BY [s].[Artist]
) AS [t]
OUTER APPLY (
    SELECT DISTINCT TOP(2) [s0].[Id], [s0].[Artist], 
        [s0].[LengthInSeconds], [s0].[Name], [s0].[YearReleased]
    FROM [Song] AS [s0]
    WHERE [t].[Artist] = [s0].[Artist]
) AS [t0]
ORDER BY [t].[Artist]
```

Output:

``` powershell
Artist = Foo Fighters, Years = 1997,1999
Artist = John Mayer, Years = 2003,2003
```

---

## Notes

The above are just a few very basic examples of the enhanced functionality. The official Microsoft EF Core 6 documentation referenced below has many more examples - however hopefully this has at least introduced the enhanced GroupBy support, and how EF Core 6 makes it easier to use.

As always, the recommendation is to benchmark the various LINQ techniques (and corresponding SQL) against your data structure and volumes, and make an informed decision about the best way to structure the LINQ.

---

## References
[Whats new in EF Crore 6 - improved GroupBy support](https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/whatsnew#improved-groupby-support)


<?# DailyDrop ?>04: 04-02-2022<?#/ DailyDrop ?>