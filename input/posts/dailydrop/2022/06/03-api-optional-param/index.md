---
title: "Optional parameters in minimal apis"
lead: "How to to send optional query string parameters to a minimal api"
Published: "06/03/2022 01:00:00+0200"
slug: "03-api-optional-param"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - minimalapi
    - api
    - querystring

---

## Daily Knowledge Drop

When using minimal apis in C#, while query string values will automatically be extracted and send to the lambda expression, they are by "default", required values. Defaults can also not be supplied, as lambda expressions do not allow default values.

Either the minimal api needs to use a method (as opposed to a lambda expression) or the parameter needs to be nullable, with the default value being set in the lambda body if not passed in.

A few examples will make this clearer.

---

## Lambda endpoint

First, lets look at a "default" minimal endpoint which uses a lambda expression. Here we expect the _count_ parameter to be supplied in a query string.

``` csharp
app.MapGet("/getdata/lambda", (int count) =>
{
    return count;
});
```

If the endpoint `/getdata/lambda?count=5` is invoked, then the result return is _5_.

However, if no parameter is supplied and `/getdata/lambda` is invoked, or if a the parameter is incorrectly name (for example `/getdata/lambda?counter=5` is invoked ) an exception will be thrown:  
_BadHttpRequestException: Required parameter "int count" was not provided from query string_

## Method endpoint

If the lambda is replaced with a method, the same exception will occur if no query string parameter is supplied:

``` csharp
// external method
int ExtMethod(int count)
{
    return count;
}

app.MapGet("/getdata/extmethod", ExtMethod);
```

Invoking `/getdata/extmethod?count=10`, will result in _10_ being returned, while invoking `/getdata/extmethod` will result in the exception:  
_BadHttpRequestException: Required parameter "int count" was not provided from query string_

## Default values

Once technique which **partially** works, is to set the parameter to have a default value - however this only works when using a method, and `not when using a lambda`.  
This is due to the fact that `lambda parameters cannot have default values` - this applies generally to lambda expression, and not specific to minimal api lambdas.

``` csharp
// This is NOT VALID and will NOT compile
// The parameter count cannot have a default value
app.MapGet("/getdata/lambda", (int count = 5) =>
{
    return count;
});

// ---

// This IS VALID and will WORK
int ExtMethod(int count = 5)
{
    return count;
}

app.MapGet("/getdata/extmethod", ExtMethod);
```

However, now if the method endpoint is invoked, `/getdata/extmethod?count=10`, _10_ will be returned and if `/getdata/extmethod` is invoked (with the parameter _count_ set), the default value of _5_ will be returned.

---

## Nullable types

The solution for both cases, is to have the parameter type changed to a `nullable type` and set it to a default value in the body, if null:


``` csharp
// int changed to int?
app.MapGet("/getdata/lambda", (int? count) =>
{
    return count ?? 5;
});

// ---

// int changed to int?
int ExtMethod(int? count)
{
    return count ?? 5;
}

app.MapGet("/getdata/extmethod", ExtMethod);
```


In both cases now, the parameter is allowed to be null, and if null, then the default value of 5 will be returned. If the parameter value is supplied, then it will be returned.

---

## Notes

Thinking through the process in a general sense and comparing it to normal method invocations, it does make sense that this is how it would work - even if its not obvious initially:

- When invoking a traditional method which has a parameter of type `int`, the value has to be explicitly supplied - just like with minimal api.  
- When invoking a traditional method which has a parameter of type `int` with a default value, the value can optionally be supplied - just like with minimal api.
- When working with a lambda outside of minimal api, default values are not allowed - just like with minimal api.
- When invoking a traditional method which has a parameter of `nullable int` (int?) - the value can optionally be supplied - just like with minimal api.

---

## References

[How to Access Query Strings in Minimal APIs ](https://wildermuth.com/2022/04/04/query-strings-optional-arguments-minimal-apis-aspnetcore/)  

<?# DailyDrop ?>88: 03-06-2022<?#/ DailyDrop ?>
