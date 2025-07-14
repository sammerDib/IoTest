using System.Xml.Serialization;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class BooleanComparator : ComparatorBase
    {
        [XmlAttribute] public bool value;

        public override bool HasSameValue(object obj)
        {
            var comparator = obj as BooleanComparator;
            return comparator != null &&
                   base.HasSameValue(obj) &&
                   value == comparator.value;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Test(object o)
        {
            bool b = (bool)o;

            return (value == b);
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return "==" + value;
        }
    }
}
