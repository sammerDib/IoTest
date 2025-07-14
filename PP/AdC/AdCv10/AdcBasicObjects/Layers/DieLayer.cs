using System;
using System.Collections.Generic;
using System.Drawing;

using AdcTools;

using UnitySC.Shared.LibMIL;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class DieLayer : ImageLayerBase
    {
        ///=================================================================
        ///<summary>
        /// Sous-classe pour représenter un couple ligne colonne
        ///</summary>
        ///=================================================================
        public struct XY
        {
            public int x;
            public int y;

            public XY(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return "XY-" + x + "-" + y;
            }
        }

        //=================================================================
        // Propriétés
        //=================================================================
        ///<summary> Nombre de dies qu'on va traiter dans cette layer </summary>
        public int NbDies;

        public double DiePitchX_µm;
        public double DiePitchY_µm;

        public double DieOriginX_µm;
        public double DieOriginY_µm;

        public double DieSizeX_µm;
        public double DieSizeY_µm;
        public double WaferCenterX;   // en pixels
        public double WaferCenterY;   // en pixels


        ///<summary> Dimension de la grille de dies <summary>
        public Rectangle DieIndexes = new Rectangle(0, 0, 1, 1);

        ///<summary> Stockage des dies par le dataloader <summary>
        private Dictionary<XY, DieImage> imageMap = new Dictionary<XY, DieImage>();

        // Les propriétés suivantes sont pour la sérialisation XML. La sérialisation par défaut du Rectangle est assez moche.
        public int MinIndexX { get { return DieIndexes.X; } set { DieIndexes.X = value; } }
        public int MinIndexY { get { return DieIndexes.Y; } set { DieIndexes.Y = value; } }
        public int MaxIndexX { get { return DieIndexes.Right; } set { DieIndexes.Width = value - MinIndexX + 1; } }
        public int MaxIndexY { get { return DieIndexes.Bottom; } set { DieIndexes.Height = value - MinIndexY + 1; } }

        //=================================================================
        // 
        //=================================================================
        public override void AddImage(ImageBase image)
        {
            if (KeepImageData)
                image.AddRef();

            DieImage die = (DieImage)image;
            XY xy = new XY(die.DieIndexX, die.DieIndexY);
            imageMap.Add(xy, die);
        }

        //=================================================================
        // 
        //=================================================================
        public DieImage GetDieImage(int DieIndexX, int DieIndexY)
        {
            XY xy = new XY(DieIndexX, DieIndexY);
            DieImage die = imageMap[xy];
            return die;
        }

        ///=================================================================
        /// <summary>
        /// Calcule le rectangle correspondant au die.
        /// Attention, c'est différent du rectangle d'une image die car 
        /// l'image ne couvre pas forcément tout le die.
        /// </summary>
        ///=================================================================
        public RectangleF GetDieMicronRect(int DieIndexX, int DieIndexY)
        {
            double x = 0;
            double y;//= DieOriginY_µm - DieIndexY * DiePitchY_µm;
            if (DieIndexX < 0)
            {
                x = Math.Abs(DieOriginX_µm) + DieIndexX * DiePitchX_µm;
            }
            else
            {
                x = Math.Abs(DieOriginX_µm) + DieIndexX * DiePitchX_µm;
            }
            y = DiePitchY_µm - DieOriginY_µm + DieIndexY * DiePitchY_µm;

            //y += DiePitchY_µm;  // Origine en bas du die
            return new RectangleF((float)x, (float)y, (float)DiePitchX_µm, (float)DiePitchY_µm);
        }
        public RectangleF GetSampleCenterLocation()
        {
            double x = DieOriginX_µm;
            double y = DieOriginY_µm;
            y -= DiePitchY_µm;  // Origine en bas du die
            return new RectangleF((float)x, (float)y, (float)DiePitchX_µm, (float)DiePitchY_µm);
        }

        public QuadF PixelToMicron(Rectangle pix)
        {
            QuadF quad = new QuadF();

            quad.corners[0] = pixelToMicron(pix.TopLeft());
            quad.corners[1] = pixelToMicron(pix.BottomLeft());
            quad.corners[2] = pixelToMicron(pix.BottomRight());
            quad.corners[3] = pixelToMicron(pix.TopRight());

            quad.Normalize();

            return quad;
        }

        private PointF pixelToMicron(Point pix)
        {
            PointF mic = new PointF();
            RectangularMatrix matrix = ((RectangularMatrix)Matrix);
            mic.X = pix.X * matrix.PixelSize.Width;
            mic.Y = pix.Y * matrix.PixelSize.Height;

            return mic;

        }

        /// <summary>
        /// Converti blobsurroundingRectangle en coordonnées dans le die : origine le coin infèrieur gauche du die 
        /// </summary>
        /// <param name="dierect_um">Coordonées du die . Référence : centre du wafer</param>
        /// <param name="cluster"> cluster du die</param>
        /// <param name="blobsurroundingRectangle">coordonnées du blob : origine top left du wafer . y à l'opposé du y klarf : </param>
        /// <returns></returns>
        public RectangleF GetRectCoordInDie(Cluster cluster, RectangleF dierect_um, RectangleF blobsurroundingRectangle)
        {
            RectangleF result = blobsurroundingRectangle;

            //result.Y = 1000  ;

            result.X -= cluster.DieOffsetImage.X * ((RectangularMatrix)Matrix).PixelWidth;
            result.Y -= cluster.DieOffsetImage.Y * ((RectangularMatrix)Matrix).PixelHeight;
            result.Y = (float)DiePitchY_µm - result.Y;

            return result;
        }


        //=================================================================
        // 
        //=================================================================
        public IEnumerable<DieImage> GetDieImageList()
        {
            return imageMap.Values;
        }

        //=================================================================
        //
        //=================================================================
        public override void CopyImageDataTo(MilImage destImage, Rectangle pixelRect) { throw new NotImplementedException(); }

        //=================================================================
        //
        //=================================================================
        public override void FreeImages()
        {
            if (KeepImageData && imageMap != null)
            {
                foreach (DieImage img in imageMap.Values)
                    img.Dispose();
                imageMap.Clear();
                imageMap = null;
            }
        }

    }
}
