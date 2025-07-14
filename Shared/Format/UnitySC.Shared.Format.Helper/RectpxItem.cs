using System;
using System.Drawing;

namespace UnitySC.Shared.Format.Helper
{
    public class RectpxItem : IComparable
    {
        private RectangleF _rc;
        public RectangleF Rectpx { get { return _rc; } set { _rc = value; _ptCenter = new PointF(_rc.X + _rc.Width * 0.5f, _rc.Y + _rc.Height * 0.5f); } }

        private PointF _ptCenter;
        public PointF Centerpx { get { return _ptCenter; } }

        public float Areapx { get { return _rc.Width * _rc.Height; } }

        public float DistancepxFrom(float ptX, float ptY)
        {
            return (float)Math.Sqrt(Math.Pow(_ptCenter.X - ptX, 2) + Math.Pow(_ptCenter.Y - ptY, 2));
        }

        public RectpxItem(RectangleF rc)
        {
            _rc = rc;
        }

        public int CompareTo(object obj)
        {
            // we sort items in decroissant area (in order to draw bigger defect in first place)- (=> avoid bigger defect to mask smaller ones)
            return -1 * Areapx.CompareTo((obj as RectpxItem).Areapx);
        }

        public RectangleF ApplyModifiers(float displayFactor, float displayMinSize)
        {
            var rectf = Rectpx;
            if (displayFactor > 1.0f)
            {
                var sf = new SizeF(Rectpx.Width * (displayFactor - 1.0f) * 0.5f, Rectpx.Height * (displayFactor - 1.0f) * 0.5f);
                rectf.Inflate(sf);
            }

            if ((rectf.Width < displayMinSize) || (rectf.Height < displayMinSize))
            {
                // on elargit le rectagle de manière à ce que le rectangle n'est pas un coté plus petit que le mini autorisé
                var sf = new SizeF(0.0F, 0.0F);
                if (rectf.Width < displayMinSize)
                {
                    //increase rect width to the min size
                    float fDiff = displayMinSize - rectf.Width;
                    sf.Width = fDiff * 0.5f;
                }
                if (rectf.Height < displayMinSize)
                {
                    //increase rect height to the min size
                    float fDiff = displayMinSize - rectf.Height; ;
                    sf.Height = fDiff * 0.5f;
                }
                rectf.Inflate(sf);
            }
            return rectf;
        }
    }
}