using System.Drawing;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class FullImageLayer : ImageLayerBase
    {
        public Size ImageSize;

        public FullImage FullImage;

        //=================================================================
        // 
        //=================================================================
        public FullImageLayer()
        {
            MaxDataIndex = 1;
        }

        //=================================================================
        // 
        //=================================================================
        public override void AddImage(ImageBase image)
        {
            if (KeepImageData)
                image.AddRef();
            FullImage = (FullImage)image;
        }

        //=================================================================
        // 
        //=================================================================
        public override void CopyImageDataTo(MilImage destImage, Rectangle pixelRect)
        {
            MilImage dest = (MilImage)destImage;
            MilImage src = FullImage.OriginalProcessingImage.GetMilImage();

            MilImage.CopyColor2d(src, dest,
                MIL.M_ALL_BAND, pixelRect.X, pixelRect.Y,   // src
                MIL.M_ALL_BAND, 0, 0,   // dest
                pixelRect.Width, pixelRect.Height
                 );
        }

        //=================================================================
        // Dispose
        //=================================================================
        public override void FreeImages()
        {
            if (KeepImageData && FullImage != null)
            {
                FullImage.Dispose();
                FullImage = null;
            }
        }


    }
}
