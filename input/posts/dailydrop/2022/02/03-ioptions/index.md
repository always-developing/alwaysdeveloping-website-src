---
title: "Use IOptions<> for application configuration"
lead: "Use IOptions for configuration and leverage the additional available interfaces "
Published: 02/03/2022
slug: "03-ioptions"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - ioptions
    - ioptionssnapshot
    - ioptionsmonitor
    - optionspattern
    - configuration
---

## Daily Knowledge Drop

Instead of trying to manually setup the dependency injection container with configuration from the, for example, appsettings.json file, use the built in .NET functionality and use the _IOptions_ interface instead - and get _IOptionsSnapshot_ and _IOptionsMonitor_ for free!

This post won't go into details around the _[options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0)_ specifically, but it's the recommended approach when dealing with application settings as it enables the application to adhere to two important software architecture principles:
- The `Interface Segregation Principle` (the I in SOLID)
- `Separation of concerns`

Onto some code examples - on startup when configuration the DI container:

❌ Rather don't do this:
``` csharp
var appOptions = new ApplicationOptions();
configuration.GetSection("appConfiguration").Bind(appOptions);
services.AddSingleton(options);
```

With the above, _**ApplicationOptions**_ can be injected into the relevant constructor and the application settings accessed.  
Nothing inherently "wrong" with this, it works and follows the _options pattern_. However there is a better way.

✅ Rather do this:
``` csharp
var optionSection = configuration.GetSection("appConfiguration");
services.Configure<ApplicationOptions>(optionSection);
```

With the above, _**ApplicationOptions**_ can NO longer be injected into the relevant constructor, instead _**IOptions\<ApplicationOptions\>**_ (or one of the other two interfaces mentioned below) can be injected, allowing for access to the settings.

---

## Why use Configure 

So why use the _IServiceCollection.Configure_ method instead of the _Bind + AddSingleton_ methods as described above.

Just by using the _IServiceCollection.Configure_ method, one automatically gets ato leverage the functionality of the three options interfaces.

For all three examples below, the following section has been added to _appsettings.json_:

``` json
  "appConfiguration": {
    "ApplicationName" : "OptionsDemo"
  }
```

And the options class, _ApplicationOptions_ defined as follows:

``` csharp
public class ApplicationOptions
{
    public string ApplicationName { get; set; }
}
```

---

### IOptions

   ✅ Added as DI container as **singleton**  
   ❌ Does **not** allow reading of the configuration settings from source after the app has started.  

``` csharp
var builder = WebApplication.CreateBuilder(args);

// get the "appConfiguration" section from the configuration 
//(appsettings.json in this case)
var optionSection = builder.Configuration.GetSection("appConfiguration");
// add to DI as ApplicationOptions 
builder.Services.Configure<ApplicationOptions>(optionSection);

var app = builder.Build();

// endpoint, which has the IOptions injected into it from the
// DI container
app.MapGet("/appname", (IOptions<ApplicationOptions> options) =>
{
    // .Value returns ApplicationOptions
    return options.Value.ApplicationName;
});

app.Run();
```

When the endpoint _/appname_ is called, the application name from the appsettings.json is returned, via _IOptions_.

This injects _IOptions\<ApplicationOptions\>_ as a `singleton`, and if the value in the appsettings.json file changes while the application is running, `the change will not be reflected in IOptions<ApplicationOptions>`.

---

### IOptionsSnapshot

   ✅ Added as DI container as **scoped**  
   ✅ Supports **named options**  
   ✅ Configuration settings can be recomputed for each request (as the service is scoped)

``` csharp
var builder = WebApplication.CreateBuilder(args);

// get the "appConfiguration" section from the configuration 
//(appsettings.json in this case)
var optionSection = builder.Configuration.GetSection("appConfiguration");
// add to DI as ApplicationOptions 
builder.Services.Configure<ApplicationOptions>(optionSection);

var app = builder.Build();

// endpoint, which has the IOptionsSnapshot injected into it from the
// DI container
app.MapGet("/appname", (IOptionsSnapshot<ApplicationOptions> options) =>
{
    // .Value returns ApplicationOptions
    return options.Value.ApplicationName;
});

app.Run();
```

This injects _IOptionsSnapshot\<ApplicationOptions\>_ as `scoped`, and if the value in the appsettings.json file changes while the application is running, this change `will be reflected in IOptionsSnapshot<ApplicationOptions>`.  

In other words, for each scope (http request) a new snapshot of the ApplicationOptions values is calculated from source, and injected.

---

### IOptionsMonitor

   ✅ Added as DI container as **singleton**  
   ✅ Supports **named options**  
   ✅ Supports options **changed notifications**  

``` csharp
var builder = WebApplication.CreateBuilder(args);

var optionSection = builder.Configuration.GetSection("appConfiguration");
builder.Services.Configure<ApplicationOptions>(optionSection);

var app = builder.Build();

app.Services.GetService<IOptionsMonitor<ApplicationOptions>>()
    .OnChange((ApplicationOptions options) =>
{
    Console.WriteLine(options.ApplicationName);
});

app.MapGet("/appname", (IOptionsMonitor<ApplicationOptions> options) =>
{
    return options.CurrentValue.ApplicationName;
});

app.Run();
```

This injects _IOptionsMonitor\<ApplicationOptions\>_ as a `singleton`, but functions very much the same as _IOptionsSnapshot_. If the value in the appsettings.json file changes while the application is running, this change `will be reflected in IOptionsMonitor<ApplicationOptions>`.  

However _IOptionsMonitor_ has the additional benefit of having an _OnChange_ method, which accepts an _Action\<\>_ which is called each time a value is changed. In other words, `one can be notified of a value change`.  

In the above example, the lambda action method is called each time the value changes and writes the new value to the console.

---

## Named Options

Both _IOptionsSnapshot_ and _IOptionsMonitor_ support `named options`. What this means, is that multiple of the same options (but different values) can be added to the DI container with a name, and then retrieved by name.

If there are multiple sets of the same configuration structure, for example:

``` json
"cloudKeyConfiguration": {
    "azure": {
        "name": "Microsoft",
        "key": "azurekey123"
    },
    "aws": {
        "name": "Amazon",
        "key": "awskey456"
    },
    "gcp": {
        "name": "Google",
        "key": "gcpkey789"
    }
}
```

And the options class, _CloudKeyOptions_ defined as follows:

``` csharp
public class CloudKeyOptions
{
    public string Name { get; set; }

    public string Key { get; set; }
}
```

Usage is as follows:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// add to DI container by name
var optionSectionAzure = builder.Configuration.GetSection("cloudKeyConfiguration:azure");
builder.Services.Configure<CloudKeyOptions>("azure", optionSectionAzure);

var optionSectionAws = builder.Configuration.GetSection("cloudKeyConfiguration:aws");
builder.Services.Configure<CloudKeyOptions>("aws", optionSectionAws);

var optionSectionGcp = builder.Configuration.GetSection("cloudKeyConfiguration:gcp");
builder.Services.Configure<CloudKeyOptions>("gcp", optionSectionGcp);

var app = builder.Build();

app.MapGet("/key/{provider}", (string provider, 
    IOptionsSnapshot<CloudKeyOptions> options) =>
{
    return options.Get(provider)?.Key;
});

app.Run();
```

_IOptionsSnapshot\<CloudKeyOptions\>_ is injected, and a query string parameter is used to determine which named option to retrieve.

---

## References
[C# configuration fundamentals](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0)  
[Options Pattern In .NET](https://thecodeblogger.com/2021/04/21/options-pattern-in-net-ioptions-ioptionssnapshot-ioptionsmonitor/)

<?# DailyDrop ?>03: 03-02-2022<?#/ DailyDrop ?>