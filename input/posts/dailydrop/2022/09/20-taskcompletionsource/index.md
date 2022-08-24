---
title: "Representing an external operation with TaskCompletionSource"
lead: "Using TaskCompletionSource to cancel an external asynchronous operation"
Published: "09/20/2022 01:00:00+0200"
slug: "20-taskcompletionsource"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - task
   - taskcompletionsource
   - handler
   - async

---

## Daily Knowledge Drop

A `TaskCompletionSource` instance can be used to represent an external asynchronous operation, allowing the external operation to be _awaited_ on. 

This is especially useful when required to wait on a callback method to be invoked before proceeding with the execution of code.

---

## Requirement

Consider the use case where an application can request that a file (containing website information in this example) be created asynchronous. Once the file is created, a callback handler method will be invoked to notify the caller that the file has been created.

Consider the below async method to request the file. The calling application has no control over this method - it just calls the _RequestFileCreation_ method, specifying the _folder_ and the _callback event_:

``` csharp
Task RequestFileCreation(string folder, Func<string, Task> callback)
{
    Task.Run(async () =>
    {
        // simulate the time it takes to create the file!
        await Task.Delay(1000);

        // invoke the callback method, notifying the called
        // the file has been created
        // filename is hardcoded in the example
        await callback.Invoke("alwaysdeveloping.txt");
    });

    return Task.CompletedTask;
}
```

When the method is invoked, a separate task is created (and not _awaited_) to run a process to have the file created asynchronously. Once the file is created, the specified callback back is invoked.

The issue with this approach is that when invoking the `RequestFileCreation` method there is `no reliable way to wait for the file to be created` if we need to perform some additional processing on the file after it has been created:

``` csharp
async Task GetWebsiteInformation()
{
    Console.WriteLine($"Start of '{nameof(GetWebsiteInformation)}' method");

    // call the method to request the file be created
    // the await is NOT awaiting the creation of the file, but is 
    // awaiting the request to create the file
    await RequestFileCreation("C:\\inputfiles", (file) =>
    {
        Console.WriteLine($"File '{file}' processed successfully");

        return Task.CompletedTask;
    });

    // Need to do some additional processing based on the
    // contents of the file here

    Console.WriteLine($"End of '{nameof(GetWebsiteInformation)}' method");
}
```

When executed, the result is the following:

``` terminal
Start of 'GetWebsiteInformation' method
End of 'GetWebsiteInformation' method
```

The method is executed and exits `without waiting for the file to be created and subsequent processing to be done`. We need a way to `wait for the file to be created`, before performing additional processing and exiting the method. 

This is exactly the situation `TaskCompletionSource` is able to solve!

---

## TaskCompletionSource

Incorporating the `TaskCompletionSource` into the process is very straight forward.

The _RequestFileCreation_ method remains exactly as it was above, but the `GetWebsiteInformation` is updated to be as follows:

``` csharp
async Task GetWebsiteInformation()
{
    Console.WriteLine($"Start of '{nameof(GetWebsiteInformation)}' method");

    // create a TaskCompletionSource<T> instance where T
    // is the return type 
    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

    // in the callback handler method, call the SetResult
    // on TaskCompletionSource instance.
    await RequestFileCreation("C:\\inputfiles", (file) =>
    {
        Console.WriteLine($"File '{file}' processed successfully");

        tcs.SetResult(file);

        return Task.CompletedTask;
    });

    Console.WriteLine($"Waiting for a file to be created");
    // await the TaskCompletionSource instance task
    // Once the SetResult method has been set, the task will be 
    // considered completed
    var fileName = await tcs.Task;

    Console.WriteLine($"File created! Performing additional processing");
    // Need to do some additional processing based on the
    // contents of the file here
    
    Console.WriteLine($"End of '{nameof(GetWebsiteInformation)}' method");
}
```

There are a few moving parts here:
1. Declare an instance of _TaskCompletionSource\<T\>_ where T is the type to be returned
1. When the callback method is invoked (once the file is created), the _SetResult_ method of _TaskCompletionSource_ is called with the information to be passed through the _TaskCompletionSource_ Task (the string filename in this example) 
1. When required to wait for the file to be created, the _TaskCompletionSource.Task_ is awaited. This task will be considered completed once the _SetResult method is called_ in the callback

Executing the above method now has the following output:

```terminal
Start of 'GetWebsiteInformation' method
Waiting for a file to be created
File 'alwaysdeveloping.txt' processed successfully
File created! Performing additional processing
End of 'GetWebsiteInformation' method
```

From this we can see, the output is exactly what we want. A request for the file to be created is initiated - the code then waits for the file to be created (the callback method being invoked), before processing continues.

--- 

## Other Set methods

Not demonstrated in this post, but there are two other `Set` methods available on the __TaskCompletionSource_:
- `SetException`: transitions the underlying Task into a _Failed_ state.
- `SetCanceled`: transitions the underlying Task into a _Canceled_ state.

---

## Notes

An incredibly valuable class to be aware of when one needs to wait on an _external asynchronous operation_ (such as a callback event), while also being able to convey information out of the external operation to the scope _awaiting_ the call.

---

## References

[Async Web API testing with TaskCompletionSource (Microservices with .NET 6.0) - FeedR episode #11](https://youtu.be/N-ofc345-58?list=PLqqD43D6Mqz0AIDkHqaZDKaEKXdfMiIAo&t=1610)   

<?# DailyDrop ?>164: 20-09-2022<?#/ DailyDrop ?>
