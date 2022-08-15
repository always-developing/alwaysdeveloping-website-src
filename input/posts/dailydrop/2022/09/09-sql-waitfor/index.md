---
title: "Delay SQL execution with WAITFOR"
lead: "Using the SQL WAITFOR command to delay the execution of a SQL statement"
Published1: "09/09/2022 01:00:00+0200"
slug: "09-sql-waitfor"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - sqlserver
    - waitfor
    - delay

---

## Daily Knowledge Drop

SQL is a `WAITFOR` command which an be used to delay the execution of a proceeding SQL statement. One of two options can be supplied to the `WAITFOR` statement:
- `TIME`: waits until the  time of day specified before executing the next statement
- `DELAY`: waits for the time span specified before executing the next statement

---

## WAITFOR Format

The format and usage of `WAITFOR` is straightforward:

``` sql
WAITFOR DELAY 'time_to_pass' | TIME 'time_to_execute'
```

---

## WAITFOR DELAY

`WAITFOR DELAY` will wait the `specified time span` before executing the next command:

``` sql
PRINT CONVERT(varchar, SYSDATETIME(), 121)
GO

WAITFOR DELAY '00:00:05';
GO

PRINT CONVERT(varchar, SYSDATETIME(), 121)
GO
```

Executing the above will result in the following output:

``` terminal
2022-09-08 20:39:51.2166773
2022-09-08 20:39:56.2387040

Completion time: 2022-09-08T20:39:56.2397037+02:00
```

A time span between `00:00:00.001` and `23:59:59.998` can be specified - anything longer will result in an error.

---

## WAITFOR TIME

`WAITFOR TIME` will wait until the `specified time of day` before executing the next command:

``` sql
PRINT CONVERT(varchar, SYSDATETIME(), 121)
GO

WAITFOR TIME '20:47:00';
GO

PRINT CONVERT(varchar, SYSDATETIME(), 121)
GO
```

Executing the above will result in the following output:

``` terminal
2022-08-15 20:46:48.9212859
2022-08-15 20:47:00.0130443

Completion time: 2022-08-15T20:47:00.0140433+02:00
```

In this example, we only had to wait _12 seconds_, but the wait could be longer depending on the time specified. Only a `time can be specified, not a particular date`. If the next execution of the specified time is the following day, the wait until the time is reached the following day.

---

## Scheduling

The below is an _example_ of how the functionality can be used to executing a command on a specific schedule. This technique is used for demo purposes not necessarily recommended for a production use case.

The following will execute the command every second for a full minute, once a day:

``` sql
-- wait until a specific time
WAITFOR TIME '20:52:00';
GO

-- while it is the 53th minute
WHILE(DATEPART(MINUTE, GETDATE()) < 54)
BEGIN
        -- print out the date time
	BEGIN
		PRINT CONVERT(varchar, SYSDATETIME(), 121)
	END

        -- wait for 1 second
	BEGIN
		WAITFOR DELAY '00:00:01';
	END
END
GO
```

The output:

```terminal
2022-08-15 20:52:00.0145585
2022-08-15 20:52:01.0162047
2022-08-15 20:52:02.0209920
2022-08-15 20:52:03.0229534
.
.
.
2022-08-15 20:53:56.6402032
2022-08-15 20:53:57.6424606
2022-08-15 20:53:58.6450557
2022-08-15 20:53:59.6499793
```

---

## Notes

While not necessarily a command which will see every day usage, it could prove useful to simulate a specific "real world" scenario when there is a time span between statements, either for testing or investigation purposes.

---

## References

[SQL WAITFOR Command to Delay SQL Code Execution](https://www.mssqltips.com/sqlservertip/7344/delay-sql-code-execution-with-sql-waitfor/)   

<?# DailyDrop ?>157: 09-09-2022<?#/ DailyDrop ?>
