using System;
using System.Runtime.CompilerServices;

namespace LibProcessing
{
    public class CSharpImage
    {
        public int width, height;
        /// <summary> In bytes </summary>
        public int pitch;
        public int depth;
        public long ptr;

        private void BoundCheck(int x, int y, int depth)
        {
            bool ok = 0 <= x && x < width;
            ok = ok && 0 <= y && y < height;
            if (!ok)
                throw new ApplicationException("invalid pixel coordinates x,y=" + x + "," + y + " WxH=" + width + "x" + height);
            if (depth != this.depth)
                throw new ApplicationException("invalid image depth: " + this.depth + " ,expecting: " + depth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte uint8(int x, int y)
        {
#if DEBUG
            BoundCheck(x, y, 8);
#endif
            unsafe
            {
                byte* bptr = (byte*)ptr;
                int index = y * pitch + x;
                return ref bptr[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref UInt16 uint16(int x, int y)
        {
#if DEBUG
            BoundCheck(x, y, 16);
#endif
            unsafe
            {
                UInt16* bptr = (UInt16*)ptr;
                int index = y * pitch + x;
                return ref bptr[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float float32(int x, int y)
        {
#if DEBUG
            BoundCheck(x, y, 32);
#endif
            unsafe
            {
                float* bptr = (float*)ptr;
                int index = y * pitch + x;
                return ref bptr[index];
            }
        }
    }

}
