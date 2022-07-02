---
title: "SQL wildcard characters"
lead: "Diving into the SQL wildcard characters (beyond %)"
Published: "07/20/2022 01:00:00+0200"
slug: "20-sql-wildcards"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - sql
    - wildcard

---

## Daily Knowledge Drop

There are a number of SQL wildcard characters, apart from the commonly used (at least in my case) `zero or more characters, %` wildcard. These include:
- `_` - representing a single character
- `[]` - representing any single character within the brackets
- `^` - representing any character not in the brackets
- `-` - representing any single character in the specified range

---

## Table setup

We have a simple `Word` table, with one column, also called `Word`. The table is populated with 5 records:

``` terminal
flout
hit
hot
hut
shout
```

---

## Usage

### Single character

The `_` is used to represent a single character.

The following will select any `three letter words which start with an 'h' and end with a 't'`:

``` sql
SELECT * 
FROM Word
WHERE Word LIKE 'h_t'
```

The result:

``` terminal
hit
hot
hut
```

---

### Single character array

The `[]` characters are used to represent any single character specified within the brackets.

The following will select any `three letter words which start with an 'h', have an 'o' OR 'u' as the middle letter, and which end with a 't'`:

``` sql
SELECT * 
FROM Word
WHERE Word LIKE 'h[ou]t'
```

The result:

``` terminal
hot
hut
```

To reiterate,  only a `single character` from within the brackets is matched. For example

``` sql
SELECT * 
FROM Word
WHERE Word LIKE 'sh[ou]t'
```

No rows will be returned, as `shout` is NOT a match. Only `shot` and `shut` would be matched.

---

### Character array negation

The `^` character is used, in conjunction with the characters in the brackets `[]`, as a negation.

The following will select any `three letter words which end in 'out', but NOT preceded by an 'l'`:

``` sql
SELECT * 
FROM Word
WHERE Word LIKE '%[^l]out'
```

The result:

``` terminal
shout
```

The negation applies to all character in the brackets.

The following will select `any three letter word, which starts with an 'h', does NOT contain 'o' or 'u' as the middle letter, and ends in 't'`.

``` sql
SELECT * 
FROM Word
WHERE Word LIKE 'h[^ou]t'
```

The result:

``` terminal
hit
```

---

### Character range

The `-` character is used in conjunction with the characters in the brackets `[]`, as a range specifier.

The following will select any `three letter words starting with 'h', has any character in the alphabet between, and including, 'o' and 'u' in the middle, and ending in 't'`:

``` sql
SELECT * 
FROM Word
WHERE Word LIKE 'h[o-u]t'
```

The result:

``` terminal
hot
hut
```

---

### Multiple characters

A quick look at the more common `%` character, which is used to represent zero or more characters.

The following will select any `words starting with zero or more of any characters, followed by an 'h', followed by zero or more of any characters, and finally ending with a 't'`:

``` sql
SELECT * 
FROM Word
WHERE Word LIKE '%h%t'
```

The result:

``` terminal
hit
hot
hut
shout
```

---

## Notes

Personally, I've never had any specific need or requirement for any of these wildcards, apart from `%` -  however, they all have their place. With the knowledge of their existence, one is at least equipped to determine if any can add any value with each specific use case or requirement.

---

## References

[SQL Wildcard Characters](https://www.w3schools.com/sql/sql_wildcards.asp)   

---

<?# DailyDrop ?>120: 20-07-2022<?#/ DailyDrop ?>
