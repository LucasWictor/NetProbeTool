using System;
using System.Net.Sockets;
using System.Threading.Tasks;

    namespace Infrastructure.Services
    {
    public class PortScanner
    {
        public async Task ScanPorts(string host, int startPort, int endPort)
        {
            var results = new List<(int Port, string Status)>();

            var tasks = Enumerable.Range(startPort, endPort - startPort + 1).Select(port => Task.Run(async () =>
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        await client.ConnectAsync(host, port);
                        return (Port: port, Status: $"Port {port} is open.");
                    }
                }
                catch (SocketException)
                {
                    return (Port: port, Status: $"Port {port} is closed.");
                }
            })).ToList();

            var scanResults = await Task.WhenAll(tasks);

            // Add results synchronized
            foreach (var result in scanResults)
            {
                results.Add(result);
            }

            // Sort by port number before print
            foreach (var result in results.OrderBy(r => r.Port))
            {
                Console.WriteLine(result.Status);
            }
        }
    }
}