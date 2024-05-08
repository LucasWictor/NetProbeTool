using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // Provides simple methods that listen for and accept incoming connection requests.
    public class TcpService
    {
        private TcpListener listener;

        public TcpService(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartListeningAsync()
        {
            listener.Start();
            Console.WriteLine("Server is listening...");

            try
            {
                while (true)
                {
                    // Returns a task that completes when a client has connected to the server.
                    var client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client Connected.");
                    // It is important to await HandleClientAsync to ensure it runs properly without blocking.
                    await HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server stopped with error: {ex.Message}");
            }
            finally
            {
                // Properly stop the listener when exiting the loop due to an exception or other reason.
                listener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                // Retrieves the underlying NetworkStream used to send and receive data.
                var stream = client.GetStream();
                var buffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("Client disconnected.");
                            break;
                        }
                        // Converts byte arrays to strings for network stream.
                        var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Received: " + message);

                        // Echo the message back to the client
                        await stream.WriteAsync(buffer, 0, bytesRead);
                        Console.WriteLine("Echoed back.");
                    }
                }
                catch (Exception ex)
                {
                    // Catch exceptions that may occur during the reading/writing from the stream.
                    Console.WriteLine($"Error during communication: {ex.Message}");
                }
            }
        }
    }
}
