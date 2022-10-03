---
title: "Performant deserialization with Utf8JsonReader"
lead: "Using Utf8JsonReader to deserialize a JSON string (to get a specific value)"
Published: "10/18/2022 01:00:00+0200"
slug: "18-utf8jsonreader"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - json
   - deserialization
   - utf8jsonreader

---

## Daily Knowledge Drop

When deserializing a string to a class, if only limited fields on the entity are required or used after the deserialization process, then it might be faster and more performant to use the `Utf8JsonReader` class read te values.

`Utf8JsonReader` is a _high-performance, low allocation, forward-only reader for UTF-8 encoded JSON text_.

---

## Serialization

First, let's set the stage and get a serialized JSON string. In the examples a `Song` class will be used for _serialization_ and _deserialization_:

``` csharp
public class Song
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public DateTime DateCreated { get; set; }
}
```

To serialize a _Song_ instance, `System.Text.Json.JsonSerializer` is used:

``` csharp
var song = new Song
{
    Id = 1,
    Name = "Everlong",
    Artist = "Foo Fighters",
    YearReleased = 1997,
    DateCreated = DateTime.Now
};

var jsonSong = JsonSerializer.Serialize(song);
```

We now have a _json string representation_ of the a Song instance to be used in the below deserialization examples.

---

## Deserialization

In the use cases below, a JSON string is sent to the application (maybe via an API call, or a message broker) but the application is only interested in the `Name` field in the serialized _Song_ instance. We are going to look at the various ways to deserialize the data to get access to that specific field.

---

### System.Text.Json

The first method is to deserialize the entire string into the class, and then access the field - this is usually the _default_ method most developers might use:

``` csharp
var options = new JsonSerializerOptions
{
    AllowTrailingCommas = true
};

var song = JsonSerializer.Deserialize<Song>(strSong, options);
var name = song.Name;

Console.WriteLine(name);
```

This will work, is straight forward - but requires that all fields be deserialized (impacting performance), when only one field is required.

---

### System.Text.Json trimmed

Another method is to create a new _Song_ class, which only contains the interested fields, and deserialize to that class.

First, and _TrimmedSong_ class is created, trimmed of all the fields not required:

``` csharp
public  class TrimmedSong
{
    public string Name { get; set; }
}
```

The deserialization process is exactly the same as the previous method, only the new _TrimmedSong_ class is used instead of _Song_:

``` csharp
var options = new JsonSerializerOptions
{
    AllowTrailingCommas = true
};

// TrimmedSong used here
var song = JsonSerializer.Deserialize<TrimmedSong>(strSong, options);
var name = song.Name;

Console.WriteLine(name);
```

The benefit of this approach is that only the required fields are deserialized, using less memory for fields which are never used.

---

### Utf8JsonReader

The final option we'll look is using the `Utf8JsonReader` class - as mentioned, this is a high-performance, low allocation, forward-only reader.

Using this is a little more complex than a straight deserialization, but not overly so:

``` csharp
// convert the json string, to a byte array
var jsonBytes = Encoding.UTF8.GetBytes(jsonSong);

// setup some optional options
var options = new JsonReaderOptions
{
    AllowTrailingCommas = true,
    CommentHandling = JsonCommentHandling.Skip
};

// instantiate the reader with the byte array
// and the options
var reader = new Utf8JsonReader(jsonBytes, options);

var songName = string.Empty;

// read each token in the json from front to back
// (forward only)
while (reader.Read() && songName == String.Empty)
{
    // switch on the token type read
    switch (reader.TokenType)
    {
        // if its a property
        case JsonTokenType.PropertyName:
            {
                // if its the property we care about
                // (which is the song name)
                if (reader.GetString() == nameof(Song.Name))
                {
                    // get the next token, which is the 
                    // property value
                    reader.Read();
                    // get the token value
                    songName = reader.GetString();
                }
                break;
            }
    }
}
```

The reader will read each token in the json start to finish, while there are tokens to read. During this process, the token type (_PropertyName_) is checked and the token string value (_Name_) is checked until the required property is reached - the value of the property is then read, and the read loop is exited.

---

## Benchmarks

So, how do the methods compare - `BenchmarkDotNet` was used to benchmark each of the above three scenarios:
- Deserialization using `System.Text.Json` into a full class
- Deserialization using `System.Text.Json` into a class with only the require fields
- Reading the json byte array using `Utf8JsonReader` until the required field is found

Using the same snippets of code as above, to get the _Song Name_ from the json string - the results:

|      Method |         Mean |       Error |      StdDev | Ratio |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|------------ |-------------:|------------:|------------:|------:|-------:|-------:|----------:|------------:|
|    FullJson | 517,707.6 ns | 8,451.42 ns | 7,905.46 ns | 1.000 | 2.9297 | 0.9766 |   20471 B |       1.000 |
| TrimmedJson | 149,723.5 ns | 2,904.13 ns | 2,982.33 ns | 0.290 | 1.2207 | 0.4883 |    8787 B |       0.429 |
|  JsonReader |     188.6 ns |     1.28 ns |     1.07 ns | 0.000 | 0.0165 |      - |     104 B |       0.005 |

From the results one can see that the `Utf8JsonReader` is orders of magnitude quicker than the `System.Text.Json` method, while using a fraction of the memory.

---

## Notes

If the entire JSON string is not required to be deserialized, consider using `Utf8JsonReader`. Benchmark its performance against `System.Text.Json` (or _NewtonSoft.Json, or which ever deserialization library is being used) to determine which is more beneficial for the specific use case, as potentially massive performance gains can be had.

---

## References

[Use Utf8JsonReader](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-use-dom-utf8jsonreader-utf8jsonwriter?pivots=dotnet-6-0#use-utf8jsonreader)   

<?# DailyDrop ?>184: 18-10-2022<?#/ DailyDrop ?>
