---
title: "Platform specific path separator"
lead: "The Path class contains platform specific characters"
Published: "11/02/2022 01:00:00+0200"
slug: "02-directory-separator"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - path
   - separator

---

## Daily Knowledge Drop

The static `Path` class contains a number of helpful, platform specific properties (and methods) to assist when working with filenames and file paths. These include:
- The operating system specific directory separator character
- The operating system specific alternate directory separator character
- The operating system specific path separator character
- The operating system specific volume separator character
- The operating system specific invalid filename characters
- The operating system specific invalid path characters

---

## Code snippet

A simple code snippet to demonstrate how to access the various characters:

``` csharp
// All the separator characters are available on the Path static class
Console.WriteLine($"DirectorySeparatorChar: '{Path.DirectorySeparatorChar}'");
Console.WriteLine($"AltDirectorySeparatorChar: '{Path.AltDirectorySeparatorChar}'");
Console.WriteLine($"PathSeparator: '{Path.PathSeparator}'");
Console.WriteLine($"VolumeSeparatorChar: '{Path.VolumeSeparatorChar}'");

// display all the invalid file name characters
Console.WriteLine($"Invalid filename chars:");
foreach(var chr in Path.GetInvalidFileNameChars())
{
    Console.WriteLine($"   {chr}");
}

// display all the invalid path characters
Console.WriteLine($"Invalid path chars:");
foreach (var chr in Path.GetInvalidPathChars())
{
    Console.WriteLine($"   {chr}");
}
```

---

### Windows output

The output when running the above on a Windows environment - the invalid filename and invalid path characters have been trimmed as the list for Windows is fairly long:

``` terminal
DirectorySeparatorChar: '\'
AltDirectorySeparatorChar: '/'
PathSeparator: ';'
VolumeSeparatorChar: ':'
Invalid filename chars:
   "
   <
   >
   |

   ☺
   ☻
   ♥
   ♦
   ♣
   ♠
// trimmed
```

---

### Linux output

Running the same code in a Linux environment yields different output - no trimming of the list here, as apparently Linux supports a larger list of characters than Windows:

``` terminal
DirectorySeparatorChar: '/'
AltDirectorySeparatorChar: '/'
PathSeparator: ':'
VolumeSeparatorChar: '/'
Invalid filename chars:
   ?
   /
Invalid path chars:
   ?
```

---

## Notes

Leveraging the above functionality makes building up and validating file names and file paths much easier, simpler and quicker, and should definitely be preferred over rolling out ones own path builder or filename validator.

---

## References

[Path Class in C#](https://code-maze.com/csharp-path-class/)  

<?# DailyDrop ?>193: 02-11-2022<?#/ DailyDrop ?>
