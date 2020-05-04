using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Network.TCP
{
    public class TcpReceiver : IDisposable
    {
        private readonly TcpListener listener;

        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public event Action<Image> ImageReceived;
        private void OnImageReceived(Image image) => ImageReceived?.Invoke(image);

        public TcpReceiver(string host, int port)
        {
            listener = new TcpListener(IPAddress.Parse(host), port);
            listener.Start();
        }

        public int FPS { get; set; }

        public void StartReceiving()
        {
            Task.Run(() =>
            {
                var client = listener.AcceptTcpClient();

                Console.WriteLine("Подключен клиент. Выполнение запроса...");

                var stream = client.GetStream();

                var stopWatcher = new Stopwatch();

                var formatter = new BinaryFormatter();

                while (!cancelTokenSource.Token.IsCancellationRequested)
                {
                    stopWatcher.Start();

                    var container = (FrameContainer)formatter.Deserialize(stream);

                    ////Read image byte array size
                    //var imageByteArray = new byte[4];
                    //stream.Read(imageByteArray, 0, 4);
                    //var imageByteArrayCount = BitConverter.ToInt32(imageByteArray, 0);

                    //Console.WriteLine($"Image length: {imageByteArrayCount}");

                    ////Read full image by portions
                    //byte[] total = new byte[imageByteArrayCount];
                    //byte[] buffer = new byte[256];

                    //int bytesReceived = 0;
                    //while (bytesReceived < imageByteArrayCount)
                    //{
                    //    bytesReceived += stream.Read(total, bytesReceived, imageByteArrayCount - bytesReceived);
                    //}

                    stopWatcher.Stop();

                    FPS = (int)(1000 / stopWatcher.ElapsedMilliseconds);

                    Console.WriteLine($"FPS: {FPS}");

                    stopWatcher.Reset();

                    //var image = ImageConverter.FromByteArray(total);
                    OnImageReceived(container.Frame);
                }

                client.Dispose();
            });
        }

        public void StopReceiving()
        {
            cancelTokenSource.Cancel();
            listener.Stop();
        }

        public void Dispose()
        {
            cancelTokenSource.Dispose();
        }
    }
}
