using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using AdcTools;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class FlatWafer : WaferBase
    {
        //=================================================================
        // Propriétés physique du Wafer
        //=================================================================
        public CircleF Circle { get; private set; }
        private double _diameter;
        public double Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
                Circle = new CircleF(new Point(0, 0), _diameter / 2);
            }
        }

        public VectorF FlatVectorVertical { get; private set; }
        private double _flatVerticalX;
        public double FlatVerticalX
        {
            get { return _flatVerticalX; }
            set
            {
                _flatVerticalX = value;
                ComputeVerticalFlatPosition();
            }
        }

        public VectorF FlatVectorHorizontal { get; private set; }
        private double _flatHorizontalY;
        public double FlatHorizontalY
        {
            get { return _flatHorizontalY; }
            set
            {
                _flatHorizontalY = value;
                ComputeHorizontalFlatPosition();
            }
        }

        public override RectangleF SurroundingRectangle { get { return Circle.SurroundingRectangle; } }

        public override RectangleF SurroundingRectangleWithFlats
        {
            get
            {
                double minX = -Circle.radius;
                double maxX = +Circle.radius;
                if (FlatVerticalX < 0)
                    minX = FlatVerticalX;
                else if (FlatVerticalX > 0)
                    maxX = FlatVerticalX;

                double minY = -Circle.radius;
                double maxY = +Circle.radius;
                if (FlatHorizontalY < 0)
                    minY = FlatHorizontalY;
                else if (FlatHorizontalY > 0)
                    maxY = FlatHorizontalY;

                return RectangleF.FromLTRB((float)minX, (float)minY, (float)maxX, (float)maxY);
            }
        }


        public bool IsDoubleFlat
        {
            get { return !double.IsNaN(FlatVerticalX) && !double.IsNaN(FlatHorizontalY); }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Init(XmlNode waferNode)
        {
            Diameter = waferNode.GetDoubleValue("Diameter");
            FlatVerticalX = waferNode.GetDoubleValue("FlatVerticalX");
            FlatHorizontalY = waferNode.GetDoubleValue("FlatHorizontalY");
        }

        //=================================================================
        // 
        //=================================================================
        public override XmlNode Save(XmlDocument xmldoc)
        {
            XmlNode waferNode = base.Save(xmldoc);

            waferNode.AppendValueElement("Diameter", Circle.Diameter);
            waferNode.AppendValueElement("FlatVerticalX", FlatVerticalX);
            waferNode.AppendValueElement("FlatHorizontalY", FlatHorizontalY);

            return waferNode;
        }

        //=================================================================
        // 
        //=================================================================
        protected void ComputeVerticalFlatPosition()
        {
            double radius = Circle.radius;

            if (Math.Abs(FlatVerticalX) >= radius)
                throw new ApplicationException("invalid wafer flats: X: " + FlatVerticalX + " Radius:" + radius);

            if (double.IsNaN(FlatVerticalX))
            {
                FlatVectorVertical = null;
            }
            else
            {
                double cos = FlatVerticalX / radius;
                double sin = Math.Sqrt(1 - cos * cos);  //cos²+sin²==1
                float x = (float)FlatVerticalX;
                float y = (float)(radius * sin);
                FlatVectorVertical = new VectorF(x, -y, x, +y);
            }
        }

        protected void ComputeHorizontalFlatPosition()
        {
            double radius = Circle.radius;

            if (Math.Abs(FlatHorizontalY) >= radius)
                throw new ApplicationException("invalid wafer flats: Y: " + FlatHorizontalY + " Radius:" + radius);

            // Horizontal Flat
            //................
            if (double.IsNaN(FlatHorizontalY))
            {
                FlatVectorHorizontal = null;
            }
            else
            {
                double cos = FlatHorizontalY / radius;
                double sin = Math.Sqrt(1 - cos * cos);  //cos²+sin²==1
                float x = (float)(radius * sin);
                float y = (float)FlatHorizontalY;
                FlatVectorHorizontal = new VectorF(-x, y, +x, y);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override bool IsPointInside(PointF p, double margin = 0)
        {
            // Test par rapport aux flats
            //...........................
            bool inside = IsPointInsideFlats(p, margin);
            if (!inside)
                return false;

            // Test par rapport au cercle
            //...........................
            CircleF c = new CircleF(Circle.center, (float)(Circle.radius - margin));
            inside = p.IsInside(c);

            return inside;
        }

        //=================================================================
        // 
        //=================================================================
        protected bool IsPointInsideFlats(PointF p, double margin = 0)
        {
            // Vertical Flat
            //..............
            if (!Double.IsNaN(FlatVerticalX))
            {
                if (FlatVerticalX < 0 && p.X < FlatVerticalX + margin)
                    return false;
                if (FlatVerticalX > 0 && FlatVerticalX - margin < p.X)
                    return false;
            }

            // Horizontal Flat
            //................
            if (!Double.IsNaN(FlatHorizontalY))
            {
                if (FlatHorizontalY < 0 && p.Y < FlatHorizontalY + margin)
                    return false;
                if (FlatHorizontalY > 0 && FlatHorizontalY - margin < p.Y)
                    return false;
            }

            return true;
        }

        //=================================================================
        // 
        //=================================================================
        public override eCompare IsRectInside(RectangleF r, double margin = 0)
        {
            QuadF q = r.ToQuadF();
            eCompare cmp = IsQuadInside(q, margin);
            return cmp;
        }

        //=================================================================
        // 
        //=================================================================
        public override eCompare IsQuadInside(QuadF q, double margin = 0)
        {
            //------------------------------------------------------------
            // Intersection des cotés du quadrilatère avec le pourtour du wafer
            //------------------------------------------------------------
            VectorF[] sides = q.Sides;

            foreach (VectorF side in sides)
            {
                if (VectorIntersect(side, margin))
                    return eCompare.QuadIntersects;
            }

            //------------------------------------------------------------
            // Pas d'intersection, test dedans/dehors
            //------------------------------------------------------------
            if (IsPointInside(q.corners[0], margin))
                return eCompare.QuadIsInside;
            else if (Circle.center.IsInside(q))
                return eCompare.QuadIntersects;
            else
                return eCompare.QuadIsOutside;
        }

        //=================================================================
        // Intersection Vecteur / pourtour du wafer
        //=================================================================
        protected bool VectorIntersect(VectorF v, double margin)
        {
            bool intersect = VectorIntersectCircle(v, margin);
            if (intersect)
                return true;

            intersect = VectorIntersectFlats(v, margin);
            if (intersect)
                return true;

            return false;
        }

        //=================================================================
        // Intersection du segment avec l'arc de cercle du wafer
        //=================================================================
        protected bool VectorIntersectCircle(VectorF v, double margin)
        {
            //------------------------------------------------------------
            // Intersection avec le cercle complet
            //------------------------------------------------------------
            CircleF circle = new CircleF(Circle.center, Circle.radius - margin);
            List<PointF> points = v.Intersect(circle);
            if (points == null || points.Count == 0)
                return false;

            //------------------------------------------------------------
            // Les points d'intersection sont-ils du bon coté des flats ?
            //------------------------------------------------------------
            foreach (PointF p in points)
            {
#if DEBUG
                VectorF test = new VectorF(circle.center, p);
                double d = test.Length;
                if (Math.Abs(d - circle.radius) > 1)
                    throw new ApplicationException("error in segment circle intersection");
#endif

                bool b = IsPointInsideFlats(p, margin);
                if (b)
                    return true;
            }

            return false;
        }

        //=================================================================
        // Intersection du segment avec les flats
        //=================================================================
        protected bool VectorIntersectFlats(VectorF v, double margin)
        {
            //------------------------------------------------------------
            // Vertical Flat
            //------------------------------------------------------------
            if (FlatVectorVertical != null)
            {
                // Prise en compte de la marge pour le flat
                //.........................................
                VectorF flatVectorVertical = new VectorF(FlatVectorVertical);
                if (FlatVerticalX < 0)
                {
                    flatVectorVertical.X0 += (float)margin;
                    flatVectorVertical.X1 += (float)margin;
                }
                else if (FlatVerticalX > 0)
                {
                    flatVectorVertical.X0 -= (float)margin;
                    flatVectorVertical.X1 -= (float)margin;
                }
                else
                {
                    throw new ApplicationException("Vertical flat: x==" + FlatVerticalX);
                }

                // On teste le vecteur par rapport au flat
                //........................................
                bool intersect = flatVectorVertical.Intersect(v);
                if (intersect)
                    return intersect;
            }

            //------------------------------------------------------------
            // Horizontal Flat
            //------------------------------------------------------------
            if (FlatVectorHorizontal != null)
            {
                // Prise en compte de la marge pour le flat
                //.........................................
                VectorF flatVectorHorizontal = new VectorF(FlatVectorHorizontal);
                if (FlatHorizontalY < 0)
                {
                    flatVectorHorizontal.Y0 += (float)margin;
                    flatVectorHorizontal.Y1 += (float)margin;
                }
                else if (FlatHorizontalY > 0)
                {
                    flatVectorHorizontal.Y0 -= (float)margin;
                    flatVectorHorizontal.Y1 -= (float)margin;
                }
                else
                {
                    throw new ApplicationException("Horizontal flat: y==" + FlatHorizontalY);
                }

                // On teste le vecteur par rapport au flat
                //........................................
                bool intersect = flatVectorHorizontal.Intersect(v);
                if (intersect)
                    return intersect;
            }

            return false;
        }


    }
}
