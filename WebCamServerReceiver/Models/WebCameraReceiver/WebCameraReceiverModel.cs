using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core.Extensions;
using Emgu.CV;
using Network;
using Network.TCP;
using Prism.Mvvm;

namespace WebCamServerReceiver.Models.WebCameraReceiver
{
    public class WebCameraReceiverModel : BindableBase, IWebCameraReceiver
    {
        private readonly TcpReceiver receiver;

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
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
        }

        private void OnImageReceived(Image image)
        {
            var bitmap = image as Bitmap;
            WriteableBitmap writeableBitmap = null;

            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (writeableBitmap == null)
                {
                    writeableBitmap = bitmap.ToWriteableBitmap();
                    Frame = (ImageSource)writeableBitmap;
                }
                else
                {
                    writeableBitmap.UpdateWith(bitmap);
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
