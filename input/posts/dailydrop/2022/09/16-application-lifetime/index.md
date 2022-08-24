---
title: "Application lifetime events with IHostApplicationLifetime"
lead: "Using IHostApplicationLifetime call back events to handle post-startup and graceful shutdown tasks"
Published: "09/16/2022 01:00:00+0200"
slug: "16-application-lifetime"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - lifetime
   - applicationlifetime

---

## Daily Knowledge Drop

Application `OnStarted`, `OnStopping` and `OnStopped` event handlers can be registered using the `IHostApplicationLifetime` interface (and implementation), to run logic when the application _starts_, is _stopping_ and has _stopped_ respectively.

Zero or many event handlers can dynamically be registered to handle the required logic at the correct application lifetime event.

---

## Registering handlers
<?# InfoBlock ?>
The `OnStopping` and `OnStopped` events are **NOT** called when debugging the application using Visual Studio. The application has to be run using `dotnet run` from the command line to have these events successfully invoked whilst developing. More on this below under the _Graceful shutdown_ section.
<?#/ InfoBlock ?>

### Api

Registering callback handlers for the events is done by injecting `IHostApplicationLifetime`, and calling the _Register_ method on the relevent _CancellationToken_ property:

``` csharp
// Define an endpoint, which when called will register a callback
// method for each of the events
app.MapGet("/registerevents", (
    IHostApplicationLifetime hostApplicationLifetime) =>
{
    // register each of the events
    hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
    hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
    hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
});

app.Run();

void OnStarted()
{
    Console.WriteLine("Application OnStarted - api");
}

void OnStopping()
{
    Console.WriteLine("Application OnStopping - api");
}

void OnStopped()
{
    Console.WriteLine("Application OnStopped - api");
}
```

Executing the application (using _dotnet run_) and then closing the application (using `Ctrl-C`) _without calling the endpoint_, will result in _no callback events being called_ - as none have been registered.  

However executing the application, and invoking the `/registerevents` endpoint, will result in the _OnStarted_ callback event being called immediately, and the other two _OnClosing_ and _OnClosed_ events called when the application is shutdown. The output of the application (other logging output removed):

``` terminal
Application OnStarted - api
Application OnStopping - api
Application OnStopped - api
```

---

### Worker

Callback events can also be registered with `background/worker` services when they are instantiated. Updating the default .NET Worker template:

``` csharp
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    // inject IHostApplicationLifetime
    public Worker(ILogger<Worker> logger, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;

        // register the callback methods
        _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void OnStarted()
    {
        Console.WriteLine("Application OnStarted - worker");
    }

    private void OnStopping()
    {
        Console.WriteLine("Application OnStopping - worker");
    }

    private void OnStopped()
    {
        Console.WriteLine("Application OnStopped - worker");
    }
}
```

This background service can be registered in the same api application used in the previous minimal api example, using:

``` csharp
builder.Services.AddHostedService<Worker>();
```

Now executing the application, invoking the endpoint and then closing the application will result in the following output:

``` terminal
Application OnStarted - worker
Application OnStarted - api
Application OnStopping - api
Application OnStopping - worker
Application OnStopped - api
Application OnStopped - worker
```

--- 

## Graceful shutdown

While the `OnStopping` and `OnStopped` events are useful for performing logic when the application is _stopping_ and _stopped_ - these events are only called in the case of a `graceful shutdown`. For example, in the case when debugging using Visual Studio - when the debugging session is stopped, a graceful shutdown is **not** performed, and as such the stopping/stopped events are not invoked. `Application functionality should not assume that the 'OnStopping' and 'OnStopped' events will always be called.`

---

## Notes

The two closing events `OnStopping` and `OnStopped` are especially useful, and can be leveraged to perform application cleanup tasks on closing (while not depending on them always being executing). For example, clearing any lingering cache files, or persisting any in memory logs to disk can be executing when the application is stopping/stopped.

---

## References

[IHostApplicationLifetime](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-6.0#ihostapplicationlifetime)   

<?# DailyDrop ?>162: 16-09-2022<?#/ DailyDrop ?>
