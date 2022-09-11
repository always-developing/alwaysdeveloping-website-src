---
title: "Manually passing parameters to a middleware component"
lead: "Using manually supplied parameters in conjunction with injected parameters"
Published: "10/05/2022 01:00:00+0200"
slug: "05-middleware-parameter"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - middleware
   - parameters

---

## Daily Knowledge Drop

It is possible to `manually pass parameters to a middleware component` when configuring the component and middleware pipeline on startup. _Manually passed parameters_ can be used in conjunction with parameters provided by the _dependency injection_ container. 

---

## Middleware configuration

In the below examples, to demonstrate a parameter provided by the DI container, a simple _ApplicationConfiguration_ class has been configured with the dependency injection container as _transient_:

``` csharp
builder.Services.AddTransient(typeof(ApplicationConfiguration));
```

### Distinct parameters

If a middleware component has `parameters which are of distinct types` then when adding the component to the middleware pipeline, the parameters can be specified in `any order`.

Below, the _DistinctParamMiddleware_ constructor has distinct types as parameters:

``` csharp
public class DistinctParamMiddleware 
{
    private readonly int _version;
    private readonly string _name;
    private readonly ApplicationConfiguration _setup;
    private readonly RequestDelegate _next;

    // all distinct parameters
    public DistinctParamMiddleware(RequestDelegate next, string name, 
        int version, ApplicationConfiguration setup)
    {
        _version = version;
        _name = name;
        _setup = setup;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"In {nameof(DistinctParamMiddleware)} with" +
            $"application name: '{_name}' and version: '{_version}'");

        await _next(context);
    }
}
```

Parameters can manually be passed in when using the `UseMiddleware` method on startup, and because the parameter types are distinct, the manually supplied parameters can be passed in any order:

``` csharp
app.UseMiddleware<DistinctParamMiddleware>( 1, "MiddlewareParamDemo");
```

In the above, `RequestDelegate` and `ApplicationConfiguration` are supplied from the _dependency injection container_, while the `string` and `int` parameters are supplied `manually`. Even though the constructor has the _string parameter_ specified first, and then the _int parameter_, when invoking the _UseMiddleware_ method, the values are not supplied in that order.

---

### Non-distinct parameters

When dealing with a middleware component which does `not have parameters which are of distinct types`, then the parameter values need to be supplied in order:

``` csharp
public class DuplicateTypeParamMiddleware
{
	private readonly ApplicationConfiguration _setup;
	private readonly RequestDelegate _next;
	private readonly string _name;
	private readonly string _version;

    // two string parameters
	public DuplicateTypeParamMiddleware(RequestDelegate next, string name, 
        string version, ApplicationConfiguration setup)
	{
		_setup = setup;
		_next = next;
		_name = name;
		_version = version;
	}

	public async Task Invoke(HttpContext context)
	{
		Console.WriteLine($"In {nameof(DuplicateTypeParamMiddleware)} with" +
			$"application name: '{_name}' and version: '{_version}'");

		await _next(context);
	}
}
```

As there are two `string parameters`, the first string value supplied is assigned to the _name_ parameter, and the second string value supplied is assigned to the _version_ parameter:

``` csharp
// Correct
app.UseMiddleware<DuplicateTypeParamMiddleware>("MiddlewareParamDemo", "2");

// Incorrect
// app.UseMiddleware<DuplicateTypeParamMiddleware>("2", "MiddlewareParamDemo");
```

---

## Notes

This functionality is especially useful in cases when a value determined at runtime (for example, _application startup datetime_) needs to be passed into middleware - having the ability to manually pass the value in instead of creating new entities to hold the values, and registering them with the dependency injection container, can save time and simplify code.

---

## References

[Inject dependency to your middleware class manually](https://github.com/dodyg/practical-aspnetcore/blob/net6.0/projects/middleware/middleware-8/Program.cs)   

<?# DailyDrop ?>175: 05-10-2022<?#/ DailyDrop ?>
