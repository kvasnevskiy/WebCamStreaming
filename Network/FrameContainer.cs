using System;
using System.Drawing;

namespace Network
{
    [Serializable]
    public class FrameContainer
    {
        public Image Frame { get; set; }

        public FrameContainer(Image frame)
        {
            Frame = frame;
        }
    }
}
