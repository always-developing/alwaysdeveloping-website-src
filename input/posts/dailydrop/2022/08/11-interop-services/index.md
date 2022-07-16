---
title: "OS information via Runtime.InteropServices"
lead: "Levering Runtime.InteropServices to execute OS specific functionality"
Published: "08/10/2022 01:00:00+0200"
slug: "11-interop-services"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - interop
   - os

---

## Daily Knowledge Drop

The `Runtime.InteropServices` namespace contains the `RuntimeInformation` static class which can be used to obtain a variety of information related to the operating system. This can be used to only execute certain code depending on the operating system on which the application is running.

---

## Basic Information

The basic operating system information is all available on the _static_ `RuntimeInformation` class and can be accessed via public properties:

``` csharp
Console.WriteLine($"Framework description: {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"OS architecture: {RuntimeInformation.OSArchitecture}");
Console.WriteLine($"OS description: {RuntimeInformation.OSDescription}");
Console.WriteLine($"Process architecture: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"Runtime identifier: {RuntimeInformation.RuntimeIdentifier}");
```

Executing the code in _Windows_:

``` terminal
Framework description: .NET 6.0.5
OS architecture: X64
OS description: Microsoft Windows 10.0.22000
Process architecture: X64
Runtime identifier: win10-x64
```

Executing the code on _WSL (Windows Subsystem for Linux)_:

``` terminal
Framework description: .NET 6.0.4
OS architecture: X64
OS description: Linux 5.10.16.3-microsoft-standard-WSL2 #1 SMP Fri Apr 2 22:23:49 UTC 2021
Process architecture: X64
Runtime identifier: ubuntu.20.04-x64
```

---

## Operating System

The namespace also contains a `OSPlatform` _struct_, which contains a list of common operating systems, and in conjunction with the `IsOSPlatform` method on `RuntimeInformation`, can be used to check the current operating system:

``` csharp
Console.WriteLine($"Is Windows?: {RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}");
Console.WriteLine($"Is Linux?: {RuntimeInformation.IsOSPlatform(OSPlatform.Linux)}");
```

Executing the code in _Windows_:

``` terminal
Is Windows?: True
Is Linux?: False
```

Executing the code on _WSL (Windows Subsystem for Linux)_:

``` terminal
Is Windows?: False
Is Linux?: True
```

This can also be used to executing specific code only when running on a certain operating system. Suppose we need to write to the Windows Event Log - this can only happen in Windows (there are better ways of handling logging, but for demo purposes, this is the requirement):

``` csharp
if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("Performing some Windows specific stuff - maybe writing to EventLog for example");
}
```

Simple and  easy to use.

---

## Notes

A useful namespace and class to be aware of, especially if doing cross platform development, or if just required to gather information about the operating system.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1541104583728136192)   

---

<?# DailyDrop ?>136: 11-08-2022<?#/ DailyDrop ?>
