using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Core.Converters;

namespace Network.TCP
{
    public class TcpReceiver : IDisposable
    {
        private readonly TcpListener listener;
        private readonly List<TcpClient> clients = new List<TcpClient>();

        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public event Action<Image, int> ImageReceived;
        private void OnImageReceived(Image image, int frameIndex) => ImageReceived?.Invoke(image, frameIndex);

        public TcpReceiver(string host, int port)
        {
            listener = new TcpListener(IPAddress.Parse(host), port);
            listener.Start();
        }

        ~TcpReceiver()
        {
            Dispose();
        }

        public int FPS { get; set; }

        public void WaitingForDisconnect(TcpClient client, Thread thread)
        {
            Task.Factory.StartNew(() =>
            {
                while (client.Connected)
                {
                    Thread.Sleep(1000);    
                }

                thread.Abort();

            }, TaskCreationOptions.LongRunning);
        }

        private void ClientProcessing(TcpClient client)
        {
            clients.Add(client);

            var clientThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var stream = client.GetStream();
                        var imageLengthBuffer = new byte[4];

                        Console.WriteLine("Start read length...");
                        var imageLengthBytesCount = stream.Read(imageLengthBuffer, 0, 4);
                        Console.WriteLine("End read length...");

                        var imageArrayLength = BitConverter.ToInt32(imageLengthBuffer, 0);
                        var buffer = new byte[imageArrayLength];

                        int bytesReceived = 0;
                        while (bytesReceived < imageArrayLength)
                        {
                            Console.WriteLine("Start read...");
                            bytesReceived += stream.Read(buffer, bytesReceived, imageArrayLength - bytesReceived);
                            Console.WriteLine("End read...");
                        }

                        OnImageReceived(ImageByteConverter.ByteArrayToImage(buffer), clients.IndexOf(client));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }

                Console.WriteLine("Uppy stop!!");
            });

            WaitingForDisconnect(client, clientThread);

            clientThread.Start();
        }

        public void StartReceiving()
        {
            Task.Factory.StartNew(() =>
            {
                while (!cancelTokenSource.Token.IsCancellationRequested)
                {
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");
                    ClientProcessing(client);
                }

            }, TaskCreationOptions.LongRunning);
        }

        public void StopReceiving()
        {
            cancelTokenSource.Cancel();
            listener.Stop();
        }

        public void Dispose()
        {
            cancelTokenSource?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
