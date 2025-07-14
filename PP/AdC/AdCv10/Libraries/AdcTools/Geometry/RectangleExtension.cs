using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AdcTools
{
    public static class RectangleExtension
    {
        //=================================================================
        // 
        //=================================================================
        public static Rectangle SnapToGrid(this Rectangle rect, int step)
        {
            return SnapToGrid(rect, step, step);
        }

        public static Rectangle SnapToGrid(this Rectangle rect, int stepX, int stepY)
        {
            Rectangle snap = new Rectangle();

            snap.X = rect.X.Floor(stepX);
            snap.Y = rect.Y.Floor(stepY);
            int right = rect.Right.Ceil(stepX);
            snap.Width = right - snap.X;
            int bottom = rect.Bottom.Ceil(stepY);
            snap.Height = bottom - snap.Y;

            return snap;
        }

        //=================================================================
        // 
        //=================================================================
        public static Rectangle DividedBy(this Rectangle rect, int x)
        {
            Rectangle divided = new Rectangle();

            divided.X = rect.X / x;
            divided.Y = rect.Y / x;
            divided.Width = rect.Width / x;
            divided.Height = rect.Height / x;

            return divided;
        }

        //=================================================================
        // 
        //=================================================================
        public static Rectangle NegativeOffset(this Rectangle r, Point offset)
        {
            r.X -= offset.X;
            r.Y -= offset.Y;
            return r;
        }

        //=================================================================
        // La meme chose que Rectangle.Union() mais qui gère le cas où un
        // Rectangle est vide.
        //=================================================================
        public static Rectangle Union(Rectangle r1, Rectangle r2)
        {
            if (r1.IsEmpty)
            {
                return r2;
            }
            else if (r2.IsEmpty)
            {
                return r1;
            }
            else
            {
                int left = Math.Min(r1.Left, r2.Left);
                int top = Math.Min(r1.Top, r2.Top);
                int right = Math.Max(r1.Right, r2.Right);
                int bottom = Math.Max(r1.Bottom, r2.Bottom);

                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }

        public static RectangleF Union(RectangleF r1, RectangleF r2)
        {
            if (r1.IsEmpty)
            {
                return r2;
            }
            else if (r2.IsEmpty)
            {
                return r1;
            }
            else
            {
                float left = Math.Min(r1.Left, r2.Left);
                float top = Math.Min(r1.Top, r2.Top);
                float right = Math.Max(r1.Right, r2.Right);
                float bottom = Math.Max(r1.Bottom, r2.Bottom);

                return RectangleF.FromLTRB(left, top, right, bottom);
            }
        }

        //=================================================================
        // Agrandissement du rectangle pour contenir un point
        //=================================================================
        public static void Union(ref this RectangleF r, PointF p)
        {
            if (float.IsNaN(r.X) || float.IsNaN(r.Y))
            {
                r.X = p.X;
                r.Y = p.Y;
            }
            else if (float.IsNaN(r.Width) || float.IsNaN(r.Height))
            {
                r.Width = p.X - r.X;
                if (r.Width < 0)
                {
                    r.X = p.X;
                    r.Width = -r.Width;
                }
                r.Height = p.Y - r.Y;
                if (r.Height < 0)
                {
                    r.Y = p.Y;
                    r.Height = -r.Height;
                }
            }
            else
            {
                if (p.X < r.X)
                {
                    r.Width += r.X - p.X;
                    r.X = p.X;
                }
                if (p.Y < r.Y)
                {
                    r.Height += r.Y - p.Y;
                    r.Y = p.Y;
                }
                if (p.X > r.Right)
                    r.Width = p.X - r.X;
                if (p.Y > r.Bottom)
                    r.Height = p.Y - r.Y;
            }
        }

        //=================================================================
        // Corners
        //=================================================================
        public static PointF TopLeft(this RectangleF rect)
        {
            PointF p = new PointF();

            p.X = rect.Left;
            p.Y = rect.Top;

            return p;
        }

        public static PointF TopRight(this RectangleF rect)
        {
            PointF p = new PointF();
            p.X = rect.Right;
            p.Y = rect.Top;

            return p;
        }

        public static PointF BottomLeft(this RectangleF rect)
        {
            PointF p = new PointF();

            p.X = rect.Left;
            p.Y = rect.Bottom;
            return p;
        }

        public static PointF BottomRight(this RectangleF rect)
        {
            PointF p = new PointF();
            p.X = rect.Right;
            p.Y = rect.Bottom;
            return p;
        }

        public static Point TopLeft(this Rectangle rect)
        {
            Point p = new Point();

            p.X = rect.Left;
            p.Y = rect.Top;

            return p;
        }

        public static Point TopRight(this Rectangle rect)
        {
            Point p = new Point();

            p.X = rect.Right;
            p.Y = rect.Top;

            return p;
        }

        public static Point BottomLeft(this Rectangle rect)
        {
            Point p = new Point();

            p.X = rect.Left;
            p.Y = rect.Bottom;
            return p;
        }

        public static Point BottomRight(this Rectangle rect)
        {
            Point p = new Point();
            p.X = rect.Right;
            p.Y = rect.Bottom;
            return p;
        }

        public static Point MidTop(this Rectangle rect)
        {
            Point p = new Point();

            p.X = (rect.Left + rect.Right) / 2;
            p.Y = rect.Top;

            return p;
        }

        public static Point MidBottom(this Rectangle rect)
        {
            Point p = new Point();

            p.X = (rect.Left + rect.Right) / 2;
            p.Y = rect.Bottom;

            return p;
        }

        public static PointF MidTop(this RectangleF rect)
        {
            PointF p = new PointF();

            p.X = (rect.Left + rect.Right) / 2;
            p.Y = rect.Top;

            return p;
        }

        public static PointF MidBottom(this RectangleF rect)
        {
            PointF p = new PointF();

            p.X = (rect.Left + rect.Right) / 2;
            p.Y = rect.Bottom;

            return p;
        }

        public static Point Middle(this Rectangle r)
        {
            Point mid = new Point();
            mid.X = (r.Left + r.Right) / 2;
            mid.Y = (r.Top + r.Bottom) / 2;
            return mid;
        }

        public static PointF Middle(this RectangleF r)
        {
            PointF mid = new PointF();
            mid.X = (r.Left + r.Right) * 0.5f;
            mid.Y = (r.Top + r.Bottom) * 0.5f;
            return mid;
        }

        //=================================================================
        // Quarters
        //=================================================================
        public static Rectangle TopLeftQuarter(this Rectangle rect)
        {
            Rectangle quarter = new Rectangle();

            quarter.X = rect.Left;
            quarter.Y = rect.Top;
            quarter.Width = rect.Width / 2;
            quarter.Height = rect.Height / 2;

            return quarter;
        }

        public static Rectangle BottomLeftQuarter(this Rectangle rect)
        {
            Rectangle quarter = new Rectangle();

            quarter.X = rect.Left;
            quarter.Y = rect.Top + rect.Height / 2;
            quarter.Width = rect.Width / 2;
            quarter.Height = rect.Bottom - quarter.Y;

            return quarter;
        }

        public static Rectangle TopRightQuarter(this Rectangle rect)
        {
            Rectangle quarter = new Rectangle();

            quarter.X = rect.Left + rect.Width / 2;
            quarter.Y = rect.Top;
            quarter.Width = rect.Right - quarter.X;
            quarter.Height = rect.Height / 2;

            return quarter;
        }

        public static Rectangle BottomRightQuarter(this Rectangle rect)
        {
            Rectangle quarter = new Rectangle();

            quarter.X = rect.Left + rect.Width / 2;
            quarter.Y = rect.Top + rect.Height / 2;
            quarter.Width = rect.Right - quarter.X;
            quarter.Height = rect.Bottom - quarter.Y;

            return quarter;
        }

        //=================================================================
        // 
        //=================================================================
        public static long Area(this Rectangle rect)
        {
            long area = Math.Abs((long)rect.Width * (long)rect.Height);
            return area;
        }

        public static double Area(this RectangleF rect)
        {
            double area = Math.Abs(rect.Width * rect.Height);
            return area;
        }

        //=================================================================
        // 
        //=================================================================
        public static Rectangle ToRectangle(this RectangleF rectF)
        {
            Rectangle rect = new Rectangle();

            rect.X = (int)(rectF.X + 0.5);
            rect.Y = (int)(rectF.Y + 0.5);
            rect.Width = (int)(rectF.Width + 0.5);
            rect.Height = (int)(rectF.Height + 0.5);

            return rect;
        }

        public static RectangleF ToRectangleF(this Rectangle rect)
        {
            RectangleF rectF = new RectangleF();

            rectF.X = rect.X;
            rectF.Y = rect.Y;
            rectF.Width = rect.Width;
            rectF.Height = rect.Height;

            return rectF;
        }

        public static System.Windows.Rect ToWinRect(this Rectangle rect)
        {
            return new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Windows.Rect ToWinRect(this RectangleF rectF)
        {
            return new System.Windows.Rect(rectF.X, rectF.Y, rectF.Width, rectF.Height);
        }

        public static Rectangle ToRectangle(this System.Windows.Rect winRect)
        {
            Rectangle rect = new Rectangle();

            rect.X = (int)(winRect.X + 0.5);
            rect.Y = (int)(winRect.Y + 0.5);
            rect.Width = (int)(winRect.Width + 0.5);
            rect.Height = (int)(winRect.Height + 0.5);

            return rect;
        }
        public static RectangleF ToRectangleF(this System.Windows.Rect winRect)
        {
            RectangleF rectF = new RectangleF();

            rectF.X = (float) winRect.X;
            rectF.Y = (float) winRect.Y;
            rectF.Width = (float) winRect.Width;
            rectF.Height = (float) winRect.Height;

            return rectF;
        }

        public static QuadF ToQuadF(this RectangleF rectF)
        {
            QuadF q = new QuadF(rectF);
            return q;
        }

        //=================================================================
        // 
        //=================================================================
        public static eCompare IsInside(this RectangleF r, CircleF c)
        {
            // Point le plus pres du centre du cercle dans le rectangle
            PointF closest = new PointF();
            closest.X = c.center.X.ClampTo(r.Left, r.Right);
            closest.Y = c.center.Y.ClampTo(r.Top, r.Bottom);

            // Distance de ce point au centre du cercle
            double distance2 = PointSizeExtension.Distance2(c.center, closest);
            double radius2 = c.radius * c.radius;

            // Le centre du cercle est loin du rectangle ?
            if (distance2 > radius2)
                return eCompare.RectIsOutside;

            // Point du rectangle le plus loin du centre
            PointF mid = r.Middle();
            PointF farthest = new Point();
            farthest.X = (mid.X < c.center.X) ? r.X : r.Right;
            farthest.Y = (mid.Y < c.center.Y) ? r.Y : r.Bottom;

            // Distance de ce point au centre du cercle
            distance2 = PointSizeExtension.Distance2(c.center, farthest);

            // Le centre du rectangle est proche du cercle ?
            if (distance2 <= radius2)
                return eCompare.RectIsInside;
            else
                return eCompare.RectIntersects;
        }

        public static eCompare IsInside(this RectangleF r1, RectangleF r2)
        {
            bool bOutside = false;
            bOutside = bOutside || (r1.Right < r2.Left) || (r2.Right < r1.Left);
            bOutside = bOutside || (r1.Bottom < r2.Top) || (r2.Bottom < r1.Top);
            if (bOutside)
                return eCompare.RectIsOutside;

            bool bInside = true;
            bInside = bInside && (r2.Left < r1.Left) && (r1.Right < r2.Right);
            bInside = bInside && (r2.Top < r1.Top) && (r1.Bottom < r2.Bottom);
            if (bInside)
                return eCompare.RectIsOutside;

            return eCompare.RectIntersects;
        }

        public static eCompare IsInside(this RectangleF r, QuadF q)
        {
            //----------------------------------------------------------------------
            // Est-ce que les cotés du quadrilatère coupent les cotés du rectangle ?
            //----------------------------------------------------------------------
            VectorF[] rsides = r.Sides();
            VectorF[] qsides = q.Sides;

            foreach (VectorF rside in rsides)
            {
                foreach (VectorF qside in qsides)
                {
                    bool b = rside.Intersect(qside);
                    if (b)
                        return eCompare.QuadIntersects;
                }
            }

            //----------------------------------------------------------------------
            // Sinon, le rectangle est-il dans le quadrilatère ?
            //----------------------------------------------------------------------
            if (r.TopLeft().IsInside(q))
                return eCompare.RectIsInside;
            else if (r.Contains(q.corners[0]))
                return eCompare.QuadIntersects; // en fait le quad est dedans
            else
                return eCompare.QuadIsOutside;
        }

        //=================================================================
        // 
        //=================================================================
        public static Rectangle Normalize(this Rectangle r)
        {
            if (r.Width < 0)
            {
                int right = r.Right;
                r.Width = -r.Width;
                r.X = right;
            }

            if (r.Height < 0)
            {
                int bottom = r.Bottom;
                r.Height = -r.Height;
                r.Y = bottom;
            }

            return r;
        }

        public static RectangleF Normalize(this RectangleF r)
        {
            if (r.Width < 0)
            {
                float right = r.Right;
                r.Width = -r.Width;
                r.X = right;
            }

            if (r.Height < 0)
            {
                float bottom = r.Bottom;
                r.Height = -r.Height;
                r.Y = bottom;
            }

            return r;
        }

        //=================================================================
        // 
        //=================================================================
        public static Point[] Corners(this Rectangle r)
        {
            Point[] corners = new Point[4];
            corners[0] = r.TopLeft();
            corners[1] = r.TopRight();
            corners[2] = r.BottomRight();
            corners[3] = r.BottomLeft();
            return corners;
        }

        public static PointF[] Corners(this RectangleF r)
        {
            PointF[] corners = new PointF[4];
            corners[0] = r.TopLeft();
            corners[1] = r.TopRight();
            corners[2] = r.BottomRight();
            corners[3] = r.BottomLeft();
            return corners;
        }

        public static VectorF[] Sides(this RectangleF r)
        {
            PointF[] corners = r.Corners();
            VectorF[] sides = new VectorF[4];
            sides[0] = new VectorF(corners[0], corners[1]);
            sides[1] = new VectorF(corners[1], corners[2]);
            sides[2] = new VectorF(corners[2], corners[3]);
            sides[3] = new VectorF(corners[3], corners[0]);
            return sides;
        }

        //=================================================================
        // Conversion Region -> Rectangle
        //=================================================================
        private static Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);

        public static IEnumerable<Rectangle> ToRectangles(this Region region)
        {
            // Convert to RecctangleF
            //.......................
            RectangleF[] rectFs;
            lock (matrix)
                rectFs = region.GetRegionScans(matrix);

            // Convert to Rectangle
            //.....................
            foreach (RectangleF rectF in rectFs)
            {
                Rectangle r = Rectangle.Ceiling(rectF);
                yield return r;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public static Rectangle FromP1P2(Point p1, Point p2)
        {
            return Rectangle.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static RectangleF FromP1P2(PointF p1, PointF p2)
        {
            return RectangleF.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
        }

        //=================================================================
        // 
        //=================================================================
        public static double Diagonal(this Rectangle rect)
        {
            double w = rect.Width;
            double h = rect.Height;
            return Math.Sqrt(w * w + h * h);
        }

        public static double Diagonal(this RectangleF rectf)
        {
            double w = rectf.Width;
            double h = rectf.Height;
            return Math.Sqrt(w * w + h * h);
        }

    }
}
