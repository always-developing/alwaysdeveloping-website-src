---
title: "EF Core INSERT vs MERGE"
lead: "Entity Framework Core will use MERGE instead of INSERT under certain conditions"
Published: "06/02/2022 01:00:00+0200"
slug: "02-ef-bulk-insert"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - efcore
    - bulk
    - insert

---

## Daily Knowledge Drop

When inserting data into a table using Entity Framework Core (EF Core), by default, if three or less records are being inserted, `separate SQL INSERT` statements will be used.  
However if four or more records are being inserted, EF Core will batch up the records and perform a `single SQL MERGE` statement.

---

## Setup

In all of the below samples, the following methods were used.

A method to retrieve a list of _Songs_ some of which will be used to insert into the database:

``` csharp
public Song[] GetSongs()
{
    return new Song[]
    {
        new Song
        {
            Name = "Everlong",
            Artist = "Foo Fighters",
            YearReleased = 1997,
            LengthInSeconds = 250
        },
        new Song
        {
            Name = "Learn to Fly",
            Artist = "Foo Fighters",
            YearReleased = 1999,
            LengthInSeconds = 238
        },
        new Song
        {
            Name = "Monkey Wrench",
            Artist = "Foo Fighters",
            YearReleased = 1997,
            LengthInSeconds = 231
        },
        new Song
        {
            Name = "My Hero",
            Artist = "Foo Fighters",
            YearReleased = 1998,
            LengthInSeconds = 260
        },
        new Song
        {
            Name = "Clarity",
            Artist = "John Mayer",
            YearReleased = 2003,
            LengthInSeconds = 268
        },
        new Song
        {
            Name = "Daughters",
            Artist = "John Mayer",
            YearReleased = 2003,
            LengthInSeconds = 238
        },
        new Song
        {
            Name = "Bigger than my Body",
            Artist = "John Mayer",
            YearReleased = 2003,
            LengthInSeconds = 266
        }
    };
}
```

A minimal endpoint which will insert a variable number of records into the database, based on the _count_ parameter passed to the endpoint:

``` csharp
app.MapGet("/insert/{count}", async (int count) =>
{
    using (var db = new BulkContext())
    {
        var songs = GetSongs();

        // add 1 or many songs
        for(int i = 0; i < count; i++)
        {
            db.Add(songs[i]);
        }

        // only call save changes once
        await db.SaveChangesAsync();
    }
});
```

Apart from this, the setup of the _BulkContext_ and the _Song_ entity is standard EF Core setup, nothing custom or outside the typical setup.

What's important here though, is that one or more entities are added to the db context, but the `SaveChangesAsync method is only called once`, after all entities have been added.

---

### Insert statement

Calling the `/insert/{count}` endpoint with 1, 2, or 3 as _count_ value, results in the following SQL executed `1, 2 or 3 separate times`:

``` sql
SET NOCOUNT ON;
INSERT INTO [Song] ([Artist], [LengthInSeconds], [Name], [YearReleased])
VALUES (@p0, @p1, @p2, @p3);
SELECT [Id]
FROM [Song]
WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();
```

EF Core generates SQL which will insert the relevant information, and then return the Id primary key generated for the record. 

If full logging is enabled, EF Core will even tell you what it's doing: _Executing update commands individually as the number of batchable commands (3) is smaller than the minimum batch size (4)._

---

### Merge statement

Calling the `/insert/{count}` endpoint with 4 or greater as a value for _count_ results in the following SQL executed `just once` (the below example inserts 5 records):

``` sql
SET NOCOUNT ON;
DECLARE @inserted0 TABLE ([Id] int, [_Position] [int]);
MERGE [Song] USING (
VALUES (@p0, @p1, @p2, @p3, 0),
(@p4, @p5, @p6, @p7, 1),
(@p8, @p9, @p10, @p11, 2),
(@p12, @p13, @p14, @p15, 3),
(@p16, @p17, @p18, @p19, 4)) AS i ([Artist], [LengthInSeconds], 
        [Name], [YearReleased], _Position) ON 1=0
WHEN NOT MATCHED THEN
INSERT ([Artist], [LengthInSeconds], [Name], [YearReleased])
VALUES (i.[Artist], i.[LengthInSeconds], i.[Name], i.[YearReleased])
OUTPUT INSERTED.[Id], i._Position
INTO @inserted0;

SELECT [i].[Id] FROM @inserted0 i
ORDER BY [i].[_Position];

```

As one can see, EF Core has changed the strategy from multiple INSERT statements to a single MERGE statement.

Again, with full logging turned on EF Core will inform you what it's doing: _Executing 5 update commands as a batch._

---

### Custom batch size

The default for when EF Core switches from multiple INSERTS to a MERGE statement is 4 records - this however can be overwritten.

When configuring the connection, the min amd max batch size can be explicitly set:

``` csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        @"Server=.\SQLEXPRESS;Database=BulkInsert;Integrated Security=True", options =>
        {
            options.MaxBatchSize(1);
        });
}
```

In this example, the max batch size is to 1, which means a single INSERT will be used for every statement, with no batching taking place at all (generally, this would *not* be advisable).

Calling the `/insert/{count}` endpoint with 4 or greater will now result in multiple INSERTS and _not_ a MERGE statement.

---

## Notes

In most cases, this is not something to worry about - the presumption being that the EF Core team have benchmarked and determined in _most_ cases, 4 is the threshold where doing a MERGE is more efficient that doing multiple INSERTS.  

However, if one determines that 4 is too high (or too low) for a specific use case, one can adjust the value up or down and benchmark how the performance of the code is impacted.

<?# DailyDrop ?>87: 02-06-2022<?#/ DailyDrop ?>
