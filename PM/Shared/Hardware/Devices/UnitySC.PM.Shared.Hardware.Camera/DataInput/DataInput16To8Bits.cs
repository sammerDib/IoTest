using System;
using System.Drawing;
using System.Threading.Tasks;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Converts a 16 bits bi dimensional array to 8 bits.
    /// </summary>
    public class DataInput16To8Bits : DataInputAsync<UInt16[,]>
    {
        /// <summary>
        /// Target stream.
        /// </summary>
        public DataInputAsync<byte[,]> Target;

        public Task DisposeAsync()
        {
            return Target.DisposeAsync();
        }

        /// <summary>
        /// 8 bits buffer.
        /// </summary>
        private byte[,] _buffer = new byte[0, 0];

        public async Task WriteAsync(Point point, ushort[,] data)
        {
            await ThreadPoolTools.Post;

            // Allocate buffer if needed.
            if ((data.GetLength(0) != _buffer.GetLength(0)) || (data.GetLength(1) != _buffer.GetLength(1)))
            {
                _buffer = new byte[data.GetLength(0), data.GetLength(1)];
            }

            unsafe
            {
                fixed (UInt16* sourceStartPtr = data)
                fixed (byte* targetStartPtr = _buffer)
                {
                    byte* sourcePtr = ((byte*)sourceStartPtr) + 1;
                    byte* targetPtr = targetStartPtr;

                    UInt16* sourceEndPtr = sourceStartPtr + data.Length;
                    while (sourcePtr < sourceEndPtr)
                    {
                        *(targetPtr++) = *sourcePtr;
                        sourcePtr += 2;
                    }
                }
            }

            await Target.WriteAsync(point, _buffer);
        }
    }
}
