---
title: "Entity Framework Core interceptors"
lead: "Query interception with Entity Framework Core interceptors"
Published: 02/24/2022
slug: "24-efcore-interceptors"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - efcore
    - ef
    - entityframework
    - entityframeworkcore
    - interceptor
    - interceptors
    
---

## Daily Knowledge Drop

Entity Framework Core has the concept of `interceptors` which allow for the `insertion of custom logic during the query execution process`.  

There a number of real world applications for the functionality, for example:
- Caching and retrieval of data
- Logging query or diagnostics information under certain conditions
- Modifying the query parameters, such as the timeout under certain conditions

---

## Interceptor structure

Creating an `interceptor` is straight forward - a class is created which _implements the abstract class DbCommandInterceptor_, and then _overrides the required relevant method(s)_.

``` csharp
// Implement from DbCommandInterceptor
public class ExecutionThresholdInterceptor : DbCommandInterceptor
{
    // declare const message string
    private const string executionRangeExceededMessage = 
            "Query ran longer than expected. Milliseconds: {0}, Query: {1}";

    // override the method executed when the datareader has been executed
    public override DbDataReader ReaderExecuted(DbCommand command, 
        CommandExecutedEventData eventData, DbDataReader result)
    {
        // check how long the query took to execute
        // if its longer than the threshold
        if( eventData.Duration.TotalMilliseconds > 10)
        {
            // Log a rudimentary error message
            Console.WriteLine(executionRangeExceededMessage, 
                eventData.Duration.TotalMilliseconds, command.CommandText);
        }

        return result;
    }
}
```

There are a number of methods available for overriding, where custom logic can be executed. Each are called at a different stage of the query creation and execution process:

|Method|Information|
|------|-----------|
|CommandCreated|Called immediately after EF calls CreateCommand()|
|CommandCreating|Called just before EF intends to call CreateCommand()|
|CommandFailed|Called when execution of a command has failed with an exception|
|CommandFailedAsync|Called when execution of a command has failed with an exception|
|DataReaderDisposing|Called when execution of a DbDataReader is about to be disposed|
|NonQueryExecuted|Called immediately after EF calls ExecuteNonQuery()|
|NonQueryExecutedAsync|Called immediately after EF calls ExecuteNonQueryAsync()|
|NonQueryExecuting|Called just before EF intends to call ExecuteNonQuery()|
|NonQueryExecutingAsync|Called just before EF intends to call ExecuteNonQueryAsync()|
|ReaderExecuted|Called immediately after EF calls ExecuteReader()|
|ReaderExecutedAsync|Called immediately after EF calls ExecuteReaderAsync()|
|ReaderExecuting|Called just before EF intends to call ExecuteReader()|
|ReaderExecutingAsync|Called just before EF intends to call ExecuteReaderAsync()|
|ScalarExecuted|Called immediately after EF calls ExecuteScalar()|
|ScalarExecutedAsync|Called immediately after EF calls ExecuteScalarAsync()|
|ScalarExecuting|Called just before EF intends to call ExecuteScalar()|
|ScalarExecutingAsync|Called just before EF intends to call ExecuteScalarAsync()|

---

## Interceptor configuration

Once we have an `interceptor` defined (implementing _DbCommandInterceptor_), the next step is to make EF Core aware of it. This is done on the _DbContext_:

``` csharp
public class EFInterceptorsContext : DbContext
{
    public DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=EFInterceptors;Integrated Security=True")
               // add the interceptor(s)
                .AddInterceptors(thresholdInterceptor); 
        }

        // Interceptors are often stateless, so a single interceptor 
        // instance can be used for all DbContext instances
        private static readonly ExecutionThresholdInterceptor thresholdInterceptor = 
            new ExecutionThresholdInterceptor();
}
```

Executing a query using the _DbContext_, we get the following output to the console window from the interceptor:

``` powershell
Query ran longer than expected. Milliseconds: 25,089, Query: 
SELECT [t].[Artist], [t0].[YearReleased], [t0].[Id]
FROM (
    SELECT [s].[Artist]
    FROM [Song] AS [s]
    GROUP BY [s].[Artist]
) AS [t]
OUTER APPLY (
    SELECT DISTINCT [s0].[Id], [s0].[Artist], [s0].[LengthInSeconds], 
        [s0].[Name], [s0].[YearReleased]
    FROM [Song] AS [s0]
    WHERE [t].[Artist] = [s0].[Artist]
) AS [t0]
ORDER BY [t].[Artist]
```

---

## Suppress Execution

It is possible to _suppress the execution_ of a query from an `interceptor` - however other installed `interceptors` will still be executed, so they will each need to check if the exception has been suppressed by a previous `interceptor`.

In this example below, we'll configure two `interceptors` to be run _before the query is executed_:

``` csharp
// This interceptor will suppress the execution of the query if the
// query is looking at the "Song" table
public class TableDownInterceptor : DbCommandInterceptor
{
    private const string suppressMessage = "Table '{0}'is not currently " +
        "available for querying. Query being suppressed: {1}";

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, 
        CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        // Simple check to see if the command text contains the "Song" table
        if (eventData.Command.CommandText.Contains("[Song]"))
        {
            // Output a message indicating the query is being suppressed
            Console.WriteLine(suppressMessage, "Song", eventData.Command.CommandText);
            // Suppress the results with a custom empty data reader
            result = InterceptionResult<DbDataReader>
                .SuppressWithResult(new TableDownDataReader());
        }
        return result;
    }
}
```

The custom _DataReader_ looks as follows. It does nothing by return an empty dataset:

``` csharp
internal class TableDownDataReader : DbDataReader
{
    public override int FieldCount
        => throw new NotImplementedException();

    public override int RecordsAffected => 0;

    public override bool HasRows => false;

    public override bool IsClosed
        => throw new NotImplementedException();

    public override int Depth => 0;

    public override bool Read() => false;

    public override int GetInt32(int ordinal) => 0;

    public override bool IsDBNull(int ordinal) => false;

    public override string GetString(int ordinal) => string.Empty;

    public override bool GetBoolean(int ordinal)
        => throw new NotImplementedException();

    public override byte GetByte(int ordinal)
        => throw new NotImplementedException();

    public override long GetBytes(int ordinal, long dataOffset, 
        byte[] buffer, int bufferOffset, int length)
        => throw new NotImplementedException();

    public override char GetChar(int ordinal)
        => throw new NotImplementedException();

    public override long GetChars(int ordinal, long dataOffset, 
        char[] buffer, int bufferOffset, int length)
        => throw new NotImplementedException();

    public override string GetDataTypeName(int ordinal)
        => throw new NotImplementedException();

    public override DateTime GetDateTime(int ordinal)
        => throw new NotImplementedException();

    public override decimal GetDecimal(int ordinal)
        => throw new NotImplementedException();

    public override double GetDouble(int ordinal)
        => throw new NotImplementedException();

    public override Type GetFieldType(int ordinal)
        => throw new NotImplementedException();

    public override float GetFloat(int ordinal)
        => throw new NotImplementedException();

    public override Guid GetGuid(int ordinal)
        => throw new NotImplementedException();

    public override short GetInt16(int ordinal)
        => throw new NotImplementedException();

    public override long GetInt64(int ordinal)
        => throw new NotImplementedException();

    public override string GetName(int ordinal)
        => throw new NotImplementedException();

    public override int GetOrdinal(string name)
        => throw new NotImplementedException();

    public override object GetValue(int ordinal)
        => throw new NotImplementedException();

    public override int GetValues(object[] values)
        => throw new NotImplementedException();

    public override object this[int ordinal]
        => throw new NotImplementedException();

    public override object this[string name]
        => throw new NotImplementedException();

    public override bool NextResult()
        => throw new NotImplementedException();

    public override IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
```

Now we configure a second interceptor which will check if the first interceptor has suppressed the result, before executing it's own logic:

``` csharp
public class LoggingInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, 
        InterceptionResult<DbDataReader> result)
    {

        // Check if the result entity already has a value (supplied by the 
        // previous interceptor) and if so, then don't execute the 
        // `interceptor` logic, as the result has already been handled.
        if (!result.HasResult)
        {
            Console.WriteLine(eventData.Command.CommandText);
        }
        return result;
    }
}
```

Multiple `interceptors` can be added to EF Core:

``` csharp
public class EFInterceptorsContext : DbContext
{
    public DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=EFInterceptors;Integrated Security=True")
            // add the interceptor(s)
            .AddInterceptors(tableDownInterceptor, loggingInterceptor); 
    }

    private static readonly LoggingInterceptor loggingInterceptor 
        = new LoggingInterceptor();

    private static readonly TableDownInterceptor tableDownInterceptor 
        = new TableDownInterceptor();
}
```

Running the same query as before we get the following output:

``` powershell
Table 'Song' is not currently available for querying. Query being suppressed: 
SELECT [t].[Artist], [t0].[YearReleased], [t0].[Id]
FROM (
    SELECT [s].[Artist]
    FROM [Song] AS [s]
    GROUP BY [s].[Artist]
) AS [t]
OUTER APPLY (
    SELECT DISTINCT [s0].[Id], [s0].[Artist], [s0].[LengthInSeconds], 
        [s0].[Name], [s0].[YearReleased]
    FROM [Song] AS [s0]
    WHERE [t].[Artist] = [s0].[Artist]
) AS [t0]
ORDER BY [t].[Artist]
```

Only the output logic from `TableDownDataReader` is executed, and not the output logic from `LoggingInterceptor`.

---

## Additional example

Another `interceptor` example which wil log the query executed when an exception occurs. Generally the exception will be surfaced, but without the actual query being executed - knowing this can assist in narrowing down the root cause of the exception:

``` csharp
public class ExceptionInterceptor : DbCommandInterceptor
{
    private const string exceptionMessage = 
        "Exception occurred running the following command: {0}";

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        Console.WriteLine(exceptionMessage, eventData.Command.CommandText);
    }
}
```

---

## Notes

`Interceptors` are very easy to setup, and can offer a wide range of real world applications. There are potential performance overheads intercepting every single query (and logging it, for example) - but as with most features, there is always a tradeoff (in this case between performance and usefulness), and these would need to be evaluated for each specific use case.

---

## References
[Interceptors](https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors)  
[DbCommandInterceptor Class](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.diagnostics.dbcommandinterceptor?view=efcore-6.0)

<?# DailyDrop ?>18: 24-02-2022<?#/ DailyDrop ?>
