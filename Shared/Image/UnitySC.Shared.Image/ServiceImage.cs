using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Data.Extensions;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Image
{
    [DataContract]
    [KnownType(typeof(ServiceImageWithStatistics))]
    public class ServiceImage
    {
        public ServiceImage()
        {
        }

        public ServiceImage(ExternalImage externalImage)
        {
            Data = externalImage.Data.ToArray();
            DataHeight = externalImage.DataHeight;
            DataWidth = externalImage.DataWidth;
            Type = (ImageType)externalImage.Type;
        }

        public enum ImageType
        { Greyscale, RGB, _3DA, Greyscale16Bit };

        [DataMember]
        public ImageType Type;

        /// <summary>
        /// Largeur hauteur de l'image transférée.
        /// </summary>
        [DataMember]
        public int DataWidth, DataHeight;

        /// <summary>
        /// Les data de l'image proprement dites
        /// </summary>
        [DataMember]
        public byte[] Data;

        /// <summary>
        /// true if the percentage of white pixels is above the threshold defined in AlgorithmsConfiguration
        /// </summary>
        [DataMember]
        public bool IsSaturated = false;

        /// <summary>
        /// Bits par pixel
        /// </summary>
        public int Depth
        {
            get
            {
                switch (Type)
                {
                    case ImageType.Greyscale: return 8;
                    case ImageType.Greyscale16Bit: return 16;
                    case ImageType.RGB: return 24;
                    case ImageType._3DA: return 32; // NOTE de RTI : c'est le format complet que l'on enregistre et pas la matrice de float !
                    default:
                        throw new ApplicationException("unknown image format");
                }
            }
        }

        private WriteableBitmap _wpfBitmapSource;
        public BitmapSource WpfBitmapSource
        { get { if (_wpfBitmapSource == null) ConvertToWpfBitmapSource(); return _wpfBitmapSource; } }

        /// <summary>
        /// Création d'une image WPF à partir de l'aimage WCF
        /// </summary>
        public BitmapSource ConvertToWpfBitmapSource()
        {
            int sizeX = DataWidth;
            int sizeY = DataHeight;

            // Création de la WriteableBitmap
            //...............................
            var format = System.Windows.Media.PixelFormats.Gray8;
            switch (Type)
            {
                case ServiceImage.ImageType.Greyscale:
                    format = System.Windows.Media.PixelFormats.Gray8;
                    break;

                case ServiceImage.ImageType.Greyscale16Bit:
                    format = System.Windows.Media.PixelFormats.Gray16;
                    break;

                case ServiceImage.ImageType.RGB:
                    format = System.Windows.Media.PixelFormats.Rgb24;
                    break;

                case ServiceImage.ImageType._3DA:
                    //throw new NotImplementedException("ServiceImage for 3DA is not yet implemented");
                    {
                        // we are going to store that in a grayscale 8 bits image
                        // note de rti : we could later use a default 1024 or highter RBG color map like in viwer if not sufficent; but need to set a standard float colormap 
                        format = System.Windows.Media.PixelFormats.Gray8;

                        float[] floatmatrix = null;
                        using (var mff = new MatrixFloatFile(Data))
                        {
                            floatmatrix = MatrixFloatFile.AggregateChunks(mff.GetChunkStatus(), mff);
                            sizeX = mff.Header.Width;
                            sizeY = mff.Header.Height;
                        }

                        int bufferSize = sizeX * sizeY;
                        byte[] graybuffer = new byte[bufferSize];

                        _wpfBitmapSource = new WriteableBitmap(sizeX, sizeY, 96, 96, format, null);
                        // Copie des pixels
                        //.................
                        unsafe
                        {
                            var filteredbuf = floatmatrix.Where(x => !Single.IsNaN(x) && !Single.IsInfinity(x));
                            float fMin = filteredbuf.Min();
                            float fMax = filteredbuf.Max();
                            float a = 255.0f / (fMax - fMin);
                            float b = -fMin * 255.0f / (fMax - fMin);

                            fixed (byte* pGray = graybuffer)
                            {
                                fixed (float* pFloat = floatmatrix)
                                {
                                    float* ptrF = pFloat;
                                    byte* ptr = pGray;
                                    Parallel.For(0, sizeY, y =>
                                    {
                                        float* pfRow = (float*)(ptrF + (y * sizeX));
                                        byte* pRow = (byte*)ptr + (y * sizeX);
                                        for (int x = 0; x < sizeX; x++)
                                        {
                                            byte cVal = 0;
                                            if (!Single.IsNaN(pfRow[0]))
                                            {
                                                cVal = (byte)Math.Round(pfRow[0] * a + b);
                                                if (cVal < 0)
                                                    cVal = 0;
                                                else if (cVal >= 255)
                                                    cVal = 255;
                                            }
                                            pRow[0] = cVal;
                                            pfRow++;
                                            pRow++;
                                        }
                                    });

                                }
                                _wpfBitmapSource.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), new IntPtr(pGray), bufferSize, sizeX);
                            }
                        }
                        return _wpfBitmapSource;
                    }

                default:
                    throw new ApplicationException("unknown image format");
            }

            _wpfBitmapSource = new WriteableBitmap(sizeX, sizeY, 96, 96, format, null);

            // Copie des pixels
            //.................
            unsafe
            {
                int pitch = sizeX * (Depth / 8);
                int bufferSize = pitch * sizeY;

                fixed (byte* pSrc = &Data[0])
                    _wpfBitmapSource.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), new IntPtr(pSrc), bufferSize, pitch);
            }
            return _wpfBitmapSource;
        }

        public void LoadFromFile(PathString filename)
        {
            Stream imageStreamSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!imageStreamSource.CanRead)
            {
                imageStreamSource.Close();
                throw new ApplicationException("Image cannot be read: " + filename);
            }

            BitmapDecoder decoder = null;
            switch (filename.Extension.ToLower())
            {
                case ".png":
                    decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".bmp":
                    decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".tif":
                case ".tiff":
                    decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".3da":
                    {
                        imageStreamSource.Close();
                        Data = File.ReadAllBytes(filename);
                        if (MatrixFloatFile.GetSizeFromMemory(Data, out int w, out int h))
                        {
                            DataWidth = w;
                            DataHeight = h;
                        }
                    }
                    break;
                default:
                    throw new ApplicationException("unkwown file extension: " + filename.Extension);
            }

            if (decoder != null)
            {
                BitmapSource img = decoder.Frames[0];
                Data = img.ConvertToByteArray();
                Type = GetTypeFromBitmapPixelFormat(img.Format);
                DataWidth = img.PixelWidth;
                DataHeight = img.PixelHeight;

                imageStreamSource.Close();
            }
        }

        public static ImageType GetTypeFromBitmapPixelFormat(PixelFormat format)
        {
            if (format == PixelFormats.Indexed8)
            {
                return ImageType.Greyscale;
            }
            else if (format == PixelFormats.Gray8)
            {
                return ImageType.Greyscale;
            }
            else if (format == PixelFormats.Gray16)
            {
                return ImageType.Greyscale16Bit;
            }
            else if (format == PixelFormats.Rgb24 || format == PixelFormats.Bgr24)
            {
                return ImageType.RGB;
            }
            else
            {
                throw new ApplicationException("unknown image format");
            }

        }

        public void CreateFromBitmap(BitmapSource img)
        {
            Data = img.ConvertToByteArray();
            Type = GetTypeFromBitmapPixelFormat(img.Format);
            DataWidth = img.PixelWidth;
            DataHeight = img.PixelHeight;
        }

        public void SaveToFile(PathString fileName, TiffCompressOption tiffCompression = TiffCompressOption.Default)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = null;
                string ext = fileName.Extension.ToLower();
                switch (ext)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".tif":
                    case ".tiff":
                        encoder = new TiffBitmapEncoder();
                        if (tiffCompression != TiffCompressOption.Default)
                            (encoder as TiffBitmapEncoder).Compression = tiffCompression;
                        break;

                    case ".jpg":
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case ".3da":
                        {
                            fileStream.Write(Data, 0, Data.Length);
                            fileStream.Flush();
                        }
                        break;

                    case ".bmp":
                    default:
                        encoder = new BmpBitmapEncoder();
                        break;
                }
                if (encoder != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(WpfBitmapSource));
                    encoder.Save(fileStream);
                }
            }
        }

        public ExternalImage ToExternalImage()
        {
            var externalImage = new ExternalImage();
            externalImage.Data = Data.ToArray();
            externalImage.DataHeight = DataHeight;
            externalImage.DataWidth = DataWidth;
            externalImage.Type = (ExternalImage.ExternalImageType)Type;
            return externalImage;
        }
    }
}
