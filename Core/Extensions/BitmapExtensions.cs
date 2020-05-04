using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Core.Extensions
{
    public static class BitmapExtensions
    {
        public static WriteableBitmap ToWriteableBitmap(this Bitmap source)
        {
            BitmapData bitmapdata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            try
            {
                return new WriteableBitmap(BitmapSource.Create(bitmapdata.Width, bitmapdata.Height, (double)source.HorizontalResolution, (double)source.VerticalResolution, PixelFormats.Bgr24, (BitmapPalette)null, bitmapdata.Scan0, bitmapdata.Stride * bitmapdata.Height, bitmapdata.Stride));
            }
            finally
            {
                source.UnlockBits(bitmapdata);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public static void UpdateWith(this WriteableBitmap target, Bitmap source)
        {
            BitmapData bitmapdata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            target.Lock();
            try
            {
                BitmapExtensions.CopyMemory(target.BackBuffer, bitmapdata.Scan0, (uint)(target.BackBufferStride * bitmapdata.Height));
                target.AddDirtyRect(new Int32Rect(0, 0, bitmapdata.Width, bitmapdata.Height));
            }
            finally
            {
                target.Unlock();
                source.UnlockBits(bitmapdata);
            }
        }
    }
}
