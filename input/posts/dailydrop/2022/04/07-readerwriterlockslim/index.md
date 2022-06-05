---
title: "Managing multi-thread access with ReaderWriterLockSlim"
lead: "Using ReaderWriterLockSlim to allow concurrent reading but exclusive writing"
Published: 04/07/2022
slug: "07-readerwriterlockslim"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - thread
    - readerwriter
    - lock

---

## Daily Knowledge Drop

The `ReaderWriterLockSlim` class can be used to allow multiple threads to _read_ a resource, while only allowing one thread to _write_ to a resource.

In a [previous Daily Drop post](../05-interlocked/) we looked at how the Interlocked can be used to lock a resource - `ReaderWriterLockSlim` is similar to this, but allows for finer control over how the resource is locked.

---

## Base setup

In the setup, we have a _Price_ class which stores a price. We want the class to allow multiple concurrent reads, but only allow one thread to update the price at a time.

``` csharp
class Price
{
    private int _price;

    // An instance of ReaderWriterLockSlim is declared for the class.
    private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    public void PrintPrice()
    {
        // The code indicates to the lock that its entering and exiting a read portion. 
        // Multiple threads are allowed to enter this block at the same time
        _lock.EnterReadLock();

        Console.WriteLine($"{DateTime.Now} => The current price is: {_price}");

        Thread.Sleep(1000);

        _lock.ExitReadLock();
    }

    public void SetPrice(int price)
    {
        // The code indicates to the lock that its entering and exiting a write portion. 
        // Only one thread will be allowed to enter this block at the same time
        _lock.EnterWriteLock();

        _price = price;

        Console.WriteLine($"{DateTime.Now} => Price updated to: {_price}");
        Thread.Sleep(1000);

        _lock.ExitWriteLock();
    }
}
```

A _Thread.Sleep_ statement has been added to each locked block to simulate a potentially long running process.

---

## Read example

The below will run a test on the _read_ portion of the code.

``` csharp
var price = new Price();
price.SetPrice(100);

var taskList = new List<Task>();
for (int x = 0; x < 10; x++)
{
    taskList.Add(Task.Factory.StartNew(() =>
    {
        price.PrintPrice();
    }));
}
Task.WaitAll(taskList.ToArray());

```

Here 10 threads are being created, and executed simultaneously to try _read_ the price.

``` powershell
2022/03/17 06:02:38 => Price updated to: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
2022/03/17 06:02:39 => The current price is: 100
```

The output gives an indication how `ReaderWriterLockSlim` operates for reading - all 10 threads accessed the resource at the same time. This confirms that the `read lock allows for multiple-threads to read at the same time`.

---

## Write example

Next we'll look at a write example.

``` csharp
var rando = new Random();
var price = new Price();
price.SetPrice(100);

var taskList = new List<Task>();
for (int x = 0; x < 10; x++)
{
    taskList.Add(Task.Factory.StartNew(() =>
    {
        price.SetPrice(rando.Next(1000));
    }));
}
Task.WaitAll(taskList.ToArray());
```

Here 10 threads are being created, and run simultaneously to try _write_ to the price (to a random value)

``` powershell
2022/03/17 06:07:07 => Price updated to: 100
2022/03/17 06:07:08 => Price updated to: 984
2022/03/17 06:07:09 => Price updated to: 701
2022/03/17 06:07:10 => Price updated to: 335
2022/03/17 06:07:11 => Price updated to: 335
2022/03/17 06:07:12 => Price updated to: 877
2022/03/17 06:07:13 => Price updated to: 621
2022/03/17 06:07:14 => Price updated to: 463
2022/03/17 06:07:15 => Price updated to: 316
2022/03/17 06:07:16 => Price updated to: 919
2022/03/17 06:07:17 => Price updated to: 38
```

The output gives an indication how `ReaderWriterLockSlim` operates for writing - the price was only updated once per second. The _write_ block is blocking access to write to the resource for 1second (due to _Thread.Sleep(1000)_). This confirms that the `write lock allows only one thread to access the resource at any one time`.

---

## Other features

`ReaderWriterLockSlim` provides additional functionality which we'll touch on here, but won't go into detail.

### TryEnterWriteLock

`ReaderWriterLockSlim` has a method called `TryEnterWriteLock` which accepts a timeout parameter. This allows for a thread to try get a write lock on a resource, but only wait for a specified time before aborting.

### EnterUpgradeableReadLock

Sometimes we might enter a _read_ lock, but then have a branch of code which will need to _write_. The `EnterUpgradeableReadLock()` caters for this scenario. It allows for a read lock to be created (and thus allowing multiple threads to still access the resource), but then the lock can be upgrade to a _write_ lock when required to block access to other threads, to allow for writing.

---

## Notes

Using `ReaderWriterLockSlim` allows for greater flexibility over using the `lock` keyword or `Interlocked` class. The latter two only allow for a blanket lock of a resource preventing any other thread from concurrently accessing the resource.  
`ReaderWriterLockSlim` allows for finer control, allowing multiple threads to read a resource, but locks the resource from multiple parallel writes.

---

## References

[ReaderWriterLockSlim Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=net-6.0)  

<?# DailyDrop ?>47: 07-04-2022<?#/ DailyDrop ?>
