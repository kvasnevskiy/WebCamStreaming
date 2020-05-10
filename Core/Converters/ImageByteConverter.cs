using System.Drawing;
using System.IO;

namespace Core.Converters
{
    public static class ImageByteConverter
    {
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ImageToByteArray(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}
