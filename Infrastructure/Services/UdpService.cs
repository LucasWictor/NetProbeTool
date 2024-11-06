using System.Net.Sockets;
using System.Text;


namespace Infrastructure.Services
{
    public class UdpService : IDisposable
    {
        private readonly UdpClient udpClient;
        private bool disposed;

        public UdpService(int port)
        {
            udpClient = new UdpClient(port);
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            await ReceiveMessageAsync(cancellationToken);
        }

        private async Task ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    string receivedData = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine($"Received: {receivedData} from {result.RemoteEndPoint}");

                    // Echo message back
                    byte[] buffer = Encoding.ASCII.GetBytes(receivedData);
                    await udpClient.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("UDP listening cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                udpClient.Dispose();
                disposed = true;
            }
        }
    }
}