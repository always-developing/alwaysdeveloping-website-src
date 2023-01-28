---
title: "ArrayPool for frequent array creation"
lead: "Using ArrayPool for performant memory reuse when creating array frequently"
Published: "01/30/2023 01:00:00+0200"
slug: "30-arraypool"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - array
   - arraypool
   - performance

---

## Daily Knowledge Drop

In situations where _arrays are created and destroyed frequently_, using the `ArrayPool<T>` class to _rent and return_ memory buffers, instead of initializing an array, can lead to improved performance and less pressure on the garbage collection process. 

---

## Array initialization

The _usual traditional_ way to instantiate an array, is to use the `new` keyword:

``` csharp
void UseInitArray(int arrayLength)
{
    ArrayItem[] array = new ArrayItem[arrayLength];

    for (int i = 0; i < arrayLength; i++)
    {
        array[i] = new ArrayItem
        {
            Id = i,
            Name = i.ToString()
        };
    }
}
```

Here an array of _arrayLength_ is initialized with `ArrayItem[] array = new ArrayItem[arrayLength];`. An _ArrayItem_ instance is then added to each element of the array.

When a large number of arrays and created and destroyed frequently, the garbage collector is (comparatively) under pressure, as there are large amounts of memory allocated to the various large arrays which need to be cleaned up.

This is where the `ArrayPool<T>` class plays its part.

---

## ArrayPool

`ArrayPool<T>` shines because it allows already allocated space to be `rented`, before it is returned back to the pool. This eliminates the need for new memory to be allocated, and then cleaned up by the garbage collector:

``` csharp
void UseArrayPool(int arrayLength)
{

    ArrayItem[] array = ArrayPool<ArrayItem>.Shared.Rent(arrayLength);

    try
    {
        for (int i = 0; i < arrayLength; i++)
        {
            array[i] = new ArrayItem
            {
                Id = i,
                Name = i.ToString()
            };
        }
    }
    finally
    {
        ArrayPool<ArrayItem>.Shared.Return(array);
    }

    // DO NOT DO THIS
    // array[0] = null;
}
```

Instead of the `new` keyword being used to instantiate the array, a block of memory (of the appropriate size for the array we required) is `rented` from the _ArrayPool_, using `ArrayItem[] array = ArrayPool<ArrayItem>.Shared.Rent(arrayLength);`

Once the array (and the memory allocated to it) is no longer required, the memory is `returned` to the _ArrayPool_, allowing it to be reused in future.

It is important to `not access the array` once the memory has been returned to the pool, as this could cause instability in the application.

---


## Notes

If an application creates and destroys a large number of array's, then using the `ArrayPool` is the way to go. It is not significantly more complicated that manually instantiating the array, and the application could gain an improvement in performance by leveraging the `ArrayPool`.

---

## References

[Stas Yakhnenko Tweet](https://twitter.com/StasYakhnenko/status/1618293902670454786)  

<?# DailyDrop ?>245: 30-01-2023<?#/ DailyDrop ?>
