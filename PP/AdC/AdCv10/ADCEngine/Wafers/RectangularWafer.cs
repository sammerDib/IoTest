using System.Drawing;
using System.Xml;

using AdcTools;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class RectangularWafer : WaferBase
    {
        private RectangleF _surroundingRectangle;
        public override RectangleF SurroundingRectangle { get { return _surroundingRectangle; } }
        public override RectangleF SurroundingRectangleWithFlats { get { return _surroundingRectangle; } }

        public float Width { get { return _surroundingRectangle.Width; } }
        public float Height { get { return _surroundingRectangle.Height; } }

        //=================================================================
        // 
        //=================================================================
        public override void Init(XmlNode waferNode)
        {
            float w = waferNode.GetFloatValue("Width");
            float h = waferNode.GetFloatValue("Height");
            Init(w, h);
        }

        public void Init(float w, float h)
        {
            float x = -w / 2;
            float y = -h / 2;

            _surroundingRectangle = new RectangleF(x, y, w, h);
        }

        //=================================================================
        // 
        //=================================================================
        public override XmlNode Save(XmlDocument xmldoc)
        {
            XmlNode waferNode = base.Save(xmldoc);

            waferNode.AppendValueElement("Width", SurroundingRectangle.Width);
            waferNode.AppendValueElement("Height", SurroundingRectangle.Height);

            return waferNode;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool IsPointInside(PointF p, double margin = 0)
        {
            RectangleF r = SurroundingRectangle;
            r.Inflate((float)-margin, (float)-margin);

            bool inside = r.Contains(p);
            return inside;
        }

        public override eCompare IsRectInside(RectangleF r, double margin = 0)
        {
            RectangleF sr = SurroundingRectangle;
            sr.Inflate((float)-margin, (float)-margin);

            var cmp = r.IsInside(sr);
            return cmp;
        }

        public override eCompare IsQuadInside(QuadF q, double margin = 0)
        {
            RectangleF r = SurroundingRectangle;
            r.Inflate((float)-margin, (float)-margin);

            var cmp = q.IsInside(r);
            return cmp;
        }

    }
}
