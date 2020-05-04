using System.Windows.Media.Imaging;

namespace Core.Extensions
{
    public static class BitmapSourceExtensions
    {
        public static byte[] ToByteArray(this BitmapSource source)
        {
            var width = source.PixelWidth;
            var height = source.PixelHeight;
            var stride = width * ((source.Format.BitsPerPixel + 7) / 8);

            var bitmapData = new byte[height * stride];

            source.CopyPixels(bitmapData, stride, 0);

            return bitmapData;
        }
    }
}
