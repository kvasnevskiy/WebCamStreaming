using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Network.TCP;
using Prism.Mvvm;

namespace WebCamClientSender.Models.WebCameraManager
{
    public class WebCameraManagerModel : BindableBase, IWebCameraManager, IDisposable
    {
        private TcpSender sender = new TcpSender(ConfigurationManager.AppSettings["receive_host"], Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

        private WriteableBitmap writeableBitmap;
        private VideoCapture capture;
        private Timer captureTimer;
        
        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
        }

        #region Constructors

        public WebCameraManagerModel()
        {
            InitializeCapture();
        }

        #endregion

        ~WebCameraManagerModel()
        {
            Dispose();
        }

        public byte[] ToByteArray(Bitmap image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        private void CaptureTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var currentFrame = capture.QueryFrame();

            if (currentFrame != null)
            {
                var currentFrameBitmap = currentFrame.ToBitmap();

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    //Send image to receiver
                    this.sender.SendImage(currentFrameBitmap);

                    if (writeableBitmap == null)
                    {
                        writeableBitmap = currentFrameBitmap.ToWriteableBitmap();
                        Frame = (ImageSource)writeableBitmap;
                    }
                    else
                    {
                        writeableBitmap.UpdateWith(currentFrameBitmap);
                    }
                });
            }
        }

        private void InitializeCapture()
        {
            capture = new VideoCapture();
            capture.SetCaptureProperty(CapProp.Fps, 30);
            capture.SetCaptureProperty(CapProp.FrameHeight, 600);
            capture.SetCaptureProperty(CapProp.FrameWidth, 800);

            captureTimer = new Timer
            {
                Interval = 1.0
            };
            captureTimer.Elapsed += CaptureTimerElapsed;
        }

        public void StartStreaming()
        {
            captureTimer.Start();
        }

        public void StopStreaming()
        {
            captureTimer.Stop();
        }

        public void Dispose()
        {
            sender?.Dispose();
            capture?.Dispose();
            captureTimer?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
