using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Extensions;
using UnitySC.Shared.Data.FormatFile;

namespace UnitySC.Shared.Data.ExternalFile
{
    /// <summary>
    /// Image data are save outside the XmlContent
    /// </summary>
    public class ExternalImage : ExternalFileBase
    {
        public enum ExternalImageType
        { Greyscale, RGB, _3DA, Greyscale16Bit };

        public override string FileExtension { get; set; } = ".tiff";

        [DataMember]
        public ExternalImageType Type { get; set; }

        [DataMember]
        [XmlIgnore]
        public int DataWidth { get; set; }

        [DataMember]
        [XmlIgnore]
        public int DataHeight { get; set; }

        /// <summary>
        /// Bits par pixel
        /// </summary>
        [XmlIgnore]
        public int Depth
        {
            get
            {
                switch (Type)
                {
                    case ExternalImageType.Greyscale: return 8;
                    case ExternalImageType.Greyscale16Bit: return 16;
                    case ExternalImageType.RGB: return 24;
                    case ExternalImageType._3DA: return 32; // NOTE de RTI : c'est le format complet que l'on enregistre et pas la matrice de float !
                    default:
                        throw new ApplicationException("unknown image format");
                }
            }
        }

        private WriteableBitmap _wpfBitmapSource;

        [XmlIgnore]
        public BitmapSource WpfBitmapSource
        {
            get
            {
                if (_wpfBitmapSource == null)
                    _wpfBitmapSource = ConvertToWriteableBitmap();
                return _wpfBitmapSource;
            }
        }

        public override void LoadFromFile(string filePath)
        {
            Stream imageStreamSource = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!imageStreamSource.CanRead)
            {
                throw new ApplicationException("Image cannot be read: " + filePath);
            }

            BitmapDecoder decoder = null;
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
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
                        Data = File.ReadAllBytes(filePath);
                        if (MatrixFloatFile.GetSizeFromMemory(Data, out int w, out int h))
                        {
                            DataWidth = w;
                            DataHeight = h;
                        }
                    }
                    break;

                default:
                    throw new ApplicationException("unkwown file extension: " + extension);
            }
            if (decoder != null)
            {
                BitmapSource img = decoder.Frames[0];
                Data = img.ConvertToByteArray();
                DataWidth = img.PixelWidth;
                DataHeight = img.PixelHeight;
            }
            // note de RTI : closing image strem source ?
        }

        public override void SaveToFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = null;
                string ext = Path.GetExtension(filePath).ToLower().ToLower();
                switch (ext)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".tif":
                    case ".tiff":
                        encoder = new TiffBitmapEncoder();
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
                    encoder.Frames.Add(BitmapFrame.Create(ConvertToWriteableBitmap()));
                    encoder.Save(fileStream);
                }
            }
        }

        private WriteableBitmap ConvertToWriteableBitmap()
        {
            // Création de la WriteableBitmap
            //...............................
            var format = System.Windows.Media.PixelFormats.Gray8;
            switch (Type)
            {
                case ExternalImageType.Greyscale:
                    format = System.Windows.Media.PixelFormats.Gray8;
                    break;

                case ExternalImageType.Greyscale16Bit:
                    format = System.Windows.Media.PixelFormats.Gray16;
                    break;

                case ExternalImageType.RGB:
                    format = System.Windows.Media.PixelFormats.Rgb24;
                    break;

                case ExternalImageType._3DA:
                    throw new NotImplementedException("3da writeable bitmap");
                //format = System.Windows.Media.PixelFormats.Gray32Float;

                default:
                    throw new ApplicationException("unknown image format");
            }
            int sizeX = DataWidth;
            int sizeY = DataHeight;
            var writeableBitmap = new WriteableBitmap(sizeX, sizeY, 96, 96, format, null);

            // Copie des pixels
            //.................
            unsafe
            {
                int pitch = sizeX * (Depth / 8);
                int bufferSize = pitch * sizeY;

                fixed (byte* pSrc = &Data[0])
                    writeableBitmap.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), new IntPtr(pSrc), bufferSize, pitch);
            }

            return writeableBitmap;
        }

        public override void UpdateWith(ExternalFileBase externalFileBase)
        {
            var newExternalImageContent = externalFileBase as ExternalImage;
            if (newExternalImageContent is null)
                throw new InvalidCastException("ExternalImage is expected in update");
            Type = newExternalImageContent.Type;
            DataWidth = newExternalImageContent.DataWidth;
            DataHeight = newExternalImageContent.DataHeight;
            Data = newExternalImageContent.Data;
            FileNameKey = externalFileBase.FileNameKey;
        }

        public ExternalImage DeepClone()
        {
            var clone = (ExternalImage)MemberwiseClone();
            return clone;
        }
    }
}
