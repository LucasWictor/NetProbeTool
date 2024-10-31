using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Infrastructure.Services
{
    public class TcpService
    {
        private TcpListener listener;
        private int port;

        public event Action<string> OnMessage;

        public TcpService(int port)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            listener.Start();
            OnMessage?.Invoke("The TCP server is now active on port 8888 and will continue to operate in the background. You can proceed with other tasks while it handles incoming data");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        OnMessage?.Invoke("Client connected.");
                        HandleClientAsync(client, cancellationToken);
                    }
                    await Task.Delay(100);  // Reduce CPU usage
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

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[1024];
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0) break;
                        var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnMessage?.Invoke($"Received: {message}");

                        // echo back the message
                        await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
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