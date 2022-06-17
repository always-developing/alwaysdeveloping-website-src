---
title: "T-SQL enhancements in SQL Server 2022"
lead: "Exploring some of the more useful T-SQL enhancement coming with SQL Server 2022"
Published: "07/06/2022 01:00:00+0200"
slug: "06-sql-2022-enhancements"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - tsql

---

## Daily Knowledge Drop

There are a number of very useful T-SQL enhancements coming with SQL Server 2022 (currently in preview). Some of these include:
- `STRING_SPLIT` - splits a string into rows of substrings, based on a specified separator character, now with **ordinal support**
- `GREATEST/LEAST` - Similar to _MAX/MIN_, but operates across columns instead of across rows
- `GENERATE_SERIES` - Produces a set-based sequence of numeric values
- `JSON functions` - Including functions to test whether a string is valid JSON, construct a JSON object and construct a JSON array
- `DATE_BUCKET` - Returns the date-time value corresponding to the start of each date-time bucket from the timestamp defined

Below we'll explore some of the enhancements I will find most useful (based on my previous experiences and requirements), while additional details on the other enhancements can be found in the references below.

---

## STRING_SPLIT

The `SPLIT_STRING` function, first introduced with SQL Server 2016, allows for the splitting a string into rows of substrings - especially useful for developers making extensive use of stored procedures (for example, with SQL Server Reporting Services)

For example, executing the following SQL:
``` sql
SELECT * from STRING_SPLIT('one,two,three,four,five', ',')
```

Results in the following output:

``` sql
value
-----------------------
one
two
three
four
five
```

<br>

Coming in SQL Server 2022, is the ability to _determine the nth item of the list_.

For example, executing the following SQL:
``` sql
SELECT * from STRING_SPLIT('one,two,three,four,five', ',' , 1) WHERE ordinal = 3
```

Results in the following output:

``` sql
value                   ordinal
----------------------- --------------------
three                   3
 ```

An additional parameter, _enable\_ordinal_ is available which will return an _ordinal_ column - which can then be used as part of a WHERE clause.

One limitation of the `SPLIT_STRING` remains, and that is that the delimiter can only be a single character, and not a string.

---

## GREATEST/LEAST

The new `GREATEST and LEAST` functions, are similar to the existing  _MAX and MIN_ functions, but operates across columns instead of across rows.

Consider the following _VisitorsPerMonth_ table which stores the number of unique visitors to a website, per month, per year.

``` sql
SELECT * FROM VisitorsPerMonth

Year        Jan         Feb         March
----------- ----------- ----------- -----------
2020        435         643         763
2021        893         1121        1327
2022        1923        2107        2782

```

We can use _Max_ to determine _maximum number of visitors in any January_ (a max across rows):

``` sql
SELECT MAX(Jan)
FROM VisitorsPerMonth
```

However, what if we wanted the _max number of visitors in any particular month in a year_ - this requires an operation across columns, which is where the `GREATEST` function comes into play.

A number of _columns_ are supplied to the function, and the greatest value will be returned:

``` sql
SELECT Year, GREATEST(Jan, Feb, March) as HighestVisitors
FROM VisitorsPerMonth
```

This results in the following output:

``` sql
Year        HighestVisitors
----------- ---------------
2020        763
2021        1327
2022        2782
```

We now know how many visitors the site had in its most popular month in each year.

---

## GENERATE_SERIES

This new function produces a set-based sequence of numeric values based on specified _START_, _STOP_ and _STEP_ values.

The usage is fairly straightforward:

``` sql
SELECT value FROM GENERATE_SERIES(START = 1, STOP = 3);
SELECT value FROM GENERATE_SERIES(START = 0, STOP = 25, STEP = 5);
```

Which results in the output:

``` sql
value
-----------
1
2
3

value
-----------
0
5
10
15
20
25
```

If no _STEP_ value is specified, a value of **1** is used.

---

## Notes

As a C# developer, in recent years, my focus has been more on using Entity Framework Core and less on raw SQL usage - however, knowledge of these enhancement will come in useful when the need arises to do raw SQL (a custom query through EF, or Dapper, or a report stored procedure, for example).

---

## References

[My Favorite T-SQL Enhancements in SQL Server 2022](https://www.mssqltips.com/sqlservertip/7265/sql-server-2022-t-sql-enhancements/)   
[DATE_BUCKET (Transact-SQL)](https://docs.microsoft.com/en-us/sql/t-sql/functions/date-bucket-transact-sql?view=sql-server-ver16)  
[JSON_OBJECT (Transact-SQL)](https://docs.microsoft.com/en-us/sql/t-sql/functions/json-object-transact-sql?view=sql-server-ver16)  

<?# DailyDrop ?>111: 06-07-2022<?#/ DailyDrop ?>
