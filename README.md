# GRPC Experiments

Experimenting with pub/sub style eventing in grpc c#.

## Build (Ubuntu)

Install .NET 6 SDK: https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

Then build

```
$ git@github.com:TeknoVenus/grpc-experiments.git
$ cd grpc-experiments
$ dotnet build
```

# Run

Launch server in one terminal

```
$ dotnet run --project Server/Server.csproj
```

This will start a grpc server on a unix socket @ `/tmp/grpcTest`. Server will start a timer and fire off events to subscribed clients every 5 seconds.

Calling the GetSystemInfo method will also fire off a manually triggered event.



Launch (multiple) clients

```
$ dotnet run --project Client/Client.csproj
```

Sample output
```
gRPC Test App
Connecting to server @ /tmp/grpcTest
Connected to grpc server
Subscribing to TestEvent
>> Got test event! ID: a6385850-830e-4396-b97a-f991656fdc94. Message: Manual event!
Got system info from plugin: 
* Computer Name: C-SF-T14s
* OS: Unix 5.15.0.50
Now waiting for new events - press any key to stop
>> Got test event! ID: ad8c6008-d7f3-4e87-b569-2d030d253540. Message: Background event
>> Got test event! ID: af60104d-bfc9-4a29-b38d-65d56ceb1aac. Message: Background event

```

