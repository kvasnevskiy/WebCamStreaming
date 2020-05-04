using Network;
using Prism.Mvvm;
using WebCamServerReceiver.Models.WebCameraReceiver;

namespace WebCamServerReceiver.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Web Camera Receiver";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private IWebCameraReceiver webCameraReceiver;
        public IWebCameraReceiver WebCameraReceiver
        {
            get => webCameraReceiver;
            set => SetProperty(ref webCameraReceiver, value);
        }

        public MainWindowViewModel(IWebCameraReceiver webCameraReceiver)
        {
            WebCameraReceiver = webCameraReceiver;
            WebCameraReceiver.StartReceiving();
        }
    }
}
