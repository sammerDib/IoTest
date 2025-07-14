using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Displays a simple bi-dimentional 8 bits array.
    /// </summary>
    public class DataInputDisplayGreyArray : DataInputAsync<byte[,]>
    {
        public Task DisposeAsync()
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// ImageSource ready to be displayed.
        /// </summary>
        public WriteableBitmap Bitmap { get; private set; } = new WriteableBitmap(1, 1, 93d, 46d, PixelFormats.Gray8, null);

        public async Task WriteAsync(System.Drawing.Point point, byte[,] data)
        {
            // Passage en UI thread.
            await Bitmap.Dispatcher;

            // Allocate bitmap if needed.
            if ((Bitmap.PixelWidth != data.GetLength(0)) || (Bitmap.PixelHeight != data.GetLength(1)))
            {
                Bitmap = new WriteableBitmap(data.GetLength(0), data.GetLength(1), 46d, 46d, PixelFormats.Gray8, null);
            }

            // Copy data.
            Bitmap.WritePixels(new Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight), data, Bitmap.PixelWidth, 0, 0);
        }
    }
}