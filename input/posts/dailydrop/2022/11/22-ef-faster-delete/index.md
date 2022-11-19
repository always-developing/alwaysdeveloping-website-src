---
title: "Efficient EF deletion"
lead: "Exploring a more efficient method to perform deletes in Entity Framework"
Published: "11/22/2022 01:00:00+0200"
slug: "22-faster-delete"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ef
   - entityframework
   - delete

---

## Daily Knowledge Drop

When using Entity Framework to perform a delete (prior to EF7), first the record in question is required to be _selected from the table_ (so that EF change tracking has visibility of it), then is _marked as deleted_, and then the actual database delete is performed with  _SaveChanges_.

Khalid Abuhakmeh has a very interesting idea, which eliminates the need for the extra round trip to _select_ the data - manually add the item to EF change tracking (without first checking the database), _mark it as deleted_, and _save the changes_. If a specific exception occurs because the record is not available for deletion, handle that separately.

---

## Use case

In our use case, we will define a _Delete_ endpoint, which when called will delete the _Blog_ record using the supplied _Id_. 

### Traditional method

Below is a sample of the "traditional" method of handling the use case:

``` csharp
// context is injected from the DI container
// id is supplied as part of the url
app.MapDelete("/blog/v1/{id:int}", async (DemoContext context, int id) =>
{
    // first lookup the blog by the id
    var blog = await context.Blogs.FindAsync(id);

    // if the blog was found
    if(blog != null)
    {
        // mark it as deleted
        context.Remove(blog);
    }

    await context.SaveChangesAsync();

});
```

1. First, a query is performed against the database to ensure that a _Blog_ with the specified Id exists (with _FindAsync_)
1. In the case when the record does exist, the row will start being tracked by Entity Framework's change tracker
1. The record is flagged as delete, with the _Remove_ command
1. The changes are finally applied to the database, with _SaveChangesAsync_

With this method, there are two round trips to the database - in steps 1 and 4.

---

### Efficient Method

With the more efficient method, one database round trip can be eliminated:

``` csharp
// context is injected from the DI container
// id is supplied as part of the url
app.MapDelete("/blog/v1/{id:int}", async (DemoContext context, int id) =>
{
    try
    {
        // create an instance of the object with the
        // supplied id. 
        var blog = new Blog { Id = id };
        // make the change tracker aware of this object
        var contextBlog = context.Blogs.Attach(blog);
        // mark it as deleted
        contextBlog.State = EntityState.Deleted;

        // delete
        await context.SaveChangesAsync();
    }
    catch(DbUpdateConcurrencyException ex)
    {
        Console.WriteLine("Swallowing delete exception.");
    }
});
```

With this method:
1. Instead of checking in the database that a record exists with the supplied _Id_, it is _assumed a record does already exist_ with the id
2. A record, with just the Id (the primary key value) set, is created, attached to the change tracker, and marked as deleted
3. _SaveChangesAsync_ is then called to perform the actual delete on the database
4. If a record with that specific Id does in fact not exist, an exception will be thrown - which is caught and swallowed (with logging)

The outcome in both cases is the same, but an expensive database round trip has been eliminated.

In the references link below, Khalid Abuhakmeh has additional ideas and code samples on how this functionality can be cleaned and wrapped into an extension method to make usage even easier.

---

## Notes

Having run into this exact issue before, this method is very interesting. Exceptions do have an overhead, but the overhead of a database round trip will almost always out-weigh the exception overhead. Definitely consider the "efficient" method if the "traditional pattern is used throughout code.
Having said that, EF7 introduces [Bulk Update/Delete functionality](../../09/23-ef-bulk-update/) which will allow for this nativity in the EF framework - so this method will become obsolete with time.

---


## References

[More Efficient Deletes With Entity Framework Core](https://khalidabuhakmeh.com/more-efficient-deletes-with-entity-framework-core)  

<?# DailyDrop ?>207: 22-11-2022<?#/ DailyDrop ?>
