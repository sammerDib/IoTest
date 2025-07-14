using System;
using System.Drawing;

namespace AdcTools
{
    public static class PointSizeExtension
    {
        //=================================================================
        //
        //=================================================================
        public static Point ClampTo(this Point point, Rectangle rect)
        {
            point.X = point.X.ClampTo(rect.Left, rect.Right);
            point.Y = point.Y.ClampTo(rect.Top, rect.Bottom);
            return point;
        }


        //=================================================================
        // 
        //=================================================================
        public static Size SnapToGrid(this Size size, int step)
        {
            Size snap = new Size();

            snap.Width = size.Width.Ceil(step);
            snap.Height = size.Height.Ceil(step);

            return snap;
        }

        //=================================================================
        // 
        //=================================================================
        public static Point DividedBy(this Point point, int x)
        {
            Point divided = new Point();

            divided.X = point.X / x;
            divided.Y = point.Y / x;

            return divided;
        }

        public static Size DividedBy(this Size size, int x)
        {
            Size divided = new Size();

            divided.Width = size.Width / x;
            divided.Height = size.Height / x;

            return divided;
        }

        //=================================================================
        // 
        //=================================================================
        public static Size Subtract(Point p1, Point p2)
        {
            Size size = new Size();

            size.Width = p1.X - p2.X;
            size.Height = p1.Y - p2.Y;

            return size;
        }

        public static Point NegativeOffset(this Point p, Point offset)
        {
            p.X -= offset.X;
            p.Y -= offset.Y;
            return p;
        }

        //=================================================================
        // 
        //=================================================================
        public static PointF ToPointF(this Point point)
        {
            PointF pointF = new PointF();

            pointF.X = point.X;
            pointF.Y = point.Y;

            return pointF;
        }

        public static Point ToPoint(this Size size)
        {
            Point point = new Point();

            point.X = size.Width;
            point.Y = size.Height;

            return point;
        }

        public static Size ToSize(this Point point)
        {
            Size size = new Size();

            size.Width = point.X;
            size.Height = point.Y;

            return size;
        }

        //=================================================================
        // 
        //=================================================================
        public static Point Invert(this Point point)
        {
            point.X = -point.X;
            point.Y = -point.Y;

            return point;
        }

        //=================================================================
        // Distance au point (0,0)
        //=================================================================
        public static double Radius(this PointF p)
        {
            double distance = Math.Sqrt(p.X * p.X + p.Y * p.Y);
            return distance;
        }

        public static double Radius(this SizeF s)
        {
            double distance = Math.Sqrt(s.Width * s.Width + s.Height * s.Height);
            return distance;
        }

        //=================================================================
        // Angle du vecteur par rapport au point (0,0)
        //=================================================================
        public static double Angle(this PointF p)
        {
            double angle = Math.Atan2(p.Y, p.X);
            return angle;
        }

        //=================================================================
        // 
        //=================================================================
        public static double Distance2(PointF p1, PointF p2)
        {
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;
            double distance2 = deltaX * deltaX + deltaY * deltaY;

            return distance2;
        }

        public static double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Distance2(p1, p2));
        }

        //=================================================================
        // 
        //=================================================================
        public static bool IsInside(this PointF p, CircleF c)
        {
            double distance2 = Distance2(p, c.center);
            double radius2 = c.radius * c.radius;
            bool inside = (distance2 <= radius2);

            return inside;
        }

        public static bool IsInside(this PointF p, QuadF q)
        {
            VectorF[] sides = q.Sides;

            foreach (VectorF s in sides)
            {
                bool left = p.IsLeft(s);
                if (!left)
                    return false;
            }
            return true;
        }

        public static bool IsLeft(this PointF p, VectorF v)
        {
            double cross = CrossProduct(v.Start, v.End, p);
            return cross >= 0;
        }

        public static bool IsRight(this PointF p, VectorF v)
        {
            double cross = CrossProduct(v.Start, v.End, p);
            return cross <= 0;
        }

        //=================================================================
        // Compute the cross product AB x AC
        //=================================================================
        private static double CrossProduct(PointF pointA, PointF pointB, PointF pointC)
        {
            VectorF AB = new VectorF(pointA, pointB);
            VectorF AC = new VectorF(pointA, pointC);

            double cross = VectorF.CrossProduct(AB, AC);
            return cross;
        }

        //=================================================================
        // Compute the dot product AB . BC
        //=================================================================
        private static double DotProduct(PointF pointA, PointF pointB, PointF pointC)
        {
            VectorF AB = new VectorF(pointA, pointB);
            VectorF BC = new VectorF(pointB, pointC);

            double dot = VectorF.DotProduct(AB, BC);
            return dot;
        }

        //=================================================================
        // Distance du point p a la ligne v
        //=================================================================
        public static double LineDistance(VectorF v, PointF p)
        {
            double dist = CrossProduct(v.Start, v.End, p) / v.Length;
            return Math.Abs(dist);
        }

        //=================================================================
        // Distance du point p au segment v
        //=================================================================
        public static double Distance(VectorF v, PointF p)
        {
            // (note de rti)
            // Je ne suis vraiment pas sur de la justesse mathématiques de cette fonction  
            //(vu cas fine bande passant en plein milieu du wafer mais dont les extrémité sont exterieure retourne une distance exuberente entre p et v.
            // soit A= v.Start, B=v.End, C=p

            /*          double dot1 = DotProduct(v.Start, v.End, p);  // fait AB . BC
                      if (dot1 > 0)
                          return Distance(v.End, p);

                      double dot2 = DotProduct(v.End, v.Start, p); // fait BA . AC
                      if (dot2 > 0)
                          return Distance(v.Start, p);

                      double dist = CrossProduct(v.Start, v.End, p) / v.Length;
                      return (float)Math.Abs(dist);*/

            ////////////////////////////////////////////////////////////////////
            //Cf http://www.faqs.org/faqs/graphics/algorithms-faq/ subject 1.02
            ////////////////////////////////////////////////////////////////////
            /* Subject 1.02: How do I find the distance from a point to a line?
             * 
            Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
            Let P be the point of perpendicular projection of C on AB.  The parameter
            r, which indicates P's position along AB, is computed by the dot product 
            of AC and AB divided by the square of the length of AB:

            (1)
                AC dot AB
            r = ---------  
                ||AB||^2

            r has the following meaning:

            r=0      P = A
            r=1      P = B
            r<0      P is on the backward extension of AB
            r>1      P is on the forward extension of AB
            0<r<1    P is interior to AB

            The length of a line segment in d dimensions, AB is computed by:

            L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 + ... + (Bd-Ad)^2)

            so in 2D:   

            L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 )

            and the dot product of two vectors in d dimensions, U dot V is computed:

            D = (Ux * Vx) + (Uy * Vy) + ... + (Ud * Vd)

            so in 2D:   

            D = (Ux * Vx) + (Uy * Vy) 

            So (1) expands to:

            (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
            r = -------------------------------
                          L^2

            The point P can then be found:

            Px = Ax + r(Bx-Ax)
            Py = Ay + r(By-Ay)

            And the distance from A to P = r*L.

            Use another parameter s to indicate the location along PC, with the 
            following meaning:
               s<0      C is left of AB
               s>0      C is right of AB
               s=0      C is on AB

            Compute s as follows:

                    (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)
                s = -----------------------------
                                L^2

            Then the distance from C to P = |s|*L.
            */

            // soit A= v.Start, B=v.End, C=p

            double dSquaredNorme = Math.Pow((v.End.X - v.Start.X), 2.0) + Math.Pow((v.End.Y - v.Start.Y), 2.0);
            if (dSquaredNorme == 0.0) // A==B
                return Distance(v.Start, p);

            double r = ((p.X - v.Start.X) * (v.End.X - v.Start.X) + (p.Y - v.Start.Y) * (v.End.Y - v.Start.Y)) / dSquaredNorme;
            if (r <= 0)
                return Distance(v.Start, p);

            if (r >= 1)
                return Distance(v.End, p);

            // on calcule "s"
            double s = ((v.Start.Y - p.Y) * (v.End.X - v.Start.X) - (v.Start.X - p.X) * (v.End.Y - v.Start.Y)) / dSquaredNorme;

            return (Math.Abs(s) * Math.Sqrt(dSquaredNorme));

        }

    }
}

