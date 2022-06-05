---
title: "Lambda chaining in C#"
lead: "Lambda Action's can be changed together in a single handle"
Published: "06/13/2022 01:00:00+0200"
slug: "13-chaining-lambda"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - lambda
    - action
    - chaining

---

## Daily Knowledge Drop

`Action lambda` expressions (expressions which take parameters but do not return a value) can be `chained together` and operated on with a single handle, the same as is done with delegates. This is because the `Action` type in C# is a delegate.

---

## Action

When defining a lambda as follows, the type of the variable is `delegate void Action`:

``` csharp
    var welcomeMsg = () => Console.Write("Good morning ");
```

As it is a delegate, multiple can be chained together:

``` csharp
var welcomeMsg = () => Console.Write("Good morning ");
welcomeMsg += () => Console.Write("Dave");
welcomeMsg += () => Console.WriteLine("");
welcomeMsg += () => Console.WriteLine("Have a great day.");

welcomeMsg();
```

Invoking the Action will result in each lambda being called in order, and the output being:

``` powershell
Good morning Dave
Have a great day.
```

To expand on this convoluted, non-practical example, logic can be applied to chain or not chain certain lambdas to the main handle.  

The below will only ouput a name if its supplied, and also allows for additional custom welcome messages to be added to the end of the main welcome message:

``` csharp
void OuputWelcomeMessage(string name ="", Action[] extraMessages = null)
{
    var welcomeMsg = () => Console.Write("Good morning ");

    if (!string.IsNullOrEmpty(name))
    {
        welcomeMsg += () => Console.Write(name);
        welcomeMsg += () => Console.Write(".");
    }

    welcomeMsg += () => Console.Write(Environment.NewLine);

    if (extraMessages != null)
    {
        foreach(var action in extraMessages)
        {
            welcomeMsg += action;
        }
    }
    welcomeMsg();
}
```


---

## Parameter Action

The same process can also be used for lambda's which take in parameters(s) - however to chain them, every lambda which is chained needs to have the same signature.

``` csharp
var updateDatabase = (int id) => 
    Console.WriteLine($"Updating record with `{id}` in database 1");
updateDatabase += (int id) => 
    Console.WriteLine($"Updating record with `{id}` in database 2");

updateDatabase(112);
```

Here we simulate updating two different databases, with the one integer parameter. The parameter will be passed into each lambda in order of them being added to the main handle.

``` powershell
Updating record with `112` in database 1
Updating record with `112` in database 2
```

---

## Async Action

Again, the same process can also be used for async lambda expressions. These now become `Func<Task>` and not `Action`, as they DO have a return value, of type Task.

The below method will accept an array of lambda's, chain them and invoke to ouput the result:

``` csharp
async Task OuputMessagesAsync(Func<Task>[] extraMessages = null)
{
    var messages = () => 
        { 
            Console.WriteLine("Executing messages:"); 
            return Task.CompletedTask; 
        };

    if (extraMessages != null)
    {
        foreach(var action in extraMessages)
        {
            messages += action;
        }
    }

    await messages();

    Console.ReadLine(); 
}
```

Invoking the method as follows, results in some interesting points:

``` csharp
await OuputMessagesAsync(
    new[] {
        async () => { await Task.Delay(4); Console.WriteLine("1"); },
        async () => { await Task.Delay(3); Console.WriteLine("2"); },
        async () => { await Task.Delay(2); Console.WriteLine("3"); },
        async () => { await Task.Delay(1); Console.WriteLine("4"); },
        async () => { await Task.Delay(0); Console.WriteLine("5"); },
    });
```

The output for the above is (and may differ each execution):

``` powershell
Executing messages:
5
2
4
3
1
```

- Only the main handle is awaiting, not the chained lambdas. If the `Console.ReadLine()` is removed, then only one value is output, and not all 5 - the application closes before all async methods have run to completion.
- This also highlights the fact that the order in which the number are output is not consistent - it can change with each execution. The lambda's are not being awaited individually before the next one in the chain is executed.

---

## Notes

I'm sure there is a valid, practical use case for this somewhere out there, but I can't think of a single situation in the past where I would have used the chaining ability. Perhaps (similar to in the above sample), allowing the caller of a method to supply additional `Action`(s) to be performed in addition to core method logic? 

Either way though, I found this to be an interesting technique and nugget of information to know about to potential future use! 

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1520811273633599489)  

<?# DailyDrop ?>94: 13-06-2022<?#/ DailyDrop ?>
