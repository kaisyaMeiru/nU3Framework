using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace nU3.Shell.Helpers
{
    public static class ScreenshotHelper
    {
        public static bool CaptureScreen(string fileName)
        {
            try
            {
                var bounds = Screen.PrimaryScreen.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    
                    var directory = Path.GetDirectoryName(fileName);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    bitmap.Save(fileName, ImageFormat.Png);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CaptureForm(Form form, string fileName)
        {
            try
            {
                var bounds = form.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height));
                    
                    var directory = Path.GetDirectoryName(fileName);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    bitmap.Save(fileName, ImageFormat.Png);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static MemoryStream CaptureScreenToStream()
        {
            try
            {
                var bounds = Screen.PrimaryScreen.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    
                    var stream = new MemoryStream();
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    return stream;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
