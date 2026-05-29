using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace ReDo.Utility
{
    public static class ScreenCapture
    {
        public static int VirtualScreenLeft => (int)SystemParameters.VirtualScreenLeft;
        public static int VirtualScreenTop => (int)SystemParameters.VirtualScreenTop;
        public static int VirtualScreenWidth => (int)SystemParameters.VirtualScreenWidth;
        public static int VirtualScreenHeight => (int)SystemParameters.VirtualScreenHeight;

        public static Bitmap CaptureVirtualScreen()
        {
            var bitmap = new Bitmap(VirtualScreenWidth, VirtualScreenHeight, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    VirtualScreenLeft,
                    VirtualScreenTop,
                    0,
                    0,
                    new System.Drawing.Size(VirtualScreenWidth, VirtualScreenHeight),
                    CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }

        public static Bitmap CaptureRegion(int screenX, int screenY, int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Capture region must have positive dimensions.");

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screenX, screenY, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }

        public static string BitmapToBase64Png(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static Bitmap Base64PngToBitmap(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(bytes))
            {
                return new Bitmap(stream);
            }
        }
    }
}
