using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MStack.MainSite.WebFramework.Untils
{
    public class ImageUntils
    {
        public static Bitmap CropImage(Bitmap b, Rectangle rect)
        {
            if (b == null)
            {
                return null;
            }
            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;
            int w = b.Width;
            int h = b.Height;
            if (x >= w || y >= h)
            {
                return null;
            }
            if (x + width > w)
            {
                width = w - x;
            }
            if (y + height > h)
            {
                height = h - y;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }

        }

        public static void CropImage(string sourcePath, string savePath, Rectangle rect)
        {
            using (var b = new Bitmap(sourcePath))
            {
                var outBitmap = CropImage(b, rect);
                outBitmap.Save(savePath);
                outBitmap.Dispose();
            }
        }

    }
}