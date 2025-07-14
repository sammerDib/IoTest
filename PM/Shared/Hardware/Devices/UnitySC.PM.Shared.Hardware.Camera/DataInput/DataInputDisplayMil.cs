using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Updates a WriteableBitmap from a stream of MillImages.
    /// Currently, only 8 bits grey images are supported.
    /// </summary>
    public class DataInputDisplayMil :
        DataInputAsync<MilImage>
    {
        public Task DisposeAsync()
        {
            return Task.FromResult(true);
        }

        public async Task WriteAsync(Point point, MilImage data)
        {
            await data.CopyToAsync(DisplaySource).ConfigureAwait(false);

            await DisplaySource.Dispatcher;
            OnImageUpdated_gui?.Invoke();
        }

        /// <summary>
        /// Called from the UI thread after each image update.
        /// </summary>
        public Action OnImageUpdated_gui;

        /// <summary>
        /// Display source.
        /// Must be set before acquisition, and must match the future incoming MillImages!
        /// </summary>
        public WriteableBitmap DisplaySource { get; set; } = new WriteableBitmap(1, 1, 46d, 46d, PixelFormats.Gray8, null);
    }
}