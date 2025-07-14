using System.Drawing;
using System.Xml.Serialization;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class RectangleComparator : ComparatorBase
    {
        [XmlAttribute] public float MinX, MaxX, MinY, MaxY;

        public override bool HasSameValue(object obj)
        {
            var comparator = obj as RectangleComparator;
            return comparator != null &&
                   base.HasSameValue(obj) &&
                   MinX == comparator.MinX &&
                   MaxX == comparator.MaxX &&
                   MinY == comparator.MinY &&
                   MaxY == comparator.MaxY;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Test(object o)
        {
            RectangleF r = (RectangleF)o;

            bool match = true;
            match = match && (MinY <= r.Bottom) && (r.Top <= MaxY); //Attention, Top vers le bas !
            match = match && (MinX <= r.Left) && (r.Right <= MaxX);

            return match;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            string str = "[";

            bool hasX = !float.IsNegativeInfinity(MinX) || !float.IsNegativeInfinity(MaxX);
            bool hasY = !float.IsNegativeInfinity(MinY) || !float.IsNegativeInfinity(MaxY);

            if (hasX)
                str += NumberExtension.MinMaxString(MinX, MaxX, "x");
            if (hasX && hasY)
                str += " & ";
            if (hasY)
                str += NumberExtension.MinMaxString(MinY, MaxY, "y");

            str += "]";
            return str;
        }
    }

}
