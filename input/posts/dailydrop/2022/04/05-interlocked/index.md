---
title: "Interlocked vs using the lock keyword"
lead: "How InterLocked can simplify the lock process in certain cases"
Published: 04/05/2022
slug: "05-interlocked"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - task
    - atomic
    - interlocked
    - lock

---

## Daily Knowledge Drop

Instead of using the `lock` keyword to block a portion of code from negative effects of multi-threading, the `Interlocked` class can be used instead to simplify the code.

---

## Why even lock?

So why would we need to even lock? consider the following example, regarding a _bank account_. 

The bank account has a balance, and two methods to _Deposit_ and _Withdraw_ money from the account.

``` csharp
class Account
{
    public int Balance { get; set; }

    public void Deposit(int depositAmount)
    {
        Balance += depositAmount;
    }

    public void Withdraw(int withdrawAmount)
    {
        Balance -= withdrawAmount;
    }
}
```

The following code simulates a high volume number of transactions on the bank account - it loops for 10000 iterations in total, creating tasks to deposit $5 and to withdraw $5. These tasks are created to run in parallel, and then at the end the application waits for all the tasks to complete, before printing out the balance.

``` csharp
var account = new Account();
var taskList = new List<Task>();

for (var x = 0; x < 100; x++)
{
    taskList.Add(Task.Factory.StartNew(() =>
    {
        for (var y = 0; y < 100; y++)
        {
            account.Deposit(5);
        }
    }));

    taskList.Add(Task.Factory.StartNew(() =>
    {
        for (var y = 0; y < 100; y++)
        {
            account.Withdraw(5);
        }
    }));
}
Task.WhenAll(taskList);
Console.WriteLine(account.Balance);
```

As the code is depositing and withdrawing $5, 10000 each, one would expect the final balance to be $0. However this is not the case

The results will vary wildly, but we I executed the code I got the following output:

``` powershell
    -625
```

So why is the balance not 0? This occurs because the += and =- operations are `not atomic`. 

---

## Atomic Process

For something to be `atomic`, it needs to be a single operations which  cannot be influenced by another thread.  
When doing the += for example, the sequence of events _internally_ are:
 1. Set temporary variable to the _Balance_ + the deposit amount
 2. Set the _Balance_ value to that of the temporary variable.

The issue here is that this is a two step process, where another thread could come in between steps 1 and 2 and complete its += or -= operation altering the value of _Balance_ from the expected value.

To mitigate this, we need to make this operation `atomic`, or use another `atomic` process to prevent the unwanted interference.

---

## lock keyword

The process can be made `atomic` by using the `lock` keyword - this keyword is used to acquire an mutual exclusive lock on an given object, effectively preventing any other code from acquiring a lock executing the code until the lock is released.

Below is the updated _Account_ class:

``` csharp
class Account
{
    // A new object is introduced to the class, the sole 
    // purpose of which is to operate as the lock.
    private object @lock = new object();
    public int Balance { get; set; }

    public void Deposit(int depositAmount)
    {
        // Whenever an operation needs to occur which modifies the Balance amount, 
        // the lock variable is now used in conjunction with the `lock` keyword.   
        lock (@lock)
        {
            Balance += depositAmount;
        }
    }

    public void Withdraw(int withdrawAmount)
    {
        // Whenever an operation needs to occur which modifies the Balance amount, 
        // the lock variable is now used in conjunction with the `lock` keyword.   
        lock (@lock)
        {
            Balance -= withdrawAmount;
        }
    }
}
```

For the duration of the `lock` statement, no other thread can acquire a lock, and as such is forced to wait before acquiring the lock, and performing its operation.

Executing the same sample 10000 iterations and updates, yields the expected result, a final balance of:

``` powershell
    0
```

---

## Interlocked

While the above process works as expected and gives us the expected result, there is an easier and cleaner way to do it, using the `Interlocked` class.

Below is the updated _Account_ class:

``` csharp
class Account
{
    // The Balance_property has been changed to use a 
    // private variable and an explicit getter. 
    private int balance;

    public int Balance { get => balance; }

    // When increasing or decreasing the balance amount, the Interlocked.Add 
    // method is now used. This takes in a reference to the value to update
    public void Deposit(int depositAmount)
    {
        Interlocked.Add(ref balance, depositAmount);
    }

    public void Withdraw(int withdrawAmount)
    {
        Interlocked.Add(ref balance, -withdrawAmount);
    }
}
```

`Interlocked.Add` is atomic and will ensure that no other threads operation on _balance_ while another operation is taking place. The result is exactly the same as when using the `lock` keyword, just with less code clutter and less manual work needing to be done by the developer.

---

Using the `Interlocked` class is a small, but useful update which can be made to code to keep it as clean as possible, without polluting the classes with _lock_ objects. However it is not the only way to create an atomic operation, and other methods should be explored if `Interlocked` is not suitable.

---

## References

[lock statement](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock)  
[Interlocked Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked?view=net-6.0)  
[.NET Parallel Programming with C#](https://tfginfotec.udemy.com/course/parallel-dotnet/learn/lecture/5650624)  

<?# DailyDrop ?>45: 05-04-2022<?#/ DailyDrop ?>
