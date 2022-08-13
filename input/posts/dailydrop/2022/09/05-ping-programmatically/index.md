---
title: "Performing a ping programmatically"
lead: "C# has a Ping class which enables Ping operations programmatically"
Published: "09/05/2022 01:00:00+0200"
slug: "05-ping-programmatically"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - ping

---

## Daily Knowledge Drop

A `Ping` class is available in the _System.Net.NetworkInformation_ namespace - this class facilitates performing _ping_ operations (as the name suggests), on a hostname or IP address from code.

---

## Usage

The usage of the class is straightforward - declare an instance of `Ping`, and call the _SendPingAsync_ method:

``` csharp
using System.Net.NetworkInformation;

var looping = true;

// format asynchronously on a separate thread
Task.Run(async () =>
{
    // create an instance of the Ping class
    Ping pinger = new Ping();

    // loop until the user presses a key
    while (looping)
    {
        // ping and get the response
        PingReply response = await pinger
            .SendPingAsync("alwaysdeveloping.net");

        // extract response information
        Console.WriteLine($"Ping to '{response.Address}' took " +
            $"{response.RoundtripTime}milliseconds and the response " +
            $"was: `{response.Status}`");

        // wait 250ms before pinging again
        await Task.Delay(250);
    }
});

// wait for a key press
// and then cancel the looping above
Console.ReadKey();
looping = false;
```

A sample response:

``` terminal
Ping to '192.11.119.201' took 604 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 307 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 307 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 321 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 311 milliseconds and was: 'Success'
Ping to '0.0.0.0' took 0 milliseconds and was: 'TimedOut'
Ping to '192.11.119.201' took 307 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 620 milliseconds and was: 'Success'
Ping to '192.11.119.201' took 308 milliseconds and was: 'Success'
```

---

## Notes

While there are better and more featureful (and more expensive) 3rd party tools available for monitoring the status and uptime of a website, with `Ping` a simple, small application could be written to ping on an interval (as in the above example) and send out an alert if numerous timeouts are received.

---

## References

[Davide Bellone Tweet](https://twitter.com/BelloneDavide/status/1553066721702977537)   

<?# DailyDrop ?>153: 05-09-2022<?#/ DailyDrop ?>
