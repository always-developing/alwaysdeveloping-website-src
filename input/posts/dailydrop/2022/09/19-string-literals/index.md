---
title: "UTF8 string literals in C#11"
lead: "How string literals are easily translated into bytes with C#11"
Published: "09/19/2022 01:00:00+0200"
slug: "19-string-literals"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - C11
   - string
   - literals
   - stringliterals

---

## Daily Knowledge Drop

Coming with C# 11 (being released later this year, coinciding with the .NET 7 release) the conversion from a `string literal to a byte[]` is becoming _easier, faster, and more efficient_.

The byte[] is often used in dealing with streams (for example) and with the current, and prior C# versions, the conversation from a string to byte[] required as explicit conversion. However with C#11, this conversion is simplified, but also gains a large performance boost.

---

## C# 10 and prior

In the current (and prior) versions of C#, when a string literal is required to be converted to a byte[], the ` System.Text.Encoding.X.GetBytes` method is used (where X is the encoding method, UTF8 specifically in this post):

``` csharp
byte[] bytes = System.Text.Encoding.UTF8.GetBytes("alwaysdeveloping.net");

using var stream = new MemoryStream();
stream.Write(bytes);
```

While not especially complicated, this does involve an explicit method call to perform the conversion.

---

## C# 11

With C#11, it's possible to do this with an implicit conversion:

``` csharp
ReadOnlySpan<byte> spanBytes = "alwaysdeveloping.net"u8;

using var stream = new MemoryStream();
stream.Write(spanBytes);
```

Although a _ReadOnlySpan_ can be used whereever a _byte[]_ is required, if a _byte[]_ is specifically needed:

``` csharp
ReadOnlySpan<byte> spanBytes = "alwaysdeveloping.net"u8;
byte[] bytes = spanBytes.ToArray();

using var stream = new MemoryStream();
stream.Write(bytes);
```

The `u8` suffix on the string, indicates to the compiler that it should convert the string value into an array of bytes - or more specifically in this case, a `ReadOnlySpan of bytes`. Using a _ReadOnlySpan_ is more efficient and uses no additional memory - but if a byte[] is specifically required, the _ToArray_ method can be leveraged to get a _byte[]_ from the _ReadOnlySpan_.

---

## Performance

Below are a couple of simple benchmarks run to compare the performance and memory usage of the old and new methods:

``` csharp

[Benchmark(Baseline = true)]
public void GetBytes()
{
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes("alwaysdeveloping.net");
}

[Benchmark]
public void StringLiteral()
{
    ReadOnlySpan<byte> spanBytes = "alwaysdeveloping.net"u8;
}

```

|        Method |       Mean |     Error |    StdDev |     Median | Ratio |  Gen 0 | Allocated |
|-------------- |-----------:|----------:|----------:|-----------:|------:|-------:|----------:|
|      GetBytes | 19.5843 ns | 0.4163 ns | 0.6956 ns | 19.6017 ns | 1.000 | 0.0076 |      48 B |
| StringLiteral |  0.0198 ns | 0.0209 ns | 0.0241 ns |  0.0085 ns | 0.001 |      - |         - |

As one can see, the new method is `exponentially faster` and requires `zero additional memory` when compared with the current method.

---

## Extend features

In the initial announcement and previews of this feature, the implicit conversion was done without specifying the `u8`:

``` csharp
byte[] array = "hello";  
Span<byte> span = "dog"; 
ReadOnlySpan<byte> span = "cat"; 
```

However, in subsequent previews, the `u8` was added to specifically indicate that the string literal should be converted to _UTF8_. Hopefully in future C# language updates, more encoding methods are added, to at least bring this feature on par with using _System.Text.Encoding.X.GetBytes_.

---

## Notes

A relatively small update on the surface, but if your application makes heavy use of string literals and encoding, converting to this new feature should gain you a performance boost.

---

## References

[Literals - Ignore everything you have seen so far](https://gsferreira.com/archive/2022/csharp-11-utf-8-string-literals-ignore-everything-you-have-seen-so-far/)   
[C# 11 Preview Updates – Raw string literals, UTF-8 and more!](https://devblogs.microsoft.com/dotnet/csharp-11-preview-updates/#utf-8-string-literals)

<?# DailyDrop ?>163: 19-09-2022<?#/ DailyDrop ?>
