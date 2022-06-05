---
title: "Lazy loading objects with Lazy<>"
lead: "Defer the loading of large objects until require with Lazy<>"
Published: 03/09/2022
slug: "09-lazy-class"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - lazy
    - loading

---

## Daily Knowledge Drop

The `loading of (large) objects can be deferred` until they are actually `used and required` using the `Lazy<>` class

---

## Sample

### Use case

In our use case, we have a `FileEntity` which contains details about a file in a specific location. There are two child entities which are properties to `FileEntity`:
- `FileSize`: stores the file size in bytes (and just stores a double value, so is small)
- `FileContents`: stores the contents of the file as a string (depending on the size of the file, this can obviously be very large)

The `contents of the file will not always be used` and are potentially `very large` - so let's look at how we can _defer loading the data until it is actually used and required_.

--- 

### The setup

``` csharp
// the main file entity class
public class FileEntity
{
    // a normal FileContentsEntity private variable,
    // just wrapped in Lazy<> 
    private readonly Lazy<FileContentsEntity> _fileContents;

    public Guid Id { get; }

    public string FileLocation { get; }

    public FileSizeEntity FileSize { get; }

    // when accessed, return Lazy<FileContentsEntity>.Value
    public FileContentsEntity FileContents => _fileContents.Value;

    public FileEntity(Guid id, string fileLocation, double fileSizeInBytes)
    {
        Console.WriteLine("FileEntity constructed");

        Id = id;
        FileLocation = fileLocation;
        FileSize = new FileSizeEntity(fileSizeInBytes);

        // instead of instantiating the fileContents,
        // instantiate Lazy<> with an startup method
        _fileContents = new Lazy<FileContentsEntity>(LoadFileContents);
    }

    // method to load the large data volume
    private FileContentsEntity LoadFileContents()
    {
        return new FileContentsEntity(FileLocation);
    }
}

// simple entity to store the file size
public class FileSizeEntity
{
    public double FileSizeInBytes { get; set; }

    public FileSizeEntity(double fileSizeInBytes)
    {
        Console.WriteLine("FileSizeEntity constructed");

        FileSizeInBytes = fileSizeInBytes;
    }
}

// simple entity to store the file contents
public class FileContentsEntity
{
    public string LargeStringValue { get; set; }

    public FileContentsEntity(string fileLocation)
    {
        Console.WriteLine($"FileContentsEntity loaded from '{fileLocation}'");

        LargeStringValue = "LargeStringValue";
    }
}
```

To use `Lazy<>`, the setup is almost _exactly the same_ as without using `Lazy<>`.

### Output

So what does the `Lazy<>` actually enable, and how does it effect execution?

``` csharp
Console.WriteLine("== pre initialization ==");

// Two FileEntity instances are created, 
// one with a small file and one with a large file
var file1 = new FileEntity(Guid.NewGuid(), @"C:\small-file.txt", 100);
var file2 = new FileEntity(Guid.NewGuid(), @"C:\large-file.txt", 1073741824);

Console.WriteLine("== post initialization ==");
Console.WriteLine("");

// The large FileContents is accessed
Console.WriteLine("Accessing file1 contents");
var file1Location = file1.FileContents;

// only the FileLocation is accessed
Console.WriteLine("Accessing file2 location");
var fileContents = file2.FileLocation;
```

The output looks as follows:

``` powershell
== pre initialization ==
FileEntity constructed
FileSizeEntity constructed
FileEntity constructed
FileSizeEntity constructed
== post initialization ==

Accessing file1 contents
FileContentsEntity loaded from 'C:\small-file.txt'
Accessing file2 location
```


The key take-away from the output, is that the `large contents of file2 are never loaded`, as they are never used.

---

## Thread safety

There are additional considerations regarding thread safety and Lazy<>. These are not addressed in this post, but can be read in details in the reference below.

---

## Notes

`Lazy<>` provides a simple and easy to use way to load objects _only when they are uses_ - this is especially useful when the large object is not loaded on every code path. This can lead to better performance and memory usage.

---

## References
[Lazy<T> Class](https://docs.microsoft.com/en-us/dotnet/api/system.lazy-1?view=net-6.0)  

<?# DailyDrop ?>27: 09-03-2022<?#/ DailyDrop ?>
