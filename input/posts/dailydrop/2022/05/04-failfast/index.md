---
title: "Environment.FailFast"
lead: "How FailFast can be used to skip the finally block"
Published: 05/04/2022
slug: "04-failfast"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - failfast
    - finally

---

## Daily Knowledge Drop

The `Environment.FaiFast` method can be used to close an application without calling any finally blocks or finalizers. 

---

## FailFast sample

In the below simple example, the application will try save a record to the database:
- if a `non-connectivity` related exception occurs, the error message is saved to the database. On exit of the code block (in the finally), a information message is also logged to the database
- if a `connectivity` related exception occurs, the system is obviously unable to save the exception to the database. So instead the application will call the `FailFast` method to close the application and bypass any database logging in the finally block.


``` csharp
try
{
    SaveRecordToDatabase();
}
catch(Exception ex)
{
    if(ex.Message == "Database connection exception")
    {
        Environment.FailFast(ex.Message);
    }

    LogExceptionToDatabase(ex);
}
finally
{
    LogMessageToDatabase();
}
```

---

### Success output

When the record is saved without an exception, the output would be as follows:

``` powershell
SaveRecordToDatabase => Record saved successfully
LogMessageToDatabase => Finished processing record
```

---

### Non-connectivity error

When the record is saved with a non-connectivity exception, the output would be as follows:

``` powershell
LogExceptionToDatabase => PK violation exception
LogMessageToDatabase => Finished processing records
```

From the output, one can see the _LogMessageToDatabase_ method in the finally block is called.

---

### Connectivity error

When the record is saved with a connectivity exception, the output would be as follows:

``` powershell
Process terminated. Database connection exception
   at System.Environment.FailFast(System.String)
   at Program.<Main>$(System.String[])
```

In this case, after the _FailFast_ method is called, the application process is terminated without the finally block being called.

---

## Notes

A fairly niche use case, but useful to know it possible to bypass any finally blocks or finalizers (deconstructor) if the need is there. The other alternative is to put checks in the finally and finalizers to exclude there execution in certain cases - but this "pollutes" the code unnecessarily.

---

## References

[12. Environment.FailFast()](https://www.automatetheplanet.com/top-15-underutilized-features-dotnet/#12)  

<?# DailyDrop ?>66: 04-05-2022<?#/ DailyDrop ?>
