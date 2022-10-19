using System.Net.Sockets;
using Grpc.Net.Client;

namespace GrpcExperiments;

public static class GrpcChannelFactory
{
    public static GrpcChannel CreateChannel(string socketPath)
    {
        var domainSocket = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixSocketFactory(domainSocket);

        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectCallback = connectionFactory.ConnectAsync
        };

        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions()
        {
            HttpHandler = socketsHttpHandler
        });
    }
}