---
title: "Calling an async method in a constructor"
lead: "Using lazy initialization to call an async method from a constructor"
Published: "01/26/2023 01:00:00+0200"
slug: "26-async-constructor"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - async
   - constructor

---

## Daily Knowledge Drop

As it is _not possible to await an async method in a constructor_, if an async method _has_ to be called, a valid technique is to `call the async method in the constructor, but defer the await until later`.


---

## Non async

Calling a `non async` method in a constructor is straight forward:

``` csharp
public class MyClass
{
	private readonly string _specialData;

    public MyClass()
    {
    	// takes long, not ideal
    	_specialData = ExternalService.GetData();
    }
}
```

Here, an external service is called to get data required for `MyClass` to function correctly. 

However, is the scenario when `ExternalService.GetData()` is an async method `ExternalService.GetDataAsync()` handled?

---

## Async

### GetResult

If the only option to get the require data is via a _async method_, simply just calling it in the constructor will not work:

``` csharp
public class MyClass
{
	private readonly string _specialData;


    public MyClass()
    {
        // Won't work!
        // Return type is Task<string> not string
        // _specialData = ExternalService.GetDataAsync();

        // Won't work!
        // To use await, the method needs to be async
        // but constructors cannot be async
        //_specialData = await ExternalService.GetDataAsync();
    }
    	
}
```

One `not recommended` option is to do the following:

``` csharp
public class MyClass
{
    private readonly string _specialDataTask;

    public MyClass()
    {
    	_specialData = ExternalService.GetDataAsync().GetAwaiter().GetResult();
    }
}
```

`.GetAwaiter().GetResult()` is used on the _Task_ returned from _GetDataAsync_ - generally not a good idea to use this approach.

---

### Lazy

A better option is to use the `lazy initialization approach`:

``` csharp
public class MyClass
{
    private readonly Task<string> _specialDataTask;

    public MyClass()
    {
       _specialDataTask = ExternalService.GetDataAsync();
    }

    public async Task DoWorkUsingSpecialDataAsync()
    {
       var _specialData = await _specialDataTask.ConfigureAwait(false);

       // Do the work using _specialData
    }
}
```

- The type of the "_specialData" variable was changed from `string to Task<string>`
- In the constructor, the `async method is called, but not awaited` - this returns a _Task\<string\>_
- When/if the "_specialData" value is required, then the _Task\<string\>_ is awaited to get the actual value

With this method, when the constructor is called, the process of getting the special data is initiated, but is not blocking. When it comes time to use the value returned from the process, `await` is used to get the value from the _Task_. Either the processing is finished and the value will be available immediately, or the process is still ongoing and the value will be returned once the process is completed.

---


## Notes

Ideally, this scenario should probably be avoided all together - but if it is a requirement, the lazy-initialization technique is great way to solve the problem.

---

## References

[Lazy and once-only C# async initialization](https://endjin.com/blog/2023/01/dotnet-csharp-lazy-async-initialization)  

<?# DailyDrop ?>243: 26-01-2023<?#/ DailyDrop ?>
