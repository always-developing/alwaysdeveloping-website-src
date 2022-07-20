---
title: "Pub/Sub with Redis"
lead: "Redis has pub/sub functionality in addition to being a key/value database"
Published: "08/16/2022 01:00:00+0200"
slug: "16-redis-pub-sub"
draft: false
toc: true
categories:
    - DailyDrop
tags:
   - c#
   - .net
   - redis
   - messaging
   - pubsub

---

## Daily Knowledge Drop

A common use case for `Redis` is as a _key-value_ stored, generally for caching - however, in addition to this _key-value_ store `Redis` also has built in `messaging functionality via pub/sub`.

---

## Redis installation

`Redis` is not officially supported on Windows - so to use it on Windows, either `WSL2` (Windows Subsystem for Linux) has to be used, or the Redis container image (available from [Docker Hub](https://hub.docker.com/_/redis)) running in _Docker Desktop_, for example. 

For the examples below, the `Redis` image from _Docker Hub_ was used, running on _Docker Desktop_ for Windows.

---

## Subscriber

First let's look at the `subscriber` side. A reference is required to the `StackExchange.Redis` NuGet package for the following code:

``` csharp
using StackExchange.Redis;

// configure the connection to the Redis endpoint
using ConnectionMultiplexer redis = 
    ConnectionMultiplexer.Connect("localhost:6379");
ISubscriber subScriber = redis.GetSubscriber();

// subscribe to the "alwaysdeveloping" channel. This needs to match the channel name
// the publisher is using as well
subScriber.Subscribe(new RedisChannel("alwaysdeveloping", 
    RedisChannel.PatternMode.Auto), 
    (channel, message) =>
{
    // this handler method will be called for each message received on the channel
    Console.WriteLine($"[{DateTime.Now}] Message received: {message}");
});

Console.ReadKey();
```

Configuring the `subscriber` is relatively straight forward:
- Define a connection to the specific `Redis` endpoint
- Get a _subscriber_ from the connection
- Subscribe to a specific channel (by name , _alwaysdeveloping_ in the above)
- Specify the handler method to be called when a message is received

The code will execute until a key is read from the console, waiting for any message to be sent to the channel. When a message is sent to the channel, it will in turn be published to `all` subscribers - it is possible to have `multiple subscribers`, each subscriber `will receive their own copy of the message`

---

## Publisher

Next let's look at the `publisher` - configuring this is also as straightforward as the subscriber:

``` csharp
using StackExchange.Redis;

// configure the connection to the Redis endpoint
using ConnectionMultiplexer redis = 
    ConnectionMultiplexer.Connect("localhost:6379");
ISubscriber sub = redis.GetSubscriber();

// loop 100 times
for (int i = 0; i <= 100; i++)
{
    // generate a unique id for each message
    var id = Guid.NewGuid().ToString();

    // publish to the "alwaysdeveloping" channel
    await sub.PublishAsync(new RedisChannel("alwaysdeveloping", 
        RedisChannel.PatternMode.Auto),
        $"Sample message with Id {id}");

    Console.WriteLine($"Message with Id '{id}' published!");
    Thread.Sleep(100);
}
```

The above code:
- Define a connection to the `Redis` endpoint
- Get a _subscriber_ from the connection
- Loops 100 times, 
- Publishes a string to a specific channel (by name, _alwaysdeveloping_ in the above)

---

## Pub/Sub Execution

Executing the `subscriber` first, so it is running waiting for messages, then executing the `publisher` yields the following output:

Sample from the the **Publisher**:

``` terminal
Message with Id '1895b2c1-61c1-4be9-8ed2-7c6359018534' published!
Message with Id '01a27c11-984c-481a-b4d0-bc879b13674d' published!
Message with Id '49e799eb-cf4c-4847-a234-d79b8e40cd79' published!
Message with Id '25700248-edbc-4c70-aaef-5aca4cb0f24f' published!
Message with Id '6a4caee2-e9ad-446f-8e2c-156cebdf6a42' published!
Message with Id '061ef1ff-e830-408c-84db-bdf649c8c446' published!
Message with Id '358c78ba-e85a-481a-970a-1c67b2ca8b1d' published!
Message with Id '5101fe4a-86fb-4356-b0db-dea0f4f1a402' published!
Message with Id 'fe944b2c-c945-417c-bb61-8b7eb7bfe8f1' published!
Message with Id '1f6d26a0-35ab-4994-8293-1e2a9e672cee' published!
Message with Id '5e276d80-9ccd-403b-9435-80b8fda0e2d3' published!
Message with Id 'a3ea7ddd-6ac6-4b3e-b35d-1276ce20e1bd' published!
```

Sample from the `Subscriber(s)`:

``` terminal
[2022/07/19 21:09:24] Message received: Sample message with Id 1895b2c1-61c1-4be9-8ed2-7c6359018534
[2022/07/19 21:09:24] Message received: Sample message with Id 01a27c11-984c-481a-b4d0-bc879b13674d
[2022/07/19 21:09:24] Message received: Sample message with Id 49e799eb-cf4c-4847-a234-d79b8e40cd79
[2022/07/19 21:09:24] Message received: Sample message with Id 25700248-edbc-4c70-aaef-5aca4cb0f24f
[2022/07/19 21:09:24] Message received: Sample message with Id 6a4caee2-e9ad-446f-8e2c-156cebdf6a42
[2022/07/19 21:09:24] Message received: Sample message with Id 061ef1ff-e830-408c-84db-bdf649c8c446
[2022/07/19 21:09:25] Message received: Sample message with Id 358c78ba-e85a-481a-970a-1c67b2ca8b1d
[2022/07/19 21:09:25] Message received: Sample message with Id 5101fe4a-86fb-4356-b0db-dea0f4f1a402
[2022/07/19 21:09:25] Message received: Sample message with Id fe944b2c-c945-417c-bb61-8b7eb7bfe8f1
[2022/07/19 21:09:25] Message received: Sample message with Id 1f6d26a0-35ab-4994-8293-1e2a9e672cee
[2022/07/19 21:09:25] Message received: Sample message with Id 5e276d80-9ccd-403b-9435-80b8fda0e2d3
[2022/07/19 21:09:25] Message received: Sample message with Id a3ea7ddd-6ac6-4b3e-b35d-1276ce20e1bd
```
---

## Notes

Configuring `Redis pub/sub` is incredibly simple and easy to implement, especially if there is a Redis instance setup (for caching for example). While not entirely comparable, having worked with other similar services such as _RabbitMq_ and _Kafka_, this was by far the simplest to implement.

---

## References

[DevMentors - FeedR episode #5](https://www.youtube.com/watch?v=80Ke9hsG_RU)   
[Install Redis on Windows](https://redis.io/docs/getting-started/installation/install-redis-on-windows/)

---

<?# DailyDrop ?>139: 16-08-2022<?#/ DailyDrop ?>
