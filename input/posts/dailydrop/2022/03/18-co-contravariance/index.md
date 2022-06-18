---
title: "Covariance and Contravariance in C#"
lead: "Covariance and Contravariance in C# explained with examples"
Published: 03/18/2022
slug: "18-co-contravariance"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - covariance
    - contravariance
    - generics

---

## Daily Knowledge Drop

The way in which base and inherited classes can automatically be cast up or down the hierarchy (depending on the situation) is referred to `covariance` and `contravariance`.

Most developers have probably used the concepts of `covariance` and `contravariance` in their code, perhaps without even realising it. Looking at a some examples, will help explain in a bit more detail.

---

## Base setup

In the examples below, the following hierarchy of classes is used:

``` csharp
public class Vehicle { }

public class LandVehicle : Vehicle { }

public class SeaVehicle : Vehicle { }

public class Car : LandVehicle { }

public class Boat : SeaVehicle { }
```

## Contravariance

`Contravariance` applies to types going _in_, in other words `parameters to methods`.

`Contravariance` allows for `methods with a parameter of a base class, to accept any type derived from the base class`.

Summary: `Contravariance` => `IN` => `parameter declared as base, but derived can be used`

An example:

``` csharp
// declare a lambda function which takes in a Vehicle 
var func = (Vehicle veh) => Console.WriteLine(veh.ToString());

// the function will accept all types of vehicles
func(new Vehicle());
func(new Car());
func(new SeaVehicle());

```

The lambda is declared with a parameter of _Vehicle_, but any type of derived vehicle will be accepted.

Because the lambda takes in a _Vehicle_ if there is a method or property _specific to a child_ which needs to be invoked, the `Vehicle type needs to be checked and downcast the derived type`

``` csharp
// the function will accept all types of vehicles
OuputExtendedDetails(new Vehicle());
OuputExtendedDetails(new Car());
OuputExtendedDetails(new SeaVehicle());

// parameter of type Vehicle
public void OuputExtendedDetails(Vehicle veh)
{
    // each IF statement will try downcast and assign to the variable (lv in below case)
    // if allowed to do so
    if(veh is LandVehicle lv)
    {
        Console.WriteLine($"{lv.GetType()} travels on land");
    }

    if (veh is Car car)
    {
        Console.WriteLine($"{car.GetType()} travels on land");
    }

    if (veh is SeaVehicle sv)
    {
        Console.WriteLine($"{sv.GetType()} travels on sea");
    }

    if (veh is Boat boat)
    {
        Console.WriteLine($"{boat.GetType()} travels on sea");
    }

    Console.WriteLine("Vehicle can travel");
}
```

The output is as follows:

``` powershell
Vehicle can travel
Car travels on land
Car travels on land
Vehicle can travel
SeaVehicle travels on sea
Vehicle can travel
```

A _Car_ for instance:
- is a _Vehicle_ so can be passed into the method
- is a _LandVehicle_ so ouput is written
- is a _Car_ so ouput is written

In short, this is `contravariance` - `the ability to use a derived class as a parameter, where a base class has been specified`.

---

## Covariance

`Covariance` applies to types coming _out_, in other words `return types from methods` or assignments.

`Covariance` allows for `passing back a derived type where a base type is expected`.

Summary: `Covariance` => `OUT` => `type declared as derived, but base can be used`

``` csharp
// Even though a Car and Boat
// are being returned, they are assigned to Vehicle
Vehicle car = GetCar();
Vehicle boat = GetBoat();

// get a Car
public Car GetCar()
{
    return new Car();
}

// get a Boat
public Boat GetBoat()
{
    return new Boat();
}
```

Here, the return types are _Car_ and _Boat_, but they can both be assigned to a variable of type _Vehicle_.

This is useful, for example, when we want to have a list of _Vehicles_. The list is declared of type _Vehicle_, and therefor can hold any type of vehicle:

``` csharp
// add a Car and Boat to a Vehicle list
var vehList = new List<Vehicle>();
vehList.Add(GetCar());
vehList.Add(GetBoat());

// output the items
foreach(var veh in vehList)
{
    Console.WriteLine(veh.ToString());
}

public Car GetCar()
{
    return new Car();
}

public Boat GetBoat()
{
    return new Boat();
}
```

The output is as follows:

``` powershell
Car
Boat
```

In short, this is `covariance` - `the ability to assign a derived type, to its base class`.

---

## Contravariance - Generics

`Contravariance` can also be applied to _Generics_, using the `in` keyword.  

Using the `in` keyword allows for the usage of a `less derived type than the one specified by the generic parameter`.

Consider the following setup (without the _in_ keyword):

``` csharp
public interface ITravel<TVehicle> { }

public class Travel<TVehicle> : ITravel<TVehicle> { }
```

Using the above setup, the following will **NOT** compile:

``` csharp
// An instance of ITravel<Car> is declared and the 
// MoveCar method is called without issue
var carTravel = new Travel<Car>();
MoveCar(carTravel); 

// An instance of ITravel<LandVehicle> is declared and the the 
// MoveCar method is tried to be called. This is NOT ALLOWED.
var landTravel = new Travel<LandVehicle>();
MoveCar(landTravel); // THIS IS NOT ALLOWED

// A method is declared which takes an instance of ITravel<Car>
public void MoveCar(ITravel<Car> travel)
{
    Console.WriteLine(travel);
}
```

As the generic parameter is not declared with the `in` keyword, it is not `contravariant`. By just adding the `in` keyword, the above will be allowed:

``` csharp
public interface ITravel<in TVehicle> { }

public class Travel<TVehicle> : ITravel<TVehicle> { }
```

Now, any type which is `less derived than Car` can be used instead of _Car_. The below is now 100% valid and will compiled with any issue:

``` csharp
var carTravel = new Travel<Car>();
MoveCar(carTravel);

var landTravel = new Travel<LandVehicle>();
MoveCar(landTravel);

var vehTravel = new Travel<Vehicle>();
MoveCar(landTravel);

public void MoveCar(ITravel<Car> travel)
{
    Console.WriteLine(travel);
}
```

The output:

``` powershell
Travel`1[Car]
Travel`1[LandVehicle]
Travel`1[Vehicle]
```


---

## Covariance - Generics

`Covariance` can also be applied in _Generics_, using the `out` keyword.

Using the `out` keyword allows for the usage of a `more derived type than the one specified by the generic parameter`.

Consider the following setup (without the _out_ keyword):

``` csharp
interface ITravel<TVehicle> { }

class Travel<TVehicle> : ITravel<TVehicle> { }
```

Using the above setup, the following will **NOT** compile:

``` csharp
ITravel<Vehicle> veh = new Travel<Vehicle>();
ITravel<Car> car = new Travel<Car>(); 

// THIS IS NOT ALLOWED
veh = car;
```

The instance of _Travel\<Car\>_ (a more derived type) **cannot**  cannot be assigned to a variable using type _Vehicle_ (a less derived type).

As the generic parameter is not declared with the `out` keyword, it is not `covariant`. By just adding the `out` keyword to the generic parameter, the above will be allowed:

``` csharp
interface ITravel<out TVehicle> { }

class Travel<TVehicle> : ITravel<TVehicle> { }
```

Now, any type which is a `more derived type than Vehicle` can be used instead of _Vehicle_. The below is now 100% valid and will compiled with any issue:

``` csharp
ITravel<Vehicle> veh = new Travel<Vehicle>();
ITravel<Car> car = new Travel<Car>();
ITravel<SeaVehicle> sea = new Travel<SeaVehicle>();

Console.WriteLine(veh);
Console.WriteLine(car);

veh = car;
Console.WriteLine(veh);

veh = sea;
Console.WriteLine(veh);
```

The output:

``` powershell
Travel`1[Vehicle]
Travel`1[Car]
Travel`1[Car]
Travel`1[SeaVehicle]
```

---

## IEnumerable - Generics

The above examples, while doing a satisfactory job in demonstrating the functionality, are not concrete examples.  

So lets have a quick look at the .NET IEnumerable<> class, which is defined with the `out` keyword:

``` csharp
namespace System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
    {
        // removed for brevity
    }
}
```

This allows IEnumerable<> to be used as follows:

``` csharp
IEnumerable<Car> cars = new List<Car>();
IEnumerable<Vehicle> vehicle = cars;
```

This is `covariance` in action - `more derived type` can be used and assigned to a `less derived type`.

---

## Notes

In summary:
- `Contravariance` => `in` => `parameter declared as base, but derived can be used`
- `Covariance` => `out` => `type declared as derived, but base can be used`

Knowing about `covariance` and `contravariance`, especially when it comes to generics, is useful to know and can be leveraged to reduce duplicate code.

---

## References
[Out (generic modified)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-generic-modifier)  
[In (generic modified)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-generic-modifier)  
[Covariance and Contravariance in C# Explained](https://blog.ndepend.com/covariance-and-contravariance-in-csharp-explained/)

<?# DailyDrop ?>34: 18-03-2022<?#/ DailyDrop ?>
