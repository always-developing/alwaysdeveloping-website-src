---
title: "View config values with GetDebugView"
lead: "Use GetDebugView to view all configuration values"
Published: "07/05/2022 01:00:00+0200"
slug: "05-ef-find-vs-single"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - debug
    - config
    - configuration

---

## Daily Knowledge Drop

All configuration values for an aapplication, as well `as their source`, can be viewed with the `GetDebugView` method on _IConfigurationRoot_.

While this core functionality has been available since .NET Core 3, enhancements are also coming with .NET7 (currently in preview) to allow for confidential values to be masked.

All of this in more detail below.

---

## GetDebugView: Current

To retrieve the configuration information is fairly straightforward - all one needs is the _IConfiguration_ implementation.

The below uses top-level statements and minimal API to expose a _config_ endpoint:

``` csharp
// inject IConfiguration from dependency injection container
app.MapGet("/config", (IConfiguration config) =>
{
    // convert to IConfigurationRoot
    var root = config as IConfigurationRoot;

    return root.GetDebugView();
});
```

With a _appsettings.json_ file which contains the following:

``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ServiceCredentials": {
    "username": "admin123",
    "password" : "admin456"
  }
}
```

Browsing to the endpoint will return the following _relevent configuration values_ (along with numerous other system environment variables):

``` powershell
AllowedHosts=* (JsonConfigurationProvider for 'appsettings.json' (Optional))
ASPNETCORE_ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: '')
Logging:
  LogLevel:
    Default=Information (JsonConfigurationProvider for 'appsettings.Development.json' (Optional))
    Microsoft.AspNetCore=Warning (JsonConfigurationProvider for 'appsettings.Development.json' (Optional))
ServiceCredentials:
  password=admin456 (JsonConfigurationProvider for 'appsettings.json' (Optional))
  username=admin123 (JsonConfigurationProvider for 'appsettings.json' (Optional))
```

The configuration from the _appsettings.json_ config file are displayed, with the corresponding provider source (_JsonConfigurationProvider_) as well as a configuration value sourced from the environment variable provider (_EnvironmentVariablesConfigurationProvider_).

---

### Secrets exposed

One limitation of the current (.NET Core 3 to .NET 6) implementation, which is demonstrated above - is that configuration values which are secrets (keys, passwords, etc.) are included in the output.

In my example the password was stored in the _appsettings.json_, which ideally shouldn't happen - however even if injected at runtime as an environment variable, the same would occur, and the value would still be exposed (just coming from a different provider source).

The enhancements in .NET7 aim to improve this.

---

## GetDebugView: Preview

Bear in mind, that the following is done using a _Preview_ version of .NET7, and may change by the time it is officially release.

The `GetDebugView` now has an overload which accepts a _Func_ and allows for custom processing and manipulation of the configuration values for display:

``` csharp
// inject IConfiguration from dependency injection container
app.MapGet("/config", (IConfiguration config) =>
{
    // convert to IConfigurationRoot
    var root = config as IConfigurationRoot;

    return root.GetDebugView(context =>
    {
        // this Func keys called for each Key in the configuration

        // if the key is one we know contains a password
        if(context.Key == "ServiceCredentials:password")
        {
            // return a masked value
            return "***";
        }
        
        // otherwise return the original configuration value
        return context.Value;
    });
});
```

With a _appsettings.json_ file the same as before

``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ServiceCredentials": {
    "username": "admin123",
    "password" : "admin456"
  }
}
```

Browsing to the endpoint will return the same as before, but with one small adjustment: 

``` powershell
AllowedHosts=* (JsonConfigurationProvider for 'appsettings.json' (Optional))
ASPNETCORE_ENVIRONMENT=Development (EnvironmentVariablesConfigurationProvider Prefix: '')
Logging:
  LogLevel:
    Default=Information (JsonConfigurationProvider for 'appsettings.Development.json' (Optional))
    Microsoft.AspNetCore=Warning (JsonConfigurationProvider for 'appsettings.Development.json' (Optional))
ServiceCredentials:
  password=*** (JsonConfigurationProvider for 'appsettings.json' (Optional))
  username=admin123 (JsonConfigurationProvider for 'appsettings.json' (Optional))
```

The password returned is now the `masked value` specified in the _Func_!

---

## Notes

Having the ability to expose all configuration values can definitely save time and effort - it eliminates the need to remote into other machines or containers and get a list of environment variables when doing investigations. However the fact that confidential information could be exposed is far from ideal, and depending on how serious security is taken, could render the functionality unusable.

However the .NET7 enhancements provides a working solution for this, which I look forward to being able to leverage.

---

## References

[Viewing .NET configuration values](https://dunnhq.com/posts/2022/viewing-configuration-values/)  

---

<?# DailyDrop ?>110: 05-07-2022<?#/ DailyDrop ?>
