---
title: "Simplifying command line argument with switch mappings"
lead: "Exploring switch mappings to simplify (and alias) command line arguments"
Published: "09/21/2022 01:00:00+0200"
slug: "21-commandline-mappings"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - commandline
   - mapping
   - switchmappping

---

## Daily Knowledge Drop

When working with command line argument, `switch mappings` can be used to create `alias` and `short keys` for the arguments, simplifying their usage for the consumer of the application.

---

## Command line setup

By default, arguments passed to an application will automatically be made available through the _IConfiguration_ interface/implementation. Consider the below minimal endpoint:

``` csharp
app.MapGet("/getarguments", (IConfiguration config) =>
{
    // access the arguments using the key, via
    // the IConfiguration implementation
    return new
    {
        Argument1 = config["argument1"],
        Argument2 = config["argument2"],
    };
});
```

Command line arguments can be passed to the application with the following command:

```powershell
dotnet run argument1=hello argument2=world
```

or by specifying the arguments in the _launchSettings.json_ file and executing in Visual Studio:

``` json
 "CommandLineMapping": {
    "commandName": "Project",
    "commandLineArgs": "argument1=hello argument2=world",
    "launchBrowser": true,
    "launchUrl": "getarguments",
    "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
    },
    "dotnetRunMessages": true,
    "applicationUrl": "http://localhost:5259"
}
```

Browsing to the _getarguments_ endpoint, we see the following response:

```json
{
  "argument1": "hello",
  "argument2": "world"
}
```

This all functions because under the hood, on startup, `builder.Configuration.AddCommandLine(args)` is being called. This explicitly maps the command line arguments to the .NET _IConfiguration_ implementation, making the arguments available to access.

So far so good - but suppose we want change the argument names from _argument1_ and _argument2_ to `arg1` and `arg2`. We don't want to remove support for the full names (as we don't want to create a breaking change), but if an argument called `arg1` is passed in, we want it to map to _argument1_ No code which specifically looks for an argument called _argument1_ is required to change.

---

## Aliases

The `builder.Configuration.AddCommandLine` has an overloaded method which takes in a `switch mapping` to perform a mapping, which can explicitly be called to created argument aliases.

On startup:

``` csharp
// define the mappings between the alias and the argument
Dictionary<string, string> mappings = new Dictionary<string, string>()
{
    ["--arg1"] = "argument1",
    ["--arg2"] = "argument2",
};
// explicitly call AddCommandLine with the mappings
builder.Configuration.AddCommandLine(args, mappings);

// build the application
var app = builder.Build();
```

Alias switch mappings are defined using a double dash (`--`) and an alias name (_arg1_) which then will be mapped to an full argument (_argument1_).

If the endpoint it updated:

``` csharp
app.MapGet("/getarguments", (IConfiguration config) =>
{
    return new
    {
        Argument1 = config["argument1"],
        Argument2 = config["argument2"],
        Arg1 = config["arg1"],
        Arg2 = config["arg2"]
    };
});
```

Now when executing:

``` powershell
 dotnet run --arg1=hello --arg2=world
```

The response is:

```json
{
  "argument1": "hello",
  "argument2": "world",
  "arg1": "hello",
  "arg2": "world"
}
```

The value supplied for _arg1_, is available through _IConfiguration["arg1"]_ as well as _IConfiguration["argument1"]_, thanks to the `switch mapping!`

If the dashes are omitted:

``` powershell
 dotnet run arg1=hello arg2=world
```

Then the response is:

```json
{
  "argument1": null,
  "argument2": null,
  "arg1": "hello",
  "arg2": "world"
}
```

The arguments are still available through the normal base configuration mechanism using _IConfiguration["arg1"]_ but are **not** mapped to, and available through, _IConfiguration["argument1"]_.

---

## Short key

Just as with aliases, it is also possible to map an argument to `short key` using a single dash prefix `-`. The setup is the same as described above with aliases:

``` csharp
// define the mappings between the alias and the argument
Dictionary<string, string> mappings = new Dictionary<string, string>()
{
    ["--arg1"] = "argument1",
    ["--arg2"] = "argument2",
    ["-a1"] = "argument1",
    ["-a2"] = "argument2",
};
// explicitly call AddCommandLine with the mappings
builder.Configuration.AddCommandLine(args, mappings);

// build the application
var app = builder.Build();
```

And the updated endpoint:

``` csharp
app.MapGet("/getarguments", (IConfiguration config) =>
{
    return new
    {
        Argument1 = config["argument1"],
        Argument2 = config["argument2"],
        Arg1 = config["arg1"],
        Arg2 = config["arg2"],
        a1 = config["a1"],
        a2 = config["a2"]
    };
});
```

`Short keys` also are used in the similar way to aliases, but with a `single dash` and a `space instead of equals (=)`:

``` powershell
dotnet run -a1 hello -a2 world
```

The response from the endpoint:

``` json
{
  "argument1": "hello",
  "argument2": "world",
  "arg1": null,
  "arg2": null,
  "a1": null,
  "a2": null
}
```

A single dash switch value cannot be access directly, as seen in the above results which reflect `null` when trying to access the values directly. The short keys have to be mapped to and accessed via the full key.

---

## Mix and match

The various techniques can be mixed and match when supplying the value to the command lines. All of the below options will yield the same (simplified) output:

``` powershell
 dotnet run --argument1=hello --arg2 world
 dotnet run -a1 hello --arg2=world
 dotnet run --arg1=hello -a2 world
```

The output

```json
{
  "argument1": "hello",
  "argument2": "world"
}
```

---

## Notes

If your application requires command line argument - consider configuring `alias keys` and `short keys`. They are simple to configure, and provide an easier, quicker and more convenient way to pass in arguments to the application for the end user.

---

## References

[CommandLineConfigurationExtensions.AddCommandLine Method](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.commandlineconfigurationextensions.addcommandline?view=dotnet-plat-ext-6.0#microsoft-extensions-configuration-commandlineconfigurationextensions-addcommandline(microsoft-extensions-configuration-iconfigurationbuilder-system-string()-system-collections-generic-idictionary((system-string-system-string))))   

<?# DailyDrop ?>165: 21-09-2022<?#/ DailyDrop ?>
