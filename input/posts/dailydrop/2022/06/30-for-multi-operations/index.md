---
title: "Multiple statements in a for loop"
lead: "A for loop can contain multiple statements in the iterator section"
Published: "06/30/2022 01:00:00+0200"
slug: "30-for-multiple-operations"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - forlop
    - for
    - iteration

---

## Daily Knowledge Drop

Not only can the _iterator section_ (the last section) in a `for loop` contain `multiple operations`, but it can also contain `statements`.

---

## Iterator section

Generally, when defining and using a `for loop`, the _iterator section_ only increases (or decreased) the initializer variable, `loopCount` in the below example:

``` csharp
for (var loopCount = 0; loopCount < 100; loopCount++) 
{ 
}
```

This _iterator section_ `can actually contain statements, separated by a comma!`

---

### Operations

In this simple example, each loop handles its iterations with the `i` variable, but also decreases a shared `progress` variable representing overall progress:

``` csharp
var valueArray = new int[] { 100, 25, 70 };
var progress = valueArray.Sum();

for (int i = 0; i < valueArray[0]; i++, progress--)
{
    // Simulate some processing
    Thread.Sleep(10);
    
    Console.Clear();
    Console.WriteLine($"{progress} items remaining to be processed");
}

for (int i = 0; i < valueArray[1]; i++, progress--)
{
    // Simulate some processing
    Thread.Sleep(10);
    
    Console.Clear();
    Console.WriteLine($"{progress} items remaining to be processed");
}

for (int i = 0; i <= valueArray[2]; i++, progress--)
{
    // Simulate some processing
    Thread.Sleep(10);
    
    Console.Clear();
    Console.WriteLine($"{progress} items remaining to be processed");
}
```

---

### Statements

The _iterator section_ can doesn't only have to contain operations, but can also contain statements.

For example purposes, consider logging is to be performed for each record being processed - you might do it as follows:

``` csharp
for (int i = 1; i <= recordsToProcess.Count(); i++)
{
    // log some information about the record
    Console.WriteLine($"Current value of process record: {recordsToProcess[i]}");

    // Simulate some processing
    Thread.Sleep(10);
}
```

The keep the code contained within the for loop block clear, one could move the log statement into the `for loop iterator section`:

``` csharp
// logging moved to into the for loop statement
for (int i = 1; i <= recordsToProcess.Count(); 
    Console.WriteLine($"Current value of process record: {recordsToProcess[i]}"), i++)
{
    // actual code block is cleaner

    // Simulate some processing
    Thread.Sleep(10);
}
```

---

### Methods

Expanding on the previous example, methods can also be invoked in the _iterator section_ - the logging can be moved to its own method to keep the code slightly cleaner:

``` csharp
for (int i = 0; i <= recordsToProcess.Count(); LogRecord(recordsToProcess[i]), i++)
{
    // Simulate some processing
    Thread.Sleep(10);
}

// ---------------------

void LogRecord(int recordValue)
{
    Console.WriteLine($"Current value of process record: {recordValue}");
}
```

---

## Notes

This bit of knowledge does have limited practical application, and one could argue that it makes the code _harder_ to read - however it does have it's place, and I think could be especially useful in the first example to keep track of multiple "progress-type" counters.

---

## References

[Khalid Abuhakmeh Tweet](https://twitter.com/buhakmeh/status/1526275436140961805)  

<?# DailyDrop ?>107: 30-06-2022<?#/ DailyDrop ?>
