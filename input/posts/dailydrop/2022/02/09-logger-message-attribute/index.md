---
title: "Performant logging with LoggerMessageAttribute"
lead: "How to leverage LoggerMessageAttribute for a highly performant logging solution"
Published: 02/09/2022
slug: "09-logger-message-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - logging
    - sourcegenerator
    - performance
    - attribute
    
---

## Daily Knowledge Drop

.NET6 introduced a new attribute called `LoggerMessageAttribute`, which leverages source generators and is _designed to deliver a highly useable and performant logging solution_.

It works by using source generators, triggered at compile time by the presence of `LoggerMessageAttribute` to generate the additional source code. This solution, due to the compile time generation, is able to eliminate performance hits, such as _boxing_, _temporary memory allocation_ as well as _copies_ which enables it to be typically considerably faster than the existing run time logging methods.

---

## Usage

### Basic usage

The `LoggerMessageAttribute` is constrained to be used on a `partial method` in a `partial class`.

``` csharp
public static partial class LoggerHelperStatic
{
    [LoggerMessage(EventId = 0,Level = LogLevel.Critical,Message = 
        "Unable to open a database connection to '{dbServer}' " +
        "after a timeout of {timeoutSeconds} seconds")]
    public static partial void UnableToOpenDbConnection(ILogger logger, 
        string dbServer, int timeoutSeconds);
}
```

In the attribute, the information for the log is defined. In the above example, the _EventId_, _LogLevel_ as well as the _Log Message_.

**Thats all there is to it. This method can now be called from anywhere in code using** `LoggerHelperStatic.UnableToOpenDbConnection(...)`.

Even though the method doesn't have a body defined, the presence of the `LoggerMessageAttribute` will trigger the source generator, which will use the log information defined to generate the required source code for the body.

### Extended usage

If the method is `static`, then an ILogger interface is **required** to be one of the parameters to the method. The method is not required to be static however, and if not made static the source generator will use an ILogger  field within the containing class.

``` csharp
public partial class LoggerHelperInstance
{
    private readonly ILogger _logger;

    public LoggerHelperInstance(ILogger logger)
    {
        _logger = logger;
    }

   [LoggerMessage(EventId = 0, Level = LogLevel.Critical, Message =
        "Unable to open a database connection to `{dbServer}` " +
        "after a timeout of {timeoutSeconds} seconds")]
    public partial void UnableToOpenDbConnection(string dbServer, 
          int timeoutSeconds);
}
```

In the above examples, the log level is also explicitly set to `Critical` in the `LoggerMessageAttribute`. Sometimes it may be required that the log level is specified at runtime - in this case the LogLevel can be omitted from the `LoggerMessageAttribute` definition, and included as a parameter to the method.

``` csharp
[LoggerMessage(EventId = 0, Message =
    "Unable to open the file '{fileName} at location '{fileLocation}'")]
public static partial void UnableToOpenFile(ILogger logger, LogLevel logLevel,
    string fileName, string fileLocation);
```

---

### How it works

This all works due to a feature introduced in .NET5 - `source generators`. At compile time, a source generator will look for a specific condition in code, and output additional source code which is added to the original code base. 

In the case of `LoggerMessageAttribute`, the condition is the presence of the attribute which then triggers the source generator to generate the body of the defined method - this is _why the class and method are required to be partial_.

Below is the generated code for the body of the _LoggerHelperInstance => UnableToOpenDbConnection_ method. It might be a bit of a challenging to read with the formatting, as I've left it in its original generated state.

``` csharp
partial class LoggerHelperInstance 
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
    private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.String, global::System.Int32, global::System.Exception?> __UnableToOpenDbConnectionCallback =
        global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.String, global::System.Int32>(global::Microsoft.Extensions.Logging.LogLevel.Critical, new global::Microsoft.Extensions.Logging.EventId(0, nameof(UnableToOpenDbConnection)), "Unable to open a database connection to `{dbServer}` after a timeout of {timeoutSeconds} seconds", new global::Microsoft.Extensions.Logging.LogDefineOptions() { SkipEnabledCheck = true }); 

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "6.0.5.2210")]
    public partial void UnableToOpenDbConnection(global::System.String dbServer, global::System.Int32 timeoutSeconds)
    {
        if (_logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Critical))
        {
            __UnableToOpenDbConnectionCallback(_logger, dbServer, timeoutSeconds, null);
        }
    }
}
```

As one can see, it's not a straightforward wrap of the _ILogger.LogCritical_ method, but instead makes use of the _LoggerMessage.Define_ method with a callback _Action_.


### Performance gains

So why does this method improve on performance?  

With the normal traditional _ILogger Log_ method:

``` csharp
// Traditional ILogger.LogCritical call
_logger.LogCritical("Unable to open a database connection to `{dbServer}` " +
    "after a timeout of {timeoutSeconds} seconds", "(local)", 10);
```

The parameters are passed in as an `object?[]`:

``` csharp
// The signature of the LogCritical method, part of Microsoft.Extensions.Logging
public static void LogCritical(this ILogger logger, EventId eventId, 
            string? message, params object?[] args)

```

In the above example, the `string value "(local)"` and the `int value 10` are boxed from their respective values into an object as part of the object[]. They are then unboxed when being used - all of this has a performance overhead.

As the source generator, used in conjunction with `LoggerMessageAttribute`, can determine the types of the parameters at compile time, it can optimize the code being generated for the individual use case to _avoid the need to box and unbox_, and also avoid the performance hits that come with it. The same applies to any _temporary memory allocation_ and _copying_ which may be occurring when using the object?[] on the _ILogger_ implementation.

---

## Constraints

There are however some constraints which must be followed when using `LoggerMessageAttribute`:

- Logging methods must be **static**, **partial**, and **return void**.
- Logging **method names** must **not start with an underscore**.
- **Parameter names** of logging methods must **not start with an underscore**.
- Logging methods may **not be defined in a nested type**.
- Logging methods **cannot be generic**.

All in all though, these constraints are pretty reasonable and make sense when thought about in conjunction with what the source generator is doing.

---

## Notes

There is more detailed functionality available with `LoggerMessageAttribute`, which is outlined in the reference link below - however the above should be a great starting point to using `LoggerMessageAttribute` and optimizing logging in your application.

---

## References
[Compile-time logging source generation](https://docs.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator)

<?# DailyDrop ?>07: 09-02-2022<?#/ DailyDrop ?>
