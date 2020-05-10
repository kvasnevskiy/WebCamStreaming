using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WebCamServerReceiver.Models.WebCameraReceiver
{
    public interface IWebCameraReceiver
    {
        ObservableCollection<WriteableBitmap> Frames { get; }

        void StartReceiving();

        void StopReceiving();
    }
}
