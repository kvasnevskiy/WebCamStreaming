using System.Windows.Media;

namespace WebCamClientSender.Models.WebCameraManager
{
    public interface IWebCameraManager
    {
        ImageSource Frame { get; }

        void StartStreaming();

        void StopStreaming();
    }
}