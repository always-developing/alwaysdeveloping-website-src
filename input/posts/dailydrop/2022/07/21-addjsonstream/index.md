---
title: "Configuration from memory with AddJsonStream"
lead: "Using AddJsonStream to load configuration in a unit test"
Published: "07/21/2022 01:00:00+0200"
slug: "21-addjsonstream"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - json
    - configuration
    - iconfiguration
    - unittest

---

## Daily Knowledge Drop

When writing `unit test` which require application configuration through `IConfiguration`, the `AddJsonStream` method can be used to load different application settings configuration from memory (instead of from an appsettings.json file).

---

## Setup

The setup is a fairly common pattern I've experienced when writing `shared, reusable, packaged libraries` - a piece of functionality is packaged into a library (not shown in this post, but published to a NuGet store). This library functionality is configured and added to the dependency injection container at startup using an extension method, with the configuration options for the library (optionally) specified in the appsettings.json file.

A `class library` is created for the shared component.

An `interface` is created as an abstraction for the logic:

``` csharp
public interface IDoWork
{
    int DoSomeWork();
}
```

And we also have an `options`/settings/configuration class which contains the settings for the library:

``` csharp
public class DoWorkOptions
{
    public bool IsEnabled { get; set; }
}
```

Then there is the implementation of the interface, the logic for the library (incredibly simple in this demo):

``` csharp
public class DoWork : IDoWork
{
    /// <summary>
    /// Stores the configuration options
    /// </summary>
    private readonly DoWorkOptions _options;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">The configuration options</param>
    public DoWork(DoWorkOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Executes the functionality of the library
    /// </summary>
    /// <returns>1 if enabled, 0 if disabled</returns>
    public int DoSomeWork()
    {
        if(_options.IsEnabled)
        {
            return 1;
        }

        return 0;
    }
}
```

And finally an extension method on `IServiceCollection` to configure all the pieces with the dependency injection container:

``` csharp
public static class DoWorkExtensions
{
    /// <summary>
    /// Configure the library with the DI container
    /// </summary>
    /// <param name="services">The IServiceCollection implementation</param>
    /// <param name="configuration">The IConfiguration implementation</param>
    /// <returns></returns>
    public static IServiceCollection AddDoWork(this IServiceCollection services, 
        IConfiguration configuration)
    {
        // load the options from the configuration implementation
        var options = new DoWorkOptions();
        configuration.GetSection("doWorkOptions").Bind(options);

        // add the functionality to DI
        services.AddTransient(typeof(IDoWork), typeof(DoWork));
        // add the options to DI
        services.AddSingleton(options);

        return services;
    }
}
```

To recap - we have:
- an interface for the functionality (_IDoWork_)
- with an implementation of the functionality (_DoWork_)
- which is configured using options (_DoWorkOptions_)
- and setup with the dependency injection container using an extension method (_DoWorkExtensions_)

Our very simple shared library is now `complete and ready to be used`! 

---

## Web api

First a look at how this would be used in an `minimal web api`.

With the necessary references in place, the extension method can be invoked on startup, which then allows for the the _IDoWork_ interface to be injected into the endpoint handler delegate:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// this line is not explicitly needed as its automatically done 
// if not specified here, but the configuration get loaded from the 
// appsettings.json file
builder.Configuration.AddJsonFile("appsettings.json");

// configured with dependency injection container
builder.Services.AddDoWork(builder.Configuration);

var app = builder.Build();

// injected IDoWork
app.MapGet("/work", (IDoWork worker) =>
{
    // execute the logic
    return worker.DoSomeWork();
});

app.Run();
```

With the appsettings.json being as follows:

``` json
{
  "DoWorkOptions": {
    "IsEnabled" :  true
  }
}
```

Calling the `/work` endpoint will return a `1 or 0` dependant on if the _DoWorkOptions -> IsEnabled_ flag is set to `true or false`

So how do we unit test a piece of code which is dependant on json configuration? - with the `AddJsonStream` method!

---

## Unit Test

`AddJsonStream` is an extension method on _ConfigurationBuilder_, which as the name implies, allows for loading of JSON configuration data from memory, instead of from file as is the the case with `AddJsonFile` above.

We can leverage this to write unit tests with different setups of configuration.

A note - there are other/better ways of performing endpoint testing, `WebApplicationFactory` for example, but these are intentionally simple and the logic is being tested directly (from the DI container), and not through an endpoint.

Test when the option is _**enabled**_:

``` csharp
[TestMethod]
public void Test_Enabled_Config()
{
    // create a memory stream with the json configuration
    var sr = new MemoryStream(Encoding.ASCII.GetBytes(@"
{
""DoWorkOptions"" : {
    ""IsEnabled"" : true
}
}"));

    // setup host
    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            // load from the memory stream, and not from file
            config.AddJsonStream(sr);
        })
        .ConfigureServices((context, services) => services
            .AddDoWork(context.Configuration)
        ).Build();

    // get the implementation from the DI container
    var worker = (IDoWork)host.Services.GetService(typeof(IDoWork));

    // invoke and assert the result
    Assert.AreEqual(1, worker.DoSomeWork());
}
```

Test when the option is _**disabled**_:

``` csharp
[TestMethod]
public void Test_Disabled_Config()
{
    // create a memory stream with the json configuration
    var sr = new MemoryStream(Encoding.ASCII.GetBytes(@"
{
""DoWorkOptions"" : {
    ""IsEnabled"" : false
}
}"));

    // setup host
    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            // load from the memory stream, and not from file
            config.AddJsonStream(sr);
        })
        .ConfigureServices((context, services) => services
            .AddDoWork(context.Configuration)
        ).Build();

    // get the implementation from the DI container
    var worker = (IDoWork)host.Services.GetService(typeof(IDoWork));

    // invoke and assert the result
    Assert.AreEqual(0, worker.DoSomeWork());
}
```

Additional tests can obviously be included for additional use cases (empty configuration file, invalid name or value in the file etc.) - and these are easy and quick to add and configure with `AddJsonStream`. 

---

## Notes

`AddJsonStream` is a very useful method when writing unit tests - I'm not sure how useful it would be outside of a unit test, during the normal execution of an application, but its available if required.

---

<?# DailyDrop ?>121: 21-07-2022<?#/ DailyDrop ?>
