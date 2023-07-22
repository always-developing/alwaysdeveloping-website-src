Title: Software
lead: Libraries, packages and frameworks | open-source 
Order: -100
showComments: true
---

# NuGet Packages

- [**Rollcall (aka Rollcall.Extensions.Microsoft.DependencyInjection)**](https://www.nuget.org/packages/Rollcall.Extensions.Microsoft.DependencyInjection/)  
    Rollcall provides support for **_named service registration_** through the use of extension methods on the IServiceCollection interface, and retrieval of registered service by name using a factory.

    For more information and benchmarks on the various ways to register `multiple implementations of the same interface`, see [this blog post.](../p/multiple-implementations/)  

- [**AlwaysDeveloping.EntityFrameworkCore.DynamicContext**](https://www.nuget.org/packages/AlwaysDeveloping.EntityFrameworkCore.DynamicContext/)  
    Provides support for executing raw SQL queries against a Entity Framework Core DbContext without a DbSet for the entity. It also provides support to return a result set of single types (int, bool, Guid, string etc).

    For more information and benchmarks on the various ways to `execute the various data retrieval`, see [this blog post.](../p/11-2020-dynamic-context/)  

- [**SourceGeneratorToolkit**](https://www.nuget.org/packages/SourceGeneratorToolkit)  
    Provides functionality to easily generate c# code/files using a _fluent builder pattern_ either in conjunction with the .NET Roslyn Source Generator process, or outside of the Roslyn Source Generator process.

    For more information and various ways to 'leverage code qualfication and generation` provided by the Source Generator Toolkit, see [this blog post.](../p/2023-07-source-gen-toolkit/)  