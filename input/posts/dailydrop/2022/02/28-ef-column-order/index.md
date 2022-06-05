---
title: "EF Core 6 column order"
lead: "Manually order columns in EF Core 6"
Published: 02/28/2022
slug: "28-ef-column-order"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - dailydrop
    - efcore
    - efcore6
    - entityframework
    - column
    - order
    - columnorder
    
---

## Daily Knowledge Drop

The `Column` attribute, as well as the new `HasColumnOrder` fluent API can be used to determine the specifically order of sequence of columns in the table.

---

## Configurations previously

In previous version of Entity Framework Core (EF) the order in which the columns were defined on the entity, where the order in which they were created on the table

``` csharp
public class Song
{
    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int Id { get; set; }

    public int LengthInSeconds { get; set; }

    public string Name { get; set; }
}
```

The _Up_ method migration created to create the table would look as below.  
The exception is the **Id primary key (PK)** field which is automatically put first in the list as it's a PK.

``` csharp
migrationBuilder.CreateTable(
    name: "Songs",
    columns: table => new
    {
        Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
        YearReleased = table.Column<int>(type: "int", nullable: false),
        LengthInSeconds = table.Column<int>(type: "int", nullable: false),
        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Songs", x => x.Id);
    });
```

---

## Configuring the order

### Column attribute

The `Column` attribute can now be used to decorate the entity and specify the column order.

``` csharp
public class Song
{
    public string Artist { get; set; }

    [Column(Order = 1)]
    public int YearReleased { get; set; }

    public int Id { get; set; }

    [Column(Order = 99)]
    public int LengthInSeconds { get; set; }

    [Column(Order = 0)]
    public string Name { get; set; }
}
```

The migration now looks as follows:

``` csharp
migrationBuilder.CreateTable(
    name: "Songs",
    columns: table => new
    {
        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
        YearReleased = table.Column<int>(type: "int", nullable: false),
        LengthInSeconds = table.Column<int>(type: "int", nullable: false),
        Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Artist = table.Column<string>(type: "nvarchar(max)", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Songs", x => x.Id);
    });
```

As you can see the `Column` attribute takes precedent over any columns without the attribute, and are ordered lowest to highest. The _Id PK column_ still gets automatically ordered to the top of the columns without any ordering specified.

If specifically marked with the `Column` attribute and an order, the order of the _PK_ column would be the order manually specified.

---

### HasColumnOrder method

If the entity can't be changed and the `Column` attribute added (e.g. its a 3rd party class) the fluent API can be used to specify the column order.

Going back to the entity without any `Column` attributes:

``` csharp
public class Song
{
    public string Artist { get; set; }

    public int YearReleased { get; set; }

    public int Id { get; set; }

    public int LengthInSeconds { get; set; }

    public string Name { get; set; }
}
```

Instead now in the _OnModelCreating_ method of the DbContext OR in a separate _IEntityTypeConfiguration_ implementation for the table, the column is manually specified using the `HasColumnOrder` method.

``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Song>()
        .Property(p => p.YearReleased)
        .HasColumnOrder(1);

    modelBuilder.Entity<Song>()
        .Property(p => p.LengthInSeconds)
        .HasColumnOrder(99);

    modelBuilder.Entity<Song>()
        .Property(p => p.Name)
        .HasColumnOrder(0);
}
```

The migration generated is as follows:

``` csharp
migrationBuilder.CreateTable(
    name: "Songs",
    columns: table => new
    {
        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
        YearReleased = table.Column<int>(type: "int", nullable: false),
        LengthInSeconds = table.Column<int>(type: "int", nullable: false),
        Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Artist = table.Column<string>(type: "nvarchar(max)", nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Songs", x => x.Id);
    });
```

As expected, its the same as using the `Column` attribute.

---

## Notes

This new feature, while not ground-breaking, is another very useful small tool in the EF suite to allow for customization in how the database is scaffolded - and more (_as simple as possible_) configuration options is always a welcome addition.

---

## References
[Entity Framework Core 6 features - Part 1](https://blog.okyrylchuk.dev/entity-framework-core-6-features-part-1)

<?# DailyDrop ?>20: 28-02-2022<?#/ DailyDrop ?>
