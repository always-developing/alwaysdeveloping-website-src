---
title: "Custom feature filters"
lead: "Defining custom feature filters to selectively enable functionality"
Published: "12/01/2022 01:00:00+0200"
slug: "01-context-feature"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - featureflag
   - context

---

## Daily Knowledge Drop

The `Microsoft.FeatureManagement` library, allows for a custom `context` to be defined, and evaluated to `determine if specific functionality should be enabled or not`, _for the specific context_.

This functionality has no dependency on ASP.NET or _HttpContext_, and as such can be used in any application type (such as console app).

---

## Setup

There are a number of moving pieces to the setup, which requires some classes be defined and some configuration specified to leverage the _custom feature filter management_.

### Define the context

The first step is to define the `context` - this is a simple class which will contain all the information which will be evaluated to determine if the feature or functionality should be enabled or not.

In this example, the context will just contain the user's email address:

``` csharp
public class User
{
    public string Email { get; set; }
}
```

In short - a `context` will be created containing the supplied email address, which will be evaluated against _settings_ to determine if _normal_ or _enhanced_ processing is to occur.

---

### Define the settings

The next step is to define a _settings_ class, which will contain properties against which the `context` values will be evaluated.  

In this example, we want the _user email address domain_, defined in the `User` context, to be compared with a _company domain_, defined in the settings class, `EnhancedUserSettings`, to determine if the user should be processed using the _enhanced/preview feature_, or the _standard existing feature_.

``` csharp
public class EnhancedUserSettings
{
    public string EmailDomain { get; set; }
}
```

Essentially what it comes down to, is evaluating if the `email address defined in the User context has the same domain defined in the settings EnhancedUserSettings`. If so, then the _feature_ is enabled, otherwise the feature is not enabled. 


The usage of this and how each class fits together will become apparent in the steps below. 

---

### Define the filter

Next, we have to define the `filter` - this piece of code will do the actual logic to evaluate and determine if the feature should be enabled or not, based on the `context` and `settings`:

``` csharp
public class EmailDomainFilter : IContextualFeatureFilter<User>
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureFilterContext, 
        User appContext)
    {
        // get the value from the parameters configured
        var settings = featureFilterContext.Parameters.Get<EnhancedUserSettings>();

        // perform the check
        return Task.FromResult(appContext.Email?.Split("@").Last() == 
            settings.EmailDomain);
    }
}
```

The filter implements `IContextualFeatureFilter<Context>`, which contains one method _EvaluateAsync_. _EvaluateAsync_ takes a generic _FeatureFilterEvaluationContext_ context, as well as the specific _User_ context defined for this filter.

In the above, the `EnhancedUserSettings` instance is obtained from the generic _FeatureFilterEvaluationContext_ context - this retrieves the values based on the configuration defined in the next section.

---

### Define the configuration

In the `appsettings.json` the filter feature functionality needs to be configured:

``` json
"FeatureManagement": {
    "EnhancedUserProcessing": {
      "EnabledFor": [
        {
          "Name": "EmailDomain",
          "Parameters": {
            "EmailDomain": "mycompany.co.za"
          }
        }
      ]
    }
  }
```

The configuration defines the filter and how it should be used:
1. There is a feature called `EnhancedUserProcessing` (the name of this is important, which we will see when it comes time to use the functionality)
1. This feature uses a filter called `EmailDomain` - the name of the class which implements `IContextualFeatureFilter`, _EmailDomainFilter_ in this example (the name of the class plus _Filter_ as a suffix to the class name), 
1. The parameters for this filter, `EnhancedUserSettings`, will have a property called _EmailDomain_ which will have a value set to `mycompany.co.za`

---

### Setup Recap

1. Define the `context` - this will be the information which is dynamic (each user/request/etc will have different context values) and which is compared to the `settings`
1. Define the `settings` - this is the class which will contain the information the `context` is compared against, to determine if the feature is enabled or not. This is static information, and is the same for all evaluations
1. Define the `filter` - this specified exactly _how_, the `context` values are compared with the `settings` values
1. Define the `configuration` - gives the filter a name, and specifies the `settings` values

---

## Usage

Finally, let's look at the usage of the `filter feature`.

Below a minimal API endpoint is defined to demonstrate the usage:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// add feature management to DI container
// and add the specific filter
builder.Services.AddFeatureManagement()
       .AddFeatureFilter<EmailDomainFilter>();

var app = builder.Build();

// inject IFeatureManager implementation from the DI container
// get the email from the URL route 
app.MapGet("/process/{email}", async ([FromServices]IFeatureManager featureManager,
    [FromRoute]string email) =>
{
    // check if the feature with the name `EnhancedUserProcessing` is enabled
    // or not for the specific context (the user email)
    if(await featureManager.IsEnabledAsync("EnhancedUserProcessing", 
        new User { Email = email }))
    {
        return "Processed using ENHANCED features";
    }

    return "Processed using NORMAL features";
});

app.Run();
```

`IFeatureManager` is injected into the relevent class/method, and the _IsEnabledAsync_ method is called. The specific feature to use (specified by name) is supplied along  with the relevent `context`. 

Browsing to the endpoint `/process/alwaysdevelpoping@mycompany.co.za` will return:

``` terminal
Processed using ENHANCED features
```

While using an email with any other domain will result in:

``` terminal
Processed using NORMAL features
```

This is a simple example, but the library does offer more sophisticated functionality around the configuration and options available to evaluate the filter (see the links available under references)

---

## Notes

The ability to turn off/on specific features for a specific subset of users/records is a valuable tool when trying to roll out useful, experimental or preview functionality quickly to get "real world" feedback before it gets rolled out completely.
There are other more sophisticated 3rd party feature management tool available (LaunchDarkly for example), but the `Microsoft.FeatureManagement` library is entirely adequate, especially as a starting point.

---


## References

[Contextual Feature Filters in ASP.NET Core](https://coderethinked.com/contextual-feature-filters-in-asp-net-core/)  
[Tutorial: Use feature flags in an ASP.NET Core app](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core?tabs=core5x)

<?# DailyDrop ?>214: 01-12-2022<?#/ DailyDrop ?>
