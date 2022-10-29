---
title: "Adding localization with dependency injection"
lead: "Adding localization support to ASP.NET Core with dependency injection support"
Published: "11/09/2022 01:00:00+0200"
slug: "09-localization-di"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - localization
   - DI
   - dependencyinjection

---

## Daily Knowledge Drop

`Localization` is the _process of translating an application's resources into localized versions for each culture that the application will support_. 

Enabling this multi-culture, localization support is as easy as adding a few lines of code on application startup, and leveraging the `IStringLocalizer` implementation when working with the variable, localized values.

---

## Non-localized

Suppose we have an endpoint, which when called will return a string containing a random color:

``` csharp
app.MapGet("/get", () =>
{
    var random = new Random();
    var randomValue = random.Next(3);

    var response = randomValue switch
    {
        0 => "Blue",
        1 => "Green",
        2 => "Yellow"
    };

    return $"The color generated is: {response}";
});
```

This code will generate a random number between 1 and 3, and return a string indicating the color generated.

Calling the endpoint returns the following (the colour value may change with each call):

``` terminal
The color generated is: Blue
```

In the above return message the American spelling of "color" is returned, and not the British/South African spelling, _colour_. We are going to add support for either variation, depending on the caller's culture - this is a fairly simple innocuous change, but the steps used here can be expended to add complete support for a different language.

---

## Localized

### Defining the variations

The first step is to define the various strings which will have different versions based on the culture. In our example this would be:
- The return message
- The three different colours 

In this specific use case these three different colour values will not change between cultures, but if the application is to support multi-culture, its a good idea to "localize" all string values.

The various culture specific strings are stored in `resx files`, which usually reside in a `Resources` folder. These files follow the naming standard of `{Class}.{culture}.resx`.

In this case, two files where added to the `Resources` folder:
- Program.en-us.resx
- Program.en-za.resx

As we are using minimal apis, the usage of the values will be in the `Program` class, hence the name of the resx files is _Program_. The cultures supported in our application will be `English-Unites States` and `English-South African`.

The resx files will both contain the `same names` (keys), but each will have the specific localized values:

`Program.en-us.resx`:  

|                  Name |                       Value |
|---------------------- |---------------------------- |
|                  Blue |                        Blue |
| ColourResponseMessage | The color generated is: {0} |
|                 Green |                       Green |
|                Yellow |                      Yellow |

`Program.en-za.resx`:  

|                  Name |                        Value |
|---------------------- |----------------------------- |
|                  Blue |                         Blue |
| ColourResponseMessage | The colour generated is: {0} |
|                 Green |                        Green |
|                Yellow |                       Yellow |

As mentioned, in this example only the _ColourResponseMessage_ will differentiate between the two, with the slightly different spelling

Now that we have the variable values defined, we begin by _adding localization support to the dependency injection container_.

---

### Dependency injection configuration

During application startup, the following is added:

``` csharp
// add the localization support to the dependency injection container
// which includes the path to the resx files
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// add the cultures which will be supported
var supportedCultures = new[]
{
    new CultureInfo("en-za"),
    new CultureInfo("en-us")
};
builder.Services.Configure<RequestLocalizationOptions>(options => {
    options.DefaultRequestCulture = new RequestCulture("en-us");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
```

Here we configured the DI container with the generic localization interfaces and implementations, as well as explicitly specified which cultures will be supported.

---

### Middleware configuration

The next step is to configure the middleware pipeline (this is defined before any of the endpoints are defined):

``` csharp
app.UseRequestLocalization(app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
```

This middleware components will _automatically set culture information for requests based on information provided by the client_.

Finally we are now able to take advantage of the localization capabilities and update the initial `/get` endpoint defined above.

---

### Localized endpoint

To leverage the localization functionality, the `IStringLocalizer` interface and implementation is used - this is injected from the dependency injection container. Instead of the string value being hardcoded, `IStringLocalizer` is used to lookup the culture specific string by name:

``` csharp
// inject IStringLocalizer with the specific class
app.MapGet("/get", ([FromServices]IStringLocalizer<Program> localizer) =>
{
    var random = new Random();
    var randomValue = random.Next(3);

    // same logic as before
    var localizedResponse = randomValue switch 
    {
        // use IStringLocalizer to get the culture specific string
        // GetString also allows for arguments to be passed in and 
        // another localized string (the colour) is being passed in
        // as a parameter to format the ColourResponseMessage
        0 => localizer.GetString("ColourResponseMessage", localizer.GetString("Blue")),
        1 => localizer.GetString("ColourResponseMessage", localizer.GetString("Green")),
        2 => localizer.GetString("ColourResponseMessage", localizer.GetString("Yellow")),
    };

    return localizedResponse.Value;
});
```

When _GetString_ is called on the _IStringLocalizer_ implementation, the current culture of the context is used - if no culture is explicitly supplied, then the default culture is used.

Calling the endpoint as it stands returns the same result as before - the default culture is used:

``` terminal
The color generated is: Yellow
```

---

### Changing culture

When the middleware pipeline was updated in a previous step using _UseRequestLocalization_, it added the functionality to `change the culture based on a query string`.

Calling the endpoint with the culture specified `/get?culture=en-za` now results in the following:

``` terminal
The colour generated is: Blue
```

The strings are now culture specific! Localization support has been added to the application.

---

## Notes

Adding localization support to an api is a relatively easy process, and only requires the steps mentioned above. For large api's, if the task seems daunting and over whelming, due to the nature of the updates, it can be done in a phased approach, one endpoint at a time, making it a bit more manageable.

---

## References

[Localization in ASP.NET Core](https://code-maze.com/aspnetcore-localization/)  

<?# DailyDrop ?>198: 09-11-2022<?#/ DailyDrop ?>
