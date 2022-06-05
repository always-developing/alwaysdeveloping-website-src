---
title: "Linked Lists in C#"
lead: "Having a look at C#'s  built in link list implementation"
Published: 04/02/2022
slug: "04-linked-list"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - linkedlist
    - list

---

## Daily Knowledge Drop

There is a built in `Linked List` implementation in C#, which can be used in certain situations to improve performance.

---

## What is a Linked List?

A `linked list` is a general, linear data structure containing multiple elements, where the elements are linked to each other via pointers (the memory address of the element)

The C# implementation of a linked list is a `double linked list`, meaning each element points to the element in front of it in the list, as well as the element behind it in the lis (vs each element only pointing to the element in front of it in the list)

``` pwoershell
                         NEXT                   NEXT                  NULL
       ┌─┬───────────┬─┐      ┌─┬───────────┬─┐      ┌─┬───────────┬─┐
       │ │           │ ├─────►│ │           │ ├─────►│ │           │ ├───►
NULL   │ │  ELEMENT1 │ │      │ │  ELEMENT2 │ │      │ │  ELEMENT3 │ │
   ◄───┤ │           │ │◄─────┤ │           │ │◄─────┤ │           │ │
       └─┴───────────┴─┘ PREV └─┴───────────┴─┘ PREV └─┴───────────┴─┘
```

Each element points to the element in front of it, except the last element which has a _NULL_ next pointer. Each element also points to the element behind it, except the first element which has a _NULL_ previous pointer.

---

## Example

Using the `LinkedList` in C# is very simple, and similar (but not the same!) to how a normal `List` would be instantiated and used.

The C# `LinkedList` implementation is generic, so the data type it is to hold is specified at instantiation.

``` csharp
var linked = new LinkedList<string>();

linked.AddLast("one");
linked.AddLast("two");
linked.AddLast("three");
linked.AddLast("four");
linked.AddLast("five");

foreach(var item in linked)
{
    Console.WriteLine(item);
}
```

Here a `ListedList` holding _string_ is instantiated, and 5 items added, each time to the end of the list.

The implementation has a _GetEnumerator_ method, so enumeration is available (see [this post for more information on enumeration](../../03/03-getenumerator/))

It is also possible to enumerate through the list, by starting with the first element, and using the _Next_ pointer to move to each element in the list

``` csharp
var linked = new LinkedList<string>();

linked.AddLast("one");
linked.AddLast("two");
linked.AddLast("three");
linked.AddLast("four");
linked.AddLast("five");

var currentItem = linked.First;
while(currentItem != null)
{
    Console.WriteLine(currentItem.Value);
    currentItem = currentItem.Next;
}
```

In both examples above, the output is as follows:

``` powershell
one
two
three
four
five
```

---

## Pros and Cons

There are positives and negatives to using a `LinkedList` when compared to the other list or collection types in C# (and in general). Some points to consider:

- Linked lists are `not indexed`: This means the list is not able to tell which value is at position X directly. The list has to be traversed from the start, until position X is reached to get the value. This can have a performance impact if traversal needs to be done often on a large list.
- `Inserting` also requires `traversing`: If an element needs to be inserted into any position other than the first or last, then the list, again, needs to be traversed to get to the correct location.
- `Adding elements is fast`: Adding an element to the front or end of the list is fast - this is because the `LinkedList` itself is not declared with a preset element count which needs to then be adjusted as new elements are added (like with a List), and more memory allocated. 

---

## Notes

For most uses cases the C# `List<>` will be suitable, and not have a noticeable performance impact - however, the `LinkedList` should be considered when:
- Mostly (only) adding items to the beginning or end of a list
- Items are not accessed except when traversing the entire list (outputting all items)
- Performance matters

---

## References

[Linked List Implementation in C#](https://www.geeksforgeeks.org/linked-list-implementation-in-c-sharp/)  

<?# DailyDrop ?>44: 04-04-2022<?#/ DailyDrop ?>
