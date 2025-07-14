using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.MathLib.Geometry
{
    /// <summary>
    /// A shape composed of a list of points
    /// </summary>

    [Serializable]
    public class Shape
    {
        private List<PointF> lPoints;

        public List<PointF> Points
        {
            get { return lPoints; }
        }

        public Shape()
        {
            lPoints = new List<PointF>();
            ShapeColor = Color.Green;
        }

        public Shape(Shape shape)
        {
            lPoints = shape.Points;
            ShapeColor = shape.ShapeColor;
        }

        public void AddPoint(PointF point)
        {
            lPoints.Add(point);
        }

        [XmlIgnore]
        public Color ShapeColor { get; set; }

        [XmlElement("ShapeColor")]
        public int ShapeColorAsArgb
        {
            get { return ShapeColor.ToArgb(); }
            set { ShapeColor = Color.FromArgb(value); }
        }

        public void Draw(Graphics gGraphic, bool drawExtremity)
        {
            Pen penInter = new Pen((ShapeColor == null || ShapeColor.IsEmpty) ? Color.FromArgb(0x00, 0xcc, 0x00) : ShapeColor);
            PointF? prevPoint = null;
            foreach (var point in Points)
            {
                if (point != null)
                {
                    if (prevPoint.HasValue)
                    {
                        gGraphic.DrawLine(penInter, prevPoint.Value.X, prevPoint.Value.Y, point.X, point.Y);
                    }
                    prevPoint = point;
                }
            }

            if (drawExtremity)
            {
                UnitySC.PM.Shared.MathLib.Geometry.GeometryTools.DrawCross(gGraphic, Points[0], penInter);
                if (Points.Count > 1)
                    UnitySC.PM.Shared.MathLib.Geometry.GeometryTools.DrawCross(gGraphic, Points[Points.Count - 1], penInter);
            }
            penInter.Dispose();
        }
    }
}