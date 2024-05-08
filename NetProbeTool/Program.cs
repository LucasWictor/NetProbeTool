using Infrastructure.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // Example of checking command-line arguments
        if (args.Length > 0 && args[0] == "--start-server")
        {
            int port = 8888; // Default port
            var tcpService = new TcpService(port);
            await tcpService.StartListeningAsync();
        }
    }
}