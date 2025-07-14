using System.Drawing;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Same as DataInputStream, but for write operations that take longer.
    /// Data will typically be buffered/fps capped between DataInputStream and DataInputAsync.
    /// </summary>
    public interface DataInputAsync<T>
    {
        /// <summary>
        /// Write some data to a specific point in the image.
        /// throws DataException.
        /// </summary>
        Task WriteAsync(Point point, T data);

        /// <summary>
        /// Disposes the stream (and its childs).
        /// throws DataException.
        /// </summary>
        Task DisposeAsync();
    }
}

namespace UnitySC.Shared.Ext
{
    public static class DataInputAsyncExt
    {
        /// <summary>
        /// Some streams, like image preview, do not care about position.
        /// </summary>
        public static Task WriteAsync<T>(this UnitySC.PM.Shared.Hardware.Camera.DataInput.DataInputAsync<T> ùThis, T data)
        {
            return ùThis.WriteAsync(new Point(), data);
        }
    }
}