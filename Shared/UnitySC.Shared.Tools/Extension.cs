using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitySC.Shared.Tools
{
    public static class Extension
    {
        /// <summary>
        /// Convert un enum en string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static double StandardDeviation(this List<double> values)
        {
            double avg = values.Average();
            double sum = values.Sum(d => Math.Pow(d - avg, 2));
            return Math.Sqrt(sum / (values.Count() - 1));
        }

        public static bool Near(this double firstValToCompare, double secondValToCompare, double epsilon)
        {
            return Math.Abs(firstValToCompare - secondValToCompare) < epsilon;
        }

        public static bool IsNullOrNaN(this double? val)
        {
            return (val == null) || double.IsNaN((double)val);
        }

        public static bool IsNearOrBothNaN(this double val, double other, double epsilon)
        {
            return val.Near(other, epsilon) || (val.IsNaN() && other.IsNaN());
        }

        public static bool IsNaN(this double val)
        {
            return double.IsNaN(val);
        }

        /// <summary>
        /// Return the opposite of 'string.IsNullOrEmpty' call,
        /// i.e. it returns true if value is not empty and not null, false otherwise.
        /// </summary>
        public static bool HasContent(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///  Convertit une string en enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string str)
        {
            object o = Enum.Parse(typeof(T), str);
            return (T)o;
        }

        public static ImageFormat GetImageFormat(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (string.IsNullOrEmpty(ext))
                return ImageFormat.Png;

            switch (ext.ToLower())
            {
                case ".bmp":
                    return ImageFormat.Bmp;

                case ".gif":
                    return ImageFormat.Gif;

                case ".jpg":
                    return ImageFormat.Jpeg;

                case ".png":
                    return ImageFormat.Png;

                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;

                case ".wmf":
                    return ImageFormat.Wmf;
            }
            return null;
        }

        /// <summary>
        /// Save a WPF BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="filename"></param>
        public static void Save(this System.Windows.Media.Imaging.BitmapSource bitmap, string filename)
        {
            bitmap.Save(filename, GetImageFormat(filename));
        }

        public static void Save(this System.Windows.Media.Imaging.BitmapSource bitmap, string filename, ImageFormat format)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                BitmapEncoder encoder;
                if (format == ImageFormat.Bmp)
                    encoder = new BmpBitmapEncoder();
                else if (format == ImageFormat.Gif)
                    encoder = new GifBitmapEncoder();
                else if (format == ImageFormat.Jpeg)
                    encoder = new JpegBitmapEncoder();
                else if (format == ImageFormat.Png)
                    encoder = new PngBitmapEncoder();
                else if (format == ImageFormat.Tiff)
                    encoder = new TiffBitmapEncoder();
                else if (format == ImageFormat.Wmf)
                    encoder = new WmpBitmapEncoder();
                else
                    throw new ArgumentException("Unkown file image format");

                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Similar to Marshall.Copy but with two IntPtr
        /// </summary>
        public static class NativeMethods
        {
            [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
            public static extern void CopyMemory(IntPtr dest, IntPtr src, Int64 count);
        }

        public static DependencyObject FindVisualChildByName(this DependencyObject parent, string name)
        {
            return parent.FindVisualChild(dp => dp.GetValue(Control.NameProperty) as string == name);
        }

        public static DependencyObject FindVisualChild(this DependencyObject parent, Predicate<DependencyObject> predicate)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (predicate(child))
                    return child;

                var result = child.FindVisualChild(predicate);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static bool HasAttribute(this Type type, Attribute attr)
        {
            var attrtype = attr.GetType();

            foreach (Attribute xattr in type.GetCustomAttributes(true))
            {
                if (xattr.GetType() == attrtype)
                    return true;
            }

            return false;
        }

        public static string GetFlattenedMessage(this Exception ex)
        {
            string msg;

            var ae = ex as AggregateException;
            if (ae == null)
            {
                msg = ex.Message;
            }
            else
            {
                ae = ae.Flatten();
                msg = ae.InnerException.Message;
            }

            return msg;
        }

        public static async Task Finally(this Task t, Action action)
        {
            await t.ContinueWith(x =>
            {
                action.Invoke();
                if (x.Exception != null)
                    throw x.Exception;
            });
        }

        #region Base64

        /// <summary>
        /// To base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] data)
        {
            return Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks);
        }

        public static string ToBase64String(this byte[,] data)
        {
            int len = data.Length;
            byte[] buf = new byte[len];
            Buffer.BlockCopy(data, 0, buf, 0, len);
            return Convert.ToBase64String(buf, Base64FormattingOptions.InsertLineBreaks);
        }

        public static string ToBase64String(this uint[,] data)
        {
            int len = data.Length * sizeof(uint);
            byte[] buf = new byte[len];
            Buffer.BlockCopy(data, 0, buf, 0, len);
            return Convert.ToBase64String(buf, Base64FormattingOptions.InsertLineBreaks);
        }

        public static string ToBase64String(this float[,] data)
        {
            int len = data.Length * sizeof(float);
            byte[] buf = new byte[len];
            Buffer.BlockCopy(data, 0, buf, 0, len);
            return Convert.ToBase64String(buf, Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// From base 64
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dim1"></param>
        /// <param name="dim2"></param>
        /// <returns></returns>
        public static byte[,] Byte2DArrayFromBase64String(string str, int dim1, int dim2)
        {
            byte[] byteArray = Convert.FromBase64String(str);
            if (byteArray.Length != dim1 * dim2)
                throw new ApplicationException("invalid base64 length");

            byte[,] data = new byte[dim1, dim2];
            Buffer.BlockCopy(byteArray, 0, data, 0, byteArray.Length);
            return data;
        }

        public static uint[] UIntArrayFromBase64String(string str)
        {
            byte[] byteArray = Convert.FromBase64String(str);
            if (byteArray.Length % sizeof(uint) != 0)
                throw new ApplicationException("invalid base64 length");

            uint[] data = new uint[byteArray.Length / sizeof(uint)];
            Buffer.BlockCopy(byteArray, 0, data, 0, byteArray.Length);
            return data;
        }

        public static uint[,] UInt2DArrayFromBase64String(string str, int dim1, int dim2)
        {
            byte[] byteArray = Convert.FromBase64String(str);
            if (byteArray.Length != dim1 * dim2 * sizeof(uint))
                throw new ApplicationException("invalid base64 length");

            uint[,] data = new uint[dim1, dim2];
            Buffer.BlockCopy(byteArray, 0, data, 0, byteArray.Length);
            return data;
        }

        public static float[] FloatArrayFromBase64String(string str)
        {
            byte[] byteArray = Convert.FromBase64String(str);
            if (byteArray.Length % sizeof(float) != 0)
                throw new ApplicationException("invalid base64 length");

            float[] data = new float[byteArray.Length / sizeof(float)];
            Buffer.BlockCopy(byteArray, 0, data, 0, byteArray.Length);
            return data;
        }

        public static float[,] Float2DArrayFromBase64String(string str, int dim1, int dim2)
        {
            byte[] byteArray = Convert.FromBase64String(str);
            if (byteArray.Length != dim1 * dim2 * sizeof(float))
                throw new ApplicationException("invalid base64 length");

            float[,] data = new float[dim1, dim2];
            Buffer.BlockCopy(byteArray, 0, data, 0, byteArray.Length);
            return data;
        }

        #endregion Base64

        /// <summary>
        /// Base 64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i]))
                    return false;
            }
            return true;
        }

        public static bool ArraysEqual<T>(T[,] a1, T[,] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.GetLength(0) != a2.GetLength(0))
                return false;
            if (a1.GetLength(1) != a2.GetLength(1))
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.GetLength(0); i++)
            {
                for (int j = 0; j < a1.GetLength(1); j++)
                    if (!comparer.Equals(a1[i, j], a2[i, j])) return false;
            }
            return true;
        }

        public static string TrimDuplicateSpaces(this string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ");
        }

        //=================================================================
        // Sauve une image pour le debug
        //=================================================================

        #region ImageJ

        private static int s_fdcount = 0;
        private static readonly int s_fdcountmax = 100;

        public static bool ImageJ_CheckValidity()
        {
            string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
            if (viewer == null)
                viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

            if (string.IsNullOrEmpty(viewer) || string.IsNullOrWhiteSpace(viewer))
                return false;

            return File.Exists(viewer);
        }

        public static int ImageJ(this System.Drawing.Image image)
        {
            // Save image
            //...........
            s_fdcount = ++s_fdcount % s_fdcountmax;
            int idx = s_fdcount;
            string filename = PathString.GetTempPath() / "fdebug-" + idx + ".png";
            image.Save(filename, ImageFormat.Png);

            // Start ImageJ
            //.............
            ImageJ(filename);

            return idx;
        }

        public static int ImageJ(this System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            // Save image
            //...........
            s_fdcount = ++s_fdcount % s_fdcountmax;
            int idx = s_fdcount;
            string filename = PathString.GetTempPath() / "fdebug-" + idx + ".png";
            bitmap.Save(filename, ImageFormat.Png);

            // Start ImageJ
            //.............
            ImageJ(filename);
            return idx;
        }

        public static void ImageJ(string filename)
        {
            string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
            if (viewer == null)
                viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

            System.Diagnostics.Process.Start(viewer, filename);
        }

        public static System.Windows.Media.Color GetPixelColor(this BitmapSource source, int x, int y)
        {
            System.Windows.Media.Color c;
            var cb = new CroppedBitmap(source, new Int32Rect(x, y, 1, 1));
            byte[] pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);
            c = System.Windows.Media.Color.FromRgb(pixels[2], pixels[1], pixels[0]);
            return c;
        }

        #endregion ImageJ

        //=================================================================
        // Snapshot screen capture
        //=================================================================

        public static void CaptureScreenWindow(out MemoryStream ms, out int width, out int height)
        {
            double screenLeft = System.Windows.Application.Current.MainWindow.Left;
            double screenTop = System.Windows.Application.Current.MainWindow.Top;
            double screenWidth = System.Windows.Application.Current.MainWindow.Width;
            double screenHeight = System.Windows.Application.Current.MainWindow.Height;

            if (System.Windows.Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                var helper = new System.Windows.Interop.WindowInteropHelper(System.Windows.Application.Current.MainWindow);
                var currentScreen = System.Windows.Forms.Screen.FromHandle(helper.Handle);
                screenLeft = currentScreen.Bounds.Left;
                screenTop = currentScreen.Bounds.Top;
                screenWidth = currentScreen.Bounds.Width;
                screenHeight = currentScreen.Bounds.Height;
            }

            ms = new MemoryStream();
            width = (int)screenWidth;
            height = (int)screenHeight;
            using (var bitmap = new System.Drawing.Bitmap(width, height))
            {
                using (var g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bitmap.Size);
                }
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
            }

            // dont forget to dispose memory stream
        }

        // use
        //  var Bitm = Extension.CaptureScreenWindow();
        //  Bitm.Save($"c:\\temp\\capture.png", System.Drawing.Imaging.ImageFormat.Png);
        //
        public static System.Drawing.Bitmap CaptureScreenWindow()
        {
            double screenLeft = System.Windows.Application.Current.MainWindow.Left;
            double screenTop = System.Windows.Application.Current.MainWindow.Top;
            double screenWidth = System.Windows.Application.Current.MainWindow.Width;
            double screenHeight = System.Windows.Application.Current.MainWindow.Height;

            if (System.Windows.Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                //// First primary screen -- Issue when windown is on another screen
                //screenLeft = 0.0;
                //screenTop = 0.0;
                //screenWidth = SystemParameters.PrimaryScreenWidth;
                //screenHeight = SystemParameters.PrimaryScreenHeight;

                //// Take all screens -- Issue when you have numerous screens (huge image)
                //screenLeft = SystemParameters.VirtualScreenLeft;
                //screenTop = SystemParameters.VirtualScreenTop;
                //screenWidth = SystemParameters.VirtualScreenWidth;
                //screenHeight = SystemParameters.VirtualScreenHeight;

                // Work OK - but need  System.Windows.Forms
                var helper = new System.Windows.Interop.WindowInteropHelper(System.Windows.Application.Current.MainWindow); //this being the wpf form
                var currentScreen = System.Windows.Forms.Screen.FromHandle(helper.Handle);
                screenLeft = currentScreen.Bounds.Left;
                screenTop = currentScreen.Bounds.Top;
                screenWidth = currentScreen.Bounds.Width;
                screenHeight = currentScreen.Bounds.Height;
            }

            var bitmap = new System.Drawing.Bitmap((int)screenWidth, (int)screenHeight);
            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bitmap.Size);
            }
            return bitmap;
            // dont forget to dispose bitmap after use
        }

        public static BitmapImage CaptureScreenWindow_MediaImaging()
        {
            CaptureScreenWindow(out var ms, out int width, out int height);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static void WriteFormat(this TextWriter sw, string format, params object[] args)
        {
            string str = String.Format(format, args);
            sw.Write(str);
        }
    }
}
