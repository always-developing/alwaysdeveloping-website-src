---
title: "Windows services with .NET Core"
lead: "Configuring a background service to run as a Windows Service"
Published: "08/05/2022 01:00:00+0200"
slug: "05-windows-service"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - api
    - minimalapi
    - contenttype

---

## Daily Knowledge Drop

Configuring code to run as a `Windows Service` in .NET is simple and fairly straight-forward (at least simpler and more straight forward than I was expecting). The `Windows Service` specific configuration only required a NuGet package and a few lines of code.

This post will describe the `code changes` required to make the code ready to be hosted as a Windows Service, but will not go into specifics on how the Windows Service is configured.

---

## Background Service

Whether hosted as a `Windows Service` or not, the logic needs to be executed in the background, usually on a schedule or at specific intervals. This is done with the .NET _BackgroundService_ class.

To start creating this background process, a class is created which inherits from _BackgroundService_:

``` csharp
public class RandomWorker : BackgroundService
{
    private readonly ILogger<RandomWorker> _logger;

    public RandomWorker(ILogger<RandomWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // continually run until cancelled
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("RandomWorker Service running " +
                "as Windows Service at: {currentTime}", DateTime.Now);

            var random = new Random();
            var randomValue = random.Next(10);

            if(randomValue > 5)
            {
                _logger.LogError("RandomWorker running as Windows " +
                    "Service threw an exception at: {currentTime}", DateTime.Now);
            }

            // wait for 10 seconds
            await Task.Delay(10000, stoppingToken);
        }
    }
}
```

- The class operates with `dependency injection`, so any registered services can be injected (_ILogger_ in this example)
- The `ExecuteAsync` method is called once the background service is started - the `while` loop in conjunction with the `Task.Delay` call, ensures that the method is forever looping effectively executing every 10 seconds (roughly), until cancelled via the _CancellationToken_.

Now that we have a _BackgroundService_ which runs our logic, we need to host it so that it can be executed.

---

## Hosting

### Console hosting

The simplest option is to host it in a _Console Application_ - this is an exe which needs to be executed, and will run until the console windows is closed.

A top-level statement _Console Application_ is shown below, with the `RandomWorker` class added as a _Hosted Service_:

``` csharp
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Add the RandomWorker as a hosted service
        services.AddHostedService<RandomWorker>();
    })
    .Build();

await host.RunAsync();
```

A sample output:

``` terminal
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 16:40:20
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 16:40:30
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 16:40:40
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 16:40:50
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 16:41:00
fail: WindowsServiceDemo.RandomWorker[0]
      RandomWorker running as Windows Service threw an exception at: 07/10/2022 16:41:00
```

Now that we have the building blocks for a background service, running on an interval in a console we can look at how to turn this into a `Windows Service`.

---

### Windows Services hosting

Enhancing a traditional _Console Application_ to be able to be used as a `Windows Service` is fairly straight forward:

1. Reference the `Microsoft.Extensions.Hosting.WindowsService` NuGet package

2. Update the startup to include additional `Windows Service` specific configuration:

    ``` csharp
    using WindowsServiceDemo;

    IHost host = Host.CreateDefaultBuilder(args)
        // configure to be able to be used in Windows Service
        .UseWindowsService(options =>
        {
            // with the name
            options.ServiceName = "RandomWorker Service";
        })
        .ConfigureServices(services =>
        {
            services.AddHostedService<RandomWorker>();
        })
        .Build();

    await host.RunAsync();
    ```

3. Optionally configure `Event Viewer` logging - this step can be skipped if logging to the Event Viewer is not required. 
    The default logging level for `Event Viewer` is _Warning_, so for development purposes the default log level can be set to _Information_.
    In `appsettings.json`:

    ``` json
    {
        "Logging": {
            "EventLog": {
            "LogLevel": {
                "Default": "Information"
            }
            },
            "LogLevel": {
            "Default": "Information",
            "Microsoft.Hosting.Lifetime": "Information"
            }
        }
    }
    ```

That's it! (from a code configuration point of view). The application can now be run as a normal console application, but is also _ready_ to be hosted as a `Windows Service`.

This post will not go into detail around _how_ to configure the `Windows Service` - but a brief summary:

- The application needs to be [published](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service#publish-the-app)  
- A `Windows Service` needs to be created using the `sc.exe` tool. The tool creates the Windows Service and [links it to the exe created in the above step](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service#create-the-windows-service)  
- Optionally [configure the Windows Service](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service#configure-the-windows-service)  

Ideally all of the above steps are done in a CI/CD pipeline, with all the steps automated.

---

### Api hosting

A note on the `BackgroundService` inherited class, `RandomWorker` - in the above example it was hosted in a _Console Application_ using:

``` csharp
    services.AddHostedService<RandomWorker>();
```

The great thing about the `BackgroundService`, is that it can be hosted in an API if required - allowing the service logic to become `cross platform`.

Below is top level statement API, using a minimal api - but also `hosting the background service`:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<RandomWorker>();

var app = builder.Build();

app.MapGet("/randomservice", (ILogger<RandomWorker> logger) =>
{
     logger.LogInformation("Endpoint called and executed " +
        "while background service is running");
});

app.Run();
```

Running this API now executes the service in the background, while still allowing endpoints to be called:

``` terminal
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 17:15:13
fail: WindowsServiceDemo.RandomWorker[0]
      RandomWorker running as Windows Service threw an exception at: 07/10/2022 17:15:13
info: WindowsServiceDemo.RandomWorker[0]
      Endpoint called and executed while background service is running
info: WindowsServiceDemo.RandomWorker[0]
      RandomWorker Service running as Windows Service at: 07/10/2022 17:15:23
```

---

## Notes

In my personal experience, in the last few years `Windows Services` have seen a decline in usage in favour of more cross-platform solutions, such as hosting the background service in an API, or using other scheduling solutions such as [HangFire](https://www.hangfire.io/).
However, in the case when hosting the code as a `Windows Service` is unavoidable - it's good to know that its fairly simple and straightforward to configure .NET Core (and beyond) code to function as a Windows Service.

---

## References

[Create a Windows Service using BackgroundService](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service)   
[Running .NET Core Applications as a Windows Service](https://code-maze.com/aspnetcore-running-applications-as-windows-service/)   

---

<?# DailyDrop ?>132: 05-08-2022<?#/ DailyDrop ?>
