using System;
using System.ComponentModel;
using System.Drawing;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Quadrilatère en coordonnées flottantes.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class QuadF
    {
        //=================================================================
        // Data
        //=================================================================
        public PointF[] corners = new PointF[4];

        //=================================================================
        // Constructeur
        //=================================================================
        public QuadF() { }

        public QuadF(RectangleF r)
        {
            corners[0] = r.TopLeft();
            corners[1] = r.BottomLeft();
            corners[2] = r.BottomRight();
            corners[3] = r.TopRight();
        }

        public QuadF(QuadF copy)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i].X = copy.corners[i].X;
                corners[i].Y = copy.corners[i].Y;
            }
        }

        //=================================================================
        // 
        //=================================================================
        [Browsable(false)]
        public VectorF[] Sides
        {
            get
            {
                VectorF[] sides = new VectorF[4];
                sides[0] = new VectorF(corners[0], corners[1]);
                sides[1] = new VectorF(corners[1], corners[2]);
                sides[2] = new VectorF(corners[2], corners[3]);
                sides[3] = new VectorF(corners[3], corners[0]);
                return sides;
            }
        }

        //=================================================================
        // Xmin / Xmax / Ymin / Ymax
        //=================================================================
        public float Xmin
        {
            get
            {
                float xmin = float.MaxValue;
                foreach (PointF p in corners)
                    xmin = Math.Min(xmin, p.X);
                return xmin;
            }
        }

        public float Xmax
        {
            get
            {
                float xmax = float.MinValue;
                foreach (PointF p in corners)
                    xmax = Math.Max(xmax, p.X);
                return xmax;
            }
        }

        public float Ymin
        {
            get
            {
                float ymin = float.MaxValue;
                foreach (PointF p in corners)
                    ymin = Math.Min(ymin, p.Y);
                return ymin;
            }
        }

        public float Ymax
        {
            get
            {
                float ymax = float.MinValue;
                foreach (PointF p in corners)
                    ymax = Math.Max(ymax, p.Y);
                return ymax;
            }
        }

        //=================================================================
        // 
        //=================================================================
        [Browsable(false)]
        public PointF Barycenter
        {
            get
            {
                float x = 0;
                float y = 0;

                foreach (PointF p in corners)
                {
                    x += p.X;
                    y += p.Y;
                }

                x /= corners.Length;
                y /= corners.Length;

                return new PointF(x, y);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public RectangleF SurroundingRectangle { get { return RectangleF.FromLTRB(Xmin, Ymin, Xmax, Ymax); } }

        //=================================================================
        // 
        //=================================================================
        public eCompare IsInside(RectangleF r)
        {
            QuadF q = this;

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
            // Sinon, le quadrilatère est-il dans le rectangle ?
            //----------------------------------------------------------------------
            if (r.Contains(q.corners[0]))
                return eCompare.QuadIsInside;
            else if (r.TopLeft().IsInside(q))
                return eCompare.QuadIntersects; // en fait le rect est dedans
            else
                return eCompare.QuadIsOutside;
        }

        public eCompare IsInside(CircleF c)
        {
            QuadF q = this;
            VectorF[] sides = q.Sides;

            //----------------------------------------------------------------------
            // Est-ce qu'un coté intersecte le cercle ?
            //----------------------------------------------------------------------
            bool are_sides_inside = true;     // est-ce que les cotés du quad sont dans le cercle ?
            bool are_sides_outside = true;
            foreach (VectorF s in sides)
            {
                eCompare cmp = s.IsInside(c);
                if (cmp == eCompare.SegmentIntersects)
                    return eCompare.SegmentIntersects;
                are_sides_inside &= (cmp == eCompare.SegmentIsInside);
                are_sides_outside &= (cmp == eCompare.SegmentIsOutside);
            }

            //----------------------------------------------------------------------
            // Si pas d'intersection
            //----------------------------------------------------------------------
            if (are_sides_inside)
            {
                return eCompare.SegmentIsInside;
            }
            else if (are_sides_outside)
            {
                bool centerInside = c.center.IsInside(q);
                if (centerInside)
                    return eCompare.SegmentIntersects;
                else
                    return eCompare.SegmentIsOutside;
            }
            else
            {
                throw new ApplicationException("corners are neither inside nor outside");   // Sanity check
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Normalize()
        {
            if (!corners[2].IsLeft(Sides[0]))
            {
                PointF c1 = corners[1];
                corners[1] = corners[3];
                corners[3] = c1;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void IsCrossed()
        {
            throw new NotImplementedException("IsCrossed");
        }

        //=================================================================
        // 
        //=================================================================
        public bool IsConvex()
        {
            var sides = Sides;

            for (int i = 0; i < corners.Length; i++)
            {
                int p = (i + 2) % corners.Length;
                PointF c = corners[p];
                if (!c.IsLeft(sides[i]))
                    return false;
            }

            return true;
        }

        //=================================================================
        // 
        //=================================================================
        public double Area()
        {
            VectorF[] sides = Sides;
            VectorF diagonal = new VectorF(corners[0], corners[2]);

            double a1 = Heron(sides[0], sides[1], diagonal);    // Premier triangle
            double a2 = Heron(sides[2], sides[3], diagonal);    // Deuxième triangle

            return a1 + a2;
        }

        /// <summary>
        /// Formule d'Héron d'Alexandrie pour le calcul de la superficie d'un triangle.
        /// va,vb,vc sont les cotés.
        /// </summary>
        private double Heron(VectorF va, VectorF vb, VectorF vc)
        {
            double a2 = va.Length2;
            double b2 = vb.Length2;
            double c2 = vc.Length2;
            double a4 = a2 * a2;
            double b4 = b2 * b2;
            double c4 = c2 * c2;

            double area = 0.25 * Math.Sqrt(Math.Pow(a2 + b2 + c2, 2) - 2 * (a4 + b4 + c4));
            return area;
        }
    }
}
