---
title: "Supporting 13 months"
lead: ".NET has support for 13 months"
Published: "11/23/2022 01:00:00+0200"
slug: "23-thirteen-months"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - datetime
   - months

---

## Daily Knowledge Drop

.NET has support for `13 months` - the _Hebrew_ and _EastAsianLunisolarCalendar_ for example, has the concept of leap months, and as such it is possible for a year to consist of 13 months.

---


## GetMonthName

The `GetMonthName` method on `DateTimeFormatInfo` supports a month parameter value of between 1 and 13.

For calendars which only support 12 months, sending a value of 13 to the `GetMonthName` method will return a blank string:

``` csharp
// this will use the current 
// culture calendar
var df = new DateTimeFormatInfo();

Console.WriteLine(df.GetMonthName(1));
Console.WriteLine(df.GetMonthName(13));
```

The output of this is:

``` terminal
January

```

However, if we switch using a _Calendar_ which supports 13 months:

``` csharp
// switch the culture and the calendar
HebrewCalendar hc = new HebrewCalendar();
CultureInfo culture = CultureInfo.CreateSpecificCulture("he-IL");
culture.DateTimeFormat.Calendar = hc;
Thread.CurrentThread.CurrentCulture = culture;

// output to the debug window as my Console did not
// support the characters
Debug.WriteLine(culture.DateTimeFormat.GetMonthName(1));
Debug.WriteLine(culture.DateTimeFormat.GetMonthName(13));
```

Now the output is as follows:

``` terminal
תשרי
אלול
```

The Debug output is used here instead of the Console, as my default Console Window encoding did not support Hebrew characters (while the Visual Studio output window does by default)

---

## Notes

This piece of knowledge is not especially useful or relevent unless one is working with one of the specific calendars/cultures, or one has to support multiple cultures in the application.
It also serves as a reminder that the culture an application executes under is important, and not to always make assumptions about how the application will always operate.

---


## References

[Misconceptions: 1 year = 12 months](https://www.meziantou.net/misconceptions-about-date-and-time.htm)  

<?# DailyDrop ?>208: 23-11-2022<?#/ DailyDrop ?>
