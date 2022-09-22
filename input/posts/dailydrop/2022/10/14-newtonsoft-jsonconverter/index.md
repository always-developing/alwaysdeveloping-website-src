---
title: "Deserializing incomparable types using JsonConverter"
lead: "How to deserialize incompatible types using Newtonsoft.Json and JsonConverter"
Published: "10/14/2022 01:00:00+0200"
slug: "14-newtonsoft-jsonconverter"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - json
   - newtonsoft
   - jsonconvert
   - datetime
   - datetimeoffset

---

## Daily Knowledge Drop

When using `Newtonsoft.Json` to perform deserialization, the `JsonConverter` class can be used to customize how the data of a specific type is deserialized - this allows to perform custom logic, allowing seemingly incompatible types to be _"compatible"_.

---

## Real world scenario
### Summary

In this use case, `Newtonsoft.Json` is being used to serialize a simplified _source entity_ which looks as follows:

``` csharp
public class SourceEntity
{
    public int Id { get; set; }

    public DateTime DateCreated { get; set; }
}
```

The json string is then being deserialized into a separate _destination entity_ which looks as follows:

``` csharp
public class DestinationEntity
{
    public int Id { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
```

The _DateCreated_ property is defined as `DateTime` in the source and `DateTimeOffset` in the destination - `aren't those two types compatible`?

Generally yes, those two types are compatible and the deserialization/serialization will occur without issue - that is unless the `SourceEntity DateCreated field contains DateTime.MinValue`.

``` csharp
var sourceDate = new SourceEntity
{
    Id = 100,
    // set to minimum value possible
    DateCreated = DateTime.MinValue
};

// serialize the source entity with a DateCreated value
// of DateTime.MinValue
string jsonEntity = 
    Newtonsoft.Json.JsonConvert.SerializeObject(sourceDate);

// this will cause an exception to occur
DestinationEntity destinationDate = Newtonsoft.Json.JsonConvert
    .DeserializeObject<DestinationEntity>(jsonEntity);
```

When this happens, then the deserialization fails with the following error:

``` terminal
An unhandled exception of type 'Newtonsoft.Json.JsonReaderException' occurred in Newtonsoft.Json.dll
Could not convert string to DateTimeOffset: 0001-01-01T00:00:00. Path 'DateCreated', line 1, position 45.
```

To solve this issue, a custom `JsonConvert` implementation can be used.

---

### JsonConvert

To implement a custom converter one needs to inherit from the  the _abstract_ `JsonConverter` class. There are a number of methods to override and implement:

``` csharp
public class DateTimeConverter : JsonConverter
{
    public override bool CanRead => true;
    public override bool CanWrite => false;

    // this converter should only apply when the 
    // destination type being converted is a
    // datetimeoffset    
    public override bool CanConvert(Type objectType) 
        => objectType == typeof(DateTimeOffset);

    // method is called when reading the json
    // this method is only called when the destination 
    // type is DateTimeOffset (as defined by the 
    // CanConvert method)
    public override object ReadJson(JsonReader reader, 
        Type objectType, object existingValue, 
        JsonSerializer serializer)
    {
        // try convert the value to a datetime offset
        if (DateTimeOffset.TryParse(reader.Value.ToString(), 
            out DateTimeOffset result))
        {
            // if it was converted successfully
            return result;
        }

        // return min value if it cannot
        return DateTimeOffset.MinValue;
    }

    // converter should not be used on serialization
    // so don't need to implement this method
    public override void WriteJson(JsonWriter writer, 
        object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
```

---

### Deserialization

When deserializing, none or many converters can be passed into the _DeserializeObject_ method:

``` csharp
var sourceDate = new SourceEntity
{
    Id = 100,
    DateCreated = DateTime.MinValue
};

string jsonEntity = 
    Newtonsoft.Json.JsonConvert.SerializeObject(sourceDate);

// pass in the DateTimeConverter when deserializing
DestinationEntity destinationDate = Newtonsoft.Json.JsonConvert
        .DeserializeObject<DestinationEntity>(jsonEntity, new DateTimeConverter());
```

With the additional of `DateTimeConverter`, the conversion is now performed without any issue.

---

## Notes

While Newtonsoft.Json should, in _most_ cases, be replaced with `System.Text.Json` due to its performance improvements, Newtonsoft.Json still gets _a lot_ of usage due to the depth of functionality and flexibility it offers (although in this specific use case, System.Text.Json does offer the ability to implement custom converters as well).
It is beneficial to be aware of Newtonsoft.Json and how it can be leveraged when System.Text.Json doesn't provide the required functionality.

---

<?# DailyDrop ?>182: 14-10-2022<?#/ DailyDrop ?>
