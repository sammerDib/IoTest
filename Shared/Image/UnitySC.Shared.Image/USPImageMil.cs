using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Image
{
    public class USPImageMil : DisposableObject, ICameraImage
    {
        public bool IsMilSimulated = ClassLocator.Default.GetInstance<IServiceConfigurationManager>().MilIsSimulated;
        protected UspImageType Type = UspImageType.Mil;

        protected MilImage _milImage;
        protected CSharpImage _csharpImage;
        protected BitmapSource Src;

        public int Width
        {
            get
            {
                if (IsMilSimulated)
                {
                    return _csharpImage?.width ?? 100;
                }
                return _milImage.SizeX;
            }
        }

        public int Height
        {
            get
            {
                if (IsMilSimulated)
                {
                    return _csharpImage?.height ?? 100;
                }
                return _milImage.SizeY;
            }
        }

        public DateTime Timestamp { get; set; }

        public ImageFormat Format
        {
            get
            {
                if (IsMilSimulated)
                {
                    return ImageFormat.GreyLevel;
                }
                if (_milImage?.Type == MIL.M_FLOAT + 32)
                    return ImageFormat.Height3D;
                else
                    return ImageFormat.GreyLevel;
            }
        }

        public USPImageMil()
        {
            Timestamp = DateTime.UtcNow;
        }

        public USPImageMil(DateTime timestamp)
        {
            Timestamp = timestamp;
        }

        public USPImageMil(string imgPath)
        {
            if (IsMilSimulated)
            {
                return;
            }

            Load(imgPath);
        }

        public USPImageMil(Bitmap bitmap)
        {
            if (IsMilSimulated)
            {
                _milImage = new MilImage();
                return;
            }

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            Src = BitmapSource.Create(
            bitmapData.Width, bitmapData.Height,
            bitmap.HorizontalResolution, bitmap.VerticalResolution,
            PixelFormats.Gray8, null,
            bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            int bytesPerPixel;
            int imageType;

            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    bytesPerPixel = 1;
                    imageType = MIL.M_UNSIGNED + 8;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                    bytesPerPixel = 2;
                    imageType = 16 + MIL.M_UNSIGNED;
                    break;

                case System.Drawing.Imaging.PixelFormat.Canonical:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    bytesPerPixel = 4;
                    imageType = 32 + MIL.M_UNSIGNED;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    bytesPerPixel = 8;
                    imageType = 32 + MIL.M_FLOAT;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                default:
                    throw new ArgumentException("The bitmap has an invalid format, cannot convert to USPImageMil");
            }

            var byteArray = new byte[bitmap.Width * bitmap.Height * bytesPerPixel];
            Src.CopyPixels(byteArray, bitmapData.Stride, 0);

            _milImage = new MilImage();
            _milImage.Alloc2d(bitmap.Width, bitmap.Height, imageType, MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC + MIL.M_UNSIGNED);
            _milImage.Put(byteArray);

            bitmap.UnlockBits(bitmapData);
        }

        public USPImageMil(ServiceImage serviceImage)
        {
            Timestamp = DateTime.UtcNow;
            _milImage = new MilImage();

            if (IsMilSimulated)
            {
                _csharpImage = new CSharpImage()
                {
                    width = serviceImage.DataWidth,
                    height = serviceImage.DataHeight,
                    depth = serviceImage.Depth
                };

                if (serviceImage.Depth != 8)
                {
                    return;
                }

                byte[,] array2d = new byte[serviceImage.DataWidth, serviceImage.DataHeight];
                for (int y = 0; y < serviceImage.DataHeight; y++)
                {
                    for (int x = 0; x < serviceImage.DataWidth; x++)
                    {
                        array2d[x, y] = serviceImage.Data[x + y * serviceImage.DataWidth];
                    }
                }
                
                PutCSharpArray(array2d);
                
                return;
            }

            int type = serviceImage.Depth;
            int width = serviceImage.DataWidth;
            int height = serviceImage.DataHeight;
            int imageType = serviceImage.Depth;
            int attributes = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC;

            switch (serviceImage.Type)
            {
                case ServiceImage.ImageType.Greyscale:
                    imageType += MIL.M_UNSIGNED;
                    _milImage.Alloc2d(width, height, imageType, attributes);
                    break;

                case ServiceImage.ImageType.Greyscale16Bit:
                    imageType += 16 + MIL.M_UNSIGNED;
                    _milImage.Alloc2d(width, height, imageType, attributes);
                    break;

                case ServiceImage.ImageType.RGB:
                    imageType = 8 + MIL.M_UNSIGNED;
                    const int channels = 3;
                    _milImage.AllocColor(channels, width, height, imageType, attributes);
                    break;

                case ServiceImage.ImageType._3DA:
                    imageType = MIL.M_FLOAT + 32; // forcement 32
                    _milImage.Alloc2d(width, height, imageType, attributes);
                    break;

                default:
                    throw new ApplicationException("unknown image type");
            }

            if (_milImage.SizeBand == 1)
            {
                if (serviceImage.Type != ServiceImage.ImageType._3DA)
                    MIL.MbufPut(_milImage.MilId, serviceImage.Data);
                else
                {
                    using (var format3da_R = new MatrixFloatFile())
                    {
                        // TODO : A implémenter plus tard - voir RTI
                        // si on execede un buffer de plus de 4go ou 2go on peut avoir un pb d'allocation de mémoire. (cas bf2d ou 3d)
                        // 2 manières de completer le buffer :
                        // - on peut aggreger car inferieur à la taille
                        // - on insere dans le buffer mil chunk par chunk
                        var status = format3da_R.ReadChunksFromMemory(serviceImage.Data, 0);
                        var matrix = MatrixFloatFile.AggregateChunks(status, format3da_R);
                        MIL.MbufPut(_milImage.MilId, matrix);
                    }
                }
            }
            else
            {
                _milImage.PutColor(MIL.M_PACKED + MIL.M_RGB24, MIL.M_ALL_BANDS, serviceImage.Data);
            }
        }

        public USPImageMil(byte[,] data, int sizeX, int sizeY, int type, int attribute)
        {
            Timestamp = DateTime.UtcNow;
            _milImage = new MilImage();

            if (IsMilSimulated)
            {
                return;
            }

            _milImage.Alloc2d(sizeX, sizeY, type, attribute);
            _milImage.Put(data);
        }

        public USPImageMil(byte[] data, int sizeX, int sizeY, int type, int attribute)
        {
            Timestamp = DateTime.UtcNow;
            _milImage = new MilImage();

            if (IsMilSimulated)
            {
                return;
            }

            _milImage.Alloc2d(sizeX, sizeY, type, attribute);
            _milImage.Put(data);
        }

        public USPImageMil(float[] data, int sizeX, int sizeY, int type, int attribute)
        {
            Timestamp = DateTime.UtcNow;
            _milImage = new MilImage();

            if (IsMilSimulated)
            {
                return;
            }

            _milImage.Alloc2d(sizeX, sizeY, type, attribute);
            _milImage.Put(data);
        }

        public USPImageMil(int sizeX, int sizeY, int type, int attribute)
        {
            Timestamp = DateTime.UtcNow;
            _milImage = new MilImage();

            if (IsMilSimulated)
            {
                return;
            }

            _milImage.Alloc2d(Mil.Instance.HostSystem, sizeX, sizeY, type, attribute);
        }

        public USPImageMil Clone()
        {
            if (IsMilSimulated)
            {
                return new USPImageMil();
            }

            var procimg = new USPImageMil(Timestamp);
            var milImage = new MilImage();
            var milGrabImage = GetMilImage();
            milImage.Alloc2d(milGrabImage.SizeX, milGrabImage.SizeY, milGrabImage.Type, MIL.M_IMAGE + MIL.M_PROC);
            MilImage.Copy(milGrabImage, milImage);
            procimg.SetMilImage(milImage);
            return procimg;
        }

        public void Clear(double color = 0)
        {
            _milImage.Clear(color);
        }

        /// <summary>
        /// Load / Save
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            if (IsMilSimulated)
            {
                return;
            }

            if (null == _milImage)
                _milImage = new MilImage();

            var milImage = GetMilImage();

            milImage.Restore(filename);

            var lut = milImage.AssociatedLut;
            if (lut != MIL.M_DEFAULT)
            {
                using (var milImageWithoutLut = new MilImage())
                {
                    milImageWithoutLut.AllocCompatibleWith(milImage);
                    MilImage.LutMap(milImage, milImageWithoutLut, lut);
                    SetMilImage(milImageWithoutLut);
                }
            }
        }

        public byte[] ToByteArray()
        {
            var milImage = GetMilImage();
            if (milImage.MilId == MIL.M_NULL)
                return null;
            var datalen = milImage.SizeX * milImage.SizeY * milImage.SizeBand * (milImage.SizeBit / 8);
            var byteArray = new byte[datalen];
            MIL.MbufGet(milImage.MilId, byteArray);
            return byteArray;
        }

        public ServiceImage ToServiceImage()
        {
            if (IsMilSimulated)
            {
                var svcImg = new ServiceImage();
                GenerateDummyServiceImage(svcImg);
                return svcImg;
            }

            var serviceImage = new ServiceImage();
            var milImage = GetMilImage();
            if (milImage.MilId == MIL.M_NULL)
                return null;

            serviceImage.DataWidth = milImage.SizeX;
            serviceImage.DataHeight = milImage.SizeY;
            if (milImage.SizeBit == 32)
            {
                serviceImage.Type = ServiceImage.ImageType._3DA;

                bool useCompression = true;
                float[] matrix = new float[serviceImage.DataWidth * serviceImage.DataHeight];
                MIL.MbufGet(milImage.MilId, matrix);
                var mil3DAHeader = new MatrixFloatFile.HeaderData(serviceImage.DataHeight, serviceImage.DataWidth);
                //, new MatrixFloatFile.AdditionnalHeaderData( 1.0, 1.0, "px", "px ? um ? ", "um ? ppm ?")); // ou trouver ces infos ?
                using (var format3da_W = new MatrixFloatFile())
                {
                    serviceImage.Data = format3da_W.WriteInMemory(mil3DAHeader, matrix, useCompression);
                }
            }
            else
            {
                serviceImage.Type = GetImageType(milImage.SizeBit, milImage.SizeBand);
                int datalen = milImage.SizeX * milImage.SizeY * milImage.SizeBand * (milImage.SizeBit / 8);
                serviceImage.Data = new byte[datalen];

                switch (milImage.SizeBit)
                {
                    case 8 when milImage.SizeBand == 1:
                        milImage.Get(serviceImage.Data);
                        break;

                    case 16 when milImage.SizeBand == 1:
                        var data16 = new UInt16[datalen / 2];
                        milImage.Get(data16);
                        Buffer.BlockCopy(data16, 0, serviceImage.Data, 0, datalen);
                        break;

                    default:
                        milImage.GetColor(MIL.M_PACKED + MIL.M_RGB24, MIL.M_ALL_BANDS, serviceImage.Data);
                        break;
                }
            }

            return serviceImage;
        }

        private ServiceImage.ImageType GetImageType(int sizeBit, int sizeBand)
        {
            switch (sizeBit)
            {
                case 32:
                    return ServiceImage.ImageType._3DA; // Note de RTI : ça va pas marcher là !!! :/
                case 8 when sizeBand == 1:
                    return ServiceImage.ImageType.Greyscale;

                case 16 when sizeBand == 1:
                    return ServiceImage.ImageType.Greyscale16Bit;

                default:
                    switch (sizeBand)
                    {
                        case 3:
                            return ServiceImage.ImageType.RGB;

                        default:
                            throw new ArgumentException("No matching image type found.");
                    }
            }
        }

        public ServiceImage ToServiceImage(Int32Rect roi, double scale = 1)
        {
            if (IsMilSimulated)
            {
                var svcImg = new ServiceImageWithStatistics();
                GenerateDummyServiceImage(svcImg);
                return svcImg;
            }

            ServiceImageWithStatistics svcimg = new ServiceImageWithStatistics();
            MilImage milCamImage = GetMilImage();
            if (milCamImage.MilId == MIL.M_NULL)
                return null;

            if (roi == Int32Rect.Empty)
                roi = new Int32Rect() { Width = (int)Width, Height = (int)Height };

            // Copie l'image dans le format WCF
            //.................................
            using (MilImage milSvcImage = new MilImage())
            {
                // Resize de l'image si nécessaire
                //................................
                if (scale == 1)
                {
                    milSvcImage.Child2d(milCamImage, roi.X, roi.Y, roi.Width, roi.Height);
                }
                else
                {
                    long destSizeX = (long)(roi.Width * scale);
                    long destSizeY = (long)(roi.Height * scale);
                    milSvcImage.Alloc2d(milCamImage.OwnerSystem, destSizeX, destSizeY, milCamImage.Type, milCamImage.Attribute);
                    MilImage.Transfer(milCamImage, milSvcImage,
                        roi.X, roi.Y, roi.Width, roi.Height, MIL.M_DEFAULT,    // Source
                        0, 0, destSizeX, destSizeY, MIL.M_DEFAULT,    // Destination
                        MIL.M_COPY + MIL.M_SCALE, MIL.M_DEFAULT, MIL.M_NULL, MIL.M_NULL);
                }

                // Copie l'image dans le format WCF
                //.................................
                svcimg.OriginalWidth = milCamImage.SizeX;
                svcimg.OriginalHeight = milCamImage.SizeY;
                svcimg.DataWidth = milSvcImage.SizeX;
                svcimg.DataHeight = milSvcImage.SizeY;
                svcimg.AcquisitionRoi = roi;
                svcimg.Scale = scale;
                svcimg.Type = GetImageType(milSvcImage.SizeBit, milSvcImage.SizeBand);
                int datalen = milSvcImage.SizeX * milSvcImage.SizeY * milSvcImage.SizeBand * (milSvcImage.SizeBit / 8);
                svcimg.Data = new byte[datalen];
                switch (milSvcImage.SizeBit)
                {
                    case 8 when milSvcImage.SizeBand == 1:
                        milSvcImage.Get(svcimg.Data);
                        break;

                    case 16 when milSvcImage.SizeBand == 1:
                        var data16 = new UInt16[datalen / 2];
                        milSvcImage.Get(data16);
                        Buffer.BlockCopy(data16, 0, svcimg.Data, 0, datalen);
                        break;

                    default:
                        milSvcImage.GetColor(MIL.M_PACKED + MIL.M_RGB24, MIL.M_ALL_BANDS, svcimg.Data);
                        break;
                }
            }
            return svcimg;
        }

        /// <summary>
        /// Convert to WPF BitmapSource
        /// </summary>
        /// <returns></returns>
        public BitmapSource ConvertToWpfBitmapSource()
        {
            if (IsMilSimulated)
            {
                var svcImg = new ServiceImage();
                GenerateDummyServiceImage(svcImg);
                return svcImg.WpfBitmapSource;
            }

            Src = GetMilImage()?.ConvertToWpfBitmapSource();
            return Src;
        }

        /// <summary>
        /// Conversion en Bitmap C#
        /// </summary>
        /// <returns></returns>
        public Bitmap ConvertToBitmap()
        {
            if (IsMilSimulated)
            {
                var svcImg = new ServiceImage();
                GenerateDummyServiceImage(svcImg);

                var bmp = new Bitmap(svcImg.DataWidth, svcImg.DataHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                Marshal.Copy(svcImg.Data, 0, bmpData.Scan0, svcImg.Data.Length);
                return bmp;
            }

            return GetMilImage()?.ConvertToBitmap();
        }

        public ExternalImage ToExternalImage()
        {
            return ToServiceImage().ToExternalImage();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _milImage?.Dispose();
                _milImage = null;
                base.Dispose(disposing);
            }
#if DEBUG
            catch (Exception e)
#else
            catch (Exception)
#endif
            {
#if DEBUG
                Console.WriteLine($"Dispose USPImageMil : {e.Message}");
                Console.WriteLine($"Dispose USPImageMil : {e.StackTrace}");
#endif
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void CloneTo(DisposableObject obj)
        {
            if (IsMilSimulated)
            {
                return;
            }

            var clone = (USPImageMil)obj;
            clone._milImage = (MilImage)GetMilImage().DeepClone();
        }

        public void SetMilImage(MilImage img)
        {
            if (IsMilSimulated)
            {
                return;
            }

            _milImage?.Dispose();

            Type = UspImageType.Mil;
            _milImage = img;
        }

        /// <summary>
        /// Copy an array to the image data
        /// </summary>
        public void PutCSharpArray(byte[,] data)
        {
            if (IsMilSimulated)
            {
                _csharpImage = new CSharpImage(data, 8);
                return;
            }

            if ((data.GetLength(0) != Width) || (data.GetLength(1) != Height))
                throw new ApplicationException("can't change image size");

            var milImage = GetMilImage();
            Type = UspImageType.Mil;
            milImage.Put(data);
        }

        /// <summary>
        /// Copy an array to the image data
        /// </summary>
        public void PutCSharpArray(byte[] data)
        {
            if (IsMilSimulated)
            {
                return;
            }

            var milImage = GetMilImage();
            Type = UspImageType.Mil;
            milImage.Put(data);
        }

        public MilImage GetMilImage()
        {
            if (IsMilSimulated)
            {
                return new MilImage();
            }

            if (null == _milImage)
            {
                _milImage = new MilImage();
            }

            switch (Type)
            {
                case UspImageType.Mil:
                    break;

                case UspImageType.CSharp:
                    if (!_milImage.IsLocked)
                        throw new ApplicationException("unlocked image");
                    _milImage.Unlock();
                    break;

                default:
                    throw new ApplicationException("unknown image type: " + Type);
            }

            Type = UspImageType.Mil;
            return _milImage;
        }

        public CSharpImage GetCSharpImage()
        {
            if (IsMilSimulated)
            {
                return _csharpImage ?? new CSharpImage();
            }

            if (null == _milImage)
                _milImage = new MilImage();

            // Create a CppImage header
            //.........................
            if (_csharpImage == null)
                _csharpImage = new CSharpImage();

            _csharpImage.width = _milImage.SizeX;
            _csharpImage.height = _milImage.SizeY;
            _csharpImage.pitch = _milImage.Pitch;

            switch (Type)
            {
                case UspImageType.Mil:
                    if (_milImage.IsLocked)
                        throw new ApplicationException("locked image");
                    _milImage.Lock();
                    _csharpImage.ptr = _milImage.HostAddress;
                    break;

                case UspImageType.CSharp:
                    break;

                default:
                    throw new ApplicationException("unknown image type: " + Type);
            }

            Type = UspImageType.CSharp;

            // Check depth and number of bands
            //................................
            if (_milImage.SizeBand != 1)
                throw new ApplicationException("invalid image format");

            int type = _milImage.Type;
            if (type == 8 + MIL.M_UNSIGNED)
                _csharpImage.depth = 8;
            else if (type == 1 + MIL.M_UNSIGNED)
                _csharpImage.depth = 8;
            else if (type == 16 + MIL.M_UNSIGNED)
                _csharpImage.depth = 16;
            else if (type == 32 + MIL.M_FLOAT)
                _csharpImage.depth = 32;
            else
                throw new ApplicationException("invalid image format");

            return _csharpImage;
        }

        public void Save(string filename)
        {
            if (IsMilSimulated)
            {
                return;
            }

            var milImage = GetMilImage();
            var format = MilImage.GetFileFormat(filename);
            milImage.Export(filename, format);
        }

        /// <summary>
        /// Debug
        /// </summary>
        /// <returns></returns>
        public int ImageJ()
        {
            if (IsMilSimulated)
            {
                return 0;
            }
            return GetMilImage().ImageJ();
        }

        private void GenerateDummyServiceImage(ServiceImage svcImg)
        {
            int width = 1920;
            int height = 1080;
            var period = 256;
            svcImg.DataWidth = width;
            svcImg.DataHeight = height;

            // Image change each 100ms
            double shift = (DateTime.Now.Millisecond / 100) % 10.0 / 10.0;
            double maxPixelValue = 250;
            byte[] data = new byte[width * height];
            int center = width / 2 - 1;
            for (int i = 0; i < data.Length; i++)
            {
                double f = (i / width - center) / (double)period;
                f = (f + shift) * 2 * Math.PI;
                f = maxPixelValue / 2 * (1 + Math.Cos(f));
                f = maxPixelValue * f / maxPixelValue;
                data[i] = (byte)f;
            }

            svcImg.Data = data;
        }
    }
}
