using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace nU3.Core.UI.Shell.Services
{
    /// <summary>
    /// Screenshot capture service
    /// Provides methods to capture screen, forms, and controls
    /// </summary>
    public static class ScreenshotService
    {
        /// <summary>
        /// Capture the entire primary screen
        /// </summary>
        /// <param name="fileName">Output file path (PNG format)</param>
        /// <returns>True if successful</returns>
        public static bool CaptureScreen(string fileName)
        {
            try
            {
                var bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                using var bitmap = new Bitmap(bounds.Width, bounds.Height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                EnsureDirectoryExists(fileName);
                bitmap.Save(fileName, ImageFormat.Png);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureScreen failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture all screens (multi-monitor)
        /// </summary>
        public static bool CaptureAllScreens(string fileName)
        {
            try
            {
                var allScreens = Screen.AllScreens;
                var totalWidth = 0;
                var maxHeight = 0;

                foreach (var screen in allScreens)
                {
                    totalWidth += screen.Bounds.Width;
                    maxHeight = Math.Max(maxHeight, screen.Bounds.Height);
                }

                using var bitmap = new Bitmap(totalWidth, maxHeight);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var x = 0;
                    foreach (var screen in allScreens)
                    {
                        graphics.CopyFromScreen(
                            screen.Bounds.Location,
                            new Point(x, 0),
                            screen.Bounds.Size);
                        x += screen.Bounds.Width;
                    }
                }

                EnsureDirectoryExists(fileName);
                bitmap.Save(fileName, ImageFormat.Png);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureAllScreens failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture a specific form
        /// </summary>
        /// <param name="form">Form to capture</param>
        /// <param name="fileName">Output file path (PNG format)</param>
        /// <returns>True if successful</returns>
        public static bool CaptureForm(Form form, string fileName)
        {
            try
            {
                if (form == null || form.IsDisposed)
                    return false;

                var bounds = form.Bounds;
                using var bitmap = new Bitmap(bounds.Width, bounds.Height);
                
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action(() => form.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height))));
                }
                else
                {
                    form.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height));
                }

                EnsureDirectoryExists(fileName);
                bitmap.Save(fileName, ImageFormat.Png);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureForm failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture a specific control
        /// </summary>
        public static bool CaptureControl(Control control, string fileName)
        {
            try
            {
                if (control == null || control.IsDisposed)
                    return false;

                using var bitmap = new Bitmap(control.Width, control.Height);
                
                if (control.InvokeRequired)
                {
                    control.Invoke(new Action(() => control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height))));
                }
                else
                {
                    control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));
                }

                EnsureDirectoryExists(fileName);
                bitmap.Save(fileName, ImageFormat.Png);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureControl failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture screen to memory stream (for email attachments, etc.)
        /// </summary>
        public static MemoryStream? CaptureScreenToStream()
        {
            try
            {
                var bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                using var bitmap = new Bitmap(bounds.Width, bounds.Height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureScreenToStream failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Capture form to memory stream
        /// </summary>
        public static MemoryStream? CaptureFormToStream(Form form)
        {
            try
            {
                if (form == null || form.IsDisposed)
                    return null;

                var bounds = form.Bounds;
                using var bitmap = new Bitmap(bounds.Width, bounds.Height);
                
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action(() => form.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height))));
                }
                else
                {
                    form.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height));
                }

                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureFormToStream failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Capture screen to byte array
        /// </summary>
        public static byte[]? CaptureScreenToBytes()
        {
            using var stream = CaptureScreenToStream();
            return stream?.ToArray();
        }

        /// <summary>
        /// Capture form to byte array
        /// </summary>
        public static byte[]? CaptureFormToBytes(Form form)
        {
            using var stream = CaptureFormToStream(form);
            return stream?.ToArray();
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
