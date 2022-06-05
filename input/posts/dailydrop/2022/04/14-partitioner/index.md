---
title: "Using Partitioner to improve parallel processing"
lead: "How chunking parallel tasks with Partitioner can improve performance"
Published: 04/14/2022
slug: "14-partitioner"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - task
    - parallel
    - partitioner

---

## Daily Knowledge Drop

When using `Parallel.ForEach` to execute tasks in parallel, it might be more performant to execute the tasks in parallel chucks (partitions), using the `Partitioner` functionality.

---

## Performing tasks

In all of the below examples, an array of 1000 sequential integers (starting at 0 all the way up to 999) is used. Each of the 1000 values are multiplied by itself and then stored in the corresponding location in the results array.  

E.g. intArray[10] contains the value 10. It will be multiplied by itself (10 x 10), and the result (100) will be stored in resultsArray[10]

### Sequential tasks

The below three examples, iterate through the 1000 items and execute the processing sequentially.

- The first uses a traditional `for` loop to iterate forward through the array
- The second uses a traditional `for` loop, but iterates backwards through the array
- The third uses a `foreach` loop to iterate through the items

``` csharp
public void SequentialLoop()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    for (int i = 0; i < intArray.Length; i++)
    {
        resultArray[i] = intArray[i] * intArray[i];
    }
}

public void BackSequentialLoop()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    for (int i = 999; i >= 0; i--)
    {
        resultArray[i] = intArray[i] * intArray[i];
    }
}

public void SingleForeach()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    Array.ForEach(intArray, i =>
    {
        resultArray[i] = i * i;
    });
}
```

Benchmarking the results (using BenchmarkDotNet) - the for loops are fairly equivalent, while the foreach is 2.5x slower.

|                     Method |      Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
|--------------------------- |----------:|----------:|----------:|-------:|-------:|----------:|
|             SequentialLoop |  1.073 us | 0.0192 us | 0.0179 us | 1.2875 | 0.0191 |      8 KB |
|         BackSequentialLoop |  1.202 us | 0.0196 us | 0.0184 us | 1.2875 | 0.0191 |      8 KB |
|              SingleForeach |  2.690 us | 0.0085 us | 0.0071 us | 1.3008 | 0.0267 |      8 KB |

---

### Parallel tasks

The below two examples, use parallel processing to process the 1000 items.

- The first uses `Parallel.Foreach`, which will process an item on a thread, and spawn up as many threads as it thinks is necessary (this can explicitly be set using _ParallelOptions_, but for this benchmark, only the default configuration was used)
- The second uses `Parallel.Foreach`, but specifies that each thread should get 100 items, and those 100 items should be processed using a `for` loop.

``` csharp
public void ParallelForeach()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    Parallel.ForEach(intArray, i => {
        resultArray[i] = i * i;
    });
}

public void PartitionerParallelForeach()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    Parallel.ForEach(Partitioner.Create(0, 100), r => {
        for (var i = r.Item1; i < r.Item2; i ++)
        {
            resultArray[i] = i * i;
        }
    });
}
```

Combining these benchmarks with the ones from above:

|                     Method |      Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
|--------------------------- |----------:|----------:|----------:|-------:|-------:|----------:|
|             SequentialLoop |  1.073 us | 0.0192 us | 0.0179 us | 1.2875 | 0.0191 |      8 KB |
|         BackSequentialLoop |  1.202 us | 0.0196 us | 0.0184 us | 1.2875 | 0.0191 |      8 KB |
|              SingleForeach |  2.690 us | 0.0085 us | 0.0071 us | 1.3008 | 0.0267 |      8 KB |
|            ParallelForeach |  9.726 us | 0.0740 us | 0.0618 us | 1.9836 | 0.0458 |     12 KB |
| PartitionerParallelForeach | 11.153 us | 0.0716 us | 0.0669 us | 2.3193 | 0.0458 |     13 KB |

We can see that the `parallel processing is slower than the traditional looping!`

In the above examples, this is because the `time taken to do the calculation is less than the overhead of creating and managing multiple threads`.

--- 

## Benchmarking v2

Next we look at what happens if the processing is slower than the time taken to create and manage the threads.

All of the benchmarks were run again, but this time a `Thread.Sleep(1)` is added to each calculation (to simulate a slightly longer running process)

See an example below of how the _Thread.Sleep_ was added.


``` csharp
public void SequentialLoop()
{
    var intArray = Enumerable.Range(0, 1000).ToArray();
    var resultArray = new int[intArray.Length];

    for (int i = 0; i < intArray.Length; i++)
    {
        resultArray[i] = intArray[i] * intArray[i];
        Thread.Sleep(1); // This was added to all benchmarks!!
    }
}
```

The benchmark results now look as follows:

|                     Method |         Mean |     Error |     StdDev |       Median | Allocated |
|--------------------------- |-------------:|----------:|-----------:|-------------:|----------:|
|             SequentialLoop | 15,392.18 ms |  6.618 ms |   5.526 ms | 15,392.32 ms |      8 KB |
|         BackSequentialLoop | 15,391.47 ms |  5.214 ms |   4.071 ms | 15,392.25 ms |      9 KB |
|              SingleForeach | 15,393.31 ms |  6.519 ms |   6.098 ms | 15,392.96 ms |      9 KB |
|            ParallelForeach |    616.28 ms | 57.985 ms | 160.677 ms |    568.01 ms |     20 KB |
| PartitionerParallelForeach |     71.18 ms |  7.555 ms |  22.277 ms |     61.29 ms |     27 KB |

The `partitioned parallel processing is order of magnitudes faster` than the sequential processing (although it does use more memory)

---

## Notes

While not always applicable in all use cases, when it is applicable the parallel processing, and especially using the `Partitioner` can be leverage to get a great increase in performance. It does however depend on the processing being executed - and as always, the specific use case should be benchmarked to determine the most performant method of processing.

---

## References

[Partitioner Class](https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.partitioner?view=net-6.0)  

<?# DailyDrop ?>52: 14-04-2022<?#/ DailyDrop ?>
