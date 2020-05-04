using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Network.TCP
{
    public class TcpSender : IDisposable
    {
        private readonly IPAddress host;
        private readonly int port;

        private readonly TcpClient client = new TcpClient();
        private NetworkStream stream;
        private readonly IFormatter formatter;

        public TcpSender(string host, int port)
        {
            this.host = IPAddress.Parse(host);
            this.port = port;

            formatter = new BinaryFormatter();

            Connect();
        }

        private void Connect()
        {
            client.Connect(host, port);
            stream = client.GetStream();
        }

        public void Send(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public Task SendAsync(byte[] data)
        {
            return stream.WriteAsync(data, 0, data.Length);
        }

        public void SendImage(Image image)
        {
            var container = new FrameContainer(image);
            formatter.Serialize(stream, container);
        }

        public Task SendImageAsync(Image image)
        {
            return Task.Run(() => SendImage(image));
        }

        public void Dispose()
        {
            client.Close();
            client.Dispose();
        }
    }
}
