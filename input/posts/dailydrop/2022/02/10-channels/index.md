---
title: "C# Channels - Produce & Consume data"
lead: "A thread-safe feature for producing and consumer data"
Published: 02/10/2022
slug: "10-channels"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - channel
    - queue
    - producer
    - consumer
    
---

## Daily Knowledge Drop

Today we dive into a little-known C# feature, I'd previously never heard about called `Channels`.

So what is a channel? - In short, a channel is a feature which allows for `passing of data between a producer and consumer(s)`. It is an `efficient, thread-safe queuing mechanism.`

---

## Usage

The examples set out below are very simple, and do not reflect a real world scenario. They have eben kept as minimal as possible to display the core concepts of the `Channel`.
The example consists of:
- an `end point` which when called will produce an item to the channel
- a `background` service which will constantly monitor the channel for new items and process them

### Setup

First a `Channel` needs to be created, and to be made available to both the producer and consumer.  

This is done by declaring a _singleton_ instance of the channel and adding it to the dependency injection container:

``` csharp
// Create a new channel and add to to the DI container
// CreateUnbounded => unlimited capacity
// Channel will hold Guids
builder.Services.AddSingleton<Channel<Guid>>(
    Channel.CreateUnbounded<Guid>(
            new UnboundedChannelOptions() { SingleReader = true }
    )
);

// Add as a background hosted service
// NOT related to channels directly, just used for this demo
builder.Services.AddHostedService<ChannelProcessor>();
```

\* When creating a channel, the options `UnBounded` (has unlimited capacity) or *`Bounded` (which has limited capacity). `Unbounded` queues are potentially dangerous to use if the consumer is not able to keep up with the producer, resulting in the application running out of memory.  

\*\* These options are not required, but by specifying them the factory method is able to specialize the implication created to be as optimal as possible.


### Producer

Here an `background service` is used to read items from the channel:

``` csharp
// The singleton implementation of the channel is injected 
// into the endpoint using dependency injection
app.MapGet("/{id}", async (Guid Id, Channel<Guid> channel) =>
{
    // The data is written to the channel
    await channel.Writer.WriteAsync(Id);
    Console.WriteLine($"Item '{Id}' successfully written to the channel");

    return await Task.FromResult($"Item '{Id}' successfully written to the channel");
});
```

As simple as that. Now we need a process to consume the data.

---

### Consumer

Here an `endpoint` is used to write an item to the channel.

``` csharp
class ChannelProcessor : BackgroundService
{
    private readonly Channel<Guid> _channel;

    // The singleton implementation of the channel is injected 
    // into the background service using dependency injection
    public QueueProcessor(Channel<Guid> channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Items are read off the channel as they arrive and processed
        await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            Console.WriteLine($"Item {item} successfully read from the channel");
        }
    }
}
```

---

## Output

If we run the application and hit the endpoint (to produce to the channel), we will see that it is consumed and processed immediately:

![Channel demo output](output.png) 

We have a simple working example of data being shared `efficiently and safely across threads`.

---

## Notes

This is a very simple example, and there is a lot more to `Channels` beyond this. `Channels` may not be a well-known feature, and not something which will be used in every day development - however just the knowledge they exist is a great starting point if you ever require them.
They are a simple, but powerful mechanism for exchanging data across Tasks - and should definitely be leveraged if the use case arises.

---

## References
[An Introduction to System.Threading.Channels](https://www.stevejgordon.co.uk/an-introduction-to-system-threading-channels)  
[DevBlogs: An Introduction to System.Threading.Channels](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/)

<?# DailyDrop ?>08: 10-02-2022<?#/ DailyDrop ?>
