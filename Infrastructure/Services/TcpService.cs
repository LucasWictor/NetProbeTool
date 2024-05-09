using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    // listen for and accept incoming connection requests
    public class TcpService
    {
        public delegate void MessageEventHandler(string message);
        public event MessageEventHandler OnMessage;

        private TcpListener listener;
        private int port;

        public TcpService(int port)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartListeningAsync()
        {
            listener.Start();
            OnMessage?.Invoke($"Server is listening on port {port}...");

            try
            {
                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    OnMessage?.Invoke("Client Connected.");
                    HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                OnMessage?.Invoke($"Server stopped with error: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            OnMessage?.Invoke("Client disconnected.");
                            break;
                        }
                        var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnMessage?.Invoke($"Received: {message}");

                        await stream.WriteAsync(buffer, 0, bytesRead);
                        OnMessage?.Invoke("Echoed back.");
                    }
                }
                catch (Exception ex)
                {
                    OnMessage?.Invoke($"Error during communication: {ex.Message}");
                }
            }
        }

    }
}