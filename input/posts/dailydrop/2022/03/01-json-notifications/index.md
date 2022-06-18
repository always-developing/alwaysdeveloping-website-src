---
title: "System.Text.Json notifications"
lead: "Exploring the new .NET6 Json notifications"
Published: 03/01/2022
slug: "01-json-notifications"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - json
    - system.text.json
    - .net6
    - notifications
    
---

## Daily Knowledge Drop

As part of .NET6, `notification` functionality was added to `System.Text.Json`, allowing for custom logic to be invoked during the serialization and deserializing of objects.

---

## New interfaces

Four new interfaces were introduced which can be implemented, according to the requirements:

- **IJsonOnSerialized**
- **IJsonOnSerializing**
- **IJsonOnDeserialized**
- **IJsonOnDeserializing**

To receive notifications, the `class being serialized/deserialized`, needs to `implement one or many of the above interfaces`.

## Example

For the examples below, consider a `Song` class, which has certain required fields in order to be valid.  
Before the `Song` is serialized (to be sent to a message broker, for instance) we need to ensure it is valid, and the same when deserializing it (consuming from a message broker, for instance).

### Validation without notifications

The class contains a _ValidateSong_ method which will throw an exception if the class is not in a valid state.

``` csharp
public class Song
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int LengthInSeconds { get; set; }

    public void ValidateSong()
    {
        var errorList = new List<string>();

        if(string.IsNullOrEmpty(Name))
        {
            errorList.Add("'Name' field is required");
        }

        if (string.IsNullOrEmpty(Artist))
        {
            errorList.Add("'Artist' field is required");
        }

        if(YearReleased <= 1900)
        {
            errorList.Add("'YearReleased' must be greater than 1900");
        }

        if(errorList.Count > 0)
        {
            throw new InvalidOperationException(
                string.Join(Environment.NewLine, errorList));
        }
    }
}
```

With this approach, before serialization and after deserialization, the _ValidateSong_ method will need to be manually invoked to ensure the instance is valid.

``` csharp
var song = new Song
{
    Id = 1,
    Artist = "Foo Fighters",
    LengthInSeconds = 250
};

song.ValidateSong();

var json = JsonSerializer.Serialize(song, 
    new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
```

With the required `Name` and `YearReleased` fields **not** supplied, the above results in the following output:

``` powershell
Unhandled exception. System.InvalidOperationException: 
    'Name' field is required
    'YearReleased' must be greater than 1900
   at JsonNotifications.Song.ValidateSong() in 
        C:\Development\Projects\Blog\JsonNotifications\JsonNotifications\Song.cs:line 46
   at Program.<Main>$(String[] args) in 
        C:\Development\Projects\Blog\JsonNotifications\JsonNotifications\Program.cs:line 14
```

---

### Validation with notifications

The new `System.Text.Json` interfaces can be leveraged to automate and simplify the validation process.

The updated `Song` class:

``` csharp
// The  IJsonOnDeserialized and IJsonOnSerialized interfaces are implemented
public class Song : IJsonOnDeserialized, IJsonOnSerialized
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int LengthInSeconds { get; set; }

    // The two methods specified by the two interfaces are implemented. 
    // Both will call the ValidateSong method
    public void OnDeserialized() => ValidateSong();

    public void OnSerialized() => ValidateSong();

    // Method has been made private (this could have been kept 
    // public to still allow for external validation though)
    private void ValidateSong()
    {
        var errorList = new List<string>();

        if(string.IsNullOrEmpty(Name))
        {
            errorList.Add("'Name' field is required");
        }

        if (string.IsNullOrEmpty(Artist))
        {
            errorList.Add("'Artist' field is required");
        }

        if(YearReleased <= 1900)
        {
            errorList.Add("'YearReleased' must be greater than 1900");
        }

        if(errorList.Count > 0)
        {
            throw new InvalidOperationException(
                string.Join(Environment.NewLine, errorList));
        }
    }
}
```

With the relevant `IJsonOnDeserialized` and `IJsonOnSerialized` interfaces implemented, the respective `OnDeserialized` and `OnSerialized` methods are now invoked automatically during the serialization/deserialization process, which in turn invokes the _ValidateSong_ method.

``` csharp
var song = new Song
{
    Id = 1,
    Artist = "Foo Fighters",
    LengthInSeconds = 250
};

var json = JsonSerializer.Serialize(song, 
    new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
```

The output is the same as before (but with a different stacktrace)

``` powershell
Unhandled exception. System.InvalidOperationException: 
'Name' field is required
'YearReleased' must be greater than 1900
   at JsonNotifications.Song.ValidateSong() in 
    C:\Development\Projects\Blog\JsonNotifications\JsonNotifications\Song.cs:line 46
   at JsonNotifications.Song.OnSerialized() in 
    C:\Development\Projects\Blog\JsonNotifications\JsonNotifications\Song.cs:line 23
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1
    .OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1
    .TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1
    .WriteCore(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.JsonSerializer.WriteUsingSerializer[TValue](Utf8JsonWriter writer, TValue& value, JsonTypeInfo jsonTypeInfo)
   at System.Text.Json.JsonSerializer.WriteStringUsingSerializer[TValue](TValue& value, JsonTypeInfo jsonTypeInfo)
   at System.Text.Json.JsonSerializer.Serialize[TValue](TValue value, JsonSerializerOptions options)
   at Program.<Main>$(String[] args) in C:\Development\Projects\Blog\JsonNotifications\JsonNotifications\Program.cs:line 16
```

The same would be experienced during deserialization, if the JSON being deserialization results in a `Song` with an invalid state, the exception will be thrown.

---

## Notes

A very useful addition to the System.Text.Json suite of functionality - which has many possibilities beyond the above simple use case.  

This approach does creates a hard dependency on System.Text.Json in the entity though - maybe not a problem, but it would need to be kept in mind and and an informed choice made for each use case.

---

## References
[Entity Framework Core 6 features - Notifications for (De)Serialization](https://blog.okyrylchuk.dev/system-text-json-features-in-the-dotnet-6#heading-notifications-for-deserialization)

<?# DailyDrop ?>21: 01-03-2022<?#/ DailyDrop ?>
