using System.Drawing;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    public interface DataInputStream<T>
    {
        /// <summary>
        /// Write some data to a specific point in the image.
        /// throws DataException.
        /// Very fast (may buffer some data for longer processings), compatible with, for example, a DMA polling thread.
        /// </summary>
        void Write(Point point, T data);

        /// <summary>
        /// Disposes the stream (and its childs).
        /// throws DataException.
        /// </summary>
        Task DisposeAsync();
    }

    internal class DataInputStreamAsAsync<T> : DataInputAsync<T>
    {
        public DataInputStream<T> Stream;

        public Task DisposeAsync()
        {
            return Stream.DisposeAsync();
        }

        public Task WriteAsync(Point point, T data)
        {
            Stream.Write(point, data);
            return Task.FromResult(true);
        }
    }
}

namespace UnitySC.Shared.Ext
{
    public static class ImageStreamExt
    {
        /// <summary>
        /// Some streams, like image preview, do not care about position.
        /// </summary>
        public static void Write<T>(this UnitySC.PM.Shared.Hardware.Camera.DataInput.DataInputStream<T> ùThis, T data)
        {
            ùThis.Write(new Point(), data);
        }

        public static PM.Shared.Hardware.Camera.DataInput.DataInputAsync<T> AsAsync<T>(this UnitySC.PM.Shared.Hardware.Camera.DataInput.DataInputStream<T> ùThis)
        {
            return new PM.Shared.Hardware.Camera.DataInput.DataInputStreamAsAsync<T>() { Stream = ùThis };
        }
    }
}