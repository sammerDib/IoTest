using System;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;

namespace LibProcessing
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class ProcessingImage : DisposableObject
    {
        protected enum eImageType { Mil, CSharp }
        protected eImageType _eImageType = eImageType.Mil;

        protected MilImage milImage = new MilImage();
        protected CSharpImage csharpImage;

        public int Width { get { return milImage.SizeX; } }
        public int Height { get { return milImage.SizeY; } }
        public enum eImageFormat { GreyLevel, Height3D }
        public eImageFormat Format
        {
            get
            {
                if (milImage.Type == MIL.M_FLOAT + 32)
                    return eImageFormat.Height3D;
                else
                    return eImageFormat.GreyLevel;
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (milImage != null)
            {
                milImage.Dispose();
                milImage = null;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            ProcessingImage clone = (ProcessingImage)obj;
            clone.milImage = (MilImage)this.GetMilImage().DeepClone();
        }

        //=================================================================
        // 
        //=================================================================
        public void SetMilImage(MilImage img)
        {
            milImage.DelRef();

            _eImageType = eImageType.Mil;
            img.AddRef();
            milImage = img;
        }

        //=================================================================
        /// <summary>
        /// Copy an array to the image data 
        /// </summary>
        //=================================================================
        public void PutCSharpArray(byte[,] data)
        {
            if ((data.GetLength(0) != Width) || (data.GetLength(1) != Height))
                throw new ApplicationException("can't change image size");

            if (_eImageType == eImageType.CSharp)
            {
                if (!milImage.IsLocked)
                    throw new ApplicationException("unlocked image");
                milImage.Unlock();
            }

            _eImageType = eImageType.Mil;
            milImage.Put(data);
        }

        //=================================================================
        // 
        //=================================================================
        public MilImage GetMilImage()
        {
            switch (_eImageType)
            {
                case eImageType.Mil:
                    break;

                case eImageType.CSharp:
                    if (!milImage.IsLocked)
                        throw new ApplicationException("unlocked image");
                    milImage.Unlock();
                    break;

                default:
                    throw new ApplicationException("unknown image type: " + _eImageType);
            }

            _eImageType = eImageType.Mil;
            return milImage;
        }

        //=================================================================
        // 
        //=================================================================
        public CSharpImage GetCSharpImage()
        {
            // Create a CppImage header
            //.........................
            if (csharpImage == null)
                csharpImage = new CSharpImage();

            csharpImage.width = milImage.SizeX;
            csharpImage.height = milImage.SizeY;
            csharpImage.pitch = milImage.Pitch;

            // Copie des données
            //..................
            switch (_eImageType)
            {
                case eImageType.Mil:
                    if (milImage.IsLocked)
                        throw new ApplicationException("locked image");
                    milImage.Lock();
                    csharpImage.ptr = milImage.HostAddress;
                    break;

                case eImageType.CSharp:
                    break;

                default:
                    throw new ApplicationException("unknown image type: " + _eImageType);
            }

            _eImageType = eImageType.CSharp;

            // Check depth and number of bands
            //................................
            if (milImage.SizeBand != 1)
                throw new ApplicationException("invalid image format");

            int type = milImage.Type;
            if (type == 8 + MIL.M_UNSIGNED)
                csharpImage.depth = 8;
            else if (type == 1 + MIL.M_UNSIGNED)
                csharpImage.depth = 8;
            else if (type == 16 + MIL.M_UNSIGNED)
                csharpImage.depth = 16;
            else if (type == 32 + MIL.M_FLOAT)
                csharpImage.depth = 32;
            else
                throw new ApplicationException("invalid image format");

            return csharpImage;
        }

        //=================================================================
        // Conversion en Bitmap C#
        //=================================================================
        public System.Drawing.Bitmap ConvertToBitmap()
        {
            MilImage milImage = GetMilImage();
            if (milImage == null)
                return null;

            return milImage.ConvertToBitmap();
        }

        //=================================================================
        // Convert to WPF BitmapSource
        //=================================================================
        public System.Windows.Media.Imaging.BitmapSource ConvertToWpfBitmapSource()
        {
            MilImage milImage = GetMilImage();
            if (milImage == null)
                return null;

            return milImage.ConvertToWpfBitmapSource();
        }
    }
}
