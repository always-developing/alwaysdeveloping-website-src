---
title: "Multiple applications - one host"
lead: "How multiple applications can be run in a single ASPNET Core host with minimal api"
Published: "07/28/2022 01:00:00+0200"
slug: "28-multi-applications"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - host
    - api

---

## Daily Knowledge Drop

Multiple applications can be executed inside a single ASPNET Core host - this effectively `allows different endpoints and functionality to be available on the same base URL, but with different ports`.

I first learnt about this in  Khalid Abuhakmeh's [post on the subject](https://khalidabuhakmeh.com/hosting-two-aspnet-core-apps-in-one-host), however his post dives into using the _IHostBuilder_ (the technique prior to .NET6's top level statement and minimal api model). This post explores how to achieve the same output, but with the .NET6 _WebApplicationBuilder_, top level statements and minimal api.

---

## Single application

First let's look at the default host setup with a single application:

``` csharp
// Create the builder
var builder = WebApplication.CreateBuilder(args);

// create the app from the builder
var app = builder.Build();

// optionally set the port
app.Urls.Add("http://*:5001");

// define endpoints to expose on the port
app.MapGet("/main", () =>
{
    return "Welcome to the main application";
});

// run the application
await app.RunAsync();

```

There are a number of steps to configuring an application for startup:
1. Create the _WebApplicationBuilder_ instance, and optionally configure the dependency injection container (not done in this example)
1. Build the application, which returns a _WebApplication_ instance
1. Optionally configure the port(s) the application exposes
1. Optionally configure any endpoints to expose on the above ports
1. Run the application

To create multiple applications, basically these `steps need to be duplicated`, with some slight changes.

---

## Multiple applications

### Startup

In our sample, we are going to create two applications to be hosted - a _main_ application, which would expose business related endpoints, and an _admin_ application which exposes admin related endpoints. This setup is just for demo purposes - I wouldn't necessarily recommend this setup for a production application as a default.

Let's start at the top and duplicate the configuration.

1. Create the `WebApplicationBuilder instance`:

    ``` csharp
    // create the main application builder
    var builder = WebApplication.CreateBuilder(args);
    // create the admin application builder
    var adminBuilder = WebApplication.CreateBuilder(args);
    ```

1. `Build` the application:

    ``` csharp
    // main application
    var app = builder.Build();
    // admin application
    var adminApp = adminBuilder.Build();
    ```

1. `Configure the port(s)` the applications each expose:

    ``` csharp
    // the main application's endpoints will be exposed 
    // on port 5001
    app.Urls.Add("http://*:5001");
    // the admin endpoints will be exposed 
    // on port 5009
    adminApp.Urls.Add("http://*:5009");
    ```

1. Configure any `endpoints to be exposed`:

    ``` csharp
    // exposed the main application endpoints
    app.MapGet("/main", () =>
    {
        return "Welcome to the main application";
    });

    // expose the admin endpoints
    adminApp.MapGet("/admin", () =>
    {
        return "Welcome to the admin application";
    });
    ```

1. `Run` the application:

    ``` csharp
    // as we have multiple applications running
    // we execute both of them and wait for either
    // to finish before shutting down the host
    await Task.WhenAny(
        app.RunAsync(),
        adminApp.RunAsync()
    );
    ```

---

### Execution

Running the project/host now:

- Expose _main application_ functionality on port `5001`. Browsing to `http://localhost:5001/main` will return:

    ``` terminal
    Welcome to the main application
    ```

    While trying to access the _admin_ endpoint on the `main application port 5001` will not return any results.


- Expose _admin_ functionality on port `5009`. Browsing to `http://localhost:5009/admin` will return:

    ``` terminal
    Welcome to the admin application
    ```

    While trying to access the _main_ endpoint on the `admin application port 5009` will not return any results.

---

## Notes

While not something I would recommend as the default go-to method, the ability to segregate endpoints into completely separate applications can prove useful in certain use cases - such as separating business and admin functionality as demonstrated in the example or if running a multi-tenant application, another use case could to separate each tenant into their own application (and own ports). 
However there are probably better ways than creating multiple applications in one host for both the above mentioned use cases - but the knowledge the this is possible, is another tool to potentially use when required.

---

## References

[Hosting Two ASP.NET Core Apps In One Host](https://khalidabuhakmeh.com/hosting-two-aspnet-core-apps-in-one-host)   

---

<?# DailyDrop ?>126: 28-07-2022<?#/ DailyDrop ?>
