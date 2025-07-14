using System.Drawing;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Matrices de conversion pixel <-> microns sans rotation.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class RectangularMatrix : MatrixBase
    {
        public PointF WaferCenter { get; set; }  // en pixels
        public SizeF PixelSize { get; set; }    // microns

        //=================================================================
        // 
        //=================================================================
        public void Init(PointF waferCenter, SizeF pixelSize)
        {
            PixelSize = pixelSize;
            WaferCenter = waferCenter;
        }

        //=================================================================
        // 
        //=================================================================
        public override PointF pixelToMicron(Point pix)
        {
            PointF mic = new PointF();

            mic.X = (pix.X - WaferCenter.X) * PixelSize.Width;
            mic.Y = (WaferCenter.Y - pix.Y) * PixelSize.Height;

            return mic;
        }

        //=================================================================
        // 
        //=================================================================
        public override Point micronToPixel(PointF mic)
        {
            Point pix = new Point();

            pix.X = (int)((mic.X / PixelSize.Width) + WaferCenter.X);
            pix.Y = (int)(WaferCenter.Y - (mic.Y / PixelSize.Height));

            return pix;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Validate()
        {
            bool valid = true;

            // NB: les test suivants fontionnent avec les NaNs
            valid = valid && (PixelSize.Width > 0);
            valid = valid && (PixelSize.Height > 0);
            valid = valid && (WaferCenter.X > 0);
            valid = valid && (WaferCenter.Y > 0);

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
            pix = pix.Normalize();
            return pix;
        }

        //=================================================================
        // Propriétées pour les bindings car on ne peut pas binder sur 
        // les structures (Point, SizeF)
        //=================================================================
        [XmlIgnore] public float WaferCenterX { get { return WaferCenter.X; } set { WaferCenter = new PointF(value, WaferCenter.Y); } }
        [XmlIgnore] public float WaferCenterY { get { return WaferCenter.Y; } set { WaferCenter = new PointF(WaferCenter.X, value); } }
        [XmlIgnore] public float PixelWidth { get { return PixelSize.Width; } set { PixelSize = new SizeF(value, PixelSize.Height); } }
        [XmlIgnore] public float PixelHeight { get { return PixelSize.Height; } set { PixelSize = new SizeF(PixelSize.Width, value); } }
    }
}
