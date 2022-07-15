---
title: "An intro to SQL SELECT-OVER"
lead: "How the SQL SELECT-OVER clause can be used to simplify t-sql statements"
Published: "08/10/2022 01:00:00+0200"
slug: "10-sql-over"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - sqlserver
    - select
    - over

---

## Daily Knowledge Drop

The SQL `OVER` clause can be used to define a window or user-specified set of rows within a result set. This effectively `allows computed values` (AVG, MAX in the below examples) to be `executed over different sets of data`, in the `same query`.

Some examples below should make this clearer.

---

## Audit table

In our examples below, we are using an `Audit` table which stored the average execution time for a process on a server for a specific day. The table will store the server name, as well as the average execution time (in milliseconds):

``` sql
SELECT * FROM Audit
```

A sample output of the data for a few servers:

``` terminal
Id          Created                 Server          ExecutionInMs
----------- ----------------------- --------------- -------------
230001      2022-07-15 06:38:54.710 Server1         2420
230002      2022-07-15 06:38:54.710 Server2         4176
230003      2022-07-15 06:38:54.710 Server3         6291
230004      2022-07-15 06:38:54.710 Server4         2508
230005      2022-07-15 06:38:54.710 Server5         6690
```

### Use case

In our use case, we want to see the `average execution` time for each server along with the `average across all servers`, to determine if any specific server(s) are executing excessively above the average. Also to include is `MIN and MAX execution time across all servers`.

In our example we only have 100 server, so one could potentially just manually look at the data to see the `MIN and MAX` - but imagine the same scenario at scale with hundreds or thousands of server. Not as easy to do manually.

## Without OVER

### Averages

First step, is to get the `average across all servers` and the following is a method to do it - using a sub query to perform the `average across all data`:

``` sql
SELECT Server, ExecutionInMs,
	(
		SELECT AVG(ExecutionInMs) FROM Audit
	) as TotalAverageExecutionTime
FROM Audit
```

Another way could be to _SELECT_ the value into a variable and then include that in the final _SELECT_:

``` sql
DECLARE @TotalAverageExecutionTime int
SELECT @TotalAverageExecutionTime = AVG(ExecutionInMs) FROM Audit

SELECT Server, ExecutionInMs, @TotalAverageExecutionTime as TotalAverageExecutionTime
FROM Audit
```

Both techniques return the same result set:

``` terminal
Server           ExecutionInMs TotalAverageExecutionTime
---------------- ------------- -------------------------
Server1          2420          4551
Server2          4176          4551
Server3          6291          4551
Server4          2508          4551
Server5          6690          4551
```

This approach is required as we are trying to combine operations `across different datasets` - the dataset of a set of rows and the dataset of all the information combined (the `AVG`).

---

### MIN and MAX

The same problem occurs with using `MIN and MAX`. Here is a sample of what the query would look like using `sub-queries` to get all the information required:

``` sql
SELECT 
	Server, 
        ExecutionInMs,
	(
		SELECT AVG(ExecutionInMs) FROM Audit
	) as TotalAverageExecutionTime,
	(
		SELECT MIN(ExecutionInMs) FROM Audit
	) as TotalMinExecutionTime,
	(
		SELECT Max(ExecutionInMs) FROM Audit
	) as TotalMaxExecutionTime
FROM Audit
```

With all of this, we have the information we require:

``` terminal
Server          ExecutionInMs TotalAverageExecutionTime TotalMinExecutionTime TotalMaxExecutionTime
--------------- ------------- ------------------------- --------------------- ---------------------
Server1         2420          4551                      2027                  10600
Server2         4176          4551                      2027                  10600
Server3         6291          4551                      2027                  10600
Server4         2508          4551                      2027                  10600
Server5         6690          4551                      2027                  10600
```

While this is all valid, but there is a easier and simpler way, using the `OVER clause`.

---

## With OVER

### Averages

As mentioned in the intro - the `OVER` clause can be used to _define a window or user-specified set of rows within a result set_.

We can select the base table information, and then use the `OVER` clause to define a separate result set to perform the `AVG` on, `in the same query`:

``` sql
SELECT	
    Server, 
    ExecutionInMs, 
    AVG(ExecutionInMs) OVER()
FROM Audit
```

The additional of `OVER()` with no parameters passed in, means: `over the entire dataset`. So in the query the _Server_ and _ExecutionInMs_ is returned for each row in the base data, while the `AVG is executed over the entire dataset of all rows`.

Definitely cleaner, easier and more concise than the other methods shown above without `OVER`.

---

### MIN and MAX

The same solution can be used for `MIN and MAX`:

``` sql
SELECT	
    Server, 
    ExecutionInMs, 
    AVG(ExecutionInMs) OVER() as TotalAverageExecutionTime,
    MIN(ExecutionInMs) OVER() as TotalMinExecutionTime,
    MAX(ExecutionInMs) OVER() as TotalMaxExecutionTime
FROM Audit
```

We now have our final required results:

``` terminal
Server           ExecutionInMs TotalAverageExecutionTime TotalMinExecutionTime TotalMaxExecutionTime
---------------- ------------- ------------------------- --------------------- ---------------------
Server1          2420          4551                      2027                  10600
Server2          4176          4551                      2027                  10600
Server3          6291          4551                      2027                  10600
Server4          2508          4551                      2027                  10600
Server5          6690          4551                      2027                  10600
```

---

## Calculations

Calculations can also be performed on the data output by using `OVER`:

In this example, the percentage above the total average for each server is calculated, and then each server is ranked by how far they are above the average:

``` sql
SELECT	
    Server, 
    ExecutionInMs, 
    AVG(ExecutionInMs) OVER() as TotalAverageExecutionTime,
    MIN(ExecutionInMs) OVER() as TotalMinExecutionTime,
    MAX(ExecutionInMs) OVER() as TotalMaxExecutionTime,
    (CONVERT(decimal, ExecutionInMs) / CONVERT(decimal, AVG(ExecutionInMs) OVER())) * 100
FROM Audit
ORDER BY 6 DESC
```

From the results, we can see that `Server10` is executing over `two times slower than the average`!

``` terminal
Server          ExecutionInMs TotalAverageExecutionTime TotalMinExecutionTime TotalMaxExecutionTime 
--------------- ------------- ------------------------- --------------------- --------------------- ---------------------
Server10        10600         4551                      2027                  10600                 232.9158426719402329
Server50        7000          4551                      2027                  10600                 153.8123489343001538
Server77        6950          4551                      2027                  10600                 152.7136892990551527
Server90        6941          4551                      2027                  10600                 152.5159305647110525
Server56        6899          4551                      2027                  10600                 151.5930564711052516
```

---

### Partition

The `PARTITION` keyword can also be used in conjunction with the `OVER` to create multiple smaller datasets _partitioned_ by the specified field.

For the below example, suppose a requirement has come into also include the average for each _second_ - that is, the `average for all server's where the execution time is between 4000-4999ms`, the average where the execution time is `between 5000-5999ms etc`. Maybe there is a threshold above which the execution jumps drastically?

First, let's include a clause to get the first digit of the _ExecutionTimeInMs_:

``` sql
SELECT	
    Server, 
    ExecutionInMs,
    SUBSTRING(CAST(ExecutionInMs as varchar), 1, 1) as FirstDigit,
    AVG(ExecutionInMs) OVER() as TotalAverageExecutionTime,
    MIN(ExecutionInMs) OVER() as TotalMinExecutionTime,
    MAX(ExecutionInMs) OVER() as TotalMaxExecutionTime,
    (CONVERT(decimal, ExecutionInMs) / CONVERT(decimal, AVG(ExecutionInMs) OVER())) * 100
FROM Audit
ORDER BY 6 DESC
```

From there, we can now use this field to create multiple datasets on which the `AVG` is `PARTITIONED` by:

``` sql
SELECT	
    Server, 
    ExecutionInMs,
    SUBSTRING(CAST(ExecutionInMs as varchar), 1, 1) as FirstDigit,
    AVG(ExecutionInMs) OVER(PARTITION BY SUBSTRING(CAST(ExecutionInMs as varchar), 1, 1)) as SecondAverageInMs,
    AVG(ExecutionInMs) OVER() as TotalAverageExecutionTime,
    MIN(ExecutionInMs) OVER() as TotalMinExecutionTime,
    MAX(ExecutionInMs) OVER() as TotalMaxExecutionTime,
    (CONVERT(decimal, ExecutionInMs) / CONVERT(decimal, AVG(ExecutionInMs) OVER())) * 100
FROM Audit
ORDER BY 6 DESC
```

A sample of the results:

``` terminal
Server          ExecutionInMs FirstDigit SecondAverageInMs TotalAverageExecutionTime TotalMinExecutionTime TotalMaxExecutionTime 
--------------- ------------- ---------- ----------------- ------------------------- --------------------- --------------------- --------------------
Server91        2813          2          2508              4551                      2027                  10600                 61.8105910788837618
Server92        2295          2          2508              4551                      2027                  10600                 50.4284772577455504
Server95        2159          2          2508              4551                      2027                  10600                 47.4401230498791474
Server96        2566          2          2508              4551                      2027                  10600                 56.3832124807734564
Server93        3818          3          3538              4551                      2027                  10600                 83.8936497473082839
Server100       3127          3          3538              4551                      2027                  10600                 68.7101735882223687
Server70        3348          3          3538              4551                      2027                  10600                 73.5662491760052736

```

From the result, we now have:
- The `average` across a specific `partition` - records which start with the same digit for _ExecutionInMs_ will have the same average
- The `average` across the entire dataset - all records will have the same average

---

## Row Numbering

A use case which often comes up (and which I've used, without fully understanding the inner workings) - and that is to `number each row` and specifically `number each row over a group/partition`.

The `ROW_NUMBER` function is used with the `OVER` and `ORDER BY` clauses to give each row a number, based on the `ORDER BY`.

Here, each row is given a number, ordered by the _ExecutionInMs_ from largest to smallest:

``` sql
SELECT ROW_NUMBER() OVER(ORDER BY ExecutionInMs DESC), Server, ExecutionInMs
FROM Audit
```

A sample of the output:

``` terminal
                     Server           ExecutionInMs
-------------------  ---------------- -------------
1                    Server10         10600
2                    Server50         7000
3                    Server77         6950
4                    Server90         6941
5                    Server56         6899
```

This can also be used with the `PARTITION` clause to generate row numbers within a specific sub-dataset.

---

## Notes

If working with an ORM (such as Entity Framework), this may not be something for every day use. However, investigations and troubleshooting will still need to be performed on the data, in which case knowledge of the usage of `OVER` and `PARTITION` can prove to be invaluable. In addition if using an ORM which does generate SQL using `OVER` its important to understand the SQL and how the code effects the SQL generation.

---

## References

[SELECT - OVER Clause (Transact-SQL)](https://docs.microsoft.com/en-us/sql/t-sql/queries/select-over-clause-transact-sql?view=sql-server-ver16)   

---

<?# DailyDrop ?>135: 10-08-2022<?#/ DailyDrop ?>
