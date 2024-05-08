using Infrastructure.Services;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                //Start TCP server
                case "--start-server":
                    int tcpPort = 8888; // default TCP port
                    if (args.Length > 2 && args[1] == "-p" && int.TryParse(args[2], out int parsedTcpPort))
                    {
                        tcpPort = parsedTcpPort;
                    }
                    var tcpService = new TcpService(tcpPort);
                    await tcpService.StartListeningAsync();
                    break;

                //Start UDP server
                case "--start-udp":
                    int udpPort = 8888; // default UDP port
                    if (args.Length > 2 && args[1] == "-p" && int.TryParse(args[2], out int parsedUdpPort))
                    {
                        udpPort = parsedUdpPort;
                    }
                    var udpService = new UdpService(udpPort);
                    udpService.StartListening();
                    break;

                default:
                    Console.WriteLine("Invalid command. Use '--start-server' for TCP or '--start-udp' for UDP.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("No command provided. Use '--start-server' for TCP or '--start-udp' for UDP.");
        }
    }
}
