using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using Emgu.CV.Cuda;

namespace WebCamServerReceiver.Service
{
    [ServiceContract]
    public interface IWebCameraReceiverService
    {
        [OperationContract]
        void Send(Stream data);
    }
}
