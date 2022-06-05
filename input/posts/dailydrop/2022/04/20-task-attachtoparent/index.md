---
title: "Creating a task in a task"
lead: "Exploring what happens when creating a task from within a task"
Published: 04/20/2022
slug: "20-task-attachtoparent"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - task
    - parallel

---

## Daily Knowledge Drop

By default, when creating a child task from within a parent task, the child task has `no dependency` on the parent task - they are unrelated and will be run independently.  

If a parent task is require to wait on the child task(s) to complete, then the `TaskCreationOptions.AttachedToParent` option is to be used when creating the child task.

---

## Single Task

First we'll look at an example of just a single task being created:

``` csharp
var parentTask = new Task(() =>
{
    Console.WriteLine("Main task started");
    Console.WriteLine("Main task finished");
});

parentTask.Start();
parentTask.Wait();

Console.WriteLine("Application closing");
```

There is no much going on here. A _parent task_ is created (which does nothing but print to the console), started and the code waits until it finishes.

The output is as follows:

``` powershell
Main task started
Main task finished
Application closing
```

---

## Detached child

Next let's create a child task in the parent task, using the default settings, and see what is output:

``` csharp
var parentTask = new Task(() =>
{
    Console.WriteLine("Main task started");
    var childTask = new Task(() =>
    {
        Console.WriteLine("Sub task started");
        Thread.Sleep(1000);
        Console.WriteLine("Sub task finished");
    });

    childTask.Start();

    Console.WriteLine("Main task finished");
});

parentTask.Start();
parentTask.Wait();

Console.WriteLine("Application closing");
```

Here, from within the _parent task_, a child task is created (which writes to the console, but also sleeps for 1 second), and is started.

The output for the above is as follows:

``` powershell
Main task started
Main task finished
Application closing
```

`No information from the child task is output!`  - this is because the code is only waiting on the _parent task_ and there is no dependency between the the child and the parent task.  

The child task doesn't have a chance to output to the console before the _parent task_ finishes processing and the application closes.

---

## Attached child

To wait on all _child tasks_ as well, the child tasks need to be `attached to the parent`:

``` csharp
var parentTask = new Task(() =>
{
    Console.WriteLine("Main task started");
    var childTask = new Task(() =>
    {
        Console.WriteLine("Sub task started");
        Thread.Sleep(1000);
        Console.WriteLine("Sub task finished");
    }, TaskCreationOptions.AttachedToParent);

    childTask.Start();

    Console.WriteLine("Main task finished");
});

parentTask.Start();
parentTask.Wait();

Console.WriteLine("Application closing");
```

When the _child task_ is created, on **line 9** the `TaskCreationOptions.AttachedToParent` option is specified. Now, when waiting on the _parent task_, all child tasks will automatically be waited on as well.

The output is now as follows:

``` powershell
Main task started
Main task finished
Sub task started
Sub task finished
Application closing
```

---

## Notes

When creating tasks in tasks - its very important to know and understand how the tasks are created and how they interact (or don't) with each other, as it can drastically change how the application operates.

The above is just one example on how the options of a task can be changed to modify its behavior.

---

## References

[Attached and Detached Child Tasks](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/attached-and-detached-child-tasks)  

<?# DailyDrop ?>56: 20-04-2022<?#/ DailyDrop ?>
