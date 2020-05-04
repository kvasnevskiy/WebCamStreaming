using Prism.Mvvm;
using WebCamClientSender.Models.WebCameraManager;

namespace WebCamClientSender.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "WebCamClientSender";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private IWebCameraManager webCameraManager;
        public IWebCameraManager WebCameraManager
        {
            get => webCameraManager;
            set => SetProperty(ref webCameraManager, value);
        }

        #region Constructors

        public MainWindowViewModel(IWebCameraManager webCameraManager)
        {
            WebCameraManager = webCameraManager;
        }

        #endregion

        #region Events

        public void Loaded()
        {
            WebCameraManager.StartStreaming();
        }

        public void Closing()
        {
            WebCameraManager.StopStreaming();
        }


        #endregion
    }
}
