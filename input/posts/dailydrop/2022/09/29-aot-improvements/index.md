---
title: "Native AOT compilation with .NET 7"
lead: "Using ahead-of-time compilation for improved application startup performance and memory usage"
Published: "09/29/2022 01:00:00+0200"
slug: "29-aot-improvements"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - aot
   - performance
   - aheadoftime

---

## Daily Knowledge Drop

New functionality is being introduced with .NET 7 adds the ability to do `native ahead-of-time (AOT)` compilation - this results in the application starting up faster, and using less memory than the traditional non-AOT alternative.

While native AOT is supported in .NET 7, only a limited number of libraries are fully compatible and the functionality is currently targeted towards console-type applications, not web applications.

---

## AOT

Publishing an application with native AOT compilation produces an app that is self-contained and that has been ahead-of-time (AOT) compiled to the native code of the architecture specified.  
When an application is not using AOT, then _just-in-time_ compilation is used, where the code is compiled into native machine code on the fly as it is being used.

From the official documentation:  
_The benefit of native AOT is most significant for workloads with a high number of deployed instances, such as cloud infrastructure and hyper-scale services. It is currently not supported with ASP.NET Core, but only console apps._

AOT in the broader sense is entirely not new to .NET, which currently makes use of [ReadyToRun Compilation](https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run) which is a form of AOT.

---

## Publish

Let's test the AOT functionality on a simple .NET 7 console application which writes a message to the console, and then waits for a key to be pressed:

``` csharp
// the entire application
Console.WriteLine("Learning about Native AOT with alwaysdeveloping.net");
Console.ReadKey();
```

---

### Non AOT

To start, we'll get a base benchmark, without using AOT:

``` powershell
dotnet publish -c Release
```

The output is a `149 KB` _AlwaysDeveloping.exe_ which took `3 seconds to compile`.

---

#### Runtime specified

_Native AOT_ requires a specific runtime architecture be specified, so let's add that into the publish command to get a better comparison. When specifying the _runtime_ architecture, _self-contained_ or _no-self-contained_ argument needs to also be specified. AOT compiled code will be self contained, so we can specify the publish as _self-contained_ as well:

``` powershell
dotnet publish -c Release -r win-x64 --self-contained
```

The output is a `67 MB folder`, containing _AlwaysDeveloping.exe, as well as all other dependencies_ required to run the application, and took `1.5 seconds to compile`.

---

#### Single file

When using _Native AOT_, the output will be a single file, so next we include that:

``` powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
```

The output is now a `62 MB` AlwaysDeveloping.exe file, which took `3.3 seconds to compile`.

---

#### Trimming

Next up, _Native AOT_, will perform _trimming_ (removal of specific portions of code never called):

``` powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
```

The output is now a `11 MB` AlwaysDeveloping.exe file, which took `5 seconds to compile.`

---

#### ReadyToRun

The final step before comparing to _Native AOT_ is to specify the existing form of AOT, `ReadyToRun`:

``` powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishReadyToRun=true --self-contained true
```

The output is now a `14 MB` AlwaysDeveloping.exe file, which took `10 seconds to compile`.

---

### AOT


FInally let's compile using _Native AOT_:

``` powershell
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

The output is now a `3.6 MB` AlwaysDeveloping.exe file, which took `3-12 seconds to compile` (I ran this a few times, and the compile time was variable between 3 and 12 seconds)

---

## Limitations

There are [limitations](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot#limitations-of-native-aot-deployment) when working with _native AOT_ which need to be kept in mind, and may limit the ability to leverage the _native AOT_ functionality:
- No dynamic loading
- No runtime code generation
- Trimming is required (quick has its own limitations)
- Single file is required (which has known incompatibilities)

---

## Notes

Using `Native AOT` compilation results in a **much** smaller output compared with the equivalent (as much as possible) configuration using non-AOT (3.6MB vs 14MB). With the improved startup performance and memory usage, if the limitations allow it, _Native AOT_ should definitely be considered. This is just the start, and I am sure there will be more improvements to come with future releases.

---

## References

[Native AOT Deployment](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot)   
[Trying out Native AOT in .NET 7 Preview 7](https://code.soundaranbu.com/trying-out-native-aot-in-net-7-preview-7/)   

<?# DailyDrop ?>171: 29-09-2022<?#/ DailyDrop ?>
