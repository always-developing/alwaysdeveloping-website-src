---
title: "Creating temporary files in .NET"
lead: "Exploring the built-in temporary file creator"
Published: 05/19/2022
slug: "19-temp-file-name"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - temporary
    - files

---

## Daily Knowledge Drop

Sometimes an application needs to create a temporary file to store some data - .NET has built-in functionality to create temporary files, using the `Path.GetTempFileName` method.

This method will create a uniquely named, zero-byte temporary file on disk and returns the full path of that file.

---

## GetTempFileName

Creating a temporary file is incredibly simple using the `GetTempFileName` method:

``` csharp
var temporaryFile = Path.GetTempFileName();
Console.WriteLine(temporaryFile);
```

This creates a temporary file in the users temporary folder - below is the output on Windows and on a Linux container respectively:

``` powershell
C:\Users\username\AppData\Local\Temp\tmp6F69.tmp

/tmp/tmpZZGMrL.tmp
```

---

## GetTempPath

If you would like to specify the name and extension of the temporary file explicitly, the `GetTempPath` method can be used to get the temporary folder location, then have the name and extension specified:

``` csharp
string tempTxtFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
Console.WriteLine(tempTxtFile);
```

This creates a temporary file in the users temporary folder with the given name and extension - below is the output on Windows and on a Linux container respectively:

``` powershell
C:\Users\username\AppData\Local\Temp\b3eeca7e-e965-4794-9ffe-1fada639689d.txt

/tmp/c79a3579-42fc-49e7-a329-42d6bc053ada.txt
```

---

## Deletion

Even though the files are called `temporary files`, they are not temporary in existence: they are not automatically deleted or cleaned up. The _temporary_ naming refers to the type of data it is designed to contain.

If the files are not manually cleaned up and deleted, it is possible that no unique name is available and a new temporary file could not be created - in which case an `IOException` is thrown.

The recommendation would be to have the application delete the file either after use, or when the application shuts down.

---

## Notes

Using the built in `GetTempFileName` is definitely easier, quicker and simpler than trying to manually manage creating temporary files and the complications regarding file and folder permissions. The `GetTempFileName` functionality should be the default go-to method for creating temporary files.

---

## References

[Path.GetTempFileName Method](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.gettempfilename?view=net-6.0)  

<?# DailyDrop ?>77: 19-05-2022<?#/ DailyDrop ?>
