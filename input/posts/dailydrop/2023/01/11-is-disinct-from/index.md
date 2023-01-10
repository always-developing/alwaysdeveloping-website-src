---
title: "SQL: IS DISTINCT FROM"
lead: "Comparing two expressions with NULL support in SQL"
Published: "01/10/2023 01:00:00+0200"
slug: "11-is-distinct-from"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - sql
   - sql2022
   - null
   - distinct

---

## Daily Knowledge Drop

SQL Server 2022 introduced a new predicate `IS [NOT] DISTINCT FROM`, which allows for two expressions to be compared, but takes `NULL` values into account.


---

## Data setup

Consider a _Blog_ table, with a `nullable DateCreated column` - this column _could_ have a valid _DateTime value_, but could also contain NULL values.

---

### Pre SQL 2022

Prior to SQL 2022, when comparing two values, `NULL` values were, by default, not taken into account.

Assume the _Blog_ table `DOES` contain rows which have _NULL_ DateCreated values:

``` sql
DECLARE @createdDate datetime

SELECT @createdDate = NULL

SELECT *
FROM Blog
WHERE DateCreated = @createdDate
```

**0 rows returned**

Even though `@createdDate` is _NULL_, and there are rows in the table with _NULL_, `no rows are returned` - _NULL_ values are not taken into account.

To successfully return rows which do contain _NULL_ values, both expressions need to be converted to the same default value in case of a _NULL_ value:

``` sql
DECLARE @createdDate datetime

SELECT @createdDate = NULL

SELECT *
FROM Blog
WHERE ISNULL(DateCreated, '1900-01-01') = 
	ISNULL(@createdDate, '1900-01-01')
```

**10 rows returned**

With this technique, _NULL_ values on either expression is converted to `'1900-01-01'`, which is then successfully compared.

With SQL Server 2022, performing this comparison becomes much easier.

---

### SQL 2022

SQL Server 2022 introduces the new `IS [NOT] DISTINCT FROM` predicate - this allows the comparison of values while taking _NULL_ values into account.

``` sql
DECLARE @createdDate datetime

SELECT @createdDate = NULL

SELECT * 
FROM Blog
WHERE DateCreated IS NOT DISTINCT FROM @createdDate
```

**10 rows returned**

The `IS [NOT] DISTINCT FROM` predicate _compares the equality of two expressions and guarantees a true or false result, even if one or both operands are NULL_

---

## Notes

A relatively minor update, but for a developer which writes a good amount SQL this small update definitely does make things simpler and easier.

---

## References

[Cool Stuff in SQL Server 2022 – IS DISTINCT FROM](https://www.sqlservercentral.com/blogs/cool-stuff-in-sql-server-2022-is-distinct-from)  

<?# DailyDrop ?>232: 11-01-2023<?#/ DailyDrop ?>
