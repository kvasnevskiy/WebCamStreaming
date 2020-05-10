using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Core.Converters;

namespace Network.TCP
{
    public class TcpReceiver : IDisposable
    {
        private readonly TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();

        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public event Action<Image> ImageReceived;
        private void OnImageReceived(Image image) => ImageReceived?.Invoke(image);

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

                        OnImageReceived(ImageByteConverter.ByteArrayToImage(buffer));
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

                

                //while (!cancelTokenSource.Token.IsCancellationRequested)
                //{
                //    Console.WriteLine("Start while...");

                //    var stopWatch = new Stopwatch();
                //    stopWatch.Start();

                //    Console.WriteLine("Try get stream...");
                    
                //    var stream = client.GetStream();

                //    Console.WriteLine("End get stream");

                //    var imageLengthBuffer = new byte[4];

                //    Console.WriteLine("Start read length...");

                //    var imageLengthBytesCount = await stream.ReadAsync(imageLengthBuffer, 0, 4, cancelTokenSource.Token);

                //    Console.WriteLine("End read length...");

                //    var imageArrayLength = BitConverter.ToInt32(imageLengthBuffer, 0);

                //    var buffer = new byte[imageArrayLength];

                //    int bytesReceived = 0;
                //    while (bytesReceived < imageArrayLength)
                //    {
                //        Console.WriteLine("Start read...");
                //        bytesReceived += await stream.ReadAsync(buffer, bytesReceived, imageArrayLength - bytesReceived, cancelTokenSource.Token);
                //        Console.WriteLine("End read...");
                //    }

                //    //Console.WriteLine($"Received bytes {bytesReceived}");

                //    OnImageReceived(ImageByteConverter.ByteArrayToImage(buffer));

                //    stopWatch.Stop();

                //    FPS = (int)(1000 / stopWatch.ElapsedMilliseconds);

                //    Console.WriteLine($"FPS: {FPS}");

                //}

                //client.Dispose();


                //Console.WriteLine("Клиент отключен...");

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
