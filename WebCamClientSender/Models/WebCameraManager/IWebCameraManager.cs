using System.Windows.Media;

namespace WebCamFaceTracking.Models.WebCameraManager
{
    public interface IWebCameraManager
    {
        ImageSource Frame { get; }

        void StartStreaming();

        void StopStreaming();
    }
}