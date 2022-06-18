---
title: "C# Records - the rundown"
lead: "Not sure what records are? Here's the quick rundown"
Published: 02/02/2022
slug: "02-record-intro"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - records
---

## Daily Knowledge Drop

Heard about the new feature of C# called `records`, but not entirely sure whats its all about? Here's the brief overview.

Records:
- Are `reference types` - just like normal classes
- Have `equality based on value` and not memory - unlike normal classes
- Are `immutable` (sometimes) - unlike normal classes
- Can `be inherited` - just like normal classes

Internally the compiler converts will convert a record declaration to a specialized class, so that it conforms to the above.

---

### Declaring a record

Records can be declared similar to class declaration. Records created with `nominal syntax` are` NOT immutable by default`:
``` csharp
public record Song
{
    public string Artist { get; set; }
    public string Title { get; set; }
}
```

Records can also be declared using `positional syntax`. These are `immutable by default`:
``` csharp
    public record Song(string Artist, string Title);
```

Internally when using the positional syntax, a class is creates with init properties. This can also be done manually using nominal syntax to achieve the same result. 

---

### Record ToString

The _ToString_ method of a record is overwritten (automatically) to output the record members, and not the record name (as with a class).

- Consider the `class declaration` and usage (using .NET 6 minimal startup):
``` csharp

var song1 = new Song("Foo Fighters", "Everlong");

// This will output the class name: Song
Console.WriteLine(song1.ToString());

public class Song
{  
    public string Artist { get; set; }
    public string Title { get; set; }
}
```

The above will output the class name: `Song`


- Now lets look at a `record declaration` and usage (using .NET 6 minimal startup):
``` csharp

var song1 = new Song("Foo Fighters", "Everlong");

// This will output the class name:
// Song { Artist = Foo Fighters, Title = Everlong }
Console.WriteLine(song1.ToString());

public record Song(string Artist, string Title);
```

The above will output the record values: `Song { Artist = Foo Fighters, Title = Everlong }`

---

### Record equality

Classes are equal if their **memory location is equal** (ie. they are the exact same object), while records are equal if the **value of the members are equal**.

- Consider the `class declaration` and usage (using .NET 6 minimal startup):
``` csharp

var song1 = new Song("Foo Fighters", "Everlong");
var song2 = new Song("Foo Fighters", "Everlong");
var song3 = song2;

// This will output FALSE, as the objects don't
// point to the same memory
Console.WriteLine(song1 == song2);

// This will output TRUE, as the objects do
// point to the same memory
Console.WriteLine(song2 == song3);

public class Song
{  
    public string Artist { get; set; }
    public string Title { get; set; }
}
```

The output for the above:  
`False`  
`True`

- Now lets look at a `record declaration` and usage (using .NET 6 minimal startup):
``` csharp

var song1 = new Song("Foo Fighters", "Everlong");
var song2 = new Song("Foo Fighters", "Everlong");
var song3 = song2;

// This will output TRUE, as the objects have
// the same property values
Console.WriteLine(song1 == song2);

// This will output TRUE, as the objects have
// the same property values
Console.WriteLine(song2 == song3);

public record Song(string Artist, string Title);
```

The output for the above:  
`True`  
`True`

---

### Immutability and cloning

As mentioned, records declared with _positional syntax_ are immutable.

❌ This will not even compile:
``` csharp
    var song1 = new Song("Foo Fighters", "Everlong");
    song1.Title = "Learn to Fly";
```

✅ This will compile and work:
``` csharp
    var song1 = new Song("Foo Fighters", "Everlong");
    var song2 = song1 with { Title = "Learn to Fly"};
```

This is creating a _copy_ of the song1 instance, and then changing the _Title_ property value. The original object is **not changed at all**.

With _nominal syntax_ declaration, both of the above are valid.

---

## Notes

As part of C#10, `record struct` was introduced. If not specified, then the default is class.

``` csharp
    // This will declare a record class
    public record Song(string Artist, string Title);

    // This will declare a record class
    public record class Song(string Artist, string Title);

    // This will declare a record struct
    public record struct Song(string Artist, string Title);
```

There is a lot more detail to records (see the references below) but the above should give a rundown of the record highlights.

---

## References
[C# Records](https://code-maze.com/csharp-records/)  
[Microsoft Docs - Records](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)

<?# DailyDrop ?>02: 02-02-2022<?#/ DailyDrop ?>
