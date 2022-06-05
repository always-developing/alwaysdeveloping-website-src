---
title: "AllowNull and DisallowNull attributes"
lead: "Exploring the usage of the AllowNull and DisallowNull attributes"
Published: "06/07/2022 01:00:00+0200"
slug: "07-allownull-disallownull-attributes"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - property
    - attribute

---

## Daily Knowledge Drop

The `AllowNull` and `DisallowNull` attributes can be used to set that a property can/should allow a `null` value or not.

These are used in a fairly narrow and niche use case, which won't necessarily effect the performance of code, but does eliminate compiler warnings.

---

## Null Property

Consider a `User` class with a `Name` property. Our system _requires that a user has a name, but does not require the user has to provide one_. If none is provided, then a default is specified.

In this case, the _Name_ will `never return null, but can be set to null`.

``` csharp
public class User
{
    private string _name;

    public string Name
    {
        get => _name;
        set => _name = value ?? "Anonymous";
    }
}
```

If an instance of _User_ is declared, and `null` assigned to _Name_, the compiler will give the following warning:  
**_Cannot convert null literal to non-nullable reference type_**

Changing the property type from _string_ to _string?_ will get rid of the warning, but this is not an entirely accurate representation of the type. _Name_ can never have a value of and return `null` - callers will never need to check _Name_ property for `null`.  

A more accurate method to get rid of the warning, is to use the `AllowNull` attribute (in the _System.Diagnostics.CodeAnalysis_ namespace):

``` csharp
[AllowNull]
public string Name
{
    get => _name;
    set => _name = value ?? "Anonymous";
}
```

This attribute specified a pre-condition which `only applies to arguments`, and since only the `set` accessor makes use of an argument, the attribute only applies to this accessor, and not the `get`.

---

## Non-null Property

Consider the opposite situation now - a property which has a `null` default value, but if explicitly set, is not allowed to be `null`. We would want to indicate to the caller that it is possible for the value to be `null`, but also that it cannot explicitly be set to `null`.

Going back to the `User` class, consider now we to add a `Email` property. The system _requires that if a user has an email address, it cannot be set to null_.  
The default value for an email is `null`, but if it is explicitly set, it cannot be `null` (for example, maybe a customer can create a user profile on an e-commerce site, without an email - but if they decide to checkout, an email is required.)

``` csharp
public class User
{
    private string _name;

    private string _email;

    [AllowNull]
    public string Name
    {
        get => _name;
        set => _name = value ?? "Anonymous";
    }

    public string Email
    {
        get => _email;
        set => _email = value ?? 
            throw new ArgumentNullException(nameof(value), "Cannot set to null");
    }

}
```

This doesn't indicate that the value _could_ be `null`. To indicate that we can make it a nullable type, `string?`.

``` csharp
public string? Email
{
    get => _email;
    set => _email = value ?? 
            throw new ArgumentNullException(nameof(value), "Cannot set to null");
}
```

Now there is a clear indication that the value _could_ be `null`, however it is still possible to assign a `null` value to the field without any compiler time warning. Only at runtime will the exception be thrown.

The `DisallowNull` attribute can be used to indicate that the parameter to the property cannot be `null`:

``` csharp
public string? Email
{
    get => _email;
    set => _email = value ?? 
            throw new ArgumentNullException(nameof(value), "Cannot set to null");
}
```

As with before, this attribute specified a pre-condition which `only applies to arguments`, and since only the `set` accessor makes use of an argument, the attribute only applies to this accessor, and not the `get`.  

Now if `null` is assigned to the _Email_ property, there **will** be a compiler warning stating:
**_Cannot convert null literal to non-nullable reference type_**

This is what we would want - an indicator that the value could be `null` (the fact the type is a nullable type), but also an indicator if it is explicitly set to `null` (a compiler warning)

---

## Notes

As mentioned, while this is a fairly narrow and niche use case, it a real use case especially for library authors. While neither of the attributes are "required" to make the code function, they assist in conveying the intended usage of the properties to the user of the properties, by either showing or suppressing a compiler warning.

---

## References

[Preconditions: AllowNull and DisallowNull](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis#preconditions-allownull-and-disallownull)  

<?# DailyDrop ?>90: 07-06-2022<?#/ DailyDrop ?>
