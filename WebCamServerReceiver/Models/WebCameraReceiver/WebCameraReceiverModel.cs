using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Core.Extensions;
using Network.TCP;
using Prism.Mvvm;

namespace WebCamServerReceiver.Models.WebCameraReceiver
{
    public class WebCameraReceiverModel : BindableBase, IWebCameraReceiver
    {
        private readonly TcpReceiver receiver;

        private ObservableCollection<WriteableBitmap> frames;
        public ObservableCollection<WriteableBitmap> Frames
        {
            get => frames;
            set => SetProperty(ref frames, value);
        }

        private Image FromByteArray(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        public WebCameraReceiverModel()
        {
            receiver = new TcpReceiver(ConfigurationManager.AppSettings["host"],Convert.ToInt32(ConfigurationManager.AppSettings["port"]));
            receiver.ImageReceived += OnImageReceived;
            
            Frames = new ObservableCollection<WriteableBitmap>();
        }

        private void OnImageReceived(Image image, int frameIndex)
        {
            var currentBitmap = image as Bitmap;

            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (Frames.Count > frameIndex)
                {
                    Frames[frameIndex].UpdateWith(currentBitmap);
                }
                else
                {
                    Frames.Add(currentBitmap.ToWriteableBitmap());
                }
            });
        }

        public void StartReceiving()
        {
            receiver.StartReceiving();
        }

        public void StopReceiving()
        {
            receiver.StopReceiving();
        }
    }
}
