using System.Net.Sockets;
using System.Text;


namespace Infrastructure.Services
{
    public class UdpService
    {
        private UdpClient udpClient;
        private int listenPort;

        public UdpService(int port)
        {
            listenPort = port;
            udpClient = new UdpClient(listenPort);
        }

        public void StartListening()
        {
            Task.Run(async () => await ReceiveMessageAsync());
        }

        private async Task ReceiveMessageAsync()
        {
            try
            {
                while (true)
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    string receivedData = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine($"Received: {receivedData} from {result.RemoteEndPoint}");
                    byte[] buffer = Encoding.ASCII.GetBytes(receivedData);
                    await udpClient.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex.Message}");
            }
        }
    }
}
