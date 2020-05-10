using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Core.Converters;

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

        ~TcpSender()
        {
            Dispose();
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
            //var container = new FrameContainer(image);

            //using (var ms = new MemoryStream())
            //{
                //formatter.Serialize(ms, container);

                var byteArray = ImageByteConverter.ImageToByteArray(image);
                stream.Write(BitConverter.GetBytes(byteArray.Length), 0, 4);

                Console.WriteLine($"Image Length -> {byteArray.Length}");

                stream.Write(byteArray, 0, byteArray.Length);
           // }
           
        }

        public Task SendImageAsync(Image image)
        {
            return Task.Run(() => SendImage(image));
        }

        public void Dispose()
        {
            client?.Close();
            client?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
