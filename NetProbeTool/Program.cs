using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services;

class Program
{
    private static TcpService tcpService = null;
    private static UdpService udpService = null;

    private static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Start TCP Server");
            Console.WriteLine("2. Start UDP Server");
            Console.WriteLine("3. Port Scanning Menu");
            Console.WriteLine("4. Exit");

            var key = Console.ReadKey().Key;
            Console.Clear();

            switch (key)
            {
                case ConsoleKey.D1:
                    if (tcpService == null)
                    {
                        tcpService = new TcpService(8888);
                        tcpService.OnMessage += Console.WriteLine;  // Subscribe to TCP server messages
                        StartTcpServer(tcpService, cts.Token);
                    }
                    else
                    {
                        Console.WriteLine("TCP server is already running.");
                    }
                    break;
                case ConsoleKey.D2:
                    if (udpService == null)
                    {
                        udpService = new UdpService(8888);
                        udpService.StartListeningAsync(cts.Token);
                        Console.WriteLine("The UDP server is now active on port 8888 and will continue to operate in the background.");
                    }
                    else
                    {
                        Console.WriteLine("UDP server is already running.");
                    }
                    break;
                case ConsoleKey.D3:
                    await PortScanningMenu();
                    break;
                case ConsoleKey.D4:
                    cts.Cancel(); 
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }

    private static void StartTcpServer(TcpService service, CancellationToken token)
    {
        Task.Run(() => service.StartListeningAsync(token), token);
    }

    private static async Task PortScanningMenu()
    {
        Console.WriteLine("Choose an option for port scanning:");
        Console.WriteLine("1. Localhost");
        Console.WriteLine("2. External IP Address");

        var key = Console.ReadKey().Key;
        Console.Clear();

        switch (key)
        {
            case ConsoleKey.D1:
                await ScanPorts("127.0.0.1");
                break;
            case ConsoleKey.D2:
                Console.Write("Enter the IP address to scan: ");
                string ip = Console.ReadLine();
                await ScanPorts(ip);
                break;
            default:
                Console.WriteLine("Invalid choice. Returning to main menu.");
                break;
        }
    }

    private static async Task ScanPorts(string ipAddress)
    {
        Console.Write("Enter the start and end ports to scan (e.g., 80 100): ");
        string[] ports = Console.ReadLine().Split(' ');
        if (ports.Length == 2 && int.TryParse(ports[0], out int start) && int.TryParse(ports[1], out int end))
        {
            PortScanner scanner = new PortScanner();
            Console.WriteLine($"Scanning ports {start} to {end} on {ipAddress}...");
            await scanner.ScanPorts(ipAddress, start, end);
            Console.WriteLine("Scanning complete.");
        }
        else
        {
            Console.WriteLine("Invalid ports entered.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
