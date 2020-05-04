using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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

                    stopWatcher.Stop();

                    FPS = (int)(1000 / stopWatcher.ElapsedMilliseconds);

                    Console.WriteLine($"FPS: {FPS}");

                    stopWatcher.Reset();

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
