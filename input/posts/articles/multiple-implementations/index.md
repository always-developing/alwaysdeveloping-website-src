---
title: "Multiple implementations of same interface - the options"
lead: "Different methods of registering the same interface with multiple implementations using .NET dependency injection"
Published: 11/06/2021
slug: "multiple-implementations"
draft: false
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - dependency injection
    - DI
    - dependency
    - injection
    - rollcall
    - transient
    - singleton
    - scoped
    - multiple implementation
    - implementation
    - interface
    - multiple
---

## The challenge

Consider a scenario where the requirement is to upload a file to an online provider (AWS S3, Azure Blob or a FTP site in the examples below), where the provider can be easily changed (either dynamically at runtime, or easily with minimal code changes), with the possibility additional providers being added in future.  

To make use of dependency injection, a generic interface is created, **_IFileUploader_**, along with three implementations **_AWSUploader_**, **_AzureUploader_** and **_FTPUploader_**. The interface prescribes that the implementations provide a method to upload a file (_UploadFile_) and a method to get the implementation name (_GetName_).

The built in .NET dependency injection (DI) container is all one will need for almost all situations (including this situation): however this scenario can be a bit more challenging to get right - **`with multiple implementations of the same interface, how do you get the right implementation from the DI container?`**

 ## The problem with .NET dependency injection container

One piece of functionality the .NET DI container does not have (which is available in some other 3rd party DI/IoC containers) is the ability to add and retrieve service implementations by name. 

Short of actually implementing one of these other 3rd party containers, below are a number of different options and techniques one can use to get the correct implementation from the DI container when there are multiple implementations registered.

<?# InfoBlock ?>
The benchmarks on the below techniques were all executed at the same time under the same conditions using [**BenchmarkDotNet**](https://benchmarkdotnet.org/articles/overview.html)  
Even though some some techniques performed poorly when compared to others, bear in mind that the time frame in question here is nanoseconds (a nanosecond is **one billionth of a second**).  
In _most_ scenarios, the DI technique used (if used correctly) is not going to make a massive material different to the performance of the application/service (of course there are exceptions, depending on how complicated the dependency tree is)
<?#/ InfoBlock ?>

## The different techniques
### IEnumerable

- **Configuration:**  
This is the simplest 'out of the box' technique, with the various implementations just all added to the DI container using the same interface:
``` csharp
private readonly IHost host;
public EnumerableBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddTransient<EnumerableHandler>()
            .AddTransient<IFileUploader, AWSUploader>()
            .AddTransient<IFileUploader, AzureUploader>()
            .AddTransient<IFileUploader, FTPUploader>()
        ).Build();
}
```

- **Usage:**  
Inject IEnumerable<Interface> into the relevant class (EnumerableHandler constructor), and then retrieve the required implementation from the IEnumerable collection:

``` csharp
public class EnumerableHandler
{
    private readonly IEnumerable<IFileUploader> _uploaders;
    public EnumerableHandler(IEnumerable<IFileUploader> uploaders)
    {
        _uploaders = uploaders;
    }

    public void Execute()
    {
        var providerName = "aws";
        var uploader = _uploaders.FirstOrDefault(up => up
            .GetName().Equals(providerName));

        if (uploader == null)
        {
            throw new ArgumentException($"No uploader with name " +
                    $"{providerName} could be found");
        }
        uploader.UploadFile();
    }
}
```

- **Pros:**
  - Easy to implement
  - Implementation can be selected/changed at runtime 

- **Cons:**
  - Every implementation is instantiated (as part of IEnumerable) even if not required or used. This could be especially problematic if the implementations themselves have a number of dependencies which then need to be instantiated (this was NOT the case with the benchmarks) which could result in a negative performance impact.
  - The logic to retrieve the implementation from IEnumerable is contained in multiple places (each class which has it injected)

- **Performance:**


|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |

---

### Factory
One of the negative aspects of the `IEnumerable` approach, is that the logic to retrieve the correct implementation could be present in multiple places (if IEnumberable is injected into multiple classes). The `Factory` approach moves the logic into a separate actory class, which is then injected and is responsible for retrieving the required implementation.

- **Configuration:**  
Configuration is the _same as `IEnumerable`_, the various implementations all added to the DI container using the same interface, with one additional class added, the factory class:

``` csharp
private readonly IHost host;
public FactoryBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddTransient<FactoryHandler>()
            .AddTransient<FileUploaderFactory>()
            .AddTransient<IFileUploader, AWSUploader>()
            .AddTransient<IFileUploader, AzureUploader>()
            .AddTransient<IFileUploader, FTPUploader>()
        ).Build();
}
```

The factory looks very similar to the handler from the `IEnumerable` approach:

``` csharp
public class FileUploaderFactory
{
    private readonly IEnumerable<IFileUploader> _uploaders;
    public FileUploaderFactory(IEnumerable<IFileUploader> uploaders)
    {
        _uploaders = uploaders; 
    }

    public IFileUploader Resolve(string providerName)
    {
        var uploader = _uploaders.FirstOrDefault(up => up
            .GetName().Equals(providerName));
        if (uploader == null)
        {
            throw new ArgumentException($"No uploader with name " +
                    $"{providerName} could be found");
        }

        return uploader;
    }
}
```

- **Usage:**  
The factory is now injected into the relevant class and is then invoked to get the requested implementation:

``` csharp
public  class FactoryHandler
{
    private readonly FileUploaderFactory _factory;
    public FactoryHandler(FileUploaderFactory factory)
    {
        _factory = factory;
    }

    public void Execute()
    {
        var providerName = "azure";
        var uploader = _factory.Resolve(providerName);
        uploader.UploadFile();
    }
}
```

- **Pros:**
  - Easy to implement
  - Implementation can be selected/changed at runtime
  - Retrieval logic is contained in a single place

- **Cons:**
  - Every implementation is instantiated (as part of IEnumerable) even if not required or used. This could have an impact on performance and memory usage.  
  - Slightly slower, and slightly more memory usage than the `IEnumerable` approach (due to the extra layer between the handler and the IEnumerable collection)

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |

---

### Type Factory
A big negative aspect of the `IEnumerable` and `Factory` approach, is that all the implementations are instantiated every time, even if not used or required. This could have big impact on performance and memory if the implementations them themselves have many dependencies (and those dependencies have dependencies and so on).  
The next approach is extends on the `Factory` technique, but only instantiates the requested implementation **based on naming conventions**.

- **Configuration:**  
Setup is the same as with the `Factory` method:

``` csharp
private readonly IHost host;
public FactoryBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddTransient<TypeFactoryHandler>()
            .AddTransient<FileUploaderTypeFactory>()
            .AddTransient<IFileUploader, AWSUploader>()
            .AddTransient<IFileUploader, AzureUploader>()
            .AddTransient<IFileUploader, FTPUploader>()
        ).Build();
}
```


The factory in this approach, takes the requested name, finds the type based on the name and gets it from the DI container:

``` csharp
public class FileUploaderTypeFactory
{
    private readonly IServiceProvider _provider;
    public FileUploaderTypeFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IFileUploader Resolve(string providerName)
    {
        var type = Assembly.GetAssembly(typeof(FileUploaderTypeFactory)).GetType(
            $"{typeof(FileUploaderTypeFactory).Namespace}.{providerName}Uploader");

        if (type == null)
        {
            throw new ArgumentException($"No uploader with name " +
                    $"{providerName} could be found");
        }

        var uploader = _provider.GetService(type);
        return uploader as IFileUploader;
    }
}
```

- **Usage:**  
The factory is now injected into the relevant class and is then invoked to get the requested implementation:

``` csharp
public  class TypeFactoryHandler
{
    private readonly FileUploaderTypeFactory _factory;
    public TypeFactoryHandler(FileUploaderTypeFactory factory)
    {
        _factory = factory;
    }

    public void Execute()
    {
        var providerName = "Azure";
        var uploader = _factory.Resolve(providerName);
        uploader.UploadFile();
    }
}
```

- **Pros:**
  - Not all implementations are instantiated
  - Better memory usage compared to other two approaches so far
  - Implementation can be selected/changed at runtime
  - Retrieval logic is contained in a single place
  
- **Cons:**
  - Use of reflection to convert the name to a Type does have an big impact on performance
  - Strict naming convention has to be followed in order for the reflection logic to work correctly

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|           TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |

---

### Delegate
The next approach tries to achieve the same as the `Type Factory` approach - not instantiating every implementation, but using a different technique.  
In short, a delegate is called at runtime when an implementation is requested, and using a switch statement the correct one is determined and returned.

- **Configuration:** 

``` csharp
private readonly IHost host;
public DelegateBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddScoped<AWSUploader>()
            .AddScoped<AzureUploader>()
            .AddScoped<FTPUploader>()
            .AddTransient<DelegateHandler>()
            .AddTransient<DelegateResolver>(serviceProvider => providerName =>
            {
                switch (providerName)
                {
                    case "aws":
                        return serviceProvider.GetService<AWSUploader>();
                    case "azure":
                        return serviceProvider.GetService<AzureUploader>();
                    case "ftp":
                        return serviceProvider.GetService<FTPUploader>();
                    default:
                        throw new ArgumentException($"No uploader with " +
                                $"name {providerName} could be found");
                }
            })).Build();
}
```

The DelegateResolver is as follows:
``` csharp
    public delegate IFileUploader DelegateResolver(string providerName);
```

- **Usage:**

The delegate is now injected into the relevant class  and is then invoked to get the requested implementation:

``` csharp
public class DelegateHandler
{
    private readonly DelegateResolver _resolver;
    public DelegateHandler(DelegateResolver resovler)
    {
        _resolver = resovler;
    }

    public void Execute()
    {
        var uploader = _resolver("ftp");
        uploader.UploadFile();
    }

}
```

- **Pros:**
  - Not all implementations are instantiated
  - Best memory usage compared to other approaches so far
  - Implementation can be selected/changed at runtime
  - Retrieval logic is contained in a single place
  
- **Cons:**
  - Slightly more complicated setup with the delegate and switch statement compared to other approaches
  - Switch statement is hardcoded and needs to be manually maintained every time a new provider is added

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|           TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|              Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |

---

### Type Delegate
The next approach extends the `Delegate` technique, and uses reflection and naming conventions to get the Type dynamically.

- **Configuration:**  
Setup is as follows, very similar to the `Delegate` approach, but instead of the switch statement, reflection is used to get the Type based on naming conventions:

``` csharp
private readonly IHost host;
public TypeDelegateBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddScoped<AWSUploader>()
            .AddScoped<AzureUploader>()
            .AddScoped<FTPUploader>()
            .AddTransient<TypeDelegateHandler>()
            .AddTransient<TypeDelegateResolver>(serviceProvider => providerName =>
            {
                var type = Assembly.GetAssembly(typeof(FileUploaderTypeFactory))
                    .GetType($"{typeof(FileUploaderTypeFactory).Namespace}
                    .{providerName}Uploader", false, true);

                if (type == null)
                {
                    throw new ArgumentException($"No uploader with " +
                                $"name {providerName} could be found");
                }

                var uploader = serviceProvider.GetService(type);
                return uploader as IFileUploader;

            })).Build();
}
```

The DelegateResolver is the same as before:
``` csharp
    public delegate IFileUploader DelegateResolver(string providerName);
```

- **Usage:**  
The delegate is now injected into the relevant class and is then invoked to get the requested implementation:

``` csharp
public class TypeDelegateHandler
{
    private readonly DelegateResolver _resolver;
    public TypeDelegateHandler(DelegateResolver resovler)
    {
        _resolver = resovler;
    }

    public void Execute()
    {
        var uploader = _resolver("ftp");
        uploader.UploadFile();
    }

}
```

- **Pros:**
  - Not all implementations are instantiated
  - Implementation can be selected/changed at runtime
  - Retrieval logic is contained in a single place
  - No switch statement to maintain when a new provider is added
  
- **Cons:**
  - Use of reflection to convert the name to a Type does have a large impact on performance
  - Strict naming convention has to be followed in order for the reflection logic to work correctly

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|           TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|              Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|          TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |

---

### Distinct
The next technique uses a wrapper to make each implementation added to the DI container unique, and hence can be retrieved uniquely.

- **Configuration:**  
Additional types are also now required to be defined and added to the DI container, _IGenericUploader_ and _GenericUploader_:

``` csharp
private readonly IHost host;
public DistinctBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddScoped<AWSUploader>()
            .AddScoped<AzureUploader>()
            .AddScoped<FTPUploader>()
            .AddTransient<DistinctHandler>()
            .AddScoped<IGenericUploader<AWSUploader>, GenericUploader<AWSUploader>>()
            .AddScoped<IGenericUploader<AzureUploader>, GenericUploader<AzureUploader>>()
            .AddScoped<IGenericUploader<FTPUploader>, GenericUploader<FTPUploader>>()
        ).Build();
}
```

IGenericUploader is defined as below:
``` csharp
    public interface IGenericUploader<T> : IFileUploader where T : IFileUploader { }
```

GenericUploader is defined as below:
``` csharp
public class GenericUploader<T> : IGenericUploader<T> where T : IFileUploader
{
    private readonly T _implementation;
    public GenericUploader(T implementation)
    {
        _implementation = implementation;
    }

    public string GetName()
    {
        return _implementation.GetName();
    }

    public void UploadFile()
    {
        _implementation.UploadFile();
    }
}
```

A new generic provider is defined (implementing the relevant interface) and the generic provider wraps the "true provider" implementation. As the generic implementation takes a T argument, this can be used to uniquely distinguish them and retrieve the correct implementation.

- **Usage:**  
The generic interface with the required implementation is now injected into the relevant class and is then invoked:

``` csharp
public class DistinctHandler
{
    private readonly IGenericUploader<AWSUploader> _uploader;
    public DistinctHandler(IGenericUploader<AWSUploader> uploader)
    {
        _uploader = uploader;
    }

    public void Execute()
    {
        _uploader.UploadFile();
    }
}
```

- **Pros:**
  - Not all implementations are instantiated
  - The default DI container is doing all the retrieval work (as a unique item is being asked for), so is very efficient
  - By far the best performing (in both time and memory usage) technique so far
  
- **Cons:**
  - Implementation can NOT be selected/changed at runtime
  - Bit of a convoluted process having a wrapper interface

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|           TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|              Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|          TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |
|              Distinct | Execute |  50.78 ns | 0.441 ns | 0.413 ns |  0.58 |    0.01 | 0.0038 |      24 B |

---

### Distinct Factory
This technique extends the `Distinct` approach, resolving the limitation of not being able to select or change the implementation at runtime.

- **Configuration:**  
Setup very similar to the `Distinct` setup, with the addition of the DistinctFactory:

``` csharp
private readonly IHost host;
public DistinctFactoryBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddScoped<AWSUploader>()
            .AddScoped<AzureUploader>()
            .AddScoped<FTPUploader>()
            .AddTransient<DistinctFactory>()
            .AddTransient<DistinctFactoryHandler>()
            .AddScoped<IGenericUploader<AWSUploader>, GenericUploader<AWSUploader>>()
            .AddScoped<IGenericUploader<AzureUploader>, GenericUploader<AzureUploader>>()
            .AddScoped<IGenericUploader<FTPUploader>, GenericUploader<FTPUploader>>()
        ).Build();
}
```

IGenericUploader and GenericUploader are exactly as defined in the `Distinct` technique.

DistinctFactoryHandler is defined as below:

``` csharp
public class DistinctFactory
{
    private readonly IServiceProvider _provider;
    public DistinctFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IFileUploader Resolve(string providerName)
    {
        switch (providerName)
        {
            case "aws":
                return _provider.GetService(typeof(
                    IGenericUploader<AWSUploader>)) as IFileUploader;
            case "azure":
                return _provider.GetService(typeof(
                    IGenericUploader<AzureUploader>)) as IFileUploader;
            case "ftp":
                return _provider.GetService(typeof(
                    IGenericUploader<FTPUploader>)) as IFileUploader;
            default:
                throw new ArgumentException($"No uploader with " +
                               $"name {providerName} could be found");
        }
    }
}
```

- **Usage:**  
The factory is now injected into the relevant class and is then invoked to get the requested implementation by name:

``` csharp
 public class DistinctFactoryHandler
{
    private readonly DistinctFactory _distinctFactory;
    public DistinctFactoryHandler(DistinctFactory distinctFactory)
    {
        _distinctFactory = distinctFactory;
    }

    public void Execute()
    {
        _distinctFactory.Resolve("ftp").UploadFile();
    }

}
```

- **Pros:**
  - Not all implementations are instantiated
  - Implementation can be selected/changed at runtime
  - Good overall performance
  
- **Cons:**
  - Switch statement is hardcoded and needs to be manually maintained every time a new provider is added
  - Bit of a convoluted process

- **Performance:**

|                  Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|            Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|               Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|           TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|              Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|          TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |
|              Distinct | Execute |  50.78 ns | 0.441 ns | 0.413 ns |  0.58 |    0.01 | 0.0038 |      24 B |
|       DistinctFactory | Execute |  96.22 ns | 1.378 ns | 1.289 ns |  1.11 |    0.02 | 0.0076 |      48 B |

---

### Distinct Lookup Factory
This approach gives implementations names as they are added to the DI container, keeps track of the name-implementation link, and facilitates lookup and retrieval of the correct implementation.

- **Configuration:**  
This setup is different, in that implementations of the same interface are grouped together by the AddNamedUploader extension method, and as implementations are added, they are given a name:

``` csharp
private readonly IHost host;
public DistinctLookupFactoryBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddNamedUploader<IFileUploader>(builder => builder
                .AddTransient("aws", typeof(AWSUploader))
                .AddTransient("azure", typeof(AzureUploader))
                .AddTransient("ftp", typeof(FTPUploader))
            )
            .AddTransient<DistinctLookupFactoryHandler>()
        ).Build();
}
```

There are a number of new components here:
1. AddNamedUploader extension method: this will setup base functionality required as expose the UploaderBuilder as a parameter
1. builder, of type UploaderBuilder: this is an Action which handles keeping track of the name-implementation link.
1. AddTransient extension method: this is **not** the same as the normal AddTransient method on IServiceCollection, but an extension method on the builder (UploaderBuilder) which wraps the usual .NET AddTransient method.

The full definition of the classes (along with all other code) can be found on [Github, here](https://github.com/always-developing/Rollcall.Extensions.Microsoft.DependencyInjection/tree/main/benchmark/MultiImplementationBenchark/8.DistinctLookupFactory)

In summary though, it works as follows:
1. _AddNamedUploader_ creates an instance of _UploaderTypes_, which keeps track of the name and the implementation Type.  _UploaderTypes_ is added to the DI container as a singleton:

``` csharp
public static IServiceCollection AddNamedUploader<T>(
    this IServiceCollection services, 
    Action<UploaderBuilder<T>> builder) where T : class
{
    var uploaderType = new UploaderTypes<T>();
    services.AddSingleton(uploaderType);
    services.AddTransient(typeof(DistinctLookupFactory<T>));

    builder.Invoke(new UploaderBuilder<T>(services, uploaderType));

    return services;
}
```

2. The _AddTransient_ method will add records to the _UploaderTypes_ class, as well as add the implementation to the DI container:

``` csharp
public static UploaderBuilder<T> AddTransient<T>(
    this UploaderBuilder<T> builder, 
    string name, 
    Type implementation) where T : class
{
    builder.Types.Add(name, implementation);
    builder.Services.AddTransient(implementation);

    return builder;
}
```

- **Usage:**  
The factory is now injected into the relevant class for a specific interface, and is then invoked to get the requested implementation by name:

``` csharp
public class DistinctLookupFactoryHandler
{
    private readonly DistinctLookupFactory<IFileUploader> _distinctFactory;
    public DistinctLookupFactoryHandler(
        DistinctLookupFactory<IFileUploader> distinctFactory)
    {
        _distinctFactory = distinctFactory;
    }

    public void Execute()
    {
        _distinctFactory.Resolve("ftp").UploadFile();
    }
}
```

- **Pros:**
  - Not all implementations are instantiated
  - Implementation can be selected/changed at runtime
  - Good overall performance
  - No hard coded switch statement which needs to be maintained
  
- **Cons:**
  - The most complicated to setup, with the most moving parts

- **Performance:**

|                   Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|----------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|             Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|                Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|            TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|               Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|           TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |
|               Distinct | Execute |  50.78 ns | 0.441 ns | 0.413 ns |  0.58 |    0.01 | 0.0038 |      24 B |
|        DistinctFactory | Execute |  96.22 ns | 1.378 ns | 1.289 ns |  1.11 |    0.02 | 0.0076 |      48 B |
|  DistinctLookupFactory | Execute |  92.96 ns | 0.764 ns | 0.714 ns |  1.07 |    0.01 | 0.0126 |      80 B |

---

### Rollcall
`Rollcall` is a library (written by me) which extends the `DistinctLookupFactory` approach and makes it generic so that it will function with any interface and implementation. [Rollcall is available on Nuget](https://www.nuget.org/packages/Rollcall.Extensions.Microsoft.DependencyInjection/) 

- **Configuration:**  
The setup is almost identical to the `DistinctLookupFactory`, but without the need for the factory, as this is built into the `Rollcall` library:

``` csharp
private readonly IHost host;
public RollcallBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddTransient<RollcallHandler>()
            .AddNamedService<IFileUploader>(builder => builder
                .AddTransient("aws", typeof(AWSUploader))
                .AddTransient("azure", typeof(AzureUploader))
                .AddTransient("ftp", typeof(FTPUploader))
            )
        ).Build();
}
```
    
- **Usage:**  
The Rollcall provider/factory is now injected into the relevant class for a specific interface, and is then invoked to get the requested implementation by name:

``` csharp
public class RollcallHandler
{
    private readonly IRollcallProvider<IFileUploader> _provider;
    public RollcallHandler(IRollcallProvider<IFileUploader> provider)
    {
        _provider = provider;
    }

    public void Execute()
    {
        var providerName = "aws";
        var uploader = _provider.GetService(providerName);
        uploader.UploadFile();
    }
}
```
Not shown above, but one could also inject IServiceProvider and used the provided GetService extension method to get the service by name.

- **Pros:**
  - Not all implementations are instantiated
  - Implementation can be selected/changed at runtime
  - Good overall performance
  - No hard coded switch statement which needs to be maintained
  - Works with any interface + implementation, and provides all functionality out the box
  
- **Cons:**
  - Slight performance overhead when compared to the non-generic method

- **Performance:**

|                   Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|----------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|             Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|                Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|            TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|               Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|           TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |
|               Distinct | Execute |  50.78 ns | 0.441 ns | 0.413 ns |  0.58 |    0.01 | 0.0038 |      24 B |
|        DistinctFactory | Execute |  96.22 ns | 1.378 ns | 1.289 ns |  1.11 |    0.02 | 0.0076 |      48 B |
|  DistinctLookupFactory | Execute |  92.96 ns | 0.764 ns | 0.714 ns |  1.07 |    0.01 | 0.0126 |      80 B |
|               Rollcall | Execute | 124.52 ns | 1.485 ns | 1.389 ns |  1.43 |    0.02 | 0.0076 |      48 B |

---

### Rollcall with Func
`Rollcall` can also be used with a implementation factory, a Func<IServiceProvider,object> method. This method is called when requesting the implementation by name from the DI container. [Available on NuGet.](https://www.nuget.org/packages/Rollcall.Extensions.Microsoft.DependencyInjection/) 

- **Configuration:**  
The setup is a little more complicated than before, as some of the configuration needs to be done manually (instead of by the `Rollcall` package):

``` csharp
private readonly IHost host;
public RollcallFuncBenchmark()
{
    host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) => services
            .AddTransient<RollcallFuncHandler>()
            .AddTransient<AWSUploader>()
            .AddTransient<AzureUploader>()
            .AddTransient<FTPUploader>()
            .AddNamedService<IFileUploader>(builder => builder
                .AddTransient("aws", sp => sp.GetService(typeof(AWSUploader)))
                .AddTransient("azure", sp => sp.GetService(typeof(AzureUploader)))
                .AddTransient("ftp", sp => sp.GetService(typeof(FTPUploader)))
            )).Build();
}
```
    
- **Usage:**  
The usage is exactly the same with the Func<> as with the normal interface + implementation (as shown above):

``` csharp
public class RollcallFuncHandler
{
    private readonly IRollcallProvider<IFileUploader> _provider;
    public RollcallFuncHandler(IRollcallProvider<IFileUploader> provider)
    {
        _provider = provider;
    }

    public void Execute()
    {
        var providerName = "aws";
        var uploader = _provider.GetService(providerName);
        uploader.UploadFile();
    }
}
```
Not shown above, but one could also inject IServiceProvider and used the provided GetService extension method to get the service by name.

- **Pros:**
  - Not all implementations are instantiated
  - Implementation can be selected/changed at runtime
  - Good overall performance
  - No hard coded switch statement which needs to be maintained
  - Works with any interface + func<>, and provides all functionality out the box
  
- **Cons:**
  - Slight performance overhead when compared to the non-generic method, and when compared 
    to the interface + implementation method.

- **Performance:**

|                   Type |  Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|----------------------- |-------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|             Enumerable | Execute |  86.99 ns | 0.987 ns | 0.924 ns |  1.00 |    0.00 | 0.0421 |     264 B |
|                Factory | Execute | 103.20 ns | 1.324 ns | 1.238 ns |  1.19 |    0.02 | 0.0459 |     288 B |
|            TypeFactory | Execute | 525.19 ns | 2.624 ns | 2.455 ns |  6.04 |    0.07 | 0.0277 |     176 B |
|               Delegate | Execute | 111.45 ns | 1.456 ns | 1.291 ns |  1.28 |    0.02 | 0.0178 |     112 B |
|           TypeDelegate | Execute | 861.84 ns | 6.599 ns | 5.850 ns |  9.90 |    0.15 | 0.0343 |     216 B |
|               Distinct | Execute |  50.78 ns | 0.441 ns | 0.413 ns |  0.58 |    0.01 | 0.0038 |      24 B |
|        DistinctFactory | Execute |  96.22 ns | 1.378 ns | 1.289 ns |  1.11 |    0.02 | 0.0076 |      48 B |
|  DistinctLookupFactory | Execute |  92.96 ns | 0.764 ns | 0.714 ns |  1.07 |    0.01 | 0.0126 |      80 B |
|               Rollcall | Execute | 124.52 ns | 1.485 ns | 1.389 ns |  1.43 |    0.02 | 0.0076 |      48 B |
|           RollcallFunc | Execute | 134.68 ns | 1.224 ns | 1.085 ns |  1.55 |    0.02 | 0.0076 |      48 B |

---

## Conclusion

There are a variety of ways to handle multiple implementations of the same interface, none of which are wrong. However, not all are suitable for every situation and using the incorrect one for the situation could result in a performance impact.  
There are trade-offs and pros and cons to using each technique - the most performant might be the most difficult to maintain for your situation.  
**Test the various methods and find which works best and is most optimal for your particular situation.**

## References and links
[Rollcall Github repo](https://github.com/always-developing/Rollcall.Extensions.Microsoft.DependencyInjection)  
[Rollcall Nuget package](https://www.nuget.org/packages/Rollcall.Extensions.Microsoft.DependencyInjection/) 

---
