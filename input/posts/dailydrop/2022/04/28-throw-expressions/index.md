---
title: "Using throw expressions in C#"
lead: "What are throw expression, and how and when can they be used?"
Published: 04/28/2022
slug: "28-throw-expressions"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - throw
    - expressions

---

## Daily Knowledge Drop

`Throw expressions`, introduced in C#7, enabled `throw` to be used as an expression as well as a statement. This allowed `throw` to be used in contexts which previously where not supported, specifically:
- Conditional operator
- Null-coalescing expression
- Expression-bodied method

---

## Examples

``` csharp
class Song
{
    public string Name { get; set; }

    public string Artist { get; set; }

    // Null-coalescing expression    
    public Song(string name) => Name = name ?? 
        throw new ArgumentNullException(nameof(name));

    public string GetOutputString()
    {
        // Conditional operator
        return !string.IsNullOrEmpty(Artist) ? 
            $"{Name} by {Artist}" : throw new ArgumentNullException(nameof(Artist));
    }

    // Expression-bodied method
    public void SaveSong() => throw new NotImplementedException();
}
```

- `Null-coalescing expression`: An example of a throw statement being used with a conditional expression.

- `Conditional operator`: An example of a throw statement being used in a null-coalescing expression. Generally each operand (the left and right hand side of the : symbol, however in this case a throw expression is allowed.

- `Expression-bodied method`: An example of a throw statement being an expression-body for a method. The entire body of the expression is just a throw statement. 

---

## Notes

A neat, small, but useful feature to be aware of, and aware of the situations when its possible to use the `throw expression`. 

---

## References

[The throw expression](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression)  
[Using throw-expressions in C# 7.0](https://www.codingame.com/playgrounds/4138/using-throw-expressions-in-c)  

<?# DailyDrop ?>62: 28-04-2022<?#/ DailyDrop ?>
