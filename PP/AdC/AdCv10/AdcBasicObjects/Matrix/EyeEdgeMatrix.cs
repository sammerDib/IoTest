using System;
using System.Drawing;

using ADCEngine;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Matrices de conversion pixel <-> microns pour le Edge.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class EyeEdgeMatrix : MatrixBase
    {
        /// <summary> Angle d'acquisition en radian, (-π/2 => Top, -π/4=> TopBevel, 0=>Apex, π/4=> BottomBevel, π/2=>Bottom) </summary>
        public double SensorVerticalAngle = double.NaN;
        /// <summary> Position du Sensor par rapport au centre du chuck, en µm </summary>
        public double SensorRadiusPosition = double.NaN;

        /// <summary> Diamètre du wafer, en µm </summary>
        public double WaferDiameter { get { return WaferRadius * 2; } set { WaferRadius = value / 2; } }
        public double WaferRadius = double.NaN;

        /// <summary> Angle de démarrage de l'acquisition </summary>
        public double StartAngle = double.NaN;

        /// <summary> Position du Wafer dans le repère chuck, en µm </summary>
        public PointF WaferPositionOnChuck = new PointF(float.NaN, float.NaN);
        /// <summary> Angle du Wafer par rapport au chuck, en radian </summary>
        public double WaferAngle = double.NaN;
        /// <summary>
        /// Taille des pixels, en µm/pix 
        /// La taille en Y correspond à la taille d'un point à un rayon r==WaferRadius
        /// </summary>
        public SizeF PixelSize = new SizeF(float.NaN, float.NaN);

        // Position dans l'image de l'angle 0 du Chuck, en pixels
        public int ChuckOriginY = -1;
        // Position du notch dans l'image, en pixels
        public int NotchY = -1;

        /// <summary> 
        /// Retourne la coordonnée Y de la fin de l'acquisition 
        /// (c'est différent de la fin de la mosaïque car la dernière image n'est pas complète) 
        /// </summary>
        public int EndOfAcquisitionY { get; private set; }

        private float sin, cos;
        private double verticalSin;


        //=================================================================
        // 
        //=================================================================
        public override PointF pixelToMicron(Point pix)
        {
            PointF chuck = new PointF();

            //-------------------------------------------------------------
            // Repère Pixel -> Repère Chuck
            //-------------------------------------------------------------
            // Conversion pixels -> microns dans l'image
            double x = pix.X * PixelSize.Width * verticalSin;
            double y = (pix.Y - NotchY) * PixelSize.Height;

            // Conversion image-cartésiennes -> chuck-polaires
            double rho = SensorRadiusPosition + x;
            double theta = y / WaferRadius - Math.PI / 2;   // y est par rapport au notch qui est à -π/2

            // Conversion polaires -> chuck-cartésiennes
            chuck.X = (float)(rho * Math.Cos(theta));
            chuck.Y = (float)(rho * Math.Sin(theta));

            //-------------------------------------------------------------
            // Repère Chuck -> Repère Wafer
            //-------------------------------------------------------------
            PointF mic = chuck;

            if (SensorVerticalAngle == 0)
            {
                // On voit toujours le bord du wafer, indépendamment de sa position sur le chuck
                // Note FDE : TODO prendre en compte WaferPositionOnChuck 
            }
            else
            {
                mic.X += WaferPositionOnChuck.X;
                mic.Y += WaferPositionOnChuck.Y;
            }

            return mic;
        }

        //=================================================================
        // 
        //=================================================================
        public override Point micronToPixel(PointF mic)
        {
            //-------------------------------------------------------------
            // Repère Wafer -> Repère Chuck
            //-------------------------------------------------------------
            PointF chuck = mic;

            chuck.X -= WaferPositionOnChuck.X;
            chuck.Y -= WaferPositionOnChuck.Y;

            //-------------------------------------------------------------
            // Repère Chuck -> Repère Image
            //-------------------------------------------------------------
            Point pix = new Point();

            double rho = chuck.Radius();
            double x = rho - SensorRadiusPosition;
            if (x < -10000 || 10000 < x)  // ±10mm
                throw new ApplicationException("point is not on the wafer edge");

            if (SensorVerticalAngle == 0)
                pix.X = 0;
            else
                pix.X = (int)(x / PixelSize.Width / verticalSin + 0.5);

            double theta = chuck.Angle() + Math.PI / 2; // par rapport au notch
            double y = theta * WaferRadius / PixelSize.Height;
            y += NotchY;
            y %= EndOfAcquisitionY;
            if (y < 0)
                y += EndOfAcquisitionY;

            pix.Y = (int)(y + 0.5);

            return pix;
        }

        //=================================================================
        // 
        //=================================================================
        public int padAngleToPixel(double angle)
        {
            double alpha = angle - StartAngle;
            double y = alpha * WaferRadius / PixelSize.Height;
            y += ChuckOriginY;
            return (int)(y + 0.5);
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Validate()
        {
            // Vérification des initialisations
            //.................................
            bool valid = true;
            valid = valid && !double.IsNaN(SensorVerticalAngle);
            valid = valid && !double.IsNaN(SensorRadiusPosition);
            valid = valid && !double.IsNaN(WaferPositionOnChuck.X);
            valid = valid && !double.IsNaN(WaferPositionOnChuck.Y);
            valid = valid && !double.IsNaN(WaferAngle);
            valid = valid && !double.IsNaN(StartAngle);
            valid = valid && PixelSize.Width >= 0;
            valid = valid && PixelSize.Height >= 0;
            valid = valid && SensorRadiusPosition >= 0;
            valid = valid && WaferRadius >= 0;
            valid = valid && (NotchY >= 0);
            valid = valid && (ChuckOriginY >= 0);

            if (valid)
            {
                verticalSin = -Math.Abs(Math.Sin(SensorVerticalAngle));  // Les images sont "retournées" pour les capteurs Top 
                sin = (float)Math.Sin(WaferAngle);
                cos = (float)Math.Cos(WaferAngle);
            }

            // Calcul du nombre de lignes acquises
            //....................................
            double alpha = 2 * Math.PI - StartAngle;
            double y = alpha * WaferRadius / PixelSize.Height;
            EndOfAcquisitionY = (int)(y + 0.5);

            return valid;
        }

        //=================================================================
        // 
        //=================================================================
        public override Rectangle micronToPixel(RectangleF mic)
        {
            Point pixTopLeft = micronToPixel(mic.TopLeft());
            Point pixBottomRight = micronToPixel(mic.BottomRight());

            Rectangle pix = RectangleExtension.FromP1P2(pixTopLeft, pixBottomRight);
            // Note FDE : Tfaut-il inverser top et bottom ?
            // Vérifie que la matrice n'inverse pas haut bas.
            // Si le rectangle a une taille négative, cela veut dire que le rectangle est sur le debut/fin de l'image
            //pix = pix.Normalize();
            return pix;
        }

    }
}
