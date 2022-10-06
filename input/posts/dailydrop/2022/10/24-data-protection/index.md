---
title: "Protecting sensitive data with data protection api"
lead: "Exploring the built-in C# data protection api"
Published: "10/24/2022 01:00:00+0200"
slug: "24-data-protection"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - dataprotector
   - protect

---

## Daily Knowledge Drop

.NET has built in methods to `protect and unprotected` sensitive data. 

These methods are typically used to protect sensitive data, but could be used to protect any data. It could also be used to expose information for a _given period of time_ only, after which trying unprotecting the data will not work.

---

## Configuration

There are effectively three steps to using the data protection apis:

- Create the `protector` from a data protection provider
- Invoke the `Protect` method to protect the data specified
- Invoke the `Unprotect` method to convert the specified protected value to plain text

In the startup of the application, the protector is configured as follows:

``` csharp
// register base data protection
// with DI container
builder.Services
    .AddDataProtection()
    .SetDefaultKeyLifetime(TimeSpan.FromDays(7));

// register the protector with DI
builder.Services
    .AddTransient<ITimeLimitedDataProtector>(sp =>
    {
        // specify where the provider key information will be stored
        IDataProtectionProvider? provider = DataProtectionProvider
            .Create(new DirectoryInfo(@"secrets"));

        // specify the purpose string for the creator
        IDataProtector? protector = provider
            .CreateProtector("customerdata");

        return protector.ToTimeLimitedDataProtector();
    });
```

Some notes on the above:
- The key information is stored in a `secrets` folder on the host machine. As such, the key information is only available to that one machine. If multiple machines need to access the key information, there are other storage providers available (Azure KeyVault, DbContext)
- The protector is created with a specific _purpose_, `customerdata` above. A value protected using a specific purpose, cannot be unprotected using a different purpose value.

---

## Protect

To _protect_ a string, the `Protect` method is called. Below, an endpoint has been defined which has the _ITimeLimitedDataProtector_ protector implementation (configured in the previous step) injected:

``` csharp
// get the value to protect from the route
app.MapGet("/protect/{protectValue}", (
    [FromRoute]string protectValue, 
    ITimeLimitedDataProtector protector) =>
{
    // return the protected value
    return protector.Protect(protectValue);
});
```

Invoking the endpoint `/protect/abc123` results in the following protected value being returned:

``` terminal
CfDJ8BdtcMmju6ZKiKJtRHvTt2BidaZbPEirTP_KEi4-_2xVBYbs58hqVxWGQmTQBjQaO_tXTiFRAxGUPGSqXkvlfIzhd80r3AavXi0m3PiCXYfuaRhmnrzCXk6apOIqXOCvHA
```

---

## Unprotect

_Unprotecting_ is as simple as protecting a value - the `Unprotect` method is called, with the previously protected value supplied:

``` csharp
app.MapGet("/unprotect/{unprotectValue}", (
    [FromRoute] string unprotectValue, 
    ITimeLimitedDataProtector protector) =>
{
    return protector.Unprotect(unprotectValue);
});
```

Invoking the endpoint `/unprotect/CfDJ8BdtcMmju6ZKiKJtRHvTt2BidaZbPEirTP_KEi4-_2xVBYbs58hqVxWGQmTQBjQaO_tXTiFRAxGUPGSqXkvlfIzhd80r3AavXi0m3PiCXYfuaRhmnrzCXk6apOIqXOCvHA` results in the following output:

```terminal
abc123
```

---

## Notes

When needing to protect sensitive data (for example, _authentication cookie_ or _bearer token_) this set of classes and functionality provides an easy, simple and effective way to protect the data.


---

## References

[Piotr Penza](https://twitter.com/ppenza/status/1573676247812653058)  
[ASP.NET Core Data Protection Overview](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-6.0)  
[Configure ASP.NET Core Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-6.0)

<?# DailyDrop ?>188: 24-10-2022<?#/ DailyDrop ?>
