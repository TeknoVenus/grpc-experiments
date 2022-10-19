using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using System.Timers;
using Google.Protobuf.WellKnownTypes;
using GrpcExperiments.Protos;

namespace GrpcExperiments.Plugins.DemoPlugin;

public class DemoPlugin
{
    // The GRPC service is a transient service in C#, meaning the DemoPluginService is constructed for
    // each incoming request. This means we can't store state in that service. Store state and other
    // here, and then inject this into the DemoPluginService
    
    public string Name() => "Demo Plugin";
    public string Description() => "A sample plugin to experiment with gRPC";

    public EventHandler<TestEvent>? TestEventHandler;
    
    private readonly ILogger<DemoPlugin> _logger;
    private readonly System.Timers.Timer _timer;
    
    public DemoPlugin(ILogger<DemoPlugin> logger)
    {
        _logger = logger;
        
        // Every 5 seconds, raise an event
        _timer = new System.Timers.Timer(5000);
        _timer.Elapsed += Generator;
        _timer.Enabled = true;
    }

    /// <summary>
    /// Return system information
    /// </summary>
    /// <returns></returns>
    public GetSystemInfoReply GetSystemInfo()
    {
        RaiseTestEvent("Manual event!");
        
        return new GetSystemInfoReply
        {
            ComputerName = Environment.MachineName,
            OsVersion = Environment.OSVersion.VersionString
        };
    }
    
    /// <summary>
    /// Raise an event that will be sent to all subscribed clients
    /// </summary>
    /// <param name="description"></param>
    private void RaiseTestEvent(string description)
    {
        _logger.LogInformation("Raising event with description {Desc} @ {Time}", description, DateTime.Now);

        var eventArgs = new TestEvent()
        {
            EventId = Guid.NewGuid().ToString("D"),
            EventMessage = description,
            EventTime = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        
        TestEventHandler?.Invoke(this, eventArgs);
    }
    
    /// <summary>
    /// Background timer task to generate events
    /// </summary>
    /// <param name="state"></param>
    /// <param name="elapsedEventArgs"></param>
    private void Generator(object? state, ElapsedEventArgs elapsedEventArgs)
    {
        RaiseTestEvent("Background event");
    }
    
}