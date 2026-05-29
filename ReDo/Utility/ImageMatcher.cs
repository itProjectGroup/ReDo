using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ReDo.Utility
{
    public class ImageMatchResult
    {
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Finds a template image on screen using normalized cross-correlation (NCC),
    /// the same approach used by RPA tools like UiPath for image-based targeting.
    /// </summary>
    public static class ImageMatcher
    {
        public static ImageMatchResult FindTemplate(Bitmap screen, Bitmap template, double threshold)
        {
            if (screen == null || template == null)
                return null;

            if (template.Width > screen.Width || template.Height > screen.Height)
                return null;

            var screenGray = ToGrayscale(screen);
            var templateGray = ToGrayscale(template);

            int screenW = screen.Width;
            int screenH = screen.Height;
            int tplW = template.Width;
            int tplH = template.Height;

            double templateMean = Mean(templateGray, tplW, tplH);
            double templateStd = StdDev(templateGray, tplW, tplH, templateMean);
            if (templateStd < 1e-6)
                return null;

            double bestScore = double.MinValue;
            int bestX = -1;
            int bestY = -1;

            // Coarse scan (every 4 px), then refine in a 3×3 neighborhood.
            const int coarseStep = 4;
            for (int y = 0; y <= screenH - tplH; y += coarseStep)
            {
                for (int x = 0; x <= screenW - tplW; x += coarseStep)
                {
                    double score = NormalizedCrossCorrelation(screenGray, screenW, screenH, x, y, tplW, tplH, templateGray, templateMean, templateStd);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestX = x;
                        bestY = y;
                    }
                }
            }

            if (bestX >= 0)
            {
                int refineStartX = Math.Max(0, bestX - coarseStep);
                int refineStartY = Math.Max(0, bestY - coarseStep);
                int refineEndX = Math.Min(screenW - tplW, bestX + coarseStep);
                int refineEndY = Math.Min(screenH - tplH, bestY + coarseStep);

                for (int y = refineStartY; y <= refineEndY; y++)
                {
                    for (int x = refineStartX; x <= refineEndX; x++)
                    {
                        double score = NormalizedCrossCorrelation(screenGray, screenW, screenH, x, y, tplW, tplH, templateGray, templateMean, templateStd);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestX = x;
                            bestY = y;
                        }
                    }
                }
            }

            if (bestScore < threshold || bestX < 0)
                return null;

            int offsetX = ScreenCapture.VirtualScreenLeft;
            int offsetY = ScreenCapture.VirtualScreenTop;

            return new ImageMatchResult
            {
                ScreenX = bestX + offsetX,
                ScreenY = bestY + offsetY,
                CenterX = bestX + offsetX + tplW / 2,
                CenterY = bestY + offsetY + tplH / 2,
                Confidence = bestScore
            };
        }

        private static double NormalizedCrossCorrelation(
            byte[] screen, int screenW, int screenH,
            int startX, int startY, int tplW, int tplH,
            byte[] template, double templateMean, double templateStd)
        {
            double windowMean = MeanWindow(screen, screenW, startX, startY, tplW, tplH);
            double windowStd = StdDevWindow(screen, screenW, startX, startY, tplW, tplH, windowMean);
            if (windowStd < 1e-6)
                return -1;

            double sum = 0;
            int count = tplW * tplH;
            for (int ty = 0; ty < tplH; ty++)
            {
                int screenRow = (startY + ty) * screenW + startX;
                int tplRow = ty * tplW;
                for (int tx = 0; tx < tplW; tx++)
                {
                    double s = screen[screenRow + tx] - windowMean;
                    double t = template[tplRow + tx] - templateMean;
                    sum += s * t;
                }
            }

            return sum / (count * windowStd * templateStd);
        }

        private static byte[] ToGrayscale(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                int stride = data.Stride;
                int width = bitmap.Width;
                int height = bitmap.Height;
                var gray = new byte[width * height];
                var buffer = new byte[stride * height];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int i = rowOffset + x * 3;
                        byte b = buffer[i];
                        byte g = buffer[i + 1];
                        byte r = buffer[i + 2];
                        gray[y * width + x] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                    }
                }
                return gray;
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        private static double Mean(byte[] pixels, int width, int height)
        {
            double sum = 0;
            for (int i = 0; i < pixels.Length; i++)
                sum += pixels[i];
            return sum / (width * height);
        }

        private static double StdDev(byte[] pixels, int width, int height, double mean)
        {
            double sumSq = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                double diff = pixels[i] - mean;
                sumSq += diff * diff;
            }
            return Math.Sqrt(sumSq / (width * height));
        }

        private static double MeanWindow(byte[] screen, int screenW, int startX, int startY, int tplW, int tplH)
        {
            double sum = 0;
            int count = tplW * tplH;
            for (int y = 0; y < tplH; y++)
            {
                int row = (startY + y) * screenW + startX;
                for (int x = 0; x < tplW; x++)
                    sum += screen[row + x];
            }
            return sum / count;
        }

        private static double StdDevWindow(byte[] screen, int screenW, int startX, int startY, int tplW, int tplH, double mean)
        {
            double sumSq = 0;
            int count = tplW * tplH;
            for (int y = 0; y < tplH; y++)
            {
                int row = (startY + y) * screenW + startX;
                for (int x = 0; x < tplW; x++)
                {
                    double diff = screen[row + x] - mean;
                    sumSq += diff * diff;
                }
            }
            return Math.Sqrt(sumSq / count);
        }
    }
}
