
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Infrastructure.Services
{
    public class UdpService
    {
        private UdpClient UdpClient;
        private int ListenPort;
        
        public UdpService(int port)
        {
            ListenPort = port;
            UdpClient = new UdpClient(ListenPort);
        }
        public void StartListening()
        {
            Task.Run(async () => await ReciveMessageAsync());
        }

        public async Task ReciveMessageAsync()
        {
            try
            {
                while (true)
                {
                    var result = await UdpClient.ReceiveAsync();
                    string RecivedData = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine($"Recived: {RecivedData} from {result.RemoteEndPoint}");

                    //Echo the message back to the sender 
                    await SendMessageAsync(RecivedData, result.RemoteEndPoint);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string message, IPEndPoint endpoint)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            await UdpClient.SendAsync(buffer, buffer.Length,  endpoint);
        }
    }
}
