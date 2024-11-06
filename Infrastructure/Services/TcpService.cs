using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Infrastructure.Services
{
    public class TcpService
    {
        private readonly TcpListener listener;
        private readonly int port;

        public event Action<string> OnMessage;

        public TcpService(int port)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            listener.Start();
            OnMessage?.Invoke($"TCP server listening on port {port}");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        OnMessage?.Invoke("Client connected.");
                        _ = HandleClientAsync(client, cancellationToken);
                    }
                    await Task.Delay(100, cancellationToken);  // delay to lower CPU usage
                }
            }
            catch (OperationCanceledException)
            {
                OnMessage?.Invoke("TCP listening cancelled.");
            }
            catch (Exception ex)
            {
                OnMessage?.Invoke($"Server stopped with error: {ex.Message}");
            }
            finally
            {
                listener.Stop();
                OnMessage?.Invoke("Server stopped.");
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

                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnMessage?.Invoke($"Received: {message}");

                        // Echo back the message
                        await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        OnMessage?.Invoke("Echoed back.");
                    }
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    OnMessage?.Invoke("Connection reset by client.");
                }
                catch (Exception ex)
                {
                    OnMessage?.Invoke($"Error during communication: {ex.Message}");
                }
            }
        }
    }
}
