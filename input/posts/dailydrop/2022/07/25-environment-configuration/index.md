---
title: "Environment specific startup Configure methods"
lead: "How environment specific named Configure methods can be used during startup"
Published: "07/25/2022 01:00:00+0200"
slug: "25-environment-configuration"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - api
    - configuration

---

## Daily Knowledge Drop

When using the `Startup` class approach for application startup (vs the top level minimal api approach introduced in .NET6), instead of the normal _Configure_ method used to configure the middleware pipeline, a method can be specified `per environment`. This environment specific _Configure_ method is automatically invoked depending on the **ASPNETCORE_ENVIRONMENT** environment variable value.

---

## Startup

Prior to .NET 6, when creating an ASP.NET Core web API project, a `Program.cs` file is created containing the _Main_ method, and a separate `Startup.cs` file is created containing the dependency injection setup method (_ConfigureServices_) as well as the middleware pipeline setup method **_Configure_**.

The Host setup in `Program.cs` is configured to use the _Startup_ class in `Startup.cs` to get its configuration:

``` csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            // Use the Startup class
            webBuilder.UseStartup<Startup>();
        });
```

In .NET6 the default changes to contain only a `Program.cs`, and instead of a instance of _HostBuilder_ being used, an instance of _WebApplicationBuilder_ is used:

``` csharp
var builder = WebApplication.CreateBuilder(args);
```

The .NET6 default can be retro-fitted to use the same approach as prior versions (Startup class), but this post is mainly focused on versions prior to .NET6.

---

### Default Configuration

By default in the `Startup.cs` the _Configure_ method which is used to setup the middleware pipeline. This method is automatically invoked by the runtime:

``` csharp
// This method gets called by the runtime. Use this method to 
// configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

If the pipeline needs to differ per environment, then checks, such as the one below, need to be implemented:

``` csharp
// Check if we are running in a development environment
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
```

Here the developer exception page is only configured if the application is running in _Development_ - when the **ASPNETCORE_ENVIRONMENT** is set to **Development**.

Using these checks is the default technique to configure the pipeline per environment - however, especially if the pipelines differ greatly per environment, these checks could become complicated and potentially confusing.

An alternative technique is to setup a `configure method per environment`.

---

### Environment Configuration

Setting up a _Configure_ method per environment is very simple - in the _Startup_ class, just create a new method called _Configure{EnvironmentName}_.

For example, the _ConfigureTest_ method will be called when running in **Test**, instead of the default _Configure_ method, and the _ConfigureProduction_ method will be called when running in **Production**:

``` csharp

public void ConfigureTest(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async (HttpContext ctx) => 
            await ctx.Response.WriteAsync("In Test environment"));
        endpoints.MapControllers();
    });
}

public void ConfigureProduction(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

It no environment specific method could be found, then the default _Configure_ method is called.

The same process can be applied to "non-built-in" environments (Development, Test, Production) - if when running locally, the **ASPNETCORE_ENVIRONMENT** environment variable is set to **LocalDevelopment**, for example, then the following method will be called:

``` csharp
// Method named Configure{EnvironmentName}
 public void ConfigureLocalDevelopment(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/debug", async (HttpContext ctx) => 
            await ctx.Response.WriteAsync("Special debug information"));
        endpoints.MapControllers();

    });
}
```

---

## Notes

Perhaps not especially relevent if working with the latest version of .NET (6 and above), or if there is not major differences in middleware pipelines between difference environments - this is still an interesting technique available if one requires it to ensure cleaner and easier to understand code.

---

<?# DailyDrop ?>123: 25-07-2022<?#/ DailyDrop ?>
