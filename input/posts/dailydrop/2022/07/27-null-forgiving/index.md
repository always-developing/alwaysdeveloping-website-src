---
title: "Null forgiving operator"
lead: "Suppressing compiler null reference warnings with the null forgiving operator"
Published: "07/27/2022 01:00:00+0200"
slug: "27-null-forgiving"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - null
    - operator

---

## Daily Knowledge Drop

C# contains a `unary, postfix, null-forgiving` (or null-suppression) operator `!`, which can be used to declare that an expression of a reference type is `not null`.

The null-forgiving operator is used in a nullable enabled context, has no effect at run time, and only affects the compiler's static flow analysis by changing the null state of the expression in question.

Effectively, the operator informs the compiler that an expression which it has determined to be `null`, is in fact not null and no diagnostic alert (a warning by default) needs to be raised.

---

## Usage

### Unit testing

A practical use of the `!` null-forgiving operator, is when performing unit tests.

Consider the following class, which has an _Artist_ property, forced to be `non-null` by the constructor:

``` csharp
public class Song
{
    public string Artist { get; }

    public Song(string artist)
    {
        Artist = artist ?? throw new ArgumentNullException(nameof(artist));
    }
}
```

When unit testing the class, one would want to test all possible combinations of _Song_ instantiation - with and without a valid _artist_ value:

``` csharp
// Instantiation to test created successfully
var song = new Song("Foo Fighters");

// Instantiation to test exception is thrown
var exceptionSong = new Song(null);
```

The issue with the above, is at compile time, the compiler sees the `null` being passed to the constructor and raises the following warning:

``` terminal
Cannot convert null literal to non-nullable reference type.
```

In this case, we **want** to pass `null` as part of a unit test to determine the correct behavior is seen. The `null-forgiving` operator can be used to effectively tell the compiler, that the _`null` expression is not null, and the compiler does not need to treat is as such_ (and thus no warning is generated)

Updating the code to the following, will remove the warning:

``` csharp
// Instantiation to test created successfully
var song = new Song("Foo Fighters");

// Instantiation to test exception is thrown
// Null-forgiving operator added
var exceptionSong = new Song(null!);
```


---

### Manual check

Another practical use of the `null-forgiving operator`, is in situations when the compiler has `incorrectly` determined that an expression could possibly be `null`.

Consider the below method to validate an instance of _Song_:

``` csharp
static bool IsValid(Song? song)
    => song is not null && song.Artist is not null;
```

And the usage:

``` csharp
// get a nullable Song instance 
// from a source
Song? s = GetSong();

// validate the song and output if valid
if (IsValid(s))
{
    Console.WriteLine($"Song by Artist `{s.Artist}` is valid");
}
```

The above code generates the following warning on the `s.Artist` usage:

``` terminal
Dereference of a possibly null reference.
```

This warning is due to the fact, that based on the data types, `s` (of type `Song?`) could contain a `null` value, in which case `s.Artist` would cause an exception. However, the `IsValid` method is performing checks to ensure that `s.Artist can never be invoked when s is null` - in this use case the warning is incorrect.

The `null-forgiving` operator can be used again to tell the compiler, that the _s_ expression in `s.Artist` can be treated as never being null.

Updating the code to the following, will remove the warning:

``` csharp
// get a nullable Song instance
Song? s = GetSong();

if (IsValid(s))
{
    // null-forgiving operator added
    Console.WriteLine($"Song by Artist `{s!.Artist}` is valid");
}
```

---

## Notes

While not an operator for every day use for most applications, the `null-forgiving` operator can be very useful in resolving certain inaccurate warnings. However take care when using the operator, and only implement when sure that the compiler is incorrect. While the code itself will not throw any exceptions **because** of the operator usage (it's ignored at runtime), the code could throw exceptions due to the the expression in question being `null` - which the compiler was trying to warn about (before being mainually overwritten with the operator)

---

## References

[! (null-forgiving) operator](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving)   

---

<?# DailyDrop ?>125: 27-07-2022<?#/ DailyDrop ?>
