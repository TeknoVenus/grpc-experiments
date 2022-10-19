using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcExperiments.Protos;

namespace GrpcExperiments;

internal class Program
{
    static async Task Main(string[] args)
    {
        const string socketPath = "/tmp/grpcTest";

        Console.WriteLine("gRPC Test App");
        Console.WriteLine($"Connecting to server @ {socketPath}");

        if (!File.Exists(socketPath))
        {
            Console.WriteLine($"Error: Cannot find server @ {socketPath} - are you sure you started it?");
            return;
        }

        try
        {
            using var channel = GrpcChannelFactory.CreateChannel(socketPath);
            var client = new DemoService.DemoServiceClient(channel);
        
            Console.WriteLine("Connected to grpc server");
        
            Console.WriteLine("Subscribing to TestEvent");
            var cts = new CancellationTokenSource();
            var eventMonitor = TestEventPrinter(client, cts.Token);

            var systemInfo = await client.GetSystemInfoAsync(new Empty());
            Console.WriteLine("Got system info from plugin: ");
            Console.WriteLine($"* Computer Name: {systemInfo.ComputerName}");
            Console.WriteLine($"* OS: {systemInfo.OsVersion}");

            Console.WriteLine("Now waiting for new events - press any key to stop");
            Console.ReadLine();
            cts.Cancel();
            cts.Dispose();
            Console.WriteLine("Stopped");
            Console.ReadLine();

        }
        catch (RpcException e)
        {
            Console.WriteLine($"RPCException: {e.Message}");
        }
       
    }

    // Subscribe to test events and print to console
    private static async Task TestEventPrinter(DemoService.DemoServiceClient client, CancellationToken cancellationToken)
    {
        var eventStream = client.SubscribeTestEvent(new Empty(), Metadata.Empty, null, cancellationToken).ResponseStream;

        // Will wait for grpc to send a message over the stream
        // "while await" will not do an infinite loop, 
        try
        {
            while (await eventStream.MoveNext(cancellationToken))
            {
                var testEvent = eventStream.Current;
                Console.WriteLine($">> Got test event! ID: {testEvent.EventId}. Message: {testEvent.EventMessage}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Event subscription cancelled");
        }
    }
}


