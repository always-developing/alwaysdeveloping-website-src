---
title: "Garbage collection configuration"
lead: "The .NET garbage collector can be configured at runtime"
Published: 02/18/2022
slug: "18-gc-variables"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - gc
    - garbagecollection
    - garbage
    
---

## Daily Knowledge Drop

There are a number of settings related to the .NET `garbage collector (GC)` which can be set at runtime using a variety of methods. Generally these do not need to be manually configured, but can be changed from the default to tweak and optimize performance of the application.

---

## Configurations

The settings which can be configured are listed below (more detailed information can be found under _references_ section below)

### GC flavour

- `Workstation or server GC`: should the application use workstation or server garbage collection.
- `Background GC`: configures if background (concurrent) garbage collection is enabled.

---

### Resource usage

- `Heap Count`: limits the number of heaps created by the garbage collector.
- `Affinitize ranges`: specifies thee list of processors to use for garbage collector threads.
- `CPU groups`: configures whether he garbage collector used CPU groups or not.
- `Affinitize`: Specifies whether to affinitize garbage collection threads with processors (To affinitize a GC thread means that it can only run on its specific CPU).
- `Heap limit`: Specifies the maximum commit size, in bytes, for the GC heap and GC bookkeeping.
- `Heap limit percent`: Specifies the allowable GC heap usage as a percentage of the total physical memory.
- `Per-object-heap limits`: Specifies the garbage collectors allowable heap usage on a per-object-heap basis. The different heaps are the large object heap (LOH), small object heap (SOH), and pinned object heap (POH).
- `Per-object-heap limit percents`: Specifies the garbage collectors allowable heap usage on a per-object-heap basis.
- `High memory percent`: The physical memory load percentage above which garbage collection becomes more aggressive about doing full, compacting garbage collections to avoid paging.
- `Retain VM`: Configures whether segments that should be deleted are put on a standby list for future use or are released back to the operating system (OS).

---

### Misc
- `Large pages`: Specifies whether large pages should be used when a heap hard limit is set.
- `Allow large objects`: Configures garbage collector support on 64-bit platforms for arrays that are greater than 2 gigabytes (GB) in total size.
- `Large object heap threshold`: Specifies the threshold size, in bytes, that causes objects to go on the large object heap (LOH).
- `Standalone garbage collector`: Specifies a path to the library containing the garbage collector that the runtime intends to load.

---

## Configuration methods

There are 4 different ways to set the garbage collector variables - however, `not all methods are available for all settings`.

Let's have a look at the `Workstation or server GC` setting as an example, which can be configured using any of the 4 methods.

|**Method**|**Setting name**|**Values**|**Version introduced**|
|------|------------|------|------------------|
|runtimeconfig.json|`System.GC.Server`|`false` - workstation / `true` - server|.NET Core 1.0|
|MSBuild property|`ServerGarbageCollection`|`false` - workstation / `true` - server|.NET Core 1.0|
|Environment variable|`COMPlus_gcServer`|`0` - workstation / `1` - server|.NET Core 1.0|
|Environment variable|`DOTNET_gcServer`|`0` - workstation / `1` - server|.NET 6|
|.NET Framework app.config|`<GCServer>` element|`false` - workstation / `true` - server||

---

## Notes

For most applications in typical situations, the default GC configuration should provide optimal performance. However if trying to achieve peak performance of a running application, these settings can be used (and benchmarked along the way)

Please see the references section for more information on each of the settings, which method can be used to set them as well as the valid values.

---

## References
[Runtime configuration options for garbage collection](https://docs.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector)

<?# DailyDrop ?>14: 18-02-2022<?#/ DailyDrop ?>
