using System;
using System.Xml.Serialization;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class RangeComparator : ComparatorBase
    {
        [XmlAttribute] private double min = Double.NegativeInfinity;
        [XmlAttribute] private double max = Double.PositiveInfinity;

        public double Min { get => min; set => min = value; }
        public double Max { get => max; set => max = value; }

        public override bool HasSameValue(object obj)
        {
            var comparator = obj as RangeComparator;
            return comparator != null &&
                   base.HasSameValue(obj) &&
                   Min == comparator.Min &&
                   Max == comparator.Max;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Test(object o)
        {
            double d = (double)o;
            return (Min <= d && d <= Max);
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            if (Min == Double.NegativeInfinity)
                return "<= " + Max;
            else if (Max == Double.PositiveInfinity)
                return ">= " + Min;
            else
                return "[ " + Min + " .. " + Max + " ]";
        }
    }

}
