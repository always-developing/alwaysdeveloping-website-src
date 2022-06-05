---
title: "Using SQL Server's OFFSET and FETCH"
lead: "Using SQL Server's built in functionality to limit the number of rows returned"
Published: 03/17/2022
slug: "17-sql-offset-fetch"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - tsql
    - offset
    - fetch
    - query

---

## Daily Knowledge Drop

SQL Server has two keywords available, `OFFSET` and `FETCH` which can be used to limit the number of rows returned by a query.

This functionality has been available in SQL Server 2012 and later, and Azure SQL Database. 

---

## Limiting rows returned

The `OFFSET` keyword can be used independently or in conjunction with the `FETCH` keyword, which cannot be used in isolation.

- `OFFSET`: determines how many rows to skip at the start of the dataset
- `FETCH`: determines how many rows to return, after the _OFFSET_ rows have been skipped

Whether using only `OFFSET` or `OFFSET + FETCH`, in both situations, the `ORDER BY` clause is required.

---

## Examples

In the following examples, the table has a simple auto incrementing _int_ Id field which is used for ordering.

### Constant value

``` sql
-- select all ids order from smallest to largest
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC

-- Skip the first 10 rows, and return
-- all other rows
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET 10 ROWS

-- Skip the first 10 rows, and return
-- the next 20 rows only
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET 10 ROWS
FETCH NEXT 20 ROWS ONLY
```

---

### Variable values

Instead of a constant number, the values used for `OFFSET` and `FETCH` can be variable:

``` sql
DECLARE @SkipRows INT = 10,
        @FetchRows INT = 20; 

-- Skip the first 10 rows, and return
-- the next 20 rows only
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET @SkipRows ROWS
FETCH NEXT @FetchRows ROWS ONLY
```

---

### Expression values

Expressions can also be used to calculate the `OFFSET` and `FETCH` values:

``` sql
DECLARE @PageNumber INT = 5,
        @PageSize INT = 20; 

-- In this example, skip 100 records
-- and return the next 20
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET @PageNumber * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY
```

This makes `paging` on the dataset very easy.

---

### Subquery values

Finally, the values can also be retrieved using a subquery

``` sql
-- Skip 10 rows and return 
-- the next X rows, determined
-- but a value in the AppSettings table
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET 10 ROWS
FETCH NEXT (
	SELECT CONVERT(INT, [VALUE]) 
	FROM AppSettings WHERE [Key] = 'PageSize'
	) ROWS ONLY
```

---

## Notes

The `OFFSET + FETCH` combination is a very easy, effective way and simple to add paging to a dataset, and offers a better alterative to having to use _ROW\_NUMBER_ to add paging.

---

## References
[SQL SELECT - ORDER BY Clause](https://docs.microsoft.com/en-us/sql/t-sql/queries/select-order-by-clause-transact-sql?view=sql-server-ver15)  

<?# DailyDrop ?>33: 17-03-2022<?#/ DailyDrop ?>
