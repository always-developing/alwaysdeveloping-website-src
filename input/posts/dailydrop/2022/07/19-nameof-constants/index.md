---
title: "nameof usage for better code quality"
lead: "How the nameof operator can be leveraged to produce cleaner, less error prone code"
Published: "07/19/2022 01:00:00+0200"
slug: "19-nameof-constants"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - nameof

---

## Daily Knowledge Drop

When defining string with constants (in particular, but other situations also apply) the `nameof operator` can be leveraged to remove explicitly set values, ensuring more consistent, cleaner and less error prone code is produced.

---

## nameof expression

So briefly, what does `nameof` do? - it _produces the name of a variable type, or member as the string constant_.

Consider the following code:

``` csharp
var fibonacci = new List<int> { 0, 1, 1, 2, 3, 5 } ;
Console.WriteLine(nameof(List<int>));  // outputs the type
Console.WriteLine(nameof(fibonacci)); // outputs the variable name
Console.WriteLine(nameof(fibonacci.Count)); // outputs the method name
```

The output is:

``` terminal
List
fibonacci
Count
```

So let's see how we can leverage `nameof` in the following use cases.

---

## Constants
### Issue

Suppose our application has a list of _system statuses_ stored as `string constants`:

``` csharp
public class SystemStatus
{
    public const string Starting = "Starting";
    public const string Running = "Running";
    public const string ShuttingDown = "ShuttingDown";
    public const string Offline = "Offline";
}
```

With the usage and output being:

``` csharp
Console.WriteLine(SystemStatus.ShuttingDown); // outputs "ShuttingDown"
```

Is there anything `inherently wrong` with this approach? Not really, it still functions as expected.

However, suppose we need to `change the "ShuttingDown" status to "Terminating"`. Navigating to one of the usages of _SystemStatus.ShuttingDown_, we rename the `const` (using F2 _Rename_, for example).

We now have this:

``` csharp
Console.WriteLine(SystemStatus.Terminating); // still outputs "ShuttingDown"!
```

However the output is still `"ShuttingDown"`, even though the `const` name has been changed to _Terminating_! We now either have `misleading code at best or a bug at worst.`

---

### Resolution

As this post implies, the solution is to use the `nameof` operator instead of a hard-coded string!

Replacing the string values, with `nameof` referencing the same variable:

``` csharp
public class SystemStatus
{
    public const string Starting = nameof(Starting);
    public const string Running = nameof(Running);
    public const string ShuttingDown = nameof(ShuttingDown);
    public const string Offline = nameof(Offline);
}
```

This for example will set the value of the _Starting_ constants to the name of the constant, "Starting".

Now when renaming one of the `const` names, it's value will be kept consistent - the same as the `const` name. If manually renaming one of the `const` names, but forgetting to rename its usage in `nameof` will result in a compiler error:

``` csharp
public class SystemStatus
{
    public const string Starting = nameof(Starting);
    public const string Running = nameof(Running);
    // Compiler error "ShuttingDown" does not exist
    public const string Terminating = nameof(ShuttingDown);
    public const string Offline = nameof(Offline);
}
```

The result - cleaner, more consistent and less error prone code!

---

## Parameters

### Issue

A similar issue can be experience in instances where a `parameter name is used`, in an exception message, for example.

If we have this method, with the parameter name hardcoded in the exception string message:


``` csharp
public int Division(int dividend, int div)
{
    if(div == 0)
    {
        // string value is hardcoded with the parameter name
        throw new ArgumentException($"Parameter 'div' cannot be zero");
    }

    return dividend / div;
}
```

We realize the `div` parameter is not a good parameter name, and should be renamed to `divisor`:

``` csharp
public int Division(int dividend, int divisor)
{
    if(divisor == 0)
    {
        // string value is hardcoded with the INCORRECT parameter name
        throw new ArgumentException($"Parameter 'div' cannot be zero");
    }

    return dividend / divisor;
}
```

Its very easy to miss the hardcoded string when refactoring, especially if using an IDE rename feature, which won't automatically rename the hand-coded parameter name within the string.

---

### Resolution

Again, the remedy is to use the `nameof` operator:

``` csharp
public int Division(int dividend, int divisor)
{
    if (divisor == 0)
    {
        throw new ArgumentException($"Parameter '{nameof(divisor)}' cannot be zero");
    }

    return dividend / divisor;
}
```

Now if renaming the parameter name, the exception message is automatically updated as well (or will throw a compiler error if not all usages of the parameter are not updated):

The result - cleaner, more consistent and less error prone code!

---

## Notes

A small, simple technique to incorporate into daily coding, which may not have any obvious immediate benefit - but if/when it comes time to do any refactoring, will definitely save time and effort due to its cleaner, more consistent approach.

---

## References

[Reddit CSharp post](https://www.reddit.com/r/csharp/comments/v81c13/can_anyone_tell_me_the_point_of_this_syntax_im/)   

---

<?# DailyDrop ?>119: 19-07-2022<?#/ DailyDrop ?>
