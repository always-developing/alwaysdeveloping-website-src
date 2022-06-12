---
title: "Keyset Pagination (vs Offset pagination)"
lead: "Exploring keyset pagination in SQL Server"
Published: "07/01/2022 01:00:00+0200"
slug: "01-keyset-pagination"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - pagination
    - keyset
    - sort

---

## Daily Knowledge Drop

In a previous daily drop post, I learn about using [OFFSET and FETCH](../../03/17-sql-offset-fetch/) to perform paging when retrieving data - however this is `not always a good method` for retrieving results, especially with a large dataset.

Instead, the usage of `keyset pagination` should be considered, as it is more performant. `Keyset pagination` returns a subset of the dataset by using a `WHERE` clause instead of the `OFFSET` clause as with _offset pagination_.

There are however some limitations with the `keyset` approach, which will be explored below.

---

## Offset

### Recap

First, a brief recap on `offset pagination` - this method uses the `OFFSET` and `FETCH` clauses to effectively skip _X_ rows, and FETCH the following _Y_ rows, ordered by the specified column(s):

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

Based on _PageNumber_ and _PageSize_, the appropriate number of rows and skipped, and then _PageSize_ rows retrieved.

This approach works, however might not be suitable for all use cases.

### Issues

- **Large datasets**

    One issue with the `offset pagination` approach, is that when the `OFFSET` clauses is used, the database is still required to go through all rows to find the correct starting record.

    For example, if we have a database with 1 000 000 rows.

    When the following is executed, the database still needs to go through the first 100 rows (the OFFSET value) to find the 101st row.

    ``` sql
    SELECT Id 
    FROM OffSetDemo
    ORDER BY Id ASC
    OFFSET 100 ROWS
    FETCH NEXT 50 ROWS ONLY
    ```

    This will probably not have a negative impact on performance, however if the following is executed:

    ``` sql
    SELECT Id 
    FROM OffSetDemo
    ORDER BY Id ASC
    OFFSET 900 000 ROWS
    FETCH NEXT 50 ROWS ONLY
    ```

    Now the database needs to go through 900 000 rows (the OFFSET value) to find the 900 001st row, which could definitely have a performance impact.  
    <br>


- **Missing/duplicate records**

    If the dataset being used is changing while being queried, the `offset` method could result in records either being missed, or records being duplicated as paging occurs.  

    With a page size of 20, when the first page is queried, the first 20 records are returned. When the second page is being queried for, the first 20 rows are skipped (with `OFFSET`) and the next 20 rows are returned. Consider however, if between loading the first and second page, a record in the first 20 is deleted. The record which was number 21 when the first page loaded, is record 20 when the second page is loaded - this is missed when loading page 1 and page 2.

    The same applied if a record is added which falls into the first 20 - the record which previously was number 20, would become record number 21, and thus would appear on page 1 and page 2.


`Keyset pagination` solves aims to solve these problems (while introducing a different set of limitations).

## Keyset

With `keyset` pagination, instead of using the `OFFSET` clause to skip rows to determine where the returned dataset should start, a `WHERE` clause is used to determine where the returned dataset should start.

A few examples will make it clearer - again consider a database with 1 000 000 rows, with an int _Id_ primary key column on which the data is sorted.

To retrieve the first page:

``` sql
-- OFFSET pagination
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET 0 ROWS
FETCH NEXT 50 ROWS ONLY

-- KEYSET pagination
SELECT TOP 50 Id 
FROM OffSetDemo
WHERE Id > 0
ORDER BY Id ASC
```

To retrieve the 10th page (for example):

``` sql
-- OFFSET pagination
SELECT Id 
FROM OffSetDemo
ORDER BY Id ASC
OFFSET 500 ROWS -- 50 x 10
FETCH NEXT 50 ROWS ONLY

-- KEYSET pagination
SELECT TOP 50 Id 
FROM OffSetDemo
WHERE Id > 500
ORDER BY Id ASC
```

In terms of performance, the keyset version is a lot more performant - assuming there are indexes created on the `WHERE` clause columns. Using the WHERE clause and the index, SQL does not need to go through _X_ number rows to find the correct starting point

This also solves the problem of records being duplicated or being missed as absolute values are being used to do the lookup and not offsets. If a record in the first 20 is deleted, it will not effect the second page of 20 records, and the same applied if a new record is added to the first 20 - the second page of 20 records is unaffected.

`Keyset pagination` is not without its own limitations though.

### Limitations

- `No random access` - with _keyset pagination_ one is unable to get, for example, the 15th page. To get a page of data, the last _Id_ (in the above example) of the previous page is required.

- `Complicated multi-column filtering` - with the above examples, the `WHERE` clause is straightforward, but this can get complicated. Consider if the data is going to be sorted by a _CreatedDate_ column, and if two record have the same _CreatedDate_, then they should be sorted by _Id_.

    ``` sql
    SELECT TOP 50 DateCreated, Id 
    FROM OffSetDemo
    WHERE ((DateCreated > '2022/06/12') OR (DateCreated = '2022/06/12' AND Id > 500))
    ORDER BY DateCreated, Id ASC
    ```

    Depending on how many filter conditions there are, the `WHERE` clause could become complex.

## Notes

The choice between the two methods (and any other), as always, comes down to the specific use case. 
- Need random access to any page? Use offset
- Large dataset, and no random access required? Use keyset
- Large dataset, but do require random access? Why not both? - offset for the random access, and keyset for the _next/previous_ functionality

---

## References

[.NET Data Community Standup - Database Pagination](https://www.youtube.com/watch?v=DIKH-q-gJNU)  

<?# DailyDrop ?>108: 01-07-2022<?#/ DailyDrop ?>
