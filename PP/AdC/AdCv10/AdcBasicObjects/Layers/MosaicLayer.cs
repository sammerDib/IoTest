using System.Collections.Generic;
using System.Drawing;

using AdcTools;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class MosaicLayer : ImageLayerBase
    {
        private object mutex = new object();
        ///=================================================================<summary>
        /// Sous-classe pour représenter un couple ligne colonne
        ///</summary>=================================================================
        public struct LC
        {
            public int l;
            public int c;

            public LC(int l, int c)
            {
                this.l = l;
                this.c = c;
            }

            public override string ToString()
            {
                return "LC-" + l + "-" + c;
            }
        }

        //=================================================================
        // Propriétés
        //=================================================================

        // Dimension de la layer
        public int NbLines { get; set; }
        public int NbColumns { get; set; }
        /// <summary> Dimension d'une mosaïque unitaire (tesselle) </summary>
        public Size MosaicImageSize;

        /// <summary> Dimension de la mosaïque complète </summary>
        public Size FullImageSize
        {
            get
            {
                int w = NbColumns * MosaicImageSize.Width;
                int h = NbLines * MosaicImageSize.Height;
                return new Size(w, h);
            }
        }

        ///<summary> List des images de la layer. Si besoin, les images sont stockées dans cette map.  </summary>
        private Dictionary<LC, MosaicImage> imageMap = new Dictionary<LC, MosaicImage>();

        //=================================================================
        // 
        //=================================================================
        public override void AddImage(ImageBase image)
        {
            if (KeepImageData)
                image.AddRef();

            MosaicImage mosaic = (MosaicImage)image;
            LC lc = new LC(mosaic.Line, mosaic.Column);
            lock (mutex)
            {
                imageMap.Add(lc, mosaic);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void CopyImageDataTo(MilImage destImage, Rectangle pixelRect)
        {
            int x0 = pixelRect.Left;
            x0 = x0 - x0 % MosaicImageSize.Width;
            int y0 = pixelRect.Top;
            y0 = y0 - y0 % MosaicImageSize.Height;

            for (int x = x0; x < pixelRect.Right; x += MosaicImageSize.Width)
            {
                for (int y = y0; y < pixelRect.Bottom; y += MosaicImageSize.Height)
                {
                    // Récupération de la mosaïque
                    //............................
                    LC lc = GetLineColum(new Point(x, y));
                    MosaicImage mosaic = imageMap[lc];

                    // Calcul des positions / size
                    //............................
                    Rectangle rect = pixelRect; // Position du morceau à copier dans le repère pixel global
                    rect.Intersect(mosaic.imageRect);

                    Point posSrc = PointSizeExtension.Subtract(rect.TopLeft(), mosaic.imageRect.TopLeft()).ToPoint(); // Position du morceau à copier dans l'image source (mosaïque)
                    Point posDest = PointSizeExtension.Subtract(rect.TopLeft(), pixelRect.TopLeft()).ToPoint();      // Position du morceau à copier dans l'image destination

                    // Copy de la partie intéressante
                    //...............................
                    MilImage srcImage = mosaic.OriginalProcessingImage.GetMilImage();
                    MilImage.CopyColor2d(srcImage, destImage,
                        MIL.M_ALL_BAND, posSrc.X, posSrc.Y,   // src
                        MIL.M_ALL_BAND, posDest.X, posDest.Y, // dest
                        rect.Width, rect.Height
                        );
                }
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        public override void FreeImages()
        {
            if (KeepImageData && imageMap != null)
            {
                foreach (MosaicImage img in imageMap.Values)
                    img.Dispose();
                imageMap.Clear();
                imageMap = null;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public LC GetLineColum(Point p)
        {
            LC lc = new LC(p.Y / MosaicImageSize.Height, p.X / MosaicImageSize.Width);
            return lc;
        }

        ///=================================================================
        ///<summary>
        /// Conversion des coordonnées FullImage en coordonées dans la mosaïque
        /// (en fait dans la tesselle).
        /// <param name="p">Coordonnées dans l'image complète</param>
        /// <returns>Coordonnées dans la tesselle</returns>
        ///</summary>
        ///=================================================================
        public Point GetPositionInMosaic(Point p)
        {
            int x = p.X % MosaicImageSize.Width;
            int y = p.Y % MosaicImageSize.Height;

            return new Point(x, y);
        }

    }
}
