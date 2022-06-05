---
title: "Visual Studio Code snippets"
lead: "Having a look at some of the code snippets shipped with Visual Studio"
Published: 05/20/2022
slug: "20-vs-snippets"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - visualstudio
    - snippet

---

## Daily Knowledge Drop

There are a number of code snippets shipped with Visual Studio which can leveraged to automatically create common boilerplate code.

It is possible to also created custom snippets - but this post will look at some of the more useful ones which ship with Visual Studio

---

## Snippets

All of the snippets mentioned below (as well as custom snippets) are invoked by typing the `relevant shortcut` and pressing `tab-tab`.  

In some snippets, after `tab-tab` has been pressed and the default snippet inserted, one is also able to change certain values from the defaults. After the snippet is inserted, the first editable portion will be highlighted. Typing will change the highlighted value - once done, `tab` or `tab-tab` will move onto the next editable portion (depending on the snippet, sometimes tab moves, sometimes double tab moves the highlighting)

### Constructor

The `ctor` shortcut can be used to create a constructor for a class:

``` csharp
// before
public class SnippetDemo
{

}

// after
public class SnippetDemo
{
    public SnippetDemo()
    {

    }
}
```

---

### Property

The `prop` shortcut can be used to create a property:

``` csharp
// before
public class SnippetDemo
{

}

// after
public class SnippetDemo
{
    public Guid Id { get; set; }
}
```

With this snippet, when `tab-tab` is pressed, the default property is `public int MyProperty { get; set; }`. Tabbing after the default snippet is inserted will allow changing of the property type and property type.

---

The `propfull` shortcut can be used to create a property with a backing field:

``` csharp
// before
public class SnippetDemo
{

}

// after
public class SnippetDemo
{
    private int _id;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
}
```

As with the `prop` snippet, the type and name can be set after the default snippet has been inserted.

---

### For loop

The `for` shortcut can be used to create a for loop:

``` csharp
// before
public void SnippetMethod()
{

}

// after
public void SnippetMethod()
{
    for (int i = 0; i < length; i++)
    {

    }
}
```

Tabbing after the default snippet is inserted will allow changing of the indexer (_i_) name, as well as set the value of _length_.

---

### Try statement

The `try` shortcut can be used to create a try-catch block:

``` csharp
// before
public void SnippetMethod()
{

}

// after
public void SnippetMethod()
{
    try
    {

    }
    catch (Exception)
    {

        throw;
    }
}
```

---

`tryf` can be used to create a try-finally block:

``` csharp
// before
public void SnippetMethod()
{

}

// after
public void SnippetMethod()
{
    try
    {

    }
    finally
    {

    }
}
```

---

### Exception

The `exception` shortcut can be used to create a custom exception. The default snippet is as follows:

``` csharp
[Serializable]
public class MyException : Exception
{
    public MyException() { }
    public MyException(string message) : base(message) { }
    public MyException(string message, Exception inner) : base(message, inner) { }
    protected MyException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
```

---

## Others

There are many other snippets available out of the box with Visual Studio - which can be explored in Visual Studio by going to `Tools => Code Snippet Manager`. Most of the common snippets (like those mentioned in this post) can be found under the `CSharp` option in the language dropdown, `Visual C#` folder.

---

## Notes

There are a large number of very useful snippets which come bundled with Visual Studio which can assist with writing code. There is the obvious benefit of less typing and faster output of boilerplate code, but it also enforces some standards on the code base (if everyone is using the same snippets).  

Then there is the added benefit of the ability to create ones own snippets to speed up often used custom blocks of code (not covered here)

---

## References

[Visual Studio 2022 Tips & Tricks: Using Code Snippets](https://www.claudiobernasconi.ch/2022/04/06/visual-studio-2022-tips-and-tricks/)  

<?# DailyDrop ?>78: 20-05-2022<?#/ DailyDrop ?>
