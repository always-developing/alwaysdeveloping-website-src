---
title: "Request binding with IParseable"
lead: "Using IParseable to bind primitive types to more complex entities"
Published: "01/12/2023 01:00:00+0200"
slug: "12-endpoint-iparseable"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - endpoint
   - iparseable
   - bind

---

## Daily Knowledge Drop

In a [previous post](../../../2022/12/05-iparseable/) we had a look at the `IParseable interface` which allows for a `string to be parsed into a type`.

This functionality can be leveraged with minimal API's to _automatically parse a query string to a complex type_.

---

## IParseable request

In this example, the _Song_ entity implements the _IParsable\<Song\>_ interface (more details in the [previous post about IParseable](../../../2022/12/05-iparseable/)):

``` csharp
public class Song : IParsable<Song>
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public int LengthInSeconds { get; set; }

    private Song(string name, string artist, int lengthInSeconds)
    {
        Name = name;
        Artist = artist;
        LengthInSeconds = lengthInSeconds;
    }

    public static Song Parse(string s, IFormatProvider? provider)
    {
        string[] songPortions = s.Split(new[] { '|' });

        if (songPortions.Length != 3) 
        { 
            throw new OverflowException("Expect format: Name|Artist|LengthInSeconds"); 
        }

        return new Song(songPortions[0], songPortions[1], Int32.Parse(songPortions[2]));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Song result)
    {
        result = null;
        if (s == null) 
        { 
            return false; 
        }

        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch { return false; }
    }
}
```

This interface implementation allows for a string value (in a specific format), to be converted to a `Song instance`:

``` csharp
Song song = "Everlong|Foo Fighters|326".Parse<Song>();
```

---

## Minimal endpoint

### Manual parsing

If we want an endpoint which accepts a _Song_ as a parameter, one option is to have the request entity `as a string` and perform the conversion to _Song_ manually:

``` csharp
app.MapGet("/song", ([FromQuery]string song) =>
{
    // perform the conversion from string
    // to Song manually, using the IParsable
    // interface
    return details.Parse<Song>();
});
```

Calling the endpoint with a _song query string_ `/song?song=Everlong|Foo Fighters|326`, results in the following response:

``` json
{"name":"Everlong","artist":"Foo Fighters","lengthInSeconds":326}
```

This is a valid approach, but because the _Song class_ implements _IParseable_, the conversion can be done automatically!

---

### Automatic parsing

A slightly easier approach that manually doing the conversions, is allowing it to happen automatically. Changing the parameter type from _string_ to _Song_:

``` csharp
// parameter is Song instead of string
app.MapGet("/song", ([FromQuery] Song song) =>
{
    return song;
});
```

Calling the same endpoint with a _song query string_ `/song?song=Everlong|Foo Fighters|326`, results in the same response:

``` json
{"name":"Everlong","artist":"Foo Fighters","lengthInSeconds":326}
```

As `Song implement IParsable, the string parameter is automatically parsed to a Song instance`.


---

## Notes

This is a small quality of life feature which makes working with the _IParsable_ interface and minimal endpoints easier and more streamline.

---

## References

[5 new MVC features in .NET 7](https://andrewlock.net/5-new-mvc-features-in-dotnet-7/#1-iparseable-tryparse-for-primitive-binding)  

<?# DailyDrop ?>241: 12-01-2023<?#/ DailyDrop ?>
