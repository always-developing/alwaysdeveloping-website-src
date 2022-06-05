---
title: "Creating a deterministic guid"
lead: "How to create a deterministic guid from a string"
Published: 05/10/2022
slug: "10-deterministic-guid"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - deterministic
    - guid

---

## Daily Knowledge Drop

The `MD5` algorithm implementation in conjunction with `string encoding` can be used to create a `deterministic guid` based on string value.

---

## Use case

Assume you are writing a system, and are using guids as primary keys for all of your data. You are required to consume employee data supplied by a third party - however this data doesn't have a primary key as such, but uses a string `employee number` to unique identify an employee.

The requirement is to repeatedly and consistently be able to convert a specific `employee number` to the `same guid` each time the specific employee record is received (so it can either be created or updated in a database).  

A `deterministic guid` is required - the output Guid will always be the same, as long as the input value remains the same.

---

## Sample Code

``` csharp
using System.Security.Cryptography;
using System.Text;

var employeeId = "EMP001";

// create an instance of the cryptography MD5 algorithm implementation
using var md5Provider = MD5.Create();

// convert the string to a byte[]
var inputByteArray = Encoding.Unicode.GetBytes(employeeId);

// hash the byte array
var hashArray = md5Provider.ComputeHash(inputByteArray);

// create a guid from the hash array
var guid = new Guid(hashArray);
```

The initial employee number byte array (created on line 10) needs to be run through the MD5 algorithm for two main reasons:  
1. `The Guid constructor expects a byte array with length of 16`. The string byte array will be of variable length, depending on the initial string length.  
The _md5Provider.ComputeHash_ method call will always produce a byte array of length 16, which can then be passed to the Guid constructor
1. The _md5Provider.ComputeHash_ method is `irreversible` - this means someone will not be able to take the Guid and reverse the process to find the employee number


---

## In action

Lets simplify the code, and run some employee numbers through the method:

``` csharp
var emp1 = "EMP001";
Console.WriteLine($"Employee Number: '{emp1}' " +
    $"results in a Guid of: '{GetDeterministicGuid(emp1)}'");

var emp2 = "EMP002";
Console.WriteLine($"Employee Number: '{emp2}' " +
    $"results in a Guid of: '{GetDeterministicGuid(emp2)}'");

var emp3 = "EMP003";
Console.WriteLine($"Employee Number: '{emp3}' " +
    $"results in a Guid of: '{GetDeterministicGuid(emp3)}'");

var emp4 = "EMP001";
Console.WriteLine($"Employee Number: '{emp4}' " +
    $"results in a Guid of: '{GetDeterministicGuid(emp4)}'");

Guid GetDeterministicGuid(string input)
{
    using var cryptoServiceProvider = MD5.Create();
    return new Guid(cryptoServiceProvider.ComputeHash(Encoding.Unicode.GetBytes(input)));
}
```

`emp1` and `emp4` have the same employee number (they are the same employee), so we expect the output to be the same Guid.  

This is confirmed by executing the sample:

``` powershell
Employee Number: 'EMP001' results in a Guid of: '7f6edb35-5271-ae41-2b70-4af2c61e721c'
Employee Number: 'EMP002' results in a Guid of: 'ee912e6e-5106-9c37-b81b-45fc3ace9eeb'
Employee Number: 'EMP003' results in a Guid of: 'e7fb42eb-3825-d0a2-f14d-dec8f5766fbf'
Employee Number: 'EMP001' results in a Guid of: '7f6edb35-5271-ae41-2b70-4af2c61e721c'
```

---

## Notes

Very simple, but incredibly useful technique for creating a non-string unique identifier for a record. If the 3rd party record doesn't have a specific field to use a input to the _GetDeterministicGuid_ method, then multiple fields could be concatenated together to unique value for the record.

---

<?# DailyDrop ?>70: 10-05-2022<?#/ DailyDrop ?>
