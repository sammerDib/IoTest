using System;
using System.Collections.Generic;
using System.Drawing;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe représentant un vecteur, ou plutôt un segment orienté
    /// </summary>
    ///////////////////////////////////////////////////////////////////////

    public class VectorF
    {
        //=================================================================
        // Data
        //=================================================================
        public float X0;
        public float Y0;
        public float X1;
        public float Y1;

        //=================================================================
        // Constructeur
        //=================================================================
        public VectorF() { }

        public VectorF(VectorF v)
        {
            X0 = v.X0;
            Y0 = v.Y0;
            X1 = v.X1;
            Y1 = v.Y1;
        }

        public VectorF(float x0, float y0, float x1, float y1)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }
        public VectorF(PointF p1, PointF p2)
        {
            X0 = p1.X;
            Y0 = p1.Y;
            X1 = p2.X;
            Y1 = p2.Y;
        }

        public VectorF(PointF p, SizeF s)
        {
            X0 = p.X;
            Y0 = p.Y;
            X1 = p.X + s.Width;
            Y1 = p.Y + s.Height;
        }

        //=================================================================
        // Propriétés
        //=================================================================
        public float Width { get { return X1 - X0; } }
        public float Height { get { return Y1 - Y0; } }
        public double Length { get { return Math.Sqrt(Length2); } }
        public double Length2 { get { return Width * Width + Height * Height; } }
        public double Angle { get { return Math.Atan2(Height, Width); } }
        public SizeF Size { get { return new SizeF(X1 - X0, Y1 - Y0); } }

        public PointF Start { get { return new PointF(X0, Y0); } }
        public PointF End { get { return new PointF(X1, Y1); } }

        public float Left { get { return Math.Min(X0, X1); } }
        public float Top { get { return Math.Min(Y0, Y1); } }

        //=================================================================
        // Operateurs
        //=================================================================
        public static VectorF operator +(VectorF v, Size s)
        {
            VectorF res = new VectorF();
            res.X0 = v.X0;
            res.Y0 = v.Y0;
            res.X1 = v.X0 + s.Width;
            res.Y1 = v.Y0 + s.Height;
            return res;
        }

        public static VectorF operator *(VectorF v, float x)
        {
            float w = v.Width * x;
            float h = v.Height * x;

            VectorF res = new VectorF();
            res.X0 = v.X0;
            res.Y0 = v.Y0;
            res.X1 = v.X0 + w;
            res.Y1 = v.Y0 + h;
            return res;
        }

        //=================================================================
        // Produit V1xV2 et V1.V2
        //=================================================================
        public static double DotProduct(VectorF v1, VectorF v2)
        {
            double dot = v1.Width * v1.Width + v1.Height * v2.Height;
            return dot;
        }

        public static double CrossProduct(VectorF v1, VectorF v2)
        {
            double cross = v1.Width * v2.Height - v1.Height * v2.Width;
            return cross;
        }

        //=================================================================
        // 
        //=================================================================
        public VectorF Invert()
        {
            return new VectorF(End, Start);
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return "{" + Start.ToString() + "->" + End.ToString() + "}";
        }

        //=================================================================
        // 
        //=================================================================
        public eCompare IsInside(CircleF c)
        {
            //----------------------------------------------------------------------
            // Position des extrémitées par rapport au cercle
            //----------------------------------------------------------------------
            bool c1 = Start.IsInside(c);
            bool c2 = End.IsInside(c);
            if (c1 && c2)
                return eCompare.SegmentIsInside;
            else if ((c1 && !c2) || (c2 & !c1))
                return eCompare.SegmentIntersects;

            //----------------------------------------------------------------------
            // Les extrémitées du segment sont en dehors, mais le segment lui même 
            // peut intersecter
            //----------------------------------------------------------------------
            double dist = PointSizeExtension.Distance(this, c.center);
            if (dist < c.radius)
                return eCompare.SegmentIntersects;
            else
                return eCompare.SegmentIsOutside;
        }

        //=================================================================
        // Intersection des segments
        // Refer to http://stackoverflow.com/a/565282/292237
        //=================================================================
        public bool Intersect(VectorF v2)
        {
            VectorF v1 = this;
            bool b;

            //-------------------------------------------------------------
            // Produits croisés
            //-------------------------------------------------------------
            VectorF CmP = new VectorF(v1.Start, v2.Start);
            double CmPxV1 = CrossProduct(CmP, v1);
            double CmPxV2 = CrossProduct(CmP, v2);
            double V1xV2 = CrossProduct(v1, v2);

            //-------------------------------------------------------------
            // Segments colinéaires
            //-------------------------------------------------------------
            if (V1xV2.IsZeroEpsilon() && CmPxV1.IsZeroEpsilon())
            {
                // Intersection si les segments se recouvrent
                b = ((v2.X0 - v1.X0 < 0) != (v2.X0 - v1.X1 < 0)) ||
                    ((v2.Y0 - v1.Y0 < 0) != (v2.Y0 - v1.Y1 < 0));
                return b;
            }

            //-------------------------------------------------------------
            // Segments parallèles
            //-------------------------------------------------------------
            if (V1xV2.IsZeroEpsilon() && !CmPxV1.IsZeroEpsilon())
                return false;

            //-------------------------------------------------------------
            // Les lignes support ont une intersection
            //-------------------------------------------------------------
            // Les lignes se coupent au point v1.Start + t v1 = v2.Start + u v2.
            double t = CmPxV2 / V1xV2;
            double u = CmPxV1 / V1xV2;

            // L'intersection est-elle dans les segments ?
            b = (0 <= t && t <= 1) && (0 <= u && u <= 1);
            return b;
        }


        //=================================================================
        // Intersection segment / cercle
        //
        // On construit le segment AB, son équation paramétrique est :
        //    x = xA + t*dx     avec  dx = xB−xA
        //    y = yA + t*dx           dy = yB−yA
        // On construit le cercle CR de centre C et de rayon R, son équation paramétrique est :
        //    (x−xC)²+(y−yC)²−R²=0.
        //
        // On obtient donc le système suivant :
        //     x=xA+t* dx
        //     y=yA+t* dx
        //     (x−xC)² + (y−yC)² − R² = 0
        //	⇔ (t∗dx+xA−xC)² + (t∗dy+yA−yC)² − R² = 0
        //	⇔ dx²t²+dy²t² + 2t(dx(xA−xC)+dy(yA−yC)) + (xA−xC)²+(yA−yC)² − R² = 0
        //
        // On pose :
        //    α = dx²+dy²
        //	  β = 2(dx(xA−xC) + dy(yA−yC))
        //	  γ = (xA−xC)² + (yA−yC)² −R²
        // On résous l'équation du second degrés : αt²+βt+γ=0
        //	 Δ=β²−4αγ
        // si Δ≥0
        //	  t1 = (β−√Δ) /2α
        //    t2 = (β+√Δ) /2α
        //    Si t1 et t2 sont compris entre 0 et 1, on obtient les points d'intersection P1 et P2 :
        //        P1(xA+t1*dx, yA+t1*dy)
        //        P2(xA+t2*dx, yA+t2*dy)
        // si Δ = 0 P1 et P2 sont confondus.
        //=================================================================
        public List<PointF> Intersect(CircleF c)
        {
            VectorF seg = this;
            List<PointF> intersectsPoints = null;

            //------------------------------------------------------------
            // Calcul du polynôme de second degré
            //------------------------------------------------------------
            double dx = seg.X1 - seg.X0;
            double dy = seg.Y1 - seg.Y0;
            double Ox = seg.X0 - c.center.X;
            double Oy = seg.Y0 - c.center.Y;
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * Ox + dy * Oy);
            double C = Ox * Ox + Oy * Oy - c.radius * c.radius;

            //------------------------------------------------------------
            // Le segment est réduit à un point
            // NB: A=0 ⇒ B=0
            //------------------------------------------------------------
            if (A.IsZeroEpsilon())
            {
                if (C.IsZeroEpsilon())
                {
                    intersectsPoints = new List<PointF>();
                    intersectsPoints.Add(seg.Start);
                }
                return intersectsPoints;
            }

            //------------------------------------------------------------
            // Un solution
            //------------------------------------------------------------
            double delta = B * B - 4 * A * C;
            if (delta.IsZeroEpsilon())
            {
                intersectsPoints = new List<PointF>();
                double t = -B / (2 * A);
                if (t >= 0 && t <= 1)
                    intersectsPoints.Add(new PointF((float)(seg.X0 + t * dx), (float)(seg.Y0 + t * dy)));
            }

            //------------------------------------------------------------
            // Deux solutions
            //------------------------------------------------------------
            if (delta > 0)
            {
                intersectsPoints = new List<PointF>();
                double t1 = (-B - Math.Sqrt(delta)) / (2 * A);
                double t2 = (-B + Math.Sqrt(delta)) / (2 * A);
                if (t1 >= 0 && t1 <= 1)
                    intersectsPoints.Add(new PointF((float)(seg.X0 + t1 * dx), (float)(seg.Y0 + t1 * dy)));
                if (t2 >= 0 && t2 <= 1)
                    intersectsPoints.Add(new PointF((float)(seg.X0 + t2 * dx), (float)(seg.Y0 + t2 * dy)));
            }
            return intersectsPoints;
        }


    }
}
