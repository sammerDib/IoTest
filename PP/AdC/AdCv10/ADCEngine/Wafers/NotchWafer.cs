using System.Drawing;
using System.Xml;

using AdcTools;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class NotchWafer : WaferBase
    {
        //=================================================================
        // Propriétés physique du Wafer
        //=================================================================
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

        public double NotchSize;

        public CircleF Circle { get; set; } = new CircleF();
        public override RectangleF SurroundingRectangle { get { return Circle.SurroundingRectangle; } }
        public override RectangleF SurroundingRectangleWithFlats { get { return Circle.SurroundingRectangle; } }

        //=================================================================
        // 
        //=================================================================
        public override void Init(XmlNode waferNode)
        {
            Diameter = waferNode.GetDoubleValue("Diameter");
            NotchSize = waferNode.GetDoubleValue("NotchSize");
        }

        //=================================================================
        // 
        //=================================================================
        public override XmlNode Save(XmlDocument xmldoc)
        {
            XmlNode waferNode = base.Save(xmldoc);

            waferNode.AppendValueElement("Diameter", Circle.Diameter);
            waferNode.AppendValueElement("NotchSize", NotchSize);

            return waferNode;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool IsPointInside(PointF p, double margin = 0)
        {
            CircleF c;
            if (margin == 0)
                c = Circle;
            else
                c = new CircleF(Circle.center, (float)(Circle.radius - margin));

            bool inside = p.IsInside(c);
            return inside;
        }

        public override eCompare IsRectInside(RectangleF r, double margin = 0)
        {
            CircleF c;
            if (margin == 0)
                c = Circle;
            else
                c = new CircleF(Circle.center, (float)(Circle.radius - margin));

            var cmp = r.IsInside(c);
            return cmp;
        }

        public override eCompare IsQuadInside(QuadF q, double margin = 0)
        {
            CircleF c;
            if (margin == 0)
                c = Circle;
            else
                c = new CircleF(Circle.center, (float)(Circle.radius - margin));

            var cmp = q.IsInside(c);
            return cmp;
        }

    }
}
