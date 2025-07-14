using System;
using System.Drawing;

using AdcTools;


namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des "Matrices" de conversion pixel <-> microns.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class MatrixBase : Serializable
    {
        public bool WaferPositionCorrected;

        //=================================================================
        // Fonctions abstraites
        //=================================================================
        public abstract PointF pixelToMicron(Point pix);
        public abstract Point micronToPixel(PointF mic);
        ///<summary> Vérifie que la matrice est initialisée correctement </summary> 
        public abstract bool Validate();

        //=================================================================
        // 
        //=================================================================
        public virtual QuadF pixelToMicron(Rectangle pix)
        {
            QuadF quad = new QuadF();

            quad.corners[0] = pixelToMicron(pix.TopLeft());
            quad.corners[1] = pixelToMicron(pix.BottomLeft());
            quad.corners[2] = pixelToMicron(pix.BottomRight());
            quad.corners[3] = pixelToMicron(pix.TopRight());

            quad.Normalize();

            return quad;
        }

        //=================================================================
        // 
        //=================================================================
        public virtual Rectangle micronToPixel(RectangleF mic)
        {
            Point pixTopLeft = micronToPixel(mic.TopLeft());
            Point pixBottomRight = micronToPixel(mic.BottomRight());

            Rectangle pix = RectangleExtension.FromP1P2(pixTopLeft, pixBottomRight);
            pix = pix.Normalize();
            return pix;
        }
    }
}
