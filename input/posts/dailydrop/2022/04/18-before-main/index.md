---
title: "Executing code before Main"
lead: "Exploring the ways code can be execute before the Main method is called"
Published: 04/18/2022
slug: "18-before-main"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - main
    - execution

---

## Daily Knowledge Drop

In C#, the `Main method` is the entry point of an application - however there are a few scenarios where it is possible to have code executed `before the Main method`

---

## Base application

Below is the base application we will build on. This is a simple console application which outputs to the console when the _Main_ method is called.

``` csharp
public class Program
{
    public static void Main()
    {
        Console.WriteLine("App started => Main");
    }
}
```

The output when the application is run:

``` powershell
    App started => Main
```

---

## Before Main
### Startup hook

There is an environment variable, `DOTNET_STARTUP_HOOKS` which can be set to execute code before the application's _Main_ method is called.

This environment variable can contain a semi-colon separated list of dll paths. These dll's need to contain class(es) which conform to a specific set of rules:
- The class must be outside of any namespace
- The class must contain a static method called _Initialize_. This method is called on startup of the application.

Below, a class has been defined in a **_separate_** project, which outputs to _StartupHook.dll_.

``` csharp
internal class StartupHook
{
    static StartupHook() => Console.WriteLine("StartupHook => static constructor");

    public static void Initialize() => Console.WriteLine("StartupHook => Initialize");
}
```

When executing the base application, the environment variable `DOTNET_STARTUP_HOOKS` can now be set to the full path of the _StartupHook.dll_

Below is a example of the _launchSettings.json_ file

``` json
{
  "profiles": {
    "BeforeMain": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_STARTUP_HOOKS": "C:\\StartupHook.dll"
      }
    }
  }
}
```

The output of the base application is now:

``` powershell
    StartupHook => static constructor
    StartupHook => Initialize
    App started => Main
```

---

### ModuleInitializer

Static method(s) can be decorated with the `ModuleInitializer` attribute, which indicates to the compiler these methods should be invoked during the containing application's initialization (start-up).

In the same project as the _Program_ class above:

``` csharp
public class ModInit
{
    public ModInit()
    {
        Console.WriteLine("ModInit => constructor");
    }

    static ModInit()
    {
        Console.WriteLine("ModInit => static constructor");
    }

    [ModuleInitializer]
    public static void ExecuteInitCode() => 
        Console.WriteLine("ModInit => ExecuteInitCode");
}
```

The output when the application is run is now as follows:

``` powershell
    StartupHook => static constructor
    StartupHook => Initialize
    ModInit => static constructor
    ModInit => ExecuteInitCode
    App started => Main
```

---

### Static Program constructor

A static constructor on the _Program_ class (the class containing the _Main_ method) can also be used to execute code before the _Main_ method: 

``` csharp
public class Program
{
    static Program()
    {
        Console.WriteLine("Program => static constructor");
    }

    public static void Main()
    {
        Console.WriteLine("App started => Main");
    }
}
```

The output when the application is run is now as follows:

``` powershell
    StartupHook => static constructor
    StartupHook => Initialize
    ModInit => static constructor
    ModInit => ExecuteInitCode
    Program => static constructor
    App started => Main
```

---

## Notes

Today we looked at the various ways code can be executed before _Main_ and the order in which the various code is executed. There are a number of different options depending on the use case, from executing code in an external DLL, initializing modules in the same code base, or just having the _Program_ constructor execute first.

---

## References

[Executing code before Main in .NET](https://www.meziantou.net/executing-code-before-main-in-dotnet.htm)  

<?# DailyDrop ?>54: 18-04-2022<?#/ DailyDrop ?>
