using System;
using System.Configuration;

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;

namespace UnitySC.Shared.LibMIL
{
    public class MilImage : AMilId
    {
        //=================================================================
        // Properties
        //=================================================================
        public int SizeX
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_X); }
        }

        public int SizeY
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_Y); }
        }

        public int Pitch
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_PITCH_BYTE); }
        }

        public int Type
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_TYPE); }
        }

        public int Attribute
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_ATTRIBUTE); }
        }

        public long SizeByte
        {
            get { return (long)MIL.MbufInquire(MilId, MIL.M_SIZE_BYTE); }
        }

        public int SizeBand
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_BAND); }
        }

        public int SizeBit
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_BIT); }
        }

        /// <summary>
        /// Gets the depth of a pixel in the image.
        /// </summary>
        public Int32 PixelDepth_bit => SizeBit;

        public Int32 PixelDepth_byte => SizeBit >> 3;

        public MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MbufInquire(_milId, MIL.M_OWNER_SYSTEM); }
        }

        public MIL_ID ParentId
        {
            get { return (MIL_ID)MIL.MbufInquire(MilId, MIL.M_PARENT_ID); }
        }

        public MIL_ID AncestorId
        {
            get { return (MIL_ID)MIL.MbufInquire(MilId, MIL.M_ANCESTOR_ID); }
        }

        public MIL_ID AssociatedLut
        {
            get { return (MIL_ID)MIL.MbufInquire(MilId, MIL.M_ASSOCIATED_LUT); }
        }

        // Indicates that the object owns its MIL buffer,
        // and shall free it upon deletion
        protected bool OwnImage = false;

        protected bool IsChildImage = false;

        // For debug
        protected static int NbImages = 0;

        protected static int NbChildImages = 0;
        protected static long InternalImageMemory = 0;

        public static long ImageMemory
        {
            get { return InternalImageMemory; }
        }

        //public static long MaxImageMemory = 512000000000L;
        //public static bool maxReached = false;
        public static int DiskInquire(String fileName, MIL_INT flag)
        {
            int DataInquire = 0;

            try
            {
                DataInquire = (int)MIL.MbufDiskInquire(fileName, flag);
            }
            catch (Exception ex)
            {
                string msg = "In file \"" + fileName + "\" " + ex.Message;
                throw new ApplicationException(msg, ex);
            }

            return DataInquire;
        }

        //=================================================================
        // Constructor
        //=================================================================
        public MilImage()
        {
        }

        public MilImage(MIL_ID milId, bool transferOnwership)
        {
            AttachMilId(milId, transferOnwership);
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_milId != MIL.M_NULL && OwnImage)
                {
                    var id = DetachMilId();
#if DEBUG
                    Console.WriteLine($"Dispose DetachMilId id: {id}");
#endif
                    MIL.MbufFree(id);
                }
                _milId = MIL.M_NULL;

                base.Dispose(disposing);
            }
#if DEBUG
            catch (Exception e)
#else
            catch (Exception)
#endif
            {
#if DEBUG
                Console.WriteLine($"Dispose MilImage : {e.Message}");
                Console.WriteLine($"Dispose MilImage : {e.StackTrace}");
#endif
                System.Diagnostics.Debugger.Break();
            }
        }

        //=================================================================
        // Clone
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            var clone = (MilImage)obj;
            clone._milId = MIL.M_NULL;
            if (MilId != MIL.M_NULL)
            {
                clone.Alloc2d(OwnerSystem, SizeX, SizeY, Type, Attribute);
                Copy(this, clone);
            }
        }

        //=================================================================
        // Attache/Détache le MilId de la MilImage
        //=================================================================
        public void AttachMilId(MIL_ID milId, bool transferOnwership)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            _milId = milId;
            OwnImage = transferOnwership;
            if (OwnImage)
            {
                Interlocked.Increment(ref NbImages);
                Interlocked.Add(ref InternalImageMemory, SizeByte);
                GC.AddMemoryPressure(SizeByte);
            }
        }

        public MIL_ID DetachMilId()
        {
            var id = _milId;

            if (_milId != MIL.M_NULL && OwnImage)
            {
                OwnImage = false;

                if (IsChildImage)
                {
                    int nbChildImages = Interlocked.Decrement(ref NbChildImages);
                    IsChildImage = false;
                }
                else
                {
                    int nbImages = Interlocked.Decrement(ref NbImages);
#if DEBUG
                    Console.WriteLine($"NbImages : {NbImages} - nbImages : {nbImages}");

                    if(SizeByte <= 0)
                        System.Diagnostics.Debugger.Break();
#endif
                    long memory = SizeByte;
                    long imageMemory = Interlocked.Add(ref InternalImageMemory, -memory);
                    GC.RemoveMemoryPressure(memory);
                    //System.Diagnostics.Debug.WriteLine("nbImages="+nbImages+" mem="+_imageMemory);
                    if (nbImages < 0 || imageMemory < 0)
                        throw new System.ApplicationException("Internal Error: Free'd too much images");
                }
            }
            _milId = MIL.M_NULL;

            return id;
        }

        //=================================================================
        // Allocate
        //=================================================================
        public void Alloc2dCompatibleWith(MilImage img)
        {
            Alloc2d(img.OwnerSystem, img.SizeX, img.SizeY, img.Type, img.Attribute);
        }
        
        public void AllocCompatibleWith(MilImage img)
        {
            if (img.SizeBand == 1)
                Alloc2d(img.OwnerSystem, img.SizeX, img.SizeY, img.Type, img.Attribute);
            else
                AllocColor(img.OwnerSystem, img.SizeBand, img.SizeX, img.SizeY, img.Type, img.Attribute);
        }

        public void Alloc2d(long sizeX, long sizeY, MIL_INT type, long attribute)
        {
            Alloc2d(Mil.Instance.HostSystem, sizeX, sizeY, type, attribute);
        }

        public void Alloc2d(MIL_ID systemId, long sizeX, long sizeY, MIL_INT type, long attribute)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            _milId = MIL.MbufAlloc2d(systemId, sizeX, sizeY, type, attribute, MIL.M_NULL);

            //// MIL just crashes when we allocate more buffers than supported !
            //// the comparison below is wrong because:
            //// _milID can be greater than MILIDTableSize ; it doesn't mean that there are no more buffers available.
            //// Let's remove this comparison and let the checkMilError to raise an exception if needed.
            //if (_milId > CutDies.MILIDTableSize)
            //    throw new System.ApplicationException("MIL is out of buffers");

            checkMilError("Failed to allocate 2D image");

            OwnImage = true;
            IsChildImage = false;
            Interlocked.Increment(ref NbImages);
            Interlocked.Add(ref InternalImageMemory, SizeByte);
            GC.AddMemoryPressure(SizeByte);
        }

        public void AllocColor(int sizeBand, long sizeX, long sizeY, MIL_INT type, long attribute)
        {
            AllocColor(Mil.Instance.HostSystem, sizeBand, sizeX, sizeY, type, attribute);
        }

        public void AllocColor(MIL_ID systemId, int sizeBand, long sizeX, long sizeY, MIL_INT type, long attribute)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            _milId = MIL.MbufAllocColor(systemId, sizeBand, sizeX, sizeY, type, attribute, MIL.M_NULL);
            checkMilError("Failed to allocate 2D color image");

            OwnImage = true;
            IsChildImage = false;
            Interlocked.Increment(ref NbImages);
            Interlocked.Add(ref InternalImageMemory, SizeByte);
            GC.AddMemoryPressure(SizeByte);
        }

        //=================================================================
        // Create Buff 2d
        //=================================================================
        public void Create2d(MIL_ID milSystem, int sizeX, int sizeY, int type, int attribute, int controlFlag, int pitch, ulong dataPtr)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            MIL.MbufCreate2d(milSystem, sizeX, sizeY, type, attribute, controlFlag, pitch, dataPtr, ref _milId);

            OwnImage = true;
            IsChildImage = false;
            Interlocked.Increment(ref NbImages);
            Interlocked.Add(ref InternalImageMemory, SizeByte);
        }

        //=================================================================
        // Get a Child Image
        //=================================================================
        public void Child2d(MilImage parentImg, long offX, long offY, long sizeX, long sizeY)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            MIL.MbufChild2d(parentImg.MilId, offX, offY, sizeX, sizeY, ref _milId);
            checkMilError("Failed to create child buffer");

            OwnImage = true;
            IsChildImage = true;
            Interlocked.Increment(ref NbChildImages);
        }

        public void Child2d(MilImage parentImg, Rect rect)
        {
            Child2d(parentImg, (long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height);
        }

        public void ChildMove(MIL_INT offsetX, MIL_INT offsetY, MIL_INT sizeX, MIL_INT sizeY, long controlFlag = MIL.M_DEFAULT)
        {
            MIL.MbufChildMove(_milId, offsetX, offsetY, sizeX, sizeY, controlFlag);
            checkMilError("Failed to move child image");
        }

        public void ChildMove(Rect rect, long controlFlag = MIL.M_DEFAULT)
        {
            ChildMove((long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height, controlFlag);
        }

        //=================================================================
        // Lock/Unlock
        //=================================================================
        public void Lock(int mode = MIL.M_READ + MIL.M_WRITE)
        {
            MIL.MbufControl(_milId, MIL.M_LOCK + mode, MIL.M_DEFAULT);
            checkMilError("Failed to lock buffer");
            IsLocked = true;
        }

        public void Unlock()
        {
            MIL.MbufControl(_milId, MIL.M_UNLOCK, MIL.M_DEFAULT);
            checkMilError("Failed to unlock buffer");
            IsLocked = false;
        }

        public bool IsLocked { get; private set; }

        public long HostAddress
        {
            get
            {
                return MIL.MbufInquire(MilId, MIL.M_HOST_ADDRESS, MIL.M_NULL);
            }
        }

        //=================================================================
        // Clear image
        //=================================================================
        public void Clear(double color = 0)
        {
            MIL.MbufClear(MilId, color);
            checkMilError("Failed to clear image");
        }

        //=================================================================
        // Copy an Image to antoher image
        //=================================================================
        public static void Copy(MilImage src, MilImage dest)
        {
            MIL.MbufCopy(src.MilId, dest.MilId);
            checkMilError("Failed to copy image");
        }

        public MilImage Clone()
        {
            MilImage ret = new MilImage();
            ret.Alloc2d(SizeX, SizeY, Type, MIL.M_IMAGE + MIL.M_PROC);

            MilImage.Copy(this, ret);
            return ret;
        }

        public static void CopyClip(MilImage src, MilImage dest, int destOffX, int destOffY)
        {
            MIL.MbufCopyClip(src.MilId, dest.MilId, destOffX, destOffY);
            checkMilError("Failed to copy image");
        }

        public static void CopyColor(MilImage src, MilImage dest, MIL_INT band)
        {
            MIL.MbufCopyColor(src.MilId, dest.MilId, band);
            checkMilError("Failed to copy image");
        }

        public static void CopyColor2d(MilImage src, MilImage dest,
                                       MIL_INT srcBand, int srcOffX, int srcOffY,
                                       MIL_INT destBand, int destOffX, int destOffY,
                                       int sizeX, int sizeY)
        {
            MIL.MbufCopyColor2d(src.MilId, dest.MilId, srcBand, srcOffX, srcOffY, destBand, destOffX, destOffY, sizeX, sizeY);
            checkMilError("Failed to copy image");
        }

        public static void Transfer(MilImage src, MilImage dest, MIL_INT srcOffX, MIL_INT srcOffY, MIL_INT srcSizeX, MIL_INT srcSizeY, MIL_INT srcBand, MIL_INT destOffX, MIL_INT destOffY, MIL_INT destSizeX, MIL_INT destSizeY, MIL_INT destBand, long transferFunction, long transferType, long operationFlag, IntPtr extraParameterPtr)
        {
            MIL.MbufTransfer(src, dest, srcOffX, srcOffY, srcSizeX, srcSizeY, srcBand, destOffX, destOffY, destSizeX, destSizeY, destBand, transferFunction, transferType, operationFlag, extraParameterPtr);
        }

        public void Resize(double scaleFactorX, double scaleFactorY, long interpolationMode)
        {
            MIL.MimResize(MilId, MilId, scaleFactorX, scaleFactorY, interpolationMode);
        }

        public static void Resize(MilImage srcImage, MilImage dstImage, double scaleFactorX, double scaleFactorY, long interpolationMode)
        {
            MIL.MimResize(srcImage, dstImage, scaleFactorX, scaleFactorY, interpolationMode);
        }

        /// <summary>
        /// Copies the lower 8 bits of this image (color is not supported) to a 8 bits target image of same resolution.
        /// Bits higher than 8 are discarded.
        /// If target is null, a new 8 bits image will be created.
        /// </summary>
        public void CropTo8bits(ref MilImage target)
        {
            if (target == null)
            {
                target = new MilImage();
                target.Alloc2d(SizeX, SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_GRAB + MIL.M_NON_PAGED);
            }

            Int64 sourceDepth_byte = PixelDepth_byte;

            Lock(MIL.M_READ);
            target.Lock(MIL.M_READ);
            unsafe
            {
                byte* sourceLineStart_p = (byte*)HostAddress;
                byte* targetLineStart_p = (byte*)(target.HostAddress);

                for (Int32 y = 0; y < SizeY; y++)
                {
                    byte* source_p = sourceLineStart_p;
                    byte* target_p = targetLineStart_p;

                    for (Int32 x = 0; x < SizeX; x++)
                    {
                        *(target_p++) = *source_p;
                        source_p += sourceDepth_byte;
                    }

                    sourceLineStart_p += Pitch;
                    targetLineStart_p += target.Pitch;
                }
            }
            Unlock();
            target.Unlock();
        }

        //=================================================================
        //
        //=================================================================
        public void Convert(MIL_ID arrayBufIdOrConversionType)
        {
            MIL.MimConvert(MilId, MilId, arrayBufIdOrConversionType);
        }

        public static void Convert(MilImage srcImage, MilImage dstImage, MIL_ID arrayBufIdOrConversionType)
        {
            MIL.MimConvert(srcImage, dstImage, arrayBufIdOrConversionType);
        }

        //=================================================================
        //
        //=================================================================
        public static void Rotate(MilImage src, MilImage dest, double angle, double srcCenX, double srcCenY, double dstCenX, double dstCenY, MIL_INT interpolationMode)
        {
            MIL.MimRotate(src.MilId, dest.MilId, angle, srcCenX, srcCenY, dstCenX, dstCenY, interpolationMode);
            checkMilError("Failed to rotate image");
        }

        //=================================================================
        //
        //=================================================================
        public static void Flip(MilImage src, MilImage dest, long operation, long opFlag = MIL.M_DEFAULT)
        {
            MIL.MimFlip(src.MilId, dest.MilId, operation, opFlag);
            checkMilError("Failed to flip image");
        }

        public void Flip(long operation, long opFlag = MIL.M_DEFAULT)
        {
            MIL.MimFlip(MilId, MilId, operation, opFlag);
            checkMilError("Failed to flip image");
        }

        //=================================================================
        //
        //=================================================================
        public void SetRegion(MIL_ID imageOrGraphicListId, long label, long operation, double Param = MIL.M_DEFAULT)
        {
            MIL.MbufSetRegion(MilId, imageOrGraphicListId, label, operation, Param);
            checkMilError("Failed to set region");
        }

        //=================================================================
        // Get/Put pixel value
        //=================================================================
        public double GetPixel(int offX, int offY)
        {
            if ((Type & 0xFF) == 32)
            {
                float[] val = new float[1];
                Get2d(offX, offY, 1, 1, val);
                return val[0];
            }
            else if ((Type & 0xFF) == 16)
            {
                ushort[] val = new ushort[1];
                Get2d(offX, offY, 1, 1, val);
                return val[0];
            }
            else
            {
                byte[] val = new byte[1];
                Get2d(offX, offY, 1, 1, val);
                return val[0];
            }
        }

        public void PutPixel(int offX, int offY, byte value)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            byte[] val = new byte[1];
            val[0] = value;
            Put2d(offX, offY, 1, 1, val);
        }

        public void PutPixel(int offX, int offY, float value)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            float[] val = new float[1];
            val[0] = value;
            Put2d(offX, offY, 1, 1, val);
        }

        public void PutHorizontalLine(int offY, Color color, int thickness)
        {
            byte[] colorArray = new byte[SizeX * thickness * 3];
            int offsetGreen = SizeX * thickness;
            int offsetBlue = SizeX * thickness * 2;
            for (int i = 0; i < SizeX * thickness; i++)
            {
                colorArray[i] = color.R;
                colorArray[offsetGreen + i] = color.G;
                colorArray[offsetBlue + i] = color.B;
            }
            MIL.MbufPutColor2d(_milId, MIL.M_PLANAR, MIL.M_ALL_BANDS, 0, offY, SizeX, thickness, colorArray);
            checkMilError("MbufPutColor2d");
        }

        public void PutVerticalLine(int offX, Color color, int thickness)
        {
            byte[] colorArray = new byte[SizeY * thickness * 3];
            int offsetGreen = SizeY * thickness;
            int offsetBlue = SizeY * thickness * 2;
            for (int i = 0; i < SizeY * thickness; i++)
            {
                colorArray[i] = color.R;
                colorArray[offsetGreen + i] = color.G;
                colorArray[offsetBlue + i] = color.B;
            }
            MIL.MbufPutColor2d(_milId, MIL.M_PLANAR, MIL.M_ALL_BANDS, offX, 0, thickness, SizeY, colorArray);
            checkMilError("MbufPutColor2d");
        }
      
        public void Get2d(MIL_INT offX, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet2d(_milId, offX, offY, sizeX, sizeY, userArrayPtr);
            checkMilError("MbufGet2d");
        }

        public void Get2d(MIL_INT offX, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, ushort[] userArrayPtr)
        {
            if (SizeBit != 16)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet2d(_milId, offX, offY, sizeX, sizeY, userArrayPtr);
            checkMilError("MbufGet2d");
        }

        public void Get2d(MIL_INT offX, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, float[] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet2d(_milId, offX, offY, sizeX, sizeY, userArrayPtr);
            checkMilError("MbufGet2d");
        }
  
        public void Get2d(MIL_INT offX, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, byte[,] userArrayPtr2d)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet2d(_milId, offX, offY, sizeX, sizeY, userArrayPtr2d);
            checkMilError("MbufGet2d");
        }

        public void Get2d(Rect rect, byte[,] userArrayPtr2d)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            Get2d((long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height, userArrayPtr2d);
        }

        public void Get(byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(UInt16[] userArrayPtr)
        {
            if (SizeBit != 16)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(byte[,] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(uint[] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(float[] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(float[,] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void GetColor(long dataFormat, MIL_INT band, byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGetColor(_milId, dataFormat, band, userArrayPtr);
            checkMilError("MbufGetColor");
        }

        public void GetColor(long dataFormat, MIL_INT band, byte[,] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufGetColor(_milId, dataFormat, band, userArrayPtr);
            checkMilError("MbufGetColor");
        }

        public void Put2d(MIL_INT offx, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut2d(_milId, offx, offY, sizeX, sizeY, userArrayPtr);
            checkMilError("MbufPut2d");
        }

        public void Put2d(MIL_INT offx, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, float[] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut2d(_milId, offx, offY, sizeX, sizeY, userArrayPtr);
            checkMilError("MbufPut2d");
        }

        public void Put2d(MIL_INT offx, MIL_INT offY, MIL_INT sizeX, MIL_INT sizeY, byte[,] userArrayPtr2d)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);
            MIL.MbufPut2d(_milId, offx, offY, sizeX, sizeY, userArrayPtr2d);
            checkMilError("MbufPut2d");
        }

        public void Put2d(Rect rect, byte[,] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            Put2d((long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height, userArrayPtr);
        }

        public void Put(byte[,] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(UInt16[] userArrayPtr)
        {
            if (SizeBit != 16)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(uint[,] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(float[,] userArrayPtr)
        {
            if (SizeBit != 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(float[] userArrayPtr)
        {
            if (SizeBit > 32)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void PutColor(long dataFormat, MIL_INT band, byte[] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPutColor(_milId, dataFormat, band, userArrayPtr);
            checkMilError("MbufPutColor");
        }

        public void PutColor(long dataFormat, MIL_INT band, byte[,] userArrayPtr)
        {
            if (SizeBit > 8)
                throw new ApplicationException("Invalid image depth: " + SizeBit);

            MIL.MbufPutColor(_milId, dataFormat, band, userArrayPtr);
            checkMilError("MbufPutColor");
        }

        //=================================================================
        // Save image to disk
        //=================================================================
        public void Save(string filename)
        {
            MIL.MbufSave(filename, MilId);
            checkMilError("Failed to save image \"" + filename + "\"");
        }

        public void Export(string filename, MIL_INT fileFormat)
        {
            MIL.MbufExport(filename, fileFormat, MilId);
            checkMilError("Failed to save (export) image \"" + filename + "\"");
        }

        //=================================================================
        // Load image from disk
        //=================================================================
        public void Restore(string filename)
        {
            Restore(filename, Mil.Instance.HostSystem);
        }

        public void Restore(string filename, MIL_ID systemId)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");
            MIL.MbufRestore(filename, systemId, ref _milId);
            checkMilError("Failed to load image \"" + filename + "\"");

            OwnImage = true;
            IsChildImage = false;
            Interlocked.Increment(ref NbImages);
            Interlocked.Add(ref InternalImageMemory, SizeByte);
            GC.AddMemoryPressure(SizeByte);
        }

        //=================================================================
        // Get file format from the filename extension
        //=================================================================
        public static MIL_INT GetFileFormat(string filename)
        {
            filename = filename.ToLower();

            MIL_INT format;
            if (filename.EndsWith("bmp"))
                format = MIL.M_BMP;
            else if (filename.EndsWith("jpg") || filename.EndsWith("jpeg"))
                format = MIL.M_JPEG_LOSSY;
            else if (filename.EndsWith("jpgls") || filename.EndsWith("jpegls"))
                format = MIL.M_JPEG_LOSSLESS;
            else if (filename.EndsWith("tif") || filename.EndsWith("tiff"))
                format = MIL.M_TIFF;
            else
                format = MIL.M_MIL;

            return format;
        }

        //=================================================================
        // Load from memory
        //=================================================================
        public long Stream(byte[] memPtrOrFileName, MIL_ID sysId, long operation, long streamType, double version = MIL.M_DEFAULT, long controlFlag = MIL.M_DEFAULT)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            MIL_INT size = 0;
            MIL.MimStream(memPtrOrFileName, sysId, operation, streamType, version, controlFlag, ref _milId, ref size);
            checkMilError("Failed to stream image");
            return size;
        }

        #region ImageJ

        private static int s_fdcount = 0;

        //=================================================================
        // Debug function to view a MIL image
        //=================================================================
        public int ImageJ()
        {
            return ImageJ(_milId);
        }

        public static int ImageJ(MIL_ID id)
        {
            // Save image
            //...........
            int idx = s_fdcount++;
            string filename = PathString.GetTempPath() / "fdebug-" + idx + ".tif";
            MIL.MbufSave(filename, id);

            // Start ImageJ
            //.............
            UnitySC.Shared.Tools.Extension.ImageJ(filename);

            return idx;
        }

        //=================================================================
        // Debug function to view a raw image
        //=================================================================
        public static int ImageJ(byte[] buf, int w, int h)
        {
            // Check that width/height match the buffer size
            //..............................................
            if (buf.Length != w * h)
                return -1;

            // Save buffer
            //............
            int idx = s_fdcount++;
            string filename = PathString.GetTempPath() / "fdebug-" + idx + ".raw";
            System.IO.File.WriteAllBytes(filename, buf);

            // Start ImageJ
            //.............
            string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
            if (viewer == null)
                viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

            string macro = "-eval \"run(\\\"Raw...\\\", \\\"open=c:\\\\temp\\\\fdebug-" + idx + ".raw image=8-bit width=" + w + " height=" + h + " offset=0 little-endian\\\");\"";
            System.Diagnostics.Process.Start(viewer, macro);
            return idx;
        }

        public static int ImageJ(byte[,] buf)
        {
            byte[] buf2 = new byte[buf.Length];

            int idx = 0;
            for (int i = 0; i < buf.GetLength(0); i++)
            {
                for (int j = 0; j < buf.GetLength(1); j++)
                    buf2[idx++] = buf[i, j];
            }

            return ImageJ(buf2, buf.GetLength(1), buf.GetLength(0));
        }

        public static int ImageJ(float[,] buf)
        {
            byte[] buf2 = new byte[buf.Length * sizeof(float)];
            Buffer.BlockCopy(buf, 0, buf2, 0, buf2.Length);
            int h = buf.GetLength(0);
            int w = buf.GetLength(1);

            // Save buffer
            //............
            int idx = s_fdcount++;
            string filename = PathString.GetTempPath() / "fdebug-" + idx + ".raw";
            System.IO.File.WriteAllBytes(filename, buf2);

            // Start ImageJ
            //.............
            string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
            if (viewer == null)
                viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

            string macro = "-eval \"run(\\\"Raw...\\\", \\\"open=c:\\\\temp\\\\fdebug-" + idx + ".raw image=[32-bit Real] width=" + w + " height=" + h + " offset=0 little-endian\\\");\"";
            System.Diagnostics.Process.Start(viewer, macro);
            return idx;
        }

        #endregion ImageJ

        #region image processing

        //=================================================================
        //  Image Processing
        //=================================================================
        public static void Convolve(MilImage srcImage, MilImage destImage, MIL_ID kernelId)
        {
            MIL.MimConvolve(srcImage._milId, destImage._milId, kernelId);
            checkMilError("MimConvolve");
        }

        //=================================================================
        public void Convolve(MIL_ID kernelId)
        {
            MIL.MimConvolve(_milId, _milId, kernelId);
            checkMilError("MimConvolve");
        }

        //=================================================================
        public void Equalization(MIL_ID operation, double lfAlpha, double lfMinValue, double lfMaxValue)
        {
            MIL.MimHistogramEqualize(_milId, _milId, operation, lfAlpha, lfMinValue, lfMaxValue);
            checkMilError("MimHistogramEqualize");
        }

        //=================================================================
        public static void EdgeDetect(MilImage srcImage, MilImage destImage, MilImage destAngleImage, MIL_ID kernelId, MIL_INT controlFlag, MIL_INT threshold)
        {
            MIL.MimEdgeDetect(srcImage._milId, destImage._milId, destAngleImage._milId, kernelId, controlFlag, threshold);
            checkMilError("MimEdgeDetect");
        }

        public void EdgeDetect(MIL_ID kernelId, MIL_INT controlFlag, MIL_INT threshold)
        {
            MIL.MimEdgeDetect(_milId, _milId, MIL.M_NULL, kernelId, controlFlag, threshold);
            checkMilError("MimEdgeDetect");
        }

        //=================================================================
        public static void Dilate(MilImage srcImage, MilImage destImage, MIL_INT nbIteration, MIL_ID kindOfPicture)
        {
            MIL.MimDilate(srcImage._milId, destImage._milId, nbIteration, kindOfPicture);
            checkMilError("MimDilate");
        }

        public void Dilate(MIL_INT nbIteration, MIL_ID kindOfPicture)
        {
            MIL.MimDilate(_milId, _milId, nbIteration, kindOfPicture);
            checkMilError("MimDilate");
        }

        //=================================================================
        public static void Erode(MilImage srcImage, MilImage destImage, MIL_INT nbIteration, MIL_ID kindOfPicture)
        {
            MIL.MimErode(srcImage._milId, destImage._milId, nbIteration, kindOfPicture);
            checkMilError("MimErode");
        }

        public void Erode(MIL_INT nbIteration, MIL_ID kindOfPicture)
        {
            MIL.MimErode(_milId, _milId, nbIteration, kindOfPicture);
            checkMilError("MimErode");
        }

        //=================================================================
        public static void Binarize(MilImage srcImage, MilImage destImage, MIL_INT condition, double lowParam, double highParam)
        {
            MIL.MimBinarize(srcImage._milId, destImage._milId, condition, lowParam, highParam);
            checkMilError("MimBinarize");
        }

        public void Binarize(MIL_INT condition, double lowParam, double highParam)
        {
            MIL.MimBinarize(_milId, _milId, condition, lowParam, highParam);
            checkMilError("MimBinarize");
        }

        //=================================================================
        public static void Clip(MilImage srcImage, MilImage destImage, MIL_INT condition, double condLow, double condHigh, double writeLow, double writeHigh)
        {
            MIL.MimClip(srcImage, destImage, condition, condLow, condHigh, writeLow, writeHigh);
            MilImage.checkMilError("MimClip");
        }

        public void Clip(MIL_INT condition, double condLow, double condHigh, double writeLow, double writeHigh)
        {
            Clip(this, this, condition, condLow, condHigh, writeLow, writeHigh);
        }

        //=================================================================
        public static void Close(MilImage srcImage, MilImage destImage, MIL_INT nbIteration, long kindOfPicture)
        {
            MIL.MimClose(srcImage._milId, destImage._milId, nbIteration, kindOfPicture);
            checkMilError("MimClose");
        }

        public void Close(MIL_INT nbIteration, long kindOfPicture)
        {
            MIL.MimClose(_milId, _milId, nbIteration, kindOfPicture);
            checkMilError("MimClose");
        }

        //=================================================================
        public static void ArithMultiple(MilImage src1Image, double src2ImageBufIdOrConst, double src3ImageBufIdOrConst, double src4Const, double src5Const, MilImage dstImage, long operation, long operationFlag)
        {
            MIL.MimArithMultiple(src1Image._milId, src2ImageBufIdOrConst, src3ImageBufIdOrConst, src4Const, src5Const, dstImage._milId, operation, operationFlag);
            checkMilError("MimArithMultiple");
        }

        //=================================================================
        public static void ArithMultiple(MilImage src1Image, double src2ImageBufIdOrConst, MilImage src3Image, double src4Const, double src5Const, MilImage dstImage, long operation, long operationFlag)
        {
            MIL.MimArithMultiple(src1Image._milId, src2ImageBufIdOrConst, src3Image._milId, src4Const, src5Const, dstImage._milId, operation, operationFlag);
            checkMilError("MimArithMultiple");
        }

        //=================================================================
        public void Arith(double @const, long operation)
        {
            MIL.MimArith(_milId, @const, _milId, operation);
            checkMilError("MimArith");
        }

        public void Arith(long operation)
        {
            MIL.MimArith(_milId, MIL.M_NULL, _milId, operation);
            checkMilError("MimArith");
        }

        public static void Arith(MilImage src1Image, double src2ImageBufIdOrConst, MilImage dstImage, long operation)
        {
            MIL.MimArith(src1Image._milId, src2ImageBufIdOrConst, dstImage._milId, operation);
            checkMilError("MimArith");
        }

        public static void Arith(MilImage src1Image, MilImage src2Image, MilImage dstImage, long operation)
        {
            MIL.MimArith(src1Image._milId, src2Image._milId, dstImage._milId, operation);
            checkMilError("MimArith");
        }

        //=================================================================
        public static void Rank(MilImage srcImage, MilImage dstImage, MilImage structElem, MIL_INT rank, long procMode)
        {
            MIL.MimRank(srcImage.MilId, dstImage.MilId, structElem.MilId, rank, procMode);
        }

        public void Rank(MilImage structElem, MIL_INT rank, long procMode)
        {
            MIL.MimRank(MilId, MilId, structElem.MilId, rank, procMode);
        }

        //=================================================================
        public void LutMap(MilBuffer1d lutBufId)
        {
            LutMap(this, this, lutBufId);
        }

        public static void LutMap(MilImage srcImageBufId, MilImage dstImageBufId, MIL_ID lutBufId)
        {
            MIL.MimLutMap(srcImageBufId, dstImageBufId, lutBufId);
            MilImage.checkMilError("MimLutMap");
        }

        //=================================================================
        public static void Projection(MilImage srcImage, MilImage dstImage, double projectionAxisAngle, Int64 operation)
        {
            Projection(srcImage, dstImage, projectionAxisAngle, operation, MIL.M_NULL);
        }

        public static void Projection(MilImage srcImage, MilImage dstImage, double projectionAxisAngle, Int64 operation, double operationValue)
        {
            MIL.MimProjection(srcImage, dstImage, projectionAxisAngle, operation, operationValue);
        }

        #endregion image processing

        //=================================================================
        // Conversion en Bitmap C#
        //=================================================================
        public System.Drawing.Bitmap ConvertToBitmap()
        {
            if (_milId == 0)
                return null;

            System.Drawing.Bitmap bitmap = null;
            System.Drawing.Imaging.BitmapData data = null;

            try
            {
                //---------------------------------------------------------
                // Get MilImage
                //---------------------------------------------------------
                int width = SizeX;
                int height = SizeY;
                int pitch = Pitch;

                // Lock source image
                //..................
                Lock(MIL.M_READ);
                long pSrc = HostAddress;

                //-----------------------------------------------------
                // Creation Bitmap
                //-----------------------------------------------------
                System.Drawing.Imaging.PixelFormat pixelFormat;
                switch (SizeBit)
                {
                    case 8: pixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed; break;
                    case 16: pixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale; break;
                    default: throw new ApplicationException("unsupported bitmap format, depth=" + SizeBit);
                }
                bitmap = new System.Drawing.Bitmap(width, height, pixelFormat);

                //-----------------------------------------------------
                // Création de la Palette
                //-----------------------------------------------------
                if (pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    var palette = bitmap.Palette;
                    System.Drawing.Color[] entries = palette.Entries;
                    for (int i = 0; i < entries.Length; i++)
                        entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                    bitmap.Palette = palette;
                }

                //-----------------------------------------------------
                // Copie des pixels
                //-----------------------------------------------------
                data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
                               System.Drawing.Imaging.ImageLockMode.WriteOnly,
                               bitmap.PixelFormat);

                long pDest = data.Scan0.ToInt64();
                long length = width * SizeBit / 8;
                for (int i = 0; i < height; i++)
                {
                    Extension.NativeMethods.CopyMemory(new IntPtr(pDest), new IntPtr(pSrc), length);
                    pSrc += pitch;
                    pDest += data.Stride;
                }
            }
            finally
            {
                // Unlock source image
                //....................
                if (IsLocked)
                    Unlock();
                if (data != null)
                {
                    bitmap.UnlockBits(data);
                    data = null;
                }
            }

            return bitmap;
        }

        //=================================================================
        // Convert to C# Bitmap - (Alternative RTI )
        // Les données sont copiées deux fois.
        //=================================================================
        public System.Drawing.Bitmap ConvertToBitmap2()
        {
            System.Drawing.Bitmap bitmap = null;
            var BufImgdib = MIL.M_NULL;
            try
            {
                //---------------------------------------------------------
                // Get MilImage
                //---------------------------------------------------------
                if (_milId == 0)
                    return null;

                MIL.MbufAlloc2d(OwnerSystem, (MIL_INT)SizeX, (MIL_INT)SizeY, Type, Attribute + MIL.M_DIB + MIL.M_GDI, ref BufImgdib);
                MIL.MbufCopy(_milId, BufImgdib);

                //-----------------------------------------------------
                // Creation Bitmap
                //-----------------------------------------------------
                IntPtr hbitmap = MIL.MbufInquire(BufImgdib, MIL.M_DIB_HANDLE, MIL.M_NULL);
                bitmap = System.Drawing.Bitmap.FromHbitmap(hbitmap);   // Copie aussi les données, Cf MSDN
            }
            finally
            {
                // free tmpdib buff
                if (BufImgdib != MIL.M_NULL)
                {
                    MIL.MbufFree(BufImgdib);
                    BufImgdib = MIL.M_NULL;
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Copies the image to a WriteableBitmap with the same characteritics as the MIL image.
        /// </summary>
        public async Task CopyToAsync(WriteableBitmap imageForDisplay)
        {
            long size_bytes = SizeByte;

            // A WriteableBitmap can only be accessed from its owner's thread (which will often not be the thread manipulating a MillImage).
            await imageForDisplay.Dispatcher;

            Lock(MIL.M_READ);
            int bufferSize = Pitch * SizeY;
            imageForDisplay.WritePixels(new Int32Rect(0, 0, SizeX, SizeY), new IntPtr(HostAddress), bufferSize, Pitch);
            Unlock();
        }

        //=================================================================
        // Conversion to WPF BitmapSource
        //=================================================================
        public System.Windows.Media.Imaging.BitmapSource ConvertToWpfBitmapSource()
        {
            System.Windows.Media.Imaging.WriteableBitmap wpfBitmap;
            if (_milId == 0)
                return null;

            //-------------------------------------------------------------
            // Image couleur
            //-------------------------------------------------------------
            if (SizeBand == 3 || SizeBand == 4)
            {
                // Récupère les pixels de MIL
                //...........................
                int bufferSize = SizeX * SizeY * SizeBand;
                byte[] buffer = new byte[bufferSize];

                int format = SizeBand == 3 ? MIL.M_BGR24 : MIL.M_BGR32;
                GetColor(format + MIL.M_PACKED, MIL.M_ALL_BANDS, buffer);

                // Création de la bitmap WPF
                //..........................
                System.Windows.Media.PixelFormat bgr = SizeBand == 3 ? System.Windows.Media.PixelFormats.Bgr24 : System.Windows.Media.PixelFormats.Bgr32;
                wpfBitmap = new System.Windows.Media.Imaging.WriteableBitmap(SizeX, SizeY, 96, 96, bgr, null);

                int stride = SizeX * SizeBand;
                wpfBitmap.WritePixels(new Int32Rect(0, 0, SizeX, SizeY), buffer, stride, 0);
            }
            //-------------------------------------------------------------
            // Image en niveaux de gris
            //-------------------------------------------------------------
            else if (SizeBand == 1)
            {
                try
                {
                    // Lock source image
                    //..................
                    Lock(MIL.M_READ);
                    long pSrc = HostAddress;

                    // Création de la WriteableBitmap
                    //...............................
                    System.Windows.Media.PixelFormat format;
                    if (Type == 32 + MIL.M_FLOAT)
                        format = System.Windows.Media.PixelFormats.Gray32Float;
                    else if (SizeBit == 16)
                        format = System.Windows.Media.PixelFormats.Gray16;
                    else if (SizeBit == 8)
                        format = System.Windows.Media.PixelFormats.Gray8;
                    else
                        throw new ApplicationException($"unknown image type: depth={SizeBit} type={Type}");

                    wpfBitmap = new System.Windows.Media.Imaging.WriteableBitmap(SizeX, SizeY, 96, 96, format, null);

                    // Copie des pixels
                    //.................
                    int bufferSize = Pitch * SizeY;
                    wpfBitmap.WritePixels(new Int32Rect(0, 0, SizeX, SizeY), new IntPtr(pSrc), bufferSize, Pitch);
                }
                finally
                {
                    // Unlock source image
                    //....................
                    if (IsLocked)
                        Unlock();
                }
            }
            //-------------------------------------------------------------
            // Les autres types d'images ne sont pas supportés
            //-------------------------------------------------------------
            else
            {
                throw new ApplicationException($"unsupported image type, SizeBand={SizeBand}");
            }

            return wpfBitmap;
        }
    }
}
