---
title: "SQL COALESCE uses"
lead: "Having a look at a few uses of SQL COALESCE"
Published: "10/28/2022 01:00:00+0200"
slug: "28-sql-coalesce"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - sql
   - coalesce

---

## Daily Knowledge Drop

The SQL COALESCE method will `return the first non-null expressing among its arguments`. On the surface this is a relatively simple, but useful feature, however it can be leveraged to also perform more complex operations.

---

## First non-NULL

The first use of `COALESCE` is straight-forward, and aligns with official definition - it _can be used to find the first non-null in a list of values_:

``` sql
-- declare variables
DECLARE @one int, @two int, @three int, @firstNonNull int
-- set all to NULL except one
SELECT @one = null, @two = null, @three = 3

-- nulls are ignored
SELECT @firstNonNull = COALESCE(@one, @two, @three)
PRINT @firstNonNull
```

The output of the above is `3`, the first _non-null_.

---

## Data pivot

Another useful, but maybe not obvious (based on the definition) use of `COALESCE` is to pivot data - that is _convert rows of data into a single row and column_:

In this example we have a table of _Songs_

``` sql
SELECT * FROM Song
```

|Id|Name|Artist|YearReleased|LengthInSeconds|
|---|---|---|---|--|
|8|Everlong|Foo Fighters|1997|250|
|9|Learn to Fly|Foo Fighters|1999|238|
|10|Monkey Wrench|Foo Fighters|1997|231|
|11|My Hero|Foo Fighters|1998|260|

Executing the following:

``` sql
DECLARE @AllSongs VARCHAR(1000)
SELECT @AllSongs = COALESCE(@AllSongs, '') + [Name] + ';'
FROM Song

PRINT @AllSongs
```

Will result in `Everlong;Learn to Fly;Monkey Wrench;My Hero;` being printed.

The `COALESCE` method will take each _Name_ field, and provided it is not null, append it to the _@AllSongs_ variable.

---

## Execute multiple statements

Another useful use of `COALESCE` is to build up a SQL string to execute multiple statements at once. This extends on the previous example - instead of a table column value being appended to a variable, a built up string is appended.

If we need to _select the data from ALL of the tables in the database_:

``` sql
DECLARE @AllTablesSQL VARCHAR(1000)

-- query sys.tables to get a list of tables in the database
-- build up a SELECT statement based on the table name
SELECT  @AllTablesSQL = COALESCE(@AllTablesSQL, '') + 'SELECT * FROM [' + name + ']' + '; '
from sys.tables

PRINT @AllTablesSQL
```

The output here is something along these lines (will obviously differ based on the tables in the database):
`SELECT * FROM [Auditing]; SELECT * FROM [Song]; SELECT * FROM [AlbumSales]; SELECT * FROM [Order]; `

This can now be manually executed to get all the data from the tables.

---

## Notes

I must admit I have encountered the `COALESCE` method before, but never fully understood what it did or how it worked - but just understanding the basics of its usage will allow for very useful queries to be executed.

---

## References

[The Many Uses of Coalesce in SQL Server](https://www.mssqltips.com/sqlservertip/1521/the-many-uses-of-coalesce-in-sql-server/)  

<?# DailyDrop ?>190: 28-10-2022<?#/ DailyDrop ?>
