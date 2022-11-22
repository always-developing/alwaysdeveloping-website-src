---
title: "Opting into experimental functionality"
lead: "Using the RequiresPreviewFeatures attribute to create preview functionality"
Published: "11/25/2022 01:00:00+0200"
slug: "25-preview-attribute"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - attribute
   - preview

---

## Daily Knowledge Drop

The `RequiresPreviewFeatures` attribute can be used to flag a specific piece of code as _experimental_ or _in preview_ - this code cannot be used unless the consumer specifically opts into _enabling preview features_, preventing the preview, potentially unstable code, from being used unwittingly. 

---

## Preview feature

### Code

To mark a _method_ (or class, property etc) as _in preview_, it is decorated with the `RequiresPreviewFeatures` attribute. Here the _DoWorkNew_ method is flagged as _in preview_:

``` csharp
public class Worker
{
    public void DoWork()
    {
        // do some work the old way
    }

    [RequiresPreviewFeatures()]
    public void DoWorkNew() 
    { 
        // do some work the new one
    }
}
```

As it stands, trying to use this method:

``` csharp
var worker = new Worker();

worker.DoWorkNew();
```

will result in the following compiler error:

``` terminal
Using 'DoWorkNew' requires opting into preview features. See https://aka.ms/dotnet-warnings/preview-features for more information.	
```

To to able to use code marked with the attribute, one specifically needs to opt into _preview features_.

---

### Project

To opt into _preview features_, in the _csproj_ file ensure the `EnablePreviewFeatures` setting is set to true:

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <EnablePreviewFeatures>true</EnablePreviewFeatures>
  </PropertyGroup>
</Project>
```

With this the code will now compile successfully and be able to leverage preview/experimental features.

---

## Notes

As a library author this is a very useful tool - allowing new experimental functionality to be introduced "safely". Usage of the functionality is semi-controlled, and the consumers are required to make an informed choice to manually opt into using potentially unstable code. 

---


## References

[Marking API's as obsolete or as experimental](https://steven-giesel.com/blogPost/1d97ea56-9a32-4067-9919-10b9af5623a6)  

<?# DailyDrop ?>210: 25-11-2022<?#/ DailyDrop ?>
