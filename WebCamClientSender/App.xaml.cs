using System.Windows;
using Prism.Ioc;
using WebCamClientSender.Models.WebCameraManager;
using WebCamClientSender.Views;

namespace WebCamClientSender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IWebCameraManager>(new WebCameraManagerModel());
        }
    }
}
