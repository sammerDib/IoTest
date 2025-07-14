using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnitySC.Shared.Image
{
    public class CSharpImage
    {
        /// <summary> In pixels </summary>
        public int width, height;

        /// <summary> In bytes </summary>
        public int pitch;

        /// <summary> In bytes </summary>
        public int depth;
        
        /// <summary> Image buffer data pointer </summary>
        public long ptr;

        public CSharpImage()
        {
            
        }
        
        public CSharpImage(byte[,] data, int depth)
        {
            this.depth = depth;
            DepthCheck(8);
            
            width = data.GetLength(0);
            height = data.GetLength(1);
            
            pitch = GetPitch(width, this.depth);
                
            var ptr = Marshal.AllocHGlobal(height * pitch);
            byte[] source = new byte[height * pitch];
                
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    source[y * pitch + x] = data[x, y];
                }
            }
                
            Marshal.Copy(source, 0, ptr, height * pitch);
            this.ptr = (long)ptr;
        }

        private void BoundCheck(int x, int y, int depth)
        {
            bool ok = 0 <= x && x < width;
            ok = ok && 0 <= y && y < height;
            if (!ok)
                throw new ApplicationException("invalid pixel coordinates x,y=" + x + "," + y + " WxH=" + width + "x" + height);
            DepthCheck(depth);
        }

        private void DepthCheck(int depth)
        {
            if (depth != this.depth)
                throw new ApplicationException("invalid image depth: " + this.depth + " ,expecting: " + depth);
        }
        
        private void PtrCheck()
        {
            if (ptr == 0)
                throw new ApplicationException("invalid image ptr : 0");
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
        
        //Return the size in bytes of a row
        //knowing that it should be a multiple of 8
        public static int GetPitch(int width, int bpp)
        {
            int widthInBytes = width * bpp;
            //The 3 last bits are emptied and if at least one of them was one
            //then the 4th rightmost bit is increased
            //example:
            //0b10010110 = 150
            //will give a pitch/stride of
            //0b10011000 = 152
            return ((widthInBytes + 0b0111) >> 3) << 3;
        }
        
        public byte[,] GetDataAsByteArray()
        {
            PtrCheck();
            
            DepthCheck(8);
            
            byte[,] data = new byte[width, height];

            for (int y = 0; y < height; y++)
            {
                 for (int x = 0; x < width; x++)
                 {
                      data[x, y] = uint8(x, y);
                  }
            }

            return data;
        }
    }
}
