using System.Threading.Tasks.Dataflow;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcExperiments.Protos;

namespace GrpcExperiments.Plugins.DemoPlugin;

public class DemoPluginService : DemoService.DemoServiceBase
{
    private readonly DemoPlugin _plugin;
    private readonly ILogger<DemoPluginService> _logger;

    public DemoPluginService(DemoPlugin plugin, ILogger<DemoPluginService> logger)
    {
        _plugin = plugin;
        _logger = logger;
    }

    public override Task<GetSystemInfoReply> GetSystemInfo(Empty request, ServerCallContext context)
    {
        return Task.FromResult(_plugin.GetSystemInfo());
    }

    public override Task<PluginMetadata> GetPluginDetails(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new PluginMetadata()
        {
            PluginName = _plugin.Name(),
            PluginDescription = _plugin.Description()
        });
    }



    public override async Task SubscribeTestEvent(Empty request, IServerStreamWriter<TestEvent> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("{Client} subscribing for test events", context.Peer);

        EventHandler<TestEvent> func = async (o, e) => await responseStream.WriteAsync(e, context.CancellationToken);

        _plugin.TestEventHandler += func;

        try
        {
            // Wait for the client to disconnect (without just spinning an infinite while loop on IsCancellationRequested
            var tcs = new TaskCompletionSource();
            await tcs.Task.WaitAsync(context.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancelling subscription");
        }
        finally
        {
            _plugin.TestEventHandler -= func;
        }
    }
}