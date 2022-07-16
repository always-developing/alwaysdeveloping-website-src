---
title: "Modifying configuration on load"
lead: "How configuration can be modified on load but still leverage the Options pattern"
Published: "08/12/2022 01:00:00+0200"
slug: "12-options-create"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - options
   - create

---

## Daily Knowledge Drop

The `Options.Create` method can be used in conjunction with the `GetSection.Bind` methods to read settings from the _Configuration_ provider(s), and modify them before adding them to the dependency injection container.

---

## Configuring options

First we'll have a look at how to load options _without_ any modification. 

We have the following section in the _appsettings.json_ file:

``` json
"Credentials": {
    "Username": "admin",
    "Password": "logMeIn"
}
```

The startup (using top level statements and a minimal endpoint) is as follows:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// Get the section called "Credentials" from configuration
var optionSection = builder.Configuration.GetSection("Credentials");
// this line will bind the section to a Credentials class
// and add it to the DI container automatically
builder.Services.Configure<Credentials>(optionSection);

var app = builder.Build();

app.MapGet("/getoptions", (IOptions<Credentials> options) =>
{
    return options.Value;
});

app.Run();
```

The `builder.Services.Configure<Credentials>(optionSection);` block of code, will convert the _optionSection_ to type _Credentials_ and automatically add it to the dependency injection container as `IOptions<Credentials>`.

It is best practice to use the [`options Pattern` (as described in this post)](../../02/03-ioptions/), over just inserting _Credentials_ as a singleton. The options pattern this is the default pattern when using _Services.Configure_.

Calling the endpoint defined above, will return the following:

``` json
{"username":"admin","password":"logMeIn"}
```

The obvious issue with all of the above, is that we are `storing the password in plain text in the source control repository`. Next we will look at how the settings can be:
1. Loaded from the configuration (without a password)
2. Modified (to add the password from an _environment variable_)
3. Added to the dependency injection container still using _Options pattern_

Only the startup code will need to change, and any other code which is using `IOptions<Credentials>` will not be affected and be required to change.

---

## Modifying options

To repeat - we are aiming to remove the password from the _appsettings.json_, inject it as an _environment variable_ at runtime but still have it populated on the `IOptions<Credentials>` class added to the dependency injection container.

Step one is to remove the password from  _appsettings.json_ file:

``` json
"Credentials": {
    "Username": "admin"
}
```

Next, we update the startup to load the configuration slightly differently:

``` csharp
var builder = WebApplication.CreateBuilder(args);

// load the Credentials section from the configuration
// and bind it to the credentialOptions instance
var credentialOptions = new Credentials();
builder.Configuration.GetSection("Credentials").Bind(credentialOptions);

// modify the instance by setting the Password value from
// the environment variables
credentialOptions.Password = builder.Configuration["Credentials.Password"];

// Create IOptions<Credentials> from an instance of Credentials
var ioptions = Options.Create(credentialOptions);
// manually add to the DI container
builder.Services.AddSingleton(ioptions);

var app = builder.Build();

app.MapGet("/getoptions", (IOptions<Credentials> options) =>
{
    return options.Value;
});

app.Run();
```

Here are few more steps are involved, when compared to the first example, but its not much more complex:
- Instead of using `builder.Services.Configure` to create an _IOptions_ from the configuration directly, the `builder.Configuration.GetSection.Bind` method is used to bind the configuration to an instance of _Credentials_ (at this point we have no _IOptions_)
- Manually update the instance using values from the _environment variable_ (in this example)
- Use the `Options.Create` method to create _IOptions\<Credentials\>_ from the instance of _Credentials_
- Add the _IOptions_ instance to the DI container (as a singleton)

Calling the endpoint defined above, now returns the following:

``` json
{"username":"admin","password":"logMeInEnvVar"}
```

We have a password, which was passed in as an `environment variable` (with the key _Credentials.Password_), and no client code had to change, only startup code!

---

## Notes

While there are better methods for dealing with passwords (such as storing them in a vault, and using a configuration provider to pull the values directly from the vault) - in some cases this is not feasible. So while this solution might not be the first choice, it definitely is a valid method to protect passwords, by not having them as part of the source code in a source control repository.

---

<?# DailyDrop ?>137: 12-08-2022<?#/ DailyDrop ?>
