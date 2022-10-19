using GrpcExperiments.Plugins.DemoPlugin;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

const string UnixSocketPath = "/tmp/grpcTest";

builder.WebHost.ConfigureKestrel(options =>
{
    if (File.Exists(UnixSocketPath))
    {
        File.Delete(UnixSocketPath);
    }

    options.ListenUnixSocket(UnixSocketPath, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<DemoPlugin>();

var app = builder.Build();

app.MapGrpcService<DemoPluginService>();

// Configure the HTTP request pipeline.
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();